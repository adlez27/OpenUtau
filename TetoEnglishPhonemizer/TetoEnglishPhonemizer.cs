using System.Diagnostics.Metrics;
using OpenUtau.Api;
using OpenUtau.Core.G2p;
using OpenUtau.Plugin.Builtin;

namespace TetoEnPhonemizer {
    [Phonemizer("Teto English Phonemizer", "EN Teto", "Adlez27", language: "EN")]
    public class TetoEnglishPhonemizer : SyllableBasedPhonemizer {
        protected override string GetDictionaryName() => "cmudict-0_7b.txt";
        protected override IG2p LoadBaseDictionary() => new ArpabetG2p();
        protected override string[] GetVowels() => "{ @ 3 A aI aU E eI i I O OI oU u U V".Split();
        protected override string[] GetConsonants() => "p b t d k g tS dZ f v T D s z S Z h m n N l r y w".Split();
        protected override Dictionary<string, string> GetDictionaryPhonemesReplacement() {
            return new Dictionary<string, string> {
                {"aa", "A" },
                {"ae", "{" },
                {"ah", "V" },
                {"ao", "O" },
                {"eh", "E" },
                {"er", "3" },
                {"ih", "I" },
                {"iy", "i" },
                {"uh", "U" },
                {"uw", "u" },
                {"ay", "aI" },
                {"ey", "eI" },
                {"oy", "OI" },
                {"ow", "oU" },
                {"aw", "aU" },
                {"ax", "@" },
                {"p", "p" },
                {"b", "b" },
                {"t", "t" },
                {"d", "d" },
                {"k", "k" },
                {"g", "g" },
                {"ch", "tS" },
                {"jh", "dZ" },
                {"f", "f" },
                {"v", "v" },
                {"th", "T" },
                {"dh", "D" },
                {"s", "s" },
                {"z", "z" },
                {"sh", "S" },
                {"zh", "Z" },
                {"hh", "h" },
                {"m", "m" },
                {"n", "n" },
                {"ng", "N" },
                {"l", "l" },
                {"r", "r" },
                {"y", "j" },
                {"w", "w" }
            };
        }

        protected override List<string> ProcessSyllable(Syllable syllable) {
            var phonemes = new List<string>();

            phonemes.Add(syllable.ToString());

            return phonemes;
        }

        protected override List<string> ProcessEnding(Ending ending) {
            if (ending.IsEndingV) {
                return new List<string> { $"{ending.prevV} -" };
            }
            
            if (ending.IsEndingVCWithOneConsonant) {
                return new List<string> { $"{ending.prevV} {ending.cc[0]}-" };
            }

            var phonemes = new List<string>();

            phonemes.Add($"{ending.prevV} {ending.cc[0]}");
            
            var cluster = $"{ending.cc[0]} ";
            if (ending.cc.Length == 2) {
                cluster += $"{ending.cc[1]}-";
            }
            if (ending.cc.Length == 3) {
                cluster += $"{ending.cc[1]}{ending.cc[2]}-";
            }
            if (HasOto(cluster, ending.tone)) {
                phonemes.Add(cluster);
                return phonemes;
            }

            // construct the cc out of bits

            return phonemes;
        }

        protected override string ValidateAlias(string alias) {
            if (alias == "- bV" || alias == "bV" || alias == "V b" || alias == "V b-") {
                return alias.Replace('V', '@');
            } else {
                return base.ValidateAlias(alias);
            }
        }
    }
}
