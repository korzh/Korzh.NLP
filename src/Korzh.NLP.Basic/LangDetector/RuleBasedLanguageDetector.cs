using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Korzh.NLP
{
    /// <summary>
    /// Allows to detect the language by text. Currently detects only ENG, RUS or UKR 
    /// Implements the <see cref="Aistant.Services.ILanguageDetectionService" />
    /// </summary>
    /// <seealso cref="Aistant.Services.ILanguageDetectionService" />
    public class RuleBasedLanguageDetector : ILanguageDetector
    {
        public Task<LangDetectionResult> DetectAsync(string text)
        {
            var result = new LangDetectionResult();

            string lang = null;
            var lowerText = text.ToLowerInvariant();

            int latinScore = 0;
            int сyrillicScore = 0;
            int rusScore = 0;
            int uaScore = 0;

            foreach (var ch in lowerText) {
                if (ch >= 'a' && ch <= 'z') {
                    latinScore++;
                }
                else {

                    if (ch == 'ё' || ch == 'ы' || ch == 'ъ'
                        || ch == 'э') {
                        rusScore++;
                    }
                    else {
                        if (ch == 'ї' || ch == 'і' || ch == 'ґ'
                           || ch == 'є' || ch == 'щ') {
                            uaScore++;
                        }
                    }

                    if (ch >= 'а' && ch <= 'я') {
                        сyrillicScore++;
                    }
                }
            }

            if (latinScore >= сyrillicScore) {
                lang = "en";
            }
            else {
                lang = (rusScore >= uaScore) ? "ru" : "ua";
            }
            result.Scores[lang] = 1f;
            result.BestGuessLang = lang;

            return Task.FromResult(result);
        }
    }
}
