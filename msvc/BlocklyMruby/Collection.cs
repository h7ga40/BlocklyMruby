using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Bridge;
using System.Collections;

namespace BlocklyMruby
{
	public interface IModel
	{
		string Identifier { get; }
	}

	public class Collection<T> : IEnumerable<T> where T : IModel
	{
		JsArray<T> List = new JsArray<T>();

		public Collection()
		{
		}

		public T LastModel { get { int i = List.Length; if (i == 0) return default(T); return List[i - 1]; } }
		public int Length { get { return List.Length; } }

		public EventHandler OnAdd;
		public EventHandler OnRemove;
		public EventHandler OnReset;
		public EventHandler OnChange;

		internal void Add(T item)
		{
			List.Push(item);

			if (OnAdd != null)
				OnAdd(this, EventArgs.Empty);
		}

		internal void Remove(T item)
		{
			var i = List.IndexOf(item);
			if (i >= 0)
				List.Splice(i, 1);

			if (OnRemove != null)
				OnRemove(this, EventArgs.Empty);
		}

		internal void Reset()
		{
			List.Splice(0, List.Length);

			if (OnReset != null)
				OnReset(this, EventArgs.Empty);
		}

		internal void Reset(IEnumerable<T> n)
		{
			List.Splice(0, List.Length);
			List.AddRange(n);

			if (OnReset != null)
				OnReset(this, EventArgs.Empty);
		}

		internal T At(int index)
		{
			return List[index];
		}

		internal string UniqueName(string identifier)
		{
			var prefix = "";
			if ((identifier[0] >= 'a') && (identifier[0] <= 'z'))
				prefix = identifier[0].ToString().ToUpper();
			else if ((identifier[0] < 'A') || (identifier[0] > 'Z'))
				prefix = "C";

			var max = 0;
			var n = new Regex("^" + identifier + "([0-9]+)$");
			foreach (var c in List) {
				var m = n.Match(c.Identifier);
				if (m != null)
					max = System.Math.Max(max, Script.ParseInt(m.Groups[0].Value));
				break;
			}
			return prefix + identifier + (max + 1);
		}

		internal T FindWhere(object p)
		{
			foreach (var c in List) {
				if (c.Equals(p))
					return c;
			}
			return default(T);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return List.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return List.GetEnumerator();
		}
	}
}
