namespace StartPos.Shared.Extesions
{
    public static class LongExtensions
    {
        public static long ConvertBytesToMegabytes(this long bytes) => bytes / (1024L * 1024L);
    }
}