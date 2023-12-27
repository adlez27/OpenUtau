using OpenUtau.Api;
using OpenUtau.Classic;
using OpenUtau.Plugins;
using Xunit;
using Xunit.Abstractions;

namespace AbsideePhonemizer.Test {
    public class AbcEnTest : PhonemizerTestBase {
        public AbcEnTest(ITestOutputHelper output) : base(output) { }

        protected override Phonemizer CreatePhonemizer() {
            return new AbsideeEnglishPhonemizer();
        }

        protected override Voicebank GetVoicebank(string _) {
            return AbsideePhonemizerUtil.GetVoicebank();
        }

        private void SameAltsTonesColorsTest(string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest("", lyrics, aliases.Select(a => $"{a}_G3").ToArray(), "", "C4", "");
        }

        private void SameAltsTonesColorsTest(string lyric, string[] aliases) {
            SameAltsTonesColorsTest("", new string[] {lyric}, aliases.Select(a => $"{a}_G3").ToArray(), "", "C4", "");
        }

        [Theory]
        [InlineData("was",
            new string[] { "- wa", "a z", "z -" })]
        [InlineData("at",
            new string[] { "- e", "e t", "t -" })]
        [InlineData("a",
            new string[] { "- @", "@ -" })]
        [InlineData("aw",
            new string[] { "- o", "o -" })]
        [InlineData("eh",
            new string[] { "- e", "e -" })]
        [InlineData("f",
            new string[] { "- e", "e ff", "ff -" })]
        [InlineData("he",
            new string[] { "- hi", "i -" })]
        [InlineData("it",
            new string[] { "- e", "e t", "t -" })]
        [InlineData("ee",
            new string[] { "- i", "i -" })]
        [InlineData("j",
            new string[] { "- je", "e i", "i -" })]
        [InlineData("r",
            new string[] { "- a", "a rr", "rr -" })]
        [InlineData("would",
            new string[] { "- w@", "@ d", "d -" })]
        [InlineData("uw",
            new string[] { "- u", "u -" })]
        [InlineData("eye",
            new string[] { "- a", "a i", "i -" })]
        [InlineData("ey",
            new string[] { "- e", "e i", "i -" })]
        [InlineData("oy",
            new string[] { "- o", "o i", "i -" })]
        [InlineData("oh",
            new string[] { "- o", "o u", "u -" })]
        [InlineData("ouch",
            new string[] { "- a", "a u", "u ch", "ch -" })]
        public void RemapPhonemesTest(string lyric, string[] aliases) {
            SameAltsTonesColorsTest(lyric, aliases);
        }

        [Theory]
        [InlineData("saw",
            new string[] { "- so", "o -" })]
        [InlineData("hi",
            new string[] { "- ha", "a i", "i -" })]
        [InlineData("it",
            new string[] { "- e", "e t", "t -" })]
        [InlineData("hide",
            new string[] { "- ha", "a i", "i d", "d -" })]
        [InlineData("rust",
            new string[] { "- rr@", "@ s", "t -" })]
        [InlineData("desks",
            new string[] { "- de", "e s", "k", "s -" })]
        [InlineData("heist",
            new string[] { "- ha", "a i", "i s", "t -" })]
        public void EndingTest(string lyric, string[] aliases) {
            SameAltsTonesColorsTest(lyric, aliases);
        }

        [Theory]
        [InlineData("ink",
            new string[] { "- e", "e ng", "n k", "k -" })]
        [InlineData("int",
            new string[] { "- e", "e n", "n t", "t -" })]
        [InlineData("imp",
            new string[] { "- e", "e m", "n p", "p -" })]
        [InlineData("things",
            new string[] { "- the", "e ng", "z -" })]
        [InlineData("its",
            new string[] { "- e", "e ts", "ts -" })]
        [InlineData("ads",
            new string[] { "- e", "e dz", "dz -" })]
        [InlineData("her",
            new string[] { "- h@", "@ rr", "rr -" })]
        [InlineData("hurt",
            new string[] { "- h@", "@ t", "t -" })]
        public void EndingSpecialTest(string lyric, string[] aliases) {
            SameAltsTonesColorsTest(lyric, aliases);
        }

