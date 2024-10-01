using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using RebelsOpdrachtenAPI.Models;


namespace RebelsOpdrachtenAPI.Repositories
{
    public interface IItemRepository
    {
        Task<Item> GetItemAsync(string uuid);
        Task<int> GetTotalCountAsync();
        Task<(List<Item> Items, Dictionary<string, AttributeValue> LastEvaluatedKey)> GetItemsAsync(string lastEvaluatedKey = null, int limit = 9);
    }

    public class ItemRepository : IItemRepository
    {
        private readonly IAmazonDynamoDB _dynamoDBClient;

        public ItemRepository(IAmazonDynamoDB dynamoDBClient)
        {
            _dynamoDBClient = dynamoDBClient;
        }

        public async Task<Item> GetItemAsync(string uuid)
        {
            var context = new DynamoDBContext(_dynamoDBClient);
            return await context.LoadAsync<Item>(uuid);
        }

        public async Task<int> GetTotalCountAsync()
        {
            var request = new ScanRequest
            {
                TableName = "ICTRebelsOpdrachten",
                Select = "COUNT"
            };

            var response = await _dynamoDBClient.ScanAsync(request);
            return response.Count;
        }

        public async Task<(List<Item> Items, Dictionary<string, AttributeValue> LastEvaluatedKey)> GetItemsAsync(string lastEvaluatedKey = null, int limit = 9)
        {
            var request = new ScanRequest
            {
                TableName = "ICTRebelsOpdrachten",
                Limit = limit,
                ExclusiveStartKey = lastEvaluatedKey != null
                    ? new Dictionary<string, AttributeValue> { { "UUID", new AttributeValue { S = lastEvaluatedKey } } }
                    : null
            };

            var response = await _dynamoDBClient.ScanAsync(request);

            var items = new List<Item>();
            foreach (var item in response.Items)
            {
                items.Add(new Item
                {
                    UUID = item["UUID"].S,
                    Broker = item["Broker"].S,
                    Titel = item["Titel"].S,
                    Locatie = item["Locatie"].S,
                    Uren = item["Uren"].S,
                    Duur = item["Duur"].S,
                    Link = item["Link"].S
                });
            }

            return (items, response.LastEvaluatedKey);
        }
    }
}
