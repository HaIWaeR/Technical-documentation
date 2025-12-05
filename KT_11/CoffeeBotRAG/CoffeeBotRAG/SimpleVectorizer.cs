using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoffeeBotRAG
{
    public class SimpleVectorizer
    {
        private Dictionary<string, float[]> wordVectors = new Dictionary<string, float[]>();
        private Random random = new Random();
        private int vectorSize = 10;

        public SimpleVectorizer()
        {
            InitializeWordVectors();
        }

        private void InitializeWordVectors()
        {
            var topicWords = new Dictionary<string, string[]>
            {
                { "технический", new[] { "техническ", "техник", "инженерн", "конструкц", "схем", "чертеж" } },
                { "функциональный", new[] { "функц", "возможност", "операц", "действ", "работ" } },
                { "оплата", new[] { "оплат", "плат", "qr", "код", "деньг", "стоимост", "чек", "расчет", "перевод" } },
                { "доставка", new[] { "достав", "курьер", "адрес", "получен", "самовывоз", "забрат", "привез", "локац" } },
                { "меню", new[] { "меню", "список", "перечень", "каталог", "ассортимент", "выбор" } },
                { "безопасность", new[] { "безопас", "защит", "данн", "конфиденци", "шифрован", "информац", "приватн" } },
                { "цифровой", new[] { "цифров", "электрон", "онлайн", "интернет", "веб", "сетев" } },
                { "интеграция", new[] { "интеграц", "api", "подключен", "соединен", "взаимодейств", "интерфейс" } },
                { "интерфейс", new[] { "интерфейс", "пользователь", "удобств", "навигац", "уведомлен", "кнопк", "элемент" } },
                { "обслуживание", new[] { "обслуживан", "поддерж", "техническ", "ремонт", "обновлен", "сопровожден", "обслуж" } },
                { "производительность", new[] { "производительн", "скорост", "быстродейств", "отклик", "время", "эффектив" } },
                { "документация", new[] { "документац", "документ", "инструкц", "руководств", "описан", "спецификац" } },
                { "проект", new[] { "проект", "разработк", "создан", "реализац", "внедрен", "запуск" } },
                { "система", new[] { "систем", "подсистем", "компонент", "модуль", "архитектур", "структур" } },
                { "требование", new[] { "требован", "услов", "критери", "стандарт", "норма", "правил" } }
            };

            foreach (var topic in topicWords)
            {
                float[] topicVector = GenerateTopicVector(topic.Key);
                wordVectors[topic.Key] = topicVector;

                foreach (string word in topic.Value)
                {
                    if (!wordVectors.ContainsKey(word))
                    {
                        float[] wordVector = new float[vectorSize];
                        for (int i = 0; i < vectorSize; i++)
                        {
                            wordVector[i] = topicVector[i] + ((float)random.NextDouble() * 0.2f - 0.1f);
                        }
                        wordVectors[word] = wordVector;
                    }
                }
            }

            string[] commonWords = new string[]
            {
                "задание", "задач", "цель", "миссия", "назначен",
                "введение", "описан", "обзор", "резюме",
                "реализац", "внедрен", "создан", "построен",
                "проверк", "тестирован", "валидац", "верификац",
                "ограничен", "лимит", "предел", "рамк",
                "допущен", "предположен", "гипотез",
                "критери", "приемк", "тестирован", "проверк",
                "клиент", "пользователь", "заказчик", "потребитель",
                "увелич", "рост", "развит", "улучшен", "оптимизац",
                "заказ", "транзакц", "операц", "процесс"
            };

            foreach (string word in commonWords)
            {
                if (!wordVectors.ContainsKey(word))
                {
                    wordVectors[word] = GenerateRandomVector();
                }
            }
        }

        private float[] GenerateTopicVector(string topic)
        {
            float[] vector = new float[vectorSize];
            int seed = topic.GetHashCode();
            Random topicRandom = new Random(seed);

            for (int i = 0; i < vectorSize; i++)
            {
                vector[i] = (float)topicRandom.NextDouble() * 2 - 1;
            }

            float magnitude = 0;
            for (int i = 0; i < vectorSize; i++)
            {
                magnitude += vector[i] * vector[i];
            }
            magnitude = (float)Math.Sqrt(magnitude);

            if (magnitude > 0)
            {
                for (int i = 0; i < vectorSize; i++)
                {
                    vector[i] /= magnitude;
                }
            }

            return vector;
        }

        private float[] GenerateRandomVector()
        {
            float[] vector = new float[vectorSize];
            for (int i = 0; i < vectorSize; i++)
            {
                vector[i] = (float)random.NextDouble() * 2 - 1;
            }

            float magnitude = 0;
            for (int i = 0; i < vectorSize; i++)
            {
                magnitude += vector[i] * vector[i];
            }
            magnitude = (float)Math.Sqrt(magnitude);

            if (magnitude > 0)
            {
                for (int i = 0; i < vectorSize; i++)
                {
                    vector[i] /= magnitude;
                }
            }

            return vector;
        }

        public float[] VectorizeText(string text)
        {
            text = text.ToLower();
            string[] words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            float[] result = new float[vectorSize];
            int matchedWords = 0;

            foreach (string word in words)
            {
                string cleanWord = CleanWord(word);
                if (cleanWord.Length > 2)
                {
                    if (wordVectors.ContainsKey(cleanWord))
                    {
                        AddVector(result, wordVectors[cleanWord]);
                        matchedWords++;
                    }
                    else
                    {
                        bool found = false;
                        foreach (var kvp in wordVectors)
                        {
                            if (kvp.Key.Contains(cleanWord) || cleanWord.Contains(kvp.Key))
                            {
                                AddVector(result, kvp.Value, 0.3f);
                                matchedWords++;
                                found = true;
                                break;
                            }
                        }

                        if (!found && cleanWord.Length > 3)
                        {
                            string theme = "технический";
                            if (cleanWord.EndsWith("ация") || cleanWord.EndsWith("ение"))
                                theme = "обслуживание";
                            else if (cleanWord.EndsWith("ость"))
                                theme = "безопасность";
                            else if (cleanWord.EndsWith("ия") || cleanWord.EndsWith("ка"))
                                theme = "функциональный";

                            if (wordVectors.ContainsKey(theme))
                            {
                                AddVector(result, wordVectors[theme], 0.1f);
                                matchedWords++;
                            }
                        }
                    }
                }
            }

            if (matchedWords > 0)
            {
                for (int i = 0; i < vectorSize; i++)
                {
                    result[i] /= matchedWords;
                }

                float magnitude = 0;
                for (int i = 0; i < vectorSize; i++)
                {
                    magnitude += result[i] * result[i];
                }
                magnitude = (float)Math.Sqrt(magnitude);

                if (magnitude > 0)
                {
                    for (int i = 0; i < vectorSize; i++)
                    {
                        result[i] /= magnitude;
                    }
                }
            }
            else
            {
                result = GenerateRandomVector();
            }

            return result;
        }

        private void AddVector(float[] target, float[] source, float weight = 1.0f)
        {
            for (int i = 0; i < vectorSize; i++)
            {
                target[i] += source[i] * weight;
            }
        }

        private string CleanWord(string word)
        {
            var sb = new StringBuilder();
            foreach (char c in word)
            {
                if (char.IsLetter(c))
                    sb.Append(c);
            }
            return sb.ToString().ToLower();
        }

        public float CosineSimilarity(float[] vector1, float[] vector2)
        {
            if (vector1 == null || vector2 == null)
                return 0;

            if (vector1.Length != vectorSize || vector2.Length != vectorSize)
                return 0;

            float dotProduct = 0;
            float magnitude1 = 0;
            float magnitude2 = 0;

            for (int i = 0; i < vectorSize; i++)
            {
                dotProduct += vector1[i] * vector2[i];
                magnitude1 += vector1[i] * vector1[i];
                magnitude2 += vector2[i] * vector2[i];
            }

            magnitude1 = (float)Math.Sqrt(magnitude1);
            magnitude2 = (float)Math.Sqrt(magnitude2);

            if (magnitude1 == 0 || magnitude2 == 0)
                return 0;

            float similarity = dotProduct / (magnitude1 * magnitude2);

            return Math.Max(-1, Math.Min(1, similarity));
        }
    }
}