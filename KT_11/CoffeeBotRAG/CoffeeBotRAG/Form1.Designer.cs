namespace CoffeeBotRAG
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            txtQuestion = new TextBox();
            button1 = new Button();
            rtbResults = new RichTextBox();
            btnLoadFile = new Button();
            btnVectorize = new Button();
            btnLoadVectors = new Button();
            btnSaveVectors = new Button();
            btnShowBlocks = new Button();
            SuspendLayout();

            // txtQuestion
            txtQuestion.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            txtQuestion.Location = new Point(20, 20);
            txtQuestion.Multiline = true;
            txtQuestion.Name = "txtQuestion";
            txtQuestion.PlaceholderText = "☕ Введите ваш вопрос о ТЗ...";
            txtQuestion.Size = new Size(860, 40);
            txtQuestion.TabIndex = 0;

            // rtbResults
            rtbResults.Font = new Font("Consolas", 10F, FontStyle.Regular, GraphicsUnit.Point);
            rtbResults.Location = new Point(20, 80);
            rtbResults.Name = "rtbResults";
            rtbResults.ReadOnly = true;
            rtbResults.Size = new Size(860, 380);
            rtbResults.TabIndex = 2;
            rtbResults.Text = "";

            // button1 - Найти
            button1.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            button1.Location = new Point(20, 480);
            button1.Name = "button1";
            button1.Size = new Size(120, 40);
            button1.TabIndex = 1;
            button1.Text = "🔍 Найти";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;

            // btnLoadFile
            btnLoadFile.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            btnLoadFile.Location = new Point(150, 480);
            btnLoadFile.Name = "btnLoadFile";
            btnLoadFile.Size = new Size(120, 40);
            btnLoadFile.TabIndex = 3;
            btnLoadFile.Text = "📁 Загрузить ТЗ";
            btnLoadFile.UseVisualStyleBackColor = false;
            btnLoadFile.Click += btnLoadFile_Click;

            // btnVectorize
            btnVectorize.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            btnVectorize.Location = new Point(280, 480);
            btnVectorize.Name = "btnVectorize";
            btnVectorize.Size = new Size(120, 40);
            btnVectorize.TabIndex = 4;
            btnVectorize.Text = "⚡ Векторизовать";
            btnVectorize.UseVisualStyleBackColor = false;
            btnVectorize.Click += btnVectorize_Click;

            // btnLoadVectors
            btnLoadVectors.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            btnLoadVectors.Location = new Point(410, 480);
            btnLoadVectors.Name = "btnLoadVectors";
            btnLoadVectors.Size = new Size(140, 40);
            btnLoadVectors.TabIndex = 5;
            btnLoadVectors.Text = "📥 Загрузить векторы";
            btnLoadVectors.UseVisualStyleBackColor = false;
            btnLoadVectors.Click += btnLoadVectors_Click;

            // btnSaveVectors
            btnSaveVectors.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            btnSaveVectors.Location = new Point(560, 480);
            btnSaveVectors.Name = "btnSaveVectors";
            btnSaveVectors.Size = new Size(140, 40);
            btnSaveVectors.TabIndex = 6;
            btnSaveVectors.Text = "💾 Сохранить векторы";
            btnSaveVectors.UseVisualStyleBackColor = false;
            btnSaveVectors.Click += btnSaveVectors_Click;

            // btnShowBlocks
            btnShowBlocks.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
            btnShowBlocks.Location = new Point(710, 480);
            btnShowBlocks.Name = "btnShowBlocks";
            btnShowBlocks.Size = new Size(140, 40);
            btnShowBlocks.TabIndex = 7;
            btnShowBlocks.Text = "📊 Показать блоки";
            btnShowBlocks.UseVisualStyleBackColor = false;
            btnShowBlocks.Click += btnShowBlocks_Click;

            // Form settings
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(255, 248, 240);
            ClientSize = new Size(900, 540);
            Controls.Add(btnShowBlocks);
            Controls.Add(btnSaveVectors);
            Controls.Add(btnLoadVectors);
            Controls.Add(btnVectorize);
            Controls.Add(btnLoadFile);
            Controls.Add(rtbResults);
            Controls.Add(button1);
            Controls.Add(txtQuestion);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "☕ CoffeeBot RAG - система для ТЗ";
            ResumeLayout(false);
            PerformLayout();
        }

        private TextBox txtQuestion;
        private Button button1;
        private RichTextBox rtbResults;
        private Button btnLoadFile;
        private Button btnVectorize;
        private Button btnLoadVectors;
        private Button btnSaveVectors;
        private Button btnShowBlocks;
    }
}