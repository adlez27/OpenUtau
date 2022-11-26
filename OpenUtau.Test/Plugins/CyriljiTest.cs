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
        // Basic function
        [InlineData("ja_cyrilji",
            new string[] {"ら", "ら" },
            new string[] {"C3", "C3"},
            new string[] {"", ""},
            new string[] {"- ла", "а л", "ла", "а -" })]
        [InlineData("ja_cyrilji",
            new string[] { "ら", "い" },
            new string[] { "C3", "C3" },
            new string[] { "", "" },
            new string[] { "- ла", "а и", "и -" })]
        // Separable tail
        [InlineData("ja_cyrilji",
            new string[] { "ら", "-" },
            new string[] { "C3", "C3" },
            new string[] { "", "" },
            new string[] { "- ла", "а -" })]
        // Palatalized consonant
        [InlineData("ja_cyrilji",
            new string[] { "と", "きょ" },
            new string[] { "C3", "C3" },
            new string[] { "", "" },
            new string[] { "- то", "о кь", "кё", "о -" })]
        //Multipitch
        [InlineData("ja_cyrilji",
            new string[] { "ら", "ら", "ら" },
            new string[] { "C3", "C4", "C3" },
            new string[] { "", "", "" },
            new string[] { "- ла", "а л", "ла_H", "а л_H", "ла", "а -" })]
        [InlineData("ja_cyrilji",
            new string[] { "ら", "い", "ら" },
            new string[] { "C3", "C4", "C3" },
            new string[] { "", "", "" },
            new string[] { "- ла", "а и_H", "и л_H", "ла", "а -" })]
        // Voice color works normally in UI
        [InlineData("ja_cyrilji",
            new string[] { "ら", "ら", "ら" },
            new string[] { "C3", "C3", "C3" },
            new string[] { "", "Power", "" },
            new string[] { "- ла", "а л", "ла_P", "а л", "ла", "а -" })]
        [InlineData("ja_cyrilji",
            new string[] { "ら", "い", "ら" },
            new string[] { "C3", "C3", "C3" },
            new string[] { "", "Power", "" },
            new string[] { "- ла", "а и_P", "и л", "ла", "а -" })]
        // Context-based nasal
        [InlineData("ja_cyrilji",
            new string[] { "ん", "や", "ん", "か", "ん", "た", "ん", "ぱ" },
            new string[] { "C3", "C3", "C3", "C3", "C3", "C3", "C3", "C3" },
            new string[] { "", "", "", "", "", "", "", "" },
            new string[] { "- Н", "н й", "я", "а нг", "н к", "ка", "а нн", "н т", "та", "а мм", "н п", "па", "а -", })]
        public void PhonemizeTest(string singerName, string[] lyrics, string[] tones, string[] colors, string[] aliases) {
            RunPhonemizeTest(singerName, lyrics, tones, colors, aliases);
        }
    }
}
