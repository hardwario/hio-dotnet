﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hio_dotnet.HWDrivers.JLink
{
    public enum JLinkErrorCodes
    {
        None = 0,

        // GENERAL ERRORS
        JLINK_ERR_EMU_NO_CONNECTION = -256,
        JLINK_ERR_EMU_COMM_ERROR = -257,
        JLINK_ERR_DLL_NOT_OPEN = -258,
        JLINK_ERR_VCC_FAILURE = -259,
        JLINK_ERR_INVALID_HANDLE = -260,
        JLINK_ERR_NO_CPU_FOUND = -261,
        JLINK_ERR_EMU_FEATURE_NOT_SUPPORTED = -262,
        JLINK_ERR_EMU_NO_MEMORY = -263,
        JLINK_ERR_TIF_STATUS_ERROR = -264,
        JLINK_ERR_FLASH_PROG_COMPARE_FAILED = -265,
        JLINK_ERR_FLASH_PROG_PROGRAM_FAILED = -266,
        JLINK_ERR_FLASH_PROG_VERIFY_FAILED = -267,
        JLINK_ERR_OPEN_FILE_FAILED = -268,
        JLINK_ERR_UNKNOWN_FILE_FORMAT = -269,
        JLINK_ERR_WRITE_TARGET_MEMORY_FAILED = -270,
        JLINK_ERR_DEVICE_FEATURE_NOT_SUPPORTED = -271,
        JLINK_ERR_WRONG_USER_CONFIG = -272,
        JLINK_ERR_NO_TARGET_DEVICE_SELECTED = -273,
        JLINK_ERR_CPU_IN_LOW_POWER_MODE = -274,
        JLINK_ERR_CPU_NOT_HALTED = -275,
        JLINK_ERR_READ_TARGET_MEMORY_FAILED = -276,
        JLINK_ERR_TARGET_FEATURE_NOT_SUPPORTED = -277,

        // FLASHING ERRORS
        JLINK_FLASH_ERR_BLOCK_VERIFICATION_ERROR = 1,
        JLINK_FLASH_ERR_ITEM_VERIFICATION_ERROR = 2,
        JLINK_FLASH_ERR_TIMEOUT = 3,
        JLINK_FLASH_ERR_PROGRAM_ERROR = 4,
        JLINK_FLASH_ERR_PROGRAM_1_OVER_0 = 5,
        JLINK_FLASH_ERR_SECTOR_IS_LOCKED = 6,
        JLINK_FLASH_ERR_ERASE_ERROR = 7,
        JLINK_FLASH_ERR_NO_FLASH_MEMORY = 8,
        JLINK_FLASH_ERR_GENERIC_ERROR = 9,
        JLINK_FLASH_ERR_ALGO_SPECIFIC_ERROR = -2,
        JLINK_FLASH_ERR_NO_FLASH_BANK = -3,
        JLINK_FLASH_ERR_PROGRAM_DOES_NOT_FIT = -4

    }
}
