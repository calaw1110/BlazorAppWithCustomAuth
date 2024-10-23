namespace BlazorAppWithCustomAuth.Auth
{
    public class UserAccountService
    {
        private List<UserAccount> _users;
        public UserAccountService()
        {
            _users = new List<UserAccount>()
            {
                new UserAccount() {UserName="admin",Password="123",Role="ADMIN" },
                new UserAccount() {UserName="user",Password="123",Role="USER" },
            };
        }

        public UserAccount? GetByUserName(string userName)
        {
            return _users.Find(x => x.UserName == userName);
        }
    }
}
