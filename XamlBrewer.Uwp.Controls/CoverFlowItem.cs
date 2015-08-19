namespace XamlBrewer.Uwp.Controls
{
    using System;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Animation;

    public class CoverFlowItem : ContentControl
    {
        public event EventHandler ItemSelected;

        private FrameworkElement LayoutRoot;
        private PlaneProjection planeProjection;
        private Storyboard Animation;
        private ScaleTransform scaleTransform;
        private EasingDoubleKeyFrame rotationKeyFrame, offestZKeyFrame, scaleXKeyFrame, scaleYKeyFrame;

        private double yRotation;
        public double YRotation
        {
            get
            {
                return yRotation;
            }
            set
            {
                yRotation = value;
                if (planeProjection != null)
                {
                    planeProjection.RotationY = value;
                }
            }
        }

        private double zOffset;
        public double ZOffset
        {
            get
            {
                return zOffset;
            }
            set
            {
                zOffset = value;
                if (planeProjection != null)
                {
                    planeProjection.LocalOffsetZ = value;
                }
            }
        }

        private double scale;
        public double Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
                if (scaleTransform != null)
                {
                    scaleTransform.ScaleX = scale;
                    scaleTransform.ScaleY = scale;
                }
            }
        }

        private Duration duration;
        private EasingFunctionBase easingFunction;

        private DoubleAnimation xAnimation;

        private double x;
        public double X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
                Canvas.SetLeft(this, value);
            }
        }

        private bool isAnimating;


        private ContentControl ContentPresenter; //ContentPresenter

        public CoverFlowItem()
        {
            DefaultStyleKey = typeof(CoverFlowItem);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ContentPresenter = (ContentControl)GetTemplateChild("ContentPresenter");
            planeProjection = (PlaneProjection)GetTemplateChild("Rotator");
            LayoutRoot = (FrameworkElement)GetTemplateChild("LayoutRoot");

            Animation = (Storyboard)GetTemplateChild("Animation");
            Animation.Completed += Animation_Completed;
            rotationKeyFrame = (EasingDoubleKeyFrame)GetTemplateChild("rotationKeyFrame");
            offestZKeyFrame = (EasingDoubleKeyFrame)GetTemplateChild("offestZKeyFrame");
            scaleXKeyFrame = (EasingDoubleKeyFrame)GetTemplateChild("scaleXKeyFrame");
            scaleYKeyFrame = (EasingDoubleKeyFrame)GetTemplateChild("scaleYKeyFrame");
            scaleTransform = (ScaleTransform)GetTemplateChild("scaleTransform");

            planeProjection.RotationY = yRotation;
            planeProjection.LocalOffsetZ = zOffset;
            if (ContentPresenter != null)
            {
                ContentPresenter.Tapped += ContentPresenter_Tapped;
            }

            if (Animation != null)
            {
                xAnimation = new DoubleAnimation();
                Animation.Children.Add(xAnimation);

                Storyboard.SetTarget(xAnimation, this);
                Storyboard.SetTargetProperty(xAnimation, "(Canvas.Left)");
            }
        }

        void Animation_Completed(object sender, object e)
        {
            isAnimating = false;
        }

        void ContentPresenter_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (ItemSelected != null)
                ItemSelected(this, null);
        }

        public void SetValues(double x, int zIndex, double r, double z, double s, Duration d, EasingFunctionBase ease, bool useAnimation)
        {
            if (useAnimation)
            {
                if (!isAnimating && Canvas.GetLeft(this) != x)
                    Canvas.SetLeft(this, this.x);

                rotationKeyFrame.Value = r;
                offestZKeyFrame.Value = z;
                scaleYKeyFrame.Value = s;
                scaleXKeyFrame.Value = s;
                xAnimation.To = x;

                if (duration != d)
                {
                    duration = d;
                    rotationKeyFrame.KeyTime = KeyTime.FromTimeSpan(d.TimeSpan);
                    offestZKeyFrame.KeyTime = KeyTime.FromTimeSpan(d.TimeSpan);
                    scaleYKeyFrame.KeyTime = KeyTime.FromTimeSpan(d.TimeSpan);
                    scaleXKeyFrame.KeyTime = KeyTime.FromTimeSpan(d.TimeSpan);
                    xAnimation.Duration = d;
                }
                if (easingFunction != ease)
                {
                    easingFunction = ease;
                    rotationKeyFrame.EasingFunction = ease;
                    offestZKeyFrame.EasingFunction = ease;
                    scaleYKeyFrame.EasingFunction = ease;
                    scaleXKeyFrame.EasingFunction = ease;
                    xAnimation.EasingFunction = ease;
                }

                isAnimating = true;
                Animation.Begin();
                Canvas.SetZIndex(this, zIndex);
            }

            this.x = x;
        }
    }
}
