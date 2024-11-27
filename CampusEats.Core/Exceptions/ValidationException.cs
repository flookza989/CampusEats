using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEats.Core.Exceptions
{
    public class ValidationException : CampusEatsException
    {
        public string PropertyName { get; }
        public object AttemptedValue { get; }

        public ValidationException(string propertyName, object attemptedValue, string message)
            : base(message)
        {
            PropertyName = propertyName;
            AttemptedValue = attemptedValue;
        }
    }
}
