namespace Game
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.Scene = new Tao.Platform.Windows.SimpleOpenGlControl();
            this.TravelTime = new System.Windows.Forms.Label();
            this.ArrowBox = new System.Windows.Forms.PictureBox();
            this.Time = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.GameTime = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.ArrowBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // Scene
            // 
            this.Scene.AccumBits = ((byte)(0));
            this.Scene.AutoCheckErrors = false;
            this.Scene.AutoFinish = false;
            this.Scene.AutoMakeCurrent = true;
            this.Scene.AutoSwapBuffers = true;
            this.Scene.BackColor = System.Drawing.Color.Black;
            this.Scene.ColorBits = ((byte)(32));
            this.Scene.DepthBits = ((byte)(16));
            this.Scene.Location = new System.Drawing.Point(4, -1);
            this.Scene.Name = "Scene";
            this.Scene.Size = new System.Drawing.Size(585, 454);
            this.Scene.StencilBits = ((byte)(0));
            this.Scene.TabIndex = 0;
            this.Scene.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Scene_MouseClick);
            this.Scene.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Scene_MouseDoubleClick);
            this.Scene.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Scene_MouseMove);
            // 
            // TravelTime
            // 
            this.TravelTime.AutoSize = true;
            this.TravelTime.BackColor = System.Drawing.Color.Transparent;
            this.TravelTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TravelTime.Location = new System.Drawing.Point(672, 249);
            this.TravelTime.Name = "TravelTime";
            this.TravelTime.Size = new System.Drawing.Size(32, 33);
            this.TravelTime.TabIndex = 1;
            this.TravelTime.Text = "0";
            // 
            // ArrowBox
            // 
            this.ArrowBox.BackColor = System.Drawing.SystemColors.Window;
            this.ArrowBox.Location = new System.Drawing.Point(617, 294);
            this.ArrowBox.Name = "ArrowBox";
            this.ArrowBox.Size = new System.Drawing.Size(111, 110);
            this.ArrowBox.TabIndex = 2;
            this.ArrowBox.TabStop = false;
            // 
            // Time
            // 
            this.Time.AutoSize = true;
            this.Time.BackColor = System.Drawing.Color.Transparent;
            this.Time.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Time.Location = new System.Drawing.Point(670, 103);
            this.Time.Name = "Time";
            this.Time.Size = new System.Drawing.Size(49, 20);
            this.Time.TabIndex = 3;
            this.Time.Text = "07:00";
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(664, 175);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 33);
            this.label1.TabIndex = 4;
            this.label1.Text = "10";
            // 
            // GameTime
            // 
            this.GameTime.AutoSize = true;
            this.GameTime.BackColor = System.Drawing.Color.Transparent;
            this.GameTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.GameTime.Location = new System.Drawing.Point(614, 68);
            this.GameTime.Name = "GameTime";
            this.GameTime.Size = new System.Drawing.Size(132, 13);
            this.GameTime.TabIndex = 5;
            this.GameTime.Text = "\"yyyy-MM-dd HH\':\'mm\':\'ss\"";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(617, 52);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 40);
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(773, 453);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.GameTime);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Time);
            this.Controls.Add(this.ArrowBox);
            this.Controls.Add(this.TravelTime);
            this.Controls.Add(this.Scene);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.ArrowBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Tao.Platform.Windows.SimpleOpenGlControl Scene;
        private System.Windows.Forms.Label TravelTime;
        private System.Windows.Forms.PictureBox ArrowBox;
        private System.Windows.Forms.Label Time;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label GameTime;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

