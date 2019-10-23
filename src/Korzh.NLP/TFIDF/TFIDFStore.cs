using System;
using System.Collections.Generic;

namespace Korzh.NLP
{
    public class TFIDFStore 
    {
        public IDictionary<string, WordData> Words { get; private set; }

        public IDictionary<string, DocData> DocDict { get; private set; }


        public TFIDFStore() 
        {
            Words = new Dictionary<string, WordData>();
            DocDict = new Dictionary<string, DocData>();
        }

        public DocData AddOrFindDocument(string docId, float initialImportance = 1f)
        {

            if (!DocDict.TryGetValue(docId, out DocData docData)) {
                docData = new DocData(docId);
                AddDocData(docData);
            }

            docData.IncImportance(initialImportance);
            return docData;
        }

        internal void AddDocData(DocData docData)
        {
            DocDict[docData.DocId] = docData;
        }

        public void RemoveDocument(string docId)
        {
            if (DocDict.TryGetValue(docId, out var docData)) {
                foreach (var we in Words) {
                    we.Value.RemoveDoc(docId);
                }

                DocDict.Remove(docId);
            }
        }

        internal void AddWordWithData(string word, WordData wordData)
        {
            Words[word] = wordData;
        }

        internal WordData GetOrCreateWord(string word)
        {
            if (!Words.TryGetValue(word, out var wordData)) {
                wordData = new WordData();
                Words[word] = wordData;
            }
            return wordData;
        }

        public void AddWordToDoc(string docId, string word, float wordValue = 1f)
        {
            var wordData = GetOrCreateWord(word);
            AddWordDataToDoc(docId, wordData, wordValue);
        }

        public void AddWordDataToDoc(string docId, WordData wordData, float wordValue = 1f)
        {
            var docData = AddOrFindDocument(docId);
            docData.IncTotalWordScore(wordValue);

            var wcs = wordData.GetOrAddValue(docData.DocId);

            wcs.IncScore(wordValue);
        }


        public void AddWordToDocs(string[] docs, string word, float wordValue = 1f)
        {
            var wordData = GetOrCreateWord(word);

            foreach (string docId in docs) {
                AddWordDataToDoc(docId, wordData, wordValue);
            }
        }

        public void Recalculate()
        {
            foreach (var wd in Words.Values) {
                wd.Recalculate(DocDict);
            }

            foreach (var doc in DocDict.Values) {
                doc.Recalculate(Words);
            }
        }

        public IDictionary<string, IList<ValueTuple<string, float>>> GetDocs() 
        {
            var result = new Dictionary<string, IList<ValueTuple<string, float>>>();

            foreach (var we in Words) {
                foreach (var wdse in we.Value.ScoreDict) {
                    if (!result.TryGetValue(wdse.Key, out var words)) {
                        words = new List<ValueTuple<string, float>>();
                        result[wdse.Key] = words;
                    }
                    words.Add((we.Key, wdse.Value.TFIDF));
                }
            }

            return result;
        }
    }

    public class DocInfo 
    { 
        public string DocId { get; set; }

        public IList<ValueTuple<string, float>> Words { get; set; }
    }
}
