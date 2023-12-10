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
    }
}
