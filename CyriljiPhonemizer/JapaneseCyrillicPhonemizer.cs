using System;
using System.Collections.Generic;
using System.Linq;
using OpenUtau.Api;
using OpenUtau.Core.Ustx;

namespace CyriljiPhonemizer {
    [Phonemizer("Japanese Cyrillic Phonemizer", "Нихонго", "Adlez27", language: "JA")]
    public class JapaneseCyrillicPhonemizer : Phonemizer {
        public struct Symbol {
            public string Prefix;
            public string Suffix;
            public string Nasal;
        }
        private Dictionary<string, Symbol> symbols;
        private Dictionary<string, string> toCyrillic;

        public JapaneseCyrillicPhonemizer() {
            symbols = new Dictionary<string, Symbol> {
                ["а"] = new Symbol { Prefix = "", Suffix = "а", Nasal = "Н" },
                ["и"] = new Symbol { Prefix = "", Suffix = "и", Nasal = "Н" },
                ["у"] = new Symbol { Prefix = "", Suffix = "у", Nasal = "Н" },
                ["э"] = new Symbol { Prefix = "", Suffix = "э", Nasal = "Н" },
                ["о"] = new Symbol { Prefix = "", Suffix = "о", Nasal = "Н" },
                ["Н"] = new Symbol { Prefix = "", Suffix = "н", Nasal = "Н" },
                ["нн"] = new Symbol { Prefix = "", Suffix = "н", Nasal = "нн" },
                ["мм"] = new Symbol { Prefix = "", Suffix = "н", Nasal = "мм" },
                ["нг"] = new Symbol { Prefix = "", Suffix = "н", Nasal = "нг" },
                ["ка"] = new Symbol { Prefix = "к", Suffix = "а", Nasal = "нг" },
                ["ку"] = new Symbol { Prefix = "к", Suffix = "у", Nasal = "нг" },
                ["кэ"] = new Symbol { Prefix = "к", Suffix = "э", Nasal = "нг" },
                ["ко"] = new Symbol { Prefix = "к", Suffix = "о", Nasal = "нг" },
                ["кя"] = new Symbol { Prefix = "кь", Suffix = "а", Nasal = "нг" },
                ["ки"] = new Symbol { Prefix = "кь", Suffix = "и", Nasal = "нг" },
                ["кю"] = new Symbol { Prefix = "кь", Suffix = "у", Nasal = "нг" },
                ["ке"] = new Symbol { Prefix = "кь", Suffix = "э", Nasal = "нг" },
                ["кё"] = new Symbol { Prefix = "кь", Suffix = "о", Nasal = "нг" },
                ["га"] = new Symbol { Prefix = "г", Suffix = "а", Nasal = "нг" },
                ["гу"] = new Symbol { Prefix = "г", Suffix = "у", Nasal = "нг" },
                ["гэ"] = new Symbol { Prefix = "г", Suffix = "э", Nasal = "нг" },
                ["го"] = new Symbol { Prefix = "г", Suffix = "о", Nasal = "нг" },
                ["гя"] = new Symbol { Prefix = "гь", Suffix = "а", Nasal = "нг" },
                ["ги"] = new Symbol { Prefix = "гь", Suffix = "и", Nasal = "нг" },
                ["гю"] = new Symbol { Prefix = "гь", Suffix = "у", Nasal = "нг" },
                ["ге"] = new Symbol { Prefix = "гь", Suffix = "э", Nasal = "нг" },
                ["гё"] = new Symbol { Prefix = "гь", Suffix = "о", Nasal = "нг" },
                ["са"] = new Symbol { Prefix = "с", Suffix = "а", Nasal = "Н" },
                ["си"] = new Symbol { Prefix = "с", Suffix = "и", Nasal = "Н" },
                ["су"] = new Symbol { Prefix = "с", Suffix = "у", Nasal = "Н" },
                ["сэ"] = new Symbol { Prefix = "с", Suffix = "э", Nasal = "Н" },
                ["со"] = new Symbol { Prefix = "с", Suffix = "о", Nasal = "Н" },
                ["ша"] = new Symbol { Prefix = "ш", Suffix = "а", Nasal = "Н" },
                ["ши"] = new Symbol { Prefix = "ш", Suffix = "и", Nasal = "Н" },
                ["шу"] = new Symbol { Prefix = "ш", Suffix = "у", Nasal = "Н" },
                ["шэ"] = new Symbol { Prefix = "ш", Suffix = "э", Nasal = "Н" },
                ["шо"] = new Symbol { Prefix = "ш", Suffix = "о", Nasal = "Н" },
                ["за"] = new Symbol { Prefix = "з", Suffix = "а", Nasal = "Н" },
                ["зи"] = new Symbol { Prefix = "з", Suffix = "и", Nasal = "Н" },
                ["зу"] = new Symbol { Prefix = "з", Suffix = "у", Nasal = "Н" },
                ["зэ"] = new Symbol { Prefix = "з", Suffix = "э", Nasal = "Н" },
                ["зо"] = new Symbol { Prefix = "з", Suffix = "о", Nasal = "Н" },
                ["ђа"] = new Symbol { Prefix = "ђ", Suffix = "а", Nasal = "Н" },
                ["ђи"] = new Symbol { Prefix = "ђ", Suffix = "и", Nasal = "Н" },
                ["ђу"] = new Symbol { Prefix = "ђ", Suffix = "у", Nasal = "Н" },
                ["ђэ"] = new Symbol { Prefix = "ђ", Suffix = "э", Nasal = "Н" },
                ["ђо"] = new Symbol { Prefix = "ђ", Suffix = "о", Nasal = "Н" },
                ["та"] = new Symbol { Prefix = "т", Suffix = "а", Nasal = "нн" },
                ["ти"] = new Symbol { Prefix = "т", Suffix = "и", Nasal = "нн" },
                ["ту"] = new Symbol { Prefix = "т", Suffix = "у", Nasal = "нн" },
                ["тэ"] = new Symbol { Prefix = "т", Suffix = "э", Nasal = "нн" },
                ["то"] = new Symbol { Prefix = "т", Suffix = "о", Nasal = "нн" },
                ["ћа"] = new Symbol { Prefix = "ћ", Suffix = "а", Nasal = "нн" },
                ["ћи"] = new Symbol { Prefix = "ћ", Suffix = "и", Nasal = "нн" },
                ["ћу"] = new Symbol { Prefix = "ћ", Suffix = "у", Nasal = "нн" },
                ["ћэ"] = new Symbol { Prefix = "ћ", Suffix = "э", Nasal = "нн" },
                ["ћо"] = new Symbol { Prefix = "ћ", Suffix = "о", Nasal = "нн" },
                ["ца"] = new Symbol { Prefix = "ц", Suffix = "а", Nasal = "нн" },
                ["ци"] = new Symbol { Prefix = "ц", Suffix = "и", Nasal = "нн" },
                ["цу"] = new Symbol { Prefix = "ц", Suffix = "у", Nasal = "нн" },
                ["цэ"] = new Symbol { Prefix = "ц", Suffix = "э", Nasal = "нн" },
                ["цо"] = new Symbol { Prefix = "ц", Suffix = "о", Nasal = "нн" },
                ["да"] = new Symbol { Prefix = "д", Suffix = "а", Nasal = "нн" },
                ["ди"] = new Symbol { Prefix = "д", Suffix = "и", Nasal = "нн" },
                ["ду"] = new Symbol { Prefix = "д", Suffix = "у", Nasal = "нн" },
                ["дэ"] = new Symbol { Prefix = "д", Suffix = "э", Nasal = "нн" },
                ["до"] = new Symbol { Prefix = "д", Suffix = "о", Nasal = "нн" },
                ["на"] = new Symbol { Prefix = "н", Suffix = "а", Nasal = "нн" },
                ["ну"] = new Symbol { Prefix = "н", Suffix = "у", Nasal = "нн" },
                ["нэ"] = new Symbol { Prefix = "н", Suffix = "э", Nasal = "нн" },
                ["но"] = new Symbol { Prefix = "н", Suffix = "о", Nasal = "нн" },
                ["ња"] = new Symbol { Prefix = "њ", Suffix = "а", Nasal = "нн" },
                ["њи"] = new Symbol { Prefix = "њ", Suffix = "и", Nasal = "нн" },
                ["њу"] = new Symbol { Prefix = "њ", Suffix = "у", Nasal = "нн" },
                ["њэ"] = new Symbol { Prefix = "њ", Suffix = "э", Nasal = "нн" },
                ["њо"] = new Symbol { Prefix = "њ", Suffix = "о", Nasal = "нн" },
                ["ха"] = new Symbol { Prefix = "х", Suffix = "а", Nasal = "Н" },
                ["ху"] = new Symbol { Prefix = "х", Suffix = "у", Nasal = "Н" },
                ["хэ"] = new Symbol { Prefix = "х", Suffix = "э", Nasal = "Н" },
                ["хо"] = new Symbol { Prefix = "х", Suffix = "о", Nasal = "Н" },
                ["фа"] = new Symbol { Prefix = "ф", Suffix = "а", Nasal = "Н" },
                ["фи"] = new Symbol { Prefix = "ф", Suffix = "и", Nasal = "Н" },
                ["фу"] = new Symbol { Prefix = "ф", Suffix = "у", Nasal = "Н" },
                ["фэ"] = new Symbol { Prefix = "ф", Suffix = "э", Nasal = "Н" },
                ["фо"] = new Symbol { Prefix = "ф", Suffix = "о", Nasal = "Н" },
                ["хя"] = new Symbol { Prefix = "хь", Suffix = "а", Nasal = "Н" },
                ["хи"] = new Symbol { Prefix = "хь", Suffix = "и", Nasal = "Н" },
                ["хю"] = new Symbol { Prefix = "хь", Suffix = "у", Nasal = "Н" },
                ["хе"] = new Symbol { Prefix = "хь", Suffix = "э", Nasal = "Н" },
                ["хё"] = new Symbol { Prefix = "хь", Suffix = "о", Nasal = "Н" },
                ["ба"] = new Symbol { Prefix = "б", Suffix = "а", Nasal = "мм" },
                ["бу"] = new Symbol { Prefix = "б", Suffix = "у", Nasal = "мм" },
                ["бэ"] = new Symbol { Prefix = "б", Suffix = "э", Nasal = "мм" },
                ["бо"] = new Symbol { Prefix = "б", Suffix = "о", Nasal = "мм" },
                ["бя"] = new Symbol { Prefix = "бь", Suffix = "а", Nasal = "мм" },
                ["би"] = new Symbol { Prefix = "бь", Suffix = "и", Nasal = "мм" },
                ["бю"] = new Symbol { Prefix = "бь", Suffix = "у", Nasal = "мм" },
                ["бе"] = new Symbol { Prefix = "бь", Suffix = "э", Nasal = "мм" },
                ["бё"] = new Symbol { Prefix = "бь", Suffix = "о", Nasal = "мм" },
                ["па"] = new Symbol { Prefix = "п", Suffix = "а", Nasal = "мм" },
                ["пу"] = new Symbol { Prefix = "п", Suffix = "у", Nasal = "мм" },
                ["пэ"] = new Symbol { Prefix = "п", Suffix = "э", Nasal = "мм" },
                ["по"] = new Symbol { Prefix = "п", Suffix = "о", Nasal = "мм" },
                ["пя"] = new Symbol { Prefix = "пь", Suffix = "а", Nasal = "мм" },
                ["пи"] = new Symbol { Prefix = "пь", Suffix = "и", Nasal = "мм" },
                ["пю"] = new Symbol { Prefix = "пь", Suffix = "у", Nasal = "мм" },
                ["пе"] = new Symbol { Prefix = "пь", Suffix = "э", Nasal = "мм" },
                ["пё"] = new Symbol { Prefix = "пь", Suffix = "о", Nasal = "мм" },
                ["ма"] = new Symbol { Prefix = "м", Suffix = "а", Nasal = "мм" },
                ["му"] = new Symbol { Prefix = "м", Suffix = "у", Nasal = "мм" },
                ["мэ"] = new Symbol { Prefix = "м", Suffix = "э", Nasal = "мм" },
                ["мо"] = new Symbol { Prefix = "м", Suffix = "о", Nasal = "мм" },
                ["мя"] = new Symbol { Prefix = "мь", Suffix = "а", Nasal = "мм" },
                ["ми"] = new Symbol { Prefix = "мь", Suffix = "и", Nasal = "мм" },
                ["мю"] = new Symbol { Prefix = "мь", Suffix = "у", Nasal = "мм" },
                ["ме"] = new Symbol { Prefix = "мь", Suffix = "э", Nasal = "мм" },
                ["мё"] = new Symbol { Prefix = "мь", Suffix = "о", Nasal = "мм" },
                ["я"] = new Symbol { Prefix = "й", Suffix = "а", Nasal = "Н" },
                ["ю"] = new Symbol { Prefix = "й", Suffix = "у", Nasal = "Н" },
                ["е"] = new Symbol { Prefix = "й", Suffix = "э", Nasal = "Н" },
                ["ё"] = new Symbol { Prefix = "й", Suffix = "о", Nasal = "Н" },
                ["ла"] = new Symbol { Prefix = "л", Suffix = "а", Nasal = "нн" },
                ["лу"] = new Symbol { Prefix = "л", Suffix = "у", Nasal = "нн" },
                ["лэ"] = new Symbol { Prefix = "л", Suffix = "э", Nasal = "нн" },
                ["ло"] = new Symbol { Prefix = "л", Suffix = "о", Nasal = "нн" },
                ["ља"] = new Symbol { Prefix = "љ", Suffix = "а", Nasal = "нн" },
                ["љи"] = new Symbol { Prefix = "љ", Suffix = "и", Nasal = "нн" },
                ["љу"] = new Symbol { Prefix = "љ", Suffix = "у", Nasal = "нн" },
                ["љэ"] = new Symbol { Prefix = "љ", Suffix = "э", Nasal = "нн" },
                ["љо"] = new Symbol { Prefix = "љ", Suffix = "о", Nasal = "нн" },
                ["ва"] = new Symbol { Prefix = "в", Suffix = "а", Nasal = "Н" },
                ["ви"] = new Symbol { Prefix = "в", Suffix = "и", Nasal = "Н" },
                ["вэ"] = new Symbol { Prefix = "в", Suffix = "э", Nasal = "Н" },
                ["во"] = new Symbol { Prefix = "в", Suffix = "о", Nasal = "Н" },
                ["-"] = new Symbol { Prefix = ""}
            };

            toCyrillic = new Dictionary<string, string> {
                ["あ"] = "а",
                ["い"] = "и",
                ["う"] = "у",
                ["え"] = "э",
                ["お"] = "о",
                ["ん"] = "Н",
                //["んn"] = "нн",
                //["んm"] = "мм",
                //["んng"] = "нг",
                ["か"] = "ка",
                ["く"] = "ку",
                ["け"] = "кэ",
                ["こ"] = "ко",
                ["きゃ"] = "кя",
                ["き"] = "ки",
                ["きゅ"] = "кю",
                ["きぇ"] = "ке",
                ["きょ"] = "кё",
                ["が"] = "га",
                ["ぐ"] = "гу",
                ["げ"] = "гэ",
                ["ご"] = "го",
                ["ぎゃ"] = "гя",
                ["ぎ"] = "ги",
                ["ぎゅ"] = "гю",
                ["ぎぇ"] = "ге",
                ["ぎょ"] = "гё",
                ["さ"] = "са",
                ["すぃ"] = "си",
                ["す"] = "су",
                ["せ"] = "сэ",
                ["そ"] = "со",
                ["しゃ"] = "ша",
                ["し"] = "ши",
                ["しゅ"] = "шу",
                ["しぇ"] = "шэ",
                ["しょ"] = "шо",
                ["ざ"] = "за",
                ["ずぃ"] = "зи",
                ["ず"] = "зу",
                ["ぜ"] = "зэ",
                ["ぞ"] = "зо",
                ["じゃ"] = "ђа",
                ["じ"] = "ђи",
                ["じゅ"] = "ђу",
                ["じぇ"] = "ђэ",
                ["じょ"] = "ђо",
                ["た"] = "та",
                ["てぃ"] = "ти",
                ["とぅ"] = "ту",
                ["て"] = "тэ",
                ["と"] = "то",
                ["ちゃ"] = "ћа",
                ["ち"] = "ћи",
                ["ちゅ"] = "ћу",
                ["ちぇ"] = "ћэ",
                ["ちょ"] = "ћо",
                ["つぁ"] = "ца",
                ["つぃ"] = "ци",
                ["つ"] = "цу",
                ["つぇ"] = "цэ",
                ["つぉ"] = "цо",
                ["だ"] = "да",
                ["でぃ"] = "ди",
                ["どぅ"] = "ду",
                ["で"] = "дэ",
                ["ど"] = "до",
                ["な"] = "на",
                ["ぬ"] = "ну",
                ["ね"] = "нэ",
                ["の"] = "но",
                ["にゃ"] = "ња",
                ["に"] = "њи",
                ["にゅ"] = "њу",
                ["にぇ"] = "њэ",
                ["にょ"] = "њо",
                ["は"] = "ха",
                ["ほぅ"] = "ху",
                ["へ"] = "хэ",
                ["ほ"] = "хо",
                ["ふぁ"] = "фа",
                ["ふぃ"] = "фи",
                ["ふ"] = "фу",
                ["ふぇ"] = "фэ",
                ["ふぉ"] = "фо",
                ["ひゃ"] = "хя",
                ["ひ"] = "хи",
                ["ひゅ"] = "хю",
                ["ひぇ"] = "хе",
                ["ひょ"] = "хё",
                ["ば"] = "ба",
                ["ぶ"] = "бу",
                ["べ"] = "бэ",
                ["ぼ"] = "бо",
                ["びゃ"] = "бя",
                ["び"] = "би",
                ["びゅ"] = "бю",
                ["びぇ"] = "бе",
                ["びょ"] = "бё",
                ["ぱ"] = "па",
                ["ぷ"] = "пу",
                ["ぺ"] = "пэ",
                ["ぽ"] = "по",
                ["ぴゃ"] = "пя",
                ["ぴ"] = "пи",
                ["ぴゅ"] = "пю",
                ["ぴぇ"] = "пе",
                ["ぴょ"] = "пё",
                ["ま"] = "ма",
                ["む"] = "му",
                ["め"] = "мэ",
                ["も"] = "мо",
                ["みゃ"] = "мя",
                ["み"] = "ми",
                ["みゅ"] = "мю",
                ["みぇ"] = "ме",
                ["みょ"] = "мё",
                ["や"] = "я",
                ["ゆ"] = "ю",
                ["いぇ"] = "е",
                ["よ"] = "ё",
                ["ら"] = "ла",
                ["る"] = "лу",
                ["れ"] = "лэ",
                ["ろ"] = "ло",
                ["りゃ"] = "ља",
                ["り"] = "љи",
                ["りゅ"] = "љу",
                ["りぇ"] = "љэ",
                ["りょ"] = "љо",
                ["わ"] = "ва",
                ["うぃ"] = "ви",
                ["うぇ"] = "вэ",
                ["うぉ"] = "во",
                ["-"] = "-"
            };
        }

