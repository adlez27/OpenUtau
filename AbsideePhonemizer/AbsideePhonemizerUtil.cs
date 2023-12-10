using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using OpenUtau.Api;
using OpenUtau.Classic;
using OpenUtau.Core.Ustx;
using WanaKanaNet;
using static OpenUtau.Api.Phonemizer;

namespace AbsideePhonemizer {
    public class AbsideePhonemizerUtil {
        private USinger singer;

        public USinger Singer { get => singer; set => singer = value; }

        public static Voicebank GetVoicebank() {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var file = Path.Join(dir, "absidee", "character.txt");

            return new Voicebank() { File = file, BasePath = dir };
        }

        public Phoneme[] AssignSuffixes(List<Phoneme> phonemes, Note[] notes, Note[] prevs) {
            var adjustedPhonemes = new List<Phoneme>();
            
            var note = notes[0];
            int noteIndex = 0;
            for (int i = 0; i < phonemes.Count; i++) {
                var attr = note.phonemeAttributes?.FirstOrDefault(attr => attr.index == i) ?? default;
                string color = attr.voiceColor;
                int toneShift = attr.toneShift;
                var phoneme = phonemes[i];
                while (noteIndex < notes.Length - 1 && notes[noteIndex].position - note.position < phoneme.position) {
                    noteIndex++;
                }
                int tone = (i == 0 && prevs != null && prevs.Length > 0)
                    ? prevs.Last().tone : notes[noteIndex].tone;

                adjustedPhonemes.AddRange(AssignSuffix(phoneme, note.tone + toneShift, color));
            }
            return adjustedPhonemes.ToArray();
        }

        public List<Phoneme> AssignSuffix(Phoneme phoneme, int tone, string color) {
            if (color == "Soft") {
                phoneme.phoneme += "_S";
                return new List<Phoneme> { phoneme };
            }

            var subbanks = (List<USubbank>)singer.Subbanks;
            var subbank = subbanks.Find(subbank => string.IsNullOrEmpty(subbank.Color) && subbank.toneSet.Contains(tone));
            var alias = phoneme.phoneme + subbank.Suffix;
            if (subbank.Suffix != "_Eb5" || (subbank.Suffix == "_Eb5" && singer.TryGetOto(alias, out _))) {
                phoneme.phoneme = alias;
                return new List<Phoneme> { phoneme };
            } 

            var aliasEnd = phoneme.phoneme.Last().ToString();
            phoneme.phoneme += "_Bb4";

            var vowel = WanaKana.IsHiragana(aliasEnd) ? WanaKana.ToRomaji(aliasEnd).Last().ToString() : aliasEnd;
            var validVowels = "a i u e o n @".Split();
            if (!validVowels.Contains(vowel)) {
                return new List<Phoneme> { phoneme };
            }

            vowel = vowel == "n" ? "N" : vowel;
            var split = new List<Phoneme> { 
                phoneme,
                new Phoneme {
                    phoneme = $"{vowel}_Eb5",
                    position = phoneme.position + 10,
                    attributes = phoneme.attributes
                }
            };
            return split;
        }

        public bool CanVCV(string alias, Note left, Note right) {
            var leftHasAttrs = left.phonemeAttributes.Length > 0;
            var rightHasAttrs = right.phonemeAttributes.Length > 0;

            var leftAttr = leftHasAttrs ? left.phonemeAttributes.MaxBy(a => a.index) : new PhonemeAttributes { index = 0, toneShift = 0, voiceColor = null };
            var rightAttr = rightHasAttrs ? right.phonemeAttributes.MinBy(a => a.index) : new PhonemeAttributes { index = 0, toneShift = 0, voiceColor = null };

            return CanVCV(alias, leftAttr, left.tone, rightAttr, right.tone);
        }

        public bool CanVCV(string alias, PhonemeAttributes[] left, int leftTone, PhonemeAttributes[] right, int rightTone) {
            var leftHasAttrs = left.Length > 0;
            var rightHasAttrs = right.Length > 0;

            var leftAttr = leftHasAttrs ? left.MaxBy(a => a.index) : new PhonemeAttributes { index = 0, toneShift = 0, voiceColor = null };
            var rightAttr = rightHasAttrs ? right.MinBy(a => a.index) : new PhonemeAttributes { index = 0, toneShift = 0, voiceColor = null };

            return CanVCV(alias, leftAttr, leftTone, rightAttr, rightTone);
        }

        private bool CanVCV(string alias, PhonemeAttributes leftAttr, int leftBaseTone, PhonemeAttributes rightAttr, int rightBaseTone) {
            // User can force CVVC with alternates
            if (leftAttr.alternate != rightAttr.alternate) { return false; }
            // Must be same voice color
            if (leftAttr.voiceColor != rightAttr.voiceColor) { return false; }

            // Must be same pitch subbank
            var subbanks = (List<USubbank>)singer.Subbanks;
            var leftTone = leftBaseTone + leftAttr.toneShift;
            var rightTone = rightBaseTone + rightAttr.toneShift;
            var subbank = subbanks.Find(subbank => string.IsNullOrEmpty(subbank.Color)
                && subbank.toneSet.Contains(leftTone)
                && subbank.toneSet.Contains(rightTone));
            if (subbank == null) {
                return false;
            }

            // Alias must exist in OTO
            return singer.TryGetMappedOto(alias, rightTone, out _);
        }
    }
}
