using api.model.Entities.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.model.Entities.TemplateDbContext
{
    public class TemplateEntity : Entity<Int64>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override Int64 Id { get => base.Id; set => base.Id = value; }
        public string ServiceType { get; set; }
    }
}
