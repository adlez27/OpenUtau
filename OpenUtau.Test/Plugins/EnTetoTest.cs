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
            new string[] { "the" },
            new string[] { "() D V", "V -"})]
        public void VowelEndTest(string singerName, string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest(singerName, lyrics, aliases);
        }

        [Theory]
        [InlineData("teto",
            new string[] { "this" },
            new string[] { "() D I", "I s-" })]
        public void SingleConsonantEndTest(string singerName, string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest(singerName, lyrics, aliases);
        }

        [Theory]
        [InlineData("teto",
            new string[] { "end" },
            new string[] { "()  E", "E n", "n d-" })]
        [InlineData("teto",
            new string[] { "ends" },
            new string[] { "()  E", "E n", "n dz-" })]
        public void SimpleConsonantClusterEndTest(string singerName, string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest(singerName, lyrics, aliases);
        }

        [Theory]
        [InlineData("teto",
            new string[] { "argd" },
            new string[] { "a" })]
        public void ComplexConsonantClusterEndTest(string singerName, string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest(singerName, lyrics, aliases);
        }

        private void SameAltsTonesColorsTest(string singerName, string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest(singerName, lyrics, "", "C4", "", aliases);
        }
    }
}
