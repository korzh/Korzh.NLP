using System.Collections.Generic;
using System.Threading.Tasks;

namespace Korzh.NLP
{
    public interface IQuestionGenerator
    {
        Task GenerateAsync(string text, IEnumerable<string> nentities, IList<string> questions);
    }
}
