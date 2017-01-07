#include "stdafx.h"

#include "mruby.h"
#include "mruby/array.h"
#include "mruby/compile.h"
#include "mruby/dump.h"
#include "mruby/variable.h"

#ifdef MRB_DISABLE_STDIO
static void
p(mrb_state *mrb, mrb_value obj)
{
	mrb_value val = mrb_inspect(mrb, obj);

	fwrite(RSTRING_PTR(val), RSTRING_LEN(val), 1, stdout);
	putc('\n', stdout);
}
#else
#define p(mrb,obj) mrb_p(mrb,obj)
#endif

struct _args
{
	FILE *rfp;
	wchar_t* cmdline;
	mrb_bool fname : 1;
	mrb_bool mrbfile : 1;
	mrb_bool check_syntax : 1;
	mrb_bool verbose : 1;
	int argc;
	wchar_t** argv;
};

static void
usage(const wchar_t *name)
{
	static const char *const usage_msg[] = {
		"switches:",
		"-b           load and execute RiteBinary (mrb) file",
		"-c           check syntax only",
		"-e 'command' one line of script",
		"-v           print version number, then run in verbose mode",
		"--verbose    run in verbose mode",
		"--version    print the version",
		"--copyright  print the copyright",
		NULL
	};
	const char *const *p = usage_msg;

	fprintf(stdout, "Usage: %ls [switches] programfile\n", name);
	while (*p)
		fprintf(stdout, "  %s\n", *p++);
}

static int
parse_args(mrb_state *mrb, int argc, wchar_t **argv, struct _args *args)
{
	wchar_t **origargv = argv;
	static const struct _args args_zero = { 0 };

	*args = args_zero;

	for (argc--, argv++; argc > 0; argc--, argv++) {
		wchar_t *item;
		if (argv[0][0] != '-') break;

		if (wcslen(*argv) <= 1) {
			argc--; argv++;
			args->rfp = stdin;
			break;
		}

		item = argv[0] + 1;
		switch (*item++) {
		case L'b':
			args->mrbfile = TRUE;
			break;
		case L'c':
			args->check_syntax = TRUE;
			break;
		case L'e':
			if (item[0]) {
				goto append_cmdline;
			}
			else if (argc > 1) {
				argc--; argv++;
				item = argv[0];
			append_cmdline:
				if (!args->cmdline) {
					size_t buflen;
					wchar_t *buf;

					buflen = (wcslen(item) + 1) * sizeof(wchar_t);
					buf = (wchar_t *)mrb_malloc(mrb, buflen);
					memcpy(buf, item, buflen);
					args->cmdline = buf;
				}
				else {
					size_t cmdlinelen;
					size_t itemlen;

					cmdlinelen = wcslen(args->cmdline);
					itemlen = wcslen(item);
					args->cmdline =
						(wchar_t *)mrb_realloc(mrb, args->cmdline, (cmdlinelen + itemlen + 2) * sizeof(wchar_t));
					args->cmdline[cmdlinelen] = L'\n';
					memcpy(args->cmdline + cmdlinelen + 1, item, (itemlen + 1) * sizeof(wchar_t));
				}
			}
			else {
				fprintf(stdout, "%ls: No code specified for -e\n", *origargv);
				return EXIT_SUCCESS;
			}
			break;
		case L'v':
			if (!args->verbose) mrb_show_version(mrb);
			args->verbose = TRUE;
			break;
		case L'-':
			if (wcscmp((*argv) + 2, L"version") == 0) {
				mrb_show_version(mrb);
				exit(EXIT_SUCCESS);
			}
			else if (wcscmp((*argv) + 2, L"verbose") == 0) {
				args->verbose = TRUE;
				break;
			}
			else if (wcscmp((*argv) + 2, L"copyright") == 0) {
				mrb_show_copyright(mrb);
				exit(EXIT_SUCCESS);
			}
		default:
			return EXIT_FAILURE;
		}
	}

	if (args->rfp == NULL && args->cmdline == NULL) {
		if (*argv == NULL) args->rfp = stdin;
		else {
			_wfopen_s(&args->rfp, argv[0], args->mrbfile ? L"rb" : L"r");
			if (args->rfp == NULL) {
				fprintf(stdout, "%ls: Cannot open program file. (%ls)\n", *origargv, *argv);
				return EXIT_FAILURE;
			}
			args->fname = TRUE;
			args->cmdline = argv[0];
			argc--; argv++;
		}
	}
	args->argv = (wchar_t **)mrb_realloc(mrb, args->argv, sizeof(wchar_t*) * (argc + 1));
	memcpy(args->argv, argv, (argc + 1) * sizeof(wchar_t*));
	args->argc = argc;

	return EXIT_SUCCESS;
}

