using System;
using System.Collections.Generic;
using System.Linq;
using OpenUtau.Api;
using OpenUtau.Core.G2p;
using OpenUtau.Core.Ustx;
using OpenUtau.Plugin.Builtin;

namespace AbsideePhonemizer {
    [Phonemizer("Absidee English Phonemizer", "EN ABC", "Adlez27", "EN")]
    public class AbsideeEnglishPhonemizer : SyllableBasedPhonemizer {
        protected override string[] GetVowels() =>
            "a i u e o @ @r ay ey oy ow aw".Split();

        protected override string[] GetConsonants() => new string[0];

        protected override string GetDictionaryName() => "";
        protected override IG2p LoadBaseDictionary() => new ArpabetG2p();

        protected override Dictionary<string, string> GetDictionaryPhonemesReplacement() =>
            new Dictionary<string, string> {
                { "aa", "a" },
                { "ae", "e" },
                { "ah", "@" },
                { "ao", "o" },
                { "eh", "e" },
                { "er", "@r" },
                { "f", "ff" },
                { "hh", "h" },
                { "ih", "e" },
                { "iy", "i" },
                { "jh", "j" },
                { "r", "rr" },
                { "uh", "@" },
                { "uw", "u" }
            };

        protected override string[] GetSymbols(Note note) {
            var symbols = base.GetSymbols(note);
            var adjustedSymbols = new List<string>();

            // combine affricates
            for (var i = 0; i < symbols.Length - 1; i++) {
                var cc = $"{symbols[i]}{symbols[i+1]}";
                if (cc == "ts" || cc == "dz") {
                    adjustedSymbols.Add(cc);
                } else {
                    adjustedSymbols.Add(symbols[i]);
                    if (i == symbols.Length - 2) {
                        adjustedSymbols.Add(symbols[i + 1]);
                    }
                }
            }

            if (symbols.Length == 1) {
                adjustedSymbols.Add(symbols[0]);
            }

            symbols = adjustedSymbols.ToArray();
            adjustedSymbols.Clear();

            // split diphthongs
            var diphthongs = new Dictionary<string, string[]> {
                { "ay", new string[] {"a", "y"} },
                { "ey", new string[] {"e", "y"} },
                { "oy", new string[] {"o", "y"} },
                { "aw", new string[] {"a", "w"} },
                { "ow", new string[] {"o", "w"} },
                { "@r", new string[] {"@", "rr"} },
            };

            foreach (var symbol in symbols) {
                if (diphthongs.Keys.Contains(symbol)) {
                    adjustedSymbols.AddRange(diphthongs[symbol]);
                } else {
                    adjustedSymbols.Add(symbol);
                }
            }
            
            return adjustedSymbols.ToArray();
        }

        protected override List<string> ProcessEnding(Ending ending) {
            if (ending.IsEndingV) {
                return new List<string> { $"{ending.prevV} -" }; 
            }

            var consonants = new List<string>();
            for (var i = 0; i < ending.cc.Length; i++) {
                var c = ending.cc[i];
                if (i == 0 && ending.cc.Length > 1 && ending.prevV == "@" && c == "rr") {
                    continue;
                }

                if (c == "y") {
                    consonants.Add("i");
                } else if (c == "w") {
                    consonants.Add("u");
                } else {
                    consonants.Add(c);
                }
            }

            var phonemes = new List<string> {
                $"{ending.prevV} {consonants[0]}"
            };
            
            if (ending.IsEndingVCWithOneConsonant) {
                phonemes.Add($"{consonants[0]} -");
            } else {
                var prevCons = consonants[0];
                var lastCons = consonants.Last();
                for (var i = 1; i < consonants.Count; i++) {
                    if (prevCons == "i" || prevCons == "u") {
                        phonemes.Add($"{prevCons} {consonants[i]}");
                    } else if (IsValidNasalCluster(prevCons, consonants[i] )) {
                        phonemes.Add($"n {consonants[i]}");
                    } else if (i < consonants.Count-1){
                        phonemes.Add(consonants[i]);
                    }
                    prevCons = consonants[i];
                }
                phonemes.Add($"{lastCons} -");
            }

            return phonemes;
        }

        private bool IsValidNasalCluster(string nasal, string consonant) {
            var valid = new Dictionary<string, string[]> {
                {"n", new string[] {"n", "t", "ch", "ts", "d", "dz", "r", "l"} },
                {"m", new string[] {"m", "b", "by", "p", "py" } },
                {"ng", new string[] {"ng","k", "ky", "g", "gy"} }
            };
            return valid[nasal].Contains(consonant);
        }

        protected override List<string> ProcessSyllable(Syllable syllable) {
            return new List<string> { syllable.ToString() + "_G3" };
        }
    }
}
