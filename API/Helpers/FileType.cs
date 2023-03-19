namespace API.Helpers
{
    public class FileType
    {
        public static readonly (string, string) JavaScript = ("Scripts", "js");
        public static readonly (string, string) Stylesheet = ("Styles", "css");
        public static readonly (string, string) RazorPage = ("Views", "cshtml");

        private readonly (string, string) _fileType;

        public string Folder => _fileType.Item1;
        public string FileExtension => _fileType.Item2;

        public FileType((string, string) fileType)
        {
            _fileType = fileType;
        }
    }
}
