using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using NumSharp.Utilities;
using OpenUtau.Api;
using OpenUtau.Core.G2p;
using OpenUtau.Core.Ustx;
using OpenUtau.Plugin.Builtin;
using YamlDotNet.Core.Tokens;

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

        private AbsideePhonemizerUtil util;
        public AbsideeEnglishPhonemizer() {
            util = new AbsideePhonemizerUtil();
        }

        public override void SetSinger(USinger singer) {
            base.SetSinger(singer);
            util.Singer = singer;
        }

        protected override Phoneme[] AssignAllAffixes(List<Phoneme> phonemes, Note[] notes, Note[] prevs) {
            return util.AssignSuffixes(phonemes, notes, prevs);
        }

        protected override string[] GetSymbols(Note note) {
            if (tails.Contains(note.lyric)) {
                return new string[] { note.lyric };
            }
            if (note.lyric.ToUpper() == "AND") {
                return new string[] { "e", "n", "d" };
            }

            var symbols = base.GetSymbols(note);

            // combine affricates unless hinted otherwise
            if (string.IsNullOrEmpty(note.phoneticHint)) {
                var symbolString = string.Join(" ", symbols);
                symbolString = symbolString.Replace("t s", "ts");
                symbolString = symbolString.Replace("d z", "dz");
                symbolString = symbolString.Replace("t rr", "ch rr");
                symbolString = symbolString.Replace("d rr", "j rr");
                symbols = symbolString.Split();
            }

            // split diphthongs
            var diphthongs = new Dictionary<string, string[]> {
                { "ay", new string[] {"a", "y"} },
                { "ey", new string[] {"e", "y"} },
                { "oy", new string[] {"o", "y"} },
                { "aw", new string[] {"a", "w"} },
                { "ow", new string[] {"o", "w"} },
                { "@r", new string[] {"@", "rr"} },
            };

            var adjustedSymbols = new List<string>();
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
            if (tails.Contains(ending.prevV)) {
                return new List<string>();
            }

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
                if (affricates.Contains(prevCons)) {
                    phonemes.Add(prevCons);
                }
                for (var i = 1; i < consonants.Count; i++) {
                    var currCons = consonants[i];
                    if (prevCons == "i" || prevCons == "u") {
                        phonemes.Add($"{prevCons} {currCons}");
                    } else if (IsValidNasalCluster(prevCons, currCons )) {
                        phonemes.Add($"n {currCons}");
                    } else if (i < consonants.Count-1){
                        phonemes.Add(currCons);
                    }
                    prevCons = currCons;
                }
                phonemes.Add($"{consonants.Last()} -");
            }

            return phonemes;
        }

        protected override List<string> ProcessSyllable(Syllable syllable) {
            syllable.prevV = tails.Contains(syllable.prevV) ? "" : syllable.prevV;
            var prevV = syllable.prevV == "" ? "" : $"{syllable.prevV} ";
            var cc = syllable.cc;
            var v = syllable.v;

            if (syllable.IsStartingV) {
                return new List<string> { $"- {v}" };
            }
            if (syllable.IsVV) {
                return new List<string> { $"{prevV}{v}" };
            }
            if (syllable.IsStartingCVWithOneConsonant) {
                return new List<string> { $"- {cc[0]}{v}" };
            }

            if (prevV == "@ " && cc[0] == "rr" && cc.Length > 1) {
                cc = cc.RemoveAt(0);
                syllable.cc = cc;
            }

            if (syllable.IsVCVWithOneConsonant) {
                var vcv = $"{prevV}{cc[0]}{v}";
                if (util.CanVCV(vcv, syllable.attr, syllable.tone, syllable.vowelAttr, syllable.vowelTone)) {
                    return new List<string> { vcv };
                } else if (!tails.Contains(v)) {
                    return new List<string> {$"{prevV}{cc[0]}", $"{cc[0]}{v}" };
                }
            }

            var liquidPairs = new Dictionary<string, string> {
                { "rr", "@" },
                { "l", "@" },
                { "y", "i" },
                { "w", "u" },
                { "i", "i" },
                { "u", "u" }
            };

            var phonemes = new List<string>();
            cc[0] = cc[0].Replace("y", "i");
            cc[0] = cc[0].Replace("w", "u");

            var prev = prevV;
            for(var i = 0; i < cc.Length-1; i++) {
                var current = cc[i];
                if (liquidPairs.ContainsKey(cc[i+1])) {
                    if (i == 0 && prevV == "") {
                        phonemes.Add($"- {current}{liquidPairs[cc[i + 1]]}");
                    } else {
                        if (prev != "") {
                            phonemes.Add($"{prev}{current}");
                        }
                        if (!liquidPairs.ContainsKey(current)) {
                            phonemes.Add($"{current}{liquidPairs[cc[i+1]]}");
                        }
                    }
                } else {
                    phonemes.Add($"{prev}{current}");
                    if (affricates.Contains(current)) {
                        phonemes.Add(current);
                    }
                }

                if (current == "i" || current == "u") {
                    prev = $"{current} ";
                } else {
                    if (IsValidNasalCluster(current, cc[i+1])) {
                        phonemes.Add($"n {cc[i+1]}");
                        i++;
                    }
                    prev = "";
                }
            }
            if (prev != "") {
                phonemes.Add($"{prev}{cc.Last()}");
            }

            // final CV
            if (v == "-") {
                phonemes.Add($"{cc.Last()} -");
            } else if (v == "hh") {
                if (GetVowels().Contains(cc.Last())) {
                    phonemes.Add($"{cc.Last()} hh");
                } else {
                    phonemes.AddRange(new string[] { $"{cc.Last()}@", "@ hh" });
                }
            } else {
                phonemes.Add($"{cc.Last()}{v}");
            }

            return phonemes;
        }

        private string[] affricates = "ch j ts dz".Split();
        private string[] tails = "- hh".Split();

        private bool IsValidNasalCluster(string nasal, string consonant) {
            var valid = new Dictionary<string, string[]> {
                {"n", new string[] {"t", "ch", "ts", "d", "dz", "r", "l"} },
                {"m", new string[] {"b", "by", "p", "py" } },
                {"ng", new string[] {"k", "ky", "g", "gy"} }
            };
            return valid.ContainsKey(nasal) ? valid[nasal].Contains(consonant) : false;
        }
    }
}
