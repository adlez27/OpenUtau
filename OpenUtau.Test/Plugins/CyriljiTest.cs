using OpenUtau.Api;
using OpenUtau.Plugins;
using Xunit;
using Xunit.Abstractions;

namespace CyriljiPhonemizer {
    public class CyriljiTest : PhonemizerTestBase {
        public CyriljiTest(ITestOutputHelper output) : base(output) { }
        protected override Phonemizer CreatePhonemizer() {
            return new JapaneseCyrillicPhonemizer();
        }

        [Theory]
        [InlineData("ja_cyrilji",
            new string[] {"ら", "ら" },
            new string[] {"- ла", "а л", "ла", "а -" })]
        [InlineData("ja_cyrilji",
            new string[] { "ら", "い" },
            new string[] { "- ла", "а и", "и -" })]
        public void BasicPhonemizeTest(string singerName, string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest(singerName, lyrics, aliases, "", "C3", "");
        }

        [Theory]
        [InlineData("ja_cyrilji",
            new string[] { "ら", "-" },
            new string[] { "- ла", "а -" })]
        public void SeparatedTailTest(string singerName, string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest(singerName, lyrics, aliases, "", "C3", "");
        }

        [Theory]
        [InlineData("ja_cyrilji",
            new string[] { "と", "きょ" },
            new string[] { "- то", "о кь", "кё", "о -" })]
        public void PalatalizedConsonantTest(string singerName, string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest(singerName, lyrics, aliases, "", "C3", "");
        }

        [Theory]
        [InlineData("ja_cyrilji",
            new string[] { "ら", "ら", "ら" },
            new string[] { "C3", "C4", "C3" },
            new string[] { "- ла", "а л", "ла_H", "а л_H", "ла", "а -" })]
        [InlineData("ja_cyrilji",
            new string[] { "ら", "い", "ら" },
            new string[] { "C3", "C4", "C3" },
            new string[] { "- ла", "а и_H", "и л_H", "ла", "а -" })]
        public void MultipitchTest(string singerName, string[] lyrics, string[] tones, string[] aliases) {
            RunPhonemizeTest(singerName, lyrics,
                RepeatString(lyrics.Length, ""),
                tones,
                RepeatString(lyrics.Length, ""), aliases);
        }

        [Theory]
        [InlineData("ja_cyrilji",
            new string[] { "ら", "ら", "ら" },
            new string[] { "", "Power", "" },
            new string[] { "- ла", "а л", "ла_P", "а л", "ла", "а -" })]
        [InlineData("ja_cyrilji",
            new string[] { "ら", "い", "ら" },
            new string[] { "", "Power", "" },
            new string[] { "- ла", "а и_P", "и л", "ла", "а -" })]
        // Color is per phoneme, it behaves as expected in UI
        public void VoiceColorTest(string singerName, string[] lyrics,  string[] colors, string[] aliases) {
            RunPhonemizeTest(singerName, lyrics,
                RepeatString(lyrics.Length, ""),
                RepeatString(lyrics.Length, "C3"),
                colors, aliases);
        }

        [Theory]
        [InlineData("ja_cyrilji",
            new string[] { "ん", "や", "ん", "か", "ん", "た", "ん", "ぱ" },
            new string[] { "- Н", "н й", "я", "а нг", "н к", "ка", "а нн", "н т", "та", "а мм", "н п", "па", "а -", })]
        public void ContextualNasalTest(string singerName, string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest(singerName, lyrics, aliases, "", "C3", "");
        }
    }
}
