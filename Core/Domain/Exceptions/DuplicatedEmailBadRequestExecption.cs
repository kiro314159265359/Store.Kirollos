using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class DuplicatedEmailBadRequestExecption(string email) : 
        BadRequestExecption($"This Email already Registerd {email}!")
    {
    }
}
