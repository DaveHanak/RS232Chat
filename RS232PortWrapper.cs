using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Interop;

namespace RS232Chat
{
    public class RS232PortWrapper
    {
        public event EventHandler<RS232PortWrapperEventArgs> MessageHandler;

        private readonly SerialPort port;
        private readonly System.Timers.Timer sendTimer;
        private readonly System.Timers.Timer pingTimer;
        private readonly Stopwatch pongStopwatch;

        private RS232PortSettings portSettings;
        private bool dsrPending = false;
        private bool pongPending = false;

        public RS232PortWrapper()
        {
            port = new()
            {
                Encoding = Encoding.ASCII
            };
            port.DataReceived += OnDataReceived;

            sendTimer = new()
            {
                Interval = RS232Constants.DefaultTimerInterval
            };
            sendTimer.Elapsed += OnSendTimeout;

            pingTimer = new()
            {
                Interval = RS232Constants.DefaultTimerInterval
            };
            pingTimer.Elapsed += OnPongTimeout;

            pongStopwatch = new();

            ApplySettings(new());
        }

        public void ApplySettings(RS232PortSettings settings)
        {
            portSettings = settings;
            ThreadPool.QueueUserWorkItem(AntiDeadlockApplySettings);
        }

        private void AntiDeadlockApplySettings(object? stateInfo)
        {
            try
            {
                HandleMessage("<info> Configuring port " + portSettings.PortName + "...");
                if (port.IsOpen)
                {
                    port.Close();

                    // https://learn.microsoft.com/en-us/dotnet/api/system.io.ports.serialport.close?view=dotnet-plat-ext-7.0#remarks
                    Thread.Sleep(1000);
                }

                port.PortName = portSettings.PortName;
                port.BaudRate = portSettings.BaudRate;
                port.DataBits = portSettings.DataBits;
                port.Parity = portSettings.Parity;
                port.StopBits = portSettings.StopBits;
                switch (portSettings.FlowControl)
                {
                    case FlowControl.None:
                        port.Handshake = Handshake.None;
                        break;
                    case FlowControl.RTS_CTS:
                        port.Handshake = Handshake.RequestToSend;
                        break;
                    case FlowControl.DTR_DSR:
                        port.Handshake = Handshake.None;
                        port.DtrEnable = true;
                        break;
                    case FlowControl.XON_XOFF:
                        port.Handshake = Handshake.XOnXOff;
                        break;
                }
                switch (portSettings.MessageTerminator)
                {
                    case MessageTerminator.None:
                        port.NewLine = "\0";
                        break;
                    case MessageTerminator.CR:
                        port.NewLine = "\r";
                        break;
                    case MessageTerminator.LF:
                        port.NewLine = "\n";
                        break;
                    case MessageTerminator.CR_LF:
                        port.NewLine = "\r\n";
                        break;
                    case MessageTerminator.UserDefined:
                        port.NewLine = string.IsNullOrEmpty(RS232UserPreferences.UserDefinedMessageTerminator) ?
                            "\0" : RS232UserPreferences.UserDefinedMessageTerminator;
                        break;
                }

                port.Open();
                HandleMessage("<info> Port " + port.PortName + " open.");
            }
            catch (Exception ex)
            {
                HandleMessage("<erro> " + ex.Message);
            }
        }

        public void SendMessage(string message)
        {
            try
            {
                if (message.StartsWith(RS232Constants.CommandSign))
                {
                    switch (message)
                    {
                        case RS232Constants.PingCommand:
                            HandleMessage("<ping> " + DateTime.Now.ToString());
                            pongPending = true;
                            pingTimer.Start();
                            pongStopwatch.Start();
                            break;
                        case RS232Constants.SetTerminatorCommand:
                            HandleMessage("<t800> Current user defined message terminator is: \"" + RS232UserPreferences.UserDefinedMessageTerminator + "\"");
                            return;
                        case RS232Constants.HelpCommand:
                            HandleMessage("<help> Available commands:\n\t/ping\n\t/t\n\t/clear");
                            return;
                        case RS232Constants.ClearCommand:
                            HandleMessage("<clear>");
                            return;
                    }
                }

                if (message.StartsWith(RS232Constants.SetTerminatorCommand) &&
                    message.Length > RS232Constants.SetTerminatorCommand.Length + 1)
                {
                    RS232UserPreferences.UserDefinedMessageTerminator = message.Substring(RS232Constants.SetTerminatorCommand.Length + 1);
                    HandleMessage("<t800> Set user defined message terminator to: \"" + RS232UserPreferences.UserDefinedMessageTerminator + "\"");
                    return;
                }

                Action<string> send = port.NewLine != "\0" ? (string msg) => port.WriteLine(msg) : (string msg) => port.Write(msg);
                if (portSettings!.FlowControl != FlowControl.DTR_DSR)
                {
                    send(message);
                }
                else
                {
                    port.DtrEnable = false;
                    dsrPending = true;
                    sendTimer.Start();
                    while (dsrPending)
                    {
                        if (port.DsrHolding)
                        {
                            sendTimer.Stop();
                            send(message);
                        }
                        break;
                    }
                    port.DtrEnable = true;
                    dsrPending = false;
                }

                if (!message.StartsWith(RS232Constants.CommandSign))
                {
                    HandleMessage("<send> " + message);
                }
            }
            catch (Exception ex)
            {
                HandleMessage("<erro> " + ex.Message);
            }
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var serialPort = (SerialPort)sender;
            string receivedMessage = "";

            try
            {
                if (port.NewLine != "\0")
                {
                    receivedMessage = serialPort.ReadLine();
                }
                else
                {
                    receivedMessage = serialPort.ReadExisting();
                }
            }
            catch (Exception ex)
            {
                HandleMessage("<erro> " + ex.Message);
            }

            if (receivedMessage.StartsWith(RS232Constants.CommandSign))
            {
                switch (receivedMessage)
                {
                    case RS232Constants.PingCommand:
                        SendMessage(RS232Constants.PongCommand);
                        break;
                    case RS232Constants.PongCommand:
                        if (pongPending)
                        {
                            pongStopwatch.Stop();
                            HandleMessage("<pong> Elapsed: " + pongStopwatch.ElapsedMilliseconds + " ms");
                            pongStopwatch.Reset();
                            pongPending = false;
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                HandleMessage("<recv> " + receivedMessage);
            }
        }

        private void OnSendTimeout(object? sender, ElapsedEventArgs e)
        {
            sendTimer.Stop();
            if (dsrPending)
            {
                HandleMessage("<erro> DTR/DSR timeout.");
                dsrPending = false;
            }
        }

        private void OnPongTimeout(object? sender, ElapsedEventArgs e)
        {
            pingTimer.Stop();
            if (pongPending)
            {
                HandleMessage("<erro> Ping pong timeout.");
                pongPending = false;
            }
        }

        private void HandleMessage(string message)
        {
            if (MessageHandler != null)
            {
                MessageHandler(this, new()
                {
                    ChatMessage = message
                });
            }
        }
    }

    public class RS232PortWrapperEventArgs : EventArgs
    {
        public string? ChatMessage { get; set; }
    }
}
