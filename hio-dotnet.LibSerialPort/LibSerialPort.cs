using System;
using System.Runtime.InteropServices;

namespace hio_dotnet.LibSerialPort
{
    /// <summary>
    /// Enums mirrored from libserialport.h
    /// </summary>
    public enum SpReturn
    {
        // Return values
        SP_OK = 0,
        SP_ERR_ARG = -1,
        SP_ERR_FAIL = -2,
        SP_ERR_MEM = -3,
        SP_ERR_SUPP = -4
    }

    [Flags]
    public enum SpMode
    {
        SP_MODE_READ = 1,
        SP_MODE_WRITE = 2,
        SP_MODE_READ_WRITE = 3
    }

    [Flags]
    public enum SpEvent
    {
        SP_EVENT_RX_READY = 1,
        SP_EVENT_TX_READY = 2,
        SP_EVENT_ERROR = 4
    }

    [Flags]
    public enum SpBuffer
    {
        SP_BUF_INPUT = 1,
        SP_BUF_OUTPUT = 2,
        SP_BUF_BOTH = 3
    }

    public enum SpStopBits
    {
        SP_STOP_BITS_NONE = 0,
        SP_STOP_BITS_ONE = 1,
        SP_STOP_BITS_TWO = 2,
        SP_STOP_BITS_ONEPOINTFIVE = 3,
    }

    public enum SpParity
    {
        SP_PARITY_INVALID = -1,
        SP_PARITY_NONE = 0,
        SP_PARITY_ODD = 1,
        SP_PARITY_EVEN = 2,
        SP_PARITY_MARK = 3,
        SP_PARITY_SPACE = 4
    }

    public enum SpRts
    {
        SP_RTS_INVALID = -1,
        SP_RTS_OFF = 0,
        SP_RTS_ON = 1,
        SP_RTS_FLOW_CONTROL = 2
    }

    public enum SpCts
    {
        SP_CTS_INVALID = -1,
        SP_CTS_IGNORE = 0,
        SP_CTS_FLOW_CONTROL = 1
    }

    public enum SpDtr
    {
        SP_DTR_INVALID = -1,
        SP_DTR_OFF = 0,
        SP_DTR_ON = 1,
        SP_DTR_FLOW_CONTROL = 2
    }

    public enum SpDsr
    {
        SP_DSR_INVALID = -1,
        SP_DSR_IGNORE = 0,
        SP_DSR_FLOW_CONTROL = 1
    }

    public enum SpXonxoff
    {
        SP_XONXOFF_INVALID = -1,
        SP_XONXOFF_DISABLED = 0,
        SP_XONXOFF_IN = 1,
        SP_XONXOFF_OUT = 2,
        SP_XONXOFF_INOUT = 3
    }

    public enum SpFlowcontrol
    {
        SP_FLOWCONTROL_NONE = 0,
        SP_FLOWCONTROL_XONXOFF = 1,
        SP_FLOWCONTROL_RTSCTS = 2,
        SP_FLOWCONTROL_DTRDSR = 3
    }

    [Flags]
    public enum SpSignal
    {
        SP_SIG_CTS = 1,
        SP_SIG_DSR = 2,
        SP_SIG_DCD = 4,
        SP_SIG_RI = 8
    }

    public enum SpTransport
    {
        SP_TRANSPORT_NATIVE,
        SP_TRANSPORT_USB,
        SP_TRANSPORT_BLUETOOTH
    }

    /// <summary>
    /// Static class with all imported libserialport functions.
    /// Comments in English, as requested.
    /// </summary>
    public static class LibSerialPort
    {
        /// <summary>
        /// This constant is the library name. On Linux it might be "libserialport",
        /// on Windows something like "libserialport-0". Adjust as needed.
        /// </summary>
        //private const string LIBSERIALPORT = "libserialport.dll"; // Or "libserialport"

#if WINDOWS
    private const string LIBSERIALPORT = "libserialport.dll";
#elif LINUX
    private const string LIBSERIALPORT = "libserialport";
#elif OSX
    private const string LIBSERIALPORT = "libserialport.dylib";
#else
        private const string LIBSERIALPORT = "libserialport";
#endif

        // ==============================
        // =    Port Enumeration       =
        // ==============================

        /// sp_get_port_by_name(const char *name, struct sp_port **port)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_get_port_by_name(
            [MarshalAs(UnmanagedType.LPStr)] string portName,
            out IntPtr port);

        /// sp_free_port(struct sp_port *port)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern void sp_free_port(IntPtr port);

