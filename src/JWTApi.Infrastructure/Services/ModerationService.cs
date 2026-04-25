using JWTApi.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JWTApi.Infrastructure.Services;


    public class ModerationService
    {
        private readonly Dictionary<string, List<string>> _sensitiveData;
        private readonly Dictionary<string, int> _wordWeights = new()
    {
        { "political", 40 },
        { "abusive", 25 },
        { "unlawful", 35 }
    };

        public ModerationService()
        {
            var json = File.ReadAllText("Data/sensitive_words.json");
            _sensitiveData = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json)
                             ?? new Dictionary<string, List<string>>();
        }

        public ModerationResult Analyze(string text)
        {
            var normalizedText = text.ToLower();
            var flags = new List<string>();
            double totalScore = 0;
            double maxPossibleScore = 0;

            foreach (var category in _sensitiveData)
            {
                var weight = _wordWeights.GetValueOrDefault(category.Key, 20);
                maxPossibleScore += weight;

                int matchCount = category.Value.Count(word =>
                    normalizedText.Contains(word.ToLower()));

                if (matchCount > 0)
                {
                    flags.Add($"{category.Key}: {matchCount} مورد");
                    double categoryScore = weight * Math.Min(1, matchCount / 3.0);
                    totalScore += categoryScore;
                }
            }

            // اضافه کردن فاکتورهای پیشرفته‌تر
            totalScore += DetectRepetition(normalizedText);     // تکرار عجیب
            totalScore += DetectAngerWords(normalizedText);     // کلمات خشم

            // نرمالایز کردن به 0-100
            double rawScore = maxPossibleScore > 0
                ? (totalScore / maxPossibleScore) * 100
                : 0;

            var finalScore = Math.Min(100, Math.Max(0, rawScore));

            return new ModerationResult
            {
                RiskScore = Math.Round(finalScore, 2),
                IsSafe = finalScore < 60,
                Flags = flags
            };
        }

        private double DetectRepetition(string text)
        {
            // تشخیص حروف تکراری مثل "ععععع" یا "!!!!"
            var repeatCount = System.Text.RegularExpressions.Regex.Matches(text, @"(.)\1{4,}").Count;
            return Math.Min(15, repeatCount * 5);
        }

        private double DetectAngerWords(string text)
        {
            string[] angerWords = { "خفه شو", "بمیر", "لعنت", "بی‌غیرت", "کاش بمیری" };
            int count = angerWords.Count(w => text.Contains(w));
            return Math.Min(20, count * 10);
        }
    }
