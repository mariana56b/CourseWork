namespace CourseWork
{
    partial class PolygonInCircleFinder
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
            this.tbxResult = new System.Windows.Forms.TextBox();
            this.btnDrawingPointsCircle = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbtAngleSearchAlgorithm = new System.Windows.Forms.RadioButton();
            this.rbtCoordinateComparisonAlgorithm = new System.Windows.Forms.RadioButton();
            this.btnPrintN_Gons = new System.Windows.Forms.Button();
            this.pResult = new System.Windows.Forms.Panel();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbxResult
            // 
            this.tbxResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbxResult.Location = new System.Drawing.Point(14, 120);
            this.tbxResult.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbxResult.Multiline = true;
            this.tbxResult.Name = "tbxResult";
            this.tbxResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbxResult.Size = new System.Drawing.Size(357, 537);
            this.tbxResult.TabIndex = 0;
            // 
            // btnDrawingPointsCircle
            // 
            this.btnDrawingPointsCircle.Location = new System.Drawing.Point(410, 13);
            this.btnDrawingPointsCircle.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnDrawingPointsCircle.Name = "btnDrawingPointsCircle";
            this.btnDrawingPointsCircle.Size = new System.Drawing.Size(600, 44);
            this.btnDrawingPointsCircle.TabIndex = 2;
            this.btnDrawingPointsCircle.Text = "Вивести коло та точки";
            this.btnDrawingPointsCircle.UseVisualStyleBackColor = true;
            this.btnDrawingPointsCircle.Click += new System.EventHandler(this.btnDrawingPointsCircle_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.rbtAngleSearchAlgorithm);
            this.groupBox1.Controls.Add(this.rbtCoordinateComparisonAlgorithm);
            this.groupBox1.Location = new System.Drawing.Point(14, 13);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(390, 99);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Вибір алгоритму ";
            // 
            // rbtAngleSearchAlgorithm
            // 
            this.rbtAngleSearchAlgorithm.AutoSize = true;
            this.rbtAngleSearchAlgorithm.Location = new System.Drawing.Point(20, 65);
            this.rbtAngleSearchAlgorithm.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbtAngleSearchAlgorithm.Name = "rbtAngleSearchAlgorithm";
            this.rbtAngleSearchAlgorithm.Size = new System.Drawing.Size(270, 26);
            this.rbtAngleSearchAlgorithm.TabIndex = 1;
            this.rbtAngleSearchAlgorithm.TabStop = true;
            this.rbtAngleSearchAlgorithm.Text = "Алгоритм пошуку за кутами ";
            this.rbtAngleSearchAlgorithm.UseVisualStyleBackColor = true;
            this.rbtAngleSearchAlgorithm.CheckedChanged += new System.EventHandler(this.rbtAngleSearchAlgorithm_CheckedChanged);
            // 
            // rbtCoordinateComparisonAlgorithm
            // 
            this.rbtCoordinateComparisonAlgorithm.AutoSize = true;
            this.rbtCoordinateComparisonAlgorithm.Location = new System.Drawing.Point(20, 29);
            this.rbtCoordinateComparisonAlgorithm.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbtCoordinateComparisonAlgorithm.Name = "rbtCoordinateComparisonAlgorithm";
            this.rbtCoordinateComparisonAlgorithm.Size = new System.Drawing.Size(303, 26);
            this.rbtCoordinateComparisonAlgorithm.TabIndex = 0;
            this.rbtCoordinateComparisonAlgorithm.TabStop = true;
            this.rbtCoordinateComparisonAlgorithm.Text = "Алгоритм порівняння координат";
            this.rbtCoordinateComparisonAlgorithm.UseVisualStyleBackColor = true;
            this.rbtCoordinateComparisonAlgorithm.CheckedChanged += new System.EventHandler(this.rbtCoordinateComparisonAlgorithm_CheckedChanged);
            // 
            // btnPrintN_Gons
            // 
            this.btnPrintN_Gons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrintN_Gons.Location = new System.Drawing.Point(410, 65);
            this.btnPrintN_Gons.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnPrintN_Gons.Name = "btnPrintN_Gons";
            this.btnPrintN_Gons.Size = new System.Drawing.Size(599, 47);
            this.btnPrintN_Gons.TabIndex = 4;
            this.btnPrintN_Gons.Text = "Виведення всіх правильно вписаних n-кутників";
            this.btnPrintN_Gons.UseVisualStyleBackColor = true;
            this.btnPrintN_Gons.Click += new System.EventHandler(this.btnPrintN_Gons_Click);
            // 
            // pResult
            // 
            this.pResult.Location = new System.Drawing.Point(377, 120);
            this.pResult.Name = "pResult";
            this.pResult.Size = new System.Drawing.Size(633, 538);
            this.pResult.TabIndex = 5;
            // 
            // PolygonInCircleFinder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 22F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.ClientSize = new System.Drawing.Size(1022, 670);
            this.Controls.Add(this.pResult);
            this.Controls.Add(this.btnPrintN_Gons);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnDrawingPointsCircle);
            this.Controls.Add(this.tbxResult);
            this.Font = new System.Drawing.Font("Microsoft Tai Le", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "PolygonInCircleFinder";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PolygonIn Circle Finder";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbxResult;
        private System.Windows.Forms.Button btnDrawingPointsCircle;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbtAngleSearchAlgorithm;
        private System.Windows.Forms.RadioButton rbtCoordinateComparisonAlgorithm;
        private System.Windows.Forms.Button btnPrintN_Gons;
        private System.Windows.Forms.Panel pResult;
    }
}

