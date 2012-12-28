using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using MetroPass.Core.Interfaces;

namespace MetroPass.UI.ViewModels
{
    public class PasswordGeneratorViewModel : Screen
    {
        private readonly IPasswordGenerator passwordGenerator;

        public PasswordGeneratorViewModel(IPasswordGenerator passwordGenerator)
        {
            this.passwordGenerator = passwordGenerator;
            this.DisplayName = "Generator";
        }


    }
}
