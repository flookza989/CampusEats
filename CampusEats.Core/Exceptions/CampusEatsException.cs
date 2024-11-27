using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEats.Core.Exceptions
{
    public class CampusEatsException : Exception
    {
        public CampusEatsException(string message) : base(message) { }
        public CampusEatsException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
