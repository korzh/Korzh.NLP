using System;
using System.Collections.Generic;
using System.Text;

namespace Korzh.NLP
{
    public class TextFilterMapper : ITextFilterMapper {
        public Func<string, bool> Filter { get; set; } = (s) => true;

        public Func<string, string> Map { get; set; } = (s) => s;

        public static TextFilterMapper LowerCaseMapper {
            get {
                return new TextFilterMapper {
                    Map = (s) => s.ToLowerInvariant()
                };
            }
        }

        public static TextFilterMapper UpperCaseMapper {
            get {
                return new TextFilterMapper {
                    Map = (s) => s.ToUpperInvariant()
                };
            }
        }
    }
}
