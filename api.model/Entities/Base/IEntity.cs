using System;
using System.Collections.Generic;
using System.Text;

namespace api.model.Entities.Base
{
    public interface IEntity<T>
    {
        T Id { get; set; }
    }
}
