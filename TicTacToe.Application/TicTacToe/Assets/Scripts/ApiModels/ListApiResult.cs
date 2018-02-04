using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.ApiModels
{
    [Serializable]
    public class ListApiResult<T> : BaseApiResponse
    {
        public IEnumerable<T> result;
    }
}
