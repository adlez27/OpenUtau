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
            new string[] {"ла"},
            new string[] {"C3"},
            new string[] {""},
            new string[] {"ла"})]
        // Multipitch
        [InlineData("ja_cyrilji",
            new string[] { "ла", "ла" },
            new string[] { "C3", "C4" },
            new string[] { "", "" },
            new string[] { "ла", "ла_H" })]
        // Voice color
        [InlineData("ja_cyrilji",
            new string[] { "ла", "ла" },
            new string[] { "C3", "C3" },
            new string[] { "", "Power" },
            new string[] { "ла", "ла_P" })]

        public void PhonemizeTest(string singerName, string[] lyrics, string[] tones, string[] colors, string[] aliases) {
            RunPhonemizeTest(singerName, lyrics, tones, colors, aliases);
        }
    }
}
