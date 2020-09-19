namespace BokujoMessage4
{
    public struct appconfig
    {
        public string GeneralFullPath;
        public string[] FilePath;

        public int SeedRNG;
        public int DefaultSeriesSelection;
        public TextEditorConfig TextEditorCfg;
        public string[] KeyWords;
    }

    public struct TextEditorConfig
    {
        public int prefLineLenght;
    }
}
