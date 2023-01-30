using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatoshiDice.Application.Common.Interfaces
{
    public interface ICacheService
    {
        Task<IEnumerable<T>> GetData<T>(string key);
        Task<T> GetDataById<T>(string key, string id);
        void SetData<T>(string key, T value, string id);
        Task<object> RemoveData(string key);
    }
}
