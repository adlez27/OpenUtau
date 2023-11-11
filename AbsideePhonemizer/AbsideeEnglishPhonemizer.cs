using System;
using System.Collections.Generic;
using System.Linq;
using OpenUtau.Api;
using OpenUtau.Core.Ustx;
using OpenUtau.Plugin.Builtin;

namespace AbsideePhonemizer {
    [Phonemizer("Absidee English Phonemizer", "EN ABC", "Adlez27", "EN")]
    public class AbsideeEnglishPhonemizer : SyllableBasedPhonemizer {
        protected override string[] GetVowels() {
            throw new NotImplementedException();
        }

        protected override List<string> ProcessEnding(Ending ending) {
            throw new NotImplementedException();
        }

        protected override List<string> ProcessSyllable(Syllable syllable) {
            throw new NotImplementedException();
        }
    }
}
