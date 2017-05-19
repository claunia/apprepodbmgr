
// This file has been generated by the GUI designer. Do not modify.
namespace osrepodbmgr
{
	public partial class dlgHelp
	{
		private global::Gtk.ScrolledWindow GtkScrolledWindow;

		private global::Gtk.TextView txtHelp;

		private global::Gtk.Button btnOK;

		private global::Gtk.Button btnDialog;

		protected virtual void Build()
		{
			global::Stetic.Gui.Initialize(this);
			// Widget osrepodbmgr.dlgHelp
			this.Name = "osrepodbmgr.dlgHelp";
			this.Title = global::Mono.Unix.Catalog.GetString("Help");
			this.TypeHint = ((global::Gdk.WindowTypeHint)(1));
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Internal child osrepodbmgr.dlgHelp.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "vbox4";
			w1.Spacing = 6;
			// Container child vbox4.Gtk.Box+BoxChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.txtHelp = new global::Gtk.TextView();
			this.txtHelp.Buffer.Text = "This is the naming scheme and folder organization conventions for the Operating System Repository.\n\nThe basic layout is as follows:\n\n<Developer>/<Product>/<Version>/<Language>/<Architecture>/oem/for <Machine>/<[format]_update/upgrade/files/source/netinstall/description>.zip\nAll fields should contain only 7-bit ASCII.\n\n<Developer>\n-----------\nThis is the main developer of the operating system or associated software. e.g. Microsoft\n\n<Product>\n---------\nThis is the name of the operating system or associated software. e.g. Windows NT\n\n<Version>\n---------\nThis is the version of the operating system or associated software. e.g. 6.00.6000.16386\nService pack and release markers should be appended. e.g. 6.10.7601.16385 (RTM)\nBuild can be specified by appending \"build\". e.g. 10.2.7 build 6S80\nAnd combined. e.g. 10.5 build 9A581 (Server)\nVersion with same version number but different build date should have it appended. Date format should be YYYYmm[dd]. e.g. 10 201009\n\n<Language>\n----------\nThis specifies the language localization and translation:\nxxx: Language variation, e.g. German = deu\nxxx_yy: Country specific language variation. e.g. Swiss German = deu_ch\nmulti: The only known variation of the product that contains more than a language\nxxx,xxx,xxx and xxx_yy,xxx_yy,xxx_yy: The variation contains more than a single language\nWhere xxx is the ISO-639-2/T language code and yy is the ISO-3166-1 alpha-2 country code.\nhttps://en.wikipedia.org/wiki/List_of_ISO_639-1_codes\nhttps://en.wikipedia.org/wiki/ISO_3166-1_alpha-2\nIf the product has ever been only released in one language variation, being it English (like CP/M) or multilanguage (like Mac OS X and Linux), this field should be omitted.\n\n<Architecture>\n--------------\nThe processor architecture the product is compiled for.\nOmitted if it has only ever been compiled for a single one, contains all supported ones in same files, or it is source code containing support for several of them.\nExact one depends on the product (that is, the one that the product uses to identify itself, should be used). Examples:\n* x86, i86, i386, i486, i586, i686, ia32: Intel Architecture 32 aka 8086 aka iAPX86\n* x64, amd64, x86_64: AMD64 aka x86_64 aka EM64T\n* ia64: Intel Architecture 64 aka Itanium\n* sparc: SPARC\n* sun4u, sun4m, sun1, sun2, sun3: Specific Sun architectures that specify not only the processor architecture but the whole system one.\n* 68k, ppc, fat: For products that run under Mac OS indicate they require a Macintosh, a Power Macintosh, or can run on both, respectively.\n* rpi, rpi2, beaglebone, bananapi: Specific whole systems that share a processor architecture but require a completely different variant.\n\noem\n---\nPresent if the variant is OEM. Omitted otherwise.\n\nfor <Machine>\n-------------\nPresent if the variant requires a specific computer to run.\nUseful for computer restoration variants.\ne.g. for Power Mac 5200\ne.g. for Tandy 1000\n\n<[format]/update/upgrade/files/source/netinstall/description>.zip\n-----------------------------------------------------------------\nThis is the file containing the product itself.\nIt should be compressed using ZIP with certain parameters (see below).\nSeveral of them can be combined separated with underscores.\nNaming as following:\n* [format]: If the variation is encoded in a disk image format that's neither a simple dump of sectors (.iso/.dsk/.xdf) or a BinCue/BinToc (.bin+.cue/.bin+.toc) format should be substituted to a descriptive name for the format. e.g.: [Nero], [CloneCD]\n* update: Should be used when the product requires and updates a previous point release or build to the new one. Product version should be the updated, not the required, one. e.g.: 1.3 updates to 1.3.1 or 2.0 updates to 2.5\n* upgrade: Should be used when the product requires and updates a previous version to the new one. Product version should be the updated, not the required, one. e.g.: 2.0 updates to 3.0 or MS-DOS updates to Windows 95.\n* files: Should be used when the contents of the product disks are dumped as is (copied from the media) or it contains already installed files.\n* source: Should be used when it contains the source code for the product.\n* netinstall: Similar to files except that the files are designed to be put in a network share for remote installation of the product.\n* description: Free form description or product part number if it is known.\n\nCompression\n-----------\nThe product should be compressed using ZIP with Deflate algorithm and UTF-8 headers. Zip64 extensions may be used. UNIX extensions MUST be used for products that require them (e.g. it contains softlinks).\nIn the doubt, use Info-ZIP with following parameters:\nzip -9ry -dd archive.zip .\nIf the product requires Acorn, BeOS or OS/2 extended attributes it should be compressed using the corresponding Info-ZIP version under that operating system so the required extension is used.\nDO NOT recompress archives in an operating system which zip product doesn't support all already present headers.\nDO NOT use TorrentZip. It discards all headers as well as date stamps.\nDO NOT use Mac OS headers. For conserving FinderInfo and Resource Fork see below.\n\nFinderInfo and Resource Fork\n----------------------------\nFinderInfo and Resource Fork should be stored as Mac OS X AppleDouble format: file and ._file\nThis format is understand by all Mac OS X versions under any filesystem or a CIFS/SMB network share.\nAlso mkisofs recognizes it and is able to create an HFS partition with them correctly set.\nOther formats should be converted to this one.\n\nMetadata\n--------\nEach archive should be accompanied with a JSON metadata file using the CICM Metadata format. Name for metadata sidecar should be same as the archive changing the extension to .json.\nIf the archive can be modified (doesn't contain ZIP headers you would lose) the metadata should be put inside the archive as a file named metadata.json.\n\nRecovery\n--------\nDisks fail, bit rot happens, so every archive as well as the metadata file should have a PAR2 recovery set created.\nExample command line (with 5% redundancy):\npar2 c -r5 archive.par2 archive.zip archive.json\n\nResult\n------\nIn the end you get something like this:\nApple/Mac OS/9.1/eng/for iMac (Early 2001) v1.1/archive.json\nApple/Mac OS/9.1/eng/for iMac (Early 2001) v1.1/archive.par2\nApple/Mac OS/9.1/eng/for iMac (Early 2001) v1.1/archive.vol000+01.par2\nApple/Mac OS/9.1/eng/for iMac (Early 2001) v1.1/archive.vol001+02.par2\nApple/Mac OS/9.1/eng/for iMac (Early 2001) v1.1/archive.vol003+04.par2\nApple/Mac OS/9.1/eng/for iMac (Early 2001) v1.1/archive.vol007+08.par2\nApple/Mac OS/9.1/eng/for iMac (Early 2001) v1.1/archive.vol015+16.par2\nApple/Mac OS/9.1/eng/for iMac (Early 2001) v1.1/archive.vol031+32.par2\nApple/Mac OS/9.1/eng/for iMac (Early 2001) v1.1/archive.vol063+37.par2\nApple/Mac OS/9.1/eng/for iMac (Early 2001) v1.1/archive.zip\nQNX/QNX/20090229/source.json\nQNX/QNX/20090229/source.par2\nQNX/QNX/20090229/source.vol000+01.par2\nQNX/QNX/20090229/source.vol001+02.par2\nQNX/QNX/20090229/source.vol003+04.par2\nQNX/QNX/20090229/source.vol007+08.par2\nQNX/QNX/20090229/source.vol015+16.par2\nQNX/QNX/20090229/source.vol031+32.par2\nQNX/QNX/20090229/source.vol063+37.par2\nQNX/QNX/20090229/source.zip";
			this.txtHelp.CanFocus = true;
			this.txtHelp.Name = "txtHelp";
			this.txtHelp.Editable = false;
			this.GtkScrolledWindow.Add(this.txtHelp);
			w1.Add(this.GtkScrolledWindow);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(w1[this.GtkScrolledWindow]));
			w3.Position = 0;
			// Container child vbox4.Gtk.Box+BoxChild
			this.btnOK = new global::Gtk.Button();
			this.btnOK.CanFocus = true;
			this.btnOK.Name = "btnOK";
			this.btnOK.UseStock = true;
			this.btnOK.UseUnderline = true;
			this.btnOK.Label = "gtk-ok";
			w1.Add(this.btnOK);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(w1[this.btnOK]));
			w4.Position = 1;
			w4.Expand = false;
			w4.Fill = false;
			// Internal child osrepodbmgr.dlgHelp.ActionArea
			global::Gtk.HButtonBox w5 = this.ActionArea;
			w5.Name = "__gtksharp_108_Stetic_TopLevelDialog_ActionArea";
			// Container child __gtksharp_108_Stetic_TopLevelDialog_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.btnDialog = new global::Gtk.Button();
			this.btnDialog.CanFocus = true;
			this.btnDialog.Name = "btnDialog";
			this.btnDialog.UseUnderline = true;
			this.btnDialog.Label = global::Mono.Unix.Catalog.GetString("GtkButton");
			this.AddActionWidget(this.btnDialog, 0);
			global::Gtk.ButtonBox.ButtonBoxChild w6 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w5[this.btnDialog]));
			w6.Expand = false;
			w6.Fill = false;
			if ((this.Child != null))
			{
				this.Child.ShowAll();
			}
			this.DefaultWidth = 400;
			this.DefaultHeight = 300;
			w5.Hide();
			this.Show();
			this.btnOK.Clicked += new global::System.EventHandler(this.OnBtnOKClicked);
		}
	}
}
