using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using OpenUtau.Api;
using OpenUtau.Classic;
using OpenUtau.Core.Ustx;
using static OpenUtau.Api.Phonemizer;

namespace AbsideePhonemizer {
    public static class AbsideePhonemizerUtil {
        public static Voicebank GetVoicebank(string _) {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var file = Path.Join(dir, "absidee", "character.txt");

            return new Voicebank() { File = file, BasePath = dir };
        }

        public static string AssignSuffix(USinger singer, string alias, int tone, string color) {
            if (color == "Soft") {
                return $"{alias}_S";
            } else {
                var subbanks = (List<USubbank>)singer.Subbanks;
                var subbank = subbanks.Find(subbank => string.IsNullOrEmpty(subbank.Color) && subbank.toneSet.Contains(tone));
                return $"{alias}{subbank.Suffix}";
            }
        }

        public static bool CanVCV(string alias, Note left, Note right, USinger singer) {
            var leftAttr = left.phonemeAttributes.MaxBy(a => a.index);
            var rightAttr = right.phonemeAttributes.MinBy(a => a.index);

            // User can force CVVC with alternates
            if (leftAttr.alternate != rightAttr.alternate) { return false; }
            // Must be same voice color
            if (leftAttr.voiceColor != rightAttr.voiceColor) { return false; }

            // Must be same pitch subbank
            var subbanks = (List<USubbank>)singer.Subbanks;
            var leftTone = left.tone + leftAttr.toneShift;
            var rightTone = right.tone + rightAttr.toneShift;
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
