using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RS232Chat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly RS232PortWrapper port;
        public MainWindow()
        {
            InitializeComponent();

            port = new();
            port.MessageHandler += OnChatUpdate;

            chatHistoryWindow.Text = "RS232 Chat started. Type /help to get a list of available commands.\n";

            change.Visibility = Visibility.Hidden;

            cmbPort.ItemsSource = SerialPort.GetPortNames();
            cmbPort.SelectedItem = SerialPort.GetPortNames().First();
            cmbPort.SelectionChanged += OnSelectionChanged;

            cmbBaudRate.ItemsSource = RS232Constants.CommonBaudRates;
            cmbBaudRate.SelectedItem = RS232Constants.DefaultBaudRate;
            cmbBaudRate.SelectionChanged += OnSelectionChanged;

            cmbDataBits.ItemsSource = RS232Constants.CommonDataBits;
            cmbDataBits.SelectedItem = RS232Constants.DefaultDataBits;
            cmbDataBits.SelectionChanged += OnSelectionChanged;

            cmbParity.ItemsSource = RS232Constants.CommonParities;
            cmbParity.SelectedItem = RS232Constants.DefaultParity;
            cmbParity.SelectionChanged += OnSelectionChanged;

            cmbStopBits.ItemsSource = RS232Constants.CommonStopBits;
            cmbStopBits.SelectedItem = RS232Constants.DefaultStopBits;
            cmbStopBits.SelectionChanged += OnSelectionChanged;

            cmbTerminator.ItemsSource = RS232Constants.CommonMessageTerminatorNames;
            cmbTerminator.SelectedItem = RS232Constants.DefaultMessageTerminatorName;
            cmbTerminator.SelectionChanged += OnSelectionChanged;

            cmbFlow.ItemsSource = RS232Constants.CommonFlowControlNames;
            cmbFlow.SelectedItem = RS232Constants.DefaultFlowControlName;
            cmbFlow.SelectionChanged += OnSelectionChanged;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            change.Visibility = Visibility.Visible;
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                port.SendMessage(chatMessageWindow.Text);
                chatMessageWindow.Text = "";
            }
        }

        private void OnApplySettingsClicked(object sender, RoutedEventArgs e)
        {
            RS232PortSettings newSettings = new()
            {
                PortName = (string)cmbPort.SelectedItem,
                BaudRate = (int)cmbBaudRate.SelectedItem,
                DataBits = (int)cmbDataBits.SelectedItem,
                Parity = RS232Constants.CommonParities[cmbDataBits.SelectedIndex],
                StopBits = RS232Constants.CommonStopBits[cmbStopBits.SelectedIndex],
                MessageTerminator = (MessageTerminator)cmbTerminator.SelectedIndex,
                FlowControl = (FlowControl)cmbFlow.SelectedIndex,
            };
            port.ApplySettings(newSettings);
            change.Visibility = Visibility.Hidden;
        }

        private void OnChatUpdate(object? sender, RS232PortWrapperEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (e.ChatMessage! != "<clear>") chatHistoryWindow.Text += e.ChatMessage! + "\n";
                else chatHistoryWindow.Text = "";
            });
        }
    }
}
