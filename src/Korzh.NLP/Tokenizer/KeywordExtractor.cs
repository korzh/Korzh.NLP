using System;

namespace Korzh.NLP
{
    public class KeywordExtractor : ITextFilterMapper
    {
        private readonly IStopWordFilter _stopWordFilter;

        private readonly IStemmer _wordStemmer;

        private readonly Func<string, bool> _filter;
        private readonly Func<string, string> _mapper;


        public KeywordExtractor(INlpServiceProvider nlpServices, string lang) 
        {
            _stopWordFilter = nlpServices.GetStopWordFilter(lang);

            _wordStemmer = nlpServices.GetStemmer(lang);

            _filter = (word) => {
                return !_stopWordFilter.IsStopWord(word);
            };

            _mapper = (word) => {
                return _wordStemmer.Stem(word);
            };
        }

        public Func<string, bool> Filter => _filter;

        public Func<string, string> Map => _mapper;
    }
}
