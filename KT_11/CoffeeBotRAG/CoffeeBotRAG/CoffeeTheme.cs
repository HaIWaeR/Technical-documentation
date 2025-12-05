using System;
using System.Drawing;
using System.Windows.Forms;

namespace CoffeeBotRAG
{
    public static class CoffeeTheme
    {
        // Цветовая палитра кофейной темы
        public static Color CoffeeDark = Color.FromArgb(111, 78, 55);      // Темный шоколад
        public static Color CoffeeMedium = Color.FromArgb(150, 100, 70);   // Средний кофе
        public static Color CoffeeLight = Color.FromArgb(196, 164, 132);   // Светлый кофе
        public static Color CoffeeCream = Color.FromArgb(245, 222, 179);   // Кремовый
        public static Color CoffeeMilk = Color.FromArgb(255, 248, 240);    // Молочный
        public static Color CoffeeGold = Color.FromArgb(218, 165, 32);     // Золотистый
        public static Color CoffeeTextDark = Color.FromArgb(76, 47, 39);   // Темный текст
        public static Color CoffeeTextLight = Color.White;                 // Светлый текст
        public static Color CoffeeSuccess = Color.FromArgb(46, 125, 50);   // Зеленый успех
        public static Color CoffeeWarning = Color.FromArgb(237, 108, 2);   // Оранжевый предупреждение
        public static Color CoffeeError = Color.FromArgb(198, 40, 40);     // Красный ошибка

        // Шрифты
        public static Font TitleFont = new Font("Segoe UI", 14, FontStyle.Bold);
        public static Font SubtitleFont = new Font("Segoe UI", 12, FontStyle.Regular);
        public static Font NormalFont = new Font("Segoe UI", 10, FontStyle.Regular);
        public static Font MonoFont = new Font("Consolas", 10, FontStyle.Regular);
        public static Font ButtonFont = new Font("Segoe UI", 10, FontStyle.Bold);

        // Стилизация кнопки
        public static void StyleButton(Button button, Color backgroundColor, Color textColor, string text = null)
        {
            button.BackColor = backgroundColor;
            button.ForeColor = textColor;
            button.Font = ButtonFont;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderColor = Color.FromArgb(150, CoffeeDark);
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(
                Math.Min(backgroundColor.R + 20, 255),
                Math.Min(backgroundColor.G + 20, 255),
                Math.Min(backgroundColor.B + 20, 255));
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(
                Math.Max(backgroundColor.R - 20, 0),
                Math.Max(backgroundColor.G - 20, 0),
                Math.Max(backgroundColor.B - 20, 0));
            button.Cursor = Cursors.Hand;
            button.Padding = new Padding(8, 5, 8, 5);

            if (text != null)
                button.Text = text;
        }

        // Создание RichTextBox в кофейном стиле
        public static RichTextBox CreateCoffeeTextBox(bool readOnly = true)
        {
            RichTextBox rtb = new RichTextBox();
            rtb.BackColor = Color.White;
            rtb.ForeColor = CoffeeTextDark;
            rtb.Font = MonoFont;
            rtb.BorderStyle = BorderStyle.FixedSingle;
            rtb.ReadOnly = readOnly;
            return rtb;
        }

        // Создание TextBox в кофейном стиле
        public static TextBox CreateCoffeeInputBox(string placeholder = "")
        {
            TextBox txt = new TextBox();
            txt.BackColor = Color.White;
            txt.ForeColor = CoffeeTextDark;
            txt.Font = new Font("Segoe UI", 11);
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.PlaceholderText = placeholder;
            return txt;
        }

        // Создание формы в кофейном стиле
        public static void StyleForm(Form form, string title = "")
        {
            form.BackColor = CoffeeMilk;
            form.Font = NormalFont;
            if (!string.IsNullOrEmpty(title))
                form.Text = $"☕ {title}";
        }

        // Получение цвета для темы (расширенный вариант)
        public static Color GetTopicColor(string topic)
        {
            return topic.ToLower() switch
            {
                "оплата" => Color.FromArgb(76, 175, 80),          // Зеленый
                "доставка" => Color.FromArgb(33, 150, 243),       // Синий
                "технический" => Color.FromArgb(156, 39, 176),    // Фиолетовый
                "безопасность" => Color.FromArgb(244, 67, 54),    // Красный
                "интерфейс" => Color.FromArgb(255, 152, 0),       // Оранжевый
                "функциональный" => Color.FromArgb(0, 150, 136),  // Бирюзовый
                "документация" => Color.FromArgb(121, 85, 72),    // Коричневый
                "производительность" => Color.FromArgb(233, 30, 99), // Розовый
                "проект" => Color.FromArgb(63, 81, 181),          // Индиго
                "система" => Color.FromArgb(0, 188, 212),         // Голубой
                "требование" => Color.FromArgb(205, 220, 57),     // Лаймовый
                "тестирование" => Color.FromArgb(255, 87, 34),    // Глубокий оранжевый
                "обслуживание" => Color.FromArgb(96, 125, 139),   // Серо-голубой
                "цифровой" => Color.FromArgb(158, 158, 158),      // Серый
                "интеграция" => Color.FromArgb(255, 193, 7),      // Янтарный
                "меню" => Color.FromArgb(139, 195, 74),           // Светло-зеленый
                _ => CoffeeMedium                                 // По умолчанию
            };
        }
    }
}