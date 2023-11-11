using OpenUtau.Api;
using OpenUtau.Classic;
using OpenUtau.Plugins;
using Xunit;
using Xunit.Abstractions;

namespace AbsideePhonemizer {
    public class AbcEnTest : PhonemizerTestBase {
        public AbcEnTest(ITestOutputHelper output) : base(output) { }

        protected override Phonemizer CreatePhonemizer() {
            return new AbsideeEnglishPhonemizer();
        }

        protected override Voicebank GetVoicebank(string singerName) {
            return AbsideePhonemizerUtil.GetVoicebank(singerName);
        }

        [Theory]
        [InlineData(
            new string[] { },
            new string[] { })]
        public void BasicPhonemizeTest(string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest("", lyrics, aliases, "", "C4", "");
        }
    }
}
