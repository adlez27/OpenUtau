using OpenUtau.Classic;
using OpenUtau.Core;
using Xunit;
using Xunit.Abstractions;
using static OpenUtau.Api.Phonemizer;

namespace AbsideePhonemizer.Test {
    public class AbcUtilTest {
        ClassicSinger LoadVoicebank() {
            var voicebank = AbsideePhonemizerUtil.GetVoicebank("");
            VoicebankLoader.LoadVoicebank(voicebank);
            var singer = new ClassicSinger(voicebank);
            singer.EnsureLoaded();
            return singer;
        }

        [Theory]
        [InlineData("a", "", "a_G3")]
        [InlineData("a", "Soft", "a_S")]
        [InlineData("a", "asdf", "a_G3")]
        public void ColorSuffixTest(string alias,  string color, string result) {
            Assert.Equal(result, AbsideePhonemizerUtil.AssignSuffix(LoadVoicebank(), alias, 60, color));
        }

        [Theory]
        [InlineData("a", "C3", "a_G3")]
        [InlineData("a", "A4", "a_Bb4")]
        [InlineData("a", "G5", "a_Eb5")]
        public void PitchSuffixTest(string alias, string tone, string result) {
            Assert.Equal(result, AbsideePhonemizerUtil.AssignSuffix(LoadVoicebank(), alias, MusicMath.NameToTone(tone), ""));
        }

        [Theory]
        [InlineData("a ki", 0, false)] // diff alt
        [InlineData("a ki", 1, false)] // diff color
        [InlineData("a ki", 2, false)] // diff pitch
        [InlineData("a ki", 3, false)] // diff shift
        [InlineData("a ka", 4, false)] // no oto
        [InlineData("a ki", 4, true)] // all same
        [InlineData("a ki", 5, true)] // shift within range
        [InlineData("a き", 0, false)] // diff alt
        [InlineData("a き", 1, false)] // diff color
        [InlineData("a き", 2, false)] // diff pitch
        [InlineData("a き", 3, false)] // diff shift
        [InlineData("a か", 4, false)] // no oto
        [InlineData("a き", 4, true)] // all same
        [InlineData("a き", 5, true)] // shift within range
        public void CanVCVTest(string alias, int caseNum, bool result) {
            Note left = new Note { tone = 60, phonemeAttributes = new PhonemeAttributes[] { new PhonemeAttributes { index = 0 } } };
            Note[] right = new Note[] {
                new Note { tone = 60, phonemeAttributes = new PhonemeAttributes[] { new PhonemeAttributes { index = 0, alternate = 1 } } },
                new Note { tone = 60, phonemeAttributes = new PhonemeAttributes[] { new PhonemeAttributes { index = 0, voiceColor = "Soft" } } },
                new Note { tone = 72, phonemeAttributes = new PhonemeAttributes[] { new PhonemeAttributes { index = 0 } } },
                new Note { tone = 60, phonemeAttributes = new PhonemeAttributes[] { new PhonemeAttributes { index = 0, toneShift = 12 } } },
                new Note { tone = 60, phonemeAttributes = new PhonemeAttributes[] { new PhonemeAttributes { index = 0 } } },
                new Note { tone = 60, phonemeAttributes = new PhonemeAttributes[] { new PhonemeAttributes { index = 0, toneShift = -1 } } }
            };

            Assert.Equal(result, AbsideePhonemizerUtil.CanVCV(alias, left, right[caseNum], LoadVoicebank()));
        }
    }
}
