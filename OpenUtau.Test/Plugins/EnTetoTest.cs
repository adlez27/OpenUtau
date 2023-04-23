﻿using OpenUtau.Api;
using OpenUtau.Plugins;
using Xunit;
using Xunit.Abstractions;

namespace TetoEnPhonemizer {
    public class EnTetoTest : PhonemizerTestBase {
        public EnTetoTest(ITestOutputHelper output) : base(output) { }
        protected override Phonemizer CreatePhonemizer() {
            return new TetoEnglishPhonemizer();
        }

        [Theory]
        [InlineData("teto",
            new string[] { "the" },
            new string[] { "C4" },
            new string[] { "" },
            new string[] { "() D V", "V -"})]
        public void VowelEndTest(string singerName, string[] lyrics, string[] tones, string[] colors, string[] aliases) {
            RunPhonemizeTest(singerName, lyrics, tones, colors, aliases);
        }

        [Theory]
        [InlineData("teto",
            new string[] { "this" },
            new string[] { "C4" },
            new string[] { "" },
            new string[] { "() D I", "I s-" })]
        public void SingleConsonantEndTest(string singerName, string[] lyrics, string[] tones, string[] colors, string[] aliases) {
            RunPhonemizeTest(singerName, lyrics, tones, colors, aliases);
        }
    }
}
