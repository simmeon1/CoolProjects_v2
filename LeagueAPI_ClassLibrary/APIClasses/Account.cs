namespace LeagueAPI_ClassLibrary
{
    public class Account
    {
        public string Id { get; set; }
        public string AccountId { get; set; }
        public string Puuid { get; set; }
        public string Name { get; set; }
        public Account(string id, string accountId, string puuid, string name)
        {
            Id = id;
            AccountId = accountId;
            Puuid = puuid;
            Name = name;
        }
    }
}