        [Theory]
        [InlineData("at",
            new string[] { "- e", "e t", "t -" })]
        [InlineData("hat",
            new string[] { "- he", "e t", "t -" })]
        [InlineData("tso",
            new string[] { "- tso", "o u", "u -" })]
        [InlineData("scuff",
            new string[] { "s", "k@", "@ ff", "ff -" })]
        [InlineData("scruff",
            new string[] { "s", "k@", "rr@", "@ ff", "ff -" })]
        [InlineData("crew",
            new string[] { "- k@", "rru", "u -" })]
        public void SyllableNoPreviousTest(string lyric, string[] aliases) {
            SameAltsTonesColorsTest(lyric, aliases);
        }

        [Theory]
        [InlineData("at",
            new string[] { "- @", "@ e", "e t", "t -" })]
        [InlineData("hat",
            new string[] { "- @", "@ h", "he", "e t", "t -" })]
        [InlineData("tso",
            new string[] { "- @", "@ ts", "tso", "o u", "u -" })]
        [InlineData("scuff",
            new string[] { "- @", "@ s", "k@", "@ ff", "ff -" })]
        [InlineData("scruff",
            new string[] { "- @", "@ s", "k@", "rr@", "@ ff", "ff -" })]
        [InlineData("crew",
            new string[] { "- @", "@ k", "k@", "rru", "u -" })]
        public void SyllablePreviousVowelTest(string lyric, string[] aliases) {
            SameAltsTonesColorsTest(new string[] { "a", lyric }, aliases);
        }

        [Theory]
        [InlineData("at",
            new string[] { "- a", "a y", "ye", "e t", "t -" })]
        [InlineData("hat",
            new string[] { "- a", "a i", "i h", "he", "e t", "t -" })]
        [InlineData("tso",
            new string[] { "- a", "a i", "i ts", "tso", "o u", "u -" })]
        [InlineData("scuff",
            new string[] { "- a", "a i", "i s", "k@", "@ ff", "ff -" })]
        [InlineData("scruff",
            new string[] { "- a", "a i", "i s", "k@", "rr@", "@ ff", "ff -" })]
        [InlineData("crew",
            new string[] { "- a", "a i", "i k", "k@", "rru", "u -" })]
        public void SyllablePreviousDiphthongTest(string lyric, string[] aliases) {
            SameAltsTonesColorsTest(new string[] { "I", lyric }, aliases);
        }

        [Theory]
        [InlineData("at",
            new string[] { "- h@", "@ rr", "rre", "e t", "t -" })]
        [InlineData("hat",
            new string[] { "- h@", "@ h", "he", "e t", "t -" })]
        [InlineData("tso",
            new string[] { "- h@", "@ ts", "tso", "o u", "u -" })]
        [InlineData("scuff",
            new string[] { "- h@", "@ s", "k@", "@ ff", "ff -" })]
        [InlineData("scruff",
            new string[] { "- h@", "@ s", "k@", "rr@", "@ ff", "ff -" })]
        [InlineData("crew",
            new string[] { "- h@", "@ k", "k@", "rru", "u -" })]
        public void SyllablePreviousRhoticTest(string lyric, string[] aliases) {
            SameAltsTonesColorsTest(new string[] { "her", lyric }, aliases);
        }

        [Theory]
        [InlineData("at",
            new string[] { "- e", "e g", "ge", "e t", "t -" })]
        [InlineData("hat",
            new string[] { "- e", "e g", "he", "e t", "t -" })]
        [InlineData("tso",
            new string[] { "- e", "e g", "tso", "o u", "u -" })]
        [InlineData("scuff",
            new string[] { "- e", "e g", "s", "k@", "@ ff", "ff -" })]
        [InlineData("scruff",
            new string[] { "- e", "e g", "s", "k@", "rr@", "@ ff", "ff -" })]
        [InlineData("crew",
            new string[] { "- e", "e g", "k@", "rru", "u -" })]
        public void SyllablePreviousConsonantTest(string lyric, string[] aliases) {
            SameAltsTonesColorsTest(new string[] { "egg", lyric }, aliases);
        }

        [Theory]
        [InlineData("at",
            new string[] { "- e", "e g", "ze", "e t", "t -" })]
        [InlineData("hat",
            new string[] { "- e", "e g", "z", "he", "e t", "t -" })]
        [InlineData("tso",
            new string[] { "- e", "e g", "z", "tso", "o u", "u -" })]
        [InlineData("scuff",
            new string[] { "- e", "e g", "z", "s", "k@", "@ ff", "ff -" })]
        [InlineData("scruff",
            new string[] { "- e", "e g", "z", "s", "k@", "rr@", "@ ff", "ff -" })]
        [InlineData("crew",
            new string[] { "- e", "e g", "z", "k@", "rru", "u -" })]
        public void SyllablePreviousClusterTest(string lyric, string[] aliases) {
            SameAltsTonesColorsTest(new string[] { "eggs", lyric }, aliases);
        }

