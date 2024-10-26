namespace cog1.DTO
{
    public class UserDTO
    {
        public int userId { get; set; }
        public string userName { get; set; }
        public bool isOperator { get; set; }
        public bool isAdmin { get; set; }
        public string localeCode { get; set; }
    }
}
