using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace OverlayTest
{
    using System.Windows.Media;

    public class ComboboxItems
    {
        public List<ComboboxItem> ComboboxItemList { get; set; } = new List<ComboboxItem>()
        {
            new ComboboxItem("På", new SolidColorBrush(Brushes.OrangeRed.Color),true),
            new ComboboxItem("Av", new SolidColorBrush(Brushes.LightGreen.Color), false),
            new ComboboxItem("Avslutt", new SolidColorBrush(Brushes.LightGreen.Color), false)
        };
    }

    public class ComboboxItem
    {
        public ComboboxItem(string text, SolidColorBrush textColor, bool lightsOn)
        {
            this.TextColor = textColor;
            Text = text;
            this.LightsOn = lightsOn;
        }

        public bool LightsOn { get; set; }

        public SolidColorBrush TextColor { get; set; }

        public string Text { get; set; }
    }
}
