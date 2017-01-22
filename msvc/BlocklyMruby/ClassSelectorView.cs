using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Bridge;
using Bridge.Html5;
using Bridge.jQuery2;

namespace BlocklyMruby
{
	public class ClassSelectorView : WebConsole
	{
		Collection<IClassWorkspace> m_Collection;
		jQuery el;
		string templateText;
		IClassWorkspace m_ClassWorkspace;

		internal IClassWorkspace Current { get { return m_ClassWorkspace; } }
		private jQuery jQuery;
		private IClassWorkspace m_SelectedClassWorkspace;

		internal event EventHandler<EventArgs> Selected;
		internal event EventHandler<ItemRemovedEventArgs> Removed;
		internal event EventHandler<EventArgs> MarkClicked;

		public ClassSelectorView()
		{
			Open(new ResourceReader("BlocklyMrubyRes"), new ClassSelectorScriptingHost(this));

			Application.Idle += Application_Idle;
		}

		private void Application_Idle(object sender, EventArgs e)
		{
			Application.Idle -= Application_Idle;

			Navigate("class_selector.html");
		}

		protected override void DocumentCompleted(Uri Url)
		{
			if (Url.Scheme == "about")
				return;

			jQuery = new jQuery(Script);

			base.DocumentCompleted(Url);

			if (m_SelectedClassWorkspace != null) {
				SelectClassWorkspace(m_SelectedClassWorkspace);
				m_SelectedClassWorkspace = null;
			}
		}

		internal void SetCollection(Collection<IClassWorkspace> collection)
		{
			el = jQuery.Select("#class-selector-tab");

			m_Collection = collection;
			collection.OnAdd += OnChange;
			collection.OnRemove += OnChange;
			collection.OnReset += OnChange;
			collection.OnChange += OnChange;

			if (collection.Length > 0)
				m_ClassWorkspace = collection.At(0);

			templateText = jQuery.Select("#class-selector-template").Text();
			jQuery.Select("#add-celltype-button").Click(null, OnAddBtnClick);
			Render();
		}

		void Render()
		{
			var charsEl = jQuery.Select("#class-selector-celltype-set");
			charsEl.Children().Remove();
			foreach (var item in m_Collection) {
				var html = jQuery.New(item.Template(templateText));
				charsEl.Append(html);
				var selectedMark = html.Find("a.selected-mark");
				if (m_ClassWorkspace == item)
					selectedMark.Show();
				else
					selectedMark.Hide();
				selectedMark.Click(item, OnSelectedMarkClick);
				html.Find("a.celltype").Click(item, OnSelectBtnClick);
				html.Find("a.modify-button").Click(item, OnModifyBtnClick);
				var removeButton = html.Find("a.remove-button");
				removeButton.Click(item, OnRemoveBtnClick);
				if (item.IsPreset())
					removeButton.Hide();
				var img = html.Find("img");
				img.On("dragstart", new Action<jQueryMouseEvent>((e) => {
					e.PreventDefault();
				}));
			}
		}

		private void OnSelectedMarkClick(jQueryMouseEvent obj)
		{
			MarkClicked?.Invoke(this, EventArgs.Empty);
		}

		internal void SelectClassWorkspace(IClassWorkspace model)
		{
			if (jQuery == null) {
				m_SelectedClassWorkspace = model;
				return;
			}

			jQuery html, selectedMark;
			var charsEl = jQuery.Select("#class-selector-celltype-set");

			if (m_ClassWorkspace != null) {
				html = charsEl.Find("#" + m_ClassWorkspace.Identifier);
				selectedMark = html.Find("a.selected-mark");
				selectedMark.Hide();
				m_ClassWorkspace.Inactivate();
			}

			m_ClassWorkspace = model;
			if ((m_ClassWorkspace == null) && (m_Collection.Length > 0)) {
				m_ClassWorkspace = m_Collection.At(0);
			}

			if (m_ClassWorkspace != null) {
				m_ClassWorkspace.Activate();

				html = charsEl.Find("#" + m_ClassWorkspace.Identifier);
				selectedMark = html.Find("a.selected-mark");
				selectedMark.Show();
			}

			Selected?.Invoke(this, EventArgs.Empty);
		}

		void RemoveClassWorkspace(IClassWorkspace item)
		{
			if (item.IsPreset())
				return;

			if (m_ClassWorkspace == item)
				m_ClassWorkspace = null;

			m_Collection.Remove(item);

			Removed?.Invoke(this, new ItemRemovedEventArgs(item));
		}

		private void OnSelectBtnClick(jQueryMouseEvent e)
		{
			var item = (IClassWorkspace)e.Data;
			e.PreventDefault();
			if (m_ClassWorkspace != item) {
				this.SelectClassWorkspace(item);
			}
		}

		private void OnModifyBtnClick(jQueryMouseEvent e)
		{
			var item = (IClassWorkspace)e.Data;
			e.PreventDefault();
			m_ClassWorkspace = item;
			m_ClassWorkspace.OpenModifyView((ok) => {
				Render();
			});
		}

		private void OnAddBtnClick(jQueryMouseEvent e)
		{
			e.PreventDefault();

			App.NewItem((item) => {
				item.OpenModifyView((ok) => {
					if (ok) {
						m_Collection.Add(item);
						SelectClassWorkspace(item);
					}
					else {
						App.RemoveItem(item);
					}
				});
			});
		}

		private void OnRemoveBtnClick(jQueryMouseEvent e)
		{
			var item = (IClassWorkspace)e.Data;
			e.PreventDefault();
			m_ClassWorkspace = null;
			RemoveClassWorkspace(item);
		}

		void OnChange(object sender, EventArgs e)
		{
			Render();
		}
	}

	class ItemRemovedEventArgs : EventArgs
	{
		public IClassWorkspace Item { get; private set; }

		public ItemRemovedEventArgs(IClassWorkspace item)
		{
			Item = item;
		}
	}

	[System.Runtime.InteropServices.ComVisible(true)]
	public class ClassSelectorScriptingHost : ScriptingHost
	{
		public ClassSelectorScriptingHost(ClassSelectorView view)
			: base(view)
		{
		}
	}
}
