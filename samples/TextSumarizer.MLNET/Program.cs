using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Text;

namespace Korzh.NLP.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1) {
                Console.WriteLine("Using: tsmln <path to text file>");
                return;
            }

            var filePath = args[0];
            if (!File.Exists(filePath)) {
                Console.WriteLine("Can't find file: " + filePath);
            }

            var inputText = File.ReadAllText(filePath);

            var summary = Summarize(inputText, 3);

			Console.WriteLine("SUMMARY:");
            Console.WriteLine(summary);
        }


        private static string Summarize(string text, int summarySentenceNum)
        {
            var mlContext = new MLContext(seed: 1);

            var textDataView = mlContext.Data.LoadFromEnumerable(GetSentences(text));

            var pipeline = mlContext.Transforms.Text.NormalizeText("NormText", "Text")
                    .Append(mlContext.Transforms.Text.TokenizeIntoWords("Tokens", "NormText"))
                    .Append(mlContext.Transforms.Text.ApplyWordEmbedding("Features", "Tokens",
                                    WordEmbeddingEstimator.PretrainedModelKind.SentimentSpecificWordEmbedding))
                    .Append(mlContext.Clustering.Trainers.KMeans(featureColumnName: "Features", 
                                                                numberOfClusters: summarySentenceNum));

            var model = pipeline.Fit(textDataView);

            var clusteredDataView = model.Transform(textDataView);

            //get all sentences ordered by their index in original text and then by the cluster number
            var clusteredSentences = mlContext.Data.CreateEnumerable<ClusteringPrediction>(clusteredDataView, false)
                .OrderBy(cs => cs.Index * 100 + cs.ClusterId)
                .ToArray();

            //get one sentence from each cluster for summary
            var summarySentences = new string[summarySentenceNum];
            var usedClusters = new List<uint>();
            var index = 0;
            foreach (var csentance in clusteredSentences) {
                if (!usedClusters.Contains(csentance.ClusterId)) {
                    summarySentences[index] = csentance.Text;
                    usedClusters.Add(csentance.ClusterId);
                    index++;
                }
            }
            return string.Join(" ", summarySentences);
        }

		
        private static IList<Sentence> GetSentences(string text)
        {
            int pos = 0;
            int index = 1;
            bool nextParagraphStart = true;

            var result = new List<Sentence>();

            while (pos < text.Length) {
                //next paragraph end
                var npe1 = text.IndexOf(".\r", pos);
                var npe2 = text.IndexOf(".\n", pos);
                var npe = Math.Max(npe1, npe2);

                var nse = text.IndexOf(". ", pos);

                var npos = Math.Min(npe, nse);

                if (npos == -1) {
                    npos = text.Length - 1;
                }

                var stext = text.Substring(pos, npos - pos+1);

                result.Add(new Sentence {
                    Index = index,
                    Text = stext,
                    IsParagraphStart = nextParagraphStart
                });

                nextParagraphStart = npos == npe;

                pos = npos + 1;
                while (pos < text.Length && char.IsWhiteSpace(text[pos])) pos++;
                index++;
            }

            return result;
        }

        public class Sentence
        {
            public int Index { get; set; }
            public string Text { get; set; }
            public bool IsParagraphStart { get; set; } = false;
        }

        public class ClusteringPrediction
        {
            [ColumnName("Text")]
            public string Text;

            [ColumnName("Index")]
            public int Index;

            [ColumnName("IsParagraphStart")]
            public bool IsParagraphStart;

            [ColumnName("PredictedLabel")]
            public uint ClusterId;

            [ColumnName("Score")]
            public float[] Distance;
        }
    }
}
