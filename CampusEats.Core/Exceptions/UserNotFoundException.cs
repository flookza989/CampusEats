using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEats.Core.Exceptions
{
    public class UserNotFoundException : CampusEatsException
    {
        public string Username { get; }
        public UserNotFoundException(string username)
            : base($"User '{username}' was not found.")
        {
            Username = username;
        }
    }
}
