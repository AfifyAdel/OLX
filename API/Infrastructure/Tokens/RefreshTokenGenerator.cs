using Application.Interfaces;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Tokens
{
    public class RefreshTokenGenerator : IRefreshTokenGenerator
    {
        public string Generate(string userName = null)
        {
            var token = $"{Guid.NewGuid()}  {Guid.NewGuid()} {userName}";
            var encoded = Encoding.UTF8.GetBytes(token);
            return Convert.ToBase64String(encoded);
        }
    }
}
