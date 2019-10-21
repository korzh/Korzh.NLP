using System;
using System.Collections.Generic;
using System.Text;

using Iveonik.Stemmers;

namespace Korzh.NLP
{

    public class BasicNlpServiceProvider : INlpServiceProvider
    {
        private ILanguageDetector _langDetector = null;

        public BasicNlpServiceProvider()
        {
        }

        public ILanguageDetector GetLanguageDetector()
        {
            if (_langDetector == null) {
                _langDetector = new RuleBasedLanguageDetector();
            }
            return _langDetector;
        }

        public INamedEntitiesExtractor GetNamedEntitiesExtractor(string lang)
        {
            switch (lang) {
                case "en":
                    return new EnglishNamedEntitiesExtractor();
                default:
                    throw new NotImplementedException();
            }
        }

        public IStemmer GetStemmer(string lang)
        {
            switch (lang) {
                case "en":
                    return new EnglishStemmer();
                case "ru":
                    return new RussianStemmer();
                default:
                    throw new NotImplementedException();

            }           
        }

        public IStopWordFilter GetStopWordFilter(string lang)
        {
            switch (lang) {
                case "en":
                    return new StopWordFilter(Dictionaries.EnglishStopWords);
                case "ru":
                    return new StopWordFilter(Dictionaries.RussianStopWords);
                default:
                    throw new NotImplementedException();
            }
        }

        public IQuestionGenerator GetQuestionGenerator(string lang)
        {
            switch (lang) {
                case "en":
                    return new BasicEnglishQuestionGenerator();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
