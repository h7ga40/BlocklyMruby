using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bridge.Html5;

namespace BlocklyMruby
{
	public enum node_type
	{
		NODE_METHOD = 1,
		NODE_FBODY,
		NODE_CFUNC,
		NODE_SCOPE,
		NODE_BLOCK,
		NODE_IF,
		NODE_CASE,
		NODE_WHEN,
		NODE_OPT_N,
		NODE_WHILE,
		NODE_UNTIL,
		NODE_ITER,
		NODE_FOR,
		NODE_BREAK,
		NODE_NEXT,
		NODE_REDO,
		NODE_RETRY,
		NODE_BEGIN,
		NODE_RESCUE,
		NODE_ENSURE,
		NODE_AND,
		NODE_OR,
		NODE_NOT,
		NODE_MASGN,
		NODE_ASGN,
		NODE_CDECL,
		NODE_CVASGN,
		NODE_CVDECL,
		NODE_OP_ASGN,
		NODE_CALL,
		NODE_SCALL,
		NODE_FCALL,
		NODE_VCALL,
		NODE_SUPER,
		NODE_ZSUPER,
		NODE_ARRAY,
		NODE_ZARRAY,
		NODE_HASH,
		NODE_RETURN,
		NODE_YIELD,
		NODE_LVAR,
		NODE_DVAR,
		NODE_GVAR,
		NODE_IVAR,
		NODE_CONST,
		NODE_CVAR,
		NODE_NTH_REF,
		NODE_BACK_REF,
		NODE_MATCH,
		NODE_MATCH2,
		NODE_MATCH3,
		NODE_INT,
		NODE_FLOAT,
		NODE_NEGATE,
		NODE_LAMBDA,
		NODE_SYM,
		NODE_STR,
		NODE_DSTR,
		NODE_XSTR,
		NODE_DXSTR,
		NODE_REGX,
		NODE_DREGX,
		NODE_DREGX_ONCE,
		NODE_LIST,
		NODE_ARG,
		NODE_ARGSCAT,
		NODE_ARGSPUSH,
		NODE_SPLAT,
		NODE_TO_ARY,
		NODE_SVALUE,
		NODE_BLOCK_ARG,
		NODE_DEF,
		NODE_SDEF,
		NODE_ALIAS,
		NODE_UNDEF,
		NODE_CLASS,
		NODE_MODULE,
		NODE_SCLASS,
		NODE_COLON2,
		NODE_COLON3,
		NODE_CREF,
		NODE_DOT2,
		NODE_DOT3,
		NODE_FLIP2,
		NODE_FLIP3,
		NODE_ATTRSET,
		NODE_SELF,
		NODE_NIL,
		NODE_TRUE,
		NODE_FALSE,
		NODE_DEFINED,
		NODE_NEWLINE,
		NODE_POSTEXE,
		NODE_ALLOCA,
		NODE_DMETHOD,
		NODE_BMETHOD,
		NODE_MEMO,
		NODE_IFUNC,
		NODE_DSYM,
		NODE_ATTRASGN,
		NODE_HEREDOC,
		NODE_LITERAL_DELIM,
		NODE_WORDS,
		NODE_SYMBOLS,
		NODE_LAST
	}

	/* lexer states */
	public enum mrb_lex_state_enum
	{
		EXPR_BEG,                   /* ignore newline, +/- is a sign. */
		EXPR_END,                   /* newline significant, +/- is an operator. */
		EXPR_ENDARG,                /* ditto, and unbound braces. */
		EXPR_ENDFN,                 /* ditto, and unbound braces. */
		EXPR_ARG,                   /* newline significant, +/- is an operator. */
		EXPR_CMDARG,                /* newline significant, +/- is an operator. */
		EXPR_MID,                   /* newline significant, +/- is an operator. */
		EXPR_FNAME,                 /* ignore newline, no reserved words. */
		EXPR_DOT,                   /* right after '.' or '::', no reserved words. */
		EXPR_CLASS,                 /* immediate after 'class', no here document. */
		EXPR_VALUE,                 /* alike EXPR_BEG but label is disallowed. */
		EXPR_MAX_STATE
	}

	public enum mrb_sym { }

	public enum stack_type { }

	[Flags]
	enum mrb_string_type
	{
		STR_FUNC_PARSING = 0x01,
		STR_FUNC_EXPAND = 0x02,
		STR_FUNC_REGEXP = 0x04,
		STR_FUNC_WORD = 0x08,
		STR_FUNC_SYMBOL = 0x10,
		STR_FUNC_ARRAY = 0x20,
		STR_FUNC_HEREDOC = 0x40,
		STR_FUNC_XQUOTE = 0x80,

		str_not_parsing = (0),
		str_squote = (STR_FUNC_PARSING),
		str_dquote = (STR_FUNC_PARSING | STR_FUNC_EXPAND),
		str_regexp = (STR_FUNC_PARSING | STR_FUNC_REGEXP | STR_FUNC_EXPAND),
		str_sword = (STR_FUNC_PARSING | STR_FUNC_WORD | STR_FUNC_ARRAY),
		str_dword = (STR_FUNC_PARSING | STR_FUNC_WORD | STR_FUNC_ARRAY | STR_FUNC_EXPAND),
		str_ssym = (STR_FUNC_PARSING | STR_FUNC_SYMBOL),
		str_ssymbols = (STR_FUNC_PARSING | STR_FUNC_SYMBOL | STR_FUNC_ARRAY),
		str_dsymbols = (STR_FUNC_PARSING | STR_FUNC_SYMBOL | STR_FUNC_ARRAY | STR_FUNC_EXPAND),
		str_heredoc = (STR_FUNC_PARSING | STR_FUNC_HEREDOC),
		str_xquote = (STR_FUNC_PARSING | STR_FUNC_XQUOTE | STR_FUNC_EXPAND),
	}

	class parser_heredoc_info
	{
		public bool allow_indent;
		public bool line_head;
		public mrb_string_type type;
		public Uint8Array term;
		public int term_len;
		public List<node> doc = new List<node>();

		public string GetString()
		{
			var sb = new StringBuilder();

			foreach (var str in doc) {
				if (str is str_node) {
					sb.Append(MrbParser.UTF8ArrayToString(((str_node)str).str, 0));
				}
				else if (str is begin_node) {
					foreach (var p in ((begin_node)str).progs) {
						if (p is str_node) {
							sb.Append(MrbParser.UTF8ArrayToString(((str_node)p).str, 0));
						}
						else if (p is heredoc_node) {
							sb.Append(((heredoc_node)p).info.GetString());
						}
						else if (p is call_node) {

						}
						else {
							throw new NotImplementedException();
						}
					}
				}
				else {
					throw new NotImplementedException();
				}
			}

			return sb.ToString();
		}

		public override string ToString()
		{
			return $"{term} {GetString()}";
		}

		internal void push_doc(node str)
		{
			doc.Add(str);
		}

		internal void claer_doc()
		{
			doc.Clear();
		}
	}

	public class node
	{
		object _car;
		object _cdr;

		public MrbParser p { get; private set; }
		public object car {
			get { return _car; }
			set {
				System.Diagnostics.Debug.Assert(GetType() == typeof(node));
				_car = value;
			}
		}
		public object cdr {
			get { return _cdr; }
			set {
				System.Diagnostics.Debug.Assert(GetType() == typeof(node));
				_cdr = value;
			}
		}
		public int lineno { get; protected set; }
		public int filename_index { get; protected set; }

		protected node(MrbParser p, node_type car)
		{
			this.p = p;
			_car = car;
			lineno = p.lineno;
			filename_index = p.current_filename_index;
		}

		public override string ToString()
		{
			if (cdr == null)
				return $"({car})";
			return $"({car}, {cdr})";
		}

		public void SET_LINENO(int n) { lineno = n; }

		public void NODE_LINENO(node n)
		{
			if (n != null) {
				filename_index = n.filename_index;
				lineno = n.lineno;
			}
		}

		public static node cons(MrbParser p, object car, object cdr)
		{
			var result = new node(p, 0);
			result.car = car;
			result.cdr = cdr;
			return result;
		}

		public virtual void append(node b)
		{
			node c = this;
			while (c.cdr != null) {
				c = (node)c.cdr;
			}
			if (b != null) {
				c.cdr = b;
			}
		}

		public static void dump_recur<T>(List<T> list, node tree)
		{
			while (tree != null) {
				list.Add((T)tree.car);
				tree = (node)tree.cdr;
			}
		}

		public virtual Element to_xml()
		{
			var a = car as node;
			if (a != null && cdr == null) {
				return a.to_xml();
			}

			throw new NotImplementedException();
		}
	}

	/* (:scope (vars..) (prog...)) */
	class scope_node : node
	{
		private List<mrb_sym> _local_variables = new List<mrb_sym>();
		private node _body;

		public scope_node(MrbParser p, node body)
			: base(p, node_type.NODE_SCOPE)
		{
			var n2 = (node)p.locals_node();

			if (n2 != null && (n2.car != null || n2.cdr != null)) {
				do {
					if (n2.car != null) {
						_local_variables.Add((mrb_sym)n2.car);
					}
					n2 = (node)n2.cdr;
				} while (n2 != null);
			}
			_body = body;
		}

		public List<mrb_sym> local_variables { get { return _local_variables; } }
		public node body { get { return _body; } }

		public override Element to_xml()
		{
			// TODO:？？？
			var s = Document.CreateElement("scope");
			var b = _body.to_xml();
			if (b != null) {
				s.AppendChild(b);
			}
			return s;
		}

		public override string ToString()
		{
			var str = "(:scope (";
			foreach (var v in local_variables) {
				str += $"{v}, ";
			}
			return str + $") {body})";
		}
	}

	/* (:begin prog...) */
	class begin_node : node
	{
		private List<node> _progs = new List<node>();

		public begin_node(MrbParser p, node body)
			: base(p, node_type.NODE_BEGIN)
		{
			while (body != null) {
				_progs.Add(body);
				body = (node)body.cdr;
			}
		}

