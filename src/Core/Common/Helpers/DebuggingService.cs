namespace LMS.Common.Helpers
{
    using System.Diagnostics;

    public static class DebuggingService
    {
        private static bool _debugging;

        [Conditional("DEBUG")]
        private static void WellAreWe()
        {
            _debugging = true;
        }

        public static bool Debug
        {
            get
            {
                WellAreWe();
                return _debugging;
            }
        }
    }
}