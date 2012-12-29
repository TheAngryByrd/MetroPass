using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using MetroPass.Core.Interfaces;
using MetroPass.Core.Security;
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

        public bool UppercaseSwitch
        {
            get { return capitals; }
            set 
            { 
                capitals = value;
                NotifyOfPropertyChange(() => UppercaseSwitch);
            }
        }


        private bool lowers;

        public bool LowercaseSwitch
        {
            get { return lowers; }
            set
            {
                lowers = value;
                NotifyOfPropertyChange(() => LowercaseSwitch);
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
          

        public async void Generate()
        {
          
            var onSwitches = this.GetType().GetRuntimeProperties().Where(p => p.Name.Contains("Switch") && (bool)p.GetValue(this) == true);
            List<string> characterSets = new List<string>();
            foreach (var item in onSwitches)
            {

                characterSets.Add(PasswordGeneratorCharacterSets.CharacterMap[item.Name.Replace("Switch", "")]);
            }

            events.Publish(new PasswordGenerateMessage
            {
                GeneratedPassword = "Hello"
            }); 
        }

    }
}
