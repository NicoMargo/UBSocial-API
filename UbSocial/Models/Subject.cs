using UbSocial.Models.Helpers;

namespace UbSocial.Models
{
    public class Subject
    {
        private string? _name;
        private int? _id;

        public Subject()
        {
        }

        public string? Name { get => _name; set => _name = value; }
        public int? Id { get => _id; set => _id = value; }

        /* public string Create(Subject subject)
        {

            Dictionary<string, object> args = new Dictionary<string, object> {
                    {"pName",subject._name},
            };

            return (DBHelper.CallNonQuery("spSubjectCreate", args));

        }
        */
    }
}
