using MetroPass.Core.Interfaces;
using MetroPass.Core.Services.Kdb4;
using MetroPass.Core.Services.Kdb4.Writer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Metropass.Core.PCL.Model.Kdb4;
using Metropass.Core.PCL.Model.Kdb4.Writer;

namespace MetroPass.Core.Services
{
    public class KdbWriterFactory
    {
        public IKdbWriter CreateWriter(IKdbTree tree)
        {
            if (tree is Kdb4Tree)
            {
                return new Kdb4Writer(new Kdb4HeaderWriter());
            }
            else
            {
                throw new NotSupportedException();
            }
      
        }
    }
}
