using IPALogger = IPA.Logging.Logger;

namespace SongPerformer
{
    internal static class Logger
    {
        internal static IPALogger log { get; set; }

        internal static void Write(string text)
        {
            log.Error(text);
        }
    }
}