		public begin_node(MrbParser p, List<node> progs)
			: base(p, node_type.NODE_BEGIN)
		{
			_progs.AddRange(progs);
		}

		public List<node> progs { get { return _progs; } }

		public override void append(node b)
		{
			_progs.Add((node)b.car);
		}

		public override Element to_xml()
		{
			switch (_progs.Count) {
			case 0:
				return null;
			case 1:
				return _progs[0].to_xml();
			}
			var b = _progs[0].to_xml();
			var p = b;
			for (int i = 1; i < _progs.Count; i++) {
				var n = Document.CreateElement("next");
				var q = _progs[i].to_xml();
				n.AppendChild(q);
				p.AppendChild(n);
				p = q;
			}

			return b;
		}

		public override string ToString()
		{
			var str = $"(:begin ";
			foreach (var v in progs) {
				str += $"{v}, ";
			}
			return str + ")";
		}
	}

	/* (:rescue body rescue else) */
	class rescue_node : node
	{
		private node _body;

		public class rescue_t
		{
			public List<node> handle_classes = new List<node>();
			public node exc_var;
			public node body;

			public override string ToString()
			{
				var str = "(";
				foreach (var c in handle_classes) {
					str += $"{c}, ";
				}
				return $"{exc_var} {body})";
			}
		}
		private List<rescue_t> _rescue = new List<rescue_t>();
		private node _else;

		public rescue_node(MrbParser p, node body, node resq, node els)
			: base(p, node_type.NODE_RESCUE)
		{
			_body = body;
			if (resq != null) {
				var n2 = (node)resq;

				while (n2 != null) {
					rescue_t r = new rescue_t();
					var n3 = (node)n2.car;
					if (n3.car != null) {
						dump_recur(r.handle_classes, (node)n3.car);
					}
					r.exc_var = (node)((node)n3.cdr).car;
					r.body = (node)((node)((node)n3.cdr).cdr).car;
					_rescue.Add(r);
					n2 = (node)n2.cdr;
				}
			}
			_else = els;
		}

		public node body { get { return _body; } }
		public List<rescue_t> rescue { get { return _rescue; } }
		public node @else { get { return _else; } }

		public override Element to_xml()
		{
			// TODO:？？？
			return body.to_xml();
		}

		public override string ToString()
		{
			var str = $"(:rescue {body} ";
			foreach (var r in rescue) {
				str += $"{r}, ";
			}
			return str + $" {@else})";
		}
	}

	/* (:ensure body ensure) */
	class ensure_node : node
	{
		private node _body;
		private node _ensure;

		public ensure_node(MrbParser p, node a, node b)
			: base(p, node_type.NODE_ENSURE)
		{
			_body = a;
			_ensure = b;
		}

		public node body { get { return _body; } }
		public node ensure { get { return _ensure; } }

