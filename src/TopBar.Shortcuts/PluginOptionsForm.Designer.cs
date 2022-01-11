namespace TopBar.Plugins.Shortcuts
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
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this._listviewShorcuts = new System.Windows.Forms.ListView();
            this.headerText = new System.Windows.Forms.ColumnHeader();
            this.buttonMoveUp = new System.Windows.Forms.Button();
            this.buttonMoveDown = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(342, 421);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(261, 421);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 3;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // _listviewShorcuts
            // 
            this._listviewShorcuts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.headerText});
            this._listviewShorcuts.FullRowSelect = true;
            this._listviewShorcuts.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this._listviewShorcuts.LabelEdit = true;
            this._listviewShorcuts.Location = new System.Drawing.Point(57, 12);
            this._listviewShorcuts.MultiSelect = false;
            this._listviewShorcuts.Name = "_listviewShorcuts";
            this._listviewShorcuts.Size = new System.Drawing.Size(360, 403);
            this._listviewShorcuts.TabIndex = 4;
            this._listviewShorcuts.UseCompatibleStateImageBehavior = false;
            this._listviewShorcuts.View = System.Windows.Forms.View.Details;
            this._listviewShorcuts.SelectedIndexChanged += new System.EventHandler(this._listviewShorcuts_SelectedIndexChanged);
            // 
            // headerText
            // 
            this.headerText.Text = "";
            this.headerText.Width = 350;
            // 
            // buttonMoveUp
            // 
            this.buttonMoveUp.Image = global::TopBar.Plugins.Shortcuts.Properties.Resources.arrow_up_32;
            this.buttonMoveUp.Location = new System.Drawing.Point(12, 164);
            this.buttonMoveUp.Name = "buttonMoveUp";
            this.buttonMoveUp.Size = new System.Drawing.Size(39, 35);
            this.buttonMoveUp.TabIndex = 5;
            this.buttonMoveUp.UseVisualStyleBackColor = true;
            this.buttonMoveUp.Click += new System.EventHandler(this.buttonMoveUp_Click);
            // 
            // buttonMoveDown
            // 
            this.buttonMoveDown.Image = global::TopBar.Plugins.Shortcuts.Properties.Resources.arrow_down_32;
            this.buttonMoveDown.Location = new System.Drawing.Point(12, 205);
            this.buttonMoveDown.Name = "buttonMoveDown";
            this.buttonMoveDown.Size = new System.Drawing.Size(39, 35);
            this.buttonMoveDown.TabIndex = 6;
            this.buttonMoveDown.UseVisualStyleBackColor = true;
            this.buttonMoveDown.Click += new System.EventHandler(this.buttonMoveDown_Click);
            // 
            // PluginOptionsForm
            // 
            this.AcceptButton = this.buttonSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(429, 451);
            this.Controls.Add(this.buttonMoveDown);
            this.Controls.Add(this.buttonMoveUp);
            this.Controls.Add(this._listviewShorcuts);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PluginOptionsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Shortcuts options";
            this.Load += new System.EventHandler(this.PluginOptions_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private Button buttonCancel;
        private Button buttonSave;
        private ListView _listviewShorcuts;
        private Button buttonMoveUp;
        private Button buttonMoveDown;
        private ColumnHeader headerText;
    }
}