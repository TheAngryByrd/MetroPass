using MetroPass.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroPass.Core.Model
{
    public class PwCommon : BindableBase
    {


        private DateTime _creationDate;

        public DateTime CreationDate
        {
            get { return _creationDate; }
            set { SetProperty(ref _creationDate, value); }
        }
        private DateTime _lastModifiedDate;

        public DateTime LastModifiedDate
        {
            get { return _lastModifiedDate; }
            set { SetProperty(ref _lastModifiedDate, value); }
        }
        private DateTime _lastAccessTime;

        public DateTime LastAccessTime
        {
            get { return _lastAccessTime; }
            set { SetProperty(ref _lastAccessTime, value); }
        }
        private DateTime _expireTime;

        public DateTime ExpireTime
        {
            get { return _expireTime; }
            set { SetProperty(ref _expireTime, value); }
        }





    }
}
