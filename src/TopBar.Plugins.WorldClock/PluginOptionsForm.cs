using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.ListViewItem;

namespace TopBar.Plugins.WorldClock
{
    public partial class PluginOptionsForm : Form
    {
        readonly ComboBox _timeZonesList;
        public PluginOptionsForm()
        {
            _timeZonesList = new ComboBox() { Visible = false };
            _timeZonesList.Items.Add("Option 1");
            _timeZonesList.Items.Add("Option 2");
            _timeZonesList.Items.Add("Option 3");

            this.Controls.Add(_timeZonesList);

            InitializeComponent();
            this._listviewShorcuts.MouseDoubleClick += _listviewShorcuts_MouseDoubleClick;
        }

        private void _listviewShorcuts_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            //https://www.codeproject.com/Articles/316204/ListView-in-line-editing


            var currentItem = _listviewShorcuts.GetItemAt(e.X, e.Y);
            if (currentItem == null)
                return;

            var currentSubItem = currentItem.GetSubItemAt(e.X, e.Y);
            if (currentSubItem == null)
                return;

            var lLeft = currentSubItem.Bounds.Left + 2;
            var lWidth = currentSubItem.Bounds.Width;

            _timeZonesList.SetBounds(lLeft + _listviewShorcuts.Left, currentSubItem.Bounds.Top + _listviewShorcuts.Top, lWidth, currentSubItem.Bounds.Height);
            _timeZonesList.Show();
            _timeZonesList.Focus();


        }



        private void PluginOptionsForm_Load(object sender, EventArgs e)
        {
            var item = new ListViewItem();

            var subitems = new ListViewSubItem[] {
            new ListViewSubItem(item,"Text1"),
            new ListViewSubItem(item,"Text2"),
            new ListViewSubItem(item,"Text3")
            };

            item.SubItems.AddRange(subitems);

            this._listviewShorcuts.Items.Add(item);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void _listviewShorcuts_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
