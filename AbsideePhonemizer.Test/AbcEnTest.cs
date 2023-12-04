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
        public void SyllablePreviousRhoticTest(string lyric, string[] aliases) {
            SameAltsTonesColorsTest(new string[] { "her", lyric }, aliases);
        }
    }
}
