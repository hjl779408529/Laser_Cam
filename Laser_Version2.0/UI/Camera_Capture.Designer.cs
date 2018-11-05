namespace Laser_Version2._0.UI
{
    partial class Camera_Capture
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
            this.Camera_Pic = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.Camera_Pic)).BeginInit();
            this.SuspendLayout();
            // 
            // Camera_Pic
            // 
            this.Camera_Pic.Location = new System.Drawing.Point(12, 12);
            this.Camera_Pic.Name = "Camera_Pic";
            this.Camera_Pic.Size = new System.Drawing.Size(702, 536);
            this.Camera_Pic.TabIndex = 0;
            this.Camera_Pic.TabStop = false;
            // 
            // Camera_Capture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(731, 564);
            this.Controls.Add(this.Camera_Pic);
            this.Name = "Camera_Capture";
            this.Text = "Camera_Capture";
            ((System.ComponentModel.ISupportInitialize)(this.Camera_Pic)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox Camera_Pic;
    }
}