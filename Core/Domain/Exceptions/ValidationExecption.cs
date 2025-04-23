using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class ValidationExecption(IEnumerable<string> errors) :
        Exception("Validation Errors")
    {
        public IEnumerable<string> Errors { get; } = errors;
    }
}
