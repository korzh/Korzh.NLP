using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using System.Threading.Tasks;

using Mosaik.Core;
using Catalyst;
using Catalyst.Models;

namespace Korzh.NLP.Samples
{
    class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>System.Threading.Tasks.Task.</returns>
        static async Task Main(string[] args)
        {
            if (args.Length < 1) {
                Console.WriteLine("Using: ttgcat <path to documents folder>");
                return;
            }

            var docsPath = args[0];
            if (!Directory.Exists(docsPath)) {
                Console.WriteLine("Can't find file: " + docsPath);
            }

            //Catalyst NLP pipeline initialization
            Storage.Current = new OnlineRepositoryStorage(new DiskStorage("catalyst-models"));
            var nlp = await Pipeline.ForAsync(Language.English);

            var tfidfStore = new TFIDFStore();

            //getting all text files
            var files = Directory.GetFiles(docsPath, "*.txt");
            foreach (var filePath in files) {
                var fileName = Path.GetFileName(filePath);
                var inputText = File.ReadAllText(filePath);
                UpdateTFIDF(nlp, tfidfStore, fileName, inputText);
            }

            tfidfStore.Recalculate();

            var docsAndTags = ExtractTags(tfidfStore, 5);

            foreach (var doc in docsAndTags) {
                Console.WriteLine(doc.DocId);
                Console.WriteLine("TAGS: " + string.Join(", ", doc.Tags));
                Console.WriteLine();
            }
        }


        private static void UpdateTFIDF(Pipeline nlp, TFIDFStore tfIdfStore, string docId, string text)
        {
            var doc = new Document(text, Language.English);
            nlp.ProcessSingle(doc);

            //Updating our documents base to calculate TF-IDF lately
            foreach (var sentence in doc.TokensData) {
                foreach (var token in sentence) {
                    var word = text.Substring(token.LowerBound, token.UpperBound - token.LowerBound + 1);
                    if (token.Tag == PartOfSpeech.NOUN || token.Tag == PartOfSpeech.PROPN) {
                        var wordValue = token.Tag == PartOfSpeech.PROPN ? 2 : 1;
                        tfIdfStore.AddWordToDoc(docId, word, wordValue);
                   }
                }
            }
        }

        private static IEnumerable<TaggedDoc> ExtractTags(TFIDFStore tfidfStore, int count)
        {
            foreach (var de in tfidfStore.GetDocs()) {
                yield return new TaggedDoc {
                    DocId = de.Key,
                    Tags = de.Value.OrderByDescending(ws => ws.Item2).Take(count).Select(ws => ws.Item1)
                };           
            }
        }
    }

    public class TaggedDoc {
        public string DocId;

        public IEnumerable<string> Tags;
    }
}
