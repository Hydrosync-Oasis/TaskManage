using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Auth {
    public interface IJwtTokenGenerator {
        public Task<string> GenerateToken(int userId);
    }
}
