using UbSocial.Models.Helpers;

namespace UbSocial.Models
{
    public class Activity
    {
        private string? _title;
        private string? _description;
        private string? _contact;
        private List<String>? _URLPhotos;
        private DateTime? _activityDate;
        private int? _id;

        public Activity()
        {
        }

        public string? Title { get => _title; set => _title = value; }
        public string? Description { get => _description; set => _description = value; }
        public int? Id { get => _id; set => _id = value; }
        public string? Contact { get => _contact; set => _contact = value; }
        public DateTime? ActivityDate { get => _activityDate; set => _activityDate = value; }
        public List<string>? URLPhotos { get => _URLPhotos; set => _URLPhotos = value; }

    }
}
