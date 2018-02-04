using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.ApiModels
{
    [Serializable]
    public class BaseApiResponse
    {
        public string message;
        public int code;
    }
}
