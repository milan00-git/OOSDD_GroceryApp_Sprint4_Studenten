
namespace Grocery.Core.Models
{
    public enum Role { None, Admin};
    public partial class Client : Model
    {
        public string EmailAddress { get; set; }

        public Role role { get; set; } = Role.None;     // De standaard role is None
        public string Password { get; set; }
        public Client(int id, string name, string emailAddress, string password) : base(id, name)
        {
            EmailAddress=emailAddress;
            Password=password;
        }
    }
}