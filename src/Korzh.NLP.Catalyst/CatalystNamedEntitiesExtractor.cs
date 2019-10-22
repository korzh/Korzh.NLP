using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Catalyst;
using Catalyst.Models;
using Mosaik.Core;

namespace Korzh.NLP.Catalyst
{
    public class CatalystEntitiesExtractor : INamedEntitiesExtractor
    {
        private readonly Pipeline _nlpPipeline;

        static CatalystEntitiesExtractor() {
        }

        public CatalystEntitiesExtractor(Pipeline nlpPipeline)
        {
            _nlpPipeline = nlpPipeline;
        }

        public Task<int> CollectNamedEntitiesAsync(string text, IList<ValueTuple<string, short>> nentities)
        {
            //var ps = new PatternSpotter(Language.English, 0, "PatternSpotterTest", "PatternSpotterCaptureTag");
            //ps.NewPattern("AllCaps", mp => mp.Add(PatternUnitPrototype.))
            //await AveragePerceptronEntityRecognizer.FromStoreAsync(Language.English, -1, "WikiNER");
            //nlp.Add(new AveragePerceptronEntityRecognizer(Language.English, 0, "WikiNER", new string[] { "Person", "Organization", "Location" }));
            //var models = nlp.GetModelsList();
            var doc = new Document(text, Language.English);

            _nlpPipeline.ProcessSingle(doc);
            int N = 0;
            foreach (var sentence in doc.TokensData) {
                //Console.WriteLine($"{N}: " + string.Join(" ", sentence.Select(td => td.Tag.ToString())));
                foreach (var token in sentence) {
                    if (token.Tag == PartOfSpeech.PROPN) {
                        var tokenText = doc.Value.Substring(token.LowerBound, token.UpperBound - token.LowerBound + 1);
                        var score = CalcScore(tokenText);
                        if (score > 0) {
                            AddNamedEntity(nentities, tokenText, score);
                        }
                        N++;
                    }
                }
            }

            return Task.FromResult(N);
        }

        private short CalcScore(string text) 
        {
            if (string.IsNullOrEmpty(text)) {
                return 0;
            }

            //is not capitalized
            if (!char.IsUpper(text[0])) {
                return 0;
            }

            var textChars = text.ToCharArray();

            //contains some special symbols
            if (text.ToCharArray().Any(ch => "~@/\\+()[]{}*^<>?:;%=".Contains(ch))) {
                return 0;
            }

            short result = 1;

            //all caps
            if (text.ToUpperInvariant() == text) {
                result += 2;
            }
            else {
                //Not only the first letter is capitalized
                var upperCount = textChars.Count(ch => char.IsUpper(ch));
                if (upperCount > 1) {
                    result += 3;
                }
            }

            //lower score if there are more than one dot in the name
            var dotCount = textChars.Count(ch => ch == '.');
            if (dotCount > 1) {
                result -= 1;
            }

            return result;
        }

        private void AddNamedEntity(IList<ValueTuple<string, short>> nentities, string newEntity, short score = 1)
        {
            if (!nentities.Any(ne => ne.Item1 == newEntity)) {
                nentities.Add((newEntity, score));
            }
        }
    }
}
