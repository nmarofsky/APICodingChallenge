using Newtonsoft.Json;

namespace APICodingChallenge.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string City { get; set; } = null!;
        public List<Player> Players { get; set; } = new();
    }
}
