/*
 * apilist.c
 */

#include "stdafx.h"

#include "mrdb.h"
#include "mrdberror.h"
#include "apilist.h"
#include <mruby/compile.h>
#include <mruby/irep.h>
#include <mruby/debug.h>

#define LINE_BUF_SIZE MAX_COMMAND_LINE

char *mrb_utf8_from_wchar(const wchar_t *wcsp, size_t wcssize);
wchar_t *mrb_wchar_from_utf8(const char *mbsp, size_t mbssize);

typedef struct source_file {
  wchar_t *path;
  uint16_t lineno;
  FILE *fp;
} source_file;

static void
source_file_free(mrb_state *mrb, source_file *file)
{
  if (file != NULL) {
    if (file->path != NULL) {
      mrb_free(mrb, file->path);
    }
    if (file->fp != NULL) {
      fclose(file->fp);
      file->fp = NULL;
    }
    mrb_free(mrb, file);
  }
}

static wchar_t*
build_path(mrb_state *mrb, const wchar_t *dir, const wchar_t *base)
{
  int len;
  wchar_t *path = NULL;

  len = wcslen(base) + 1;

  if (wcscmp(dir, L".")) {
    len += wcslen(dir) + sizeof(L"/") - 1;
  }

  path = (wchar_t *)mrb_malloc(mrb, len * sizeof(wchar_t));
  memset(path, 0, len * sizeof(wchar_t));

  if (wcscmp(dir, L".")) {
    wcscat_s(path, len, dir);
    wcscat_s(path, len, L"/");
  }
  wcscat_s(path, len, base);

  return path;
}

static wchar_t*
dirname(mrb_state *mrb, const wchar_t *path)
{
  size_t len;
  const wchar_t *p;
  wchar_t *dir;

  if (path == NULL) {
    return NULL;
  }

  p = wcsrchr(path, L'/');
  len = p != NULL ? (size_t)(p - path) : wcslen(path);

  dir = (wchar_t *)mrb_malloc(mrb, (len + 1) * sizeof(wchar_t));
  wcsncpy_s(dir, len + 1, path, len);
  dir[len] = L'\0';

  return dir;
}

static source_file*
source_file_new(mrb_state *mrb, mrb_debug_context *dbg, wchar_t *filename)
{
  source_file *file;

  file = (source_file *)mrb_malloc(mrb, sizeof(source_file));

  memset(file, L'\0', sizeof(source_file));
  _wfopen_s(&file->fp, filename, L"rb");

  if (file->fp == NULL) {
    source_file_free(mrb, file);
    return NULL;
  }

  file->lineno = 1;
  int len = wcslen(filename) + 1;
  file->path = (wchar_t *)mrb_malloc(mrb, len * sizeof(wchar_t));
  wcscpy_s(file->path, len, filename);
  return file;
}

static mrb_bool
remove_newlines(char *s, FILE *fp)
{
  int c;
  char *p;
  size_t len;

  if ((len = strlen(s)) == 0) {
    return FALSE;
  }

  p = s + len - 1;

  if (*p != '\r' && *p != '\n') {
    return FALSE;
  }

  if (*p == '\r') {
    /* peek the next character and skip '\n' */
    if ((c = fgetc(fp)) != '\n') {
      ungetc(c, fp);
    }
  }

  /* remove trailing newline characters */
  while (s <= p && (*p == '\r' || *p == '\n')) {
    *p-- = '\0';
  }

  return TRUE;
}

static void
show_lines(source_file *file, uint16_t line_min, uint16_t line_max)
{
  char buf[LINE_BUF_SIZE];
  int show_lineno = 1, found_newline = 0, is_printed = 0;

  if (file->fp == NULL) {
    return;
  }

  while (fgets(buf, sizeof(buf), file->fp) != NULL) {
    found_newline = remove_newlines(buf, file->fp);

    if (line_min <= file->lineno) {
      if (show_lineno) {
        fprintf(stdout, "%-8d", file->lineno);
      }
      show_lineno = found_newline;
      fprintf(stdout, found_newline ? "%s\n" : "%s", buf);
      is_printed = 1;
    }

    if (found_newline) {
      if (line_max < ++file->lineno) {
        break;
      }
    }
  }

  if (is_printed && !found_newline) {
    fprintf(stdout, "\n");
  }
}

wchar_t*
mrb_debug_get_source(mrb_state *mrb, mrdb_state *mrdb, const wchar_t *srcpath, const wchar_t *filename)
{
  int i;
  FILE *fp;
  const wchar_t *search_path[3];
  wchar_t *refname = mrb_wchar_from_utf8(mrb_debug_get_filename(mrdb->dbg->irep, 0), -1);
  wchar_t *path = NULL;
  const wchar_t *srcname = wcsrchr(filename, L'/');

  if (srcname) srcname++;
  else srcname = filename;

  search_path[0] = srcpath;
  search_path[1] = dirname(mrb, refname);
  search_path[2] = L".";

  for (i = 0; i < 3; i++) {
    if (search_path[i] == NULL) {
      continue;
    }

    if ((path = build_path(mrb, search_path[i], srcname)) == NULL) {
      continue;
    }

    if (_wfopen_s(&fp, path, L"rb") != 0) {
      mrb_free(mrb, path);
      path = NULL;
      continue;
    }
    fclose(fp);
    break;
  }

  mrb_utf8_free(refname);
  mrb_free(mrb, (void *)search_path[1]);

  return path;
}

int32_t
mrb_debug_list(mrb_state *mrb, mrb_debug_context *dbg, wchar_t *filename, uint16_t line_min, uint16_t line_max)
{
  wchar_t *ext;
  source_file *file;

  if (mrb == NULL || dbg == NULL || filename == NULL) {
    return MRB_DEBUG_INVALID_ARGUMENT;
  }

  ext = wcsrchr(filename, L'.');

  if (ext == NULL || wcscmp(ext, L".rb")) {
    fprintf(stdout, "List command only supports .rb file.\n");
    return MRB_DEBUG_INVALID_ARGUMENT;
  }

  if (line_min > line_max) {
    return MRB_DEBUG_INVALID_ARGUMENT;
  }

  if ((file = source_file_new(mrb, dbg, filename)) != NULL) {
    show_lines(file, line_min, line_max);
    source_file_free(mrb, file);
    return MRB_DEBUG_OK;
  }
  else {
    fprintf(stdout, "Invalid source file named %ls.\n", filename);
    return MRB_DEBUG_INVALID_ARGUMENT;
  }
}
