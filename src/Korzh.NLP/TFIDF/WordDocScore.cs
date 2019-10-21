namespace Korzh.NLP
{
    public class WordDocScore
    {

        public string DocId { get; private set; }

        //The value (score) of this word within this class
        //In the basic case it's just the number of word occurencies
        //However, it's not true in a general case since 
        private float _score;

        //term frequency 
        //Number of times this word occurs in this class divide on total number of words in this class
        //Isn't it the same as likehood in Naive Bayes???
        public float TF { get; private set; }

        //TD-IDF
        public float TFIDF { get; private set; }

        public WordDocScore(string docId)
        {
            DocId = docId;
            _score = 0;
            TF = 0f;
            TFIDF = 0f;
        }

        public float Score => _score;


        public void IncScore(float scoreShift) =>
            _score += scoreShift;

        public void Recalculate(float totalWordScoreInClass, float idf)
        {
            if (totalWordScoreInClass > 0) {
                TF = Score / totalWordScoreInClass;
            }

            TFIDF = TF * idf;
        }
    }


}
