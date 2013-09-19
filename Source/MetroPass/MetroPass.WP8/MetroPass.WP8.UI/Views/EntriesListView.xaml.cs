using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Microsoft.Phone.Controls;
using Telerik.Windows.Controls;

namespace MetroPass.WP8.UI.Views
{
    public class SequentialAnimator
    {
        private List<RadAnimation> animations = new List<RadAnimation>();

        public int[] AnimationSequenceIndices
        {
            get;
            set;
        }

        public bool InvertAnimationSequence
        {
            get;
            set;
        }

        public double TimeDelta
        {
            get;
            set;
        }

        public IList<UIElement> AnimationTargets
        {
            get;
            set;
        }

        public Func<RadAnimation> AnimationGenerator
        {
            get;
            set;
        }

        public void PlayAnimation()
        {
            if (this.AnimationTargets == null)
            {
                return;
            }

            this.animations.Clear();
            this.FillAnimationList();

            for (int i = 0, j = this.InitAnimationLoopCounter(); i < this.AnimationTargets.Count; ++i, this.UpdateAnimationCounter(ref j))
            {
                RadAnimationManager.Play(this.AnimationTargets[this.GetSequenceIndex(i)], this.animations[j]);
            }
        }

        protected virtual RadAnimation DefaultAnimationGenerator()
        {
            return new RadFadeAnimation();
        }

        private int GetSequenceIndex(int actualIndex)
        {
            if (this.AnimationSequenceIndices == null)
            {
                return actualIndex;
            }

            if (this.AnimationSequenceIndices.Length != this.AnimationTargets.Count)
            {
                return actualIndex;
            }

            return this.AnimationSequenceIndices[actualIndex];
        }

        private int InitAnimationLoopCounter()
        {
            if (this.InvertAnimationSequence)
            {
                return this.animations.Count - 1;
            }

            return 0;
        }

        private void UpdateAnimationCounter(ref int counter)
        {
            if (this.InvertAnimationSequence)
            {
                counter--;
            }
            else
            {
                counter++;
            }
        }

        private Func<RadAnimation> GetAnimationGenerator()
        {
            if (this.AnimationGenerator == null)
            {
                return this.DefaultAnimationGenerator;
            }
            else
            {
                return this.AnimationGenerator;
            }
        }

        private void FillAnimationList()
        {
            if (this.AnimationTargets == null)
            {
                return;
            }

            Func<RadAnimation> generator = this.GetAnimationGenerator();

            for (int i = 0; i < this.AnimationTargets.Count; ++i)
            {
                RadAnimation animation = generator();
                animation.InitialDelay = TimeSpan.FromSeconds(i * this.TimeDelta);
                this.animations.Add(animation);
            }
        }
    }

    public partial class EntriesListView : PhoneApplicationPage
    {
        private SequentialAnimator animator;

        public EntriesListView()
        {
            InitializeComponent();
            
            Loaded += EntriesListView_Loaded;
        }

        void EntriesListView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Task.Factory.RunOnUI(async () =>
            {
                //Animate();
            });
        }
  
        private void Animate()
        {
            animator = new SequentialAnimator();
            var itemsToAnimate = Items.ChildrenOfType<Grid>().Cast<UIElement>().ToList();
            itemsToAnimate.ForEach(i => i.Opacity = 0);
            animator.AnimationTargets = itemsToAnimate;
            animator.AnimationSequenceIndices = Enumerable.Range(0, itemsToAnimate.Count() - 1).ToArray();
            animator.TimeDelta = .06;

            RadFadeAnimation fadePrototype = new RadFadeAnimation();
            fadePrototype.Duration = new Duration(TimeSpan.FromSeconds(0.001));
            var group = new RadAnimationGroup();
            group.Children.Add(fadePrototype.Clone());
            group.Children.Add(new RadPlaneProjectionAnimation() { Easing = new ExponentialEase() { Exponent = 3, EasingMode = EasingMode.EaseIn }, CenterX = -2 });
            animator.AnimationGenerator = () => group.Clone();
            
            animator.PlayAnimation();
        }
    }

    public static class TaskEx
    {
        public static Task RunOnUI(this TaskFactory taskfactory, Action action)
        {
            return taskfactory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
