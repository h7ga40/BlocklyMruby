// dllmain.cpp : DLL アプリケーションのエントリ ポイントを定義します。
#include "stdafx.h"

BOOL APIENTRY DllMain(HMODULE hModule,
	DWORD  ul_reason_for_call,
	LPVOID lpReserved
)
{
	switch (ul_reason_for_call) {
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}

typedef void __stdcall clearerr_t(int fno);
typedef int __stdcall feof_t(int fno);
typedef int __stdcall fflush_t(int fno);
typedef size_t __stdcall fwrite_t(void const* buffer, size_t len, int fno);
typedef int __stdcall getc_t(int fno);
typedef void __stdcall abort_t();

clearerr_t *clearerr_ptr;
feof_t *feof_ptr;
fflush_t *fflush_ptr;
fwrite_t *fwrite_ptr;
getc_t *getc_ptr;
abort_t *abort_ptr;

__declspec(dllexport) void __stdcall set_func(clearerr_t *pclearerr, feof_t *pfeof, getc_t *pgetc, fwrite_t *pfwrite, fflush_t *pfflush, abort_t *pabort)
{
	clearerr_ptr = pclearerr;
	feof_ptr = pfeof;
	getc_ptr = pgetc;
	fwrite_ptr = pfwrite;
	fflush_ptr = pfflush;
	abort_ptr = pabort;
}

__declspec(dllexport) void __stdcall clear_func()
{
	clearerr_ptr = NULL;
	feof_ptr = NULL;
	getc_ptr = NULL;
	fwrite_ptr = NULL;
	fflush_ptr = NULL;
}

#undef clearerr
#undef feof
#undef fflush
#undef fprintf
#undef fputs
#undef fwrite
#undef getc
#undef putc

int get_fno(FILE* stream)
{
	if (clearerr_ptr == NULL)
		return -1;
	if (stream == stdin)
		return 0;
	if (stream == stdout)
		return 1;
	if (stream == stderr)
		return 2;
	return -1;
}

void clearerr_rd(FILE* stream)
{
	int fno = get_fno(stream);
	if (fno >= 0)
		clearerr_ptr(fno);
	clearerr(stream);
}

int feof_rd(FILE* stream)
{
	int fno = get_fno(stream);
	if (fno >= 0)
		return feof_ptr(fno);
	return feof(stream);
}

int fflush_rd(FILE* stream)
{
	int fno = get_fno(stream);
	if (fno >= 0)
		return fflush_ptr(fno);
	return fflush(stream);
}

int fprintf_rd(FILE* const stream, char const* const format, ...)
{
	int len;
	va_list args;
	va_start(args, format);

	int fno = get_fno(stream);
	if (fno >= 0) {
		len = _vscprintf_p(format, args) + 1;
		char *buffer = (char *)malloc(len * sizeof(char));
		if (buffer) {
			_vsprintf_p(buffer, len, format, args);
			fwrite_ptr(buffer, len, fno);
			free(buffer);
		}
	}
	else {
		len = vfprintf(stream, format, args);
	}

	va_end(args);
	return len;
}

int fputs_rd(char const* buffer, FILE* stream)
{
	int fno = get_fno(stream);
	if (fno >= 0)
		return fwrite_ptr(buffer, strlen(buffer), fno);
	return fputs(buffer, stream);
}

size_t fwrite_rd(void const* buffer, size_t elementSize, size_t elementCount, FILE* stream)
{
	int fno = get_fno(stream);
	if (fno >= 0)
		return fwrite_ptr(buffer, elementSize * elementCount, fno);
	return fwrite(buffer, elementSize, elementCount, stream);
}

int getc_rd(FILE* stream)
{
	int fno = get_fno(stream);
	if (fno >= 0) {
		int ret = getc_ptr(fno);
		if (ret > 0) {
			char ch = ret;
			fwrite_ptr(&ch, 1, fno);
		}
		return ret;
	}
	return getc(stream);
}

int putc_rd(int character, FILE* stream)
{
	char ch = character;
	int fno = get_fno(stream);
	if (fno >= 0)
		return fwrite_ptr(&ch, 1, fno);
	return putc(character, stream);
}

void mrdb_abort()
{
	if (abort_ptr != NULL)
		abort_ptr();
	abort();
}
