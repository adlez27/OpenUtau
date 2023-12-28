using System;
using System.Collections.Generic;
using System.Linq;
using OpenUtau.Api;
using OpenUtau.Core.Ustx;

namespace AbsideePhonemizer {
    [Phonemizer("Absidee Japanese Phonemizer", "JA ABC", "Adlez27", "JA")]
    public class AbsideeJapanesePhonemizer : Phonemizer {
        public struct Symbol {
            public string Prefix;
            public string Suffix;
            public string Nasal;
        }
        private Dictionary<string, Symbol> symbols;

        public AbsideeJapanesePhonemizer() {
            util = new AbsideePhonemizerUtil();
            symbols = new Dictionary<string, Symbol> {
                ["あ"] = new Symbol { Prefix = "", Suffix = "a", Nasal = "N" },
                ["い"] = new Symbol { Prefix = "", Suffix = "i", Nasal = "N" },
                ["う"] = new Symbol { Prefix = "", Suffix = "u", Nasal = "N" },
                ["え"] = new Symbol { Prefix = "", Suffix = "e", Nasal = "N" },
                ["お"] = new Symbol { Prefix = "", Suffix = "o", Nasal = "N" },
                ["を"] = new Symbol { Prefix = "", Suffix = "o", Nasal = "N" },
                ["ん"] = new Symbol { Prefix = "", Suffix = "n", Nasal = "N" },
                ["んn"] = new Symbol { Prefix = "", Suffix = "n", Nasal = "nn" },
                ["んm"] = new Symbol { Prefix = "", Suffix = "n", Nasal = "mm" },
                ["んng"] = new Symbol { Prefix = "", Suffix = "n", Nasal = "nng" },
                ["'あ"] = new Symbol { Prefix = "'", Suffix = "a", Nasal = "N" },
                ["'い"] = new Symbol { Prefix = "'", Suffix = "i", Nasal = "N" },
                ["'う"] = new Symbol { Prefix = "'", Suffix = "u", Nasal = "N" },
                ["'え"] = new Symbol { Prefix = "'", Suffix = "e", Nasal = "N" },
                ["'お"] = new Symbol { Prefix = "'", Suffix = "o", Nasal = "N" },
                ["'を"] = new Symbol { Prefix = "'", Suffix = "o", Nasal = "N" },
                ["'ん"] = new Symbol { Prefix = "'", Suffix = "n", Nasal = "N" },
                ["'んn"] = new Symbol { Prefix = "'", Suffix = "n", Nasal = "nn" },
                ["'んm"] = new Symbol { Prefix = "'", Suffix = "n", Nasal = "mm" },
                ["'んng"] = new Symbol { Prefix = "'", Suffix = "n", Nasal = "nng" },
                ["か"] = new Symbol { Prefix = "k", Suffix = "a", Nasal = "nng" },
                ["く"] = new Symbol { Prefix = "k", Suffix = "u", Nasal = "nng" },
                ["け"] = new Symbol { Prefix = "k", Suffix = "e", Nasal = "nng" },
                ["こ"] = new Symbol { Prefix = "k", Suffix = "o", Nasal = "nng" },
                ["きゃ"] = new Symbol { Prefix = "ky", Suffix = "a", Nasal = "nng" },
                ["き"] = new Symbol { Prefix = "ky", Suffix = "i", Nasal = "nng" },
                ["きゅ"] = new Symbol { Prefix = "ky", Suffix = "u", Nasal = "nng" },
                ["きぇ"] = new Symbol { Prefix = "ky", Suffix = "e", Nasal = "nng" },
                ["きょ"] = new Symbol { Prefix = "ky", Suffix = "o", Nasal = "nng" },
                ["が"] = new Symbol { Prefix = "g", Suffix = "a", Nasal = "nng" },
                ["ぐ"] = new Symbol { Prefix = "g", Suffix = "u", Nasal = "nng" },
                ["げ"] = new Symbol { Prefix = "g", Suffix = "e", Nasal = "nng" },
                ["ご"] = new Symbol { Prefix = "g", Suffix = "o", Nasal = "nng" },
                ["ぎゃ"] = new Symbol { Prefix = "gy", Suffix = "a", Nasal = "nng" },
                ["ぎ"] = new Symbol { Prefix = "gy", Suffix = "i", Nasal = "nng" },
                ["ぎゅ"] = new Symbol { Prefix = "gy", Suffix = "u", Nasal = "nng" },
                ["ぎぇ"] = new Symbol { Prefix = "gy", Suffix = "e", Nasal = "nng" },
                ["ぎょ"] = new Symbol { Prefix = "gy", Suffix = "o", Nasal = "nng" },
                ["ガ"] = new Symbol { Prefix = "ng", Suffix = "a", Nasal = "nng" },
                ["ギ"] = new Symbol { Prefix = "ng", Suffix = "i", Nasal = "nng" },
                ["グ"] = new Symbol { Prefix = "ng", Suffix = "u", Nasal = "nng" },
                ["ゲ"] = new Symbol { Prefix = "ng", Suffix = "e", Nasal = "nng" },
                ["ゴ"] = new Symbol { Prefix = "ng", Suffix = "o", Nasal = "nng" },
                ["さ"] = new Symbol { Prefix = "s", Suffix = "a", Nasal = "N" },
                ["すぃ"] = new Symbol { Prefix = "s", Suffix = "i", Nasal = "N" },
                ["す"] = new Symbol { Prefix = "s", Suffix = "u", Nasal = "N" },
                ["せ"] = new Symbol { Prefix = "s", Suffix = "e", Nasal = "N" },
                ["そ"] = new Symbol { Prefix = "s", Suffix = "o", Nasal = "N" },
                ["しゃ"] = new Symbol { Prefix = "sh", Suffix = "a", Nasal = "N" },
                ["し"] = new Symbol { Prefix = "sh", Suffix = "i", Nasal = "N" },
                ["しゅ"] = new Symbol { Prefix = "sh", Suffix = "u", Nasal = "N" },
                ["しぇ"] = new Symbol { Prefix = "sh", Suffix = "e", Nasal = "N" },
                ["しょ"] = new Symbol { Prefix = "sh", Suffix = "o", Nasal = "N" },
                ["ざ"] = new Symbol { Prefix = "z", Suffix = "a", Nasal = "N" },
                ["ずぃ"] = new Symbol { Prefix = "z", Suffix = "i", Nasal = "N" },
                ["ず"] = new Symbol { Prefix = "z", Suffix = "u", Nasal = "N" },
                ["ぜ"] = new Symbol { Prefix = "z", Suffix = "e", Nasal = "N" },
                ["ぞ"] = new Symbol { Prefix = "z", Suffix = "o", Nasal = "N" },
                ["じゃ"] = new Symbol { Prefix = "j", Suffix = "a", Nasal = "N" },
                ["じ"] = new Symbol { Prefix = "j", Suffix = "i", Nasal = "N" },
                ["じゅ"] = new Symbol { Prefix = "j", Suffix = "u", Nasal = "N" },
                ["じぇ"] = new Symbol { Prefix = "j", Suffix = "e", Nasal = "N" },
                ["じょ"] = new Symbol { Prefix = "j", Suffix = "o", Nasal = "N" },
                ["た"] = new Symbol { Prefix = "t", Suffix = "a", Nasal = "nn" },
                ["てぃ"] = new Symbol { Prefix = "t", Suffix = "i", Nasal = "nn" },
                ["とぅ"] = new Symbol { Prefix = "t", Suffix = "u", Nasal = "nn" },
                ["て"] = new Symbol { Prefix = "t", Suffix = "e", Nasal = "nn" },
                ["と"] = new Symbol { Prefix = "t", Suffix = "o", Nasal = "nn" },
                ["ちゃ"] = new Symbol { Prefix = "ch", Suffix = "a", Nasal = "nn" },
                ["ち"] = new Symbol { Prefix = "ch", Suffix = "i", Nasal = "nn" },
                ["ちゅ"] = new Symbol { Prefix = "ch", Suffix = "u", Nasal = "nn" },
                ["ちぇ"] = new Symbol { Prefix = "ch", Suffix = "e", Nasal = "nn" },
                ["ちょ"] = new Symbol { Prefix = "ch", Suffix = "o", Nasal = "nn" },
                ["つぁ"] = new Symbol { Prefix = "ts", Suffix = "a", Nasal = "nn" },
                ["つぃ"] = new Symbol { Prefix = "ts", Suffix = "i", Nasal = "nn" },
                ["つ"] = new Symbol { Prefix = "ts", Suffix = "u", Nasal = "nn" },
                ["つぇ"] = new Symbol { Prefix = "ts", Suffix = "e", Nasal = "nn" },
                ["つぉ"] = new Symbol { Prefix = "ts", Suffix = "o", Nasal = "nn" },
                ["だ"] = new Symbol { Prefix = "d", Suffix = "a", Nasal = "nn" },
                ["でぃ"] = new Symbol { Prefix = "d", Suffix = "i", Nasal = "nn" },
                ["どぅ"] = new Symbol { Prefix = "d", Suffix = "u", Nasal = "nn" },
                ["で"] = new Symbol { Prefix = "d", Suffix = "e", Nasal = "nn" },
                ["ど"] = new Symbol { Prefix = "d", Suffix = "o", Nasal = "nn" },
                ["づぁ"] = new Symbol { Prefix = "dz", Suffix = "a", Nasal = "nn" },
                ["づぃ"] = new Symbol { Prefix = "dz", Suffix = "i", Nasal = "nn" },
                ["づ"] = new Symbol { Prefix = "dz", Suffix = "u", Nasal = "nn" },
                ["づぇ"] = new Symbol { Prefix = "dz", Suffix = "e", Nasal = "nn" },
                ["づぉ"] = new Symbol { Prefix = "dz", Suffix = "o", Nasal = "nn" },
                ["な"] = new Symbol { Prefix = "n", Suffix = "a", Nasal = "nn" },
                ["ぬ"] = new Symbol { Prefix = "n", Suffix = "u", Nasal = "nn" },
                ["ね"] = new Symbol { Prefix = "n", Suffix = "e", Nasal = "nn" },
                ["の"] = new Symbol { Prefix = "n", Suffix = "o", Nasal = "nn" },
                ["にゃ"] = new Symbol { Prefix = "ny", Suffix = "a", Nasal = "nn" },
                ["に"] = new Symbol { Prefix = "ny", Suffix = "i", Nasal = "nn" },
                ["にゅ"] = new Symbol { Prefix = "ny", Suffix = "u", Nasal = "nn" },
                ["にぇ"] = new Symbol { Prefix = "ny", Suffix = "e", Nasal = "nn" },
                ["にょ"] = new Symbol { Prefix = "ny", Suffix = "o", Nasal = "nn" },
                ["は"] = new Symbol { Prefix = "h", Suffix = "a", Nasal = "N" },
                ["ほぅ"] = new Symbol { Prefix = "h", Suffix = "u", Nasal = "N" },
                ["へ"] = new Symbol { Prefix = "h", Suffix = "e", Nasal = "N" },
                ["ほ"] = new Symbol { Prefix = "h", Suffix = "o", Nasal = "N" },
                ["ふぁ"] = new Symbol { Prefix = "f", Suffix = "a", Nasal = "N" },
                ["ふぃ"] = new Symbol { Prefix = "f", Suffix = "i", Nasal = "N" },
                ["ふ"] = new Symbol { Prefix = "f", Suffix = "u", Nasal = "N" },
                ["ふぇ"] = new Symbol { Prefix = "f", Suffix = "e", Nasal = "N" },
                ["ふぉ"] = new Symbol { Prefix = "f", Suffix = "o", Nasal = "N" },
                ["ファ"] = new Symbol { Prefix = "ff", Suffix = "a", Nasal = "N" },
                ["フィ"] = new Symbol { Prefix = "ff", Suffix = "i", Nasal = "N" },
                ["フ"] = new Symbol { Prefix = "ff", Suffix = "u", Nasal = "N" },
                ["フェ"] = new Symbol { Prefix = "ff", Suffix = "e", Nasal = "N" },
                ["フォ"] = new Symbol { Prefix = "ff", Suffix = "o", Nasal = "N" },
                ["ひゃ"] = new Symbol { Prefix = "hy", Suffix = "a", Nasal = "N" },
                ["ひ"] = new Symbol { Prefix = "hy", Suffix = "i", Nasal = "N" },
                ["ひゅ"] = new Symbol { Prefix = "hy", Suffix = "u", Nasal = "N" },
                ["ひぇ"] = new Symbol { Prefix = "hy", Suffix = "e", Nasal = "N" },
                ["ひょ"] = new Symbol { Prefix = "hy", Suffix = "o", Nasal = "N" },
                ["ば"] = new Symbol { Prefix = "b", Suffix = "a", Nasal = "mm" },
                ["ぶ"] = new Symbol { Prefix = "b", Suffix = "u", Nasal = "mm" },
                ["べ"] = new Symbol { Prefix = "b", Suffix = "e", Nasal = "mm" },
                ["ぼ"] = new Symbol { Prefix = "b", Suffix = "o", Nasal = "mm" },
                ["びゃ"] = new Symbol { Prefix = "by", Suffix = "a", Nasal = "mm" },
                ["び"] = new Symbol { Prefix = "by", Suffix = "i", Nasal = "mm" },
                ["びゅ"] = new Symbol { Prefix = "by", Suffix = "u", Nasal = "mm" },
                ["びぇ"] = new Symbol { Prefix = "by", Suffix = "e", Nasal = "mm" },
                ["びょ"] = new Symbol { Prefix = "by", Suffix = "o", Nasal = "mm" },
                ["ぱ"] = new Symbol { Prefix = "p", Suffix = "a", Nasal = "mm" },
                ["ぷ"] = new Symbol { Prefix = "p", Suffix = "u", Nasal = "mm" },
                ["ぺ"] = new Symbol { Prefix = "p", Suffix = "e", Nasal = "mm" },
                ["ぽ"] = new Symbol { Prefix = "p", Suffix = "o", Nasal = "mm" },
                ["ぴゃ"] = new Symbol { Prefix = "py", Suffix = "a", Nasal = "mm" },
                ["ぴ"] = new Symbol { Prefix = "py", Suffix = "i", Nasal = "mm" },
                ["ぴゅ"] = new Symbol { Prefix = "py", Suffix = "u", Nasal = "mm" },
                ["ぴぇ"] = new Symbol { Prefix = "py", Suffix = "e", Nasal = "mm" },
                ["ぴょ"] = new Symbol { Prefix = "py", Suffix = "o", Nasal = "mm" },
                ["ま"] = new Symbol { Prefix = "m", Suffix = "a", Nasal = "mm" },
                ["む"] = new Symbol { Prefix = "m", Suffix = "u", Nasal = "mm" },
                ["め"] = new Symbol { Prefix = "m", Suffix = "e", Nasal = "mm" },
                ["も"] = new Symbol { Prefix = "m", Suffix = "o", Nasal = "mm" },
                ["みゃ"] = new Symbol { Prefix = "my", Suffix = "a", Nasal = "mm" },
                ["み"] = new Symbol { Prefix = "my", Suffix = "i", Nasal = "mm" },
                ["みゅ"] = new Symbol { Prefix = "my", Suffix = "u", Nasal = "mm" },
                ["みぇ"] = new Symbol { Prefix = "my", Suffix = "e", Nasal = "mm" },
                ["みょ"] = new Symbol { Prefix = "my", Suffix = "o", Nasal = "mm" },
                ["や"] = new Symbol { Prefix = "y", Suffix = "a", Nasal = "N" },
                ["ゆ"] = new Symbol { Prefix = "y", Suffix = "u", Nasal = "N" },
                ["いぇ"] = new Symbol { Prefix = "y", Suffix = "e", Nasal = "N" },
                ["よ"] = new Symbol { Prefix = "y", Suffix = "o", Nasal = "N" },
                ["ら"] = new Symbol { Prefix = "r", Suffix = "a", Nasal = "nn" },
                ["る"] = new Symbol { Prefix = "r", Suffix = "u", Nasal = "nn" },
                ["れ"] = new Symbol { Prefix = "r", Suffix = "e", Nasal = "nn" },
                ["ろ"] = new Symbol { Prefix = "r", Suffix = "o", Nasal = "nn" },
                ["りゃ"] = new Symbol { Prefix = "ry", Suffix = "a", Nasal = "nn" },
                ["り"] = new Symbol { Prefix = "ry", Suffix = "i", Nasal = "nn" },
                ["りゅ"] = new Symbol { Prefix = "ry", Suffix = "u", Nasal = "nn" },
                ["りぇ"] = new Symbol { Prefix = "ry", Suffix = "e", Nasal = "nn" },
                ["りょ"] = new Symbol { Prefix = "ry", Suffix = "o", Nasal = "nn" },
                ["ラ"] = new Symbol { Prefix = "l", Suffix = "a", Nasal = "nn" },
                ["リ"] = new Symbol { Prefix = "l", Suffix = "i", Nasal = "nn" },
                ["ル"] = new Symbol { Prefix = "l", Suffix = "u", Nasal = "nn" },
                ["レ"] = new Symbol { Prefix = "l", Suffix = "e", Nasal = "nn" },
                ["ロ"] = new Symbol { Prefix = "l", Suffix = "o", Nasal = "nn" },
                ["わ"] = new Symbol { Prefix = "w", Suffix = "a", Nasal = "N" },
                ["うぃ"] = new Symbol { Prefix = "w", Suffix = "i", Nasal = "N" },
                ["うぇ"] = new Symbol { Prefix = "w", Suffix = "e", Nasal = "N" },
                ["うぉ"] = new Symbol { Prefix = "w", Suffix = "o", Nasal = "N" },
                ["ヴぁ"] = new Symbol { Prefix = "v", Suffix = "a", Nasal = "N" },
                ["ヴぃ"] = new Symbol { Prefix = "v", Suffix = "i", Nasal = "N" },
                ["ヴ"] = new Symbol { Prefix = "v", Suffix = "u", Nasal = "N" },
                ["ヴぇ"] = new Symbol { Prefix = "v", Suffix = "e", Nasal = "N" },
                ["ヴぉ"] = new Symbol { Prefix = "v", Suffix = "o", Nasal = "N" },
                ["-"] = new Symbol { Prefix = "" },
                ["hh"] = new Symbol { Prefix = "" }
            };
        }

