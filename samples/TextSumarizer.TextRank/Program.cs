using System;
using System.IO;

using Korzh.NLP;
using Korzh.NLP.TextRank;

namespace Korzh.NLP.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            string text = File.ReadAllText(args[0]);

            var nlp = new BasicNlpServiceProvider();
            var ts = new TextRankSummarizer(nlp);

            var summary = ts.Summarize(text);
            Console.WriteLine("Summary:\n" + summary);

            Console.ReadKey();
        }
    }
}