static void
cleanup(mrb_state *mrb, struct _args *args)
{
	if (args->rfp && args->rfp != stdin)
		fclose(args->rfp);
	if (!args->fname)
		mrb_free(mrb, args->cmdline);
	mrb_free(mrb, args->argv);
	mrb_close(mrb);
}

char*
mrb_utf8_from_wchar(const wchar_t *wcsp, size_t wcssize)
{
	char* mbsp;
	size_t mbssize;

	if (wcssize == 0)
		return _strdup("");
	if (wcssize == -1)
		wcssize = wcslen(wcsp);

	mbssize = WideCharToMultiByte(CP_UTF8, 0, (LPCWSTR)wcsp, -1, NULL, 0, NULL, NULL);
	mbsp = (char*)malloc((mbssize + 1));
	if (!mbsp)
		return NULL;

	mbssize = WideCharToMultiByte(CP_UTF8, 0, (LPCWSTR)wcsp, -1, mbsp, mbssize, NULL, NULL);
	mbsp[mbssize] = 0;
	return mbsp;
}

__declspec(dllexport) int _stdcall
mruby_main(int argc, wchar_t **argv)
{
	mrb_state *mrb = mrb_open();
	int n = -1;
	int i;
	struct _args args;
	mrb_value ARGV;
	mrbc_context *c;
	mrb_value v;
	mrb_sym zero_sym;

	if (mrb == NULL) {
		fputs("Invalid mrb_state, exiting mruby\n", stderr);
		return EXIT_FAILURE;
	}

	n = parse_args(mrb, argc, argv, &args);
	if (n == EXIT_FAILURE || (args.cmdline == NULL && args.rfp == NULL)) {
		cleanup(mrb, &args);
		usage(argv[0]);
		return n;
	}

	ARGV = mrb_ary_new_capa(mrb, args.argc);
	for (i = 0; i < args.argc; i++) {
		char* utf8 = mrb_utf8_from_wchar(args.argv[i], -1);
		if (utf8) {
			mrb_ary_push(mrb, ARGV, mrb_str_new_cstr(mrb, utf8));
			mrb_utf8_free(utf8);
		}
	}
	mrb_define_global_const(mrb, "ARGV", ARGV);

	c = mrbc_context_new(mrb);
	if (args.verbose)
		c->dump_result = TRUE;
	if (args.check_syntax)
		c->no_exec = TRUE;

	/* Set $0 */
	zero_sym = mrb_intern_lit(mrb, "$0");
	if (args.rfp) {
		char *utf8 = NULL;
		const char *cmdline;
		if (args.cmdline != NULL)
			cmdline = utf8 = mrb_utf8_from_wchar(args.cmdline, -1);
		else
			cmdline = "-";
		mrbc_filename(mrb, c, cmdline);
		mrb_gv_set(mrb, zero_sym, mrb_str_new_cstr(mrb, cmdline));
		if (utf8 != NULL)
			mrb_utf8_free(utf8);
	}
	else {
		mrbc_filename(mrb, c, "-e");
		mrb_gv_set(mrb, zero_sym, mrb_str_new_lit(mrb, "-e"));
	}

	/* Load program */
	if (args.mrbfile) {
		v = mrb_load_irep_file_cxt(mrb, args.rfp, c);
	}
	else if (args.rfp) {
		v = mrb_load_file_cxt(mrb, args.rfp, c);
	}
	else {
		char* utf8 = mrb_utf8_from_wchar(args.cmdline, -1);
		if (!utf8) abort();
		v = mrb_load_string_cxt(mrb, utf8, c);
		mrb_utf8_free(utf8);
	}

	mrbc_context_free(mrb, c);
	if (mrb->exc) {
		if (!mrb_undef_p(v)) {
			mrb_print_error(mrb);
		}
		n = -1;
	}
	else if (args.check_syntax) {
		fprintf(stdout, "Syntax OK\n");
	}
	cleanup(mrb, &args);

	return n == 0 ? EXIT_SUCCESS : EXIT_FAILURE;
}
