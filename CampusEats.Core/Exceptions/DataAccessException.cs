using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEats.Core.Exceptions
{
    public class DataAccessException : CampusEatsException
    {
        public string Operation { get; }
        public string EntityType { get; }

        public DataAccessException(string operation, string entityType, Exception innerException)
            : base($"Error occurred during {operation} operation on {entityType}.", innerException)
        {
            Operation = operation;
            EntityType = entityType;
        }
    }
}
