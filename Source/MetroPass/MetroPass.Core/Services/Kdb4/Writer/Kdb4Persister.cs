using MetroPass.Core.Model;
using MetroPass.Core.Model.Kdb4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroPass.Core.Services.Kdb4.Writer
{
    public class Kdb4Persister
    {
        public void Persist(Kdb4Tree tree)
        {

        }

        public void UpdateGroup(PwGroup group)
        {
            var element = group.Element.Element("Name");
            
        }
    }
}
