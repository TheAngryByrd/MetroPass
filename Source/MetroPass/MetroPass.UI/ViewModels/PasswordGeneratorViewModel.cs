using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using MetroPass.Core.Interfaces;
using MetroPass.UI.ViewModels.Messages;

namespace MetroPass.UI.ViewModels
{
    public class PasswordGeneratorViewModel : Screen
    {
        private readonly IPasswordGenerator passwordGenerator;

        private readonly IEventAggregator events;

        public PasswordGeneratorViewModel(IPasswordGenerator passwordGenerator, IEventAggregator events)
        {
            this.events = events;
            this.passwordGenerator = passwordGenerator;
            this.DisplayName = "Generator";
            this.Length = 6;
        }


        private bool capitals;

        public bool CapitalSwitch
        {
            get { return capitals; }
            set 
            { 
                capitals = value;
                NotifyOfPropertyChange(() => CapitalSwitch);
            }
        }


        private bool lowers;

        public bool LowerSwitch
        {
            get { return lowers; }
            set
            {
                lowers = value;
                NotifyOfPropertyChange(() => LowerSwitch);
            }
        }

        private bool digits;

        public bool DigitSwitch
        {
            get { return digits; }
            set
            {
                digits = value;
                NotifyOfPropertyChange(() => DigitSwitch);
            }
        }

        private bool specials;

        public bool SpecialSwitch
        {
            get { return specials; }
            set
            {
                specials = value;
                NotifyOfPropertyChange(() => SpecialSwitch);
            }
        }  
        
        private bool underscores;

        public bool UnderscoreSwitch
        {
            get { return underscores; }
            set
            {
                underscores = value;
                NotifyOfPropertyChange(() => UnderscoreSwitch);
            }
        }  
        private bool spaces;

        public bool SpaceSwitch
        {
            get { return spaces; }
            set
            {
                spaces = value;
                NotifyOfPropertyChange(() => SpaceSwitch);
            }
        }  
        private bool minuses;

        public bool MinusSwitch
        {
            get { return minuses; }
            set
            {
                minuses = value;
                NotifyOfPropertyChange(() => MinusSwitch);
            }
        }  
        private bool brackets;

        public bool BracketSwitch
        {
            get { return brackets; }
            set
            {
                brackets = value;
                NotifyOfPropertyChange(() => BracketSwitch);
            }
        }  
        private int length;

        public int Length
        {
            get { return length; }
            set
            {
                length = value;
                NotifyOfPropertyChange(() => Length);
            }
        }  
          

        public void Generate()
        {
            events.Publish(new PasswordGenerateMessage
            {
                GeneratedPassword = "Hello"
            }); 
        }

    }
}
