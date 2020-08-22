using System;
using Eto.Forms;
using Eto.Serialization.Xaml;

namespace apprepodbmgr.Eto
{
    public class pnlStrings : Panel
    {
        public pnlStrings()
        {
            XamlReader.Load(this);

            treeStrings.AllowMultipleSelection = false;
            treeStrings.ShowHeader             = false;

            treeStrings.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell
                {
                    Binding = Binding.Property<string, string>(r => r)
                },
                HeaderText = "String"
            });
        }

        void OnBtnDeveloperClick(object sender, EventArgs eventArgs)
        {
            txtDeveloper.Text = "";

            if(!(treeStrings.SelectedItem is string str))
                return;

            txtDeveloper.Text = str;
        }

        void OnBtnPublisherClick(object sender, EventArgs eventArgs)
        {
            txtPublisher.Text = "";

            if(!(treeStrings.SelectedItem is string str))
                return;

            txtPublisher.Text = str;
        }

        void OnBtnProductClick(object sender, EventArgs eventArgs)
        {
            txtProduct.Text = "";

            if(!(treeStrings.SelectedItem is string str))
                return;

            txtProduct.Text = str;
        }

        void OnBtnVersionClick(object sender, EventArgs eventArgs)
        {
            txtVersion.Text = "";

            if(!(treeStrings.SelectedItem is string str))
                return;

            txtVersion.Text = str;
        }

        #region XAML UI elements
        #pragma warning disable 0649
        public TextBox  txtDeveloper;
        public TextBox  txtPublisher;
        public TextBox  txtProduct;
        public TextBox  txtVersion;
        public GridView treeStrings;
        #pragma warning restore 0649
        #endregion XAML UI elements
    }
}