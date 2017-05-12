
// This file has been generated by the GUI designer. Do not modify.
namespace osrepodbmgr
{
	public partial class frmMain
	{
		private global::Gtk.VBox vbox2;

		private global::Gtk.Frame frame1;

		private global::Gtk.Alignment GtkAlignment;

		private global::Gtk.ScrolledWindow GtkScrolledWindow;

		private global::Gtk.TreeView treeOSes;

		private global::Gtk.Label lblOSes;

		private global::Gtk.HBox hbox2;

		private global::Gtk.Label lblProgress;

		private global::Gtk.ProgressBar prgProgress;

		private global::Gtk.HBox hbox3;

		private global::Gtk.Label lblProgress2;

		private global::Gtk.ProgressBar prgProgress2;

		private global::Gtk.HBox hbox1;

		private global::Gtk.Button btnAdd;

		private global::Gtk.Button btnRemove;

		private global::Gtk.Button btnQuit;

		private global::Gtk.Button btnSettings;

		private global::Gtk.Button btnHelp;

		private global::Gtk.Button btnSave;

		private global::Gtk.Button btnCompress;

		private global::Gtk.Button btnStop;

		protected virtual void Build()
		{
			global::Stetic.Gui.Initialize(this);
			// Widget osrepodbmgr.frmMain
			this.Name = "osrepodbmgr.frmMain";
			this.Title = global::Mono.Unix.Catalog.GetString("OS Repository DB Manager");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Container child osrepodbmgr.frmMain.Gtk.Container+ContainerChild
			this.vbox2 = new global::Gtk.VBox();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			// Container child vbox2.Gtk.Box+BoxChild
			this.frame1 = new global::Gtk.Frame();
			this.frame1.Name = "frame1";
			this.frame1.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child frame1.Gtk.Container+ContainerChild
			this.GtkAlignment = new global::Gtk.Alignment(0F, 0F, 1F, 1F);
			this.GtkAlignment.Name = "GtkAlignment";
			this.GtkAlignment.LeftPadding = ((uint)(12));
			// Container child GtkAlignment.Gtk.Container+ContainerChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.treeOSes = new global::Gtk.TreeView();
			this.treeOSes.Sensitive = false;
			this.treeOSes.CanFocus = true;
			this.treeOSes.Name = "treeOSes";
			this.GtkScrolledWindow.Add(this.treeOSes);
			this.GtkAlignment.Add(this.GtkScrolledWindow);
			this.frame1.Add(this.GtkAlignment);
			this.lblOSes = new global::Gtk.Label();
			this.lblOSes.Name = "lblOSes";
			this.lblOSes.LabelProp = global::Mono.Unix.Catalog.GetString("<b>Operating systems</b>");
			this.lblOSes.UseMarkup = true;
			this.frame1.LabelWidget = this.lblOSes;
			this.vbox2.Add(this.frame1);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.frame1]));
			w4.Position = 0;
			// Container child vbox2.Gtk.Box+BoxChild
			this.hbox2 = new global::Gtk.HBox();
			this.hbox2.Name = "hbox2";
			this.hbox2.Spacing = 6;
			// Container child hbox2.Gtk.Box+BoxChild
			this.lblProgress = new global::Gtk.Label();
			this.lblProgress.Name = "lblProgress";
			this.lblProgress.LabelProp = global::Mono.Unix.Catalog.GetString("label1");
			this.hbox2.Add(this.lblProgress);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.hbox2[this.lblProgress]));
			w5.Position = 0;
			w5.Expand = false;
			w5.Fill = false;
			// Container child hbox2.Gtk.Box+BoxChild
			this.prgProgress = new global::Gtk.ProgressBar();
			this.prgProgress.Name = "prgProgress";
			this.hbox2.Add(this.prgProgress);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.hbox2[this.prgProgress]));
			w6.Position = 1;
			this.vbox2.Add(this.hbox2);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.hbox2]));
			w7.Position = 1;
			w7.Expand = false;
			w7.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.hbox3 = new global::Gtk.HBox();
			this.hbox3.Name = "hbox3";
			this.hbox3.Spacing = 6;
			// Container child hbox3.Gtk.Box+BoxChild
			this.lblProgress2 = new global::Gtk.Label();
			this.lblProgress2.Name = "lblProgress2";
			this.lblProgress2.LabelProp = global::Mono.Unix.Catalog.GetString("label2");
			this.hbox3.Add(this.lblProgress2);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.hbox3[this.lblProgress2]));
			w8.Position = 0;
			w8.Expand = false;
			w8.Fill = false;
			// Container child hbox3.Gtk.Box+BoxChild
			this.prgProgress2 = new global::Gtk.ProgressBar();
			this.prgProgress2.Name = "prgProgress2";
			this.hbox3.Add(this.prgProgress2);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.hbox3[this.prgProgress2]));
			w9.Position = 1;
			this.vbox2.Add(this.hbox3);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.hbox3]));
			w10.Position = 2;
			w10.Expand = false;
			w10.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.btnAdd = new global::Gtk.Button();
			this.btnAdd.CanFocus = true;
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.UseStock = true;
			this.btnAdd.UseUnderline = true;
			this.btnAdd.Label = "gtk-add";
			this.hbox1.Add(this.btnAdd);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.btnAdd]));
			w11.Position = 0;
			w11.Expand = false;
			w11.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.btnRemove = new global::Gtk.Button();
			this.btnRemove.CanFocus = true;
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.UseStock = true;
			this.btnRemove.UseUnderline = true;
			this.btnRemove.Label = "gtk-remove";
			this.hbox1.Add(this.btnRemove);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.btnRemove]));
			w12.Position = 1;
			w12.Expand = false;
			w12.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.btnQuit = new global::Gtk.Button();
			this.btnQuit.CanFocus = true;
			this.btnQuit.Name = "btnQuit";
			this.btnQuit.UseStock = true;
			this.btnQuit.UseUnderline = true;
			this.btnQuit.Label = "gtk-quit";
			this.hbox1.Add(this.btnQuit);
			global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.btnQuit]));
			w13.PackType = ((global::Gtk.PackType)(1));
			w13.Position = 2;
			w13.Expand = false;
			w13.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.btnSettings = new global::Gtk.Button();
			this.btnSettings.CanFocus = true;
			this.btnSettings.Name = "btnSettings";
			this.btnSettings.UseUnderline = true;
			this.btnSettings.Label = global::Mono.Unix.Catalog.GetString("_Settings");
			global::Gtk.Image w14 = new global::Gtk.Image();
			w14.Pixbuf = global::Stetic.IconLoader.LoadIcon(this, "gtk-preferences", global::Gtk.IconSize.Menu);
			this.btnSettings.Image = w14;
			this.hbox1.Add(this.btnSettings);
			global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.btnSettings]));
			w15.PackType = ((global::Gtk.PackType)(1));
			w15.Position = 3;
			w15.Expand = false;
			w15.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.btnHelp = new global::Gtk.Button();
			this.btnHelp.CanFocus = true;
			this.btnHelp.Name = "btnHelp";
			this.btnHelp.UseStock = true;
			this.btnHelp.UseUnderline = true;
			this.btnHelp.Label = "gtk-help";
			this.hbox1.Add(this.btnHelp);
			global::Gtk.Box.BoxChild w16 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.btnHelp]));
			w16.PackType = ((global::Gtk.PackType)(1));
			w16.Position = 4;
			w16.Expand = false;
			w16.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.btnSave = new global::Gtk.Button();
			this.btnSave.CanFocus = true;
			this.btnSave.Name = "btnSave";
			this.btnSave.UseUnderline = true;
			this.btnSave.Label = global::Mono.Unix.Catalog.GetString("Save _As");
			global::Gtk.Image w17 = new global::Gtk.Image();
			w17.Pixbuf = global::Stetic.IconLoader.LoadIcon(this, "gtk-directory", global::Gtk.IconSize.Menu);
			this.btnSave.Image = w17;
			this.hbox1.Add(this.btnSave);
			global::Gtk.Box.BoxChild w18 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.btnSave]));
			w18.PackType = ((global::Gtk.PackType)(1));
			w18.Position = 5;
			w18.Expand = false;
			w18.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.btnCompress = new global::Gtk.Button();
			this.btnCompress.CanFocus = true;
			this.btnCompress.Name = "btnCompress";
			this.btnCompress.UseUnderline = true;
			this.btnCompress.Label = global::Mono.Unix.Catalog.GetString("Compress to");
			global::Gtk.Image w19 = new global::Gtk.Image();
			w19.Pixbuf = global::Stetic.IconLoader.LoadIcon(this, "gtk-save", global::Gtk.IconSize.Menu);
			this.btnCompress.Image = w19;
			this.hbox1.Add(this.btnCompress);
			global::Gtk.Box.BoxChild w20 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.btnCompress]));
			w20.PackType = ((global::Gtk.PackType)(1));
			w20.Position = 6;
			w20.Expand = false;
			w20.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.btnStop = new global::Gtk.Button();
			this.btnStop.CanFocus = true;
			this.btnStop.Name = "btnStop";
			this.btnStop.UseStock = true;
			this.btnStop.UseUnderline = true;
			this.btnStop.Label = "gtk-stop";
			this.hbox1.Add(this.btnStop);
			global::Gtk.Box.BoxChild w21 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.btnStop]));
			w21.PackType = ((global::Gtk.PackType)(1));
			w21.Position = 7;
			w21.Expand = false;
			w21.Fill = false;
			this.vbox2.Add(this.hbox1);
			global::Gtk.Box.BoxChild w22 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.hbox1]));
			w22.Position = 3;
			w22.Expand = false;
			w22.Fill = false;
			this.Add(this.vbox2);
			if((this.Child != null))
			{
				this.Child.ShowAll();
			}
			this.DefaultWidth = 612;
			this.DefaultHeight = 365;
			this.lblProgress2.Hide();
			this.prgProgress2.Hide();
			this.btnAdd.Hide();
			this.btnRemove.Hide();
			this.btnSettings.Hide();
			this.btnHelp.Hide();
			this.btnSave.Hide();
			this.btnStop.Hide();
			this.Show();
			this.DeleteEvent += new global::Gtk.DeleteEventHandler(this.OnDeleteEvent);
			this.btnAdd.Clicked += new global::System.EventHandler(this.OnBtnAddClicked);
			this.btnRemove.Clicked += new global::System.EventHandler(this.OnBtnRemoveClicked);
			this.btnStop.Clicked += new global::System.EventHandler(this.OnBtnStopClicked);
			this.btnCompress.Clicked += new global::System.EventHandler(this.OnBtnCompressClicked);
			this.btnSave.Clicked += new global::System.EventHandler(this.OnBtnSaveClicked);
			this.btnHelp.Clicked += new global::System.EventHandler(this.OnBtnHelpClicked);
			this.btnSettings.Clicked += new global::System.EventHandler(this.OnBtnSettingsClicked);
			this.btnQuit.Clicked += new global::System.EventHandler(this.OnBtnQuitClicked);
		}
	}
}