        private AbsideePhonemizerUtil util;

        private USinger? singer;
        public override void SetSinger(USinger singer) {
            this.singer = singer;
            util.Singer = singer;
        }

        public override Result Process(Note[] notes, Note? prev, Note? next, Note? prevNeighbour, Note? nextNeighbour, Note[] prevs) {
            var note = notes[0];

            // Use phonetic hint to force alias
            if (!string.IsNullOrEmpty(note.phoneticHint)) {
                return MakeSimpleResult(note.phoneticHint);
            }

            var isFirst = prevNeighbour == null;
            var isLast = nextNeighbour == null;
            string lyric = note.lyric;
            var prevLyric = isFirst ? "" : prevNeighbour.Value.lyric;
            var nextLyric = isLast ? "" : nextNeighbour.Value.lyric;
            var isVowel = symbols[lyric].Prefix == "";
            var nextIsVowel = isLast ? false : symbols[nextLyric].Prefix == "";
            var phonemes = new List<Phoneme>();
            int totalDuration = notes.Sum(n => n.duration);

            // CV
            if (isFirst) {
                phonemes.Add(new Phoneme {
                    phoneme = $"- {lyric}"
                });
            } else {
                if (isVowel) {
                    if (lyric == "ん" && !isLast) {
                        phonemes.Add(new Phoneme {
                            phoneme = $"{symbols[prevLyric].Suffix} {symbols[nextNeighbour.Value.lyric].Nasal}"
                        });
                    } else {
                        phonemes.Add(new Phoneme {
                            phoneme = $"{symbols[prevLyric].Suffix} {lyric}"
                        });
                    }
                } else {
                    var vcv = $"{symbols[prevLyric].Suffix} {lyric}";
                    if (util.CanVCV(vcv, (Note)prev, note)) {
                        phonemes.Add(new Phoneme {
                            phoneme = vcv
                        });
                    } else {
                        phonemes.Add(new Phoneme {
                            phoneme = lyric
                        });
                    }
                }
            }

            // VC
            if (isLast && lyric != "-" && lyric != "hh") {
                phonemes.Add(new Phoneme {
                    phoneme = $"{symbols[lyric].Suffix} -",
                    position = totalDuration - Math.Min(totalDuration / 2, 120)
                });
            } else if (lyric != "-" && lyric != "hh" && !nextIsVowel) {
                var vcv = $"{symbols[lyric].Suffix} {nextLyric}";
                if (!util.CanVCV(vcv, note, (Note)next)) {
                    var vc = $"{symbols[lyric].Suffix} {symbols[nextLyric].Prefix}";

                    int vcLength = 120;
                    var attr = nextNeighbour.Value.phonemeAttributes?.FirstOrDefault(attr => attr.index == 0) ?? default;
                    if (singer.TryGetMappedOto(nextLyric, nextNeighbour.Value.tone + attr.toneShift, attr.voiceColor, out var oto)) {
                        vcLength = MsToTick(oto.Preutter);
                    }
                    vcLength = Convert.ToInt32(Math.Min(totalDuration / 2, vcLength * (attr.consonantStretchRatio ?? 1)));

                    phonemes.Add(new Phoneme {
                        phoneme = vc,
                        position = totalDuration - vcLength
                    });
                }
            }

            return new Result { phonemes = util.AssignSuffixes(phonemes, notes, prevs) };
        }
    }
}
