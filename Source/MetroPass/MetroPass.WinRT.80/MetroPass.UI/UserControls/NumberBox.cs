using System;
using System.Linq;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace MetroPass.UI.UserControls
{
    public class NumberBox :TextBox
    {
        #region Constructors
        /*
         * The default constructor
         */
        public NumberBox()
        {
            this.InputScope = new InputScope();
            this.InputScope.Names.Add(new InputScopeName(InputScopeNameValue.Number));
            TextChanged += new TextChangedEventHandler(OnTextChanged);
            KeyDown += new KeyEventHandler(OnKeyDown);
        }
        #endregion
 
        #region Properties
     
        new public String Text
        {
            get { return base.Text; }
            set
            {
                base.Text = LeaveOnlyNumbers(value);
            }
        }
 
        #endregion
 
        #region Functions
        private bool IsNumberKey(VirtualKey inKey)
        {
            if (inKey < VirtualKey.Number0 || inKey > VirtualKey.Number9 && inKey < VirtualKey.NumberPad0 || inKey > VirtualKey.NumberPad9)
            {
                return false;
            }
            return true;
        }

        private bool IsDelOrBackspaceOrTabKey(VirtualKey inKey)
        {
            return inKey == VirtualKey.Delete || inKey == VirtualKey.Back || inKey == VirtualKey.Tab;
        }
 
        private string LeaveOnlyNumbers(String inString)
        {
            String tmp = inString;
            foreach (char c in inString.ToCharArray())
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(c.ToString(), "^[0-9]*$"))
                {
                    tmp = tmp.Replace(c.ToString(), "");
                }
            }
            return tmp;
        }
        #endregion
 
        #region Event Functions
        protected void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            e.Handled = !IsNumberKey(e.Key) && !IsDelOrBackspaceOrTabKey(e.Key);
        }
       
 
        protected void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            base.Text = LeaveOnlyNumbers(Text);
        }
        #endregion
    }
}
