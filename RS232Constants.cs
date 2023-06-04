using System.IO.Ports;
using System.Security.Policy;

namespace RS232Chat
{
    public class RS232Constants
    {
        public const int DefaultBaudRate = 9600;
        public static readonly int[] CommonBaudRates =
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

        public const int DefaultDataBits = 7;
        public static readonly int[] CommonDataBits =
        {
            7,
            8,
        };

        public const Parity DefaultParity = Parity.Even;
        public static readonly Parity[] CommonParities =
        {
            Parity.None,
            Parity.Odd,
            Parity.Even,
        };

        public const StopBits DefaultStopBits = StopBits.One;
        public static readonly StopBits[] CommonStopBits =
        {
            StopBits.One,
            StopBits.Two,
        };

        public const FlowControl DefaultFlowControl = FlowControl.None;
        public const string DefaultFlowControlName = "None";
        public static readonly string[] CommonFlowControlNames =
        {
            "None",
            "RTS/CTS",
            "DTR/DSR",
            "XON/XOFF",
        };

        public const MessageTerminator DefaultMessageTerminator = MessageTerminator.CR;
        public const string DefaultMessageTerminatorName = "CR";
        public static readonly string[] CommonMessageTerminatorNames =
        {
            "None",
            "CR",
            "LF",
            "CR LF",
            "User defined",
        };

        public const double DefaultTimerInterval = 1000.0;

        public const string CommandSign = "/";
        public const string HelpCommand = "/help";
        public const string PingCommand = "/ping";
        public const string PongCommand = "/pong";
        public const string SetTerminatorCommand = "/t";
        public const string ClearCommand = "/clear";
    }
}
