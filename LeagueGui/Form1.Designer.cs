
namespace LeagueGui
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.damageButton = new System.Windows.Forms.Button();
            this.clearLogButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonR = new System.Windows.Forms.CheckBox();
            this.buttonD = new System.Windows.Forms.CheckBox();
            this.buttonF = new System.Windows.Forms.CheckBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.5102F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.5102F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.32653F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.32653F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.32653F));
            this.tableLayoutPanel1.Controls.Add(this.listBox1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.damageButton, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.clearLogButton, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonR, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.buttonD, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.buttonF, 4, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(851, 570);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // listBox1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.listBox1, 5);
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 15;
            this.listBox1.Location = new System.Drawing.Point(3, 75);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(845, 492);
            this.listBox1.TabIndex = 0;
            // 
            // damageButton
            // 
            this.damageButton.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.damageButton, 2);
            this.damageButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.damageButton.Location = new System.Drawing.Point(3, 3);
            this.damageButton.MaximumSize = new System.Drawing.Size(0, 30);
            this.damageButton.MinimumSize = new System.Drawing.Size(0, 30);
            this.damageButton.Name = "damageButton";
            this.damageButton.Size = new System.Drawing.Size(428, 30);
            this.damageButton.TabIndex = 1;
            this.damageButton.Text = "Get Enemy Team Damage";
            this.damageButton.UseVisualStyleBackColor = true;
            this.damageButton.Click += new System.EventHandler(this.damageButton_Click);
            // 
            // clearLogButton
            // 
            this.clearLogButton.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.clearLogButton, 2);
            this.clearLogButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clearLogButton.Location = new System.Drawing.Point(3, 39);
            this.clearLogButton.MaximumSize = new System.Drawing.Size(0, 30);
            this.clearLogButton.MinimumSize = new System.Drawing.Size(0, 30);
            this.clearLogButton.Name = "clearLogButton";
            this.clearLogButton.Size = new System.Drawing.Size(428, 30);
            this.clearLogButton.TabIndex = 3;
            this.clearLogButton.Text = "Clear";
            this.clearLogButton.UseVisualStyleBackColor = true;
            this.clearLogButton.Click += new System.EventHandler(this.clearLog_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 3);
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(437, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(411, 36);
            this.label1.TabIndex = 7;
            this.label1.Text = "Reminders";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonR
            // 
            this.buttonR.Appearance = System.Windows.Forms.Appearance.Button;
            this.buttonR.AutoSize = true;
            this.buttonR.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonR.Location = new System.Drawing.Point(437, 39);
            this.buttonR.Name = "buttonR";
            this.buttonR.Size = new System.Drawing.Size(132, 30);
            this.buttonR.TabIndex = 8;
            this.buttonR.Tag = "938,1030";
            this.buttonR.Text = "R";
            this.buttonR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.buttonR.UseVisualStyleBackColor = true;
            // 
            // buttonD
            // 
            this.buttonD.Appearance = System.Windows.Forms.Appearance.Button;
            this.buttonD.AutoSize = true;
            this.buttonD.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonD.Location = new System.Drawing.Point(575, 39);
            this.buttonD.Name = "buttonD";
            this.buttonD.Size = new System.Drawing.Size(132, 30);
            this.buttonD.TabIndex = 9;
            this.buttonD.Tag = "987,1019";
            this.buttonD.Text = "D";
            this.buttonD.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.buttonD.UseVisualStyleBackColor = true;
            // 
            // buttonF
            // 
            this.buttonF.Appearance = System.Windows.Forms.Appearance.Button;
            this.buttonF.AutoSize = true;
            this.buttonF.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonF.Location = new System.Drawing.Point(713, 39);
            this.buttonF.Name = "buttonF";
            this.buttonF.Size = new System.Drawing.Size(135, 30);
            this.buttonF.TabIndex = 10;
            this.buttonF.Tag = "1022,1023";
            this.buttonF.Text = "F";
            this.buttonF.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.buttonF.UseVisualStyleBackColor = true;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(851, 570);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button damageButton;
        private System.Windows.Forms.Button clearLogButton;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox buttonR;
        private System.Windows.Forms.CheckBox buttonD;
        private System.Windows.Forms.CheckBox buttonF;
    }
}

