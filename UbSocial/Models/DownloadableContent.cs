using UbSocial.Models.Helpers;

namespace UbSocial.Models
{
    public class DownloadableContent
    {
        private string? _title;
        private int? _cantidadDeDescargas;
        private string? _URL;
        private int? _idSubject;
        private DateTime? _downloadableContentDate;
        private int? _id;
        private IFormFile? _file;

        public DownloadableContent()
        {
        }

        public string? Title { get => _title; set => _title = value; }
        public int? Id { get => _id; set => _id = value; }
        public int? CantidadDeDescargas { get => _cantidadDeDescargas; set => _cantidadDeDescargas = value; }
        public DateTime? DownloadableContentDate { get => _downloadableContentDate; set => _downloadableContentDate = value; }
        public string? URL { get => _URL; set => _URL = value; }
        public IFormFile? File { get => _file; set => _file = value; }
        public int? IdSubject { get => _idSubject; set => _idSubject = value; }

    }
}
