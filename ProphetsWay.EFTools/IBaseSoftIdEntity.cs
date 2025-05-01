using System;
using ProphetsWay.BaseDataAccess;

namespace ProphetsWay.EFTools
{
    public interface IBaseSoftIdEntity<T> : IBaseIdEntity<T>
    {
        DateTime CreatedDate { get; set; }
        
        DateTime? UpdatedDate { get; set; }

        DateTime? DeletedDate { get; set; }

    }
}
