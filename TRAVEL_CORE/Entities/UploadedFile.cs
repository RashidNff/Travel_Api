namespace TRAVEL_CORE.Entities
{
    public class UploadedFile
    {
        public int FileId { get; set; } 

        public int FileType { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public string FileFolder { get; set; }

        public int UserId { get; set; }
    }
}
