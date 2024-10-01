using Amazon.DynamoDBv2.DataModel;

namespace RebelsOpdrachtenAPI.Models
{
    [DynamoDBTable("ICTRebelsOpdrachten")]
    public class Item
    {
        public string UUID { get; set; }
        public string Broker { get; set; }
        public string Titel { get; set; }
        public string Locatie { get; set; }
        public string Uren { get; set; }
        public string Duur { get; set; }
        public string Link { get; set; }
    }
}