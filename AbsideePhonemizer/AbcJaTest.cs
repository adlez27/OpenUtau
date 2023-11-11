using OpenUtau.Api;
using OpenUtau.Classic;
using OpenUtau.Plugins;
using Xunit;
using Xunit.Abstractions;

namespace AbsideePhonemizer {
    public class AbcJaTest : PhonemizerTestBase {
        public AbcJaTest(ITestOutputHelper output) : base(output) { }

        protected override Phonemizer CreatePhonemizer() {
            return new AbsideeJapanesePhonemizer();
        }

        protected override Voicebank GetVoicebank(string singerName) {
            return AbsideePhonemizerUtil.GetVoicebank(singerName);
        }

        [Theory]
        [InlineData(
            new string[] { "ら", "ら" },
            new string[] { "- ら", "a r", "ら", "a -" })]
        [InlineData(
            new string[] { "ら", "い" },
            new string[] { "- ら", "a い", "i -" })]
        public void BasicPhonemizeTest(string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest("", lyrics, aliases, "", "C4", "");
        }

        [Theory]
        [InlineData(
            new string[] { "ら", "-" },
            new string[] { "- ら", "a -" })]
        [InlineData(
            new string[] { "ら", "hh" },
            new string[] { "- ら", "a hh" })]
        public void SeparatedTailTest(string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest("", lyrics, aliases, "", "C4", "");
        }

        [Theory]
        [InlineData(
            new string[] { "ん", "や", "ん", "か", "ん", "た", "ん", "ぱ" },
            new string[] { "- ん", "n y", "や", "a nng", "n k", "か", "a nn", "n t", "た", "a mm", "n p", "ぱ", "a -", })]
        public void ContextualNasalTest(string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest("", lyrics, aliases, "", "C3", "");
        }

        [Fact]
        public void ColorTest() {
            RunPhonemizeTest("", new NoteParams[] {
                new NoteParams { lyric = "ら", hint = "", tone = "C4", phonemes = SamePhonemeParams(2, 0, 0, "Soft") }
            }, new string[] { "- ら_S", "a -_S" });
        }
    }
}
