namespace BlocklyMruby
{
	partial class Form1
	{
		/// <summary>
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナーで生成されたコード

		/// <summary>
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.BlocklyWb = new System.Windows.Forms.WebBrowser();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.LoadBtn = new System.Windows.Forms.Button();
			this.SaveBtn = new System.Windows.Forms.Button();
			this.RunBtn = new System.Windows.Forms.Button();
			this.DebugBtn = new System.Windows.Forms.Button();
			this.ExportRubyBtn = new System.Windows.Forms.Button();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.saveFileDialog2 = new System.Windows.Forms.SaveFileDialog();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.BlockTabPage = new System.Windows.Forms.TabPage();
			this.RubyTabPage = new System.Windows.Forms.TabPage();
			this.RubyEditorWb = new System.Windows.Forms.WebBrowser();
			this.ConsoleTabPage = new System.Windows.Forms.TabPage();
			this.ConsoleWb = new System.Windows.Forms.WebBrowser();
			this.flowLayoutPanel1.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.BlockTabPage.SuspendLayout();
			this.RubyTabPage.SuspendLayout();
			this.ConsoleTabPage.SuspendLayout();
			this.SuspendLayout();
			// 
			// BlocklyWb
			// 
			this.BlocklyWb.AllowNavigation = false;
			this.BlocklyWb.AllowWebBrowserDrop = false;
			this.BlocklyWb.Dock = System.Windows.Forms.DockStyle.Fill;
			this.BlocklyWb.Location = new System.Drawing.Point(3, 3);
			this.BlocklyWb.MinimumSize = new System.Drawing.Size(20, 20);
			this.BlocklyWb.Name = "BlocklyWb";
			this.BlocklyWb.Size = new System.Drawing.Size(770, 501);
			this.BlocklyWb.TabIndex = 0;
			this.BlocklyWb.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.BlocklyPage_DocumentCompleted);
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel1.Controls.Add(this.LoadBtn);
			this.flowLayoutPanel1.Controls.Add(this.SaveBtn);
			this.flowLayoutPanel1.Controls.Add(this.RunBtn);
			this.flowLayoutPanel1.Controls.Add(this.DebugBtn);
			this.flowLayoutPanel1.Controls.Add(this.ExportRubyBtn);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(784, 29);
			this.flowLayoutPanel1.TabIndex = 1;
			// 
			// LoadBtn
			// 
			this.LoadBtn.Location = new System.Drawing.Point(3, 3);
			this.LoadBtn.Name = "LoadBtn";
			this.LoadBtn.Size = new System.Drawing.Size(75, 23);
			this.LoadBtn.TabIndex = 0;
			this.LoadBtn.Text = "読み込み";
			this.LoadBtn.UseVisualStyleBackColor = true;
			this.LoadBtn.Click += new System.EventHandler(this.LoadBtn_Click);
			// 
			// SaveBtn
			// 
			this.SaveBtn.Location = new System.Drawing.Point(84, 3);
			this.SaveBtn.Name = "SaveBtn";
			this.SaveBtn.Size = new System.Drawing.Size(75, 23);
			this.SaveBtn.TabIndex = 1;
			this.SaveBtn.Text = "保存";
			this.SaveBtn.UseVisualStyleBackColor = true;
			this.SaveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
			// 
			// RunBtn
			// 
			this.RunBtn.Location = new System.Drawing.Point(165, 3);
			this.RunBtn.Name = "RunBtn";
			this.RunBtn.Size = new System.Drawing.Size(75, 23);
			this.RunBtn.TabIndex = 2;
			this.RunBtn.Text = "実行";
			this.RunBtn.UseVisualStyleBackColor = true;
			this.RunBtn.Click += new System.EventHandler(this.RunBtn_Click);
			// 
			// DebugBtn
			// 
			this.DebugBtn.Location = new System.Drawing.Point(246, 3);
			this.DebugBtn.Name = "DebugBtn";
			this.DebugBtn.Size = new System.Drawing.Size(75, 23);
			this.DebugBtn.TabIndex = 4;
			this.DebugBtn.Text = "デバッグ";
			this.DebugBtn.UseVisualStyleBackColor = true;
			this.DebugBtn.Click += new System.EventHandler(this.debugBtn_Click);
			// 
			// ExportRubyBtn
			// 
			this.ExportRubyBtn.Location = new System.Drawing.Point(327, 3);
			this.ExportRubyBtn.Name = "ExportRubyBtn";
			this.ExportRubyBtn.Size = new System.Drawing.Size(75, 23);
			this.ExportRubyBtn.TabIndex = 3;
			this.ExportRubyBtn.Text = "Rubyで保存";
			this.ExportRubyBtn.UseVisualStyleBackColor = true;
			this.ExportRubyBtn.Click += new System.EventHandler(this.ExportRubyBtn_Click);
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.DefaultExt = "xml";
			this.openFileDialog1.FileName = "workspace.xml";
			this.openFileDialog1.Filter = "xml ファイル|*.xml|すべてのファイル|*.*";
			// 
			// saveFileDialog1
			// 
			this.saveFileDialog1.DefaultExt = "xml";
			this.saveFileDialog1.FileName = "workspace.xml";
			this.saveFileDialog1.Filter = "xml ファイル|*.xml|すべてのファイル|*.*";
			// 
			// saveFileDialog2
			// 
			this.saveFileDialog2.DefaultExt = "rb";
			this.saveFileDialog2.FileName = "workspace.rb";
			this.saveFileDialog2.Filter = "Ruby ファイル|*.rb|すべてのファイル|*.*";
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.BlockTabPage);
			this.tabControl1.Controls.Add(this.RubyTabPage);
			this.tabControl1.Controls.Add(this.ConsoleTabPage);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 29);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(784, 533);
			this.tabControl1.TabIndex = 2;
			this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
			// 
			// BlockTabPage
			// 
			this.BlockTabPage.Controls.Add(this.BlocklyWb);
			this.BlockTabPage.Location = new System.Drawing.Point(4, 22);
			this.BlockTabPage.Name = "BlockTabPage";
			this.BlockTabPage.Padding = new System.Windows.Forms.Padding(3);
			this.BlockTabPage.Size = new System.Drawing.Size(776, 507);
			this.BlockTabPage.TabIndex = 0;
			this.BlockTabPage.Text = "ブロック";
			this.BlockTabPage.UseVisualStyleBackColor = true;
			// 
			// RubyTabPage
			// 
			this.RubyTabPage.Controls.Add(this.RubyEditorWb);
			this.RubyTabPage.Location = new System.Drawing.Point(4, 22);
			this.RubyTabPage.Name = "RubyTabPage";
			this.RubyTabPage.Padding = new System.Windows.Forms.Padding(3);
			this.RubyTabPage.Size = new System.Drawing.Size(776, 507);
			this.RubyTabPage.TabIndex = 2;
			this.RubyTabPage.Text = "Ruby";
			this.RubyTabPage.UseVisualStyleBackColor = true;
			// 
			// RubyEditorWb
			// 
			this.RubyEditorWb.AllowNavigation = false;
			this.RubyEditorWb.AllowWebBrowserDrop = false;
			this.RubyEditorWb.Dock = System.Windows.Forms.DockStyle.Fill;
			this.RubyEditorWb.Location = new System.Drawing.Point(3, 3);
			this.RubyEditorWb.MinimumSize = new System.Drawing.Size(20, 20);
			this.RubyEditorWb.Name = "RubyEditorWb";
			this.RubyEditorWb.Size = new System.Drawing.Size(770, 501);
			this.RubyEditorWb.TabIndex = 0;
			this.RubyEditorWb.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.RubyEditorPage_DocumentCompleted);
			// 
			// ConsoleTabPage
			// 
			this.ConsoleTabPage.Controls.Add(this.ConsoleWb);
			this.ConsoleTabPage.Location = new System.Drawing.Point(4, 22);
			this.ConsoleTabPage.Name = "ConsoleTabPage";
			this.ConsoleTabPage.Padding = new System.Windows.Forms.Padding(3);
			this.ConsoleTabPage.Size = new System.Drawing.Size(776, 507);
			this.ConsoleTabPage.TabIndex = 1;
			this.ConsoleTabPage.Text = "コンソール";
			this.ConsoleTabPage.UseVisualStyleBackColor = true;
			// 
			// ConsoleWb
			// 
			this.ConsoleWb.AllowNavigation = false;
			this.ConsoleWb.AllowWebBrowserDrop = false;
			this.ConsoleWb.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ConsoleWb.Location = new System.Drawing.Point(3, 3);
			this.ConsoleWb.MinimumSize = new System.Drawing.Size(20, 20);
			this.ConsoleWb.Name = "ConsoleWb";
			this.ConsoleWb.Size = new System.Drawing.Size(770, 501);
			this.ConsoleWb.TabIndex = 0;
			this.ConsoleWb.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.ConsolePage_DocumentCompleted);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(784, 562);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.flowLayoutPanel1);
			this.KeyPreview = true;
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "BlocklyMruby";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.BlockTabPage.ResumeLayout(false);
			this.RubyTabPage.ResumeLayout(false);
			this.ConsoleTabPage.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.WebBrowser BlocklyWb;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Button LoadBtn;
		private System.Windows.Forms.Button SaveBtn;
		private System.Windows.Forms.Button RunBtn;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.Button ExportRubyBtn;
		private System.Windows.Forms.SaveFileDialog saveFileDialog2;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage BlockTabPage;
		private System.Windows.Forms.TabPage ConsoleTabPage;
		private System.Windows.Forms.WebBrowser ConsoleWb;
		private System.Windows.Forms.Button DebugBtn;
		private System.Windows.Forms.TabPage RubyTabPage;
		private System.Windows.Forms.WebBrowser RubyEditorWb;
	}
}

