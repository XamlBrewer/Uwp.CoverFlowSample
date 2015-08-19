using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace XamlBrewer.Uwp.Controls
{
    public delegate void SelectedItemChangedEvent(CoverFlowEventArgs e);

    public class CoverFlow : ItemsControl, INotifyPropertyChanged
    {
        public event SelectedItemChangedEvent SelectedItemChanged;

        private FrameworkElement LayoutRoot;
        private ItemsPresenter ItemsPresenter;
        private Dictionary<object, CoverFlowItem> _objectToItemContainer;
        private List<CoverFlowItem> items;

        private int selectedIndex;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                IndexSelected(value, false);
            }
        }

        private void IndexSelected(int index, bool mouseclick)
        {
            IndexSelected(index, mouseclick, true);
        }

        private void IndexSelected(int index, bool mouseclick, bool layoutChildren)
        {
            if (items.Count > 0)
            {
                selectedIndex = index;
                if (layoutChildren)
                    LayoutChildren();

                CoverFlowEventArgs e = new CoverFlowEventArgs() { Index = index, Item = items[index].Content, MouseClick = mouseclick };

                if (SelectedItemChanged != null)
                    SelectedItemChanged(e);
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectedIndex"));
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectedItem"));
                }
            }
        }

        public object SelectedItem
        {
            get
            {

                return items.Count > 0 ? items[SelectedIndex].Content : null;
            }
            set
            {
                CoverFlowItem o = GetItemContainerForObject(value);
                if (o != null)
                    SelectedIndex = items.IndexOf(o);
            }
        }

        #region SpaceBetweenItems (DependencyProperty) (l)

        public double SpaceBetweenItems
        {
            get { return (double)GetValue(SpaceBetweenItemsProperty); }
            set { SetValue(SpaceBetweenItemsProperty, value); }
        }
        public static readonly DependencyProperty SpaceBetweenItemsProperty =
            DependencyProperty.Register("SpaceBetweenItems", typeof(double), typeof(CoverFlow), new PropertyMetadata(60d, new PropertyChangedCallback(CoverFlow.OnValuesChanged)));

        #endregion

        #region SpaceBetweenSelectedItemAndItems (DependencyProperty) (k)

        public double SpaceBetweenSelectedItemAndItems
        {
            get { return (double)GetValue(SpaceBetweenSelectedItemAndItemsProperty); }
            set { SetValue(SpaceBetweenSelectedItemAndItemsProperty, value); }
        }
        public static readonly DependencyProperty SpaceBetweenSelectedItemAndItemsProperty =
            DependencyProperty.Register("SpaceBetweenSelectedItemAndItems", typeof(double), typeof(CoverFlow), new PropertyMetadata(140d, new PropertyChangedCallback(CoverFlow.OnValuesChanged)));

        #endregion

        #region RotationAngle (DependencyProperty) (r)

        public double RotationAngle
        {
            get { return (double)GetValue(RotationAngleProperty); }
            set { SetValue(RotationAngleProperty, value); }
        }
        public static readonly DependencyProperty RotationAngleProperty =
            DependencyProperty.Register("RotationAngle", typeof(double), typeof(CoverFlow), new PropertyMetadata(45d, new PropertyChangedCallback(CoverFlow.OnValuesChanged)));

        #endregion

        #region ZDistance (DependencyProperty) (z)

        public double ZDistance
        {
            get { return (double)GetValue(ZDistanceProperty); }
            set { SetValue(ZDistanceProperty, value); }
        }
        public static readonly DependencyProperty ZDistanceProperty =
            DependencyProperty.Register("ZDistance", typeof(double), typeof(CoverFlow), new PropertyMetadata(0.0d, new PropertyChangedCallback(CoverFlow.OnValuesChanged)));

        #endregion
        #region Scale (DependencyProperty) (s)

        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }
        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(CoverFlow), new PropertyMetadata(.7d, new PropertyChangedCallback(CoverFlow.OnValuesChanged)));

        #endregion

        #region SingleItemDuration (DependencyProperty)

        public Duration SingleItemDuration
        {
            get { return (Duration)GetValue(SingleItemDurationProperty); }
            set { SetValue(SingleItemDurationProperty, value); }
        }
        public static readonly DependencyProperty SingleItemDurationProperty =
            DependencyProperty.Register("SingleItemDuration", typeof(Duration), typeof(CoverFlow), null);

        #endregion

        #region SingleItemDuration (DependencyProperty)

        public Duration PageDuration
        {
            get { return (Duration)GetValue(PageDurationProperty); }
            set { SetValue(PageDurationProperty, value); }
        }
        public static readonly DependencyProperty PageDurationProperty =
            DependencyProperty.Register("PageDuration", typeof(Duration), typeof(CoverFlow), null);

        #endregion


        #region EasingFunction (DependencyProperty)

        public EasingFunctionBase EasingFunction
        {
            get { return (EasingFunctionBase)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }
        public static readonly DependencyProperty EasingFunctionProperty =
            DependencyProperty.Register("EasingFunction", typeof(EasingFunctionBase), typeof(CoverFlow), null);

        #endregion

        private double k { get { return SpaceBetweenSelectedItemAndItems; } }
        private double l { get { return SpaceBetweenItems; } }
        private double r { get { return RotationAngle; } }
        private double z { get { return ZDistance; } }
        private Duration duration;


        public CoverFlow()
        {
            DefaultStyleKey = typeof(CoverFlow);
            items = new List<CoverFlowItem>();
            SingleItemDuration = new Duration(TimeSpan.FromMilliseconds(600));
            PageDuration = new Duration(TimeSpan.FromMilliseconds(900));
            duration = SingleItemDuration;
            EasingFunction = new CubicEase();
        }

        #region ItemContainer methods

        protected CoverFlowItem GetItemContainerForObject(object value)
        {
            CoverFlowItem item = value as CoverFlowItem;
            if (item == null)
            {
                this.ObjectToItemContainer.TryGetValue(value, out item);
            }
            return item;
        }

        protected Dictionary<object, CoverFlowItem> ObjectToItemContainer
        {
            get
            {
                if (this._objectToItemContainer == null)
                {
                    this._objectToItemContainer = new Dictionary<object, CoverFlowItem>();
                }
                return this._objectToItemContainer;
            }
        }

        #endregion

        private static void OnValuesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CoverFlow).LayoutChildren();
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            CoverFlowItem item = new CoverFlowItem();
            return item;
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is CoverFlowItem);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            LayoutRoot = (FrameworkElement)GetTemplateChild("LayoutRoot");
            ItemsPresenter = (ItemsPresenter)GetTemplateChild("ItemsPresenter");
        }

        // O dear, US keyboard only (D A S W for arrows ?).
        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.VirtualKey == VirtualKey.Right || e.VirtualKey == VirtualKey.D)
            {
                NextItem();
                e.Handled = true;
            }
            else if (e.VirtualKey == VirtualKey.Left || e.VirtualKey == VirtualKey.A)
            {
                PreviousItem();
                e.Handled = true;
            }
            else if (e.VirtualKey == VirtualKey.PageDown || e.VirtualKey == VirtualKey.S || e.VirtualKey == VirtualKey.Down)
            {
                NextPage();
                e.Handled = true;
            }
            else if (e.VirtualKey == VirtualKey.PageUp || e.VirtualKey == VirtualKey.W || e.VirtualKey == VirtualKey.Up)
            {
                PreviousPage();
                e.Handled = true;
            }
            else if (e.VirtualKey == VirtualKey.Home || e.VirtualKey == VirtualKey.Q)
            {
                First();
                e.Handled = true;
            }
            else if (e.VirtualKey == VirtualKey.End || e.VirtualKey == VirtualKey.E)
            {
                Last();
                e.Handled = true;
            }
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            CoverFlowItem item2 = element as CoverFlowItem;
            if (item2 != item)
            {
                this.ObjectToItemContainer[item] = item2;
            }
            if (!items.Contains(item2))
            {
                items.Add(item2);
                item2.ItemSelected += new EventHandler(item2_ItemSelected);
                item2.SizeChanged += new SizeChangedEventHandler(item2_SizeChanged);
            }
            if (items.Count == 1)
                IndexSelected(0, false, false);
        }

        void item2_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CoverFlowItem item = sender as CoverFlowItem;
            int index = items.IndexOf(item);
            LayoutChild(item, index);
        }

        void item2_ItemSelected(object sender, EventArgs e)
        {
            CoverFlowItem item = sender as CoverFlowItem;
            if (item == null)
                return;
            int index = items.IndexOf(item);
            if (index >= 0)
                IndexSelected(index, true);
            //SelectedIndex = index;

        }
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);
            CoverFlowItem item2 = element as CoverFlowItem;
            if (item2 != item)
            {
                this.ObjectToItemContainer.Remove(item);
            }
            items.Remove(item2);
        }


        protected void LayoutChildren()
        {
            for (int i = 0; i < items.Count; i++)
            {
                LayoutChild(items[i], i);
            }
        }

        protected void LayoutChild(CoverFlowItem item, int index)
        {
            double m = ItemsPresenter.ActualWidth / 2;

            int b = index - SelectedIndex;
            double mu = 0;
            if (b < 0)
                mu = -1;
            else if (b > 0)
                mu = 1;
            double x = (m + ((double)b * l + (k * mu))) - (item.ActualWidth / 2);

            double s = mu == 0 ? 1 : Scale;

            int zindex = items.Count - Math.Abs(b);

            if (((x + item.ActualWidth) < 0 || x > ItemsPresenter.ActualWidth)
                && ((item.X + item.ActualWidth) < 0 || item.X > ItemsPresenter.ActualWidth)
                && !((x + item.ActualWidth) < 0 && item.X > ItemsPresenter.ActualWidth)
                && !((item.X + item.ActualWidth) < 0 && x > ItemsPresenter.ActualWidth))
            {
                item.SetValues(x, zindex, r * mu, z * Math.Abs(mu), s, duration, EasingFunction, false);
            }
            else
            {
                item.SetValues(x, zindex, r * mu, z * Math.Abs(mu), s, duration, EasingFunction, true);
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size size = base.ArrangeOverride(finalSize);
            RectangleGeometry visibleArea = new RectangleGeometry();
            Rect clip = new Rect(0, 0, ItemsPresenter.ActualWidth, ItemsPresenter.ActualHeight);
            foreach (CoverFlowItem item in items)
            {
                item.Height = ItemsPresenter.ActualHeight;
            }
            visibleArea.Rect = clip;
            ItemsPresenter.Clip = visibleArea;

            double m = ItemsPresenter.ActualWidth / 2;

            for (int index = 0; index < items.Count; index++)
            {
                CoverFlowItem item = items[index];
                int b = index - SelectedIndex;
                double mu = 0;
                if (b < 0)
                    mu = -1;
                else if (b > 0)
                    mu = 1;
                double x = (m + ((double)b * l + (k * mu))) - (item.ActualWidth / 2);

                double s = mu == 0 ? 1 : Scale;

                int zindex = items.Count - Math.Abs(b);

                item.X = x;
                item.YRotation = r * mu;
                item.ZOffset = z * Math.Abs(mu);
                item.Scale = s;
            }

            return size;
        }

        public void NextItem()
        {
            if (SelectedIndex < items.Count - 1)
            {
                duration = SingleItemDuration;
                SelectedIndex++;
            }
        }
        public void PreviousItem()
        {
            if (SelectedIndex > 0)
            {
                duration = SingleItemDuration;
                SelectedIndex--;
            }
        }

        public void NextPage()
        {
            if (SelectedIndex == items.Count - 1)
                return;

            duration = PageDuration;
            int i = GetPageCount();
            if (SelectedIndex + i >= items.Count)
                SelectedIndex = items.Count - 1;
            else
                SelectedIndex += i;
        }
        public void PreviousPage()
        {
            if (SelectedIndex == 0)
                return;
            duration = PageDuration;
            int i = GetPageCount();
            if (SelectedIndex - i < 0)
                SelectedIndex = 0;
            else
                SelectedIndex -= i;
        }

        protected int GetPageCount()
        {
            double m = ItemsPresenter.ActualWidth / 2;
            m -= k;
            return (int)(m / l);
        }

        public void First()
        {
            if (items.Count == 0)
                return;
            duration = PageDuration;
            SelectedIndex = 0;
        }
        public void Last()
        {
            if (items.Count == 0)
                return;
            duration = PageDuration;
            SelectedIndex = items.Count - 1;
        }

        public void UpdatePositions()
        {
            LayoutChildren();
        }

        public void UpdatePositions(object value)
        {
            CoverFlowItem item = GetItemContainerForObject(value);
            if (item == null)
                return;

            int index = items.IndexOf(item);
            LayoutChild(item, index);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
