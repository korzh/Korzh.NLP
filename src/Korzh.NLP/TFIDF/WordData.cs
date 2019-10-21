using System;
using System.Linq;
using System.Collections.Generic;

namespace Korzh.NLP
{
    public class WordData
    {
        public IDictionary<string, WordDocScore> ScoreDict { get; private set; }

        public WordData()
        {
            ScoreDict = new Dictionary<string, WordDocScore>();
        }

        public void RemoveDoc(string docId)
        {
            ScoreDict.Remove(docId);
        }

        public void Recalculate(IDictionary<string, DocData> allDocs)
        {
            float totalDocsWithWord = ScoreDict.Count(wde => wde.Value.Score > 0);

            float docsIDF = (float)Math.Log((float)(allDocs.Count + 2) / totalDocsWithWord + 1);

            int i = 0;
            foreach (var wde in ScoreDict) {
                var dd = allDocs[wde.Value.DocId];
                wde.Value.Recalculate(dd.TotalWordScore, docsIDF);
                i++;
            }
        }

        internal WordDocScore GetOrAddValue(string docId)
        {
            if (!ScoreDict.TryGetValue(docId, out var wcs)) {
                wcs = new WordDocScore(docId);
                ScoreDict[wcs.DocId] = wcs;
            }
            return wcs;
        }
    }
}
