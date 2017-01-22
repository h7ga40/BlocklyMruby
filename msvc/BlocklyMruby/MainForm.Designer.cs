namespace BlocklyMruby
{
	partial class MainForm
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
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.LoadBtn = new System.Windows.Forms.Button();
			this.SaveBtn = new System.Windows.Forms.Button();
			this.RunBtn = new System.Windows.Forms.Button();
			this.DebugBtn = new System.Windows.Forms.Button();
			this.ExportRubyBtn = new System.Windows.Forms.Button();
			this.DebugRunBtn = new System.Windows.Forms.Button();
			this.StepBtn = new System.Windows.Forms.Button();
			this.ContinueBtn = new System.Windows.Forms.Button();
			this.BreakBtn = new System.Windows.Forms.Button();
			this.QuitBtn = new System.Windows.Forms.Button();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.saveFileDialog2 = new System.Windows.Forms.SaveFileDialog();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.RubyTabPage = new System.Windows.Forms.TabPage();
			this.BlockTabPage = new System.Windows.Forms.TabPage();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this.aceView1 = new BlocklyMruby.AceView();
			this.blocklyView1 = new BlocklyMruby.BlocklyView();
			this.classSelectorView1 = new BlocklyMruby.ClassSelectorView();
			this.xTermView1 = new BlocklyMruby.XTermView();
			this.flowLayoutPanel1.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.RubyTabPage.SuspendLayout();
			this.BlockTabPage.SuspendLayout();
			this.SuspendLayout();
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
			this.flowLayoutPanel1.Controls.Add(this.DebugRunBtn);
			this.flowLayoutPanel1.Controls.Add(this.StepBtn);
			this.flowLayoutPanel1.Controls.Add(this.ContinueBtn);
			this.flowLayoutPanel1.Controls.Add(this.BreakBtn);
			this.flowLayoutPanel1.Controls.Add(this.QuitBtn);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(984, 28);
			this.flowLayoutPanel1.TabIndex = 1;
			// 
			// LoadBtn
			// 
			this.LoadBtn.AutoSize = true;
			this.LoadBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.LoadBtn.Location = new System.Drawing.Point(3, 3);
			this.LoadBtn.Name = "LoadBtn";
			this.LoadBtn.Size = new System.Drawing.Size(61, 22);
			this.LoadBtn.TabIndex = 0;
			this.LoadBtn.Text = "読み込み";
			this.LoadBtn.UseVisualStyleBackColor = true;
			this.LoadBtn.Click += new System.EventHandler(this.LoadBtn_Click);
			// 
			// SaveBtn
			// 
			this.SaveBtn.AutoSize = true;
			this.SaveBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.SaveBtn.Location = new System.Drawing.Point(70, 3);
			this.SaveBtn.Name = "SaveBtn";
			this.SaveBtn.Size = new System.Drawing.Size(39, 22);
			this.SaveBtn.TabIndex = 1;
			this.SaveBtn.Text = "保存";
			this.SaveBtn.UseVisualStyleBackColor = true;
			this.SaveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
			// 
			// RunBtn
			// 
			this.RunBtn.AutoSize = true;
			this.RunBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.RunBtn.Location = new System.Drawing.Point(115, 3);
			this.RunBtn.Name = "RunBtn";
			this.RunBtn.Size = new System.Drawing.Size(39, 22);
			this.RunBtn.TabIndex = 2;
			this.RunBtn.Text = "実行";
			this.RunBtn.UseVisualStyleBackColor = true;
			this.RunBtn.Click += new System.EventHandler(this.RunBtn_Click);
			// 
			// DebugBtn
			// 
			this.DebugBtn.AutoSize = true;
			this.DebugBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.DebugBtn.Location = new System.Drawing.Point(160, 3);
			this.DebugBtn.Name = "DebugBtn";
			this.DebugBtn.Size = new System.Drawing.Size(51, 22);
			this.DebugBtn.TabIndex = 4;
			this.DebugBtn.Text = "デバッグ";
			this.DebugBtn.UseVisualStyleBackColor = true;
			this.DebugBtn.Click += new System.EventHandler(this.debugBtn_Click);
			// 
			// ExportRubyBtn
			// 
			this.ExportRubyBtn.AutoSize = true;
			this.ExportRubyBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ExportRubyBtn.Location = new System.Drawing.Point(217, 3);
			this.ExportRubyBtn.Name = "ExportRubyBtn";
			this.ExportRubyBtn.Size = new System.Drawing.Size(75, 22);
			this.ExportRubyBtn.TabIndex = 3;
			this.ExportRubyBtn.Text = "Rubyで保存";
			this.ExportRubyBtn.UseVisualStyleBackColor = true;
			this.ExportRubyBtn.Click += new System.EventHandler(this.ExportRubyBtn_Click);
			// 
			// DebugRunBtn
			// 
			this.DebugRunBtn.AutoSize = true;
			this.DebugRunBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.DebugRunBtn.Location = new System.Drawing.Point(298, 3);
			this.DebugRunBtn.Name = "DebugRunBtn";
			this.DebugRunBtn.Size = new System.Drawing.Size(39, 22);
			this.DebugRunBtn.TabIndex = 7;
			this.DebugRunBtn.Text = "実行";
			this.DebugRunBtn.UseVisualStyleBackColor = true;
			this.DebugRunBtn.Click += new System.EventHandler(this.DebugRunBtn_Click);
			// 
			// StepBtn
			// 
			this.StepBtn.AutoSize = true;
			this.StepBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.StepBtn.Location = new System.Drawing.Point(343, 3);
			this.StepBtn.Name = "StepBtn";
			this.StepBtn.Size = new System.Drawing.Size(49, 22);
			this.StepBtn.TabIndex = 5;
			this.StepBtn.Text = "ステップ";
			this.StepBtn.UseVisualStyleBackColor = true;
			this.StepBtn.Click += new System.EventHandler(this.StepBtn_Click);
			// 
			// ContinueBtn
			// 
			this.ContinueBtn.AutoSize = true;
			this.ContinueBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ContinueBtn.Location = new System.Drawing.Point(398, 3);
			this.ContinueBtn.Name = "ContinueBtn";
			this.ContinueBtn.Size = new System.Drawing.Size(39, 22);
			this.ContinueBtn.TabIndex = 6;
			this.ContinueBtn.Text = "継続";
			this.ContinueBtn.UseVisualStyleBackColor = true;
			this.ContinueBtn.Click += new System.EventHandler(this.ContinueBtn_Click);
			// 
			// BreakBtn
			// 
			this.BreakBtn.AutoSize = true;
			this.BreakBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BreakBtn.Location = new System.Drawing.Point(443, 3);
			this.BreakBtn.Name = "BreakBtn";
			this.BreakBtn.Size = new System.Drawing.Size(39, 22);
			this.BreakBtn.TabIndex = 8;
			this.BreakBtn.Text = "停止";
			this.BreakBtn.UseVisualStyleBackColor = true;
			this.BreakBtn.Click += new System.EventHandler(this.BreakBtn_Click);
			// 
			// QuitBtn
			// 
			this.QuitBtn.AutoSize = true;
			this.QuitBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.QuitBtn.Location = new System.Drawing.Point(488, 3);
			this.QuitBtn.Name = "QuitBtn";
			this.QuitBtn.Size = new System.Drawing.Size(39, 22);
			this.QuitBtn.TabIndex = 9;
			this.QuitBtn.Text = "終了";
			this.QuitBtn.UseVisualStyleBackColor = true;
			this.QuitBtn.Click += new System.EventHandler(this.QuitBtn_Click);
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
			this.tabControl1.Controls.Add(this.RubyTabPage);
			this.tabControl1.Controls.Add(this.BlockTabPage);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(203, 28);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(781, 514);
			this.tabControl1.TabIndex = 2;
			this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
			// 
			// RubyTabPage
			// 
			this.RubyTabPage.Controls.Add(this.aceView1);
			this.RubyTabPage.Location = new System.Drawing.Point(4, 22);
			this.RubyTabPage.Name = "RubyTabPage";
			this.RubyTabPage.Size = new System.Drawing.Size(773, 488);
			this.RubyTabPage.TabIndex = 2;
			this.RubyTabPage.Text = "Ruby";
			this.RubyTabPage.UseVisualStyleBackColor = true;
			// 
			// BlockTabPage
			// 
			this.BlockTabPage.Controls.Add(this.blocklyView1);
			this.BlockTabPage.Location = new System.Drawing.Point(4, 22);
			this.BlockTabPage.Name = "BlockTabPage";
			this.BlockTabPage.Size = new System.Drawing.Size(773, 488);
			this.BlockTabPage.TabIndex = 0;
			this.BlockTabPage.Text = "ブロック";
			this.BlockTabPage.UseVisualStyleBackColor = true;
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(200, 28);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 514);
			this.splitter1.TabIndex = 4;
			this.splitter1.TabStop = false;
			// 
			// splitter2
			// 
			this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter2.Location = new System.Drawing.Point(0, 542);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(984, 3);
			this.splitter2.TabIndex = 5;
			this.splitter2.TabStop = false;
			// 
			// aceView1
			// 
			this.aceView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.aceView1.Location = new System.Drawing.Point(0, 0);
			this.aceView1.Name = "aceView1";
			this.aceView1.Size = new System.Drawing.Size(773, 488);
			this.aceView1.TabIndex = 0;
			// 
			// blocklyView1
			// 
			this.blocklyView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.blocklyView1.Location = new System.Drawing.Point(0, 0);
			this.blocklyView1.Name = "blocklyView1";
			this.blocklyView1.Size = new System.Drawing.Size(773, 488);
			this.blocklyView1.TabIndex = 0;
			// 
			// classSelectorView1
			// 
			this.classSelectorView1.Dock = System.Windows.Forms.DockStyle.Left;
			this.classSelectorView1.Location = new System.Drawing.Point(0, 28);
			this.classSelectorView1.Name = "classSelectorView1";
			this.classSelectorView1.Size = new System.Drawing.Size(200, 514);
			this.classSelectorView1.TabIndex = 3;
			// 
			// xTermView1
			// 
			this.xTermView1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.xTermView1.Location = new System.Drawing.Point(0, 545);
			this.xTermView1.Name = "xTermView1";
			this.xTermView1.Size = new System.Drawing.Size(984, 160);
			this.xTermView1.TabIndex = 0;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(984, 705);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.classSelectorView1);
			this.Controls.Add(this.splitter2);
			this.Controls.Add(this.xTermView1);
			this.Controls.Add(this.flowLayoutPanel1);
			this.KeyPreview = true;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "BlocklyMruby";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.tabControl1.ResumeLayout(false);
			this.RubyTabPage.ResumeLayout(false);
			this.BlockTabPage.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
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
		private System.Windows.Forms.Button DebugBtn;
		private System.Windows.Forms.TabPage RubyTabPage;
		private System.Windows.Forms.Button StepBtn;
		private System.Windows.Forms.Button ContinueBtn;
		private System.Windows.Forms.Button DebugRunBtn;
		private System.Windows.Forms.Button BreakBtn;
		private System.Windows.Forms.Button QuitBtn;
		private BlocklyView blocklyView1;
		private AceView aceView1;
		private XTermView xTermView1;
		private ClassSelectorView classSelectorView1;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Splitter splitter2;
	}
}

