#include "stdafx.h"

#include "mruby.h"
#include "mruby/compile.h"
#include "mruby/dump.h"
#include "mruby/proc.h"

#define RITEBIN_EXT L".mrb"
#define C_EXT       L".c"

struct mrbc_args
{
	int argc;
	wchar_t **argv;
	int idx;
	const wchar_t *prog;
	const wchar_t *outfile;
	const wchar_t *initname;
	mrb_bool check_syntax : 1;
	mrb_bool verbose : 1;
	unsigned int flags : 4;
	char *fn;
	char *initname_utf8;
};

char *mrb_utf8_from_wchar(const wchar_t *wcsp, size_t wcssize);

static void
usage(const wchar_t *name)
{
	static const char *const usage_msg[] = {
		"switches:",
		"-c           check syntax only",
		"-o<outfile>  place the output into <outfile>",
		"-v           print version number, then turn on verbose mode",
		"-g           produce debugging information",
		"-B<symbol>   binary <symbol> output in C language format",
		"-e           generate little endian iseq data",
		"-E           generate big endian iseq data",
		"--verbose    run at verbose mode",
		"--version    print the version",
		"--copyright  print the copyright",
		NULL
	};
	const char *const *p = usage_msg;

	fprintf(stdout, "Usage: %ls [switches] programfile\n", name);
	while (*p)
		fprintf(stdout, "  %s\n", *p++);
}

static wchar_t *
get_outfilename(mrb_state *mrb, wchar_t *infile, const wchar_t *ext)
{
	size_t infilelen;
	size_t extlen;
	wchar_t *outfile;
	wchar_t *p;

	infilelen = wcslen(infile);
	extlen = wcslen(ext);
	outfile = (wchar_t*)mrb_malloc(mrb, (infilelen + extlen + 1) * sizeof(wchar_t));
	memcpy(outfile, infile, (infilelen + 1) * sizeof(wchar_t));
	if (*ext) {
		if ((p = wcsrchr(outfile, L'.')) == NULL)
			p = outfile + infilelen;
		memcpy(p, ext, (extlen + 1) * sizeof(wchar_t));
	}

	return outfile;
}

static int
parse_args(mrb_state *mrb, int argc, wchar_t **argv, struct mrbc_args *args)
{
	static const struct mrbc_args args_zero = { 0 };
	int i;

	*args = args_zero;
	args->argc = argc;
	args->argv = argv;
	args->prog = argv[0];

	for (i = 1; i < argc; i++) {
		if (argv[i][0] == L'-') {
			switch ((argv[i])[1]) {
			case L'o':
				if (args->outfile) {
					fprintf(stderr, "%ls: an output file is already specified. (%ls)\n",
						args->prog, args->outfile);
					return -1;
				}
				if (argv[i][2] == L'\0' && argv[i + 1]) {
					i++;
					args->outfile = get_outfilename(mrb, argv[i], L"");
				}
				else {
					args->outfile = get_outfilename(mrb, argv[i] + 2, L"");
				}
				break;
			case L'B':
				if (argv[i][2] == '\0' && argv[i + 1]) {
					i++;
					args->initname = argv[i];
				}
				else {
					args->initname = argv[i] + 2;
				}
				if (*args->initname == L'\0') {
					fprintf(stderr, "%ls: function name is not specified.\n", args->prog);
					return -1;
				}
				break;
			case L'c':
				args->check_syntax = TRUE;
				break;
			case L'v':
				if (!args->verbose) mrb_show_version(mrb);
				args->verbose = TRUE;
				break;
			case L'g':
				args->flags |= DUMP_DEBUG_INFO;
				break;
			case L'E':
				args->flags = DUMP_ENDIAN_BIG | (args->flags & ~DUMP_ENDIAN_MASK);
				break;
			case L'e':
				args->flags = DUMP_ENDIAN_LIL | (args->flags & ~DUMP_ENDIAN_MASK);
				break;
			case L'h':
				return -1;
			case L'-':
				if (argv[i][1] == L'\n') {
					return i;
				}
				if (wcscmp(argv[i] + 2, L"version") == 0) {
					mrb_show_version(mrb);
					exit(EXIT_SUCCESS);
				}
				else if (wcscmp(argv[i] + 2, L"verbose") == 0) {
					args->verbose = TRUE;
					break;
				}
				else if (wcscmp(argv[i] + 2, L"copyright") == 0) {
					mrb_show_copyright(mrb);
					exit(EXIT_SUCCESS);
				}
				return -1;
			default:
				return i;
			}
		}
		else {
			break;
		}
	}
	if (args->verbose && args->initname && (args->flags & DUMP_ENDIAN_MASK) == 0) {
		fprintf(stderr, "%ls: generating %s endian C file. specify -e/-E for cross compiling.\n",
			args->prog, bigendian_p() ? "big" : "little");
	}
	return i;
}

static void
cleanup(mrb_state *mrb, struct mrbc_args *args)
{
	mrb_free(mrb, (void*)args->outfile);
	mrb_close(mrb);
}

