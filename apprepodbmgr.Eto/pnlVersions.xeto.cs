using Eto.Forms;
using Eto.Serialization.Xaml;

namespace apprepodbmgr.Eto
{
    public class pnlVersions : Panel
    {
        public pnlVersions()
        {
            XamlReader.Load(this);

            treeArchs.AllowMultipleSelection    = true;
            treeOs.AllowMultipleSelection       = true;
            treeVersions.AllowMultipleSelection = false;

            treeArchs.ShowHeader    = false;
            treeOs.ShowHeader       = true;
            treeVersions.ShowHeader = false;

            treeArchs.Columns.Add(new GridColumn
            {
                DataCell   = new TextBoxCell {Binding = Binding.Property<string, string>(r => r)},
                HeaderText = "Arch"
            });
            treeOs.Columns.Add(new GridColumn
            {
                DataCell   = new TextBoxCell {Binding = Binding.Property<TargetOsEntry, string>(r => r.name)},
                HeaderText = "Name"
            });
            treeOs.Columns.Add(new GridColumn
            {
                DataCell   = new TextBoxCell {Binding = Binding.Property<TargetOsEntry, string>(r => r.version)},
                HeaderText = "Version"
            });
            treeVersions.Columns.Add(new GridColumn
            {
                DataCell   = new TextBoxCell {Binding = Binding.Property<string, string>(r => r)},
                HeaderText = "Version"
            });
        }

        #region XAML UI elements
        #pragma warning disable 0649
        public GridView treeArchs;
        public GridView treeOs;
        public GridView treeVersions;
        #pragma warning restore 0649
        #endregion XAML UI elements
    }
}