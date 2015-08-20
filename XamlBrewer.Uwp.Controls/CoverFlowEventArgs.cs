using System;

namespace XamlBrewer.Uwp.Controls
{
    public class CoverFlowEventArgs : EventArgs
    {
        public int Index { get; set; }
        public object Item { get; set; }
    }
}