        private USinger? singer;
        public override void SetSinger(USinger singer) => this.singer = singer;

        public override Result Process(Note[] notes, Note? prev, Note? next, Note? prevNeighbour, Note? nextNeighbour, Note[] prevs) {
            var note = notes[0];

            // Use phonetic hint to force alias
            if (!string.IsNullOrEmpty(note.phoneticHint)) {
                return MakeSimpleResult(note.phoneticHint);
            }

            var isFirst = prevNeighbour == null;
            var isLast = nextNeighbour == null;
            string lyric = toCyrillic[note.lyric];
            var prevLyric = isFirst ? "" : toCyrillic[prevNeighbour.Value.lyric];
            var nextLyric = isLast ? "" : toCyrillic[nextNeighbour.Value.lyric];
            var isVowel = symbols[lyric].Prefix == "";
            var nextIsVowel = isLast ? false : symbols[nextLyric].Prefix == "";
            var phonemes = new List<Phoneme>();
            int totalDuration = notes.Sum(n => n.duration);

            if (isFirst) {
                phonemes.Add(new Phoneme {
                    phoneme = $"- {lyric}"
                });
            } else {
                if (isVowel) {
                    if (lyric == "Н" && !isLast) {
                        phonemes.Add(new Phoneme {
                            phoneme = $"{symbols[prevLyric].Suffix} {symbols[toCyrillic[nextNeighbour.Value.lyric]].Nasal}"
                        });
                    } else {
                        phonemes.Add(new Phoneme {
                            phoneme = $"{symbols[prevLyric].Suffix} {lyric}"
                        });
                    }
                } else {
                    phonemes.Add(new Phoneme { 
                        phoneme = lyric
                    });
                }
            }

            if (isLast && lyric != "-") {
                phonemes.Add(new Phoneme {
                    phoneme = $"{symbols[lyric].Suffix} -",
                    position = totalDuration - Math.Min(totalDuration / 2, 120)
                });
            } else if (lyric != "-" && !nextIsVowel) {
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

            // Assign pitch/color suffixes
            int noteIndex = 0;
            for (int i = 0; i < phonemes.Count; i++) {
                var attr = note.phonemeAttributes?.FirstOrDefault(attr => attr.index == i) ?? default;
                string alt = attr.alternate?.ToString() ?? string.Empty;
                string color = attr.voiceColor;
                int toneShift = attr.toneShift;
                var phoneme = phonemes[i];
                while (noteIndex < notes.Length - 1 && notes[noteIndex].position - note.position < phoneme.position) {
                    noteIndex++;
                }
                int tone = (i == 0 && prevs != null && prevs.Length > 0)
                    ? prevs.Last().tone : notes[noteIndex].tone;
                if (singer.TryGetMappedOto($"{phoneme.phoneme}{(alt == "0" ? "" : alt)}", note.tone + toneShift, color, out var oto)) {
                    phoneme.phoneme = oto.Alias;
                } 
                phonemes[i] = phoneme;
            }

            return new Result { phonemes = phonemes.ToArray()};
        }
    }
}
