using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Korzh.NLP
{
    public class EnglishNamedEntitiesExtractor : INamedEntitiesExtractor
    {
        public Task<int> CollectNamedEntitiesAsync(string text, IList<ValueTuple<string, short>> nentities)
        {
            AddNamedEntity(nentities, "NamedEntity1", 1);
            AddNamedEntity(nentities, "NamedEntity2", 1);
            AddNamedEntity(nentities, "NamedEntity3", 1);

            return Task.FromResult(3);
        }

        private void AddNamedEntity(IList<ValueTuple<string, short>> nentities, string newEntity, short score = 1)
        {
            if (!nentities.Any(ne => ne.Item1 == newEntity)) { 
                nentities.Add((newEntity, score));
            }
        }
    }
}
