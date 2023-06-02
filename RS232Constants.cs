using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS232Chat
{
    internal class RS232Constants
    {
        public static int DefaultBaudRate = 9600;
        public static int[] CommonBaudRates =
        {
            150,
            200,
            300,
            600,
            1200,
            1800,
            2400,
            4800,
            9600,
            19200,
            28800,
            38400,
            57600,
            76800,
            115200,
        };

        public static int DefaultDataBits = 7;
        public static int[] CommonDataBits =
        {
            7,
            8,
        };

        public static Parity DefaultParity = Parity.Even;
        public static Parity[] CommonParities =
        {
            Parity.None,
            Parity.Odd,
            Parity.Even,
        };

        public static StopBits DefaultStopBits = StopBits.One;
        public static StopBits[] CommonStopBits =
        {
            StopBits.One,
            StopBits.Two,
        };

        public static FlowControl DefaultFlowControl = FlowControl.None;
        public static string DefaultFlowControlName = "None";
        public static string[] CommonFlowControlNames =
        {
            "None",
            "RTS/CTS",
            "DTR/DSR",
            "XON/XOFF",
        };

        public static MessageTerminator DefaultMessageTerminator = MessageTerminator.CR;
        public static string DefaultMessageTerminatorName = "CR";
        public static string[] CommonMessageTerminatorNames =
        {
            "None",
            "CR",
            "LF",
            "CR LF",
            "User defined",
        };
    }
}
