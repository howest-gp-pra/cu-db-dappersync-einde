using System;
using Dapper.Contrib.Extensions;

namespace Pra.Bibliotheek.Core.Entities
{
    [Table("Publisher")]
    public class Publisher
    {
        [ExplicitKey]
        public string ID { get; }
        public string Name { get; set; }

        public Publisher(string name)
        {
            ID = Guid.NewGuid().ToString();
            Name = name;
        }
        public Publisher(string id, string name)
        {
            ID = id;
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
