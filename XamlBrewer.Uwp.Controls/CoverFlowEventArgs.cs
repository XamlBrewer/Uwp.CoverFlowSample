namespace XamlBrewer.Uwp.Controls
{
    using System;

    public class CoverFlowEventArgs : EventArgs
    {
        public int Index { get; set; }
        public object Item { get; set; }
    }
}
