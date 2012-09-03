using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroPass.Core.Model
{
    public class PwCommon
    {
        public DateTime CreationDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public DateTime LastAccessTime { get; set; }
        public DateTime ExpireTime { get; set; }
    }
}
