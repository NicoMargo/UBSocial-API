using UbSocial.Models.Helpers;

namespace UbSocial.Models
{
    public class DownloadableContent
    {
        private string? _title;
        private string? _description;
        private int? _cantidadDeDescargas;
        private String? _URL;
        private DateTime? _downloadableContentDate;
        private int? _id;

        public DownloadableContent()
        {
        }

        public string? Title { get => _title; set => _title = value; }
        public string? Description { get => _description; set => _description = value; }
        public int? Id { get => _id; set => _id = value; }
        public int? CantidadDeDescargas { get => _cantidadDeDescargas; set => _cantidadDeDescargas = value; }
        public DateTime? DownloadableContentDate { get => _downloadableContentDate; set => _downloadableContentDate = value; }
        public String? URL { get => _URL; set => _URL = value; }

        public string Create(DownloadableContent downloadableContent)
        {

            Dictionary<string, object> args = new Dictionary<string, object> {
                    {"pTitle",downloadableContent._title},
                    {"pDescription",downloadableContent._description},
                    {"pURLPhotos",downloadableContent._URL},
                    {"pDownloadableContentDate",downloadableContent._downloadableContentDate},
                    {"pId",downloadableContent._id},
                    {"pCantidadDeDescargas",downloadableContent._cantidadDeDescargas},
            };

            return (DBHelper.CallNonQuery("spDownloadableContentCreate", args));

        }
    }
}
