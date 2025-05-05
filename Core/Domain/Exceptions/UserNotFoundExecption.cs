using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class UserNotFoundExecption(string Email) :
        NotFoundExecption($"user With Email:{Email} Not Found !!")
    {
    }
}
