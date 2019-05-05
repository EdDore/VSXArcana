namespace SimpleEditor
{
    partial class SimpleWinFormEditor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnGetAppName = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnGetAppName
            // 
            this.btnGetAppName.Location = new System.Drawing.Point(0, 0);
            this.btnGetAppName.Name = "btnGetAppName";
            this.btnGetAppName.Size = new System.Drawing.Size(106, 23);
            this.btnGetAppName.TabIndex = 0;
            this.btnGetAppName.Text = "GetAppName...";
            this.btnGetAppName.UseVisualStyleBackColor = true;
            this.btnGetAppName.Click += new System.EventHandler(this.OnGetAppName_Click);
            // 
            // SimpleWinFormControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnGetAppName);
            this.Name = "SimpleWinFormControl";
            this.Size = new System.Drawing.Size(270, 217);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnGetAppName;
    }
}
