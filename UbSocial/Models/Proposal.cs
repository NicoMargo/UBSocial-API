using UbSocial.Models.Helpers;

namespace UbSocial.Models
{
    public class Proposal
    {
        private string? _title;
        private string? _description;

        public Proposal() {
        }

        public string? Title { get => _title; set => _title = value; }
        public string? Description { get => _description; set => _description = value; }

        public string Create (Proposal proposal)
        {            
            
            Dictionary<string, object> args = new Dictionary<string, object> {
                    {"pTitle",proposal._title},
                    {"pdescription",proposal._description},
            };

            return (DBHelper.CallNonQuery("spProposalCreate", args));           
            
        }
    }
}
