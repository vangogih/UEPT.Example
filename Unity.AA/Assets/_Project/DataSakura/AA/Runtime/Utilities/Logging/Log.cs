namespace DataSakura.AA.Runtime.Utilities.Logging
{
    public static class Log
    {
        public static readonly TagLog Default = new(string.Empty);
        public static readonly TagLog Loading = new("LOADING");

        public static readonly BuilderLogPool Builder = new(new TagLog(string.Empty), 5);
    }
}