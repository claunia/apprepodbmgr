using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Serialization.Xaml;
using EncodingInfo = Claunia.Encoding.EncodingInfo;

namespace apprepodbmgr.Eto
{
    public class pnlDescription : Panel
    {
        ObservableCollection<ListItem> cmbCodepagesItems;
        Encoding                       currentEncoding;
        public string                  description;

        public pnlDescription()
        {
            XamlReader.Load(this);

            treeFiles.Columns.Add(new GridColumn
            {
                DataCell   = new TextBoxCell {Binding = Binding.Property<string, string>(r => r)},
                HeaderText = "File"
            });

            treeFiles.AllowMultipleSelection =  false;
            treeFiles.SelectionChanged       += TreeFilesOnSelectionChanged;
            cmbCodepagesItems                =  new ObservableCollection<ListItem>();
            foreach(EncodingInfo enc in Claunia.Encoding.Encoding.GetEncodings())
                cmbCodepagesItems.Add(new ListItem {Key = enc.Name, Text = enc.DisplayName});
            foreach(System.Text.EncodingInfo enc in Encoding.GetEncodings())
                cmbCodepagesItems.Add(new ListItem {Key = enc.Name, Text = enc.GetEncoding().EncodingName});
            cmbCodepages.DataStore = cmbCodepagesItems.OrderBy(t => t.Text);
            try
            {
                currentEncoding          = Claunia.Encoding.Encoding.GetEncoding("ibm437");
                cmbCodepages.SelectedKey = currentEncoding.BodyName;
            }
            catch { currentEncoding = Encoding.ASCII; }

            cmbCodepages.SelectedIndexChanged += CmbCodepagesOnSelectedIndexChanged;
        }

        void CmbCodepagesOnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            try { currentEncoding = Claunia.Encoding.Encoding.GetEncoding(cmbCodepages.SelectedKey); }
            catch
            {
                currentEncoding          = Encoding.ASCII;
                cmbCodepages.SelectedKey = currentEncoding.BodyName;
            }

            TreeFilesOnSelectionChanged(sender, eventArgs);
        }

        void TreeFilesOnSelectionChanged(object sender, EventArgs eventArgs)
        {
            txtDescription.Text = "";
            description         = null;
            if(!(treeFiles.SelectedItem is string file)) return;

            StreamReader sr = new StreamReader(file, currentEncoding);
            description         = sr.ReadToEnd();
            txtDescription.Text = description;
            sr.Close();
        }

        void OnBtnClearClick(object sender, EventArgs eventArgs)
        {
            txtDescription.Text = "";
            description         = null;
            treeFiles.UnselectAll();
        }

        #region XAML UI elements
        #pragma warning disable 0649
        public GridView treeFiles;
        TextArea        txtDescription;
        DropDown        cmbCodepages;
        Button          btnClear;
        #pragma warning restore 0649
        #endregion XAML UI elements
    }
}