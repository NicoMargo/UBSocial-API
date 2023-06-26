using UbSocial.Models.Helpers;

namespace UbSocial.Models
{
    public class Activity
    {
        private string? _title;
        private string? _description;
        private string? _contact;
        private int? _idActivity;
        private int? _idUser;
        private String? _URLPhotos;
        private DateTime? _activityDate;
        private DateTime? _activityDateFinished;
        private int? _id;
        private IFormFile? _file;

        public Activity()
        {
        }

        public string? Title { get => _title; set => _title = value; }
        public string? Description { get => _description; set => _description = value; }
        public int? Id { get => _id; set => _id = value; }
        public string? Contact { get => _contact; set => _contact = value; }
        public DateTime? ActivityDate { get => _activityDate; set => _activityDate = value; }
        public string? URLPhotos { get => _URLPhotos; set => _URLPhotos = value; }
        public DateTime? ActivityDateFinished { get => _activityDateFinished; set => _activityDateFinished = value; }
        public IFormFile? File { get => _file; set => _file = value; }
        public int? IdActivity { get => _idActivity; set => _idActivity = value; }
        public int? IdUser { get => _idUser; set => _idUser = value; }
    }
}