        /// sp_list_ports(struct sp_port ***list_ptr)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_list_ports(out IntPtr portList);

        /// sp_free_port_list(struct sp_port **ports)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern void sp_free_port_list(IntPtr portList);

        /// sp_copy_port(const struct sp_port *port, struct sp_port **copy_ptr)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_copy_port(IntPtr port, out IntPtr copyPort);

        // ==============================
        // =      Port Handling        =
        // ==============================

        /// sp_open(struct sp_port *port, enum sp_mode flags)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_open(IntPtr port, SpMode flags);

        /// sp_close(struct sp_port *port)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_close(IntPtr port);

        /// sp_get_port_name(const struct sp_port *port)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr sp_get_port_name(IntPtr port);

        public static string sp_get_port_name_private(IntPtr port)
        {
            // We need to convert the returned IntPtr (C string) to a C# string.
            IntPtr ptr = sp_get_port_name(port);
            return Marshal.PtrToStringAnsi(ptr);
        }

        /// sp_get_port_description(const struct sp_port *port)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr sp_get_port_description_private(IntPtr port);

        public static string sp_get_port_description(IntPtr port)
        {
            IntPtr ptr = sp_get_port_description_private(port);
            return Marshal.PtrToStringAnsi(ptr);
        }

        /// sp_get_port_transport(const struct sp_port *port)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern SpTransport sp_get_port_transport(IntPtr port);

        /// sp_get_port_usb_bus_address(const struct sp_port *port, int *usb_bus, int *usb_address)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_get_port_usb_bus_address(IntPtr port, out int usbBus, out int usbAddress);

        /// sp_get_port_usb_vid_pid(const struct sp_port *port, int *usb_vid, int *usb_pid)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_get_port_usb_vid_pid(IntPtr port, out int vid, out int pid);

        /// sp_get_port_usb_manufacturer(const struct sp_port *port)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr sp_get_port_usb_manufacturer_private(IntPtr port);

        public static string sp_get_port_usb_manufacturer(IntPtr port)
        {
            IntPtr ptr = sp_get_port_usb_manufacturer_private(port);
            return Marshal.PtrToStringAnsi(ptr);
        }

        /// sp_get_port_usb_product(const struct sp_port *port)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr sp_get_port_usb_product_private(IntPtr port);

        public static string sp_get_port_usb_product(IntPtr port)
        {
            IntPtr ptr = sp_get_port_usb_product_private(port);
            return Marshal.PtrToStringAnsi(ptr);
        }

        /// sp_get_port_usb_serial(const struct sp_port *port)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr sp_get_port_usb_serial_private(IntPtr port);

        public static string sp_get_port_usb_serial(IntPtr port)
        {
            IntPtr ptr = sp_get_port_usb_serial_private(port);
            return Marshal.PtrToStringAnsi(ptr);
        }

        /// sp_get_port_bluetooth_address(const struct sp_port *port)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr sp_get_port_bluetooth_address_private(IntPtr port);

        public static string sp_get_port_bluetooth_address(IntPtr port)
        {
            IntPtr ptr = sp_get_port_bluetooth_address_private(port);
            return Marshal.PtrToStringAnsi(ptr);
        }

        /// sp_get_port_handle(const struct sp_port *port, void *result_ptr)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_get_port_handle(IntPtr port, IntPtr resultPtr);

        // ==============================
        // =      Configuration        =
        // ==============================

        /// sp_new_config(struct sp_port_config **config_ptr)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_new_config(out IntPtr config);

        /// sp_free_config(struct sp_port_config *config)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern void sp_free_config(IntPtr config);

        /// sp_get_config(struct sp_port *port, struct sp_port_config *config)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_get_config(IntPtr port, IntPtr config);

        /// sp_set_config(struct sp_port *port, const struct sp_port_config *config)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_set_config(IntPtr port, IntPtr config);

        /// sp_set_baudrate(struct sp_port *port, int baudrate)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_set_baudrate(IntPtr port, int baudrate);

        /// sp_get_config_baudrate(const struct sp_port_config *config, int *baudrate_ptr)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_get_config_baudrate(IntPtr config, out int baudrate);

        /// sp_set_config_baudrate(struct sp_port_config *config, int baudrate)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_set_config_baudrate(IntPtr config, int baudrate);

        /// sp_set_bits(struct sp_port *port, int bits)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_set_bits(IntPtr port, int bits);

        /// sp_get_config_bits(const struct sp_port_config *config, int *bits_ptr)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_get_config_bits(IntPtr config, out int bits);

