using System;
using System.Collections.Generic;
using System.Linq;
using OpenUtau.Api;
using OpenUtau.Core.Ustx;

namespace CyriljiPhonemizer {
    [Phonemizer("Japanese Cyrillic Phonemizer", "Нихонго", "Adlez27", language: "JA")]
    public class JapaneseCyrillicPhonemizer : Phonemizer {
        private USinger singer;
        public override void SetSinger(USinger singer) => this.singer = singer;

        public override Result Process(Note[] notes, Note? prev, Note? next, Note? prevNeighbour, Note? nextNeighbour, Note[] prevs) {
            string alias = notes[0].lyric;
            var attr0 = notes[0].phonemeAttributes?.FirstOrDefault(attr => attr.index == 0) ?? default;
            if (singer.TryGetMappedOto(notes[0].lyric, notes[0].tone + attr0.toneShift, attr0.voiceColor, out var oto)) {
                alias = oto.Alias;
            }
            return new Result {
                phonemes = new Phoneme[] {
                    new Phoneme {
                        phoneme = alias,
                    }
                }
            };
        }
    }
}
