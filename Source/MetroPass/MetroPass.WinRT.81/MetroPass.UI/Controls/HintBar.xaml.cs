using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace WpWinNl.Controls
{
  public sealed partial class HintBar : UserControl
  {
    public HintBar()
    {
      InitializeComponent();
    }



    private void HintBarGridTapped(object sender, TappedRoutedEventArgs e)
    {
      var commandBar = GetAssociatedCommandBar(this);
      if (commandBar != null)
      {
        commandBar.IsOpen = true;
      }
    }

          #region Attached Dependency Property AssociatedCommandBar
#if W8
      public static readonly DependencyProperty AssociatedCommandBarProperty =
         DependencyProperty.RegisterAttached("AssociatedCommandBar",
         typeof(AppBar),
         typeof(HintBar),
         new PropertyMetadata(default(AppBar)));

      public static AppBar GetAssociatedCommandBar(DependencyObject obj)
    {
      return obj.GetValue(AssociatedCommandBarProperty) as AppBar;
    }

    // Called when Property is set
    public static void SetAssociatedCommandBar(
       DependencyObject obj,
       AppBar value)
    {
      obj.SetValue(AssociatedCommandBarProperty, value);
    }

#else

    public static readonly DependencyProperty AssociatedCommandBarProperty =
         DependencyProperty.RegisterAttached("AssociatedCommandBar",
         typeof(CommandBar),
         typeof(HintBar),
         new PropertyMetadata(default(CommandBar)));

    // Called when Property is retrieved
    public static CommandBar GetAssociatedCommandBar(DependencyObject obj)
    {
      return obj.GetValue(AssociatedCommandBarProperty) as CommandBar;
    }

    // Called when Property is set
    public static void SetAssociatedCommandBar(
       DependencyObject obj,
       CommandBar value)
    {
      obj.SetValue(AssociatedCommandBarProperty, value);
    }





#endif

          #endregion

    //#region Attached Dependency Property ForeGround
    //public static readonly DependencyProperty ForeGroundProperty =
    //     DependencyProperty.RegisterAttached("ForeGround",
    //     typeof(Brush),
    //     typeof(HintBar),
    //     new PropertyMetadata(default(Brush), ForeGroundChanged));

    //// Called when Property is retrieved
    //public static Brush GetForeGround(DependencyObject obj)
    //{
    //  return obj.GetValue(ForeGroundProperty) as Brush;
    //}

    //// Called when Property is set
    //public static void SetForeGround(
    //   DependencyObject obj,
    //   Brush value)
    //{
    //  obj.SetValue(ForeGroundProperty, value);
    //}

    //// Called when property is changed
    //private static void ForeGroundChanged(
    // object sender,
    // DependencyPropertyChangedEventArgs args)
    //{
    //  var thisObject = sender as HintBar;
    //  if (thisObject != null)
    //  {
    //    thisObject.HintBarText.Foreground = args.NewValue as Brush;
    //  }
    //}
    //#endregion

  }
}