        /// sp_set_config_bits(struct sp_port_config *config, int bits)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_set_config_bits(IntPtr config, int bits);

        /// sp_set_parity(struct sp_port *port, enum sp_parity parity)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_set_parity(IntPtr port, SpParity parity);

        /// sp_get_config_parity(const struct sp_port_config *config, enum sp_parity *parity_ptr)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_get_config_parity(IntPtr config, out SpParity parity);

        /// sp_set_config_parity(struct sp_port_config *config, enum sp_parity parity)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_set_config_parity(IntPtr config, SpParity parity);

        /// sp_set_stopbits(struct sp_port *port, int stopbits)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_set_stopbits(IntPtr port, int stopbits);

        public static int sp_set_stopbits_ext(IntPtr port, SpStopBits stopbits)
        {
            int res = sp_set_stopbits(port, (int)stopbits);
            return res;
        }

        /// sp_get_config_stopbits(const struct sp_port_config *config, int *stopbits_ptr)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_get_config_stopbits(IntPtr config, out int stopbits);

        /// sp_set_config_stopbits(struct sp_port_config *config, int stopbits)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_set_config_stopbits(IntPtr config, int stopbits);

        public static int sp_set_config_stopbits_ext(IntPtr port, SpStopBits stopbits)
        {
            int res = sp_set_config_stopbits(port, (int)stopbits);
            return res;
        }

        /// sp_set_rts(struct sp_port *port, enum sp_rts rts)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_set_rts(IntPtr port, SpRts rts);

        /// sp_get_config_rts(const struct sp_port_config *config, enum sp_rts *rts_ptr)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_get_config_rts(IntPtr config, out SpRts rts);

        /// sp_set_config_rts(struct sp_port_config *config, enum sp_rts rts)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_set_config_rts(IntPtr config, SpRts rts);

        /// sp_set_cts(struct sp_port *port, enum sp_cts cts)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_set_cts(IntPtr port, SpCts cts);

        /// sp_get_config_cts(const struct sp_port_config *config, enum sp_cts *cts_ptr)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_get_config_cts(IntPtr config, out SpCts cts);

        /// sp_set_config_cts(struct sp_port_config *config, enum sp_cts cts)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_set_config_cts(IntPtr config, SpCts cts);

        /// sp_set_dtr(struct sp_port *port, enum sp_dtr dtr)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_set_dtr(IntPtr port, SpDtr dtr);

        /// sp_get_config_dtr(const struct sp_port_config *config, enum sp_dtr *dtr_ptr)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_get_config_dtr(IntPtr config, out SpDtr dtr);

        /// sp_set_config_dtr(struct sp_port_config *config, enum sp_dtr dtr)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_set_config_dtr(IntPtr config, SpDtr dtr);

        /// sp_set_dsr(struct sp_port *port, enum sp_dsr dsr)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_set_dsr(IntPtr port, SpDsr dsr);

        /// sp_get_config_dsr(const struct sp_port_config *config, enum sp_dsr *dsr_ptr)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_get_config_dsr(IntPtr config, out SpDsr dsr);

        /// sp_set_config_dsr(struct sp_port_config *config, enum sp_dsr dsr)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_set_config_dsr(IntPtr config, SpDsr dsr);

        /// sp_set_xon_xoff(struct sp_port *port, enum sp_xonxoff xon_xoff)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_set_xon_xoff(IntPtr port, SpXonxoff xon_xoff);

        /// sp_get_config_xon_xoff(const struct sp_port_config *config, enum sp_xonxoff *xon_xoff_ptr)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_get_config_xon_xoff(IntPtr config, out SpXonxoff xonXoff);

        /// sp_set_config_xon_xoff(struct sp_port_config *config, enum sp_xonxoff xon_xoff)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_set_config_xon_xoff(IntPtr config, SpXonxoff xonXoff);

        /// sp_set_config_flowcontrol(struct sp_port_config *config, enum sp_flowcontrol flowcontrol)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_set_config_flowcontrol(IntPtr config, SpFlowcontrol flowcontrol);

        /// sp_set_flowcontrol(struct sp_port *port, enum sp_flowcontrol flowcontrol)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_set_flowcontrol(IntPtr port, SpFlowcontrol flowcontrol);

        // ==============================
        // =       Data Handling       =
        // ==============================

