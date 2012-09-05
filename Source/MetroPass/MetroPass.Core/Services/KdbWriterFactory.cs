using MetroPass.Core.Interfaces;
using MetroPass.Core.Model.Kdb4;
using MetroPass.Core.Services.Kdb4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroPass.Core.Services
{
    public class KdbWriterFactory
    {
        public IKdbWriter CreateWriter(IKdbTree tree)
        {
            if (tree is Kdb4Tree)
            {
                return new Kdb4Writer();
            }
            else
            {
                throw new NotSupportedException();
            }
      
        }
    }
}
