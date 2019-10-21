using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

using Korzh.NLP.TextRank.Graph;

namespace Korzh.NLP.TextRank
{
    public class TextRankSummarizer: SummarizerBase
    {

        private IStopWordFilter _stopWordFilter;

        public TextRankSummarizer(INlpServiceProvider nlpServiceProvider): base(nlpServiceProvider)
        {
        
        }

        public override string Process(string text, string lang = "en")
        {
            var sentances = SplitTextOnSentances(text);

            int summarySize = 5;
            if (sentances.Count <= summarySize) {
                return text;
            }

            _stopWordFilter = _nlpServiceProvider.GetStopWordFilter(lang);

            var stemmer = _nlpServiceProvider.GetStemmer(lang);

            var tokenizedSentances = new List<IList<string>>();

            foreach (var sentance in sentances) {
                var tokenizer = new TextTokenizer(sentance, filterMapper: new TextFilterMapper { Map = (t) => stemmer.Stem(t) });
                tokenizedSentances.Add(tokenizer.ToList());
            }

            var matrix = BuildSimilarityMatrix(tokenizedSentances);


            var graph = BuildDirectedGraph(matrix);

            var result = new PageRank<double>()
                            .Rank(graph)
                            .OrderBy(kv => kv.Value); //Less value, better result

            var summary = "";
            var topSentances = result.Take(summarySize).OrderBy(kv => kv.Key); //Sentances order in text
            foreach (var topSent in topSentances) {
                summary += sentances[topSent.Key] + ". ";
            }

            return summary;

        }


        private Matrix<double> BuildSimilarityMatrix(IList<IList<string>> sentances)
        {
            var matrix = CreateMatrix.Dense<double>(sentances.Count, sentances.Count);

            for (int idx1 = 0; idx1 < sentances.Count; idx1++) {
                for (int idx2 = 0; idx2 < sentances.Count; idx2++) {
                    if (idx1 != idx2)
                        matrix[idx1, idx2] = SentanceSimilarity(sentances[idx1], sentances[idx2]);
                }
            }

            return matrix;
        }

        private double SentanceSimilarity(IList<string> sentance1, IList<string> sentance2)
        {

            var allWords = sentance1.Concat(sentance2).Distinct().ToList();

            var v1 = new DenseVector(allWords.Count);
            var v2 = new DenseVector(allWords.Count);

            foreach (var word in sentance1) {
                if (_stopWordFilter.IsStopWord(word))
                    continue;

                var index = allWords.IndexOf(word);
                v1[index] += 1;
            }

            foreach (var word in sentance2) {
                if (_stopWordFilter.IsStopWord(word))
                    continue;

                var index = allWords.IndexOf(word);
                v2[index] += 1;
            }

            return 1 - Utils.CosineSimilarity(v1, v2);
        }

        private DirectedGraph<int> BuildDirectedGraph(Matrix<double> matrix)
        {
            if (matrix.RowCount != matrix.ColumnCount) {
                throw new ArgumentException("Matrix mudst be square.");
            }

            var dgraph = new DirectedGraph<int>();
            for (var i = 0; i < matrix.RowCount; i++) {
                for (var j = 0; j < matrix.ColumnCount; j++) {
                    dgraph.AddEdge(i, j, matrix[i, j]);
                }
            }

            return dgraph;

        }
    }
}
