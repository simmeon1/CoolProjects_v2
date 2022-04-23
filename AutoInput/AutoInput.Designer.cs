namespace AutoInput
{
    partial class AutoInput
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
            this.copyStatesToClipboardButton = new System.Windows.Forms.Button();
            this.forgetStatesButton = new System.Windows.Forms.Button();
            this.listBox = new System.Windows.Forms.ListBox();
            this.pixelLogTimer = new System.Windows.Forms.Timer(this.components);
            this.respawnButton = new System.Windows.Forms.Button();
            this.recordStatesButton = new System.Windows.Forms.Button();
            this.replayStatesButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // copyStatesToClipboardButton
            // 
            this.copyStatesToClipboardButton.Location = new System.Drawing.Point(485, 12);
            this.copyStatesToClipboardButton.Name = "copyStatesToClipboardButton";
            this.copyStatesToClipboardButton.Size = new System.Drawing.Size(153, 39);
            this.copyStatesToClipboardButton.TabIndex = 0;
            this.copyStatesToClipboardButton.Text = "Copy States To Clipboard";
            this.copyStatesToClipboardButton.UseVisualStyleBackColor = true;
            this.copyStatesToClipboardButton.Click += new System.EventHandler(this.copyStatesToClipboardButton_Click);
            // 
            // forgetStatesButton
            // 
            this.forgetStatesButton.Location = new System.Drawing.Point(171, 13);
            this.forgetStatesButton.Name = "forgetStatesButton";
            this.forgetStatesButton.Size = new System.Drawing.Size(153, 39);
            this.forgetStatesButton.TabIndex = 1;
            this.forgetStatesButton.Text = "Forget States";
            this.forgetStatesButton.UseVisualStyleBackColor = true;
            this.forgetStatesButton.Click += new System.EventHandler(this.forgetStatesButton_Click);
            // 
            // listBox
            // 
            this.listBox.FormattingEnabled = true;
            this.listBox.ItemHeight = 15;
            this.listBox.Location = new System.Drawing.Point(6, 158);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(791, 289);
            this.listBox.TabIndex = 3;
            // 
            // pixelLogTimer
            // 
            this.pixelLogTimer.Enabled = true;
            this.pixelLogTimer.Interval = 1000;
            this.pixelLogTimer.Tick += new System.EventHandler(this.pixelLogTimer_Tick);
            // 
            // respawnButton
            // 
            this.respawnButton.Location = new System.Drawing.Point(644, 12);
            this.respawnButton.Name = "respawnButton";
            this.respawnButton.Size = new System.Drawing.Size(144, 39);
            this.respawnButton.TabIndex = 4;
            this.respawnButton.Text = "Respawn";
            this.respawnButton.UseVisualStyleBackColor = true;
            this.respawnButton.Click += new System.EventHandler(this.respawnButton_Click);
            // 
            // recordStatesButton
            // 
            this.recordStatesButton.Location = new System.Drawing.Point(12, 12);
            this.recordStatesButton.Name = "recordStatesButton";
            this.recordStatesButton.Size = new System.Drawing.Size(153, 39);
            this.recordStatesButton.TabIndex = 5;
            this.recordStatesButton.Text = "Record States";
            this.recordStatesButton.UseVisualStyleBackColor = true;
            this.recordStatesButton.Click += new System.EventHandler(this.recordStatesButton_Click);
            // 
            // replayStatesButton
            // 
            this.replayStatesButton.Location = new System.Drawing.Point(330, 13);
            this.replayStatesButton.Name = "replayStatesButton";
            this.replayStatesButton.Size = new System.Drawing.Size(149, 39);
            this.replayStatesButton.TabIndex = 6;
            this.replayStatesButton.Text = "Replay States";
            this.replayStatesButton.UseVisualStyleBackColor = true;
            this.replayStatesButton.Click += new System.EventHandler(this.replayStatesButton_Click);
            // 
            // AutoInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.replayStatesButton);
            this.Controls.Add(this.recordStatesButton);
            this.Controls.Add(this.respawnButton);
            this.Controls.Add(this.listBox);
            this.Controls.Add(this.forgetStatesButton);
            this.Controls.Add(this.copyStatesToClipboardButton);
            this.KeyPreview = true;
            this.Name = "AutoInput";
            this.Text = "Form1";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button copyStatesToClipboardButton;
        private System.Windows.Forms.Button forgetStatesButton;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.Timer pixelLogTimer;
        private System.Windows.Forms.Button respawnButton;
        private System.Windows.Forms.Button recordStatesButton;
        private System.Windows.Forms.Button replayStatesButton;
    }
}