using BitDesktop.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace BitDesktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OptionsForm _optionsForm;
        private int _currBtcPrice { get; set; }
        private DispatcherTimer _updateBtcValueTimer;

        public MainWindow()
        {

            _updateBtcValueTimer = new DispatcherTimer();
            _updateBtcValueTimer.Tick += UpdateBtcValue;
            _updateBtcValueTimer.Interval = new TimeSpan(0, 2, 0);
            _updateBtcValueTimer.Start();
            UpdateBtcValue(null, null);
            InitializeComponent();
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
            => setPosition();

        private void imgOptions_Loaded(object sender, RoutedEventArgs e)
            => imgOptions.Visibility = Visibility.Hidden;

        private void imgExit_Loaded(object sender, RoutedEventArgs e)
            => imgExit.Visibility = Visibility.Hidden;

        private void MoveForm(object sender, MouseButtonEventArgs e)
        {
            if (!Settings.Default.Lock && Mouse.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void ShowOptions(object sender, MouseEventArgs e)
        {
            imgOptions.Visibility = Visibility.Visible;
            imgExit.Visibility = Visibility.Visible;
        }

        private void HideOptions(object sender, MouseEventArgs e)
        {
            imgOptions.Visibility = Visibility.Hidden;
            imgExit.Visibility = Visibility.Hidden;

            //save pos
            Settings.Default.Posx = window.Top;
            Settings.Default.Posy = window.Left;
            Settings.Default.Save();
        }

        private void OpenOptions(object sender, MouseButtonEventArgs e)
        {
            _optionsForm = new OptionsForm(this);
            _optionsForm.Show();
        }

        private void CloseApplication(object sender, MouseButtonEventArgs e)
            => this.Close();

        private void setPosition()
        {
            var posx = Settings.Default.Posx;
            var posy = Settings.Default.Posy;
            if (posx > 0 && posy > 0)
            {
                window.Top = posx;
                window.Left = posy;
            }
        }

        private void setRevenues()
        {
            if (Settings.Default.BtcPrice > 0 && Settings.Default.BtcAmount > 0)
            {
                if(lblBtcEarn.Visibility == Visibility.Hidden)
                    lblBtcEarn.Visibility = Visibility.Visible;

                char symbol = '+';
                lblBtcEarn.Foreground = Brushes.LimeGreen;

                var btcDiff = _currBtcPrice - Settings.Default.BtcPrice;
                var priceDiff = btcDiff * Settings.Default.BtcAmount + Settings.Default.BtcFixed;

                if (priceDiff < 0)
                {
                    symbol = '-';
                    lblBtcEarn.Foreground = Brushes.OrangeRed;
                }

                lblBtcEarn.Content = $"{symbol}${String.Format("{0:##,###}", Math.Abs(priceDiff))}";
            }
            else
                lblBtcEarn.Visibility = Visibility.Hidden;
        }

        public async void UpdateBtcValue(object sender, EventArgs e)
        {
            try
            {
                _currBtcPrice = await BtcDataFactory.GetBtcPrice(Settings.Default.BtcProvider);
                lblBtcPrice.Content = $"${String.Format("{0:##,###}", _currBtcPrice)}";
                setRevenues();
                rectBox.Stroke = Brushes.Black;
            }
            catch (WebException)
            {
                rectBox.Stroke = Brushes.Purple;
            }
            catch
            {
                rectBox.Stroke = Brushes.Red;
            }
        }
    }
}
