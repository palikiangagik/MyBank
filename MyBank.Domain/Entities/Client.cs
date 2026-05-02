using MyBank.Domain.ValueObjects;

namespace MyBank.Domain.Entities
{
    public class Client
    {
        public IntId Id { get; private set; }
        public ClientName Name { get; private set; }

        public Client(IntId id, ClientName name)
        {
            Id = id;
            Name = name;
        }
    }
}
