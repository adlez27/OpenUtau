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
            "a i u e o @ ay ey oy ow aw".Split();

        protected override string[] GetConsonants() => new string[0];
            //"k ky g gy ng s sh z j zh t ch ts d dz n ny h hy f ff b by p py m my y r ry l rr w v th dh".Split();

        protected override string GetDictionaryName() => "";
        protected override IG2p LoadBaseDictionary() => new ArpabetG2p();

        protected override Dictionary<string, string> GetDictionaryPhonemesReplacement() =>
            new Dictionary<string, string> {
                { "aa", "a" },
                { "ae", "e" },
                { "ah", "@" },
                { "ao", "o" },
                { "eh", "e" },
                { "er", "@" },
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
            var diphthongs = "ay ey oy ow aw".Split();
            var adjustedSymbols = new List<string>();
            foreach(var symbol in symbols) {
                if (diphthongs.Contains(symbol)) {
                    adjustedSymbols.Add(symbol.First().ToString());
                    adjustedSymbols.Add(symbol.Last().ToString());
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
            foreach(var c in ending.cc) {
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
                    } else if (i < consonants.Count-1){
                        phonemes.Add(consonants[i]);
                    }
                    prevCons = consonants[i];
                }
                phonemes.Add($"{lastCons} -");
            }


            return phonemes;
        }

        protected override List<string> ProcessSyllable(Syllable syllable) {
            return new List<string> { syllable.ToString() + "_G3" };
        }
    }
}
