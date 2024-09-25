using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.Common.Enums
{
    public enum DeviceType
    {
        None,
        Fake,
        CHESTER_M,
        CHESTER_DEVKIT,
        CHESTER_CLIME,
        CHESTER_CLIME_IAQ,
        CHESTER_COUNTER,
        CHESTER_CURRENT,
        CHESTER_SCALE,
        CHESTER_CONTROL,
        CHESTER_INPUT,
        CHESTER_MOTION,
        CHESTER_METEO,
        CHESTER_WMBUS,
        CHESTER_RANGE,
        CHESTER_RADON,

        FAKE_CHESTER_M = 2000,
        FAKE_CHESTER_DEVKIT = 2001,
        FAKE_CHESTER_CLIME = 2002,
        FAKE_CHESTER_CLIME_IAQ = 2003,
        FAKE_CHESTER_COUNTER = 2004,
        FAKE_CHESTER_CURRENT = 2005,
        FAKE_CHESTER_SCALE = 2006,
        FAKE_CHESTER_CONTROL = 2007,
        FAKE_CHESTER_INPUT = 2008,
        FAKE_CHESTER_MOTION = 2009,
        FAKE_CHESTER_METEO = 2010,
        FAKE_CHESTER_WMBUS = 2011,
        FAKE_CHESTER_RANGE = 2012,
        FAKE_CHESTER_RADON = 2013,

        STICKER_INPUT = 4000,
        STICKER_CLIME = 4001,
        STICKER_MOTION = 4002,

        FAKE_STICKER_INPUT = 6000,
        FAKE_STICKER_CLIME = 6001,
        FAKE_STICKER_MOTION = 6002,

        GAUGER_BASIC = 8000,
    }
}