        /// sp_blocking_read(struct sp_port *port, void *buf, size_t count, unsigned int timeout_ms)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_blocking_read(IntPtr port,
                                                  [Out] byte[] buffer,
                                                  IntPtr count,
                                                  uint timeoutMs);

        /// sp_blocking_read_next(struct sp_port *port, void *buf, size_t count, unsigned int timeout_ms)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_blocking_read_next(IntPtr port,
                                                       [Out] byte[] buffer,
                                                       IntPtr count,
                                                       uint timeoutMs);

        /// sp_nonblocking_read(struct sp_port *port, void *buf, size_t count)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_nonblocking_read(IntPtr port,
                                                     [Out] byte[] buffer,
                                                     IntPtr count);

        /// sp_blocking_write(struct sp_port *port, const void *buf, size_t count, unsigned int timeout_ms)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_blocking_write(IntPtr port,
                                                   byte[] buffer,
                                                   IntPtr count,
                                                   uint timeoutMs);

        /// sp_nonblocking_write(struct sp_port *port, const void *buf, size_t count)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_nonblocking_write(IntPtr port,
                                                      byte[] buffer,
                                                      IntPtr count);

        /// sp_input_waiting(struct sp_port *port)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_input_waiting(IntPtr port);

        /// sp_output_waiting(struct sp_port *port)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_output_waiting(IntPtr port);

        /// sp_flush(struct sp_port *port, enum sp_buffer buffers)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_flush(IntPtr port, SpBuffer buffers);

        /// sp_drain(struct sp_port *port)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_drain(IntPtr port);

        // ==============================
        // =        Waiting / Events   =
        // ==============================

        /// sp_new_event_set(struct sp_event_set **result_ptr)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_new_event_set(out IntPtr eventSet);

        /// sp_add_port_events(struct sp_event_set *event_set, const struct sp_port *port, enum sp_event mask)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_add_port_events(IntPtr eventSet,
                                                    IntPtr port,
                                                    SpEvent mask);

        /// sp_wait(struct sp_event_set *event_set, unsigned int timeout_ms)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_wait(IntPtr eventSet, uint timeoutMs);

        /// sp_free_event_set(struct sp_event_set *eventSet)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern void sp_free_event_set(IntPtr eventSet);

        // ==============================
        // =         Signals           =
        // ==============================

        /// sp_get_signals(struct sp_port *port, enum sp_signal *signal_mask)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_get_signals(IntPtr port, out SpSignal signals);

        /// sp_start_break(struct sp_port *port)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_start_break(IntPtr port);

        /// sp_end_break(struct sp_port *port)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_end_break(IntPtr port);

        // ==============================
        // =          Errors           =
        // ==============================

        /// sp_last_error_code(void)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_last_error_code();

        /// sp_last_error_message(void)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr sp_last_error_message_private();

        public static string sp_last_error_message()
        {
            IntPtr ptr = sp_last_error_message_private();
            // Must be freed later by sp_free_error_message, so we copy it to a .NET string:
            if (ptr == IntPtr.Zero)
                return null;

            string msg = Marshal.PtrToStringAnsi(ptr);
            sp_free_error_message(ptr);
            return msg;
        }

        /// sp_free_error_message(char *message)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern void sp_free_error_message(IntPtr message);

        /// sp_set_debug_handler(void (*handler)(const char *format, ...))
        // This is tricky with varargs. We usually cannot straightforwardly handle that in .NET.
        // We'll skip it or handle it in a more advanced way.
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern void sp_set_debug_handler(IntPtr handler);

        /// sp_default_debug_handler(const char *format, ...)
        // Same issue with varargs.
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern void sp_default_debug_handler(string format /*, varargs not supported*/);

        // ==============================
        // =         Versions          =
        // ==============================

        /// sp_get_major_package_version(void)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_get_major_package_version();

        /// sp_get_minor_package_version(void)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_get_minor_package_version();

        /// sp_get_micro_package_version(void)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_get_micro_package_version();

        /// sp_get_package_version_string(void)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr sp_get_package_version_string_private();

        public static string sp_get_package_version_string()
        {
            IntPtr ptr = sp_get_package_version_string_private();
            return Marshal.PtrToStringAnsi(ptr);
        }

        /// sp_get_current_lib_version(void)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_get_current_lib_version();

        /// sp_get_revision_lib_version(void)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_get_revision_lib_version();

        /// sp_get_age_lib_version(void)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        public static extern int sp_get_age_lib_version();

        /// sp_get_lib_version_string(void)
        [DllImport(LIBSERIALPORT, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr sp_get_lib_version_string_private();

        public static string sp_get_lib_version_string()
        {
            IntPtr ptr = sp_get_lib_version_string_private();
            return Marshal.PtrToStringAnsi(ptr);
        }
    }
}
