using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Korzh.NLP
{
    /// <summary>
    /// Defines the interface for a language-detection service
    /// </summary>
    public interface ILanguageDetector
    {
        /// <summary>
        /// Returns an array of detected languages with score
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        Task<LangDetectionResult> DetectAsync(string text);
    }

    public class LangDetectionResult
    {
        public IDictionary<string, float> Scores { get; private set; } = new Dictionary<string, float>();

        public string BestGuessLang { get; set; }

        public bool Success {
            get {
                return Scores.Count > 0;
            }
        }
    }
}
