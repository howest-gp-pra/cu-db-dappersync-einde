using System;
using Dapper.Contrib.Extensions;

namespace Pra.Bibliotheek.Core.Entities
{
    [Table ("Author")]
    public class Author
    {
        [ExplicitKey]
        public string ID { get; }
        public string Name { get; set; }
        public Author()
        {
            ID = Guid.NewGuid().ToString();
        }
        public Author(string name) : this()
        {
            Name = name;
        }
        public Author(string id, string name)
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
