using OpenUtau.Api;
using OpenUtau.Plugins;
using Xunit;
using Xunit.Abstractions;

namespace TetoEnPhonemizer {
    public class EnTetoTest : PhonemizerTestBase {
        public EnTetoTest(ITestOutputHelper output) : base(output) { }
        protected override Phonemizer CreatePhonemizer() {
            return new TetoEnglishPhonemizer();
        }

        [Theory]
        [InlineData("teto",
            new string[] { "test", "words" },
            new string[] { "C4", "C4" },
            new string[] { "", "", },
            new string[] { "- tE", "E s", "st", "tw", "w3", "3 d", "d z-" })]
        public void BasicPhonemizingTest(string singerName, string[] lyrics, string[] tones, string[] colors, string[] aliases) {
            RunPhonemizeTest(singerName, lyrics, tones, colors, aliases);
        }
    }
}
