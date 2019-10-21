using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Korzh.NLP
{

    public class BasicEnglishQuestionGenerator : IQuestionGenerator
    {
        public Task GenerateAsync(string text, IEnumerable<string> nentities, IList<string> questions)
        {
            foreach (var nent in nentities) {
                AddQuestion(questions, $"What is {nent}?");
                AddQuestion(questions, $"How to use {nent}?");
            }
            return Task.CompletedTask;
        }

        private void AddQuestion(IList<string> questions, string newQuestion)
        {
            if (!questions.Contains(newQuestion)) {
                questions.Add(newQuestion);
            }
        }
    }
}
