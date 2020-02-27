using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.model.Entities.Base
{
    public abstract class Entity<T> : IEntity<T>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // This can be overridden in derived entity using annotations.
        public virtual T Id
        {
            get { return _id; }
            set { _id = (T)value; }
        }
        private T _id;
    }
}
