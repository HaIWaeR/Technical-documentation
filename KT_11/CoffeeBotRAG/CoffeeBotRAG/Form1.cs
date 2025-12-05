using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.Json;
using System.Drawing;

namespace CoffeeBotRAG
{
    public partial class Form1 : Form
    {
        private List<TzBlock> tzBlocks = new List<TzBlock>();
        private string loadedFileName = "";
        private bool isVectorized = false;
        private SimpleVectorizer vectorizer = new SimpleVectorizer();
        private TzParser tzParser = new TzParser();
        private string vectorsFilePath = "";
        private List<(TzBlock block, float similarity, string topic)> currentSearchResults = new List<(TzBlock block, float similarity, string topic)>();

        public Form1()
        {
            InitializeComponent();
            ApplyCoffeeTheme();
            this.Text = "☕ CoffeeBot RAG - система для ТЗ";
        }

        private void ApplyCoffeeTheme()
        {
            // Фон формы
            this.BackColor = CoffeeTheme.CoffeeMilk;

            // Стилизация текстового поля вопроса
            txtQuestion.BackColor = Color.White;
            txtQuestion.ForeColor = CoffeeTheme.CoffeeTextDark;
            txtQuestion.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            txtQuestion.PlaceholderText = "☕ Введите вопрос о ТЗ...";
            txtQuestion.BorderStyle = BorderStyle.FixedSingle;

            // Стилизация кнопок
            CoffeeTheme.StyleButton(button1, CoffeeTheme.CoffeeDark, CoffeeTheme.CoffeeTextLight, "🔍 Найти");
            CoffeeTheme.StyleButton(btnLoadFile, CoffeeTheme.CoffeeDark, CoffeeTheme.CoffeeTextLight, "📁 Загрузить ТЗ");
            CoffeeTheme.StyleButton(btnVectorize, CoffeeTheme.CoffeeGold, Color.White, "⚡ Векторизовать");
            CoffeeTheme.StyleButton(btnLoadVectors, CoffeeTheme.CoffeeMedium, CoffeeTheme.CoffeeTextLight, "📥 Загрузить векторы");
            CoffeeTheme.StyleButton(btnSaveVectors, CoffeeTheme.CoffeeMedium, CoffeeTheme.CoffeeTextLight, "💾 Сохранить векторы");
            CoffeeTheme.StyleButton(btnShowBlocks, CoffeeTheme.CoffeeDark, CoffeeTheme.CoffeeTextLight, "📊 Показать блоки");

            // Стилизация RichTextBox для результатов
            rtbResults.BackColor = Color.White;
            rtbResults.ForeColor = CoffeeTheme.CoffeeTextDark;
            rtbResults.Font = CoffeeTheme.MonoFont;
            rtbResults.BorderStyle = BorderStyle.FixedSingle;

            // Приветственный текст с кофейной тематикой
            rtbResults.Text = @"☕ ДОБРО ПОЖАЛОВАТЬ В COFFEEBOT RAG!
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

🎯 Универсальная RAG система для анализа ТЗ
   с приятной кофейной атмосферой

📌 КАК ИСПОЛЬЗОВАТЬ:
   1. 📁 'Загрузить ТЗ' - выберите файл .txt/.md
   2. ⚡ 'Векторизовать' - создаст семантические векторы
   3. 💾 'Сохранить векторы' - сохранит в JSON файл
   4. 📥 'Загрузить векторы' - загрузит готовые векторы
   5. 📊 'Показать блоки' - просмотр структуры ТЗ
   6. 🔍 'Найти' - семантический поиск по ТЗ

📁 ПОДДЕРЖИВАЕМЫЕ ФОРМАТЫ:
   • Markdown (# заголовки)
   • Простой текст с разделителями (|)
   • Любой структурированный текст

💡 СОВЕТ: Начните с загрузки ТЗ, затем векторизуйте
   для точного семантического поиска!

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
☕ Готов к работе! Выберите действие...";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string question = txtQuestion.Text.Trim();

            if (string.IsNullOrEmpty(question))
            {
                MessageBox.Show("☕ Введите вопрос!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (tzBlocks.Count == 0)
            {
                MessageBox.Show("☕ Сначала загрузите ТЗ или векторы!",
                    "Нет данных", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (isVectorized)
            {
                SearchWithVectors(question);
            }
            else
            {
                SearchSimple(question);
            }
        }

        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Текстовые файлы (*.txt;*.md)|*.txt;*.md|Все файлы (*.*)|*.*";
                dialog.Title = "☕ Выберите файл с ТЗ";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        loadedFileName = Path.GetFileName(dialog.FileName);
                        string fileContent = File.ReadAllText(dialog.FileName, Encoding.UTF8);

                        string[] lines = fileContent.Split('\n');
                        tzBlocks = tzParser.ParseText(lines);
                        isVectorized = false;

                        ShowLoadedBlocksInfo();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"☕ Ошибка загрузки: {ex.Message}",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ShowLoadedBlocksInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"☕ ФАЙЛ ЗАГРУЖЕН!");
            sb.AppendLine($"📁 Файл: {loadedFileName}");
            sb.AppendLine($"📊 Распознано блоков: {tzBlocks.Count}");
            sb.AppendLine($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

            int showCount = Math.Min(3, tzBlocks.Count);
            sb.AppendLine($"📋 Примеры блоков:");

            for (int i = 0; i < showCount; i++)
            {
                var block = tzBlocks[i];
                sb.AppendLine($"\n☕ Блок {i + 1}:");
                sb.AppendLine($"   📍 Раздел: {block.Section}");
                sb.AppendLine($"   📝 Текст: {block.Content}");
                sb.AppendLine($"   ────────────────────────────────");
            }

            if (tzBlocks.Count > showCount)
            {
                sb.AppendLine($"\n☕ ... и ещё {tzBlocks.Count - showCount} блоков");
            }

            sb.AppendLine($"\n💡 Нажмите '📊 Показать блоки' для полного просмотра");

            rtbResults.Text = sb.ToString();
        }

        private void btnVectorize_Click(object sender, EventArgs e)
        {
            if (tzBlocks.Count == 0)
            {
                MessageBox.Show("☕ Сначала загрузите ТЗ!",
                    "Нет данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                rtbResults.Text = "☕ Начинаю векторизацию...\n";
                Application.DoEvents();

                int vectorizedCount = 0;
                int total = tzBlocks.Count;

                for (int i = 0; i < total; i++)
                {
                    tzBlocks[i].Vector = vectorizer.VectorizeText(tzBlocks[i].DisplayText);
                    vectorizedCount++;

                    if (i % 5 == 0 || i == total - 1)
                    {
                        rtbResults.Text = $"☕ Векторизация...\n\n" +
                                         $"📊 Обработано: {vectorizedCount} из {total} блоков";
                        Application.DoEvents();
                    }
                }

                isVectorized = true;

                rtbResults.Text = $"✅ ВЕКТОРИЗАЦИЯ ЗАВЕРШЕНА!\n\n" +
                                 $"📊 Обработано блоков: {vectorizedCount}\n" +
                                 $"🔢 Размер вектора: 10 измерений\n\n" +
                                 $"💡 Теперь нажмите '💾 Сохранить векторы'";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"☕ Ошибка векторизации: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                rtbResults.Text = "❌ Ошибка векторизации";
            }
        }

        private void btnSaveVectors_Click(object sender, EventArgs e)
        {
            if (!isVectorized || tzBlocks.Count == 0)
            {
                MessageBox.Show("☕ Сначала загрузите и векторизуйте ТЗ!",
                    "Нет данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "JSON файлы (*.json)|*.json|Все файлы (*.*)|*.*";
                dialog.FileName = $"vectors_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                dialog.Title = "☕ Сохранить векторы в файл";
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                dialog.DefaultExt = "json";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        vectorsFilePath = dialog.FileName;
                        SaveVectorsToFile(vectorsFilePath);
                        FileInfo info = new FileInfo(vectorsFilePath);

                        rtbResults.Text = $"✅ ВЕКТОРЫ СОХРАНЕНЫ!\n\n" +
                                         $"📁 Файл: {Path.GetFileName(vectorsFilePath)}\n" +
                                         $"📍 Путь: {vectorsFilePath}\n" +
                                         $"📏 Размер: {info.Length} байт\n" +
                                         $"📊 Блоков: {tzBlocks.Count}\n\n" +
                                         $"💡 Этот файл можно загрузить в GitHub";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"☕ Ошибка сохранения: {ex.Message}",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnLoadVectors_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "JSON файлы (*.json)|*.json|Все файлы (*.*)|*.*";
                dialog.Title = "☕ Выберите файл с векторами";
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    vectorsFilePath = dialog.FileName;
                    LoadVectorsFromFile(vectorsFilePath);
                }
            }
        }

        private void btnShowBlocks_Click(object sender, EventArgs e)
        {
            if (tzBlocks.Count == 0)
            {
                MessageBox.Show("☕ Нет загруженных блоков!", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ShowAllBlocksWindow();
        }

        private void ShowAllBlocksWindow()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"☕ ВСЕ БЛОКИ ТЗ ({tzBlocks.Count} шт.)");
            sb.AppendLine($"📁 Файл: {loadedFileName}");
            sb.AppendLine($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

            var groupedBlocks = tzBlocks.GroupBy(b => b.Section);
            int blockCounter = 1;

            foreach (var group in groupedBlocks)
            {
                sb.AppendLine($"\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                sb.AppendLine($"📂 РАЗДЕЛ: {group.Key}");
                sb.AppendLine($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

                foreach (var block in group)
                {
                    sb.AppendLine($"\n☕ Блок #{blockCounter++}:");
                    sb.AppendLine($"   📍 Раздел: {block.Section}");
                    sb.AppendLine($"   📝 Содержание: {block.Content}");
                    sb.AppendLine($"   ────────────────────────────────");
                }
            }

            Form blocksForm = new Form();
            blocksForm.Text = $"☕ Просмотр блоков ТЗ - {loadedFileName}";
            blocksForm.Size = new Size(1000, 700);
            blocksForm.StartPosition = FormStartPosition.CenterParent;
            blocksForm.BackColor = CoffeeTheme.CoffeeMilk;

            // Панель с кнопками
            Panel topPanel = new Panel();
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 50;
            topPanel.BackColor = CoffeeTheme.CoffeeDark;

            // Кнопка копирования
            Button btnCopy = new Button();
            btnCopy.Text = "📋 Скопировать всё";
            btnCopy.Size = new Size(150, 35);
            btnCopy.Location = new Point(20, 7);
            CoffeeTheme.StyleButton(btnCopy, CoffeeTheme.CoffeeLight, CoffeeTheme.CoffeeTextLight);
            btnCopy.Click += (s, e) =>
            {
                Clipboard.SetText(sb.ToString());
                MessageBox.Show("☕ Все блоки скопированы в буфер обмена", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            topPanel.Controls.Add(btnCopy);

            // RichTextBox для отображения
            RichTextBox rtb = CoffeeTheme.CreateCoffeeTextBox();
            rtb.Dock = DockStyle.Fill;
            rtb.Text = sb.ToString();

            blocksForm.Controls.Add(rtb);
            blocksForm.Controls.Add(topPanel);
            topPanel.BringToFront();

            blocksForm.Show();
        }

        private void LoadVectorsFromFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    rtbResults.Text = $"❌ Файл не найден: {filePath}";
                    return;
                }

                string json = File.ReadAllText(filePath, Encoding.UTF8);
                var data = JsonSerializer.Deserialize<VectorFileData>(json);

                if (data != null)
                {
                    tzBlocks.Clear();
                    foreach (var vectorData in data.Vectors)
                    {
                        tzBlocks.Add(new TzBlock
                        {
                            Section = vectorData.Section,
                            Content = vectorData.Content,
                            Vector = vectorData.Vector
                        });
                    }

                    loadedFileName = data.SourceFile;
                    isVectorized = true;

                    rtbResults.Text = $"✅ ВЕКТОРЫ ЗАГРУЖЕНЫ!\n\n" +
                                     $"📁 Файл: {Path.GetFileName(filePath)}\n" +
                                     $"📊 Блоков: {tzBlocks.Count}\n\n" +
                                     $"💡 Теперь можно выполнять поиск";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"☕ Ошибка загрузки: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string DetectTopic(string text)
        {
            text = text.ToLower();

            Dictionary<string, string[]> topicPatterns = new Dictionary<string, string[]>
            {
                { "технический", new[] { "техническ", "техник", "инженер", "конструкц", "схем", "чертеж" } },
                { "функциональный", new[] { "функц", "возможност", "операц", "действ", "работ", "выполнен" } },
                { "оплата", new[] { "оплат", "плат", "qr", "код", "деньг", "стоимост", "чек", "расчет" } },
                { "доставка", new[] { "достав", "курьер", "адрес", "получен", "самовывоз", "забрат", "привез" } },
                { "интерфейс", new[] { "интерфейс", "пользователь", "удобств", "навигац", "уведомлен", "кнопк" } },
                { "безопасность", new[] { "безопас", "защит", "данн", "конфиденци", "шифрован", "информац" } },
                { "производительность", new[] { "производительн", "скорост", "быстродейств", "отклик", "время" } },
                { "документация", new[] { "документац", "документ", "инструкц", "руководств", "описан" } },
                { "проект", new[] { "проект", "разработк", "создан", "реализац", "внедрен", "запуск" } },
                { "система", new[] { "систем", "подсистем", "компонент", "модуль", "архитектур" } },
                { "требование", new[] { "требован", "услов", "критери", "стандарт", "норма" } },
                { "тестирование", new[] { "тестирован", "проверк", "валидац", "верификац", "тест" } }
            };

            foreach (var topic in topicPatterns)
            {
                foreach (string pattern in topic.Value)
                {
                    if (text.Contains(pattern))
                        return topic.Key;
                }
            }

            return "общее";
        }

        private void SearchWithVectors(string question)
        {
            List<string> results = new List<string>();
            results.Add($"🎯 СЕМАНТИЧЕСКИЙ ПОИСК (RAG)");
            results.Add($"📝 Запрос: \"{question}\"");
            results.Add($"📁 Файл: {loadedFileName}");
            results.Add($"📊 Блоков ТЗ: {tzBlocks.Count}");
            results.Add($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

            string questionTopic = DetectTopic(question);
            results.Add($"🏷️  Тема запроса: {questionTopic}");

            float[] questionVector = vectorizer.VectorizeText(question);
            currentSearchResults.Clear();

            foreach (var block in tzBlocks)
            {
                if (block.Vector != null)
                {
                    float similarity = vectorizer.CosineSimilarity(questionVector, block.Vector);
                    string blockTopic = DetectTopic(block.Content);

                    if (questionTopic == blockTopic && questionTopic != "общее")
                    {
                        similarity *= 1.3f;
                    }
                    else if (questionTopic != "общее" && blockTopic != "общее" && questionTopic != blockTopic)
                    {
                        similarity *= 0.4f;
                    }

                    float threshold = 0.25f;
                    if (similarity > threshold)
                    {
                        currentSearchResults.Add((block, similarity, blockTopic));
                    }
                }
            }

            currentSearchResults.Sort((a, b) => b.similarity.CompareTo(a.similarity));

            if (currentSearchResults.Count > 0)
            {
                var relevantResults = currentSearchResults.Where(r => r.similarity > 0.4f).ToList();
                int count = Math.Min(5, relevantResults.Count);

                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        var result = relevantResults[i];
                        results.Add($"\n☕ РЕЗУЛЬТАТ #{i + 1}");
                        results.Add($"📈 Сходство: {result.similarity:F3}");
                        results.Add($"🏷️  Тема: {result.topic}");
                        results.Add($"📄 Раздел: {result.block.Section}");
                        results.Add($"📝 Содержание: {result.block.Content}");
                        results.Add($"────────────────────────────────────────────────");
                    }

                    results.Add($"\n✅ Найдено: {currentSearchResults.Count} блоков");
                    results.Add($"🎯 Релевантных: {relevantResults.Count}");

                    // Добавляем кнопку для просмотра остальных результатов
                    if (relevantResults.Count > 5)
                    {
                        results.Add($"\n🔍 Показано: 5 из {relevantResults.Count} релевантных");
                        results.Add($"📋 Ещё доступно: {relevantResults.Count - 5} блоков");
                        results.Add($"\n💡 Нажмите 'Показать остальные' для просмотра всех результатов");

                        // Добавляем кнопку на форму
                        AddShowMoreButton(question, relevantResults);
                    }
                }
                else
                {
                    results.Add($"\n⚠️  Низкая релевантность");
                }
            }
            else
            {
                results.Add($"\n❌ Похожих блоков не найдено.");
            }

            rtbResults.Text = string.Join("\n", results);
        }

        // Метод для добавления кнопки "Показать остальные"
        private void AddShowMoreButton(string question, List<(TzBlock block, float similarity, string topic)> relevantResults)
        {
            // Удаляем старую кнопку, если она есть
            foreach (Control control in this.Controls)
            {
                if (control is Button btn && btn.Name == "btnShowMoreResults")
                {
                    this.Controls.Remove(control);
                    control.Dispose();
                    break;
                }
            }

            // Создаем новую кнопку
            Button btnShowMore = new Button();
            btnShowMore.Name = "btnShowMoreResults";
            btnShowMore.Text = "📋 Показать все результаты";
            btnShowMore.Location = new Point(710, 430);
            btnShowMore.Size = new Size(170, 35);
            CoffeeTheme.StyleButton(btnShowMore, CoffeeTheme.CoffeeGold, Color.White);
            btnShowMore.Click += (sender, e) => ShowAllResultsWindow(question, relevantResults);

            this.Controls.Add(btnShowMore);
            btnShowMore.BringToFront();
        }

        // Метод для показа всех результатов в отдельном окне
        private void ShowAllResultsWindow(string question, List<(TzBlock block, float similarity, string topic)> relevantResults)
        {
            Form resultsForm = new Form();
            resultsForm.Text = $"☕ Все результаты поиска: \"{question}\"";
            resultsForm.Size = new Size(1000, 700);
            resultsForm.StartPosition = FormStartPosition.CenterParent;
            resultsForm.BackColor = CoffeeTheme.CoffeeMilk;

            // Панель с кнопками
            Panel topPanel = new Panel();
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 60;
            topPanel.BackColor = CoffeeTheme.CoffeeDark;

            // Заголовок
            Label lblTitle = new Label();
            lblTitle.Text = $"📊 Все результаты ({relevantResults.Count} блоков)";
            lblTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 15);

            // Кнопка копирования
            Button btnCopy = new Button();
            btnCopy.Text = "📋 Скопировать всё";
            btnCopy.Size = new Size(150, 35);
            btnCopy.Location = new Point(250, 12);
            CoffeeTheme.StyleButton(btnCopy, CoffeeTheme.CoffeeLight, CoffeeTheme.CoffeeTextLight);
            btnCopy.Click += (s, e) =>
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"☕ ВСЕ РЕЗУЛЬТАТЫ ПОИСКА");
                sb.AppendLine($"📝 Запрос: \"{question}\"");
                sb.AppendLine($"📁 Файл: {loadedFileName}");
                sb.AppendLine($"📊 Найдено блоков: {relevantResults.Count}");
                sb.AppendLine($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

                for (int i = 0; i < relevantResults.Count; i++)
                {
                    var result = relevantResults[i];
                    sb.AppendLine($"\n☕ РЕЗУЛЬТАТ #{i + 1}");
                    sb.AppendLine($"📈 Сходство: {result.similarity:F3}");
                    sb.AppendLine($"🏷️  Тема: {result.topic}");
                    sb.AppendLine($"📄 Раздел: {result.block.Section}");
                    sb.AppendLine($"📝 Содержание: {result.block.Content}");
                    sb.AppendLine($"────────────────────────────────────────────────");
                }

                Clipboard.SetText(sb.ToString());
                MessageBox.Show("☕ Все результаты скопированы в буфер обмена", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            topPanel.Controls.Add(lblTitle);
            topPanel.Controls.Add(btnCopy);

            // RichTextBox для отображения результатов
            RichTextBox rtb = CoffeeTheme.CreateCoffeeTextBox();
            rtb.Dock = DockStyle.Fill;

            // Формируем содержимое
            StringBuilder sbContent = new StringBuilder();
            sbContent.AppendLine($"☕ ВСЕ РЕЗУЛЬТАТЫ ПОИСКА");
            sbContent.AppendLine($"📝 Запрос: \"{question}\"");
            sbContent.AppendLine($"📁 Файл: {loadedFileName}");
            sbContent.AppendLine($"📊 Найдено блоков: {relevantResults.Count}");
            sbContent.AppendLine($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

            for (int i = 0; i < relevantResults.Count; i++)
            {
                var result = relevantResults[i];
                sbContent.AppendLine($"\n☕ РЕЗУЛЬТАТ #{i + 1}");
                sbContent.AppendLine($"📈 Сходство: {result.similarity:F3}");
                sbContent.AppendLine($"🏷️  Тема: {result.topic}");
                sbContent.AppendLine($"📄 Раздел: {result.block.Section}");
                sbContent.AppendLine($"📝 Содержание: {result.block.Content}");
                sbContent.AppendLine($"────────────────────────────────────────────────");
            }

            rtb.Text = sbContent.ToString();

            // Добавляем подсветку темы цветом
            for (int i = 0; i < relevantResults.Count; i++)
            {
                int lineIndex = 6 + i * 7; // Позиция строки с темой
                string topic = relevantResults[i].topic;
                Color topicColor = CoffeeTheme.GetTopicColor(topic);

                // Находим позицию строки с темой
                int start = rtb.GetFirstCharIndexFromLine(lineIndex);
                if (start >= 0)
                {
                    int end = rtb.GetFirstCharIndexFromLine(lineIndex + 1);
                    if (end < 0) end = rtb.Text.Length;

                    rtb.Select(start, end - start);
                    rtb.SelectionColor = topicColor;
                }
            }

            // Возвращаем курсор в начало
            rtb.Select(0, 0);

            resultsForm.Controls.Add(rtb);
            resultsForm.Controls.Add(topPanel);
            topPanel.BringToFront();

            resultsForm.Show();

            // Удаляем кнопку "Показать все результаты" после открытия окна
            foreach (Control control in this.Controls)
            {
                if (control is Button btn && btn.Name == "btnShowMoreResults")
                {
                    this.Controls.Remove(control);
                    control.Dispose();
                    break;
                }
            }
        }

        private void SearchSimple(string question)
        {
            List<string> results = new List<string>();
            results.Add($"🔍 ПРОСТОЙ ПОИСК (по словам)");
            results.Add($"📝 Запрос: \"{question}\"");
            results.Add($"📁 Файл: {loadedFileName}");
            results.Add($"📊 Блоков ТЗ: {tzBlocks.Count}");
            results.Add($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

            string questionLower = question.ToLower();
            int foundCount = 0;
            var simpleResults = new List<TzBlock>();

            foreach (var block in tzBlocks)
            {
                string fullText = block.Content.ToLower();
                string[] words = questionLower.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                int matches = 0;

                foreach (string word in words)
                {
                    if (word.Length > 2 && fullText.Contains(word))
                        matches++;
                }

                if (matches > 0)
                {
                    foundCount++;
                    simpleResults.Add(block);
                    results.Add($"\n☕ РЕЗУЛЬТАТ #{foundCount}");
                    results.Add($"📈 Совпадений: {matches}");
                    results.Add($"📄 Раздел: {block.Section}");
                    results.Add($"📝 Содержание: {block.Content}");
                    results.Add($"────────────────────────────────────────────────");
                }
            }

            if (foundCount > 0)
            {
                results.Add($"\n✅ Найдено: {foundCount} блоков");
                results.Add($"💡 Для семантического поиска нажмите '⚡ Векторизовать'");

                // Добавляем кнопку для просмотра всех результатов простого поиска
                if (foundCount > 5)
                {
                    results.Add($"\n🔍 Показано: 5 из {foundCount} результатов");
                    results.Add($"📋 Ещё доступно: {foundCount - 5} блоков");
                    results.Add($"\n💡 Нажмите 'Показать остальные' для просмотра всех результатов");

                    AddShowMoreSimpleButton(question, simpleResults);
                }
            }
            else
            {
                results.Add($"\n❌ Совпадений не найдено.");
            }

            rtbResults.Text = string.Join("\n", results);
        }

        // Метод для добавления кнопки "Показать остальные" для простого поиска
        private void AddShowMoreSimpleButton(string question, List<TzBlock> simpleResults)
        {
            // Удаляем старую кнопку, если она есть
            foreach (Control control in this.Controls)
            {
                if (control is Button btn && btn.Name == "btnShowMoreSimpleResults")
                {
                    this.Controls.Remove(control);
                    control.Dispose();
                    break;
                }
            }

            // Создаем новую кнопку
            Button btnShowMore = new Button();
            btnShowMore.Name = "btnShowMoreSimpleResults";
            btnShowMore.Text = "📋 Показать все результаты";
            btnShowMore.Location = new Point(710, 430);
            btnShowMore.Size = new Size(170, 35);
            CoffeeTheme.StyleButton(btnShowMore, CoffeeTheme.CoffeeGold, Color.White);
            btnShowMore.Click += (sender, e) => ShowAllSimpleResultsWindow(question, simpleResults);

            this.Controls.Add(btnShowMore);
            btnShowMore.BringToFront();
        }

        // Метод для показа всех результатов простого поиска
        private void ShowAllSimpleResultsWindow(string question, List<TzBlock> simpleResults)
        {
            Form resultsForm = new Form();
            resultsForm.Text = $"☕ Все результаты простого поиска: \"{question}\"";
            resultsForm.Size = new Size(1000, 700);
            resultsForm.StartPosition = FormStartPosition.CenterParent;
            resultsForm.BackColor = CoffeeTheme.CoffeeMilk;

            // Панель с кнопками
            Panel topPanel = new Panel();
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 60;
            topPanel.BackColor = CoffeeTheme.CoffeeDark;

            // Заголовок
            Label lblTitle = new Label();
            lblTitle.Text = $"📊 Все результаты простого поиска ({simpleResults.Count} блоков)";
            lblTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 15);

            // Кнопка копирования
            Button btnCopy = new Button();
            btnCopy.Text = "📋 Скопировать всё";
            btnCopy.Size = new Size(150, 35);
            btnCopy.Location = new Point(350, 12);
            CoffeeTheme.StyleButton(btnCopy, CoffeeTheme.CoffeeLight, CoffeeTheme.CoffeeTextLight);

            topPanel.Controls.Add(lblTitle);
            topPanel.Controls.Add(btnCopy);

            // RichTextBox для отображения результатов
            RichTextBox rtb = CoffeeTheme.CreateCoffeeTextBox();
            rtb.Dock = DockStyle.Fill;

            // Формируем содержимое
            StringBuilder sbContent = new StringBuilder();
            sbContent.AppendLine($"☕ ВСЕ РЕЗУЛЬТАТЫ ПРОСТОГО ПОИСКА");
            sbContent.AppendLine($"📝 Запрос: \"{question}\"");
            sbContent.AppendLine($"📁 Файл: {loadedFileName}");
            sbContent.AppendLine($"📊 Найдено блоков: {simpleResults.Count}");
            sbContent.AppendLine($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

            for (int i = 0; i < simpleResults.Count; i++)
            {
                var block = simpleResults[i];
                string contentLower = block.Content.ToLower();
                string questionLower = question.ToLower();
                string[] words = questionLower.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                int matches = 0;

                foreach (string word in words)
                {
                    if (word.Length > 2 && contentLower.Contains(word))
                        matches++;
                }

                sbContent.AppendLine($"\n☕ РЕЗУЛЬТАТ #{i + 1}");
                sbContent.AppendLine($"📈 Совпадений: {matches}");
                sbContent.AppendLine($"📄 Раздел: {block.Section}");
                sbContent.AppendLine($"📝 Содержание: {block.Content}");
                sbContent.AppendLine($"────────────────────────────────────────────────");
            }

            rtb.Text = sbContent.ToString();

            btnCopy.Click += (s, e) =>
            {
                Clipboard.SetText(sbContent.ToString());
                MessageBox.Show("☕ Все результаты скопированы в буфер обмена", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            resultsForm.Controls.Add(rtb);
            resultsForm.Controls.Add(topPanel);
            topPanel.BringToFront();

            resultsForm.Show();

            // Удаляем кнопку "Показать все результаты" после открытия окна
            foreach (Control control in this.Controls)
            {
                if (control is Button btn && btn.Name == "btnShowMoreSimpleResults")
                {
                    this.Controls.Remove(control);
                    control.Dispose();
                    break;
                }
            }
        }

        private void SaveVectorsToFile(string filePath)
        {
            try
            {
                var vectorData = new List<VectorData>();
                foreach (var block in tzBlocks)
                {
                    vectorData.Add(new VectorData
                    {
                        Section = block.Section,
                        Content = block.Content,
                        Vector = block.Vector
                    });
                }

                var data = new VectorFileData
                {
                    Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    SourceFile = loadedFileName,
                    BlockCount = tzBlocks.Count,
                    VectorSize = 10,
                    Vectors = vectorData
                };

                string json = JsonSerializer.Serialize(data, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });

                File.WriteAllText(filePath, json, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка сохранения: {ex.Message}");
            }
        }
    }

    public class VectorFileData
    {
        public string Timestamp { get; set; } = "";
        public string SourceFile { get; set; } = "";
        public int BlockCount { get; set; }
        public int VectorSize { get; set; }
        public List<VectorData> Vectors { get; set; } = new List<VectorData>();
    }

    public class VectorData
    {
        public string Section { get; set; } = "";
        public string Content { get; set; } = "";
        public float[]? Vector { get; set; }
    }
}