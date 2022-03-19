
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
            this.mainPanel = new System.Windows.Forms.TableLayoutPanel();
            this.log = new System.Windows.Forms.ListBox();
            this.clearLogButton = new System.Windows.Forms.Button();
            this.damageButton = new System.Windows.Forms.Button();
            this.reminderPanel = new System.Windows.Forms.TableLayoutPanel();
            this.button1 = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.CheckBox();
            this.button3 = new System.Windows.Forms.CheckBox();
            this.reminderLabel = new System.Windows.Forms.Label();
            this.buttonR = new System.Windows.Forms.CheckBox();
            this.buttonD = new System.Windows.Forms.CheckBox();
            this.buttonF = new System.Windows.Forms.CheckBox();
            this.reminderToggleButton = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.mainPanel.SuspendLayout();
            this.reminderPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.ColumnCount = 2;
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainPanel.Controls.Add(this.log, 0, 2);
            this.mainPanel.Controls.Add(this.clearLogButton, 0, 0);
            this.mainPanel.Controls.Add(this.damageButton, 0, 1);
            this.mainPanel.Controls.Add(this.reminderPanel, 1, 0);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.RowCount = 3;
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.mainPanel.Size = new System.Drawing.Size(693, 560);
            this.mainPanel.TabIndex = 0;
            // 
            // log
            // 
            this.mainPanel.SetColumnSpan(this.log, 2);
            this.log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.log.FormattingEnabled = true;
            this.log.ItemHeight = 15;
            this.log.Location = new System.Drawing.Point(3, 96);
            this.log.Name = "log";
            this.log.Size = new System.Drawing.Size(687, 461);
            this.log.TabIndex = 0;
            // 
            // clearLogButton
            // 
            this.clearLogButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clearLogButton.Location = new System.Drawing.Point(3, 3);
            this.clearLogButton.Name = "clearLogButton";
            this.clearLogButton.Size = new System.Drawing.Size(340, 25);
            this.clearLogButton.TabIndex = 3;
            this.clearLogButton.Text = "Clear";
            this.clearLogButton.UseVisualStyleBackColor = true;
            this.clearLogButton.Click += new System.EventHandler(this.clearLog_Click);
            // 
            // damageButton
            // 
            this.damageButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.damageButton.Location = new System.Drawing.Point(3, 34);
            this.damageButton.Name = "damageButton";
            this.damageButton.Size = new System.Drawing.Size(340, 56);
            this.damageButton.TabIndex = 1;
            this.damageButton.Text = "Get Enemy Team Damage";
            this.damageButton.UseVisualStyleBackColor = true;
            this.damageButton.Click += new System.EventHandler(this.damageButton_Click);
            // 
            // reminderPanel
            // 
            this.reminderPanel.ColumnCount = 3;
            this.reminderPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.reminderPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.reminderPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.reminderPanel.Controls.Add(this.button1, 0, 2);
            this.reminderPanel.Controls.Add(this.button2, 0, 2);
            this.reminderPanel.Controls.Add(this.button3, 0, 2);
            this.reminderPanel.Controls.Add(this.reminderLabel, 0, 0);
            this.reminderPanel.Controls.Add(this.buttonR, 0, 1);
            this.reminderPanel.Controls.Add(this.buttonD, 1, 1);
            this.reminderPanel.Controls.Add(this.buttonF, 2, 1);
            this.reminderPanel.Controls.Add(this.reminderToggleButton, 1, 0);
            this.reminderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reminderPanel.Location = new System.Drawing.Point(346, 0);
            this.reminderPanel.Margin = new System.Windows.Forms.Padding(0);
            this.reminderPanel.Name = "reminderPanel";
            this.reminderPanel.RowCount = 3;
            this.mainPanel.SetRowSpan(this.reminderPanel, 2);
            this.reminderPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.reminderPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.reminderPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.reminderPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.reminderPanel.Size = new System.Drawing.Size(347, 93);
            this.reminderPanel.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Appearance = System.Windows.Forms.Appearance.Button;
            this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button1.Location = new System.Drawing.Point(3, 65);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(109, 25);
            this.button1.TabIndex = 16;
            this.button1.Tag = "1073,1017,0.61960787";
            this.button1.Text = "1";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Appearance = System.Windows.Forms.Appearance.Button;
            this.button2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button2.Location = new System.Drawing.Point(118, 65);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(109, 25);
            this.button2.TabIndex = 15;
            this.button2.Tag = "1105,1020";
            this.button2.Text = "2";
            this.button2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Appearance = System.Windows.Forms.Appearance.Button;
            this.button3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button3.Location = new System.Drawing.Point(233, 65);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(111, 25);
            this.button3.TabIndex = 14;
            this.button3.Tag = "1136,1013";
            this.button3.Text = "3";
            this.button3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.button3.UseVisualStyleBackColor = true;
            // 
            // reminderLabel
            // 
            this.reminderLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reminderLabel.Location = new System.Drawing.Point(3, 0);
            this.reminderLabel.Name = "reminderLabel";
            this.reminderLabel.Size = new System.Drawing.Size(109, 31);
            this.reminderLabel.TabIndex = 7;
            this.reminderLabel.Text = "Reminders";
            this.reminderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonR
            // 
            this.buttonR.Appearance = System.Windows.Forms.Appearance.Button;
            this.buttonR.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonR.Location = new System.Drawing.Point(3, 34);
            this.buttonR.Name = "buttonR";
            this.buttonR.Size = new System.Drawing.Size(109, 25);
            this.buttonR.TabIndex = 9;
            this.buttonR.Tag = "938,1030";
            this.buttonR.Text = "R";
            this.buttonR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.buttonR.UseVisualStyleBackColor = true;
            // 
            // buttonD
            // 
            this.buttonD.Appearance = System.Windows.Forms.Appearance.Button;
            this.buttonD.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonD.Location = new System.Drawing.Point(118, 34);
            this.buttonD.Name = "buttonD";
            this.buttonD.Size = new System.Drawing.Size(109, 25);
            this.buttonD.TabIndex = 13;
            this.buttonD.Tag = "987,1019";
            this.buttonD.Text = "D";
            this.buttonD.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.buttonD.UseVisualStyleBackColor = true;
            // 
            // buttonF
            // 
            this.buttonF.Appearance = System.Windows.Forms.Appearance.Button;
            this.buttonF.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonF.Location = new System.Drawing.Point(233, 34);
            this.buttonF.Name = "buttonF";
            this.buttonF.Size = new System.Drawing.Size(111, 25);
            this.buttonF.TabIndex = 11;
            this.buttonF.Tag = "1022,1023";
            this.buttonF.Text = "F";
            this.buttonF.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.buttonF.UseVisualStyleBackColor = true;
            // 
            // reminderToggleButton
            // 
            this.reminderPanel.SetColumnSpan(this.reminderToggleButton, 2);
            this.reminderToggleButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reminderToggleButton.Location = new System.Drawing.Point(118, 3);
            this.reminderToggleButton.Name = "reminderToggleButton";
            this.reminderToggleButton.Size = new System.Drawing.Size(226, 25);
            this.reminderToggleButton.TabIndex = 17;
            this.reminderToggleButton.Text = "Toggle All";
            this.reminderToggleButton.UseVisualStyleBackColor = true;
            this.reminderToggleButton.Click += new System.EventHandler(this.reminderToggleButton_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(693, 560);
            this.Controls.Add(this.mainPanel);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.mainPanel.ResumeLayout(false);
            this.reminderPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainPanel;
        private System.Windows.Forms.Button damageButton;
        private System.Windows.Forms.Button clearLogButton;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label reminderLabel;
        private System.Windows.Forms.TableLayoutPanel reminderPanel;
        private System.Windows.Forms.CheckBox buttonF;
        private System.Windows.Forms.CheckBox buttonR;
        private System.Windows.Forms.CheckBox buttonD;
        private System.Windows.Forms.CheckBox button1;
        private System.Windows.Forms.CheckBox button2;
        private System.Windows.Forms.CheckBox button3;
        private System.Windows.Forms.Button reminderToggleButton;
        private System.Windows.Forms.ListBox log;
    }
}