        [Theory]
        [InlineData(new string[] { "crew" },
            new string[] { "- k@", "rru", "u -" })]
        [InlineData(new string[] { "claw" },
            new string[] { "- k@", "lo", "o -" })]
        [InlineData(new string[] { "cute" },
            new string[] { "- ki", "yu", "u t", "t -" })]
        [InlineData(new string[] { "queen" },
            new string[] { "- ku", "wi", "i n", "n -" })]
        [InlineData(new string[] { "a", "crew" },
            new string[] { "- @", "@ k", "k@", "rru", "u -" })]
        [InlineData(new string[] { "a", "claw" },
            new string[] { "- @", "@ k", "k@", "lo", "o -" })]
        [InlineData(new string[] { "a", "cute" },
            new string[] { "- @", "@ k", "ki", "yu", "u t", "t -" })]
        [InlineData(new string[] { "a", "queen" },
            new string[] { "- @", "@ k", "ku", "wi", "i n", "n -" })]
        [InlineData(new string[] { "I", "you" },
            new string[] { "- a", "a i", "i y", "yu", "u -" })]
        [InlineData(new string[] { "all", "you" },
            new string[] { "- o", "o l", "yu", "u -" })]
        public void SyllableLiquidGlideClusterTest(string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest(lyrics, aliases);
        }

        [Theory]
        [InlineData(new string[] { "true" },
            new string[] { "- ch@", "rru", "u -" })]
        [InlineData(new string[] { "drew" },
            new string[] { "- j@", "rru", "u -" })]
        [InlineData(new string[] { "string" },
            new string[] { "s", "ch@", "rre", "e ng", "ng -" })]
        [InlineData(new string[] { "a", "true" },
            new string[] { "- @", "@ ch", "ch@", "rru", "u -" })]
        [InlineData(new string[] { "a", "drew" },
            new string[] { "- @", "@ j", "j@", "rru", "u -" })]
        [InlineData(new string[] { "a", "string" },
            new string[] { "- @", "@ s", "ch@", "rre", "e ng", "ng -" })]
        [InlineData(new string[] { "I", "true" },
            new string[] { "- a", "a i", "i ch", "ch@", "rru", "u -" })]
        [InlineData(new string[] { "I", "drew" },
            new string[] { "- a", "a i", "i j", "j@", "rru", "u -" })]
        [InlineData(new string[] { "I", "string" },
            new string[] { "- a", "a i", "i s", "ch@", "rre", "e ng", "ng -" })]
        [InlineData(new string[] { "her", "true" },
            new string[] { "- h@", "@ ch", "ch@", "rru", "u -" })]
        [InlineData(new string[] { "her", "drew" },
            new string[] { "- h@", "@ j", "j@", "rru", "u -" })]
        [InlineData(new string[] { "her", "string" },
            new string[] { "- h@", "@ s", "ch@", "rre", "e ng", "ng -" })]
        [InlineData(new string[] { "egg", "true" },
            new string[] { "- e", "e g", "ch@", "rru", "u -" })]
        [InlineData(new string[] { "egg", "drew" },
            new string[] { "- e", "e g", "j@", "rru", "u -" })]
        [InlineData(new string[] { "egg", "string" },
            new string[] { "- e", "e g", "s", "ch@", "rre", "e ng", "ng -" })]
        [InlineData(new string[] { "eggs", "true" },
            new string[] { "- e", "e g", "z", "ch@", "rru", "u -" })]
        [InlineData(new string[] { "eggs", "drew" },
            new string[] { "- e", "e g", "z", "j@", "rru", "u -" })]
        [InlineData(new string[] { "eggs", "string" },
            new string[] { "- e", "e g", "z", "s", "ch@", "rre", "e ng", "ng -" })]
        public void SyllableAffricateClusterTest(string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest(lyrics, aliases);
        }

