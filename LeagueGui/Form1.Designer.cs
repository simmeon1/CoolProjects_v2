
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
            this.reminderButton = new System.Windows.Forms.Button();
            this.clearLogButton = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.listBox1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.damageButton, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.reminderButton, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.clearLogButton, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1081, 635);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // listBox1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.listBox1, 2);
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 15;
            this.listBox1.Location = new System.Drawing.Point(3, 75);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(1075, 557);
            this.listBox1.TabIndex = 0;
            // 
            // damageButton
            // 
            this.damageButton.AutoSize = true;
            this.damageButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.damageButton.Location = new System.Drawing.Point(3, 3);
            this.damageButton.MaximumSize = new System.Drawing.Size(0, 30);
            this.damageButton.MinimumSize = new System.Drawing.Size(0, 30);
            this.damageButton.Name = "damageButton";
            this.damageButton.Size = new System.Drawing.Size(534, 30);
            this.damageButton.TabIndex = 1;
            this.damageButton.Text = "Get Enemy Team Damage";
            this.damageButton.UseVisualStyleBackColor = true;
            this.damageButton.Click += new System.EventHandler(this.damageButton_Click);
            // 
            // reminderButton
            // 
            this.reminderButton.AutoSize = true;
            this.reminderButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reminderButton.Location = new System.Drawing.Point(543, 3);
            this.reminderButton.MaximumSize = new System.Drawing.Size(0, 30);
            this.reminderButton.MinimumSize = new System.Drawing.Size(0, 30);
            this.reminderButton.Name = "reminderButton";
            this.reminderButton.Size = new System.Drawing.Size(535, 30);
            this.reminderButton.TabIndex = 2;
            this.reminderButton.Text = "Enable Spell Reminders";
            this.reminderButton.UseVisualStyleBackColor = true;
            this.reminderButton.Click += new System.EventHandler(this.reminderButton_Click);
            // 
            // clearLogButton
            // 
            this.clearLogButton.AutoSize = true;
            this.clearLogButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clearLogButton.Location = new System.Drawing.Point(543, 39);
            this.clearLogButton.MaximumSize = new System.Drawing.Size(0, 30);
            this.clearLogButton.MinimumSize = new System.Drawing.Size(0, 30);
            this.clearLogButton.Name = "clearLogButton";
            this.clearLogButton.Size = new System.Drawing.Size(535, 30);
            this.clearLogButton.TabIndex = 3;
            this.clearLogButton.Text = "Clear";
            this.clearLogButton.UseVisualStyleBackColor = true;
            this.clearLogButton.Click += new System.EventHandler(this.clearLog_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1081, 635);
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
        private System.Windows.Forms.Button reminderButton;
        private System.Windows.Forms.Button clearLogButton;
        private System.Windows.Forms.Timer timer1;
    }
}

