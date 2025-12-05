using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeBotRAG
{
    public class TzParser
    {
        public List<TzBlock> ParseText(string[] lines)
        {
            var blocks = new List<TzBlock>();
            string currentSection = "Общее";
            string currentSubSection = "";
            StringBuilder currentContent = new StringBuilder();

            foreach (string line in lines)
            {
                string trimmed = line.Trim();

                if (string.IsNullOrEmpty(trimmed))
                {
                    SaveIfNotEmpty(blocks, currentSection, currentSubSection, currentContent);
                    continue;
                }

                if (trimmed.Contains("|"))
                {
                    SaveIfNotEmpty(blocks, currentSection, currentSubSection, currentContent);

                    string[] parts = trimmed.Split('|', 2);
                    if (parts.Length == 2)
                    {
                        blocks.Add(new TzBlock
                        {
                            Section = parts[0].Trim(),
                            Content = parts[1].Trim()
                        });
                    }
                    continue;
                }

                if (trimmed.StartsWith("# "))
                {
                    SaveIfNotEmpty(blocks, currentSection, currentSubSection, currentContent);
                    currentSection = trimmed.Substring(2).Trim();
                    currentSubSection = "";
                    continue;
                }

                if (trimmed.StartsWith("## "))
                {
                    SaveIfNotEmpty(blocks, currentSection, currentSubSection, currentContent);
                    currentSubSection = trimmed.Substring(3).Trim();
                    continue;
                }

                if (trimmed.StartsWith("### "))
                {
                    SaveIfNotEmpty(blocks, currentSection, currentSubSection, currentContent);
                    blocks.Add(new TzBlock
                    {
                        Section = currentSection,
                        Content = $"{currentSubSection}: {trimmed.Substring(4).Trim()}"
                    });
                    continue;
                }

                if (System.Text.RegularExpressions.Regex.IsMatch(trimmed, @"^\d+\.\d+\.") ||
                    System.Text.RegularExpressions.Regex.IsMatch(trimmed, @"^\d+\.\d+\.\d+\."))
                {
                    SaveIfNotEmpty(blocks, currentSection, currentSubSection, currentContent);

                    string content = System.Text.RegularExpressions.Regex.Replace(trimmed, @"^\d+(\.\d+)*\.?\s*", "");
                    blocks.Add(new TzBlock
                    {
                        Section = currentSubSection.Length > 0 ? currentSubSection : currentSection,
                        Content = content
                    });
                    continue;
                }

                if (trimmed.StartsWith("- ") || trimmed.StartsWith("• ") || trimmed.StartsWith("* "))
                {
                    SaveIfNotEmpty(blocks, currentSection, currentSubSection, currentContent);

                    string listItem = trimmed.Substring(2).Trim();
                    blocks.Add(new TzBlock
                    {
                        Section = currentSubSection.Length > 0 ? currentSubSection : currentSection,
                        Content = listItem
                    });
                    continue;
                }

                if (trimmed.Length > 3 && !IsNumberingOnly(trimmed))
                {
                    if (currentContent.Length > 0)
                        currentContent.Append(" ");
                    currentContent.Append(trimmed);
                }
            }

            SaveIfNotEmpty(blocks, currentSection, currentSubSection, currentContent);

            return blocks;
        }

        private void SaveIfNotEmpty(List<TzBlock> blocks, string section, string subsection, StringBuilder content)
        {
            if (content.Length > 0)
            {
                string fullSection = section;
                if (!string.IsNullOrEmpty(subsection))
                    fullSection = $"{section} - {subsection}";

                blocks.Add(new TzBlock
                {
                    Section = fullSection,
                    Content = content.ToString()
                });
                content.Clear();
            }
        }

        private bool IsNumberingOnly(string text)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(text, @"^\d+(\.\d+)*$");
        }
    }

    public class TzBlock
    {
        public string Section { get; set; } = "";
        public string Content { get; set; } = "";
        public float[]? Vector { get; set; }

        // Вычисляемые свойства
        public string DisplayText => Content;
        public string FullText => $"{Section}: {Content}";
    }
}