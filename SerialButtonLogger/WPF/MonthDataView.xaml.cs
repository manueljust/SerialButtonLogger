using SerialButtonLogger.Util;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace SerialButtonLogger.WPF
{
    /// <summary>
    /// Interaction logic for MonthDataView.xaml
    /// </summary>
    public partial class MonthDataView : UserControl, INotifyPropertyChanged
    {
        public MonthData MonthData { get; private set; }
        public FlowDocument DayDataViewSource { get; private set; } = new FlowDocument() { IsOptimalParagraphEnabled=true, PagePadding=new Thickness(0) };

        public event PropertyChangedEventHandler PropertyChanged;

        public MonthDataView(MonthData monthData)
        {
            MonthData = monthData;
            DayDataViewSource.FontFamily = this.FontFamily;

            foreach (DayData d in MonthData.DayData)
            {
                d.Stamps.CollectionChanged += (o, e) => UpdateTable();
            }

            UpdateTable();

            InitializeComponent();

            MonthDataViewer.Initialized += AutoScroll;
        }

        private void AutoScroll(object sender, EventArgs e)
        {
            // scroll to this week for the current month
            if (DateTime.Now.Year == MonthData.Year && DateTime.Now.Month == MonthData.Month && DayDataViewSource.PageWidth > MonthDataViewer.ActualWidth)
            {
                int thisSunday = DateTime.Now.Day - (int)DateTime.Now.DayOfWeek;

                ScrollViewer scrollViewer = (ScrollViewer)MonthDataViewer.Template.FindName("PART_ContentHost", MonthDataViewer);
                scrollViewer.ScrollToHorizontalOffset(Math.Max(thisSunday - 1, 0) * DayDataViewSource.PageWidth / DateTime.DaysInMonth(MonthData.Year, MonthData.Month));
            }
        }

        private void UpdateTable()
        {
            int columnCount = DateTime.DaysInMonth(MonthData.Year, MonthData.Month);

            // create table rows and add title cell
            TableRow dateRow = new TableRow();
            TableRow hoursRow = new TableRow();
            TableRow stampsRow = new TableRow();

            // add data
            for (int i = 0; i < columnCount; i++)
            {
                dateRow.Cells.Add(new TableCell(new Paragraph(new Run(string.Format("{0:D2}.{1:D2}", i+1, MonthData.Month)))));
                hoursRow.Cells.Add(new TableCell(new Paragraph(new Run(string.Format("{0:0.#####}", MonthData.DayData[i].TotalHours)))));
                string stamps = 0 == MonthData.DayData[i].Stamps.Count % 2 ? string.Empty : "Stamps Odd!" + Environment.NewLine;
                stampsRow.Cells.Add(new TableCell(new Paragraph(new Run(stamps + string.Join(Environment.NewLine, MonthData.DayData[i].Stamps.Select(s => s.ToString("HH:mm:ss")))))));
            }

            // consolidate rows in a group
            TableRowGroup rowGroup = new TableRowGroup();
            rowGroup.Rows.Add(dateRow);
            rowGroup.Rows.Add(hoursRow);
            rowGroup.Rows.Add(stampsRow);

            // add rowgroup to table
            Table table = new Table();
            table.RowGroups.Add(rowGroup);

            double totalWidth = table.CellSpacing;

            // style the columns for width and background depending on day of week
            for (int i = 0; i < columnCount ; i++)
            {
                double width = rowGroup.Rows.Max(r => r.Cells[i].GetDesiredWidth()) + 25;
                totalWidth += table.CellSpacing + width;

                table.Columns.Add(new TableColumn() { Width = new GridLength(width) });

                // Set blue background on weekends.
                switch ((new DateTime(MonthData.Year, MonthData.Month, i+1)).DayOfWeek)
                {
                    case DayOfWeek.Saturday:
                    case DayOfWeek.Sunday:
                        rowGroup.Rows[0].Cells[i].Background = Brushes.LightSteelBlue;
                        rowGroup.Rows[1].Cells[i].Background = Brushes.LightSteelBlue;
                        break;
                }
            }

            // update the table in the flowdocument blocks
            DayDataViewSource.Blocks.Clear();
            DayDataViewSource.Blocks.Add(table);
            DayDataViewSource.PageWidth = totalWidth;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DayDataViewSource)));
        }
    }
}
