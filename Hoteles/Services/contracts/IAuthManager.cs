using Hoteles.Data.DTOs;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Hoteles.Services.contracts
{
     public interface IAuthManager
    {
        Task<bool> ValidateUser(LoginUserDTO loginUserDTO);

        Task<string> CreateToken();
        
    }
}
