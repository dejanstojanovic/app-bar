namespace TopBar.Plugins.WorldClock
{
    partial class PluginOptionsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PluginOptionsForm));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this._listviewTimezones = new System.Windows.Forms.ListView();
            this.headerTitle = new System.Windows.Forms.ColumnHeader();
            this.headerTimeZone = new System.Windows.Forms.ColumnHeader();
            this.buttonMoveUp = new System.Windows.Forms.Button();
            this.buttonMoveDown = new System.Windows.Forms.Button();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(620, 421);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSave.Location = new System.Drawing.Point(539, 421);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 3;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // _listviewTimezones
            // 
            this._listviewTimezones.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._listviewTimezones.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.headerTitle,
            this.headerTimeZone});
            this._listviewTimezones.FullRowSelect = true;
            this._listviewTimezones.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this._listviewTimezones.LabelEdit = true;
            this._listviewTimezones.Location = new System.Drawing.Point(57, 12);
            this._listviewTimezones.MultiSelect = false;
            this._listviewTimezones.Name = "_listviewTimezones";
            this._listviewTimezones.Size = new System.Drawing.Size(638, 403);
            this._listviewTimezones.TabIndex = 4;
            this._listviewTimezones.UseCompatibleStateImageBehavior = false;
            this._listviewTimezones.View = System.Windows.Forms.View.Details;
            // 
            // headerTitle
            // 
            this.headerTitle.Text = "Title";
            this.headerTitle.Width = 250;
            // 
            // headerTimeZone
            // 
            this.headerTimeZone.Text = "Time Zone";
            this.headerTimeZone.Width = 350;
            // 
            // buttonMoveUp
            // 
            this.buttonMoveUp.Image = ((System.Drawing.Image)(resources.GetObject("buttonMoveUp.Image")));
            this.buttonMoveUp.Location = new System.Drawing.Point(12, 164);
            this.buttonMoveUp.Name = "buttonMoveUp";
            this.buttonMoveUp.Size = new System.Drawing.Size(39, 35);
            this.buttonMoveUp.TabIndex = 5;
            this.buttonMoveUp.UseVisualStyleBackColor = true;
            // 
            // buttonMoveDown
            // 
            this.buttonMoveDown.Image = ((System.Drawing.Image)(resources.GetObject("buttonMoveDown.Image")));
            this.buttonMoveDown.Location = new System.Drawing.Point(12, 205);
            this.buttonMoveDown.Name = "buttonMoveDown";
            this.buttonMoveDown.Size = new System.Drawing.Size(39, 35);
            this.buttonMoveDown.TabIndex = 6;
            this.buttonMoveDown.UseVisualStyleBackColor = true;
            // 
            // buttonAdd
            // 
            this.buttonAdd.Image = ((System.Drawing.Image)(resources.GetObject("buttonAdd.Image")));
            this.buttonAdd.Location = new System.Drawing.Point(12, 380);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(39, 35);
            this.buttonAdd.TabIndex = 7;
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // PluginOptionsForm
            // 
            this.AcceptButton = this.buttonSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(707, 451);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.buttonMoveDown);
            this.Controls.Add(this.buttonMoveUp);
            this.Controls.Add(this._listviewTimezones);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PluginOptionsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Shortcuts options";
            this.Load += new System.EventHandler(this.PluginOptionsForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Button buttonMoveDown;
        private Button buttonMoveUp;
        private ListView _listviewTimezones;
        private ColumnHeader headerTitle;
        private Button buttonSave;
        private Button buttonCancel;
        private ColumnHeader headerTimeZone;
        private Button buttonAdd;
    }
}