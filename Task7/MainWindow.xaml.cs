using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Data;
using SciChart.Charting.Model.DataSeries;
using SciChart.Data.Model;
using Microsoft.Win32;
using System.Windows.Threading;

namespace Task7
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OhlcDataSeries<DateTime, double> ohlcSeries;
        private XyDataSeries<DateTime, double> dataSeries;
        private Monitor Monitor { get; set; }
        private int candleCount = 0;
        public MainWindow()
        {
            InitializeComponent();
            ReloadChartSeries();
            StockChart.XAxis.AutoRange = SciChart.Charting.Visuals.Axes.AutoRange.Once;
            indicatorChar.XAxis.AutoRange = SciChart.Charting.Visuals.Axes.AutoRange.Once;
            // indicatorChar.XAxes = SciChart.Charting.Visuals.Axes.AutoRange.Once;
            LoadDataSource(new Reporter(new XLSXLoader(@"C:\Users\Kat\Desktop\2.1\prices.xlsx")));
            this.Loaded += OnLoaded;
        }
        private void LoadDataSource(Reporter reporter)
        {
            if (this.Monitor != null)
            {
                Monitor.Dispose();
                Monitor = null;
            }

            this.Monitor = new Monitor(reporter, new ChaikinOscellator());
            this.Monitor.NewCandle += Monitor_NewCandle;
            ReloadChartSeries();
            this.Monitor.Run();
        }

        private void Monitor_NewCandle(Candle candle, decimal indicator)
        {
            StockChart.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { AddNewCandle(candle, indicator); }));
            //indicatorChar.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => { AddNewPoint(candle, indicator); }));
        }

        private void AddNewCandle(Candle candle, decimal indicator)
        {
            using (dataSeries.SuspendUpdates())
            using (ohlcSeries.SuspendUpdates())
            {

                ohlcSeries.Append(candle.Time, (double)candle.Open, (double)candle.High, (double)candle.Low, (double)candle.Close);
                dataSeries.Append(candle.Time, (double)indicator);
                candleCount++;

                StockChart.XAxis.VisibleRange = new IndexRange(candleCount - 50, candleCount);


                // DateTime d = candle.Time;
                indicatorChar.XAxis.VisibleRange = new IndexRange(candleCount - 50, candleCount);

            }
        }
        private void AddNewPoint(Candle candle, decimal value)
        {
            using (dataSeries.SuspendUpdates())
            {
                dataSeries.Append(candle.Time, (double)value);
                // DateTime d = candle.Time;
                indicatorChar.XAxis.VisibleRange = new IndexRange(candleCount - 50, candleCount);
            }
        }
        private void ReloadChartSeries()
        {
            ohlcSeries = new OhlcDataSeries<DateTime, double>() { SeriesName = "Candles", FifoCapacity = 10000 };
            //easeSeries = new XyDataSeries<DateTime, decimal>() { SeriesName = "Ease of movement", FifoCapacity = 10000 };
            dataSeries = new XyDataSeries<DateTime, double> { SeriesName = "Chaikin Oscellator", FifoCapacity = 10000 };
            CandleSeries.DataSeries = ohlcSeries;
            LineSeries.DataSeries = dataSeries;
            //LineSeries.DataContext = dataSeries;
        }

        private void MainMenu_FileOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XLSX files (*.xlsx)|*.xlsx|JSON files (*.json)|*.json";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string extension = System.IO.Path.GetExtension(openFileDialog.FileName);

                    if (extension == ".xlsx")
                        LoadDataSource(new Reporter(new XLSXLoader(openFileDialog.FileName)));
                    else if (extension == ".json")
                        LoadDataSource(new Reporter(new JSONLoader(openFileDialog.FileName)));
                    else
                        throw new Exception();
                    dataSeries.Clear();

                    ohlcSeries.Clear();
                    StockChart.Annotations.Clear();
                    indicatorChar.Annotations.Clear();
                    ReloadChartSeries();
                    //LineSeries
                }
                catch (Exception er)
                {
                    MessageBox.Show("Произошла ошибка загрузки данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
        }
    }
}
