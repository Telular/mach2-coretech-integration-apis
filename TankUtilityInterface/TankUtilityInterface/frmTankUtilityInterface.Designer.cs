namespace TankUtilityInterface
{
    partial class frmTankUtilityInterface
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
            this.btnTest = new System.Windows.Forms.Button();
            this.listViewStatus = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listViewMsg = new System.Windows.Forms.ListView();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnConfigInfo = new System.Windows.Forms.Button();
            this.bkgWorkerTx = new System.ComponentModel.BackgroundWorker();
            this.bkgWorkerRx = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // btnTest
            // 
            this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTest.Location = new System.Drawing.Point(530, 32);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(75, 23);
            this.btnTest.TabIndex = 51;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Visible = false;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // listViewStatus
            // 
            this.listViewStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewStatus.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader13});
            this.listViewStatus.FullRowSelect = true;
            this.listViewStatus.HideSelection = false;
            this.listViewStatus.LabelEdit = true;
            this.listViewStatus.Location = new System.Drawing.Point(47, 217);
            this.listViewStatus.Name = "listViewStatus";
            this.listViewStatus.Size = new System.Drawing.Size(706, 202);
            this.listViewStatus.TabIndex = 50;
            this.listViewStatus.UseCompatibleStateImageBehavior = false;
            this.listViewStatus.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Date / Time";
            this.columnHeader3.Width = 127;
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "Status";
            this.columnHeader13.Width = 557;
            // 
            // listViewMsg
            // 
            this.listViewMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewMsg.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader1,
            this.columnHeader7,
            this.columnHeader8});
            this.listViewMsg.FullRowSelect = true;
            this.listViewMsg.HideSelection = false;
            this.listViewMsg.LabelEdit = true;
            this.listViewMsg.Location = new System.Drawing.Point(47, 64);
            this.listViewMsg.Name = "listViewMsg";
            this.listViewMsg.Size = new System.Drawing.Size(706, 136);
            this.listViewMsg.TabIndex = 49;
            this.listViewMsg.UseCompatibleStateImageBehavior = false;
            this.listViewMsg.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Date / Time";
            this.columnHeader5.Width = 127;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "R/T";
            this.columnHeader6.Width = 35;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Time Stamp";
            this.columnHeader1.Width = 127;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Source Device";
            this.columnHeader7.Width = 156;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Data";
            this.columnHeader8.Width = 250;
            // 
            // btnConfigInfo
            // 
            this.btnConfigInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfigInfo.Location = new System.Drawing.Point(644, 32);
            this.btnConfigInfo.Name = "btnConfigInfo";
            this.btnConfigInfo.Size = new System.Drawing.Size(109, 23);
            this.btnConfigInfo.TabIndex = 48;
            this.btnConfigInfo.Text = "Config Info...";
            this.btnConfigInfo.UseVisualStyleBackColor = true;
            this.btnConfigInfo.Click += new System.EventHandler(this.btnConfigInfo_Click);
            // 
            // bkgWorkerTx
            // 
            this.bkgWorkerTx.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bkgWorkerTx_DoWork);
            // 
            // bkgWorkerRx
            // 
            this.bkgWorkerRx.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bkgWorkerRx_DoWork);
            // 
            // frmTankUtilityInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.listViewStatus);
            this.Controls.Add(this.listViewMsg);
            this.Controls.Add(this.btnConfigInfo);
            this.Name = "frmTankUtilityInterface";
            this.Text = "TankUtilityInterface";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.ListView listViewStatus;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.ListView listViewMsg;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.Button btnConfigInfo;
        private System.ComponentModel.BackgroundWorker bkgWorkerTx;
        private System.ComponentModel.BackgroundWorker bkgWorkerRx;
    }
}

