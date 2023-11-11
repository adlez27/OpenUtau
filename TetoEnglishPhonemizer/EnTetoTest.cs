using System.Reflection;
using System.Text;
using OpenUtau.Api;
using OpenUtau.Classic;
using OpenUtau.Plugins;
using Xunit;
using Xunit.Abstractions;

namespace TetoEnPhonemizer {
    public class EnTetoTest : PhonemizerTestBase {
        public EnTetoTest(ITestOutputHelper output) : base(output) { }
        protected override Phonemizer CreatePhonemizer() {
            return new TetoEnglishPhonemizer();
        }

        protected override Voicebank GetVoicebank(string singerName) {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var file = Path.Join(dir, "teto", "character.txt");

            return new Voicebank() { File = file, BasePath = dir };
        }

        [Theory]
        [InlineData(
            new string[] { "the" },
            new string[] { "() D V", "V -"})]
        public void VowelEndTest(string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest(lyrics, aliases);
        }

        [Theory]
        [InlineData(
            new string[] { "this" },
            new string[] { "() D I", "I s-" })]
        public void SingleConsonantEndTest(string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest(lyrics, aliases);
        }

        [Theory]
        [InlineData(
            new string[] { "end" },
            new string[] { "()  E", "E n", "n d-" })]
        [InlineData(
            new string[] { "ends" },
            new string[] { "()  E", "E n", "n dz-" })]
        public void SimpleConsonantClusterEndTest(string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest(lyrics, aliases);
        }

        [Theory]
        [InlineData(
            new string[] { "argd" },
            new string[] { "a" })]
        public void ComplexConsonantClusterEndTest(string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest(lyrics, aliases);
        }

        private void SameAltsTonesColorsTest(string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest("", lyrics, aliases, "", "C4", "");
        }
    }
}
