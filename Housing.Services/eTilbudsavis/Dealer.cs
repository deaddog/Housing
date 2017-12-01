namespace Housing.Services.eTilbudsavis
{
    public class Dealer
    {
        public Dealer(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; }
        public string Name { get; }
    }
}
