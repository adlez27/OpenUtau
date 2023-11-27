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
        [InlineData("want",
            new string[] { "() w a", "(a) n t" })]
        [InlineData("at",
            new string[] { "()  e", "(e) t" })]
        [InlineData("a",
            new string[] { "()  @", "(@) " })]
        [InlineData("aw",
            new string[] { "()  o", "(o) " })]
        [InlineData("eh",
            new string[] { "()  e", "(e) " })]
        [InlineData("er",
            new string[] { "()  @", "(@) " })]
        [InlineData("f",
            new string[] { "()  e", "(e) ff" })]
        [InlineData("he",
            new string[] { "() h i", "(i) " })]
        [InlineData("it",
            new string[] { "()  e", "(e) t" })]
        [InlineData("ee",
            new string[] { "()  i", "(i) " })]
        [InlineData("j",
            new string[] { "() j e", "(e) y" })]
        [InlineData("would",
            new string[] { "() w @", "(@) d" })]
        [InlineData("uw",
            new string[] { "()  u", "(u) " })]
        [InlineData("eye",
            new string[] { "()  a", "(a) y" })]
        [InlineData("ey",
            new string[] { "()  e", "(e) y" })]
        [InlineData("oy",
            new string[] { "()  o", "(o) y" })]
        [InlineData("oh",
            new string[] { "()  o", "(o) w" })]
        [InlineData("ouch",
            new string[] { "()  a", "(a) w ch" })]
        public void RemapPhonemesTest(string lyric, string[] aliases) {
            SameAltsTonesColorsTest("", new string[] { lyric }, aliases, "", "C4", "");
        }
    }
}
