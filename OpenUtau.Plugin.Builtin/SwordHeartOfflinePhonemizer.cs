using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenUtau.Api;
using OpenUtau.Core.G2p;

namespace OpenUtau.Plugin.Builtin {
    [Phonemizer("Sword Heart Offline Phonemizer", "EN SHO E", "Adlez27",
        language:"EN")]
    public class SwordHeartOfflinePhonemizer : SyllableBasedPhonemizer {
        protected override string[] GetVowels() => vowels;
        private static readonly string[] vowels = "a e i o u @".Split();
        protected override string[] GetConsonants() => consonants;
        private static readonly string[] consonants =
            "b by ch d dh dz f g gy h hy j k ky l m my n ny ng p py r ry rr s sh t ts th v w y z zh"
            .Split();
        protected override string GetDictionaryName() => "cmudict-0_7b.txt";
        protected override IG2p LoadBaseDictionary() => new ArpabetG2p();
        protected override Dictionary<string, string> GetDictionaryPhonemesReplacement() => replacement;
        private static readonly Dictionary<string, string> replacement =
            new Dictionary<string, string> {
                { "aa", "a" },
                { "ae", "e" },
                { "ah", "@" },
                { "ao", "o" },
                { "aw", "aw" },
                { "ay", "ay" },
                { "b", "b" },
                { "ch", "ch" },
                { "d", "d" },
                { "dh", "dh" },
                { "eh", "e" },
                { "er", "@" },
                { "ey", "ey" },
                { "f", "f" },
                { "g", "g" },
                { "hh", "h" },
                { "ih", "e" },
                { "iy", "i" },
                { "jh", "j" },
                { "k", "k" },
                { "l", "l" },
                { "m", "m" },
                { "n", "n" },
                { "ng", "ng" },
                { "ow", "ow" },
                { "oy", "oy" },
                { "p", "p" },
                { "r", "rr" },
                { "s", "s" },
                { "sh", "sh" },
                { "t", "t" },
                { "th", "th" },
                { "uh", "@" },
                { "uw", "u" },
                { "v", "v" },
                { "w", "w" },
                { "y", "y" },
                { "z", "z" },
                { "zh", "zh" },
            };
        private string[] SpecialClusters = "ky gy ts dz ny hy by py my".Split();

        protected override string[] GetSymbols(Note note) {
            string[] original = base.GetSymbols(note);
            if (original == null) {
                return null;
            }
            List<string> modified = new List<string>();
            string[] diphthongs = new[] { "ay", "ey", "oy", "ow", "aw" };
            foreach (string s in original) {
                if (diphthongs.Contains(s)) {
                    modified.AddRange(new string[] { s[0].ToString(), s[1].ToString() });
                } else {
                    modified.Add(s);
                }
            }
            return modified.ToArray();
        }
        
        protected override List<string> ProcessSyllable(Syllable syllable) {
            // Skip processing if this note extends the prevous syllable
            if (CanMakeAliasExtension(syllable)) {
                return new List<string> { null };
            }

            var prevV = syllable.prevV;
            var cc = syllable.cc;
            var v = syllable.v;
            var phonemes = new List<string>();

            if (prevV.Length == 0) {
                prevV = "-";
            }

            if (cc.Length == 0) {
                phonemes.Add($"{prevV} {v}");
                return phonemes;
            } else if (syllable.IsStartingCVWithOneConsonant) {
                phonemes.Add($"- {cc[0]}{v}");
                return phonemes;
            } else if (syllable.IsVCVWithOneConsonant) {
                phonemes.Add($"{cc[0]}{v}");
                return phonemes;
            }

            // Check CCs for special clusters
            var adjustedCC = new List<string>();
            for (var i = 0; i < cc.Length; i++) {
                if (i == cc.Length - 1) {
                    adjustedCC.Add(cc[i]);
                } else {
                    if (cc[i] == cc[i + 1]) {
                        adjustedCC.Add(cc[i]);
                        i++;
                        continue;
                    }
                    var diphone = $"{cc[i]}{cc[i + 1]}";
                    if (SpecialClusters.Contains(diphone)) {
                        adjustedCC.Add(diphone);
                        i++;
                    } else {
                        adjustedCC.Add(cc[i]);
                    }
                }
            }
            cc = adjustedCC.ToArray();

            // Separate CCs and main CV
            var finalCons = cc[cc.Length - 1];
            for (var i = 0; i < cc.Length - 1; i++) {
                var cons = $"{cc[i]}@";
                if (i == 0) {
                    if (cons == "y@")
                        cons = "i";
                    else if (cons == "w@")
                        cons = "u";
                }
                var vcv = $"{prevV} {cons}";
                    
                if (HasOto(vcv, syllable.tone)) {
                    phonemes.Add(vcv);
                } else if (HasOto(cons, syllable.tone)) {
                    phonemes.Add(cons);
                }
                prevV = "@";
            }

            phonemes.Add($"{finalCons}{v}");

            return phonemes;
        }

        protected override List<string> ProcessEnding(Ending ending) {
            var prevV = ending.prevV;
            var cc = ending.cc;
            var phonemes = new List<string>();

            if (ending.IsEndingV) {
                phonemes.Add($"{prevV} -");
                return phonemes;
            }

            // Check CCs for special clusters
            var adjustedCC = new List<string>();
            for (var i = 0; i < cc.Length; i++) {
                if (i == cc.Length - 1) {
                    adjustedCC.Add(cc[i]);
                } else {
                    if (cc[i] == cc[i + 1]) {
                        adjustedCC.Add(cc[i]);
                        i++;
                        continue;
                    }
                    var diphone = $"{cc[i]}{cc[i + 1]}";
                    if (SpecialClusters.Contains(diphone)) {
                        adjustedCC.Add(diphone);
                        i++;
                    } else {
                        adjustedCC.Add(cc[i]);
                    }
                }
            }
            cc = adjustedCC.ToArray();

            for (var i = 0; i < cc.Length; i++) {
                var cons = $"{cc[i]}@";
                if (i == 0) {
                    if (cons == "y@")
                        cons = "i";
                    else if (cons == "w@")
                        cons = "u";
                }
                var vcv = $"{prevV} {cons}";

                if (HasOto(vcv, ending.tone)) {
                    phonemes.Add(vcv);
                } else if (HasOto(cons, ending.tone)) {
                    phonemes.Add(cons);
                }
                prevV = "@";
            }

            return phonemes;
        }
    }
}
