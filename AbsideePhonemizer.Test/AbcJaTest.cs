using System.Reflection;
using System.Text;
using OpenUtau.Api;
using OpenUtau.Classic;
using OpenUtau.Plugins;
using SharpCompress;
using Xunit;
using Xunit.Abstractions;

namespace AbsideePhonemizer.Test {
    public class AbcJaTest : PhonemizerTestBase {
        public AbcJaTest(ITestOutputHelper output) : base(output) { }

        protected override Phonemizer CreatePhonemizer() {
            return new AbsideeJapanesePhonemizer();
        }

        protected override Voicebank GetVoicebank(string _) {
            return AbsideePhonemizerUtil.GetVoicebank();
        }

        private void SameAltsTonesColorsTest(string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest("", lyrics, aliases.Select(a => $"{a}_G3").ToArray(), "", "C4", "");
        }

        [Theory]
        [InlineData(
            new string[] { "ら", "ら" },
            new string[] { "- ら", "a r", "ら", "a -" })]
        [InlineData(
            new string[] { "ら", "い" },
            new string[] { "- ら", "a い", "i -" })]
        public void BasicPhonemizeTest(string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest(lyrics, aliases);
        }

        [Theory]
        [InlineData(
            new string[] { "ら", "-" },
            new string[] { "- ら", "a -" })]
        [InlineData(
            new string[] { "ら", "hh" },
            new string[] { "- ら", "a hh" })]
        public void SeparatedTailTest(string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest(lyrics, aliases);
        }

        [Theory]
        [InlineData(
            new string[] { "ん", "ゆ", "ん", "か", "ん", "た", "ん", "ぱ" },
            new string[] { "- ん", "n y", "ゆ", "u nng", "n k", "か", "a nn", "n t", "た", "a mm", "n p", "ぱ", "a -", })]
        public void ContextualNasalTest(string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest(lyrics, aliases);
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

        [Fact]
        public void PrioritizeVCVTest() {
            SameAltsTonesColorsTest(new string[] { "あ", "き"}, new string[] { "- あ", "a き", "i -" });
        }

        [Theory]
        [InlineData("", "F4", 0,
            new string[] { "- か_Bb4", "a -_Bb4" })]
        [InlineData("", "F4", -1,
            new string[] { "- か_Bb4", "a -_Bb4" })]
        [InlineData("", "F4", 1,
            new string[] {"- か_Bb4", "a -_Bb4" })]
        [InlineData("", "F4", -12,
            new string[] {"- か_G3", "a -_G3" })]
        [InlineData("", "F4", 12,
            new string[] { "- か_Bb4", "a_Eb5", "a -_Bb4" })]
        [InlineData("", "F5", 0,
            new string[] { "- か_Bb4", "a_Eb5", "a -_Bb4" })]
        [InlineData("", "F5", -1,
            new string[] { "- か_Bb4", "a_Eb5", "a -_Bb4" })]
        [InlineData("", "F5", 1,
            new string[] { "- か_Bb4", "a_Eb5", "a -_Bb4" })]
        [InlineData("", "F5", -12,
            new string[] { "- か_Bb4", "a -_Bb4" })]
        [InlineData("", "F5", 12,
            new string[] { "- か_Bb4", "a_Eb5", "a -_Bb4" })]
        [InlineData("Soft", "F4", 0,
            new string[] { "- か_S", "a -_S" })]
        [InlineData("Soft", "F4", -1,
            new string[] { "- か_S", "a -_S" })]
        [InlineData("Soft", "F4", 1,
            new string[] { "- か_S", "a -_S" })]
        [InlineData("Soft", "F4", -12,
            new string[] { "- か_S", "a -_S" })]
        [InlineData("Soft", "F4", 12,
            new string[] { "- か_S", "a -_S" })]
        [InlineData("Soft", "F5", 0,
            new string[] { "- か_S", "a -_S" })]
        [InlineData("Soft", "F5", -1,
            new string[] { "- か_S", "a -_S" })]
        [InlineData("Soft", "F5", 1,
            new string[] { "- か_S", "a -_S" })]
        [InlineData("Soft", "F5", -12,
            new string[] { "- か_S", "a -_S" })]
        [InlineData("Soft", "F5", 12,
            new string[] { "- か_S", "a -_S" })]
        public void CutHighNotesTest(string color, string tone, int shift, string[] aliases) {
            RunPhonemizeTest("", new NoteParams[] {
                new NoteParams { lyric = "か", hint = "", tone = tone, phonemes = SamePhonemeParams(3, shift, 0, color) },
            }, aliases);
        }

        [Theory]
        [InlineData(
            new string[] { "ん", "ん" },
            new string[] { "- ん_Bb4", "N_Eb5", "n ん_Bb4", "N_Eb5", "n -_Bb4" })]
        [InlineData(
            new string[] { "あ", "ん" },
            new string[] { "- あ_Eb5", "a ん_Bb4", "N_Eb5", "n -_Bb4" })]
        [InlineData(
            new string[] { "ん", "あ" },
            new string[] { "- ん_Bb4", "N_Eb5", "n あ_Bb4", "a_Eb5", "a -_Bb4" })]
        [InlineData(
            new string[] { "い", "ん" },
            new string[] { "- い_Eb5", "i ん_Bb4", "N_Eb5", "n -_Bb4" })]
        [InlineData(
            new string[] { "ん", "い" },
            new string[] { "- ん_Bb4", "N_Eb5", "n い_Bb4", "i_Eb5", "i -_Bb4" })]
        [InlineData(
            new string[] { "う", "ん" },
            new string[] { "- う_Eb5", "u ん_Bb4", "N_Eb5", "n -_Bb4" })]
        [InlineData(
            new string[] { "ん", "う" },
            new string[] { "- ん_Bb4", "N_Eb5", "n う_Bb4", "u_Eb5", "u -_Bb4" })]
        [InlineData(
            new string[] { "ん", "hh" },
            new string[] { "- ん_Bb4", "N_Eb5", "n hh_Bb4" })]
        public void FallbackTest(string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest("", lyrics, aliases, "", "F5", "");
        }

        [Theory]
        [InlineData(
            new string[] { "あ", "さ" },
            new string[] { "- あ_Eb5", "a s_Bb4", "さ_Bb4", "a_Eb5", "a -_Bb4" })]
        public void OnlySplitHighVowelsTest(string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest("", lyrics, aliases, "", "F5", "");
        }
    }
}
