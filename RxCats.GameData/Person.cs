using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RxCats.GameData
{
    [Table("person")]
    public class Person
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        
        [Column("name")]
        public string Name { get; set; }

        [Column("number")]
        public int? Number { get; set; }

        [Column("updated_dt")]
        public DateTime? UpdatedDate { get; set; }

        [Column("created_dt")]
        public DateTime? CreatedDate { get; set; }

        public override string ToString()
        {
            return "Person(Id=" + Id + ", Name=" + Name + ", Number=" + Number + ", UpdatedDate=" + UpdatedDate + ", CreatedDate=" + CreatedDate + ")";
        }
    }
}