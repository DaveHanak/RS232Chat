using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS232Chat
{
    public enum FlowControl
    {
        None,
        RTS_CTS,
        DTR_DSR,
        XON_XOFF,
    }
    public enum MessageTerminator
    {
        None,
        CR,
        LF,
        CR_LF,
        UserDefined,
    }
    public class RS232PortSettings
    {  
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public int DataBits { get; set; }
        public Parity Parity { get; set; }
        public StopBits StopBits { get; set; }
        public FlowControl FlowControl { get; set; }
        public MessageTerminator MessageTerminator { get; set; }

        public RS232PortSettings()
        {
            PortName = SerialPort.GetPortNames().First();
            BaudRate = RS232Constants.DefaultBaudRate;
            DataBits = RS232Constants.DefaultDataBits;
            Parity = RS232Constants.DefaultParity;
            StopBits = RS232Constants.DefaultStopBits;
            FlowControl = RS232Constants.DefaultFlowControl;
            MessageTerminator = RS232Constants.DefaultMessageTerminator;
        }
    }
}
