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
            this.logListBox = new System.Windows.Forms.ListBox();
            this.recordStatesTimer = new System.Windows.Forms.Timer(this.components);
            this.playActionsButton = new System.Windows.Forms.Button();
            this.mainButtonsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.recordStatesButton = new System.Windows.Forms.Button();
            this.actionsListBox = new System.Windows.Forms.CheckedListBox();
            this.listButtonsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.moveActionUpButton = new System.Windows.Forms.Button();
            this.moveActionDownButton = new System.Windows.Forms.Button();
            this.addActionButton = new System.Windows.Forms.Button();
            this.addActionTextBox = new System.Windows.Forms.TextBox();
            this.clearLogButton = new System.Windows.Forms.Button();
            this.deleteActionButton = new System.Windows.Forms.Button();
            this.updateActionButton = new System.Windows.Forms.Button();
            this.updateActionTextBox = new System.Windows.Forms.TextBox();
            this.loadActionsButton = new System.Windows.Forms.Button();
            this.saveActionsButton = new System.Windows.Forms.Button();
            this.connectControllerButton = new System.Windows.Forms.Button();
            this.disconnectControllerButton = new System.Windows.Forms.Button();
            this.resetControllerHandleButton = new System.Windows.Forms.Button();
            this.mainButtonsPanel.SuspendLayout();
            this.listButtonsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // logListBox
            // 
            this.mainButtonsPanel.SetColumnSpan(this.logListBox, 2);
            this.logListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logListBox.FormattingEnabled = true;
            this.logListBox.ItemHeight = 15;
            this.logListBox.Location = new System.Drawing.Point(570, 65);
            this.logListBox.Name = "logListBox";
            this.logListBox.Size = new System.Drawing.Size(374, 492);
            this.logListBox.TabIndex = 3;
            // 
            // recordStatesTimer
            // 
            this.recordStatesTimer.Interval = 1;
            this.recordStatesTimer.Tick += new System.EventHandler(this.recordStatesTimer_Tick);
            // 
            // playActionsButton
            // 
            this.playActionsButton.AutoSize = true;
            this.playActionsButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.playActionsButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.playActionsButton.Location = new System.Drawing.Point(192, 3);
            this.playActionsButton.Name = "playActionsButton";
            this.playActionsButton.Size = new System.Drawing.Size(183, 25);
            this.playActionsButton.TabIndex = 6;
            this.playActionsButton.Text = "Play Actions";
            this.playActionsButton.UseVisualStyleBackColor = true;
            this.playActionsButton.Click += new System.EventHandler(this.playActionsButton_Click);
            // 
            // mainButtonsPanel
            // 
            this.mainButtonsPanel.ColumnCount = 5;
            this.mainButtonsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.mainButtonsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.mainButtonsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.mainButtonsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.mainButtonsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.mainButtonsPanel.Controls.Add(this.recordStatesButton, 0, 0);
            this.mainButtonsPanel.Controls.Add(this.logListBox, 3, 2);
            this.mainButtonsPanel.Controls.Add(this.actionsListBox, 0, 2);
            this.mainButtonsPanel.Controls.Add(this.listButtonsPanel, 2, 2);
            this.mainButtonsPanel.Controls.Add(this.playActionsButton, 1, 0);
            this.mainButtonsPanel.Controls.Add(this.loadActionsButton, 2, 0);
            this.mainButtonsPanel.Controls.Add(this.saveActionsButton, 3, 0);
            this.mainButtonsPanel.Controls.Add(this.connectControllerButton, 0, 1);
            this.mainButtonsPanel.Controls.Add(this.disconnectControllerButton, 1, 1);
            this.mainButtonsPanel.Controls.Add(this.resetControllerHandleButton, 2, 1);
            this.mainButtonsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainButtonsPanel.Location = new System.Drawing.Point(0, 0);
            this.mainButtonsPanel.Name = "mainButtonsPanel";
            this.mainButtonsPanel.RowCount = 3;
            this.mainButtonsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainButtonsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainButtonsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainButtonsPanel.Size = new System.Drawing.Size(947, 560);
            this.mainButtonsPanel.TabIndex = 7;
            // 
            // recordStatesButton
            // 
            this.recordStatesButton.AutoSize = true;
            this.recordStatesButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.recordStatesButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.recordStatesButton.Location = new System.Drawing.Point(3, 3);
            this.recordStatesButton.Name = "recordStatesButton";
            this.recordStatesButton.Size = new System.Drawing.Size(183, 25);
            this.recordStatesButton.TabIndex = 5;
            this.recordStatesButton.Text = "Record States";
            this.recordStatesButton.UseVisualStyleBackColor = true;
            this.recordStatesButton.Click += new System.EventHandler(this.recordStatesButton_Click);
            // 
            // actionsListBox
            // 
            this.mainButtonsPanel.SetColumnSpan(this.actionsListBox, 2);
            this.actionsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.actionsListBox.FormattingEnabled = true;
            this.actionsListBox.Location = new System.Drawing.Point(3, 65);
            this.actionsListBox.Name = "actionsListBox";
            this.actionsListBox.Size = new System.Drawing.Size(372, 492);
            this.actionsListBox.TabIndex = 9;
            this.actionsListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.actionsListBox_ItemCheck);
            this.actionsListBox.SelectedIndexChanged += new System.EventHandler(this.actionsListBox_SelectedIndexChanged);
            // 
            // listButtonsPanel
            // 
            this.listButtonsPanel.ColumnCount = 1;
            this.listButtonsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.listButtonsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.listButtonsPanel.Controls.Add(this.moveActionUpButton, 0, 0);
            this.listButtonsPanel.Controls.Add(this.moveActionDownButton, 0, 1);
            this.listButtonsPanel.Controls.Add(this.addActionButton, 0, 2);
            this.listButtonsPanel.Controls.Add(this.addActionTextBox, 0, 3);
            this.listButtonsPanel.Controls.Add(this.clearLogButton, 0, 7);
            this.listButtonsPanel.Controls.Add(this.deleteActionButton, 0, 6);
            this.listButtonsPanel.Controls.Add(this.updateActionButton, 0, 4);
            this.listButtonsPanel.Controls.Add(this.updateActionTextBox, 0, 5);
            this.listButtonsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listButtonsPanel.Location = new System.Drawing.Point(381, 65);
            this.listButtonsPanel.Name = "listButtonsPanel";
            this.listButtonsPanel.RowCount = 10;
            this.listButtonsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.listButtonsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.listButtonsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.listButtonsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.listButtonsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.listButtonsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.listButtonsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.listButtonsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.listButtonsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.listButtonsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.listButtonsPanel.Size = new System.Drawing.Size(183, 492);
            this.listButtonsPanel.TabIndex = 10;
            // 
            // moveActionUpButton
            // 
            this.moveActionUpButton.AutoSize = true;
            this.moveActionUpButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.moveActionUpButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.moveActionUpButton.Location = new System.Drawing.Point(3, 3);
            this.moveActionUpButton.Name = "moveActionUpButton";
            this.moveActionUpButton.Size = new System.Drawing.Size(177, 43);
            this.moveActionUpButton.TabIndex = 0;
            this.moveActionUpButton.Text = "Move Action Up";
            this.moveActionUpButton.UseVisualStyleBackColor = true;
            this.moveActionUpButton.Click += new System.EventHandler(this.moveActionUpButton_Click);
            // 
            // moveActionDownButton
            // 
            this.moveActionDownButton.AutoSize = true;
            this.moveActionDownButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.moveActionDownButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.moveActionDownButton.Location = new System.Drawing.Point(3, 52);
            this.moveActionDownButton.Name = "moveActionDownButton";
            this.moveActionDownButton.Size = new System.Drawing.Size(177, 43);
            this.moveActionDownButton.TabIndex = 1;
            this.moveActionDownButton.Text = "Move Action Down";
            this.moveActionDownButton.UseVisualStyleBackColor = true;
            this.moveActionDownButton.Click += new System.EventHandler(this.moveActionDownButton_Click);
            // 
            // addActionButton
            // 
            this.addActionButton.AutoSize = true;
            this.addActionButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.addActionButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.addActionButton.Location = new System.Drawing.Point(3, 101);
            this.addActionButton.Name = "addActionButton";
            this.addActionButton.Size = new System.Drawing.Size(177, 43);
            this.addActionButton.TabIndex = 2;
            this.addActionButton.Text = "Add Action";
            this.addActionButton.UseVisualStyleBackColor = true;
            this.addActionButton.Click += new System.EventHandler(this.addActionButton_Click);
            // 
            // addActionTextBox
            // 
            this.addActionTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.addActionTextBox.Location = new System.Drawing.Point(3, 150);
            this.addActionTextBox.MaxLength = 2147483647;
            this.addActionTextBox.Multiline = true;
            this.addActionTextBox.Name = "addActionTextBox";
            this.addActionTextBox.PlaceholderText = "Put action json and press Add Action";
            this.addActionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.addActionTextBox.Size = new System.Drawing.Size(177, 43);
            this.addActionTextBox.TabIndex = 5;
            // 
            // clearLogButton
            // 
            this.clearLogButton.AutoSize = true;
            this.clearLogButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.clearLogButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clearLogButton.Location = new System.Drawing.Point(3, 346);
            this.clearLogButton.Name = "clearLogButton";
            this.clearLogButton.Size = new System.Drawing.Size(177, 43);
            this.clearLogButton.TabIndex = 4;
            this.clearLogButton.Text = "Clear Log";
            this.clearLogButton.UseVisualStyleBackColor = true;
            this.clearLogButton.Click += new System.EventHandler(this.clearLogButton_Click);
            // 
            // deleteActionButton
            // 
            this.deleteActionButton.AutoSize = true;
            this.deleteActionButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.deleteActionButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deleteActionButton.Location = new System.Drawing.Point(3, 297);
            this.deleteActionButton.Name = "deleteActionButton";
            this.deleteActionButton.Size = new System.Drawing.Size(177, 43);
            this.deleteActionButton.TabIndex = 3;
            this.deleteActionButton.Text = "Delete Action";
            this.deleteActionButton.UseVisualStyleBackColor = true;
            this.deleteActionButton.Click += new System.EventHandler(this.deleteActionButton_Click);
            // 
            // updateActionButton
            // 
            this.updateActionButton.AutoSize = true;
            this.updateActionButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.updateActionButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.updateActionButton.Location = new System.Drawing.Point(3, 199);
            this.updateActionButton.Name = "updateActionButton";
            this.updateActionButton.Size = new System.Drawing.Size(177, 43);
            this.updateActionButton.TabIndex = 6;
            this.updateActionButton.Text = "Update Action";
            this.updateActionButton.UseVisualStyleBackColor = true;
            this.updateActionButton.Click += new System.EventHandler(this.updateActionButton_Click);
            // 
            // updateActionTextBox
            // 
            this.updateActionTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.updateActionTextBox.Location = new System.Drawing.Point(3, 248);
            this.updateActionTextBox.MaxLength = 2147483647;
            this.updateActionTextBox.Multiline = true;
            this.updateActionTextBox.Name = "updateActionTextBox";
            this.updateActionTextBox.PlaceholderText = "Edit selected action and click Update Action";
            this.updateActionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.updateActionTextBox.Size = new System.Drawing.Size(177, 43);
            this.updateActionTextBox.TabIndex = 7;
            // 
            // loadActionsButton
            // 
            this.loadActionsButton.AutoSize = true;
            this.loadActionsButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.loadActionsButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loadActionsButton.Location = new System.Drawing.Point(381, 3);
            this.loadActionsButton.Name = "loadActionsButton";
            this.loadActionsButton.Size = new System.Drawing.Size(183, 25);
            this.loadActionsButton.TabIndex = 7;
            this.loadActionsButton.Text = "Load Actions";
            this.loadActionsButton.UseVisualStyleBackColor = true;
            this.loadActionsButton.Click += new System.EventHandler(this.loadActionsButton_Click);
            // 
            // saveActionsButton
            // 
            this.saveActionsButton.AutoSize = true;
            this.saveActionsButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.saveActionsButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.saveActionsButton.Location = new System.Drawing.Point(570, 3);
            this.saveActionsButton.Name = "saveActionsButton";
            this.saveActionsButton.Size = new System.Drawing.Size(183, 25);
            this.saveActionsButton.TabIndex = 8;
            this.saveActionsButton.Text = "Save Actions";
            this.saveActionsButton.UseVisualStyleBackColor = true;
            this.saveActionsButton.Click += new System.EventHandler(this.saveActionsButton_Click);
            // 
            // connectControllerButton
            // 
            this.connectControllerButton.AutoSize = true;
            this.connectControllerButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.connectControllerButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.connectControllerButton.Location = new System.Drawing.Point(3, 34);
            this.connectControllerButton.Name = "connectControllerButton";
            this.connectControllerButton.Size = new System.Drawing.Size(183, 25);
            this.connectControllerButton.TabIndex = 11;
            this.connectControllerButton.Text = "Connect Contoller";
            this.connectControllerButton.UseVisualStyleBackColor = true;
            this.connectControllerButton.Click += new System.EventHandler(this.connectControllerButton_Click);
            // 
            // disconnectControllerButton
            // 
            this.disconnectControllerButton.AutoSize = true;
            this.disconnectControllerButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.disconnectControllerButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.disconnectControllerButton.Location = new System.Drawing.Point(192, 34);
            this.disconnectControllerButton.Name = "disconnectControllerButton";
            this.disconnectControllerButton.Size = new System.Drawing.Size(183, 25);
            this.disconnectControllerButton.TabIndex = 12;
            this.disconnectControllerButton.Text = "Disconnect Controller";
            this.disconnectControllerButton.UseVisualStyleBackColor = true;
            this.disconnectControllerButton.Click += new System.EventHandler(this.disconnectControllerButton_Click);
            // 
            // resetControllerHandleButton
            // 
            this.resetControllerHandleButton.AutoSize = true;
            this.resetControllerHandleButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.resetControllerHandleButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resetControllerHandleButton.Location = new System.Drawing.Point(381, 34);
            this.resetControllerHandleButton.Name = "resetControllerHandleButton";
            this.resetControllerHandleButton.Size = new System.Drawing.Size(183, 25);
            this.resetControllerHandleButton.TabIndex = 13;
            this.resetControllerHandleButton.Text = "Reset Controller Handle";
            this.resetControllerHandleButton.UseVisualStyleBackColor = true;
            this.resetControllerHandleButton.Click += new System.EventHandler(this.resetControllerHandleButton_Click);
            // 
            // AutoInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(947, 560);
            this.Controls.Add(this.mainButtonsPanel);
            this.KeyPreview = true;
            this.Name = "AutoInput";
            this.Text = "AutoInput";
            this.Shown += new System.EventHandler(this.AutoInput_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AutoInput_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.AutoInput_KeyUp);
            this.mainButtonsPanel.ResumeLayout(false);
            this.mainButtonsPanel.PerformLayout();
            this.listButtonsPanel.ResumeLayout(false);
            this.listButtonsPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer recordStatesTimer;
        private System.Windows.Forms.Button playActionsButton;
        private System.Windows.Forms.TableLayoutPanel mainButtonsPanel;
        private System.Windows.Forms.Button recordStatesButton;
        private System.Windows.Forms.Button loadActionsButton;
        private System.Windows.Forms.Button saveActionsButton;
        private System.Windows.Forms.ListBox logListBox;
        private System.Windows.Forms.CheckedListBox actionsListBox;
        private System.Windows.Forms.TableLayoutPanel listButtonsPanel;
        private System.Windows.Forms.Button moveActionUpButton;
        private System.Windows.Forms.Button moveActionDownButton;
        private System.Windows.Forms.Button addActionButton;
        private System.Windows.Forms.Button deleteActionButton;
        private System.Windows.Forms.Button clearLogButton;
        private System.Windows.Forms.TextBox addActionTextBox;
        private System.Windows.Forms.Button updateActionButton;
        private System.Windows.Forms.TextBox updateActionTextBox;
        private System.Windows.Forms.Button connectControllerButton;
        private System.Windows.Forms.Button disconnectControllerButton;
        private System.Windows.Forms.Button resetControllerHandleButton;
    }
}