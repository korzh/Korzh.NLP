using System.Collections.Generic;

namespace Korzh.NLP
{
    /// <summary>
    /// Contains necessary data for one document
    /// </summary>
    public class DocData
    {
        public string DocId { get; internal set; }

        private float _totalWordScore;
        //Total score of all words included in this document
        //In the simplest case - it just the word count
        public float TotalWordScore => _totalWordScore;

        private float _importance = 0;

        private float _totalTFIDF = 0;

        public float TotalTFIFD => _totalTFIDF;

        public float Importance => _importance;


        public DocData(string docId)
        {
            DocId = docId;
            _totalWordScore = 0f;
        }

        public void IncTotalWordScore(float score)
        {
            _totalWordScore += score;
        }

        public void IncImportance(float value)
        {
            _importance += value;
        }

        public void Recalculate(IDictionary<string, WordData> words) 
        {
            _totalTFIDF = 0; 

            foreach (var word in words) { 
                if (word.Value.ScoreDict.TryGetValue(DocId, out var score)) {
                    _totalTFIDF += score.TFIDF;
                }
            }

            _totalTFIDF /= TotalWordScore;
        
        }

    }


}
