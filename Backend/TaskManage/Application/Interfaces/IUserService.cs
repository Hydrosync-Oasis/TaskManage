using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interfaces
{
    public interface IUserService
    {
        public Task Register(string username, string password);
        public Task<LoginResultDto> Login(string username, string password);
    }
}
