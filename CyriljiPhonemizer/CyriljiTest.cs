using System.Reflection;
using System.Text;
using OpenUtau.Api;
using OpenUtau.Classic;
using OpenUtau.Plugins;
using Xunit;
using Xunit.Abstractions;

namespace CyriljiPhonemizer {
    public class CyriljiTest : PhonemizerTestBase {
        public CyriljiTest(ITestOutputHelper output) : base(output) { }
        protected override Phonemizer CreatePhonemizer() {
            return new JapaneseCyrillicPhonemizer();
        }

        protected override Voicebank GetVoicebank(string singerName) {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var file = Path.Join(dir, "ja_cyrilji", "character.txt");

            return new Voicebank() { File = file, BasePath = dir };
        }

        [Theory]
        [InlineData(
            new string[] {"ら", "ら" },
            new string[] {"- ла", "а л", "ла", "а -" })]
        [InlineData(
            new string[] { "ら", "い" },
            new string[] { "- ла", "а и", "и -" })]
        public void BasicPhonemizeTest(string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest("", lyrics, aliases, "", "C3", "");
        }

        [Theory]
        [InlineData(
            new string[] { "ら", "-" },
            new string[] { "- ла", "а -" })]
        public void SeparatedTailTest(string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest("", lyrics, aliases, "", "C3", "");
        }

        [Theory]
        [InlineData(
            new string[] { "と", "きょ" },
            new string[] { "- то", "о кь", "кё", "о -" })]
        public void PalatalizedConsonantTest(string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest("", lyrics, aliases, "", "C3", "");
        }

        [Theory]
        [InlineData(
            new string[] { "ら", "ら", "ら" },
            new string[] { "C3", "C4", "C3" },
            new string[] { "- ла", "а л", "ла_H", "а л_H", "ла", "а -" })]
        [InlineData(
            new string[] { "ら", "い", "ら" },
            new string[] { "C3", "C4", "C3" },
            new string[] { "- ла", "а и_H", "и л_H", "ла", "а -" })]
        public void MultipitchTest(string[] lyrics, string[] tones, string[] aliases) {
            RunPhonemizeTest("", lyrics,
                RepeatString(lyrics.Length, ""),
                tones,
                RepeatString(lyrics.Length, ""), aliases);
        }

        [Theory]
        [InlineData(
            new string[] { "ら", "ら", "ら" },
            new string[] { "", "Power", "" },
            new string[] { "- ла", "а л", "ла_P", "а л_P", "ла", "а -" })]
        [InlineData(
            new string[] { "ら", "い", "ら" },
            new string[] { "", "Power", "" },
            new string[] { "- ла", "а и_P", "и л_P", "ла", "а -" })]
        public void VoiceColorTest(string[] lyrics,  string[] colors, string[] aliases) {
            //RunPhonemizeTest("", lyrics,
            //    RepeatString(lyrics.Length, ""),
            //    RepeatString(lyrics.Length, "C3"),
            //    colors, aliases);
            RunPhonemizeTest("", new NoteParams[] {
                new NoteParams { lyric = lyrics[0], hint = "", tone = "C3", phonemes = SamePhonemeParams(2, 0, 0, colors[0])},
                new NoteParams { lyric = lyrics[1], hint = "", tone = "C3", phonemes = SamePhonemeParams(2, 0, 0, colors[1])},
                new NoteParams { lyric = lyrics[2], hint = "", tone = "C3", phonemes = SamePhonemeParams(2, 0, 0, colors[2])}
            }, aliases);
        }

        [Theory]
        [InlineData(
            new string[] { "ん", "や", "ん", "か", "ん", "た", "ん", "ぱ" },
            new string[] { "- Н", "н й", "я", "а нг", "н к", "ка", "а нн", "н т", "та", "а мм", "н п", "па", "а -", })]
        public void ContextualNasalTest(string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest("", lyrics, aliases, "", "C3", "");
        }
    }
}
