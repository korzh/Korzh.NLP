using System;
using System.Collections.Generic;
using System.Text;

namespace Korzh.NLP
{
    public interface IStopWordFilter 
    {
        bool IsStopWord(string word);
    }

    public class StopWordFilter : IStopWordFilter
    {
        private HashSet<string> _stopWordSet;

        public StopWordFilter(string stopWordDict) {
            _stopWordSet = new HashSet<string>(stopWordDict.Split(','));
        }

        public bool IsStopWord(string word) {
            return (word.Length < 3) || _stopWordSet.Contains(word.ToLowerInvariant());
        }
    }
}
