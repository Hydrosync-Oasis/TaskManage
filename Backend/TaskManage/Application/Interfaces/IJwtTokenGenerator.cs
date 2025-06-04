namespace Infrastructure.Auth {
    public interface IJwtTokenGenerator {
        public Task<string> GenerateToken(int userId);
    }
}
