using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korzh.NLP
{

    public class TfIdfSummarizer: SummarizerBase
    {
        public TfIdfSummarizer(INlpServiceProvider nlpServiceProvider) : base(nlpServiceProvider) { 
        
        }

        public override string Process(string text, string lang = "en")
        {
            var sentances = SplitTextOnSentances(text);

            int summarySize = 5;
            if (sentances.Count <= summarySize) {
                return text;
            }

            var tfidf = new TfIdfDepot();

            var filterMapper = new KeywordExtractor(_nlpServiceProvider, lang);

            for (var i = 0; i < sentances.Count; i++) {

                var docId = i.ToString();
                tfidf.AddOrFindDocument(docId);

                var tokenizer = new TextTokenizer(sentances[i], filterMapper: filterMapper);
                var token = tokenizer.FirstToken();
                while (token != null) {
                    tfidf.AddWordToDoc(docId, token);
                    token = tokenizer.NextToken();
                }

            }

            tfidf.Recalculate();     

            var docIds = tfidf
                .DocDict
                .OrderByDescending(kv => kv.Value.TotalTFIFD)
                .Take(summarySize)
                .Select(kb => int.Parse(kb.Value.DocId))
                .OrderBy(id => id)
                .ToList();

            var result = "";
            foreach (var docId in docIds) {
                result += sentances[docId] + ". ";
            }
            
            return result;

        }
  

    }


}
