using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.HWDrivers.PPK2
{
    public class PPK2_Command
    {
        // Serial command opcodes
        public const byte NO_OP = 0x00;
        public const byte TRIGGER_SET = 0x01;
        public const byte AVG_NUM_SET = 0x02;
        public const byte TRIGGER_WINDOW_SET = 0x03;
        public const byte TRIGGER_INTERVAL_SET = 0x04;
        public const byte TRIGGER_SINGLE_SET = 0x05;
        public const byte AVERAGE_START = 0x06;
        public const byte AVERAGE_STOP = 0x07;
        public const byte RANGE_SET = 0x08;
        public const byte LCD_SET = 0x09;
        public const byte TRIGGER_STOP = 0x0a;
        public const byte DEVICE_RUNNING_SET = 0x0c;
        public const byte REGULATOR_SET = 0x0d;
        public const byte SWITCH_POINT_DOWN = 0x0e;
        public const byte SWITCH_POINT_UP = 0x0f;
        public const byte TRIGGER_EXT_TOGGLE = 0x11;
        public const byte SET_POWER_MODE = 0x11;
        public const byte RES_USER_SET = 0x12;
        public const byte SPIKE_FILTERING_ON = 0x15;
        public const byte SPIKE_FILTERING_OFF = 0x16;
        public const byte GET_META_DATA = 0x19;
        public const byte RESET = 0x20;
        public const byte SET_USER_GAINS = 0x25;
    }
}
