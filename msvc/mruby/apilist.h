/*
 * apilist.h
 */

#ifndef APILIST_H_
#define APILIST_H_

#include <mruby.h>
#include "mrdb.h"

int32_t mrb_debug_list(mrb_state *, mrb_debug_context *, wchar_t *, uint16_t, uint16_t);
wchar_t* mrb_debug_get_source(mrb_state *, mrdb_state *, const wchar_t *, const wchar_t *);

#endif /* APILIST_H_ */
