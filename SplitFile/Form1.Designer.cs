namespace SplitFile
{
    partial class Form1
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
            this.txtSrcFile = new System.Windows.Forms.TextBox();
            this.btnSelFile = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.txtChunkSize = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.txtMessages = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtSrcFile
            // 
            this.txtSrcFile.Location = new System.Drawing.Point(119, 33);
            this.txtSrcFile.Name = "txtSrcFile";
            this.txtSrcFile.Size = new System.Drawing.Size(276, 20);
            this.txtSrcFile.TabIndex = 0;
            // 
            // btnSelFile
            // 
            this.btnSelFile.Location = new System.Drawing.Point(26, 31);
            this.btnSelFile.Name = "btnSelFile";
            this.btnSelFile.Size = new System.Drawing.Size(75, 23);
            this.btnSelFile.TabIndex = 1;
            this.btnSelFile.Text = "Select File";
            this.btnSelFile.UseVisualStyleBackColor = true;
            this.btnSelFile.Click += new System.EventHandler(this.btnSelFile_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(26, 113);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(116, 17);
            this.checkBox1.TabIndex = 2;
            this.checkBox1.Text = "First Line is Header";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // txtChunkSize
            // 
            this.txtChunkSize.Location = new System.Drawing.Point(119, 75);
            this.txtChunkSize.Name = "txtChunkSize";
            this.txtChunkSize.Size = new System.Drawing.Size(100, 20);
            this.txtChunkSize.TabIndex = 3;
            this.txtChunkSize.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtChunkSize_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Lines Per File";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(26, 152);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Split";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(119, 152);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(310, 23);
            this.progressBar1.TabIndex = 6;
            // 
            // txtMessages
            // 
            this.txtMessages.Location = new System.Drawing.Point(30, 217);
            this.txtMessages.Multiline = true;
            this.txtMessages.Name = "txtMessages";
            this.txtMessages.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMessages.Size = new System.Drawing.Size(399, 82);
            this.txtMessages.TabIndex = 7;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(461, 326);
            this.Controls.Add(this.txtMessages);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtChunkSize);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.btnSelFile);
            this.Controls.Add(this.txtSrcFile);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSrcFile;
        private System.Windows.Forms.Button btnSelFile;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox txtChunkSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TextBox txtMessages;
    }
}

