using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS232Chat
{
    internal class RS232PortWrapper
    {
        private readonly SerialPort port;
        public RS232PortWrapper()
        {
            port = new()
            {
                Encoding = Encoding.UTF8
            };
            port.DataReceived += OnDataReceived;

            ApplySettings(new());
        }

        public void ApplySettings(RS232PortSettings settings)
        {
            port.PortName = settings.PortName;
            port.BaudRate = settings.BaudRate;
            port.DataBits = settings.DataBits;
            port.Parity   = settings.Parity;
            port.StopBits = settings.StopBits;
            switch (settings.FlowControl)
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
            switch (settings.MessageTerminator)
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
                    //something
                    break;
            }
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var serialPort = (SerialPort)sender;

        }
    }
}
