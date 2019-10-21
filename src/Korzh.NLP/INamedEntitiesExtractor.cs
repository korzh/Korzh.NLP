using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Korzh.NLP
{
    public interface INamedEntitiesExtractor
    {
        Task<int> CollectNamedEntitiesAsync(string text, IList<ValueTuple<string, short>> nentities);
    }
}
