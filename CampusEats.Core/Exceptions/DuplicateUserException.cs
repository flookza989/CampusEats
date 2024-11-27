using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEats.Core.Exceptions
{
    public class DuplicateUserException : CampusEatsException
    {
        public string Field { get; }
        public string Value { get; }
        public DuplicateUserException(string field, string value)
            : base($"A user with {field} '{value}' already exists.")
        {
            Field = field;
            Value = value;
        }
    }
}