        [Theory]
        [InlineData(
            new string[] { "la", "-" },
            new string[] { "- la", "a -" })]
        [InlineData(
            new string[] { "I", "-" },
            new string[] { "- a", "a i", "i -" })]
        [InlineData(
            new string[] { "her", "-" },
            new string[] { "- h@", "@ rr", "rr -" })]
        [InlineData(
            new string[] { "egg", "-" },
            new string[] { "- e", "e g", "g -" })]
        [InlineData(
            new string[] { "eggs", "-" },
            new string[] { "- e", "e g", "z -" })]
        [InlineData(
            new string[] { "la", "hh" },
            new string[] { "- la", "a hh" })]
        [InlineData(
            new string[] { "I", "hh" },
            new string[] { "- a", "a i", "i hh" })]
        [InlineData(
            new string[] { "her", "hh" },
            new string[] { "- h@", "@ rr", "rr@", "@ hh" })]
        [InlineData(
            new string[] { "egg", "hh" },
            new string[] { "- e", "e g", "g@", "@ hh" })]
        [InlineData(
            new string[] { "eggs", "hh" },
            new string[] { "- e", "e g", "z@", "@ hh" })]
        [InlineData(
            new string[] { "a", "-", "a" },
            new string[] { "- @", "@ -", "- @", "@ -" })]
        [InlineData(
            new string[] { "a", "hh", "a" },
            new string[] { "- @", "@ hh", "- @", "@ -" })]
        public void SeparatedTailTest(string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest(lyrics, aliases);
        }

        [Fact]
        public void ColorTest() {
            RunPhonemizeTest("", new NoteParams[] {
                new NoteParams { lyric = "la", hint = "", tone = "C4", phonemes = new PhonemeParams[] {
                    new PhonemeParams { alt = 0, color = "", shift = 0},
                    new PhonemeParams { alt = 0, color = "Soft", shift = 0},
                } }
            }, new string[] { "- la_G3", "a -_S" });
        }

        [Theory]
        [InlineData("A4", 0)]
        [InlineData("A3", 12)]
        public void PitchTest(string tone, int shift) {
            RunPhonemizeTest("", new NoteParams[] {
                new NoteParams { lyric = "a", hint = "", tone = "A3", phonemes = new PhonemeParams[] {
                    new PhonemeParams { alt = 0, color = "", shift = 0}
                } },
                new NoteParams { lyric = "see", hint = "", tone = tone, phonemes = new PhonemeParams[] {
                    new PhonemeParams { alt = 0, color = "", shift = 0},
                    new PhonemeParams { alt = 0, color = "", shift = shift},
                    new PhonemeParams { alt = 0, color = "", shift = shift},
                } }
            }, new string[] { "- @_G3", "@ s_G3", "si_Bb4", "i -_Bb4" });
        }

        [Fact]
        public void PrioritizeVCVTest() {
            SameAltsTonesColorsTest(new string[] { "a", "dark" }, 
                new string[] { "- @", "@ da", "a rr", "k -" });
        }

        [Theory]
        [InlineData("F4", 0,
            new string[] { "- la_Bb4", "a -_Bb4" })]
        [InlineData("F4", -1,
            new string[] { "- la_Bb4", "a -_Bb4" })]
        [InlineData("F4", 1,
            new string[] { "- la_Bb4", "a -_Bb4" })]
        [InlineData("F4", -12,
            new string[] { "- la_G3", "a -_G3" })]
        [InlineData("F4", 12,
            new string[] { "- la_Bb4", "a_Eb5", "a -_Bb4" })]
        [InlineData("F5", 0,
            new string[] { "- la_Bb4", "a_Eb5", "a -_Bb4" })]
        [InlineData("F5", -1,
            new string[] { "- la_Bb4", "a_Eb5", "a -_Bb4" })]
        [InlineData("F5", 1,
            new string[] { "- la_Bb4", "a_Eb5", "a -_Bb4" })]
        [InlineData("F5", -12,
            new string[] { "- la_Bb4", "a -_Bb4" })]
        [InlineData("F5", 12,
            new string[] { "- la_Bb4", "a_Eb5", "a -_Bb4" })]
        public void CutHighNotesTest(string tone, int shift, string[] aliases) {
            RunPhonemizeTest("", new NoteParams[] {
                new NoteParams { lyric = "la", hint = "", tone = tone, phonemes = SamePhonemeParams(3, shift, 0, "") },
            }, aliases);
        }

