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

        [Theory]
        // Rewrite this later
        [InlineData("was",
            new string[] { "() w a", "a z", "z -" })]
        [InlineData("at",
            new string[] { "()  e", "e t", "t -" })]
        [InlineData("a",
            new string[] { "()  @", "@ -" })]
        [InlineData("aw",
            new string[] { "()  o", "o -" })]
        [InlineData("eh",
            new string[] { "()  e", "e -" })]
        [InlineData("er",
            new string[] { "()  @", "@ -" })]
        [InlineData("f",
            new string[] { "()  e", "e ff", "ff -" })]
        [InlineData("he",
            new string[] { "() h i", "i -" })]
        [InlineData("it",
            new string[] { "()  e", "e t", "t -" })]
        [InlineData("ee",
            new string[] { "()  i", "i -" })]
        [InlineData("j",
            new string[] { "() j e", "e i", "i -" })]
        [InlineData("r",
            new string[] { "()  a", "a rr", "rr -" })]
        [InlineData("would",
            new string[] { "() w @", "@ d", "d -" })]
        [InlineData("uw",
            new string[] { "()  u", "u -" })]
        [InlineData("eye",
            new string[] { "()  a", "a i", "i -" })]
        [InlineData("ey",
            new string[] { "()  e", "e i", "i -" })]
        [InlineData("oy",
            new string[] { "()  o", "o i", "i -" })]
        [InlineData("oh",
            new string[] { "()  o", "o u", "u -" })]
        [InlineData("ouch",
            new string[] { "()  a", "a u", "u ch", "ch -" })]
        public void RemapPhonemesTest(string lyric, string[] aliases) {
            SameAltsTonesColorsTest(new string[] { lyric }, aliases);
        }

        [Theory]
        // Rewrite beginning syllables later
        [InlineData("saw",
            new string[] { "() s o", "o -" })]
        [InlineData("hi",
            new string[] { "() h a", "a i", "i -" })]
        [InlineData("it",
            new string[] { "()  e", "e t", "t -" })]
        [InlineData("hide",
            new string[] { "() h a", "a i", "i d", "d -" })]
        [InlineData("rust",
            new string[] { "() rr @", "@ s", "t -" })]
        [InlineData("desks",
            new string[] { "() d e", "e s", "k", "s -" })]
        [InlineData("heist",
            new string[] { "() h a", "a i", "i s", "t -" })]
        [InlineData("ink",
            new string[] { "()  e", "e ng", "n k", "k -" })]
        [InlineData("int",
            new string[] { "()  e", "e n", "n t", "t -" })]
        [InlineData("imp",
            new string[] { "()  e", "e m", "n p", "p -" })]
        [InlineData("unt",
            new string[] { "()  @", "@ n", "t -" })]
        public void EndingTest(string lyric, string[] aliases) {
            SameAltsTonesColorsTest(new string[] { lyric }, aliases);
        }
    }
}