static int
partial_hook(struct mrb_parser_state *p)
{
	mrbc_context *c = p->cxt;
	struct mrbc_args *args = (struct mrbc_args *)c->partial_data;
	const wchar_t *fn;

	if (p->f) fclose(p->f);
	if (args->idx >= args->argc) {
		p->f = NULL;
		return -1;
	}
	fn = args->argv[args->idx++];
	_wfopen_s(&p->f, fn, L"r");
	if (p->f == NULL) {
		fprintf(stderr, "%ls: cannot open program file. (%ls)\n", args->prog, fn);
		return -1;
	}
	args->fn = mrb_utf8_from_wchar(fn, -1);
	mrb_parser_set_filename(p, args->fn);
	return 0;
}

static mrb_value
load_file(mrb_state *mrb, struct mrbc_args *args)
{
	mrbc_context *c;
	mrb_value result;
	wchar_t *input = args->argv[args->idx];
	FILE *infile;
	mrb_bool need_close = FALSE;

	c = mrbc_context_new(mrb);
	if (args->verbose)
		c->dump_result = TRUE;
	c->no_exec = TRUE;
	if (input[0] == L'-' && input[1] == L'\0') {
		infile = stdin;
	}
	else {
		need_close = TRUE;
		if (_wfopen_s(&infile, input, L"r") != 0) {
			fprintf(stderr, "%ls: cannot open program file. (%ls)\n", args->prog, input);
			return mrb_nil_value();
		}
	}
	mrbc_filename(mrb, c, mrb_utf8_from_wchar(input, -1));
	args->idx++;
	if (args->idx < args->argc) {
		need_close = FALSE;
		mrbc_partial_hook(mrb, c, partial_hook, (void*)args);
	}

	result = mrb_load_file_cxt(mrb, infile, c);
	if (need_close) fclose(infile);
	mrbc_context_free(mrb, c);
	if (mrb_undef_p(result)) {
		return mrb_nil_value();
	}
	return result;
}

static int
dump_file(mrb_state *mrb, FILE *wfp, const wchar_t *outfile, struct RProc *proc, struct mrbc_args *args)
{
	int n = MRB_DUMP_OK;
	mrb_irep *irep = proc->body.irep;

	if (args->initname) {
		args->initname_utf8 = mrb_utf8_from_wchar(args->initname, -1);
		n = mrb_dump_irep_cfunc(mrb, irep, args->flags, wfp, args->initname_utf8);
		if (n == MRB_DUMP_INVALID_ARGUMENT) {
			fprintf(stderr, "%ls: invalid C language symbol name\n", args->initname);
		}
	}
	else {
		n = mrb_dump_irep_binary(mrb, irep, args->flags, wfp);
	}
	if (n != MRB_DUMP_OK) {
		fprintf(stderr, "%ls: error in mrb dump (%ls) %d\n", args->prog, outfile, n);
	}
	return n;
}

__declspec(dllexport) int __stdcall
mrbc_main(int argc, wchar_t **argv)
{
	mrb_state *mrb = mrb_open();
	int n, result;
	struct mrbc_args args;
	FILE *wfp;
	mrb_value load;

	if (mrb == NULL) {
		fputs("Invalid mrb_state, exiting mrbc\n", stderr);
		return EXIT_FAILURE;
	}

	n = parse_args(mrb, argc, argv, &args);
	if (n < 0) {
		cleanup(mrb, &args);
		usage(argv[0]);
		return EXIT_FAILURE;
	}
	if (n == argc) {
		fprintf(stderr, "%ls: no program file given\n", args.prog);
		return EXIT_FAILURE;
	}
	if (args.outfile == NULL && !args.check_syntax) {
		if (n + 1 == argc) {
			args.outfile = get_outfilename(mrb, argv[n], args.initname ? C_EXT : RITEBIN_EXT);
		}
		else {
			fprintf(stderr, "%ls: output file should be specified to compile multiple files\n", args.prog);
			return EXIT_FAILURE;
		}
	}

	args.idx = n;
	load = load_file(mrb, &args);
	if (mrb_nil_p(load)) {
		cleanup(mrb, &args);
		return EXIT_FAILURE;
	}
	if (args.check_syntax) {
		fprintf(stdout, "%ls:%ls:Syntax OK\n", args.prog, argv[n]);
	}

	if (args.check_syntax) {
		cleanup(mrb, &args);
		return EXIT_SUCCESS;
	}

	if (args.outfile) {
		if (wcscmp(L"-", args.outfile) == 0) {
			wfp = stdout;
		}
		else if ((_wfopen_s(&wfp, args.outfile, L"wb")) != 0) {
			fprintf(stderr, "%ls: cannot open output file:(%ls)\n", args.prog, args.outfile);
			return EXIT_FAILURE;
		}
	}
	else {
		fprintf(stderr, "Output file is required\n");
		return EXIT_FAILURE;
	}
	result = dump_file(mrb, wfp, args.outfile, mrb_proc_ptr(load), &args);
	fclose(wfp);
	cleanup(mrb, &args);
	if (result != MRB_DUMP_OK) {
		return EXIT_FAILURE;
	}
	return EXIT_SUCCESS;
}

void
mrb_init_mrblib(mrb_state *mrb)
{
}

#ifndef DISABLE_GEMS
void
mrb_init_mrbgems(mrb_state *mrb)
{
}

void
mrb_final_mrbgems(mrb_state *mrb)
{
}
#endif