        [Fact]
        public void CutHighMultiSyllableTest() {
            RunPhonemizeTest("", new NoteParams[] {
                new NoteParams {lyric = "connect", hint = "", tone = "C5", phonemes = SamePhonemeParams(1, 0, 0, "")},
                new NoteParams {lyric = "+", hint = "", tone = "D5", phonemes = SamePhonemeParams(10, 0, 0, "")},
                new NoteParams {lyric = "all", hint = "", tone = "D5", phonemes = SamePhonemeParams(10, 0, 0, "")},
            }, new string[] { "- k@_Bb4", "@ n_Bb4", "ne_Bb4", "e_Eb5", "e k_Bb4", "to_Bb4", "o_Eb5", "o l_Bb4", "l -_Bb4" });
        }

        [Theory]
        [InlineData(
            new string[] { "a", "see" },
            new string[] { "- @_Eb5", "@ s_Bb4", "si_Bb4", "i_Eb5", "i -_Bb4" })]
        public void OnlySplitHighVowelsTest(string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest("", lyrics, aliases, "", "F5", "");
        }

        [Theory]
        [InlineData("its", "",
            new string[] { "- e_G3", "e ts_G3", "ts -_G3" })]
        [InlineData("its", "e t s",
            new string[] { "- e_G3", "e t_G3", "s -_G3" })]
        [InlineData("ads", "",
            new string[] { "- e_G3", "e dz_G3", "dz -_G3" })]
        [InlineData("ads", "e d z",
            new string[] { "- e_G3", "e d_G3", "z -_G3" })]
        [InlineData("true", "",
            new string[] { "- ch@_G3", "rru_G3", "u -_G3" })]
        [InlineData("true", "t rr u",
            new string[] { "- t@_G3", "rru_G3", "u -_G3" })]
        [InlineData("drew", "",
            new string[] { "- j@_G3", "rru_G3", "u -_G3" })]
        [InlineData("drew", "d rr u",
            new string[] { "- d@_G3", "rru_G3", "u -_G3" })]
        public void DontMergeHintsTest(string lyric, string hint, string[] aliases) {
            RunPhonemizeTest("", new NoteParams[] {
                new NoteParams { lyric = lyric, hint = hint, tone = "C4", phonemes = SamePhonemeParams(10, 0, 0, "")}
            }, aliases);
        }

        [Fact]
        public void AndTest() {
            SameAltsTonesColorsTest("and", new string[] { "- e", "e n", "n d", "d -" });
        }

        [Theory]
        [InlineData(new string[] { "etched" },
            new string[] { "- e", "e ch", "ch", "t -" })]
        [InlineData(new string[] { "etched", "a" },
            new string[] { "- e", "e ch", "ch", "t@", "@ -" })]
        [InlineData(new string[] { "edged" },
            new string[] { "- e", "e j", "j", "d -" })]
        [InlineData(new string[] { "edged", "a" },
            new string[] { "- e", "e j", "j", "d@", "@ -" })]
        [InlineData(new string[] { "batch", "check" },
            new string[] { "- be", "e ch", "ch", "che", "e k", "k -" })]
        [InlineData(new string[] { "addsd" },
            new string[] { "- e", "e dz", "dz", "d -" })]
        [InlineData(new string[] { "adds", "to" },
            new string[] { "- e", "e dz", "dz", "tu", "u -" })]
        [InlineData(new string[] { "itst" },
            new string[] { "- e", "e ts", "ts", "t -" })]
        [InlineData(new string[] { "its", "to" },
            new string[] { "- e", "e ts", "ts", "tu", "u -" })]
        public void AffricateClusterTest(string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest(lyrics, aliases);
        }

        [Theory]

        [InlineData(new string[] { "ink", "a" },
            new string[] { "- e", "e ng", "n k", "k@", "@ -" })]
        [InlineData(new string[] { "int", "a" },
            new string[] { "- e", "e n", "n t", "t@", "@ -" })]
        [InlineData(new string[] { "imp", "a" },
            new string[] { "- e", "e m", "n p", "p@", "@ -" })]
        [InlineData(new string[] { "am", "to" },
            new string[] { "- e", "e m", "tu", "u -" })]
        [InlineData(new string[] { "am", "my" },
            new string[] { "- e", "e m", "ma", "a i", "i -" })]
        [InlineData(new string[] { "int", "star" },
            new string[] { "- e", "e n", "n t", "s", "ta", "a rr", "rr -" })]
        public void SyllableNasalClusterTest(string[] lyrics, string[] aliases) {
            SameAltsTonesColorsTest(lyrics, aliases);
        }
    }
}
