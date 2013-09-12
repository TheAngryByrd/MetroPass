using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Caliburn.Micro;
using MetroPass.UI.ViewModels.Messages;
using Metropass.Core.PCL.PasswordGeneration;

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
                NotifyOfPropertyChange(() => CanGenerate);
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
                NotifyOfPropertyChange(() => CanGenerate);
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
                NotifyOfPropertyChange(() => CanGenerate);
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
                NotifyOfPropertyChange(() => CanGenerate);
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
                NotifyOfPropertyChange(() => CanGenerate);
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
                NotifyOfPropertyChange(() => CanGenerate);
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
                NotifyOfPropertyChange(() => CanGenerate);
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
                NotifyOfPropertyChange(() => CanGenerate);
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
                NotifyOfPropertyChange(() => CanGenerate);
            }
        }

        public bool CanGenerate
        {
            get
            {
                return OnSwitches().Count() > 0;
            }
        }
        

        public async void Generate()
        {
            var onSwitches = OnSwitches();
            List<string> characterSets = MapSwitchesToCharacterSets(onSwitches);

            var password = await GeneratePassword(characterSets);

            SendMessageToEntryEditScreen(password); 
        }

        public async Task<string> GeneratePassword(List<string> characterSets)
        {
            var password = await passwordGenerator.GeneratePasswordAsync(Length, characterSets.ToArray());
            return password;
        }
  
        public void SendMessageToEntryEditScreen(string password)
        {
            events.Publish(new PasswordGenerateMessage
            {
                GeneratedPassword = password
            });
        }
  
        public List<string> MapSwitchesToCharacterSets(IEnumerable<PropertyInfo> onSwitches)
        {
            List<string> characterSets = new List<string>();
            foreach (var item in onSwitches)
            {
                characterSets.Add(PasswordGeneratorCharacterSets.CharacterMap[item.Name.Replace("Switch", "")]);
            }
            return characterSets;
        }
  
        public IEnumerable<PropertyInfo> OnSwitches()
        {
            var onSwitches = this.GetType().GetRuntimeProperties().Where(p => p.Name.Contains("Switch") && (bool)p.GetValue(this) == true);
            return onSwitches;
        }

    }
}
