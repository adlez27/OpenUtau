using System.IO;
using System.Reflection;
using System.Text;
using OpenUtau.Api;
using OpenUtau.Classic;
using OpenUtau.Core.Ustx;

namespace AbsideePhonemizer {
    public static class AbsideePhonemizerUtil {
        public static Voicebank GetVoicebank(string singerName) {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var file = Path.Join(dir, "absidee", "character.txt");

            return new Voicebank() { File = file, BasePath = dir };
        }
    }
}
