using OpenUtau.Api;
using OpenUtau.Classic;
using OpenUtau.Plugins;
using Xunit;
using Xunit.Abstractions;

namespace AbsideePhonemizer.Test {
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
            new string[] { "- ら_G3", "a r_G3", "ら_G3", "a -_G3" })]
        [InlineData(
            new string[] { "ら", "い" },
            new string[] { "- ら_G3", "a い_G3", "i -_G3" })]
        public void BasicPhonemizeTest(string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest("", lyrics, aliases, "", "C4", "");
        }

        [Theory]
        [InlineData(
            new string[] { "ら", "-" },
            new string[] { "- ら_G3", "a -_G3" })]
        [InlineData(
            new string[] { "ら", "hh" },
            new string[] { "- ら_G3", "a hh_G3" })]
        public void SeparatedTailTest(string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest("", lyrics, aliases, "", "C4", "");
        }

        [Theory]
        [InlineData(
            new string[] { "ん", "や", "ん", "か", "ん", "た", "ん", "ぱ" },
            new string[] { "- ん_G3", "n y_G3", "や_G3", "a nng_G3", "n k_G3", "か_G3", "a nn_G3", "n t_G3", "た_G3", "a mm_G3", "n p_G3", "ぱ_G3", "a -_G3", })]
        public void ContextualNasalTest(string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest("", lyrics, aliases, "", "C3", "");
        }

        [Fact]
        public void ColorTest() {
            RunPhonemizeTest("", new NoteParams[] {
                new NoteParams { lyric = "ら", hint = "", tone = "C4", phonemes = new PhonemeParams[] { 
                    new PhonemeParams { alt = 0, color = "", shift = 0},
                    new PhonemeParams { alt = 0, color = "Soft", shift = 0},
                } }
            }, new string[] { "- ら_G3", "a -_S" });
        }

        [Fact]
        public void PitchTest() {
            RunPhonemizeTest("", new NoteParams[] {
                new NoteParams { lyric = "あ", hint = "", tone = "C4", phonemes = new PhonemeParams[] {
                    new PhonemeParams { alt = 0, color = "", shift = 0},
                    new PhonemeParams { alt = 0, color = "", shift = 12},
                } },
                new NoteParams { lyric = "か", hint = "", tone = "C4", phonemes = new PhonemeParams[] {
                    new PhonemeParams { alt = 0, color = "", shift = 0},
                    new PhonemeParams { alt = 0, color = "", shift = 0},
                } }
            }, new string[] { "- あ_G3", "a k_Bb4", "か_G3", "a -_G3" });
        }
    }
}
