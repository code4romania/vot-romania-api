namespace VotRomania.Options
{
    public class ApplicationUsersOptions
    {
        public UserOptions[] Users { get; set; }
    }

    public class UserOptions
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string[] Scopes { get; set; }
    }
}