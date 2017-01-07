// stdafx.h : 標準のシステム インクルード ファイルのインクルード ファイル、または
// 参照回数が多く、かつあまり変更されない、プロジェクト専用のインクルード ファイル
// を記述します。
//

#pragma once

#include "targetver.h"

#define WIN32_LEAN_AND_MEAN             // Windows ヘッダーから使用されていない部分を除外します。
// Windows ヘッダー ファイル:
#include <windows.h>

// TODO: プログラムに必要な追加ヘッダーをここで参照してください
#include <ctype.h>
#include <io.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <tchar.h>

void clearerr_rd(FILE* stream);
int feof_rd(FILE* stream);
int fflush_rd(FILE* stream);
int fprintf_rd(FILE* const stream, char const* const format, ...);
int fputs_rd(char const* buffer, FILE* stream);
size_t fwrite_rd(void const* buffer, size_t elementSize, size_t elementCount, FILE* stream);
int getc_rd(FILE* stream);
int putc_rd(int character, FILE* stream);

#define clearerr(stream)				clearerr_rd(stream)
#define feof(stream)					feof_rd(stream)
#define fflush(stream)					fflush_rd(stream)
#define fprintf(stream, format, ...)	fprintf_rd(stream, format, __VA_ARGS__)
#define fputs(buffer, stream)			fputs_rd(buffer, stream)
#define fwrite(buffer, elementSize, elementCount, stream)	fwrite_rd(buffer, elementSize, elementCount, stream)
#define getc(stream)					getc_rd(stream)
#define putc(character, stream)			putc_rd(character, stream)