		public override Element to_xml()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return $"(:ensure {body} {ensure})";
		}
	}

	/* (:nil) */
	class nil_node : node, IEvaluatable
	{
		public nil_node(MrbParser p)
			: base(p, node_type.NODE_NIL)
		{
		}

		public override Element to_xml()
		{
			var block = Document.CreateElement("block");
			block.SetAttribute("type", "logic_null");
			return block;
		}

		public override string ToString()
		{
			return $"(:nil)";
		}

		public node evaluate(string method, List<node> args)
		{
			switch (method) {
			case "!":
				if (args.Count != 0)
					break;
				return new true_node(p);
			}
			return null;
		}
	}

	/* (:true) */
	class true_node : node, IEvaluatable
	{
		public true_node(MrbParser p)
			: base(p, node_type.NODE_TRUE)
		{
		}

		public override Element to_xml()
		{
			var block = Document.CreateElement("block");
			block.SetAttribute("type", "logic_boolean");

			var field = Document.CreateElement("field");
			field.SetAttribute("name", "BOOL");
			field.AppendChild(Document.CreateTextNode("TRUE"));
			block.AppendChild(field);

			return block;
		}

		public override string ToString()
		{
			return $"(:true)";
		}

		public node evaluate(string method, List<node> args)
		{
			switch (method) {
			case "!":
				if (args.Count != 0)
					break;
				return new false_node(p);
			}
			return null;
		}
	}

	/* (:false) */
	class false_node : node, IEvaluatable
	{
		public false_node(MrbParser p)
			: base(p, node_type.NODE_FALSE)
		{
		}

		public override Element to_xml()
		{
			var block = Document.CreateElement("block");
			block.SetAttribute("type", "logic_boolean");

			var field = Document.CreateElement("field");
			field.SetAttribute("name", "BOOL");
			field.AppendChild(Document.CreateTextNode("FALSE"));
			block.AppendChild(field);

			return block;
		}

		public override string ToString()
		{
			return $"(:false)";
		}

		public node evaluate(string method, List<node> args)
		{
			switch (method) {
			case "!":
				if (args.Count != 0)
					break;
				return new true_node(p);
			}
			return null;
		}
	}

	/* (:alias new old) */
	class alias_node : node
	{
		private mrb_sym _new;
		private mrb_sym _old;

		public alias_node(MrbParser p, mrb_sym a, mrb_sym b)
			: base(p, node_type.NODE_ALIAS)
		{
			_new = a;
			_old = b;
		}

		public mrb_sym @new { get { return _new; } }
		public mrb_sym old { get { return _old; } }

		public override Element to_xml()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return $"(:alias {p.sym2name(@new)} {p.sym2name(old)})";
		}
	}

	/* (:if cond then else) */
	class if_node : node
	{
		private node _cond;
		private node _then;
		private node _else;

		public if_node(MrbParser p, node a, node b, node c)
			: base(p, node_type.NODE_IF)
		{
			_cond = a;
			_then = b;
			_else = c;
		}

		public node cond { get { return _cond; } }
		public node then { get { return _then; } }
		public node @else { get { return _else; } }

		public override Element to_xml()
		{
			var _elsif = new List<Tuple<node, node>>();
			node _else = this._else;

			_elsif.Add(new Tuple<node, node>(_cond, _then));
			for (var c = _else as if_node; c != null; _else = c._else, c = _else as if_node) {
				_elsif.Add(new Tuple<node, node>(c._cond, c._then));
			}

			var block = Document.CreateElement("block");
			block.SetAttribute("type", "controls_if");

			var mutation = Document.CreateElement("mutation");
			mutation.SetAttribute("elseif", _elsif.Count.ToString());
			mutation.SetAttribute("else", _else != null ? "1" : "0");
			block.AppendChild(mutation);

			int i = 0;
			foreach (var e in _elsif) {
				var value = Document.CreateElement("value");
				value.SetAttribute("name", $"IF{i}");
				value.AppendChild(e.Item1.to_xml());
				block.AppendChild(value);

				var statement = Document.CreateElement("statement");
				statement.SetAttribute("name", $"DO{i}");
				statement.AppendChild(e.Item2.to_xml());
				block.AppendChild(statement);
				i++;
			}

			if (_else != null) {
				var statement = Document.CreateElement("statement");
				statement.SetAttribute("name", "ELSE");
				statement.AppendChild(_else.to_xml());
				block.AppendChild(statement);
			}

			return block;
		}

		public override string ToString()
		{
			return $"(:if {cond} {then} {@else})";
		}
	}

	/* (:unless cond then else) */
	class unless_node : node
	{
		private node _cond;
		private node _then;
		private node _else;

		public unless_node(MrbParser p, node a, node b, node c)
			: base(p, node_type.NODE_IF)
		{
			_cond = a;
			_then = c;
			_else = b;
		}

		public node cond { get { return _cond; } }
		public node then { get { return _then; } }
		public node @else { get { return _else; } }

		public override Element to_xml()
		{
			var block = Document.CreateElement("block");
			block.SetAttribute("type", "controls_if");

			var value = Document.CreateElement("value");
			value.SetAttribute("name", "IF0");
			value.AppendChild(_cond.to_xml());
			block.AppendChild(value);

			if (_then != null) {
				var statement = Document.CreateElement("statement");
				statement.SetAttribute("name", "DO0");
				block.AppendChild(statement);

				statement.AppendChild(_then.to_xml());
			}

			if (_else != null) {
				var statement = Document.CreateElement("statement");
				statement.SetAttribute("name", "ELSE");
				block.AppendChild(statement);

				statement.AppendChild(_else.to_xml());
			}

			return block;
		}

		public override string ToString()
		{
			return $"(:unless {cond} {then} {@else})";
		}
	}

	/* (:while cond body) */
	class while_node : node
	{
		private node _cond;
		private node _body;

		public while_node(MrbParser p, node a, node b)
			: base(p, node_type.NODE_WHILE)
		{
			_cond = a;
			_body = b;
		}

		public node cond { get { return _cond; } }
		public node body { get { return _body; } }

		public override Element to_xml()
		{
			var block = Document.CreateElement("block");
			block.SetAttribute("type", "controls_whileUntil");

			var field = Document.CreateElement("field");
			field.SetAttribute("name", "MODE");
			field.AppendChild(Document.CreateTextNode("WHILE"));
			block.AppendChild(field);

			var value = Document.CreateElement("value");
			value.SetAttribute("name", "BOOL");
			value.AppendChild(_cond.to_xml());
			block.AppendChild(value);

			var statement = Document.CreateElement("statement");
			statement.SetAttribute("name", "DO");
			statement.AppendChild(_body.to_xml());
			block.AppendChild(statement);

			return block;
		}

		public override string ToString()
		{
			return $"(:while {cond} {body})";
		}
	}

	/* (:until cond body) */
	class until_node : node
	{
		private node _cond;
		private node _body;

		public until_node(MrbParser p, node a, node b)
			: base(p, node_type.NODE_UNTIL)
		{
			_cond = a;
			_body = b;
		}

		public node cond { get { return _cond; } }
		public node body { get { return _body; } }

		public override Element to_xml()
		{
			var block = Document.CreateElement("block");
			block.SetAttribute("type", "controls_whileUntil");

			var field = Document.CreateElement("field");
			field.SetAttribute("name", "MODE");
			field.AppendChild(Document.CreateTextNode("UNTIL"));
			block.AppendChild(field);

			var value = Document.CreateElement("value");
			value.SetAttribute("name", "BOOL");
			value.AppendChild(_cond.to_xml());
			block.AppendChild(value);

			var statement = Document.CreateElement("statement");
			statement.SetAttribute("name", "DO");
			statement.AppendChild(_body.to_xml());
			block.AppendChild(statement);

			return block;
		}

		public override string ToString()
		{
			return $"(:until {cond} {body})";
		}
	}

	/* (:for var obj body) */
	class for_node : node
	{
		public class var_t
		{
			public List<node> pre = new List<node>();
			public node rest;
			public List<node> post = new List<node>();

			public override string ToString()
			{
				var str = "(";
				foreach (var p in pre) {
					str += $"{p} ";
				}
				str += $"{rest} ";
				foreach (var p in post) {
					str += $"{p} ";
				}
				return str + ")";
			}
		}
		private var_t _var;
		private node _in;
		private node _do;

		public for_node(MrbParser p, node v, node o, node b)
			: base(p, node_type.NODE_FOR)
		{
			_var = new var_t();
			node n2 = v;

			if (n2.car != null) {
				dump_recur(_var.pre, (node)n2.car);
			}
			n2 = (node)n2.cdr;
			if (n2 != null) {
				if (n2.car != null) {
					_var.rest = (node)n2.car;
				}
				n2 = (node)n2.cdr;
				if (n2 != null) {
					if (n2.car != null) {
						dump_recur(_var.post, (node)n2.car);
					}
				}
			}
			_in = o;
			_do = b;
		}

		public var_t var { get { return _var; } }
		public node @in { get { return _in; } }
		public node @do { get { return _do; } }

		public override Element to_xml()
		{
			var block = Document.CreateElement("block");
			block.SetAttribute("type", "controls_forEach");

			var field = Document.CreateElement("field");
			field.SetAttribute("name", "VAR");
			// TODO:var？
			var pre = var.pre[0];
			switch ((node_type)pre.car) {
			case node_type.NODE_GVAR:
				field.AppendChild(Document.CreateTextNode(p.sym2name(((gvar_node)pre).name)));
				break;
			case node_type.NODE_CVAR:
				field.AppendChild(Document.CreateTextNode(p.sym2name(((cvar_node)pre).name)));
				break;
			case node_type.NODE_IVAR:
				field.AppendChild(Document.CreateTextNode(p.sym2name(((ivar_node)pre).name)));
				break;
			case node_type.NODE_LVAR:
				field.AppendChild(Document.CreateTextNode(p.sym2name(((lvar_node)pre).name)));
				break;
			default:
				// TODO: ？
				throw new NotImplementedException();
			}
			block.AppendChild(field);

			var value = Document.CreateElement("value");
			value.SetAttribute("name", "LIST");
			value.AppendChild(_in.to_xml());
			block.AppendChild(value);

			var statement = Document.CreateElement("statement");
			statement.SetAttribute("name", "DO");
			statement.AppendChild(_do.to_xml());
			block.AppendChild(statement);

			return block;
		}

		public override string ToString()
		{
			return $"(:for {var} {@in} {@do})";
		}
	}

	/* (:case a ((when ...) body) ((when...) body)) */
	class case_node : node
	{
		private node _arg;
		public class when_t
		{
			public List<node> value = new List<node>();
			public node body;

			public override string ToString()
			{
				var str = $"(when ";
				foreach (var c in value) {
					str += $"{c} ";
				}
				return str + $"{body})";
			}
		}
		private List<when_t> _when = new List<when_t>();

		public case_node(MrbParser p, node a, node b)
			: base(p, node_type.NODE_CASE)
		{
			_arg = a;
			while (b != null) {
				var w = new when_t();
				dump_recur(w.value, (node)((node)b.car).car);
				w.body = (node)((node)b.car).cdr;
				_when.Add(w);
				b = (node)b.cdr;
			}
		}

		public node arg { get { return _arg; } }
		public List<when_t> when { get { return _when; } }

		public override Element to_xml()
		{
			var block = Document.CreateElement("block");
			block.SetAttribute("type", "switch_case_number");

			int c = 0, d = 0;
			when_t default_node = null;
			foreach (var w in _when) {
				if (w.value.Count == 0) {
					default_node = w;
					d = 1;
				}
				else
					c++;
			}

			var mutation = Document.CreateElement("mutation");
			mutation.SetAttribute("case", c.ToString());
			mutation.SetAttribute("default", d.ToString());
			block.AppendChild(mutation);

			int i = 0;
			foreach (var w in _when) {
				if (w.value.Count == 0)
					continue;

				var field = Document.CreateElement("field");
				field.SetAttribute("name", "CONST" + i);
				// TODO:whenの値が複数の場合
				field.AppendChild(Document.CreateTextNode(MrbParser.UTF8ArrayToString(((int_node)w.value[0]).num, 0)));
				block.AppendChild(field);
				i++;
			}

			var value = Document.CreateElement("value");
			value.SetAttribute("name", "SWITCH");
			block.AppendChild(value);

			value.AppendChild(_arg.to_xml());

			foreach (var w in _when) {
				if (w.value.Count == 0)
					continue;

				var statement = Document.CreateElement("statement");
				statement.SetAttribute("name", "DO" + i);
				statement.AppendChild(w.body.to_xml());
				block.AppendChild(statement);
			}

			if (default_node != null) {
				var statement = Document.CreateElement("statement");
				statement.SetAttribute("name", "DEFAULT");
				statement.AppendChild(default_node.body.to_xml());
				block.AppendChild(statement);
			}

			return block;
		}

		public override string ToString()
		{
			var str = $"(:case {arg} ";
			foreach (var w in when) {
				str += $"{w} ";
			}
			return str + ")";
		}
	}

	/* (:postexe a) */
	class postexe_node : node
	{
		private node _postexe;

		public postexe_node(MrbParser p, node a)
			: base(p, node_type.NODE_POSTEXE)
		{
			_postexe = a;
		}

		public node postexe { get { return _postexe; } }

		public override Element to_xml()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return $"(:postexe {postexe})";
		}
	}

	/* (:self) */
	class self_node : node
	{
		public self_node(MrbParser p)
			: base(p, node_type.NODE_SELF)
		{
		}

		public override Element to_xml()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return $"(:self)";
		}
	}

	/* (:call a b c) */
	class call_node : node
	{
		private node _obj;
		private mrb_sym _method;
		private List<node> _args = new List<node>();
		private node _block;
		private MrbTokens _pass;

		public call_node(MrbParser p, node a, mrb_sym b, node c, MrbTokens pass)
			: base(p, pass != 0 ? node_type.NODE_CALL : node_type.NODE_SCALL)
		{
			_pass = pass;
			NODE_LINENO(a);

			_obj = a;
			_method = b;
			if (c != null) {
				dump_recur(_args, (node)c.car);
				if (c.cdr != null) {
					_block = (node)c.cdr;
				}
			}
		}

		public call_node(MrbParser p, node a, mrb_sym b, List<node> args, node block)
			: base(p, node_type.NODE_CALL)
		{
			_obj = a;
			_method = b;
			_args.AddRange(args);
			_block = block;
		}

		public node obj { get { return _obj; } }
		public mrb_sym method { get { return _method; } }
		public List<node> args { get { return _args; } }
		public node block { get { return _block; } }

		internal void add_block(node b)
		{
			if (b != null) {
				if (_block != null) {
					p.yyError("both block arg and actual block given");
				}
				_block = b;
			}
		}

		public override Element to_xml()
		{
			var method = p.sym2name(_method);
			switch (method) {
			case "==": return logic_compare("EQ");
			case "!=": return logic_compare("NEQ");
			case "<": return logic_compare("LT");
			case "<=": return logic_compare("LTE");
			case ">": return logic_compare("GT");
			case ">=": return logic_compare("GTE");
			}

			return procedures_callreturn(method);
		}

		private Element procedures_callreturn(string method)
		{
			var block = Document.CreateElement("block");
			block.SetAttribute("type", "procedures_callreturn");

			var mutation = Document.CreateElement("mutation");
			mutation.SetAttribute("name", method);
			block.AppendChild(mutation);

			int i = 0;
			foreach (var a in args) {
				var arg = Document.CreateElement("arg");
				// TODO: 引数名を持ってくkる
				arg.SetAttribute("name", i.ToString());
				arg.AppendChild(a.to_xml());
				block.AppendChild(arg);
				i++;
			}

			return block;
		}

		private Element logic_compare(string op)
		{
			var block = Document.CreateElement("block");
			block.SetAttribute("type", "logic_compare");

			var field = Document.CreateElement("field");
			field.SetAttribute("name", "OP");
			field.AppendChild(Document.CreateTextNode(op));
			block.AppendChild(field);

			var value = Document.CreateElement("value");
			value.SetAttribute("name", "A");
			value.AppendChild(_obj.to_xml());
			block.AppendChild(value);

			// argsは１つ
			value = Document.CreateElement("value");
			value.SetAttribute("name", "B");
			value.AppendChild(args[0].to_xml());
			block.AppendChild(value);

			return block;
		}

		public override string ToString()
		{
			var str = $"(:call {obj} {p.sym2name(method)} ";
			foreach (var v in args) {
				str += v.ToString() + " ";
			}
			return str + $"{block})";
		}
	}

	/* (:fcall self mid args) */
	class fcall_node : node
	{
		private node _self;
		private mrb_sym _method;
		private List<node> _args = new List<node>();
		private node _block;

		public fcall_node(MrbParser p, mrb_sym b, node c)
			: base(p, node_type.NODE_FCALL)
		{
			node n = p.new_self();
			n.NODE_LINENO(c);
			NODE_LINENO(c);

			_self = n;
			_method = b;
			if (c != null) {
				dump_recur(_args, (node)c.car);
				if (c.cdr != null) {
					_block = (node)c.cdr;
				}
			}
		}

		public node self { get { return _self; } }
		public mrb_sym method { get { return _method; } }
		public List<node> args { get { return _args; } }
		public node block { get { return _block; } }

		internal void add_block(node b)
		{
			if (b != null) {
				if (_block != null) {
					p.yyError("both block arg and actual block given");
				}
				_block = b;
			}
		}

		public override Element to_xml()
		{
			var block = Document.CreateElement("block");
			block.SetAttribute("type", "procedures_callreturn");

			var mutation = Document.CreateElement("mutation");
			mutation.SetAttribute("name", p.sym2name(_method));
			block.AppendChild(mutation);

			int i = 0;
			foreach (var a in args) {
				var arg = Document.CreateElement("arg");
				// TODO: 引数名を持ってくkる
				arg.SetAttribute("name", i.ToString());
				arg.AppendChild(a.to_xml());
				block.AppendChild(arg);
				i++;
			}

			return block;
		}

		public override string ToString()
		{
			var str = $"(:fcall {self} {p.sym2name(method)} ";
			foreach (var v in args) {
				str += v.ToString() + " ";
			}
			return str + $"{block})";
		}
	}

	/* (:super . c) */
	class super_node : node
	{
		private List<node> _args = new List<node>();
		private node _block;

		public super_node(MrbParser p, node c)
			: base(p, node_type.NODE_SUPER)
		{
			if (c != null) {
				dump_recur(_args, (node)c.car);
				if (c.cdr != null) {
					_block = (node)c.cdr;
				}
			}
		}

		public List<node> args { get { return _args; } }
		public node block { get { return _block; } }

		public override Element to_xml()
		{
			var block = Document.CreateElement("block");
			block.SetAttribute("type", "procedures_callreturn");

			var mutation = Document.CreateElement("mutation");
			mutation.SetAttribute("name", "super");
			block.AppendChild(mutation);

			int i = 0;
			foreach (var a in args) {
				var arg = Document.CreateElement("arg");
				// TODO: 引数名を持ってくkる
				arg.SetAttribute("name", i.ToString());
				arg.AppendChild(a.to_xml());
				block.AppendChild(arg);
				i++;
			}

			return block;
		}

		public override string ToString()
		{
			var str = "(:super ";
			foreach (var v in args) {
				str += v.ToString() + " ";
			}
			return $"{block})";
		}

		internal void add_block(node b)
		{
			if (b != null) {
				if (_block != null) {
					p.yyError("both block arg and actual block given");
				}
				_block = b;
			}
		}
	}

	/* (:zsuper) */
	class zsuper_node : node
	{
		private node _block; // 必要?

		public zsuper_node(MrbParser p)
			: base(p, node_type.NODE_ZSUPER)
		{
		}

		internal void add_block(node b)
		{
			if (b != null) {
				if (_block != null) {
					p.yyError("both block arg and actual block given");
				}
				_block = b;
			}
		}

		public override Element to_xml()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return $"(:zsuper)";
		}
	}

	/* (:yield . c) */
	class yield_node : node
	{
		private List<node> _args = new List<node>();

		public yield_node(MrbParser p, node c)
			: base(p, node_type.NODE_YIELD)
		{
			if (c != null) {
				if (c.cdr != null) {
					p.yyError("both block arg and actual block given");
				}
				c = (node)c.car;
			}

			dump_recur(_args, (node)c);
		}

		public List<node> args { get { return _args; } }

		public override Element to_xml()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			var str = "(:yield ";
			foreach (var v in args) {
				str += v.ToString() + " ";
			}
			return str + ")";
		}
	}

	/* (:return . c) */
	class return_node : node
	{
		private node _retval;

		public return_node(MrbParser p, node c)
			: base(p, node_type.NODE_RETURN)
		{
			_retval = c;
		}

		public node retval { get { return _retval; } }

		public override Element to_xml()
		{
			var block = Document.CreateElement("block");
			block.SetAttribute("type", "procedures_return");

			var mutation = Document.CreateElement("mutation");
			mutation.SetAttribute("value", (_retval != null) ? "0" : "1");
			block.AppendChild(mutation);

			if (_retval != null) {
				var value = Document.CreateElement("value");
				value.SetAttribute("name", "VALUE");
				value.AppendChild(_retval.to_xml());
				block.AppendChild(value);
			}

			return block;
		}

		public override string ToString()
		{
			return $"(:return . {retval})";
		}
	}

	/* (:break . c) */
	class break_node : node
	{
		private node _retval;

		public break_node(MrbParser p, node c)
			: base(p, node_type.NODE_BREAK)
		{
			_retval = c;
		}

		public node retval { get { return _retval; } }

		public override Element to_xml()
		{
			var block = Document.CreateElement("block");
			block.SetAttribute("type", "controls_flow_statements");

			var field = Document.CreateElement("field");
			field.SetAttribute("name", "FLOW");
			field.AppendChild(Document.CreateTextNode("BREAK"));
			block.AppendChild(field);

			return block;
		}

		public override string ToString()
		{
			return $"(:break . {retval})";
		}
	}

	/* (:next . c) */
	class next_node : node
	{
		private node _retval;

		public next_node(MrbParser p, node c)
			: base(p, node_type.NODE_NEXT)
		{
			_retval = c;
		}

		public node retval { get { return _retval; } }

		public override Element to_xml()
		{
			var block = Document.CreateElement("block");
			block.SetAttribute("type", "controls_flow_statements");

			var field = Document.CreateElement("field");
			field.SetAttribute("name", "FLOW");
			field.AppendChild(Document.CreateTextNode("CONTINUE"));
			block.AppendChild(field);

			return block;
		}

		public override string ToString()
		{
			return $"(:next . {retval})";
		}
	}

	/* (:redo) */
	class redo_node : node
	{
		public redo_node(MrbParser p)
			: base(p, node_type.NODE_REDO)
		{
		}

		public override Element to_xml()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return $"(:redo)";
		}
	}

	/* (:retry) */
	class retry_node : node
	{
		public retry_node(MrbParser p)
			: base(p, node_type.NODE_RETRY)
		{
		}

		public override Element to_xml()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return $"(:retry)";
		}
	}

	/* (:dot2 a b) */
	class dot2_node : node
	{
		private node _a;
		private node _b;

		public dot2_node(MrbParser p, node a, node b)
			: base(p, node_type.NODE_DOT2)
		{
			_a = a;
			_b = b;
		}

		public node a { get { return _a; } }

		public node b { get { return _b; } }

		public override Element to_xml()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return $"(:dot2 {a} {b})";
		}
	}

	/* (:dot3 a b) */
	class dot3_node : node
	{
		private node _a;
		private node _b;

		public dot3_node(MrbParser p, node a, node b)
			: base(p, node_type.NODE_DOT3)
		{
			_a = a;
			_b = b;
		}

		public node a { get { return _a; } }

		public node b { get { return _b; } }

		public override Element to_xml()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return $"(:dot3 {a} {b})";
		}
	}

	/* (:colon2 b c) */
	class colon2_node : node
	{
		private node _b;
		private mrb_sym _c;

		public colon2_node(MrbParser p, node b, mrb_sym c)
			: base(p, node_type.NODE_COLON2)
		{
			_b = b;
			_c = c;
		}

		public node b { get { return _b; } }
		public mrb_sym c { get { return _c; } }

		public override Element to_xml()
		{
			// TODO:？？？
			var block = Document.CreateElement("class");
			block.SetAttribute("const", p.sym2name(((const_node)_b).name));
			block.SetAttribute("name", p.sym2name(_c));

			return block;
		}

		public override string ToString()
		{
			return $"(:colon2 {b} {p.sym2name(c)})";
		}
	}

	/* (:colon3 . c) */
	class colon3_node : node
	{
		private mrb_sym _c;

		public colon3_node(MrbParser p, mrb_sym c)
			: base(p, node_type.NODE_COLON3)
		{
			_c = c;
		}

		public mrb_sym c { get { return _c; } }

		public override Element to_xml()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return $"(:colon3 . {p.sym2name(c)})";
		}
	}

	/* (:and a b) */
	class and_node : node
	{
		private node _a;
		private node _b;

		public and_node(MrbParser p, node a, node b)
			: base(p, node_type.NODE_AND)
		{
			_a = a;
			_b = b;
		}

		public node a { get { return _a; } }

		public node b { get { return _b; } }

		public override Element to_xml()
		{
			var block = Document.CreateElement("block");
			block.SetAttribute("type", "logic_operation");

			var field = Document.CreateElement("field");
			field.SetAttribute("name", "OP");
			field.AppendChild(Document.CreateTextNode("AND"));
			block.AppendChild(field);

			var value = Document.CreateElement("value");
			value.SetAttribute("name", "A");
			value.AppendChild(_a.to_xml());
			block.AppendChild(value);

			value = Document.CreateElement("value");
			value.SetAttribute("name", "B");
			value.AppendChild(_b.to_xml());
			block.AppendChild(value);

			return block;
		}

		public override string ToString()
		{
			return $"(:and {a} {b})";
		}
	}

	/* (:or a b) */
	class or_node : node
	{
		private node _a;
		private node _b;

		public or_node(MrbParser p, node a, node b)
			: base(p, node_type.NODE_OR)
		{
			_a = a;
			_b = b;
		}

		public node a { get { return _a; } }
		public node b { get { return _b; } }

		public override Element to_xml()
		{
			var block = Document.CreateElement("block");
			block.SetAttribute("type", "logic_operation");

			var field = Document.CreateElement("field");
			field.SetAttribute("name", "OP");
			field.AppendChild(Document.CreateTextNode("OR"));
			block.AppendChild(field);

			var value = Document.CreateElement("value");
			value.SetAttribute("name", "A");
			value.AppendChild(_a.to_xml());
			block.AppendChild(value);

			value = Document.CreateElement("value");
			value.SetAttribute("name", "B");
			value.AppendChild(_b.to_xml());
			block.AppendChild(value);

			return block;
		}

		public override string ToString()
		{
			return $"(:or {a} {b})";
		}
	}

	/* (:array a...) */
	class array_node : node
	{
		private List<node> _array = new List<node>();

		public array_node(MrbParser p, node a)
			: base(p, node_type.NODE_ARRAY)
		{
			dump_recur(_array, a);
		}

		public List<node> array { get { return _array; } }

		public override Element to_xml()
		{
			var block = Document.CreateElement("block");
			block.SetAttribute("type", "lists_create_with");

			var mutation = Document.CreateElement("mutation");
			mutation.SetAttribute("items", _array.Count.ToString());
			block.AppendChild(mutation);

			int i = 0;
			foreach (var item in _array) {
				var value = Document.CreateElement("value");
				value.SetAttribute("name", $"ADD{i}");
				value.AppendChild(_array[i].to_xml());
				block.AppendChild(value);
				i++;
			}

			return block;
		}

		public override string ToString()
		{
			var str = $"(:array ";
			foreach (var n in array) {
				str += $"{n.car} ";
			}
			return str + ")";
		}
	}

	/* (:splat . a) */
	class splat_node : node
	{
		private node _a;

		public splat_node(MrbParser p, node a)
			: base(p, node_type.NODE_SPLAT)
		{
			_a = a;
		}

		public node a { get { return _a; } }

		public override Element to_xml()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return $"(:splat . {a})";
		}
	}

	/* (:hash (k . v) (k . v)...) */
	class hash_node : node
	{
		public class kv_t
		{
			public node key;
			public node value;

			public override string ToString()
			{
				return $"({key} . {value})";
			}
		}
		List<kv_t> _kvs = new List<kv_t>();

		public hash_node(MrbParser p, node a)
			: base(p, node_type.NODE_HASH)
		{
			while (a != null) {
				var kv = new kv_t();
				kv.key = (node)((node)a.car).car;
				kv.value = (node)((node)a.car).cdr;
				_kvs.Add(kv);
				a = (node)a.cdr;
			}
		}

		public List<kv_t> kvs { get { return _kvs; } }

		public override Element to_xml()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			var str = $"(:hash ";
			foreach (var n in kvs) {
				str += $"{n} ";
			}
			return str + ")";
		}
	}

	/* (:sym . a) */
	class sym_node : node
	{
		private mrb_sym _name;

		public sym_node(MrbParser p, mrb_sym sym)
			: base(p, node_type.NODE_SYM)
		{
			_name = sym;
		}

		public mrb_sym name { get { return _name; } }

		public override Element to_xml()
		{
			var block = Document.CreateElement("block");
			block.SetAttribute("type", "variables_get");

			var field = Document.CreateElement("field");
			field.SetAttribute("name", "VAR");
			field.AppendChild(Document.CreateTextNode(":" + p.sym2name(_name)));
			block.AppendChild(field);

			return block;
		}

		public override string ToString()
		{
			return $"(:sym . {p.sym2name(name)})";
		}
	}

	/* (:lvar . a) */
	class lvar_node : node
	{
		private mrb_sym _name;

		public lvar_node(MrbParser p, mrb_sym sym)
			: base(p, node_type.NODE_LVAR)
		{
			_name = sym;
		}

		public mrb_sym name { get { return _name; } }

		public override Element to_xml()
		{
			var block = Document.CreateElement("block");
			block.SetAttribute("type", "variables_get");

			var field = Document.CreateElement("field");
			field.SetAttribute("name", "VAR");
			field.AppendChild(Document.CreateTextNode(p.sym2name(_name)));
			block.AppendChild(field);

			return block;
		}

		public override string ToString()
		{
			return $"(:lvar . {p.sym2name(name)})";
		}
	}

	/* (:gvar . a) */
	class gvar_node : node
	{
		private mrb_sym _name;

		public gvar_node(MrbParser p, mrb_sym sym)
			: base(p, node_type.NODE_GVAR)
		{
			_name = sym;
		}

		public mrb_sym name { get { return _name; } }

		public override Element to_xml()
		{
			var block = Document.CreateElement("block");
			block.SetAttribute("type", "variables_get");

			var field = Document.CreateElement("field");
			field.SetAttribute("name", "VAR");
			field.AppendChild(Document.CreateTextNode(p.sym2name(_name)));
			block.AppendChild(field);

			return block;
		}

		public override string ToString()
		{
			return $"(:gvar . {p.sym2name(name)})";
		}
	}

	/* (:ivar . a) */
	class ivar_node : node
	{
		private mrb_sym _name;

		public ivar_node(MrbParser p, mrb_sym sym)
			: base(p, node_type.NODE_IVAR)
		{
			_name = sym;
		}

		public mrb_sym name { get { return _name; } }

		public override Element to_xml()
		{
			var block = Document.CreateElement("block");
			block.SetAttribute("type", "variables_get");

			var field = Document.CreateElement("field");
			field.SetAttribute("name", "VAR");
			field.AppendChild(Document.CreateTextNode(p.sym2name(_name)));
			block.AppendChild(field);

			return block;
		}

		public override string ToString()
		{
			return $"(:ivar . {p.sym2name(name)})";
		}
	}

	/* (:cvar . a) */
	class cvar_node : node
	{
		private mrb_sym _name;

		public cvar_node(MrbParser p, mrb_sym sym)
			: base(p, node_type.NODE_CVAR)
		{
			_name = sym;
		}

		public mrb_sym name { get { return _name; } }

		public override Element to_xml()
		{
			var block = Document.CreateElement("block");
			block.SetAttribute("type", "variables_get");

			var field = Document.CreateElement("field");
			field.SetAttribute("name", "VAR");
			field.AppendChild(Document.CreateTextNode(p.sym2name(_name)));
			block.AppendChild(field);

			return block;
		}

		public override string ToString()
		{
			return $"(:cvar . {p.sym2name(name)})";
		}
	}

	/* (:const . a) */
	class const_node : node
	{
		private mrb_sym _name;

		public const_node(MrbParser p, mrb_sym sym)
			: base(p, node_type.NODE_CONST)
		{
			_name = sym;
		}

		public mrb_sym name { get { return _name; } }

		public override Element to_xml()
		{
			var block = Document.CreateElement("block");
			block.SetAttribute("type", "variables_get");

			var field = Document.CreateElement("field");
			field.SetAttribute("name", "VAR");
			field.AppendChild(Document.CreateTextNode(p.sym2name(_name)));
			block.AppendChild(field);

			return block;
		}

		public override string ToString()
		{
			return $"(:const . {p.sym2name(name)})";
		}
	}

	/* (:undef a...) */
	class undef_node : node
	{
		private List<mrb_sym> _syms = new List<mrb_sym>();

		public undef_node(MrbParser p, mrb_sym sym)
			: base(p, node_type.NODE_UNDEF)
		{
			_syms.Add(sym);
		}

		public List<mrb_sym> syms { get { return _syms; } }

		public override void append(node b)
		{
			_syms.Add((mrb_sym)b.car);
		}

		public override Element to_xml()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			var str = "(:undef ";
			foreach (var n in syms) {
				str += $"{p.sym2name(n)}, ";
			}
			return str + ")";
		}
	}

	/* (:class class super body) */
	class class_node : node
	{
		private string _prefix;
		private node _type;
		private mrb_sym _name;
		private node _super;
		private node _body;

		public class_node(MrbParser p, node c, node s, node b)
			: base(p, node_type.NODE_CLASS)
		{
			if (c.car is int) {
				var type = (int)c.car;
				if (type == 0) {
					_prefix = ":";
					_name = (mrb_sym)c.cdr;
				}
				else if (type == 1) {
					_prefix = "::";
					_name = (mrb_sym)c.cdr;
				}
			}
			else {
				_prefix = "::";
				_type = (node)c.car;
				_name = (mrb_sym)c.cdr;
			}
			_super = (node)p.locals_node();
			_body = b;
		}

		public string prefix { get { return _prefix; } }
		public mrb_sym name { get { return _name; } }
		public node super { get { return _super; } }
		public node body { get { return _body; } }

		public override Element to_xml()
		{
			// TODO:クラス？
			var block = Document.CreateElement("class");
			block.SetAttribute("name", p.sym2name(_name));

			if (_super != null) {
				var field = Document.CreateElement("field");
				field.SetAttribute("name", "SUPER");
				field.AppendChild(_super.to_xml());
				block.AppendChild(field);
			}

			var statement = Document.CreateElement("statement");
			statement.SetAttribute("name", "BODY");
			statement.AppendChild(_body.to_xml());
			block.AppendChild(statement);

			return block;
		}

		public override string ToString()
		{
			return $"(:class {prefix}{p.sym2name(name)} {super} {body})";
		}
	}

	/* (:sclass obj body) */
	class sclass_node : node
	{
		private node _obj;
		private node _super;
		private node _body;

		public sclass_node(MrbParser p, node o, node b)
			: base(p, node_type.NODE_SCLASS)
		{
			_obj = o;
			_super = (node)p.locals_node();
			_body = b;
		}

		public node obj { get { return _obj; } }
		public node body { get { return _body; } }

		public override Element to_xml()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return $"(:sclass {obj} {body})";
		}
	}

	/* (:module module body) */
	class module_node : node
	{
		private string _prefix;
		private mrb_sym _name;
		private object _type;
		private node _super;
		private node _body;

		public module_node(MrbParser p, node m, node b)
			: base(p, node_type.NODE_MODULE)
		{
			if (m.car is int) {
				if ((int)m.car == 0) {
					_prefix = ":";
					_name = (mrb_sym)m.cdr;
				}
				else if ((int)m.car == 1) {
					_prefix = "::";
					_name = (mrb_sym)m.cdr;
				}
				_type = m.car;
			}
			else {
				_prefix = "::";
				_type = m.car;
				_name = (mrb_sym)m.cdr;
			}
			_super = (node)p.locals_node();
			_body = b;
		}

		public string prefix { get { return _prefix; } }
		public mrb_sym name { get { return _name; } }
		public object type { get { return _type; } }
		public node body { get { return _body; } }

		public override Element to_xml()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return $"(:module {prefix}{p.sym2name(name)} {body})";
		}
	}

	public class args_t
	{
		MrbParser p;
		public mrb_sym name;
		public node arg;

		public args_t(MrbParser p)
		{
			this.p = p;
		}

		public override string ToString()
		{
			return $"({p.sym2name(name)} {arg})";
		}
	}

	/* (:def m lv (arg . body)) */
	class def_node : node
	{
		private mrb_sym _name;
		private List<mrb_sym> _local_variables = new List<mrb_sym>();
		private List<arg_node> _mandatory_args = new List<arg_node>();
		private List<args_t> _optional_args = new List<args_t>();
		private mrb_sym _rest;
		private List<arg_node> _post_mandatory_args = new List<arg_node>();
		private mrb_sym _blk;
		private node _body;

		public def_node(MrbParser p, mrb_sym m, node a, node b)
			: base(p, node_type.NODE_DEF)
		{
			_name = m;
			{
				var n2 = (node)p.locals_node();

				if (n2 != null && (n2.car != null || n2.cdr != null)) {
					while (n2 != null) {
						if ((mrb_sym)n2.car != 0) {
							_local_variables.Add((mrb_sym)n2.car);
						}
						n2 = (node)n2.cdr;
					}
				}
			}
			if (a != null) {
				node n = a;

				if (n.car != null) {
					dump_recur(_mandatory_args, (node)n.car);
				}
				n = (node)n.cdr;
				if (n.car != null) {
					var n2 = (node)n.car;

					while (n2 != null) {
						var arg = new args_t(p);
						arg.name = (mrb_sym)((node)n2.car).car;
						arg.arg = (node)((node)n2.car).cdr;
						_optional_args.Add(arg);
						n2 = (node)n2.cdr;
					}
				}
				n = (node)n.cdr;
				if (n.car != null) {
					_rest = (mrb_sym)n.car;
				}
				n = (node)n.cdr;
				if (n.car != null) {
					dump_recur(_post_mandatory_args, (node)n.car);
				}
				if (n.cdr != null) {
					_blk = (mrb_sym)n.cdr;
				}
			}
			_body = b;
		}

		public mrb_sym name { get { return _name; } }
		public List<mrb_sym> local_variables { get { return _local_variables; } }
		public List<arg_node> mandatory_args { get { return _mandatory_args; } }
		internal List<args_t> optional_args { get { return _optional_args; } }
		public mrb_sym rest { get { return _rest; } }
		public List<arg_node> post_mandatory_args { get { return _post_mandatory_args; } }
		public mrb_sym blk { get { return _blk; } }
		public node body { get { return _body; } }

		public override Element to_xml()
		{
			var block = Document.CreateElement("block");
			block.SetAttribute("type", "procedures_defreturn");

			var field = Document.CreateElement("field");
			field.SetAttribute("name", "NAME");
			field.AppendChild(Document.CreateTextNode(p.sym2name(_name)));
			block.AppendChild(field);

			Element bxml;
			if (_body != null && (bxml = _body.to_xml()) != null) {
				var statement = Document.CreateElement("statement");
				statement.SetAttribute("name", "STACK");
				statement.AppendChild(bxml);
				block.AppendChild(statement);
			}

			return block;
		}

		public override string ToString()
		{
			var str = $"(:def {p.sym2name(name)} ";
			foreach (var n in local_variables) {
				str += $"{p.sym2name(n)}, ";
			}
			str += $" (";
			foreach (var n in mandatory_args) {
				str += $"{n}, ";
			}
			foreach (var n in optional_args) {
				str += $"{n}, ";
			}
			str += $"{p.sym2name(rest)}, ";
			foreach (var n in post_mandatory_args) {
				str += $"{n}, ";
			}
			str += $"{p.sym2name(blk)}, ";
			return str + $" . {body}))";
		}
	}

	/* (:sdef obj m lv (arg . body)) */
	class sdef_node : node
	{
		private node _obj;
		private mrb_sym _name;
		private node _lv;
		private List<arg_node> _mandatory_args = new List<arg_node>();
		private List<args_t> _optional_args = new List<args_t>();
		private mrb_sym _rest;
		private List<arg_node> _post_mandatory_args = new List<arg_node>();
		private mrb_sym _blk;
		private node _body;

		public sdef_node(MrbParser p, node o, mrb_sym m, node a, node b)
			: base(p, node_type.NODE_SDEF)
		{
			_obj = o;
			_name = m;
			_lv = (node)p.locals_node();
			if (a != null) {
				node n = a;

				if (n.car != null) {
					dump_recur(_mandatory_args, (node)n.car);
				}
				n = (node)n.cdr;
				if (n.car != null) {
					var n2 = (node)n.car;

					while (n2 != null) {
						var arg = new args_t(p);
						arg.name = (mrb_sym)((node)n2.car).car;
						arg.arg = (node)((node)n2.car).cdr;
						_optional_args.Add(arg);
						n2 = (node)n2.cdr;
					}
				}
				n = (node)n.cdr;
				if (n.car != null) {
					_rest = (mrb_sym)n.car;
				}
				n = (node)n.cdr;
				if (n.car != null) {
					dump_recur(_post_mandatory_args, (node)n.car);
				}
				_blk = (mrb_sym)n.cdr;
			}
			_body = b;
		}

		public node obj { get { return _obj; } }
		public mrb_sym name { get { return _name; } }
		public List<arg_node> mandatory_args { get { return _mandatory_args; } }
		internal List<args_t> optional_args { get { return _optional_args; } }
		public mrb_sym rest { get { return _rest; } }
		public List<arg_node> post_mandatory_args { get { return _post_mandatory_args; } }
		public mrb_sym blk { get { return _blk; } }
		public node body { get { return _body; } }

		public override Element to_xml()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			var str = $"(:sdef {obj} {p.sym2name(name)} (";
			foreach (var n in mandatory_args) {
				str += $"{n}, ";
			}
			foreach (var n in optional_args) {
				str += $"{n}, ";
			}
			str += $"{p.sym2name(rest)}, ";
			foreach (var n in post_mandatory_args) {
				str += $"{n}, ";
			}
			str += $"{p.sym2name(blk)}, ";
			return str + $" . {body}))";
		}
	}

	/* (:arg . sym) */
	class arg_node : node
	{
		private mrb_sym _name;

		public arg_node(MrbParser p, mrb_sym sym)
			: base(p, node_type.NODE_ARG)
		{
			_name = sym;
		}

		public mrb_sym name { get { return _name; } }

		public override Element to_xml()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return $"(:arg . {p.sym2name(name)})";
		}
	}

	/* (:block_arg . a) */
	class block_arg_node : node
	{
		private node _a;

		public block_arg_node(MrbParser p, node a)
			: base(p, node_type.NODE_BLOCK_ARG)
		{
			_a = a;
		}

		public node a { get { return _a; } }

		public override Element to_xml()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return $"(:block_arg . {a})";
		}
	}

	/* (:block arg body) */
	class block_node : node
	{
		private List<mrb_sym> _mandatory_args = new List<mrb_sym>();
		private List<args_t> _optional_args = new List<args_t>();
		private node _body;

		public block_node(MrbParser p, node a, node b)
			: base(p, node_type.NODE_BLOCK)
		{
			var t = (node)p.locals_node();
			if (t != null) {
				node n = t;

				while (n != null) {
					_mandatory_args.Add((mrb_sym)n.car);
					n = (node)n.cdr;
				}
				if (a != null) {
					var n2 = (node)a.car;

					while (n2 != null) {
						var arg = new args_t(p);
						arg.name = (mrb_sym)((node)n2.car).car;
						arg.arg = (node)((node)n2.car).cdr;
						_optional_args.Add(arg);
						n2 = (node)n2.cdr;
					}
				}
			}
			_body = b;
		}

		public List<mrb_sym> mandatory_args { get { return _mandatory_args; } }
		internal List<args_t> optional_args { get { return _optional_args; } }
		public node body { get { return _body; } }

		public override Element to_xml()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			var str = $"(:block ";
			foreach (var n in mandatory_args) {
				str += $"{p.sym2name(n)}, ";
			}
			foreach (var n in optional_args) {
				str += $"{n}, ";
			}
			return str + $" . {body}))";
		}
	}

	/* (:lambda arg body) */
	class lambda_node : node
	{
		private List<mrb_sym> _mandatory_args = new List<mrb_sym>();
		private List<args_t> _optional_args = new List<args_t>();
		private node _body;

		public lambda_node(MrbParser p, node a, node b)
			: base(p, node_type.NODE_LAMBDA)
		{
			var t = (node)p.locals_node();
			if (t != null) {
				node n = t;

				while (n != null) {
					_mandatory_args.Add((mrb_sym)n.car);
					n = (node)n.cdr;
				}
				if (a != null) {
					var n2 = (node)a.car;

					while (n2 != null) {
						var arg = new args_t(p);
						arg.name = (mrb_sym)((node)n2.car).car;
						arg.arg = (node)((node)n2.car).cdr;
						_optional_args.Add(arg);
						n2 = (node)n2.cdr;
					}
				}
			}
			_body = b;
		}

		public List<mrb_sym> mandatory_args { get { return _mandatory_args; } }
		internal List<args_t> optional_args { get { return _optional_args; } }
		public node body { get { return _body; } }

		public override Element to_xml()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			var str = $"(:lambda ";
			foreach (var n in mandatory_args) {
				str += $"{n}, ";
			}
			foreach (var n in optional_args) {
				str += $"{n}, ";
			}
			return str + $" . {body}))";
		}
	}

	/* (:asgn lhs rhs) */
	class asgn_node : node
	{
		private node _lhs;
		private node _rhs;

		public asgn_node(MrbParser p, node a, node b)
			: base(p, node_type.NODE_ASGN)
		{
			_lhs = a;
			_rhs = b;
		}

		public node lhs { get { return _lhs; } }
		public node rhs { get { return _rhs; } }

		public override Element to_xml()
		{
			var block = Document.CreateElement("block");
			block.SetAttribute("type", "variables_set");

			var field = Document.CreateElement("field");
			field.SetAttribute("name", "VAR");
			switch ((node_type)_lhs.car) {
			case node_type.NODE_GVAR:
				field.AppendChild(Document.CreateTextNode(p.sym2name(((gvar_node)_lhs).name)));
				break;
			case node_type.NODE_CVAR:
				field.AppendChild(Document.CreateTextNode(p.sym2name(((cvar_node)_lhs).name)));
				break;
			case node_type.NODE_IVAR:
				field.AppendChild(Document.CreateTextNode(p.sym2name(((ivar_node)_lhs).name)));
				break;
			case node_type.NODE_LVAR:
				field.AppendChild(Document.CreateTextNode(p.sym2name(((lvar_node)_lhs).name)));
				break;
			default:
				// TODO: list[0] = ...？
				field.AppendChild(_lhs.to_xml());
				break;
			}
			block.AppendChild(field);

			var value = Document.CreateElement("value");
			value.SetAttribute("name", "VALUE");
			block.AppendChild(value);

			value.AppendChild(_rhs.to_xml());

			return block;
		}

		public override string ToString()
		{
			return $"(:asgn {lhs} {rhs})";
		}
	}

	/* (:masgn mlhs=(pre rest post)  mrhs) */
	class masgn_node : node
	{
		public class mlhs_t
		{
			public List<node> pre = new List<node>();
			public node rest;
			public List<node> post = new List<node>();

			public override string ToString()
			{
				var str = "(";
				foreach (var p in pre) {
					str += $"{p} ";
				}
				str += $"{rest} ";
				foreach (var p in post) {
					str += $"{p} ";
				}
				return str + ")";
			}
		}
		private mlhs_t _mlhs;
		private node _mrhs;

		public masgn_node(MrbParser p, node a, node b)
			: base(p, node_type.NODE_MASGN)
		{
			_mlhs = new mlhs_t();
			{
				node n2 = a;

				if (n2.car != null) {
					dump_recur(_mlhs.pre, (node)n2.car);
				}
				n2 = (node)n2.cdr;
				if (n2 != null) {
					if (n2.car != null) {
						if (n2.car is int && (int)n2.car == -1) {
							_mlhs.rest = null; //(empty)?
						}
						else {
							_mlhs.rest = (node)n2.car;
						}
					}
					n2 = (node)n2.cdr;
					if (n2 != null) {
						if (n2.car != null) {
							dump_recur(_mlhs.post, (node)n2.car);
						}
					}
				}
			}
			_mrhs = b;
		}

		public mlhs_t mlhs { get { return _mlhs; } }
		public node mrhs { get { return _mrhs; } }

		public override Element to_xml()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return $"(:masgn {mlhs} {mrhs})";
		}
	}

	/* (:asgn lhs rhs) */
	class op_asgn_node : node
	{
		private node _lhs;
		private mrb_sym _op;
		private node _rhs;

		public op_asgn_node(MrbParser p, node lhs, mrb_sym op, node rhs)
			: base(p, node_type.NODE_OP_ASGN)
		{
			_lhs = lhs;
			_op = op;
			_rhs = rhs;
		}

		public node lhs { get { return _lhs; } }
		public mrb_sym op { get { return _op; } }
		public node rhs { get { return _rhs; } }

		public override Element to_xml()
		{
			// TODO:Rubyの演算は数値だけとは限らない
			var block = Document.CreateElement("block");
			block.SetAttribute("type", "math_arithmetic");

			var field = Document.CreateElement("field");
			field.SetAttribute("name", "OP");
			switch (p.sym2name(op)) {
			case "+": field.AppendChild(Document.CreateTextNode("ADD")); break;
			case "-": field.AppendChild(Document.CreateTextNode("MINUS")); break;
			case "*": field.AppendChild(Document.CreateTextNode("MULTIPLY")); break;
			case "/": field.AppendChild(Document.CreateTextNode("DIVIDE")); break;
			case "**": field.AppendChild(Document.CreateTextNode("POWER")); break;
			}
			block.AppendChild(field);

			var value = Document.CreateElement("value");
			value.SetAttribute("name", "A");
			value.AppendChild(lhs.to_xml());
			block.AppendChild(value);

			value = Document.CreateElement("value");
			value.SetAttribute("name", "B");
			value.AppendChild(rhs.to_xml());
			block.AppendChild(value);

			return block;
		}

		public override string ToString()
		{
			return $"(:asgn {lhs} {p.sym2name(op)} {rhs})";
		}
	}

	class negate_node : node
	{
		node _n;

		public negate_node(MrbParser p, node n)
			: base(p, node_type.NODE_NEGATE)
		{
			this._n = n;
		}

		public node n { get { return _n; } }

		public override Element to_xml()
		{
			var block = Document.CreateElement("block");
			block.SetAttribute("type", "math_single");

			var field = Document.CreateElement("field");
			field.SetAttribute("name", "OP");
			field.AppendChild(Document.CreateTextNode("NEG"));
			block.AppendChild(field);

			var value = Document.CreateElement("value");
			value.SetAttribute("name", "NUM");
			value.AppendChild(_n.to_xml());
			block.AppendChild(value);

			return block;
		}

		public override string ToString()
		{
			return $"(:nagete {n})";
		}
	}

	/* (:int . i) */
	class int_node : node, IEvaluatable
	{
		private Uint8Array _s;
		private int _base;

		public int_node(MrbParser p, Uint8Array s, int @base)
			: base(p, node_type.NODE_INT)
		{
			_s = MrbParser.strdup(s, 0);
			_base = @base;
		}

		public Uint8Array num { get { return _s; } }
		public int @base { get { return _base; } }

		public override Element to_xml()
		{
			var block = Document.CreateElement("block");
			block.SetAttribute("type", "math_number");

			var field = Document.CreateElement("field");
			field.SetAttribute("name", "NUM");
			field.AppendChild(Document.CreateTextNode(GetString()));
			block.AppendChild(field);

			return block;
		}

		public override string ToString()
		{
			string num = GetString();
			return $"(:int . {num})";
		}

		private string GetString()
		{
			string num;

			switch (_base) {
			case 2:
				num = "0b" + MrbParser.UTF8ArrayToString(_s, 0);
				break;
			case 8:
				num = "0o" + MrbParser.UTF8ArrayToString(_s, 0);
				break;
			case 16:
				num = "0x" + MrbParser.UTF8ArrayToString(_s, 0);
				break;
			default:
				num = MrbParser.UTF8ArrayToString(_s, 0);
				break;
			}

			return num;
		}

		internal long to_i()
		{
			var str = MrbParser.UTF8ArrayToString(_s, 0);
			return Convert.ToInt64(str, _base);
		}

		public node evaluate(string method, List<node> args)
		{
			if (args.Count != 1)
				return null;

			var arg = args[0];
			var a = to_i();

			if (arg is int_node) {
				var b = ((int_node)arg).to_i();

				switch (method) {
				case "+": {
						var c = MrbParser.UTF8StringToArray((a + b).ToString());
						return new int_node(p, c, 10);
					}
				case "-": {
						var c = MrbParser.UTF8StringToArray((a - b).ToString());
						return new int_node(p, c, 10);
					}
				case "*": {
						var c = MrbParser.UTF8StringToArray((a * b).ToString());
						return new int_node(p, c, 10);
					}
				case "/": {
						var c = MrbParser.UTF8StringToArray((a / b).ToString());
						return new int_node(p, c, 10);
					}
				case "%": {
						var c = MrbParser.UTF8StringToArray((a % b).ToString());
						return new int_node(p, c, 10);
					}
				case "==": {
						if (a == b)
							return new true_node(p);
						else
							return new false_node(p);
					}
				case "&": {
						var c = MrbParser.UTF8StringToArray((a & b).ToString());
						return new int_node(p, c, 10);
					}
				case "|": {
						var c = MrbParser.UTF8StringToArray((a | b).ToString());
						return new int_node(p, c, 10);
					}
				case "^": {
						var c = MrbParser.UTF8StringToArray((a ^ b).ToString());
						return new int_node(p, c, 10);
					}
				case "<<": {
						var c = MrbParser.UTF8StringToArray((a << (int)b).ToString());
						return new int_node(p, c, 10);
					}
				case ">>": {
						var c = MrbParser.UTF8StringToArray((a >> (int)b).ToString());
						return new int_node(p, c, 10);
					}
				}
			}
			else if (arg is float_node) {
				var b = ((float_node)arg).to_f();

				switch (method) {
				case "+": {
						var c = MrbParser.UTF8StringToArray((a + b).ToString());
						return new float_node(p, c);
					}
				case "-": {
						var c = MrbParser.UTF8StringToArray((a - b).ToString());
						return new float_node(p, c);
					}
				case "*": {
						var c = MrbParser.UTF8StringToArray((a * b).ToString());
						return new float_node(p, c);
					}
				case "/": {
						var c = MrbParser.UTF8StringToArray((a / b).ToString());
						return new float_node(p, c);
					}
				case "%": {
						var c = MrbParser.UTF8StringToArray((a % b).ToString());
						return new float_node(p, c);
					}
				case "==": {
						if (a == b)
							return new true_node(p);
						else
							return new false_node(p);
					}
				}
			}

			return null;
		}
	}

	/* (:float . i) */
	class float_node : node, IEvaluatable
	{
		private Uint8Array _s;

		public float_node(MrbParser p, Uint8Array s)
			: base(p, node_type.NODE_FLOAT)
		{
			_s = MrbParser.strdup(s, 0);
		}

		public Uint8Array num { get { return _s; } }

		public override Element to_xml()
		{
			var block = Document.CreateElement("block");
			block.SetAttribute("type", "math_number");

			var field = Document.CreateElement("field");
			field.SetAttribute("name", "NUM");
			field.AppendChild(Document.CreateTextNode(MrbParser.UTF8ArrayToString(_s, 0)));
			block.AppendChild(field);

			return block;
		}

		public override string ToString()
		{
			return $"(:float . {num})";
		}

		internal double to_f()
		{
			var str = MrbParser.UTF8ArrayToString(_s, 0);
			return Convert.ToDouble(str);
		}

		public node evaluate(string method, List<node> args)
		{
			if (args.Count != 1)
				return null;

			var arg = args[0];
			var a = to_f();

			if (arg is int_node) {
				var b = ((int_node)arg).to_i();

				switch (method) {
				case "+": {
						var c = MrbParser.UTF8StringToArray((a + b).ToString());
						return new float_node(p, c);
					}
				case "-": {
						var c = MrbParser.UTF8StringToArray((a - b).ToString());
						return new float_node(p, c);
					}
				case "*": {
						var c = MrbParser.UTF8StringToArray((a * b).ToString());
						return new float_node(p, c);
					}
				case "/": {
						var c = MrbParser.UTF8StringToArray((a / b).ToString());
						return new float_node(p, c);
					}
				case "%": {
						var c = MrbParser.UTF8StringToArray((a % b).ToString());
						return new float_node(p, c);
					}
				case "==": {
						if (a == b)
							return new true_node(p);
						else
							return new false_node(p);
					}
				}
			}
			else if (arg is float_node) {
				var b = ((float_node)arg).to_f();

				switch (method) {
				case "+": {
						var c = MrbParser.UTF8StringToArray((a + b).ToString());
						return new float_node(p, c);
					}
				case "-": {
						var c = MrbParser.UTF8StringToArray((a - b).ToString());
						return new float_node(p, c);
					}
				case "*": {
						var c = MrbParser.UTF8StringToArray((a * b).ToString());
						return new float_node(p, c);
					}
				case "/": {
						var c = MrbParser.UTF8StringToArray((a / b).ToString());
						return new float_node(p, c);
					}
				case "%": {
						var c = MrbParser.UTF8StringToArray((a % b).ToString());
						return new float_node(p, c);
					}
				case "==": {
						if (a == b)
							return new true_node(p);
						else
							return new false_node(p);
					}
				}
			}

			return null;
		}
	}

	/* (:str . (s . len)) */
	class str_node : node, IEvaluatable
	{
		private Uint8Array _str;
		private int _len;

		public str_node(MrbParser p, Uint8Array s, int len)
			: base(p, node_type.NODE_STR)
		{
			_str = MrbParser.strndup(s, 0, len);
			_len = len;
		}

		public Uint8Array str { get { return _str; } }
		public int len { get { return _len; } }

		public override Element to_xml()
		{
			var block = Document.CreateElement("block");
			block.SetAttribute("type", "text");

			var field = Document.CreateElement("field");
			field.SetAttribute("name", "TEXT");
			field.AppendChild(Document.CreateTextNode(MrbParser.UTF8ArrayToString(_str, 0)));
			block.AppendChild(field);

			return block;
		}

		public override string ToString()
		{
			return $"(:str . ('{str}' . {len}))";
		}

		public string to_s()
		{
			return MrbParser.UTF8ArrayToString(_str, 0);
		}

		public node evaluate(string method, List<node> args)
		{
			var s = to_s();

			switch (method) {
			case "<=>": {
					if ((args.Count != 1) || !(args[0] is str_node))
						break;
					var c = MrbParser.UTF8StringToArray(String.Compare(s, ((str_node)args[0]).to_s()).ToString());
					return new int_node(p, c, 10);
				}
			case "==": {
					if ((args.Count != 1) || !(args[0] is str_node))
						break;
					if (String.Compare(s, ((str_node)args[0]).to_s()) == 0)
						return new true_node(p);
					else
						return new true_node(p);
				}
			case "+": {
					if ((args.Count != 1) || !(args[0] is str_node))
						break;
					var t = MrbParser.UTF8StringToArray(s + ((str_node)args[0]).to_s());
					return new str_node(p, t, t.Length - 1);
				}
			case "*": {
					int a;
					if (args.Count != 1)
						break;
					if (args[0] is int_node)
						a = (int)((int_node)args[0]).to_i();
					else if (args[0] is float_node)
						a = (int)((float_node)args[0]).to_f();
					else
						break;
					var sb = new StringBuilder();
					for (var i = 0; i < a; i++) {
						sb.Append(a);
					}
					var t = MrbParser.UTF8StringToArray(sb.ToString());
					return new str_node(p, t, t.Length - 1);
				}
			}

			return null;
		}
	}

	/* (:dstr . a) */
	class dstr_node : node
	{
		private List<node> _a = new List<node>();

		public dstr_node(MrbParser p, node a)
			: base(p, node_type.NODE_DSTR)
		{
			dump_recur(_a, a);
		}

		public List<node> a { get { return _a; } }

		public override Element to_xml()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			var str = $"(:dstr . ";
			foreach (var n in a) {
				str += $"{n} ";
			}
			return str + ")";
		}
	}

	/* (:str . (s . len)) */
	class xstr_node : node
	{
		private Uint8Array _str;
		private int _len;

		public xstr_node(MrbParser p, Uint8Array s, int len)
			: base(p, node_type.NODE_XSTR)
		{
			_str = MrbParser.strndup(s, 0, len);
			_len = len;
		}

		public Uint8Array str { get { return _str; } }
		public int len { get { return _len; } }

		public override Element to_xml()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return $"(:str . ({str} . {len}))";
		}
	}

	/* (:xstr . a) */
	class dxstr_node : node
	{
		private List<node> _a = new List<node>();

		public dxstr_node(MrbParser p, node a)
			: base(p, node_type.NODE_DXSTR)
		{
			dump_recur(_a, a);
		}

		public List<node> a { get { return _a; } }

		public override Element to_xml()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			var str = $"(:xstr . ";
			foreach (var n in a) {
				str += $"{n} ";
			}
			return str + ")";
		}
	}

	/* (:dsym . a) */
	class dsym_node : node
	{
		private dstr_node _a;

		public dsym_node(MrbParser p, node a)
			: base(p, node_type.NODE_DSYM)
		{
			_a = p.new_dstr(a);
		}

		public List<node> a { get { return _a.a; } }

		public override Element to_xml()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return $"(:dsym . {a})";
		}
	}

	/* (:regx . (a . a)) */
	class regx_node : node
	{
		Uint8Array _pattern;
		Uint8Array _flags;
		Uint8Array _encp;

		public regx_node(MrbParser p, Uint8Array pattern, Uint8Array flags, Uint8Array encp)
			: base(p, node_type.NODE_REGX)
		{
			_pattern = pattern;
			_flags = flags;
			_encp = encp;
		}

		public Uint8Array pattern { get { return _pattern; } }
		public Uint8Array flags { get { return _flags; } }
		public Uint8Array encp { get { return _encp; } }

		public override Element to_xml()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return $"(:regx . ({pattern} . {flags} . {encp}))";
		}
	}

	/* (:dregx . a) */
	class dregx_node : node
	{
		private List<node> _a = new List<node>();
		private Uint8Array _opt;
		private Uint8Array _tail;

		public dregx_node(MrbParser p, node a, node b)
			: base(p, node_type.NODE_DREGX)
		{
			dump_recur(_a, a);
			_tail = (Uint8Array)((node)b.cdr).car;
			_opt = (Uint8Array)((node)b.cdr).cdr;
		}

		public List<node> a { get { return _a; } }
		public Uint8Array opt { get { return _opt; } }
		public Uint8Array tail { get { return _tail; } }

		public override Element to_xml()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			var str = $"(:dregx . ";
			foreach (var n in a) {
				str += $"{n} ";
			}
			return str + $"{opt} {tail})";
		}
	}

	/* (:backref . n) */
	class back_ref_node : node
	{
		private int _n;

		public back_ref_node(MrbParser p, int n)
			: base(p, node_type.NODE_BACK_REF)
		{
			_n = n;
		}

		public int n { get { return _n; } }

		public override Element to_xml()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return $"(:backref . {n})";
		}
	}

	/* (:nthref . n) */
	class nth_ref_node : node
	{
		private int _n;

		public nth_ref_node(MrbParser p, int n)
			: base(p, node_type.NODE_NTH_REF)
		{
			_n = n;
		}

		public int n { get { return _n; } }

		public override Element to_xml()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return $"(:nthref . {n})";
		}
	}

	/* (:heredoc . a) */
	class heredoc_node : node
	{
		private parser_heredoc_info _info;

		public heredoc_node(MrbParser p)
			: base(p, node_type.NODE_HEREDOC)
		{
			_info = new parser_heredoc_info();
		}

		public parser_heredoc_info info { get { return _info; } }

		public override Element to_xml()
		{
			var block = Document.CreateElement("block");
			block.SetAttribute("type", "text");

			var field = Document.CreateElement("field");
			field.SetAttribute("name", "TEXT");
			field.AppendChild(Document.CreateTextNode(info.GetString()));
			block.AppendChild(field);

			return block;
		}

		public override string ToString()
		{
			return $"(:heredoc . {info})";
		}
	}

	class literal_delim_node : node
	{
		public literal_delim_node(MrbParser p)
			: base(p, node_type.NODE_LITERAL_DELIM)
		{
		}

		public override Element to_xml()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return $"(:literal_delim)";
		}
	}

	/* (:words . a) */
	class words_node : node
	{
		private node _a;

		public words_node(MrbParser p, node a)
			: base(p, node_type.NODE_WORDS)
		{
			_a = a;
		}

		public node a { get { return _a; } }

		public override Element to_xml()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return $"(:words . {a})";
		}
	}

	/* (:symbols . a) */
	class symbols_node : node
	{
		private node _a;

		public symbols_node(MrbParser p, node a)
			: base(p, node_type.NODE_SYMBOLS)
		{
			_a = a;
		}

		public node a { get { return _a; } }

		public override Element to_xml()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return $"(:symbols . {a})";
		}
	}
}
