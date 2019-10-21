using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Korzh.NLP
{
    public abstract class SummarizerBase : ITextSummarizer 
    {
        private static Regex _trimRegex = new Regex(@"\s+");

        protected readonly INlpServiceProvider _nlpServiceProvider;

        public SummarizerBase(INlpServiceProvider nlpServiceProvider)
        {
            _nlpServiceProvider = nlpServiceProvider;
        }

        protected List<string> SplitTextOnSentances(string text) 
        {

            if (string.IsNullOrEmpty(text))
                return new List<string>();

            var sentances =  _trimRegex.Replace(text, " ")
                                       .Split(new string[] { ". " }, StringSplitOptions.None)
                                       .ToList();

            sentances.RemoveAll(s => string.IsNullOrEmpty(s));

            return sentances;
        }

        public abstract string Summarize(string text, string lang = "en");
    }


}
