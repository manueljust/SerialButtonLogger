using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace SerialButtonLogger
{
    public static class TableCellExtensions
    {
        public static double GetDesiredWidth(this TableCell cell)
        {
            TextRange textRange = new TextRange(cell.ContentStart, cell.ContentEnd);
            return new FormattedText(
                textRange.Text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(
                    textRange.GetPropertyValue(TextElement.FontFamilyProperty) as FontFamily,
                    (FontStyle)textRange.GetPropertyValue(TextElement.FontStyleProperty),
                    (FontWeight)textRange.GetPropertyValue(TextElement.FontWeightProperty),
                    FontStretches.Normal),
                    (double)textRange.GetPropertyValue(TextElement.FontSizeProperty),
                Brushes.Black,
                null,
                TextFormattingMode.Display).Width;
        }
    }
}
