namespace Messenger_For_Team_Server
{
    partial class serverF
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtIP = new System.Windows.Forms.TextBox();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.lbPort = new System.Windows.Forms.Label();
            this.lbIP = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.lbLog = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(114, 57);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(256, 28);
            this.txtIP.TabIndex = 0;
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(440, 57);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(117, 28);
            this.txtPort.TabIndex = 1;
            // 
            // lbPort
            // 
            this.lbPort.AutoSize = true;
            this.lbPort.Location = new System.Drawing.Point(393, 60);
            this.lbPort.Name = "lbPort";
            this.lbPort.Size = new System.Drawing.Size(41, 18);
            this.lbPort.TabIndex = 2;
            this.lbPort.Text = "Port";
            // 
            // lbIP
            // 
            this.lbIP.AutoSize = true;
            this.lbIP.Location = new System.Drawing.Point(12, 60);
            this.lbIP.Name = "lbIP";
            this.lbIP.Size = new System.Drawing.Size(96, 18);
            this.lbIP.TabIndex = 3;
            this.lbIP.Text = "IP Address";
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(440, 156);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(233, 359);
            this.txtLog.TabIndex = 4;
            // 
            // lbLog
            // 
            this.lbLog.AutoSize = true;
            this.lbLog.Location = new System.Drawing.Point(437, 125);
            this.lbLog.Name = "lbLog";
            this.lbLog.Size = new System.Drawing.Size(97, 18);
            this.lbLog.TabIndex = 5;
            this.lbLog.Text = "Server Log";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(571, 36);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(102, 67);
            this.btnStart.TabIndex = 7;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // serverF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(701, 538);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.lbLog);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.lbIP);
            this.Controls.Add(this.lbPort);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.txtIP);
            this.Name = "serverF";
            this.Text = "Server";
            this.Load += new System.EventHandler(this.serverF_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label lbPort;
        private System.Windows.Forms.Label lbIP;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Label lbLog;
        private System.Windows.Forms.Button btnStart;
    }
}

