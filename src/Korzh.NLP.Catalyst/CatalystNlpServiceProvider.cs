using System;
using System.Collections.Generic;
using System.Text;

using Catalyst;
using Mosaik.Core;

namespace Korzh.NLP.Catalyst
{
    public class CatalystNlpServiceProvider : INlpServiceProvider
    {
        private ILanguageDetector _langDetector = null;

        private IDictionary<string, LangServices> _pipelinePool = new Dictionary<string, LangServices>();
        private readonly BasicNlpServiceProvider _basicNlpServiceProvider;

        public CatalystNlpServiceProvider()
        {
            _basicNlpServiceProvider = new BasicNlpServiceProvider();
        }

        protected LangServices GetLangServices(string lang)
        {
            if (!_pipelinePool.TryGetValue(lang, out var langServices)) {
                Storage.Current = new OnlineRepositoryStorage(new DiskStorage("catalyst-models"));
                var langValue = GetLanguageByCode(lang);
                langServices = new LangServices {
                    Pipeline = Pipeline.For(langValue)
                };
            }

            return langServices;
        }

        private Language GetLanguageByCode(string lang)
        {
            switch (lang) {
                case "en":
                    return Language.English;
                case "ru":
                    return Language.Russian;
                case "uk":
                    return Language.Ukrainian;
                default:
                    throw new NotSupportedException(lang + " language is not supported yet");

            }
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
            var langServices = GetLangServices(lang);
            if (langServices.NamedEntitiesExtractor == null) {
                langServices.NamedEntitiesExtractor = new CatalystEntitiesExtractor(langServices.Pipeline);
            }

            return langServices.NamedEntitiesExtractor;
        }

        public IStemmer GetStemmer(string lang)
        {
            return _basicNlpServiceProvider.GetStemmer(lang);
        }

        public IStopWordFilter GetStopWordFilter(string lang)
        {
            return _basicNlpServiceProvider.GetStopWordFilter(lang);
        }

        public IQuestionGenerator GetQuestionGenerator(string lang)
        {
            return _basicNlpServiceProvider.GetQuestionGenerator(lang);
        }
    }

    public class LangServices 
    {
        public Pipeline Pipeline = null;
        public INamedEntitiesExtractor NamedEntitiesExtractor;
    }
}
