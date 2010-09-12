// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Sassner.SmarterSql.PInvoke {
	public class NativeWIN32 {
		#region Constants

		#region Windows messages

		public enum WindowsMessages {
			WM_ACTIVATE = 0x6,
			WM_ACTIVATEAPP = 0x1C,
			WM_AFXFIRST = 0x360,
			WM_AFXLAST = 0x37F,
			WM_APP = 0x8000,
			WM_ASKCBFORMATNAME = 0x30C,
			WM_CANCELJOURNAL = 0x4B,
			WM_CANCELMODE = 0x1F,
			WM_CAPTURECHANGED = 0x215,
			WM_CHANGECBCHAIN = 0x30D,
			WM_CHAR = 0x102,
			WM_CHARTOITEM = 0x2F,
			WM_CHILDACTIVATE = 0x22,
			WM_CLEAR = 0x303,
			WM_CLOSE = 0x10,
			WM_COMMAND = 0x111,
			WM_COMPACTING = 0x41,
			WM_COMPAREITEM = 0x39,
			WM_CONTEXTMENU = 0x7B,
			WM_COPY = 0x301,
			WM_COPYDATA = 0x4A,
			WM_CREATE = 0x1,
			WM_CTLCOLORBTN = 0x135,
			WM_CTLCOLORDLG = 0x136,
			WM_CTLCOLOREDIT = 0x133,
			WM_CTLCOLORLISTBOX = 0x134,
			WM_CTLCOLORMSGBOX = 0x132,
			WM_CTLCOLORSCROLLBAR = 0x137,
			WM_CTLCOLORSTATIC = 0x138,
			WM_CUT = 0x300,
			WM_DEADCHAR = 0x103,
			WM_DELETEITEM = 0x2D,
			WM_DESTROY = 0x2,
			WM_DESTROYCLIPBOARD = 0x307,
			WM_DEVICECHANGE = 0x219,
			WM_DEVMODECHANGE = 0x1B,
			WM_DISPLAYCHANGE = 0x7E,
			WM_DRAWCLIPBOARD = 0x308,
			WM_DRAWITEM = 0x2B,
			WM_DROPFILES = 0x233,
			WM_ENABLE = 0xA,
			WM_ENDSESSION = 0x16,
			WM_ENTERIDLE = 0x121,
			WM_ENTERMENULOOP = 0x211,
			WM_ENTERSIZEMOVE = 0x231,
			WM_ERASEBKGND = 0x14,
			WM_EXITMENULOOP = 0x212,
			WM_EXITSIZEMOVE = 0x232,
			WM_FONTCHANGE = 0x1D,
			WM_GETDLGCODE = 0x87,
			WM_GETFONT = 0x31,
			WM_GETHOTKEY = 0x33,
			WM_GETICON = 0x7F,
			WM_GETMINMAXINFO = 0x24,
			WM_GETOBJECT = 0x3D,
			WM_GETSYSMENU = 0x313,
			WM_GETTEXT = 0xD,
			WM_GETTEXTLENGTH = 0xE,
			WM_HANDHELDFIRST = 0x358,
			WM_HANDHELDLAST = 0x35F,
			WM_HELP = 0x53,
			WM_HOTKEY = 0x312,
			WM_HSCROLL = 0x114,
			WM_HSCROLLCLIPBOARD = 0x30E,
			WM_ICONERASEBKGND = 0x27,
			WM_IME_CHAR = 0x286,
			WM_IME_COMPOSITION = 0x10F,
			WM_IME_COMPOSITIONFULL = 0x284,
			WM_IME_CONTROL = 0x283,
			WM_IME_ENDCOMPOSITION = 0x10E,
			WM_IME_KEYDOWN = 0x290,
			WM_IME_KEYLAST = 0x10F,
			WM_IME_KEYUP = 0x291,
			WM_IME_NOTIFY = 0x282,
			WM_IME_REQUEST = 0x288,
			WM_IME_SELECT = 0x285,
			WM_IME_SETCONTEXT = 0x281,
			WM_IME_STARTCOMPOSITION = 0x10D,
			WM_INITDIALOG = 0x110,
			WM_INITMENU = 0x116,
			WM_INITMENUPOPUP = 0x117,
			WM_INPUTLANGCHANGE = 0x51,
			WM_INPUTLANGCHANGEREQUEST = 0x50,
			WM_KEYDOWN = 0x100,
			WM_KEYFIRST = 0x100,
			WM_KEYLAST = 0x108,
			WM_KEYUP = 0x101,
			WM_KILLFOCUS = 0x8,
			WM_LBUTTONDBLCLK = 0x203,
			WM_LBUTTONDOWN = 0x201,
			WM_LBUTTONUP = 0x202,
			WM_MBUTTONDBLCLK = 0x209,
			WM_MBUTTONDOWN = 0x207,
			WM_MBUTTONUP = 0x208,
			WM_MDIACTIVATE = 0x222,
			WM_MDICASCADE = 0x227,
			WM_MDICREATE = 0x220,
			WM_MDIDESTROY = 0x221,
			WM_MDIGETACTIVE = 0x229,
			WM_MDIICONARRANGE = 0x228,
			WM_MDIMAXIMIZE = 0x225,
			WM_MDINEXT = 0x224,
			WM_MDIREFRESHMENU = 0x234,
			WM_MDIRESTORE = 0x223,
			WM_MDISETMENU = 0x230,
			WM_MDITILE = 0x226,
			WM_MEASUREITEM = 0x2C,
			WM_MENUCHAR = 0x120,
			WM_MENUCOMMAND = 0x126,
			WM_MENUDRAG = 0x123,
			WM_MENUGETOBJECT = 0x124,
			WM_MENURBUTTONUP = 0x122,
			WM_MENUSELECT = 0x11F,
			WM_MOUSEACTIVATE = 0x21,
			WM_MOUSEFIRST = 0x200,
			WM_MOUSEHOVER = 0x2A1,
			WM_XBUTTONDOWN = 0x020B,
			WM_XBUTTONUP = 0x020C,
			WM_XBUTTONDBLCLK = 0x020D,
			WM_MOUSELAST = 0x20D,
			WM_MOUSELEAVE = 0x2A3,
			WM_MOUSEMOVE = 0x200,
			WM_MOUSEWHEEL = 0x20A,
			WM_MOUSEHWHEEL = 0x20E,
			WM_MOVE = 0x3,
			WM_MOVING = 0x216,
			WM_NCACTIVATE = 0x86,
			WM_NCCALCSIZE = 0x83,
			WM_NCCREATE = 0x81,
			WM_NCDESTROY = 0x82,
			WM_NCHITTEST = 0x84,
			WM_NCLBUTTONDBLCLK = 0xA3,
			WM_NCLBUTTONDOWN = 0xA1,
			WM_NCLBUTTONUP = 0xA2,
			WM_NCMBUTTONDBLCLK = 0xA9,
			WM_NCMBUTTONDOWN = 0xA7,
			WM_NCMBUTTONUP = 0xA8,
			WM_NCMOUSEHOVER = 0x2A0,
			WM_NCMOUSELEAVE = 0x2A2,
			WM_NCMOUSEMOVE = 0xA0,
			WM_NCPAINT = 0x85,
			WM_NCRBUTTONDBLCLK = 0xA6,
			WM_NCRBUTTONDOWN = 0xA4,
			WM_NCRBUTTONUP = 0xA5,
			WM_NEXTDLGCTL = 0x28,
			WM_NEXTMENU = 0x213,
			WM_NOTIFY = 0x4E,
			WM_NOTIFYFORMAT = 0x55,
			WM_NULL = 0x0,
			WM_PAINT = 0xF,
			WM_PAINTCLIPBOARD = 0x309,
			WM_PAINTICON = 0x26,
			WM_PALETTECHANGED = 0x311,
			WM_PALETTEISCHANGING = 0x310,
			WM_PARENTNOTIFY = 0x210,
			WM_PASTE = 0x302,
			WM_PENWINFIRST = 0x380,
			WM_PENWINLAST = 0x38F,
			WM_POWER = 0x48,
			WM_PRINT = 0x317,
			WM_PRINTCLIENT = 0x318,
			WM_QUERYDRAGICON = 0x37,
			WM_QUERYENDSESSION = 0x11,
			WM_QUERYNEWPALETTE = 0x30F,
			WM_QUERYOPEN = 0x13,
			WM_QUERYUISTATE = 0x129,
			WM_QUEUESYNC = 0x23,
			WM_QUIT = 0x12,
			WM_RBUTTONDBLCLK = 0x206,
			WM_RBUTTONDOWN = 0x204,
			WM_RBUTTONUP = 0x205,
			WM_RENDERALLFORMATS = 0x306,
			WM_RENDERFORMAT = 0x305,
			WM_SETCURSOR = 0x20,
			WM_SETFOCUS = 0x7,
			WM_SETFONT = 0x30,
			WM_SETHOTKEY = 0x32,
			WM_SETICON = 0x80,
			WM_SETREDRAW = 0xB,
			WM_SETTEXT = 0xC,
			WM_SETTINGCHANGE = 0x1A,
			WM_SHOWWINDOW = 0x18,
			WM_SIZE = 0x5,
			WM_SIZECLIPBOARD = 0x30B,
			WM_SIZING = 0x214,
			WM_SPOOLERSTATUS = 0x2A,
			WM_STYLECHANGED = 0x7D,
			WM_STYLECHANGING = 0x7C,
			WM_SYNCPAINT = 0x88,
			WM_SYSCHAR = 0x106,
			WM_SYSCOLORCHANGE = 0x15,
			WM_SYSCOMMAND = 0x112,
			WM_SYSDEADCHAR = 0x107,
			WM_SYSKEYDOWN = 0x104,
			WM_SYSKEYUP = 0x105,
			WM_SYSTIMER = 0x118, // undocumented, see http://support.microsoft.com/?id=108938
			WM_TCARD = 0x52,
			WM_TIMECHANGE = 0x1E,
			WM_TIMER = 0x113,
			WM_UNDO = 0x304,
			WM_UNINITMENUPOPUP = 0x125,
			WM_USER = 0x400,
			WM_USERCHANGED = 0x54,
			WM_VKEYTOITEM = 0x2E,
			WM_VSCROLL = 0x115,
			WM_VSCROLLCLIPBOARD = 0x30A,
			WM_WINDOWPOSCHANGED = 0x47,
			WM_WINDOWPOSCHANGING = 0x46,
			WM_WININICHANGE = 0x1A,
		}

		#endregion

		#region SPI

		/// <summary>
		/// SPI_ System-wide parameter - Used in SystemParametersInfo function 
		/// </summary>
		[Description("SPI_(System-wide parameter - Used in SystemParametersInfo function )")]
		public enum SPI : uint {
			/// <summary>
			/// Determines whether the warning beeper is on. 
			/// The pvParam parameter must point to a BOOL variable that receives TRUE if the beeper is on, or FALSE if it is off.
			/// </summary>
			SPI_GETBEEP = 0x0001,

			/// <summary>
			/// Turns the warning beeper on or off. The uiParam parameter specifies TRUE for on, or FALSE for off.
			/// </summary>
			SPI_SETBEEP = 0x0002,

			/// <summary>
			/// Retrieves the two mouse threshold values and the mouse speed.
			/// </summary>
			SPI_GETMOUSE = 0x0003,

			/// <summary>
			/// Sets the two mouse threshold values and the mouse speed.
			/// </summary>
			SPI_SETMOUSE = 0x0004,

			/// <summary>
			/// Retrieves the border multiplier factor that determines the width of a window's sizing border. 
			/// The pvParam parameter must point to an integer variable that receives this value.
			/// </summary>
			SPI_GETBORDER = 0x0005,

			/// <summary>
			/// Sets the border multiplier factor that determines the width of a window's sizing border. 
			/// The uiParam parameter specifies the new value.
			/// </summary>
			SPI_SETBORDER = 0x0006,

			/// <summary>
			/// Retrieves the keyboard repeat-speed setting, which is a value in the range from 0 (approximately 2.5 repetitions per second) 
			/// through 31 (approximately 30 repetitions per second). The actual repeat rates are hardware-dependent and may vary from 
			/// a linear scale by as much as 20%. The pvParam parameter must point to a DWORD variable that receives the setting
			/// </summary>
			SPI_GETKEYBOARDSPEED = 0x000A,

			/// <summary>
			/// Sets the keyboard repeat-speed setting. The uiParam parameter must specify a value in the range from 0 
			/// (approximately 2.5 repetitions per second) through 31 (approximately 30 repetitions per second). 
			/// The actual repeat rates are hardware-dependent and may vary from a linear scale by as much as 20%. 
			/// If uiParam is greater than 31, the parameter is set to 31.
			/// </summary>
			SPI_SETKEYBOARDSPEED = 0x000B,

			/// <summary>
			/// Not implemented.
			/// </summary>
			SPI_LANGDRIVER = 0x000C,

			/// <summary>
			/// Sets or retrieves the width, in pixels, of an icon cell. The system uses this rectangle to arrange icons in large icon view. 
			/// To set this value, set uiParam to the new value and set pvParam to null. You cannot set this value to less than SM_CXICON.
			/// To retrieve this value, pvParam must point to an integer that receives the current value.
			/// </summary>
			SPI_ICONHORIZONTALSPACING = 0x000D,

			/// <summary>
			/// Retrieves the screen saver time-out value, in seconds. The pvParam parameter must point to an integer variable that receives the value.
			/// </summary>
			SPI_GETSCREENSAVETIMEOUT = 0x000E,

			/// <summary>
			/// Sets the screen saver time-out value to the value of the uiParam parameter. This value is the amount of time, in seconds, 
			/// that the system must be idle before the screen saver activates.
			/// </summary>
			SPI_SETSCREENSAVETIMEOUT = 0x000F,

			/// <summary>
			/// Determines whether screen saving is enabled. The pvParam parameter must point to a bool variable that receives TRUE 
			/// if screen saving is enabled, or FALSE otherwise.
			/// </summary>
			SPI_GETSCREENSAVEACTIVE = 0x0010,

			/// <summary>
			/// Sets the state of the screen saver. The uiParam parameter specifies TRUE to activate screen saving, or FALSE to deactivate it.
			/// </summary>
			SPI_SETSCREENSAVEACTIVE = 0x0011,

			/// <summary>
			/// Retrieves the current granularity value of the desktop sizing grid. The pvParam parameter must point to an integer variable 
			/// that receives the granularity.
			/// </summary>
			SPI_GETGRIDGRANULARITY = 0x0012,

			/// <summary>
			/// Sets the granularity of the desktop sizing grid to the value of the uiParam parameter.
			/// </summary>
			SPI_SETGRIDGRANULARITY = 0x0013,

			/// <summary>
			/// Sets the desktop wallpaper. The value of the pvParam parameter determines the new wallpaper. To specify a wallpaper bitmap, 
			/// set pvParam to point to a null-terminated string containing the name of a bitmap file. Setting pvParam to "" removes the wallpaper. 
			/// Setting pvParam to SETWALLPAPER_DEFAULT or null reverts to the default wallpaper.
			/// </summary>
			SPI_SETDESKWALLPAPER = 0x0014,

			/// <summary>
			/// Sets the current desktop pattern by causing Windows to read the Pattern= setting from the WIN.INI file.
			/// </summary>
			SPI_SETDESKPATTERN = 0x0015,

			/// <summary>
			/// Retrieves the keyboard repeat-delay setting, which is a value in the range from 0 (approximately 250 ms delay) through 3 
			/// (approximately 1 second delay). The actual delay associated with each value may vary depending on the hardware. The pvParam parameter must point to an integer variable that receives the setting.
			/// </summary>
			SPI_GETKEYBOARDDELAY = 0x0016,

			/// <summary>
			/// Sets the keyboard repeat-delay setting. The uiParam parameter must specify 0, 1, 2, or 3, where zero sets the shortest delay 
			/// (approximately 250 ms) and 3 sets the longest delay (approximately 1 second). The actual delay associated with each value may 
			/// vary depending on the hardware.
			/// </summary>
			SPI_SETKEYBOARDDELAY = 0x0017,

			/// <summary>
			/// Sets or retrieves the height, in pixels, of an icon cell. 
			/// To set this value, set uiParam to the new value and set pvParam to null. You cannot set this value to less than SM_CYICON.
			/// To retrieve this value, pvParam must point to an integer that receives the current value.
			/// </summary>
			SPI_ICONVERTICALSPACING = 0x0018,

			/// <summary>
			/// Determines whether icon-title wrapping is enabled. The pvParam parameter must point to a bool variable that receives TRUE 
			/// if enabled, or FALSE otherwise.
			/// </summary>
			SPI_GETICONTITLEWRAP = 0x0019,

			/// <summary>
			/// Turns icon-title wrapping on or off. The uiParam parameter specifies TRUE for on, or FALSE for off.
			/// </summary>
			SPI_SETICONTITLEWRAP = 0x001A,

			/// <summary>
			/// Determines whether pop-up menus are left-aligned or right-aligned, relative to the corresponding menu-bar item. 
			/// The pvParam parameter must point to a bool variable that receives TRUE if left-aligned, or FALSE otherwise.
			/// </summary>
			SPI_GETMENUDROPALIGNMENT = 0x001B,

			/// <summary>
			/// Sets the alignment value of pop-up menus. The uiParam parameter specifies TRUE for right alignment, or FALSE for left alignment.
			/// </summary>
			SPI_SETMENUDROPALIGNMENT = 0x001C,

			/// <summary>
			/// Sets the width of the double-click rectangle to the value of the uiParam parameter. 
			/// The double-click rectangle is the rectangle within which the second click of a double-click must fall for it to be registered 
			/// as a double-click.
			/// To retrieve the width of the double-click rectangle, call GetSystemMetrics with the SM_CXDOUBLECLK flag.
			/// </summary>
			SPI_SETDOUBLECLKWIDTH = 0x001D,

			/// <summary>
			/// Sets the height of the double-click rectangle to the value of the uiParam parameter. 
			/// The double-click rectangle is the rectangle within which the second click of a double-click must fall for it to be registered 
			/// as a double-click.
			/// To retrieve the height of the double-click rectangle, call GetSystemMetrics with the SM_CYDOUBLECLK flag.
			/// </summary>
			SPI_SETDOUBLECLKHEIGHT = 0x001E,

			/// <summary>
			/// Retrieves the logical font information for the current icon-title font. The uiParam parameter specifies the size of a LOGFONT structure, 
			/// and the pvParam parameter must point to the LOGFONT structure to fill in.
			/// </summary>
			SPI_GETICONTITLELOGFONT = 0x001F,

			/// <summary>
			/// Sets the double-click time for the mouse to the value of the uiParam parameter. The double-click time is the maximum number 
			/// of milliseconds that can occur between the first and second clicks of a double-click. You can also call the SetDoubleClickTime 
			/// function to set the double-click time. To get the current double-click time, call the GetDoubleClickTime function.
			/// </summary>
			SPI_SETDOUBLECLICKTIME = 0x0020,

			/// <summary>
			/// Swaps or restores the meaning of the left and right mouse buttons. The uiParam parameter specifies TRUE to swap the meanings 
			/// of the buttons, or FALSE to restore their original meanings.
			/// </summary>
			SPI_SETMOUSEBUTTONSWAP = 0x0021,

			/// <summary>
			/// Sets the font that is used for icon titles. The uiParam parameter specifies the size of a LOGFONT structure, 
			/// and the pvParam parameter must point to a LOGFONT structure.
			/// </summary>
			SPI_SETICONTITLELOGFONT = 0x0022,

			/// <summary>
			/// This flag is obsolete. Previous versions of the system use this flag to determine whether ALT+TAB fast task switching is enabled. 
			/// For Windows 95, Windows 98, and Windows NT version 4.0 and later, fast task switching is always enabled.
			/// </summary>
			SPI_GETFASTTASKSWITCH = 0x0023,

			/// <summary>
			/// This flag is obsolete. Previous versions of the system use this flag to enable or disable ALT+TAB fast task switching. 
			/// For Windows 95, Windows 98, and Windows NT version 4.0 and later, fast task switching is always enabled.
			/// </summary>
			SPI_SETFASTTASKSWITCH = 0x0024,

			//#if(WINVER >= 0x0400)
			/// <summary>
			/// Sets dragging of full windows either on or off. The uiParam parameter specifies TRUE for on, or FALSE for off. 
			/// Windows 95:  This flag is supported only if Windows Plus! is installed. See SPI_GETWINDOWSEXTENSION.
			/// </summary>
			SPI_SETDRAGFULLWINDOWS = 0x0025,

			/// <summary>
			/// Determines whether dragging of full windows is enabled. The pvParam parameter must point to a BOOL variable that receives TRUE 
			/// if enabled, or FALSE otherwise. 
			/// Windows 95:  This flag is supported only if Windows Plus! is installed. See SPI_GETWINDOWSEXTENSION.
			/// </summary>
			SPI_GETDRAGFULLWINDOWS = 0x0026,

			/// <summary>
			/// Retrieves the metrics associated with the nonclient area of nonminimized windows. The pvParam parameter must point 
			/// to a NONCLIENTMETRICS structure that receives the information. Set the cbSize member of this structure and the uiParam parameter 
			/// to sizeof(NONCLIENTMETRICS).
			/// </summary>
			SPI_GETNONCLIENTMETRICS = 0x0029,

			/// <summary>
			/// Sets the metrics associated with the nonclient area of nonminimized windows. The pvParam parameter must point 
			/// to a NONCLIENTMETRICS structure that contains the new parameters. Set the cbSize member of this structure 
			/// and the uiParam parameter to sizeof(NONCLIENTMETRICS). Also, the lfHeight member of the LOGFONT structure must be a negative value.
			/// </summary>
			SPI_SETNONCLIENTMETRICS = 0x002A,

			/// <summary>
			/// Retrieves the metrics associated with minimized windows. The pvParam parameter must point to a MINIMIZEDMETRICS structure 
			/// that receives the information. Set the cbSize member of this structure and the uiParam parameter to sizeof(MINIMIZEDMETRICS).
			/// </summary>
			SPI_GETMINIMIZEDMETRICS = 0x002B,

			/// <summary>
			/// Sets the metrics associated with minimized windows. The pvParam parameter must point to a MINIMIZEDMETRICS structure 
			/// that contains the new parameters. Set the cbSize member of this structure and the uiParam parameter to sizeof(MINIMIZEDMETRICS).
			/// </summary>
			SPI_SETMINIMIZEDMETRICS = 0x002C,

			/// <summary>
			/// Retrieves the metrics associated with icons. The pvParam parameter must point to an ICONMETRICS structure that receives 
			/// the information. Set the cbSize member of this structure and the uiParam parameter to sizeof(ICONMETRICS).
			/// </summary>
			SPI_GETICONMETRICS = 0x002D,

			/// <summary>
			/// Sets the metrics associated with icons. The pvParam parameter must point to an ICONMETRICS structure that contains 
			/// the new parameters. Set the cbSize member of this structure and the uiParam parameter to sizeof(ICONMETRICS).
			/// </summary>
			SPI_SETICONMETRICS = 0x002E,

			/// <summary>
			/// Sets the size of the work area. The work area is the portion of the screen not obscured by the system taskbar 
			/// or by application desktop toolbars. The pvParam parameter is a pointer to a RECT structure that specifies the new work area rectangle, 
			/// expressed in virtual screen coordinates. In a system with multiple display monitors, the function sets the work area 
			/// of the monitor that contains the specified rectangle.
			/// </summary>
			SPI_SETWORKAREA = 0x002F,

			/// <summary>
			/// Retrieves the size of the work area on the primary display monitor. The work area is the portion of the screen not obscured 
			/// by the system taskbar or by application desktop toolbars. The pvParam parameter must point to a RECT structure that receives 
			/// the coordinates of the work area, expressed in virtual screen coordinates. 
			/// To get the work area of a monitor other than the primary display monitor, call the GetMonitorInfo function.
			/// </summary>
			SPI_GETWORKAREA = 0x0030,

			/// <summary>
			/// Windows Me/98/95:  Pen windows is being loaded or unloaded. The uiParam parameter is TRUE when loading and FALSE 
			/// when unloading pen windows. The pvParam parameter is null.
			/// </summary>
			SPI_SETPENWINDOWS = 0x0031,

			/// <summary>
			/// Retrieves information about the HighContrast accessibility feature. The pvParam parameter must point to a HIGHCONTRAST structure 
			/// that receives the information. Set the cbSize member of this structure and the uiParam parameter to sizeof(HIGHCONTRAST). 
			/// For a general discussion, see remarks.
			/// Windows NT:  This value is not supported.
			/// </summary>
			/// <remarks>
			/// There is a difference between the High Contrast color scheme and the High Contrast Mode. The High Contrast color scheme changes 
			/// the system colors to colors that have obvious contrast; you switch to this color scheme by using the Display Options in the control panel. 
			/// The High Contrast Mode, which uses SPI_GETHIGHCONTRAST and SPI_SETHIGHCONTRAST, advises applications to modify their appearance 
			/// for visually-impaired users. It involves such things as audible warning to users and customized color scheme 
			/// (using the Accessibility Options in the control panel). For more information, see HIGHCONTRAST on MSDN.
			/// For more information on general accessibility features, see Accessibility on MSDN.
			/// </remarks>
			SPI_GETHIGHCONTRAST = 0x0042,

			/// <summary>
			/// Sets the parameters of the HighContrast accessibility feature. The pvParam parameter must point to a HIGHCONTRAST structure 
			/// that contains the new parameters. Set the cbSize member of this structure and the uiParam parameter to sizeof(HIGHCONTRAST).
			/// Windows NT:  This value is not supported.
			/// </summary>
			SPI_SETHIGHCONTRAST = 0x0043,

			/// <summary>
			/// Determines whether the user relies on the keyboard instead of the mouse, and wants applications to display keyboard interfaces 
			/// that would otherwise be hidden. The pvParam parameter must point to a BOOL variable that receives TRUE 
			/// if the user relies on the keyboard; or FALSE otherwise.
			/// Windows NT:  This value is not supported.
			/// </summary>
			SPI_GETKEYBOARDPREF = 0x0044,

			/// <summary>
			/// Sets the keyboard preference. The uiParam parameter specifies TRUE if the user relies on the keyboard instead of the mouse, 
			/// and wants applications to display keyboard interfaces that would otherwise be hidden; uiParam is FALSE otherwise.
			/// Windows NT:  This value is not supported.
			/// </summary>
			SPI_SETKEYBOARDPREF = 0x0045,

			/// <summary>
			/// Determines whether a screen reviewer utility is running. A screen reviewer utility directs textual information to an output device, 
			/// such as a speech synthesizer or Braille display. When this flag is set, an application should provide textual information 
			/// in situations where it would otherwise present the information graphically.
			/// The pvParam parameter is a pointer to a BOOL variable that receives TRUE if a screen reviewer utility is running, or FALSE otherwise.
			/// Windows NT:  This value is not supported.
			/// </summary>
			SPI_GETSCREENREADER = 0x0046,

			/// <summary>
			/// Determines whether a screen review utility is running. The uiParam parameter specifies TRUE for on, or FALSE for off.
			/// Windows NT:  This value is not supported.
			/// </summary>
			SPI_SETSCREENREADER = 0x0047,

			/// <summary>
			/// Retrieves the animation effects associated with user actions. The pvParam parameter must point to an ANIMATIONINFO structure 
			/// that receives the information. Set the cbSize member of this structure and the uiParam parameter to sizeof(ANIMATIONINFO).
			/// </summary>
			SPI_GETANIMATION = 0x0048,

			/// <summary>
			/// Sets the animation effects associated with user actions. The pvParam parameter must point to an ANIMATIONINFO structure 
			/// that contains the new parameters. Set the cbSize member of this structure and the uiParam parameter to sizeof(ANIMATIONINFO).
			/// </summary>
			SPI_SETANIMATION = 0x0049,

			/// <summary>
			/// Determines whether the font smoothing feature is enabled. This feature uses font antialiasing to make font curves appear smoother 
			/// by painting pixels at different gray levels. 
			/// The pvParam parameter must point to a BOOL variable that receives TRUE if the feature is enabled, or FALSE if it is not.
			/// Windows 95:  This flag is supported only if Windows Plus! is installed. See SPI_GETWINDOWSEXTENSION.
			/// </summary>
			SPI_GETFONTSMOOTHING = 0x004A,

			/// <summary>
			/// Enables or disables the font smoothing feature, which uses font antialiasing to make font curves appear smoother 
			/// by painting pixels at different gray levels. 
			/// To enable the feature, set the uiParam parameter to TRUE. To disable the feature, set uiParam to FALSE.
			/// Windows 95:  This flag is supported only if Windows Plus! is installed. See SPI_GETWINDOWSEXTENSION.
			/// </summary>
			SPI_SETFONTSMOOTHING = 0x004B,

			/// <summary>
			/// Sets the width, in pixels, of the rectangle used to detect the start of a drag operation. Set uiParam to the new value. 
			/// To retrieve the drag width, call GetSystemMetrics with the SM_CXDRAG flag.
			/// </summary>
			SPI_SETDRAGWIDTH = 0x004C,

			/// <summary>
			/// Sets the height, in pixels, of the rectangle used to detect the start of a drag operation. Set uiParam to the new value. 
			/// To retrieve the drag height, call GetSystemMetrics with the SM_CYDRAG flag.
			/// </summary>
			SPI_SETDRAGHEIGHT = 0x004D,

			/// <summary>
			/// Used internally; applications should not use this value.
			/// </summary>
			SPI_SETHANDHELD = 0x004E,

			/// <summary>
			/// Retrieves the time-out value for the low-power phase of screen saving. The pvParam parameter must point to an integer variable 
			/// that receives the value. This flag is supported for 32-bit applications only.
			/// Windows NT, Windows Me/98:  This flag is supported for 16-bit and 32-bit applications.
			/// Windows 95:  This flag is supported for 16-bit applications only.
			/// </summary>
			SPI_GETLOWPOWERTIMEOUT = 0x004F,

			/// <summary>
			/// Retrieves the time-out value for the power-off phase of screen saving. The pvParam parameter must point to an integer variable 
			/// that receives the value. This flag is supported for 32-bit applications only.
			/// Windows NT, Windows Me/98:  This flag is supported for 16-bit and 32-bit applications.
			/// Windows 95:  This flag is supported for 16-bit applications only.
			/// </summary>
			SPI_GETPOWEROFFTIMEOUT = 0x0050,

			/// <summary>
			/// Sets the time-out value, in seconds, for the low-power phase of screen saving. The uiParam parameter specifies the new value. 
			/// The pvParam parameter must be null. This flag is supported for 32-bit applications only.
			/// Windows NT, Windows Me/98:  This flag is supported for 16-bit and 32-bit applications.
			/// Windows 95:  This flag is supported for 16-bit applications only.
			/// </summary>
			SPI_SETLOWPOWERTIMEOUT = 0x0051,

			/// <summary>
			/// Sets the time-out value, in seconds, for the power-off phase of screen saving. The uiParam parameter specifies the new value. 
			/// The pvParam parameter must be null. This flag is supported for 32-bit applications only.
			/// Windows NT, Windows Me/98:  This flag is supported for 16-bit and 32-bit applications.
			/// Windows 95:  This flag is supported for 16-bit applications only.
			/// </summary>
			SPI_SETPOWEROFFTIMEOUT = 0x0052,

			/// <summary>
			/// Determines whether the low-power phase of screen saving is enabled. The pvParam parameter must point to a BOOL variable 
			/// that receives TRUE if enabled, or FALSE if disabled. This flag is supported for 32-bit applications only.
			/// Windows NT, Windows Me/98:  This flag is supported for 16-bit and 32-bit applications.
			/// Windows 95:  This flag is supported for 16-bit applications only.
			/// </summary>
			SPI_GETLOWPOWERACTIVE = 0x0053,

			/// <summary>
			/// Determines whether the power-off phase of screen saving is enabled. The pvParam parameter must point to a BOOL variable 
			/// that receives TRUE if enabled, or FALSE if disabled. This flag is supported for 32-bit applications only.
			/// Windows NT, Windows Me/98:  This flag is supported for 16-bit and 32-bit applications.
			/// Windows 95:  This flag is supported for 16-bit applications only.
			/// </summary>
			SPI_GETPOWEROFFACTIVE = 0x0054,

			/// <summary>
			/// Activates or deactivates the low-power phase of screen saving. Set uiParam to 1 to activate, or zero to deactivate. 
			/// The pvParam parameter must be null. This flag is supported for 32-bit applications only.
			/// Windows NT, Windows Me/98:  This flag is supported for 16-bit and 32-bit applications.
			/// Windows 95:  This flag is supported for 16-bit applications only.
			/// </summary>
			SPI_SETLOWPOWERACTIVE = 0x0055,

			/// <summary>
			/// Activates or deactivates the power-off phase of screen saving. Set uiParam to 1 to activate, or zero to deactivate. 
			/// The pvParam parameter must be null. This flag is supported for 32-bit applications only.
			/// Windows NT, Windows Me/98:  This flag is supported for 16-bit and 32-bit applications.
			/// Windows 95:  This flag is supported for 16-bit applications only.
			/// </summary>
			SPI_SETPOWEROFFACTIVE = 0x0056,

			/// <summary>
			/// Reloads the system cursors. Set the uiParam parameter to zero and the pvParam parameter to null.
			/// </summary>
			SPI_SETCURSORS = 0x0057,

			/// <summary>
			/// Reloads the system icons. Set the uiParam parameter to zero and the pvParam parameter to null.
			/// </summary>
			SPI_SETICONS = 0x0058,

			/// <summary>
			/// Retrieves the input locale identifier for the system default input language. The pvParam parameter must point 
			/// to an HKL variable that receives this value. For more information, see Languages, Locales, and Keyboard Layouts on MSDN.
			/// </summary>
			SPI_GETDEFAULTINPUTLANG = 0x0059,

			/// <summary>
			/// Sets the default input language for the system shell and applications. The specified language must be displayable 
			/// using the current system character set. The pvParam parameter must point to an HKL variable that contains 
			/// the input locale identifier for the default language. For more information, see Languages, Locales, and Keyboard Layouts on MSDN.
			/// </summary>
			SPI_SETDEFAULTINPUTLANG = 0x005A,

			/// <summary>
			/// Sets the hot key set for switching between input languages. The uiParam and pvParam parameters are not used. 
			/// The value sets the shortcut keys in the keyboard property sheets by reading the registry again. The registry must be set before this flag is used. the path in the registry is \HKEY_CURRENT_USER\keyboard layout\toggle. Valid values are "1" = ALT+SHIFT, "2" = CTRL+SHIFT, and "3" = none.
			/// </summary>
			SPI_SETLANGTOGGLE = 0x005B,

			/// <summary>
			/// Windows 95:  Determines whether the Windows extension, Windows Plus!, is installed. Set the uiParam parameter to 1. 
			/// The pvParam parameter is not used. The function returns TRUE if the extension is installed, or FALSE if it is not.
			/// </summary>
			SPI_GETWINDOWSEXTENSION = 0x005C,

			/// <summary>
			/// Enables or disables the Mouse Trails feature, which improves the visibility of mouse cursor movements by briefly showing 
			/// a trail of cursors and quickly erasing them. 
			/// To disable the feature, set the uiParam parameter to zero or 1. To enable the feature, set uiParam to a value greater than 1 
			/// to indicate the number of cursors drawn in the trail.
			/// Windows 2000/NT:  This value is not supported.
			/// </summary>
			SPI_SETMOUSETRAILS = 0x005D,

			/// <summary>
			/// Determines whether the Mouse Trails feature is enabled. This feature improves the visibility of mouse cursor movements 
			/// by briefly showing a trail of cursors and quickly erasing them. 
			/// The pvParam parameter must point to an integer variable that receives a value. If the value is zero or 1, the feature is disabled. 
			/// If the value is greater than 1, the feature is enabled and the value indicates the number of cursors drawn in the trail. 
			/// The uiParam parameter is not used.
			/// Windows 2000/NT:  This value is not supported.
			/// </summary>
			SPI_GETMOUSETRAILS = 0x005E,

			/// <summary>
			/// Windows Me/98:  Used internally; applications should not use this flag.
			/// </summary>
			SPI_SETSCREENSAVERRUNNING = 0x0061,

			/// <summary>
			/// Same as SPI_SETSCREENSAVERRUNNING.
			/// </summary>
			SPI_SCREENSAVERRUNNING = SPI_SETSCREENSAVERRUNNING,
			//#endif /* WINVER >= 0x0400 */

			/// <summary>
			/// Retrieves information about the FilterKeys accessibility feature. The pvParam parameter must point to a FILTERKEYS structure 
			/// that receives the information. Set the cbSize member of this structure and the uiParam parameter to sizeof(FILTERKEYS).
			/// </summary>
			SPI_GETFILTERKEYS = 0x0032,

			/// <summary>
			/// Sets the parameters of the FilterKeys accessibility feature. The pvParam parameter must point to a FILTERKEYS structure 
			/// that contains the new parameters. Set the cbSize member of this structure and the uiParam parameter to sizeof(FILTERKEYS).
			/// </summary>
			SPI_SETFILTERKEYS = 0x0033,

			/// <summary>
			/// Retrieves information about the ToggleKeys accessibility feature. The pvParam parameter must point to a TOGGLEKEYS structure 
			/// that receives the information. Set the cbSize member of this structure and the uiParam parameter to sizeof(TOGGLEKEYS).
			/// </summary>
			SPI_GETTOGGLEKEYS = 0x0034,

			/// <summary>
			/// Sets the parameters of the ToggleKeys accessibility feature. The pvParam parameter must point to a TOGGLEKEYS structure 
			/// that contains the new parameters. Set the cbSize member of this structure and the uiParam parameter to sizeof(TOGGLEKEYS).
			/// </summary>
			SPI_SETTOGGLEKEYS = 0x0035,

			/// <summary>
			/// Retrieves information about the MouseKeys accessibility feature. The pvParam parameter must point to a MOUSEKEYS structure 
			/// that receives the information. Set the cbSize member of this structure and the uiParam parameter to sizeof(MOUSEKEYS).
			/// </summary>
			SPI_GETMOUSEKEYS = 0x0036,

			/// <summary>
			/// Sets the parameters of the MouseKeys accessibility feature. The pvParam parameter must point to a MOUSEKEYS structure 
			/// that contains the new parameters. Set the cbSize member of this structure and the uiParam parameter to sizeof(MOUSEKEYS).
			/// </summary>
			SPI_SETMOUSEKEYS = 0x0037,

			/// <summary>
			/// Determines whether the Show Sounds accessibility flag is on or off. If it is on, the user requires an application 
			/// to present information visually in situations where it would otherwise present the information only in audible form. 
			/// The pvParam parameter must point to a BOOL variable that receives TRUE if the feature is on, or FALSE if it is off. 
			/// Using this value is equivalent to calling GetSystemMetrics (SM_SHOWSOUNDS). That is the recommended call.
			/// </summary>
			SPI_GETSHOWSOUNDS = 0x0038,

			/// <summary>
			/// Sets the parameters of the SoundSentry accessibility feature. The pvParam parameter must point to a SOUNDSENTRY structure 
			/// that contains the new parameters. Set the cbSize member of this structure and the uiParam parameter to sizeof(SOUNDSENTRY).
			/// </summary>
			SPI_SETSHOWSOUNDS = 0x0039,

			/// <summary>
			/// Retrieves information about the StickyKeys accessibility feature. The pvParam parameter must point to a STICKYKEYS structure 
			/// that receives the information. Set the cbSize member of this structure and the uiParam parameter to sizeof(STICKYKEYS).
			/// </summary>
			SPI_GETSTICKYKEYS = 0x003A,

			/// <summary>
			/// Sets the parameters of the StickyKeys accessibility feature. The pvParam parameter must point to a STICKYKEYS structure 
			/// that contains the new parameters. Set the cbSize member of this structure and the uiParam parameter to sizeof(STICKYKEYS).
			/// </summary>
			SPI_SETSTICKYKEYS = 0x003B,

			/// <summary>
			/// Retrieves information about the time-out period associated with the accessibility features. The pvParam parameter must point 
			/// to an ACCESSTIMEOUT structure that receives the information. Set the cbSize member of this structure and the uiParam parameter 
			/// to sizeof(ACCESSTIMEOUT).
			/// </summary>
			SPI_GETACCESSTIMEOUT = 0x003C,

			/// <summary>
			/// Sets the time-out period associated with the accessibility features. The pvParam parameter must point to an ACCESSTIMEOUT 
			/// structure that contains the new parameters. Set the cbSize member of this structure and the uiParam parameter to sizeof(ACCESSTIMEOUT).
			/// </summary>
			SPI_SETACCESSTIMEOUT = 0x003D,

			//#if(WINVER >= 0x0400)
			/// <summary>
			/// Windows Me/98/95:  Retrieves information about the SerialKeys accessibility feature. The pvParam parameter must point 
			/// to a SERIALKEYS structure that receives the information. Set the cbSize member of this structure and the uiParam parameter 
			/// to sizeof(SERIALKEYS).
			/// Windows Server 2003, Windows XP/2000/NT:  Not supported. The user controls this feature through the control panel.
			/// </summary>
			SPI_GETSERIALKEYS = 0x003E,

			/// <summary>
			/// Windows Me/98/95:  Sets the parameters of the SerialKeys accessibility feature. The pvParam parameter must point 
			/// to a SERIALKEYS structure that contains the new parameters. Set the cbSize member of this structure and the uiParam parameter 
			/// to sizeof(SERIALKEYS). 
			/// Windows Server 2003, Windows XP/2000/NT:  Not supported. The user controls this feature through the control panel.
			/// </summary>
			SPI_SETSERIALKEYS = 0x003F,
			//#endif /* WINVER >= 0x0400 */ 

			/// <summary>
			/// Retrieves information about the SoundSentry accessibility feature. The pvParam parameter must point to a SOUNDSENTRY structure 
			/// that receives the information. Set the cbSize member of this structure and the uiParam parameter to sizeof(SOUNDSENTRY).
			/// </summary>
			SPI_GETSOUNDSENTRY = 0x0040,

			/// <summary>
			/// Sets the parameters of the SoundSentry accessibility feature. The pvParam parameter must point to a SOUNDSENTRY structure 
			/// that contains the new parameters. Set the cbSize member of this structure and the uiParam parameter to sizeof(SOUNDSENTRY).
			/// </summary>
			SPI_SETSOUNDSENTRY = 0x0041,

			//#if(_WIN32_WINNT >= 0x0400)
			/// <summary>
			/// Determines whether the snap-to-default-button feature is enabled. If enabled, the mouse cursor automatically moves 
			/// to the default button, such as OK or Apply, of a dialog box. The pvParam parameter must point to a BOOL variable 
			/// that receives TRUE if the feature is on, or FALSE if it is off. 
			/// Windows 95:  Not supported.
			/// </summary>
			SPI_GETSNAPTODEFBUTTON = 0x005F,

			/// <summary>
			/// Enables or disables the snap-to-default-button feature. If enabled, the mouse cursor automatically moves to the default button, 
			/// such as OK or Apply, of a dialog box. Set the uiParam parameter to TRUE to enable the feature, or FALSE to disable it. 
			/// Applications should use the ShowWindow function when displaying a dialog box so the dialog manager can position the mouse cursor. 
			/// Windows 95:  Not supported.
			/// </summary>
			SPI_SETSNAPTODEFBUTTON = 0x0060,
			//#endif /* _WIN32_WINNT >= 0x0400 */

			//#if (_WIN32_WINNT >= 0x0400) || (_WIN32_WINDOWS > 0x0400)
			/// <summary>
			/// Retrieves the width, in pixels, of the rectangle within which the mouse pointer has to stay for TrackMouseEvent 
			/// to generate a WM_MOUSEHOVER message. The pvParam parameter must point to a UINT variable that receives the width. 
			/// Windows 95:  Not supported.
			/// </summary>
			SPI_GETMOUSEHOVERWIDTH = 0x0062,

			/// <summary>
			/// Retrieves the width, in pixels, of the rectangle within which the mouse pointer has to stay for TrackMouseEvent 
			/// to generate a WM_MOUSEHOVER message. The pvParam parameter must point to a UINT variable that receives the width. 
			/// Windows 95:  Not supported.
			/// </summary>
			SPI_SETMOUSEHOVERWIDTH = 0x0063,

			/// <summary>
			/// Retrieves the height, in pixels, of the rectangle within which the mouse pointer has to stay for TrackMouseEvent 
			/// to generate a WM_MOUSEHOVER message. The pvParam parameter must point to a UINT variable that receives the height. 
			/// Windows 95:  Not supported.
			/// </summary>
			SPI_GETMOUSEHOVERHEIGHT = 0x0064,

			/// <summary>
			/// Sets the height, in pixels, of the rectangle within which the mouse pointer has to stay for TrackMouseEvent 
			/// to generate a WM_MOUSEHOVER message. Set the uiParam parameter to the new height.
			/// Windows 95:  Not supported.
			/// </summary>
			SPI_SETMOUSEHOVERHEIGHT = 0x0065,

			/// <summary>
			/// Retrieves the time, in milliseconds, that the mouse pointer has to stay in the hover rectangle for TrackMouseEvent 
			/// to generate a WM_MOUSEHOVER message. The pvParam parameter must point to a UINT variable that receives the time. 
			/// Windows 95:  Not supported.
			/// </summary>
			SPI_GETMOUSEHOVERTIME = 0x0066,

			/// <summary>
			/// Sets the time, in milliseconds, that the mouse pointer has to stay in the hover rectangle for TrackMouseEvent 
			/// to generate a WM_MOUSEHOVER message. This is used only if you pass HOVER_DEFAULT in the dwHoverTime parameter in the call to TrackMouseEvent. Set the uiParam parameter to the new time. 
			/// Windows 95:  Not supported.
			/// </summary>
			SPI_SETMOUSEHOVERTIME = 0x0067,

			/// <summary>
			/// Retrieves the number of lines to scroll when the mouse wheel is rotated. The pvParam parameter must point 
			/// to a UINT variable that receives the number of lines. The default value is 3. 
			/// Windows 95:  Not supported.
			/// </summary>
			SPI_GETWHEELSCROLLLINES = 0x0068,

			/// <summary>
			/// Sets the number of lines to scroll when the mouse wheel is rotated. The number of lines is set from the uiParam parameter. 
			/// The number of lines is the suggested number of lines to scroll when the mouse wheel is rolled without using modifier keys. 
			/// If the number is 0, then no scrolling should occur. If the number of lines to scroll is greater than the number of lines viewable, 
			/// and in particular if it is WHEEL_PAGESCROLL (		public const byted as UINT_MAX), the scroll operation should be interpreted 
			/// as clicking once in the page down or page up regions of the scroll bar.
			/// Windows 95:  Not supported.
			/// </summary>
			SPI_SETWHEELSCROLLLINES = 0x0069,

			/// <summary>
			/// Retrieves the time, in milliseconds, that the system waits before displaying a shortcut menu when the mouse cursor is 
			/// over a submenu item. The pvParam parameter must point to a DWORD variable that receives the time of the delay. 
			/// Windows 95:  Not supported.
			/// </summary>
			SPI_GETMENUSHOWDELAY = 0x006A,

			/// <summary>
			/// Sets uiParam to the time, in milliseconds, that the system waits before displaying a shortcut menu when the mouse cursor is 
			/// over a submenu item. 
			/// Windows 95:  Not supported.
			/// </summary>
			SPI_SETMENUSHOWDELAY = 0x006B,

			/// <summary>
			/// Determines whether the IME status window is visible (on a per-user basis). The pvParam parameter must point to a BOOL variable 
			/// that receives TRUE if the status window is visible, or FALSE if it is not.
			/// Windows NT, Windows 95:  This value is not supported.
			/// </summary>
			SPI_GETSHOWIMEUI = 0x006E,

			/// <summary>
			/// Sets whether the IME status window is visible or not on a per-user basis. The uiParam parameter specifies TRUE for on or FALSE for off.
			/// Windows NT, Windows 95:  This value is not supported.
			/// </summary>
			SPI_SETSHOWIMEUI = 0x006F,
			//#endif

			//#if(WINVER >= 0x0500)
			/// <summary>
			/// Retrieves the current mouse speed. The mouse speed determines how far the pointer will move based on the distance the mouse moves. 
			/// The pvParam parameter must point to an integer that receives a value which ranges between 1 (slowest) and 20 (fastest). 
			/// A value of 10 is the default. The value can be set by an end user using the mouse control panel application or 
			/// by an application using SPI_SETMOUSESPEED.
			/// Windows NT, Windows 95:  This value is not supported.
			/// </summary>
			SPI_GETMOUSESPEED = 0x0070,

			/// <summary>
			/// Sets the current mouse speed. The pvParam parameter is an integer between 1 (slowest) and 20 (fastest). A value of 10 is the default. 
			/// This value is typically set using the mouse control panel application.
			/// Windows NT, Windows 95:  This value is not supported.
			/// </summary>
			SPI_SETMOUSESPEED = 0x0071,

			/// <summary>
			/// Determines whether a screen saver is currently running on the window station of the calling process. 
			/// The pvParam parameter must point to a BOOL variable that receives TRUE if a screen saver is currently running, or FALSE otherwise.
			/// Windows NT, Windows 95:  This value is not supported.
			/// </summary>
			SPI_GETSCREENSAVERRUNNING = 0x0072,

			/// <summary>
			/// Retrieves the full path of the bitmap file for the desktop wallpaper. The pvParam parameter must point to a buffer 
			/// that receives a null-terminated path string. Set the uiParam parameter to the size, in characters, of the pvParam buffer. The returned string will not exceed MAX_PATH characters. If there is no desktop wallpaper, the returned string is empty.
			/// Windows NT, Windows Me/98/95:  This value is not supported.
			/// </summary>
			SPI_GETDESKWALLPAPER = 0x0073,
			//#endif /* WINVER >= 0x0500 */

			//#if(WINVER >= 0x0500)
			/// <summary>
			/// Determines whether active window tracking (activating the window the mouse is on) is on or off. The pvParam parameter must point 
			/// to a BOOL variable that receives TRUE for on, or FALSE for off.
			/// Windows NT, Windows 95:  This value is not supported.
			/// </summary>
			SPI_GETACTIVEWINDOWTRACKING = 0x1000,

			/// <summary>
			/// Sets active window tracking (activating the window the mouse is on) either on or off. Set pvParam to TRUE for on or FALSE for off.
			/// Windows NT, Windows 95:  This value is not supported.
			/// </summary>
			SPI_SETACTIVEWINDOWTRACKING = 0x1001,

			/// <summary>
			/// Determines whether the menu animation feature is enabled. This master switch must be on to enable menu animation effects. 
			/// The pvParam parameter must point to a BOOL variable that receives TRUE if animation is enabled and FALSE if it is disabled. 
			/// If animation is enabled, SPI_GETMENUFADE indicates whether menus use fade or slide animation.
			/// Windows NT, Windows 95:  This value is not supported.
			/// </summary>
			SPI_GETMENUANIMATION = 0x1002,

			/// <summary>
			/// Enables or disables menu animation. This master switch must be on for any menu animation to occur. 
			/// The pvParam parameter is a BOOL variable; set pvParam to TRUE to enable animation and FALSE to disable animation.
			/// If animation is enabled, SPI_GETMENUFADE indicates whether menus use fade or slide animation.
			/// Windows NT, Windows 95:  This value is not supported.
			/// </summary>
			SPI_SETMENUANIMATION = 0x1003,

			/// <summary>
			/// Determines whether the slide-open effect for combo boxes is enabled. The pvParam parameter must point to a BOOL variable 
			/// that receives TRUE for enabled, or FALSE for disabled.
			/// Windows NT, Windows 95:  This value is not supported.
			/// </summary>
			SPI_GETCOMBOBOXANIMATION = 0x1004,

			/// <summary>
			/// Enables or disables the slide-open effect for combo boxes. Set the pvParam parameter to TRUE to enable the gradient effect, 
			/// or FALSE to disable it.
			/// Windows NT, Windows 95:  This value is not supported.
			/// </summary>
			SPI_SETCOMBOBOXANIMATION = 0x1005,

			/// <summary>
			/// Determines whether the smooth-scrolling effect for list boxes is enabled. The pvParam parameter must point to a BOOL variable 
			/// that receives TRUE for enabled, or FALSE for disabled.
			/// Windows NT, Windows 95:  This value is not supported.
			/// </summary>
			SPI_GETLISTBOXSMOOTHSCROLLING = 0x1006,

			/// <summary>
			/// Enables or disables the smooth-scrolling effect for list boxes. Set the pvParam parameter to TRUE to enable the smooth-scrolling effect,
			/// or FALSE to disable it.
			/// Windows NT, Windows 95:  This value is not supported.
			/// </summary>
			SPI_SETLISTBOXSMOOTHSCROLLING = 0x1007,

			/// <summary>
			/// Determines whether the gradient effect for window title bars is enabled. The pvParam parameter must point to a BOOL variable 
			/// that receives TRUE for enabled, or FALSE for disabled. For more information about the gradient effect, see the GetSysColor function.
			/// Windows NT, Windows 95:  This value is not supported.
			/// </summary>
			SPI_GETGRADIENTCAPTIONS = 0x1008,

			/// <summary>
			/// Enables or disables the gradient effect for window title bars. Set the pvParam parameter to TRUE to enable it, or FALSE to disable it. 
			/// The gradient effect is possible only if the system has a color depth of more than 256 colors. For more information about 
			/// the gradient effect, see the GetSysColor function.
			/// Windows NT, Windows 95:  This value is not supported.
			/// </summary>
			SPI_SETGRADIENTCAPTIONS = 0x1009,

			/// <summary>
			/// Determines whether menu access keys are always underlined. The pvParam parameter must point to a BOOL variable that receives TRUE 
			/// if menu access keys are always underlined, and FALSE if they are underlined only when the menu is activated by the keyboard.
			/// Windows NT, Windows 95:  This value is not supported.
			/// </summary>
			SPI_GETKEYBOARDCUES = 0x100A,

			/// <summary>
			/// Sets the underlining of menu access key letters. The pvParam parameter is a BOOL variable. Set pvParam to TRUE to always underline menu 
			/// access keys, or FALSE to underline menu access keys only when the menu is activated from the keyboard.
			/// Windows NT, Windows 95:  This value is not supported.
			/// </summary>
			SPI_SETKEYBOARDCUES = 0x100B,

			/// <summary>
			/// Same as SPI_GETKEYBOARDCUES.
			/// </summary>
			SPI_GETMENUUNDERLINES = SPI_GETKEYBOARDCUES,

			/// <summary>
			/// Same as SPI_SETKEYBOARDCUES.
			/// </summary>
			SPI_SETMENUUNDERLINES = SPI_SETKEYBOARDCUES,

			/// <summary>
			/// Determines whether windows activated through active window tracking will be brought to the top. The pvParam parameter must point 
			/// to a BOOL variable that receives TRUE for on, or FALSE for off.
			/// Windows NT, Windows 95:  This value is not supported.
			/// </summary>
			SPI_GETACTIVEWNDTRKZORDER = 0x100C,

			/// <summary>
			/// Determines whether or not windows activated through active window tracking should be brought to the top. Set pvParam to TRUE 
			/// for on or FALSE for off.
			/// Windows NT, Windows 95:  This value is not supported.
			/// </summary>
			SPI_SETACTIVEWNDTRKZORDER = 0x100D,

			/// <summary>
			/// Determines whether hot tracking of user-interface elements, such as menu names on menu bars, is enabled. The pvParam parameter 
			/// must point to a BOOL variable that receives TRUE for enabled, or FALSE for disabled. 
			/// Hot tracking means that when the cursor moves over an item, it is highlighted but not selected. You can query this value to decide 
			/// whether to use hot tracking in the user interface of your application.
			/// Windows NT, Windows 95:  This value is not supported.
			/// </summary>
			SPI_GETHOTTRACKING = 0x100E,

			/// <summary>
			/// Enables or disables hot tracking of user-interface elements such as menu names on menu bars. Set the pvParam parameter to TRUE 
			/// to enable it, or FALSE to disable it.
			/// Hot-tracking means that when the cursor moves over an item, it is highlighted but not selected.
			/// Windows NT, Windows 95:  This value is not supported.
			/// </summary>
			SPI_SETHOTTRACKING = 0x100F,

			/// <summary>
			/// Determines whether menu fade animation is enabled. The pvParam parameter must point to a BOOL variable that receives TRUE 
			/// when fade animation is enabled and FALSE when it is disabled. If fade animation is disabled, menus use slide animation. 
			/// This flag is ignored unless menu animation is enabled, which you can do using the SPI_SETMENUANIMATION flag. 
			/// For more information, see AnimateWindow.
			/// Windows NT, Windows Me/98/95:  This value is not supported.
			/// </summary>
			SPI_GETMENUFADE = 0x1012,

			/// <summary>
			/// Enables or disables menu fade animation. Set pvParam to TRUE to enable the menu fade effect or FALSE to disable it. 
			/// If fade animation is disabled, menus use slide animation. he The menu fade effect is possible only if the system 
			/// has a color depth of more than 256 colors. This flag is ignored unless SPI_MENUANIMATION is also set. For more information, 
			/// see AnimateWindow.
			/// Windows NT, Windows Me/98/95:  This value is not supported.
			/// </summary>
			SPI_SETMENUFADE = 0x1013,

			/// <summary>
			/// Determines whether the selection fade effect is enabled. The pvParam parameter must point to a BOOL variable that receives TRUE 
			/// if enabled or FALSE if disabled. 
			/// The selection fade effect causes the menu item selected by the user to remain on the screen briefly while fading out 
			/// after the menu is dismissed.
			/// Windows NT, Windows Me/98/95:  This value is not supported.
			/// </summary>
			SPI_GETSELECTIONFADE = 0x1014,

			/// <summary>
			/// Set pvParam to TRUE to enable the selection fade effect or FALSE to disable it.
			/// The selection fade effect causes the menu item selected by the user to remain on the screen briefly while fading out 
			/// after the menu is dismissed. The selection fade effect is possible only if the system has a color depth of more than 256 colors.
			/// Windows NT, Windows Me/98/95:  This value is not supported.
			/// </summary>
			SPI_SETSELECTIONFADE = 0x1015,

			/// <summary>
			/// Determines whether ToolTip animation is enabled. The pvParam parameter must point to a BOOL variable that receives TRUE 
			/// if enabled or FALSE if disabled. If ToolTip animation is enabled, SPI_GETTOOLTIPFADE indicates whether ToolTips use fade or slide animation.
			/// Windows NT, Windows Me/98/95:  This value is not supported.
			/// </summary>
			SPI_GETTOOLTIPANIMATION = 0x1016,

			/// <summary>
			/// Set pvParam to TRUE to enable ToolTip animation or FALSE to disable it. If enabled, you can use SPI_SETTOOLTIPFADE 
			/// to specify fade or slide animation.
			/// Windows NT, Windows Me/98/95:  This value is not supported.
			/// </summary>
			SPI_SETTOOLTIPANIMATION = 0x1017,

			/// <summary>
			/// If SPI_SETTOOLTIPANIMATION is enabled, SPI_GETTOOLTIPFADE indicates whether ToolTip animation uses a fade effect or a slide effect.
			///  The pvParam parameter must point to a BOOL variable that receives TRUE for fade animation or FALSE for slide animation. 
			///  For more information on slide and fade effects, see AnimateWindow.
			/// Windows NT, Windows Me/98/95:  This value is not supported.
			/// </summary>
			SPI_GETTOOLTIPFADE = 0x1018,

			/// <summary>
			/// If the SPI_SETTOOLTIPANIMATION flag is enabled, use SPI_SETTOOLTIPFADE to indicate whether ToolTip animation uses a fade effect 
			/// or a slide effect. Set pvParam to TRUE for fade animation or FALSE for slide animation. The tooltip fade effect is possible only 
			/// if the system has a color depth of more than 256 colors. For more information on the slide and fade effects, 
			/// see the AnimateWindow function.
			/// Windows NT, Windows Me/98/95:  This value is not supported.
			/// </summary>
			SPI_SETTOOLTIPFADE = 0x1019,

			/// <summary>
			/// Determines whether the cursor has a shadow around it. The pvParam parameter must point to a BOOL variable that receives TRUE 
			/// if the shadow is enabled, FALSE if it is disabled. This effect appears only if the system has a color depth of more than 256 colors.
			/// Windows NT, Windows Me/98/95:  This value is not supported.
			/// </summary>
			SPI_GETCURSORSHADOW = 0x101A,

			/// <summary>
			/// Enables or disables a shadow around the cursor. The pvParam parameter is a BOOL variable. Set pvParam to TRUE to enable the shadow 
			/// or FALSE to disable the shadow. This effect appears only if the system has a color depth of more than 256 colors.
			/// Windows NT, Windows Me/98/95:  This value is not supported.
			/// </summary>
			SPI_SETCURSORSHADOW = 0x101B,

			//#if(_WIN32_WINNT >= 0x0501)
			/// <summary>
			/// Retrieves the state of the Mouse Sonar feature. The pvParam parameter must point to a BOOL variable that receives TRUE 
			/// if enabled or FALSE otherwise. For more information, see About Mouse Input on MSDN.
			/// Windows 2000/NT, Windows 98/95:  This value is not supported.
			/// </summary>
			SPI_GETMOUSESONAR = 0x101C,

			/// <summary>
			/// Turns the Sonar accessibility feature on or off. This feature briefly shows several concentric circles around the mouse pointer 
			/// when the user presses and releases the CTRL key. The pvParam parameter specifies TRUE for on and FALSE for off. The default is off. 
			/// For more information, see About Mouse Input.
			/// Windows 2000/NT, Windows 98/95:  This value is not supported.
			/// </summary>
			SPI_SETMOUSESONAR = 0x101D,

			/// <summary>
			/// Retrieves the state of the Mouse ClickLock feature. The pvParam parameter must point to a BOOL variable that receives TRUE 
			/// if enabled, or FALSE otherwise. For more information, see About Mouse Input.
			/// Windows 2000/NT, Windows 98/95:  This value is not supported.
			/// </summary>
			SPI_GETMOUSECLICKLOCK = 0x101E,

			/// <summary>
			/// Turns the Mouse ClickLock accessibility feature on or off. This feature temporarily locks down the primary mouse button 
			/// when that button is clicked and held down for the time specified by SPI_SETMOUSECLICKLOCKTIME. The uiParam parameter specifies 
			/// TRUE for on, 
			/// or FALSE for off. The default is off. For more information, see Remarks and About Mouse Input on MSDN.
			/// Windows 2000/NT, Windows 98/95:  This value is not supported.
			/// </summary>
			SPI_SETMOUSECLICKLOCK = 0x101F,

			/// <summary>
			/// Retrieves the state of the Mouse Vanish feature. The pvParam parameter must point to a BOOL variable that receives TRUE 
			/// if enabled or FALSE otherwise. For more information, see About Mouse Input on MSDN.
			/// Windows 2000/NT, Windows 98/95:  This value is not supported.
			/// </summary>
			SPI_GETMOUSEVANISH = 0x1020,

			/// <summary>
			/// Turns the Vanish feature on or off. This feature hides the mouse pointer when the user types; the pointer reappears 
			/// when the user moves the mouse. The pvParam parameter specifies TRUE for on and FALSE for off. The default is off. 
			/// For more information, see About Mouse Input on MSDN.
			/// Windows 2000/NT, Windows 98/95:  This value is not supported.
			/// </summary>
			SPI_SETMOUSEVANISH = 0x1021,

			/// <summary>
			/// Determines whether native User menus have flat menu appearance. The pvParam parameter must point to a BOOL variable 
			/// that returns TRUE if the flat menu appearance is set, or FALSE otherwise.
			/// Windows 2000/NT, Windows Me/98/95:  This value is not supported.
			/// </summary>
			SPI_GETFLATMENU = 0x1022,

			/// <summary>
			/// Enables or disables flat menu appearance for native User menus. Set pvParam to TRUE to enable flat menu appearance 
			/// or FALSE to disable it. 
			/// When enabled, the menu bar uses COLOR_MENUBAR for the menubar background, COLOR_MENU for the menu-popup background, COLOR_MENUHILIGHT 
			/// for the fill of the current menu selection, and COLOR_HILIGHT for the outline of the current menu selection. 
			/// If disabled, menus are drawn using the same metrics and colors as in Windows 2000 and earlier.
			/// Windows 2000/NT, Windows Me/98/95:  This value is not supported.
			/// </summary>
			SPI_SETFLATMENU = 0x1023,

			/// <summary>
			/// Determines whether the drop shadow effect is enabled. The pvParam parameter must point to a BOOL variable that returns TRUE 
			/// if enabled or FALSE if disabled.
			/// Windows 2000/NT, Windows Me/98/95:  This value is not supported.
			/// </summary>
			SPI_GETDROPSHADOW = 0x1024,

			/// <summary>
			/// Enables or disables the drop shadow effect. Set pvParam to TRUE to enable the drop shadow effect or FALSE to disable it. 
			/// You must also have CS_DROPSHADOW in the window class style.
			/// Windows 2000/NT, Windows Me/98/95:  This value is not supported.
			/// </summary>
			SPI_SETDROPSHADOW = 0x1025,

			/// <summary>
			/// Retrieves a BOOL indicating whether an application can reset the screensaver's timer by calling the SendInput function 
			/// to simulate keyboard or mouse input. The pvParam parameter must point to a BOOL variable that receives TRUE 
			/// if the simulated input will be blocked, or FALSE otherwise. 
			/// </summary>
			SPI_GETBLOCKSENDINPUTRESETS = 0x1026,

			/// <summary>
			/// Determines whether an application can reset the screensaver's timer by calling the SendInput function to simulate keyboard 
			/// or mouse input. The uiParam parameter specifies TRUE if the screensaver will not be deactivated by simulated input, 
			/// or FALSE if the screensaver will be deactivated by simulated input.
			/// </summary>
			SPI_SETBLOCKSENDINPUTRESETS = 0x1027,
			//#endif /* _WIN32_WINNT >= 0x0501 */

			/// <summary>
			/// Determines whether UI effects are enabled or disabled. The pvParam parameter must point to a BOOL variable that receives TRUE 
			/// if all UI effects are enabled, or FALSE if they are disabled.
			/// Windows NT, Windows Me/98/95:  This value is not supported.
			/// </summary>
			SPI_GETUIEFFECTS = 0x103E,

			/// <summary>
			/// Enables or disables UI effects. Set the pvParam parameter to TRUE to enable all UI effects or FALSE to disable all UI effects.
			/// Windows NT, Windows Me/98/95:  This value is not supported.
			/// </summary>
			SPI_SETUIEFFECTS = 0x103F,

			/// <summary>
			/// Retrieves the amount of time following user input, in milliseconds, during which the system will not allow applications 
			/// to force themselves into the foreground. The pvParam parameter must point to a DWORD variable that receives the time.
			/// Windows NT, Windows 95:  This value is not supported.
			/// </summary>
			SPI_GETFOREGROUNDLOCKTIMEOUT = 0x2000,

			/// <summary>
			/// Sets the amount of time following user input, in milliseconds, during which the system does not allow applications 
			/// to force themselves into the foreground. Set pvParam to the new timeout value.
			/// The calling thread must be able to change the foreground window, otherwise the call fails.
			/// Windows NT, Windows 95:  This value is not supported.
			/// </summary>
			SPI_SETFOREGROUNDLOCKTIMEOUT = 0x2001,

			/// <summary>
			/// Retrieves the active window tracking delay, in milliseconds. The pvParam parameter must point to a DWORD variable 
			/// that receives the time.
			/// Windows NT, Windows 95:  This value is not supported.
			/// </summary>
			SPI_GETACTIVEWNDTRKTIMEOUT = 0x2002,

			/// <summary>
			/// Sets the active window tracking delay. Set pvParam to the number of milliseconds to delay before activating the window 
			/// under the mouse pointer.
			/// Windows NT, Windows 95:  This value is not supported.
			/// </summary>
			SPI_SETACTIVEWNDTRKTIMEOUT = 0x2003,

			/// <summary>
			/// Retrieves the number of times SetForegroundWindow will flash the taskbar button when rejecting a foreground switch request. 
			/// The pvParam parameter must point to a DWORD variable that receives the value.
			/// Windows NT, Windows 95:  This value is not supported.
			/// </summary>
			SPI_GETFOREGROUNDFLASHCOUNT = 0x2004,

			/// <summary>
			/// Sets the number of times SetForegroundWindow will flash the taskbar button when rejecting a foreground switch request. 
			/// Set pvParam to the number of times to flash.
			/// Windows NT, Windows 95:  This value is not supported.
			/// </summary>
			SPI_SETFOREGROUNDFLASHCOUNT = 0x2005,

			/// <summary>
			/// Retrieves the caret width in edit controls, in pixels. The pvParam parameter must point to a DWORD that receives this value.
			/// Windows NT, Windows Me/98/95:  This value is not supported.
			/// </summary>
			SPI_GETCARETWIDTH = 0x2006,

			/// <summary>
			/// Sets the caret width in edit controls. Set pvParam to the desired width, in pixels. The default and minimum value is 1.
			/// Windows NT, Windows Me/98/95:  This value is not supported.
			/// </summary>
			SPI_SETCARETWIDTH = 0x2007,

			//#if(_WIN32_WINNT >= 0x0501)
			/// <summary>
			/// Retrieves the time delay before the primary mouse button is locked. The pvParam parameter must point to DWORD that receives 
			/// the time delay. This is only enabled if SPI_SETMOUSECLICKLOCK is set to TRUE. For more information, see About Mouse Input on MSDN.
			/// Windows 2000/NT, Windows 98/95:  This value is not supported.
			/// </summary>
			SPI_GETMOUSECLICKLOCKTIME = 0x2008,

			/// <summary>
			/// Turns the Mouse ClickLock accessibility feature on or off. This feature temporarily locks down the primary mouse button 
			/// when that button is clicked and held down for the time specified by SPI_SETMOUSECLICKLOCKTIME. The uiParam parameter 
			/// specifies TRUE for on, or FALSE for off. The default is off. For more information, see Remarks and About Mouse Input on MSDN.
			/// Windows 2000/NT, Windows 98/95:  This value is not supported.
			/// </summary>
			SPI_SETMOUSECLICKLOCKTIME = 0x2009,

			/// <summary>
			/// Retrieves the type of font smoothing. The pvParam parameter must point to a UINT that receives the information.
			/// Windows 2000/NT, Windows Me/98/95:  This value is not supported.
			/// </summary>
			SPI_GETFONTSMOOTHINGTYPE = 0x200A,

			/// <summary>
			/// Sets the font smoothing type. The pvParam parameter points to a UINT that contains either FE_FONTSMOOTHINGSTANDARD, 
			/// if standard anti-aliasing is used, or FE_FONTSMOOTHINGCLEARTYPE, if ClearType is used. The default is FE_FONTSMOOTHINGSTANDARD. 
			/// When using this option, the fWinIni parameter must be set to SPIF_SENDWININICHANGE | SPIF_UPDATEINIFILE; otherwise, 
			/// SystemParametersInfo fails.
			/// </summary>
			SPI_SETFONTSMOOTHINGTYPE = 0x200B,

			/// <summary>
			/// Retrieves a contrast value that is used in ClearType™ smoothing. The pvParam parameter must point to a UINT 
			/// that receives the information.
			/// Windows 2000/NT, Windows Me/98/95:  This value is not supported.
			/// </summary>
			SPI_GETFONTSMOOTHINGCONTRAST = 0x200C,

			/// <summary>
			/// Sets the contrast value used in ClearType smoothing. The pvParam parameter points to a UINT that holds the contrast value. 
			/// Valid contrast values are from 1000 to 2200. The default value is 1400.
			/// When using this option, the fWinIni parameter must be set to SPIF_SENDWININICHANGE | SPIF_UPDATEINIFILE; otherwise, 
			/// SystemParametersInfo fails.
			/// SPI_SETFONTSMOOTHINGTYPE must also be set to FE_FONTSMOOTHINGCLEARTYPE.
			/// Windows 2000/NT, Windows Me/98/95:  This value is not supported.
			/// </summary>
			SPI_SETFONTSMOOTHINGCONTRAST = 0x200D,

			/// <summary>
			/// Retrieves the width, in pixels, of the left and right edges of the focus rectangle drawn with DrawFocusRect. 
			/// The pvParam parameter must point to a UINT.
			/// Windows 2000/NT, Windows Me/98/95:  This value is not supported.
			/// </summary>
			SPI_GETFOCUSBORDERWIDTH = 0x200E,

			/// <summary>
			/// Sets the height of the left and right edges of the focus rectangle drawn with DrawFocusRect to the value of the pvParam parameter.
			/// Windows 2000/NT, Windows Me/98/95:  This value is not supported.
			/// </summary>
			SPI_SETFOCUSBORDERWIDTH = 0x200F,

			/// <summary>
			/// Retrieves the height, in pixels, of the top and bottom edges of the focus rectangle drawn with DrawFocusRect. 
			/// The pvParam parameter must point to a UINT.
			/// Windows 2000/NT, Windows Me/98/95:  This value is not supported.
			/// </summary>
			SPI_GETFOCUSBORDERHEIGHT = 0x2010,

			/// <summary>
			/// Sets the height of the top and bottom edges of the focus rectangle drawn with DrawFocusRect to the value of the pvParam parameter.
			/// Windows 2000/NT, Windows Me/98/95:  This value is not supported.
			/// </summary>
			SPI_SETFOCUSBORDERHEIGHT = 0x2011,

			/// <summary>
			/// Not implemented.
			/// </summary>
			SPI_GETFONTSMOOTHINGORIENTATION = 0x2012,

			/// <summary>
			/// Not implemented.
			/// </summary>
			SPI_SETFONTSMOOTHINGORIENTATION = 0x2013,
		}

		#endregion // SPI

		#region __FCSTORAGEFLAGS enum

		[Flags]
		public enum __FCSTORAGEFLAGS {
			FCSF_READONLY = 0x00000001, // Open registry keys for reading only.
			FCSF_LOADDEFAULTS = 0x00000002, // Get settings from defaults provider if not saved in registry.
			FCSF_PROPAGATECHANGES = 0x00000004, // Propagate changes to font&color events handler.
			FCSF_NOAUTOCOLORS = 0x00000008 // Return real RGB values instead of COLORREF_AUTO
		}

		#endregion

		#region ClassStyles enum

		[Flags]
		public enum ClassStyles {
			CS_VREDRAW = 0x0001,
			CS_HREDRAW = 0x0002,
			CS_DBLCLKS = 0x0008,
			CS_OWNDC = 0x0020,
			CS_CLASSDC = 0x0040,
			CS_PARENTDC = 0x0080,
			CS_NOCLOSE = 0x0200,
			CS_SAVEBITS = 0x0800,
			CS_BYTEALIGNCLIENT = 0x1000,
			CS_BYTEALIGNWINDOW = 0x2000,
			CS_GLOBALCLASS = 0x4000,
		}

		#endregion

		#region FrameControlState enum

		public enum FrameControlState {
			DFCS_CAPTIONCLOSE = 0x0000,
			DFCS_CAPTIONMIN = 0x0001,
			DFCS_CAPTIONMAX = 0x0002,
			DFCS_CAPTIONRESTORE = 0x0003,
			DFCS_CAPTIONHELP = 0x0004,
			DFCS_MENUARROW = 0x0000,
			DFCS_MENUCHECK = 0x0001,
			DFCS_MENUBULLET = 0x0002,
			DFCS_MENUARROWRIGHT = 0x0004,
			DFCS_SCROLLUP = 0x0000,
			DFCS_SCROLLDOWN = 0x0001,
			DFCS_SCROLLLEFT = 0x0002,
			DFCS_SCROLLRIGHT = 0x0003,
			DFCS_SCROLLCOMBOBOX = 0x0005,
			DFCS_SCROLLSIZEGRIP = 0x0008,
			DFCS_SCROLLSIZEGRIPRIGHT = 0x0010,
			DFCS_BUTTONCHECK = 0x0000,
			DFCS_BUTTONRADIOIMAGE = 0x0001,
			DFCS_BUTTONRADIOMASK = 0x0002,
			DFCS_BUTTONRADIO = 0x0004,
			DFCS_BUTTON3STATE = 0x0008,
			DFCS_BUTTONPUSH = 0x0010,
			DFCS_INACTIVE = 0x0100,
			DFCS_PUSHED = 0x0200,
			DFCS_CHECKED = 0x0400,
			DFCS_TRANSPARENT = 0x0800,
			DFCS_HOT = 0x1000,
			DFCS_ADJUSTRECT = 0x2000,
			DFCS_FLAT = 0x4000,
			DFCS_MONO = 0x8000,
		}

		#endregion

		#region FrameControlType enum

		public enum FrameControlType {
			DFC_CAPTION = 1,
			DFC_MENU = 2,
			DFC_SCROLL = 3,
			DFC_BUTTON = 4,
			DFC_POPUPMENU = 5,
		}

		#endregion

		#region markerbehaviorflags2 enum

		[Flags]
		public enum markerbehaviorflags2 {
			MB_DEFAULT = 0x00000000, // default stream behavior
			MB_LINESPAN = 0x00000001, // a marker that always adjusts itself to span a line at a time
			MB_LEFTEDGE_LEFTTRACK = 0x00000002,
			MB_RIGHTEDGE_RIGHTTRACK = 0x00000004,
			MB_MULTILINESPAN = 0x00000008,
			MB_DONT_DELETE_IF_ZEROLEN = 0x00000010,
			MB_INHERIT_FOREGROUND = 0x00000020, // Marker leaves foreground color unchanged, inheriting from whatever is "behind" it
			MB_INHERIT_BACKGROUND = 0x00000040, // Marker leaves background color unchanged, inheriting from whatever is "behind" it
			MB_VIEW_SPECIFIC = 0x00000080, // Marker only shows up in certain views
			/*
			MB_TRACK_ON_RELOAD forces a marker to track every edit as a replace, ignoring any reload semantics.
			In other words, IVsTextLines::ReloadLines() will have the same effect as IVsTextLines::ReplaceLines()
			for markers with this style set.  Do not use this unless you have markers that need to guarantee that
			they're tracking in response to OnChangeLineText() events.  (You should not specify this style unless
			you're doing something special and have contacted the VS text editor team about it.)
			*/
			//MB_TRACK_EDIT_ON_RELOAD = 0x00000100
		}

		#endregion

		#region SPIF enum

		[Flags]
		public enum SPIF {
			None = 0x00,
			SPIF_UPDATEINIFILE = 0x01, // Writes the new system-wide parameter setting to the user profile.
			SPIF_SENDCHANGE = 0x02, // Broadcasts the WM_SETTINGCHANGE message after updating the user profile.
			SPIF_SENDWININICHANGE = 0x02 // Same as SPIF_SENDCHANGE.
		}

		#endregion

		#region StockCursors enum

		public enum StockCursors {
			IDC_HAND = 0x7f89,
			IDC_ARROW = 32512,
			IDC_IBEAM = 32513,
			IDC_WAIT = 32514,
			IDC_CROSS = 32515,
			IDC_UPARROW = 32516,
			IDC_SIZE = 32640,
			IDC_ICON = 32641,
			IDC_SIZENWSE = 32642,
			IDC_SIZENESW = 32643,
			IDC_SIZEWE = 32644,
			IDC_SIZENS = 32645,
			IDC_SIZEALL = 32646,
			IDC_NO = 32648,
			IDC_APPSTARTING = 32650,
			IDC_HELP = 32651,
		}

		#endregion

		#region StockObjects enum

		public enum StockObjects {
			WHITE_BRUSH = 0,
			LTGRAY_BRUSH = 1,
			GRAY_BRUSH = 2,
			DKGRAY_BRUSH = 3,
			BLACK_BRUSH = 4,
			NULL_BRUSH = 5,
			HOLLOW_BRUSH = NULL_BRUSH,
			WHITE_PEN = 6,
			BLACK_PEN = 7,
			NULL_PEN = 8,
			OEM_FIXED_FONT = 10,
			ANSI_FIXED_FONT = 11,
			ANSI_VAR_FONT = 12,
			SYSTEM_FONT = 13,
			DEVICE_DEFAULT_FONT = 14,
			DEFAULT_PALETTE = 15,
			SYSTEM_FIXED_FONT = 16,
			DEFAULT_GUI_FONT = 17,
			DC_BRUSH = 18,
			DC_PEN = 19,
		}

		#endregion

		#region VirtualKeys enum

		/// <summary>
		/// Enumeration for virtual keys.
		/// </summary>
		public enum VirtualKeys : ushort {
			LeftButton = 0x01,
			RightButton = 0x02,
			Cancel = 0x03,
			MiddleButton = 0x04,
			ExtraButton1 = 0x05,
			ExtraButton2 = 0x06,
			Back = 0x08,
			Tab = 0x09,
			Clear = 0x0C,
			Return = 0x0D,
			Shift = 0x10,
			Control = 0x11,
			Menu = 0x12,
			Pause = 0x13,
			Capital = 0x14,
			Kana = 0x15,
			Hangeul = 0x15,
			Hangul = 0x15,
			Junja = 0x17,
			Final = 0x18,
			Hanja = 0x19,
			Kanji = 0x19,
			Escape = 0x1B,
			Convert = 0x1C,
			NonConvert = 0x1D,
			Accept = 0x1E,
			ModeChange = 0x1F,
			Space = 0x20,
			Prior = 0x21,
			Next = 0x22,
			End = 0x23,
			Home = 0x24,
			Left = 0x25,
			Up = 0x26,
			Right = 0x27,
			Down = 0x28,
			Select = 0x29,
			Print = 0x2A,
			Execute = 0x2B,
			Snapshot = 0x2C,
			Insert = 0x2D,
			Delete = 0x2E,
			Help = 0x2F,
			N0 = 0x30,
			N1 = 0x31,
			N2 = 0x32,
			N3 = 0x33,
			N4 = 0x34,
			N5 = 0x35,
			N6 = 0x36,
			N7 = 0x37,
			N8 = 0x38,
			N9 = 0x39,
			A = 0x41,
			B = 0x42,
			C = 0x43,
			D = 0x44,
			E = 0x45,
			F = 0x46,
			G = 0x47,
			H = 0x48,
			I = 0x49,
			J = 0x4A,
			K = 0x4B,
			L = 0x4C,
			M = 0x4D,
			N = 0x4E,
			O = 0x4F,
			P = 0x50,
			Q = 0x51,
			R = 0x52,
			S = 0x53,
			T = 0x54,
			U = 0x55,
			V = 0x56,
			W = 0x57,
			X = 0x58,
			Y = 0x59,
			Z = 0x5A,
			LeftWindows = 0x5B,
			RightWindows = 0x5C,
			Application = 0x5D,
			Sleep = 0x5F,
			Numpad0 = 0x60,
			Numpad1 = 0x61,
			Numpad2 = 0x62,
			Numpad3 = 0x63,
			Numpad4 = 0x64,
			Numpad5 = 0x65,
			Numpad6 = 0x66,
			Numpad7 = 0x67,
			Numpad8 = 0x68,
			Numpad9 = 0x69,
			Multiply = 0x6A,
			Add = 0x6B,
			Separator = 0x6C,
			Subtract = 0x6D,
			Decimal = 0x6E,
			Divide = 0x6F,
			F1 = 0x70,
			F2 = 0x71,
			F3 = 0x72,
			F4 = 0x73,
			F5 = 0x74,
			F6 = 0x75,
			F7 = 0x76,
			F8 = 0x77,
			F9 = 0x78,
			F10 = 0x79,
			F11 = 0x7A,
			F12 = 0x7B,
			F13 = 0x7C,
			F14 = 0x7D,
			F15 = 0x7E,
			F16 = 0x7F,
			F17 = 0x80,
			F18 = 0x81,
			F19 = 0x82,
			F20 = 0x83,
			F21 = 0x84,
			F22 = 0x85,
			F23 = 0x86,
			F24 = 0x87,
			NumLock = 0x90,
			ScrollLock = 0x91,
			NEC_Equal = 0x92,
			Fujitsu_Jisho = 0x92,
			Fujitsu_Masshou = 0x93,
			Fujitsu_Touroku = 0x94,
			Fujitsu_Loya = 0x95,
			Fujitsu_Roya = 0x96,
			LeftShift = 0xA0,
			RightShift = 0xA1,
			LeftControl = 0xA2,
			RightControl = 0xA3,
			LeftMenu = 0xA4,
			RightMenu = 0xA5,
			BrowserBack = 0xA6,
			BrowserForward = 0xA7,
			BrowserRefresh = 0xA8,
			BrowserStop = 0xA9,
			BrowserSearch = 0xAA,
			BrowserFavorites = 0xAB,
			BrowserHome = 0xAC,
			VolumeMute = 0xAD,
			VolumeDown = 0xAE,
			VolumeUp = 0xAF,
			MediaNextTrack = 0xB0,
			MediaPrevTrack = 0xB1,
			MediaStop = 0xB2,
			MediaPlayPause = 0xB3,
			LaunchMail = 0xB4,
			LaunchMediaSelect = 0xB5,
			LaunchApplication1 = 0xB6,
			LaunchApplication2 = 0xB7,
			OEM1 = 0xBA,
			OEMPlus = 0xBB,
			OEMComma = 0xBC,
			OEMMinus = 0xBD,
			OEMPeriod = 0xBE,
			OEM2 = 0xBF,
			OEM3 = 0xC0,
			OEM4 = 0xDB,
			OEM5 = 0xDC,
			OEM6 = 0xDD,
			OEM7 = 0xDE,
			OEM8 = 0xDF,
			OEMAX = 0xE1,
			OEM102 = 0xE2,
			ICOHelp = 0xE3,
			ICO00 = 0xE4,
			ProcessKey = 0xE5,
			ICOClear = 0xE6,
			Packet = 0xE7,
			OEMReset = 0xE9,
			OEMJump = 0xEA,
			OEMPA1 = 0xEB,
			OEMPA2 = 0xEC,
			OEMPA3 = 0xED,
			OEMWSCtrl = 0xEE,
			OEMCUSel = 0xEF,
			OEMATTN = 0xF0,
			OEMFinish = 0xF1,
			OEMCopy = 0xF2,
			OEMAuto = 0xF3,
			OEMENLW = 0xF4,
			OEMBackTab = 0xF5,
			ATTN = 0xF6,
			CRSel = 0xF7,
			EXSel = 0xF8,
			EREOF = 0xF9,
			Play = 0xFA,
			Zoom = 0xFB,
			Noname = 0xFC,
			PA1 = 0xFD,
			OEMClear = 0xFE
		}

		#endregion

		#region VK enum

		public enum VK : ushort {
			SHIFT = 0x10,
			CONTROL = 0x11,
			MENU = 0x12,
			ESCAPE = 0x1B
		}

		#endregion

		#region WindowStyles enum

		[Flags]
		public enum WindowStyles : uint {
			WS_OVERLAPPED = 0x00000000,
			WS_POPUP = 0x80000000,
			WS_CHILD = 0x40000000,
			WS_MINIMIZE = 0x20000000,
			WS_VISIBLE = 0x10000000,
			WS_DISABLED = 0x08000000,
			WS_CLIPSIBLINGS = 0x04000000,
			WS_CLIPCHILDREN = 0x02000000,
			WS_MAXIMIZE = 0x01000000,
			WS_BORDER = 0x00800000,
			WS_DLGFRAME = 0x00400000,
			WS_VSCROLL = 0x00200000,
			WS_HSCROLL = 0x00100000,
			WS_SYSMENU = 0x00080000,
			WS_THICKFRAME = 0x00040000,
			WS_GROUP = 0x00020000,
			WS_TABSTOP = 0x00010000,

			WS_MINIMIZEBOX = 0x00020000,
			WS_MAXIMIZEBOX = 0x00010000,

			WS_CAPTION = WS_BORDER | WS_DLGFRAME,
			WS_TILED = WS_OVERLAPPED,
			WS_ICONIC = WS_MINIMIZE,
			WS_SIZEBOX = WS_THICKFRAME,
			WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,

			WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
			WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
			WS_CHILDWINDOW = WS_CHILD,
		}

		#endregion

		#region WindowStylesEx enum

		[Flags]
		public enum WindowStylesEx : long {
			WS_EX_DLGMODALFRAME = 0x00000001L,
			WS_EX_NOPARENTNOTIFY = 0x00000004L,
			WS_EX_TOPMOST = 0x00000008L,
			WS_EX_ACCEPTFILES = 0x00000010L,
			WS_EX_TRANSPARENT = 0x00000020L,
			WS_EX_MDICHILD = 0x00000040L,
			WS_EX_TOOLWINDOW = 0x00000080L,
			WS_EX_WINDOWEDGE = 0x00000100L,
			WS_EX_CLIENTEDGE = 0x00000200L,
			WS_EX_CONTEXTHELP = 0x00000400L,
			WS_EX_RIGHT = 0x00001000L,
			WS_EX_LEFT = 0x00000000L,
			WS_EX_RTLREADING = 0x00002000L,
			WS_EX_LTRREADING = 0x00000000L,
			WS_EX_LEFTSCROLLBAR = 0x00004000L,
			WS_EX_RIGHTSCROLLBAR = 0x00000000L,
			WS_EX_CONTROLPARENT = 0x00010000L,
			WS_EX_STATICEDGE = 0x00020000L,
			WS_EX_APPWINDOW = 0x00040000L,
			WS_EX_LAYERED = 0x00080000L,
			WS_EX_NOINHERITLAYOUT = 0x00100000L, // Disable inheritence of mirroring by children
			WS_EX_LAYOUTRTL = 0x00400000L, // Right to left mirroring
			WS_EX_COMPOSITED = 0x02000000L,
			WS_EX_NOACTIVATE = 0x08000000L,
		}

		#endregion

		#region WmSizeFlags enum

		public enum WmSizeFlags {
			SIZE_RESTORED,
			SIZE_MINIMIZED,
			SIZE_MAXIMIZED,
			SIZE_MAXSHOW,
			SIZE_MAXHIDE
		}

		#endregion

		public const uint DFC_BUTTON = 4;

		public const uint DFC_CAPTION = 1;
		public const uint DFC_MENU = 2;
		public const uint DFC_POPUPMENU = 5;
		public const uint DFC_SCROLL = 3;
		public const uint DFCS_ADJUSTRECT = 0x2000;
		public const uint DFCS_BUTTON3STATE = 0x0008;
		public const uint DFCS_BUTTONCHECK = 0x0000;
		public const uint DFCS_BUTTONPUSH = 0x0010;
		public const uint DFCS_BUTTONRADIO = 0x0004;
		public const uint DFCS_BUTTONRADIOIMAGE = 0x0001;
		public const uint DFCS_BUTTONRADIOMASK = 0x0002;

		public const uint DFCS_CAPTIONCLOSE = 0x0000;
		public const uint DFCS_CAPTIONHELP = 0x0004;
		public const uint DFCS_CAPTIONMAX = 0x0002;
		public const uint DFCS_CAPTIONMIN = 0x0001;
		public const uint DFCS_CAPTIONRESTORE = 0x0003;
		public const uint DFCS_CHECKED = 0x0400;
		public const uint DFCS_FLAT = 0x4000;
		public const uint DFCS_HOT = 0x1000;
		public const uint DFCS_INACTIVE = 0x0100;
		public const uint DFCS_MENUARROW = 0x0000;
		public const uint DFCS_MENUARROWRIGHT = 0x0004;
		public const uint DFCS_MENUBULLET = 0x0002;
		public const uint DFCS_MENUCHECK = 0x0001;
		public const uint DFCS_MONO = 0x8000;
		public const uint DFCS_PUSHED = 0x0200;
		public const uint DFCS_SCROLLCOMBOBOX = 0x0005;
		public const uint DFCS_SCROLLDOWN = 0x0001;
		public const uint DFCS_SCROLLLEFT = 0x0002;
		public const uint DFCS_SCROLLRIGHT = 0x0003;
		public const uint DFCS_SCROLLSIZEGRIP = 0x0008;
		public const uint DFCS_SCROLLSIZEGRIPRIGHT = 0x0010;
		public const uint DFCS_SCROLLUP = 0x0000;
		public const uint DFCS_TRANSPARENT = 0x0800;
		public const int GWL_EXSTYLE = -20;
		public const int GWL_WNDPROC = -4;

		public const int HOVER_DEFAULT = -1;

		// SetWindowPos()
		public const int HWND_BOTTOM = 1;
		public const int HWND_MESSAGE = -3;
		public const int HWND_NOTOPMOST = -2;
		public const int HWND_TOP = 0;
		public const int HWND_TOPMOST = -1;
		public const int INPUT_KEYBOARD = 1;
		public const uint KEYEVENTF_KEYUP = 0x0002;

		public const int LWA_COLORKEY = 0x00000001;
		public const uint OBJID_CLIENT = 0xFFFFFFFC;

		public const uint OBJID_HSCROLL = 0xFFFFFFFA;
		public const uint OBJID_VSCROLL = 0xFFFFFFFB;
		public const int PW_CLIENTONLY = 1;

		public const int SB_HORZ = 0;
		public const int SB_VERT = 1;
		public const uint TME_CANCEL = 0x8000000;
		public const uint TME_HOVER = 0x00000001;
		public const uint TME_LEAVE = 0x00000002;
		public const uint TME_NONCLIENT = 0x00000010;
		public const uint TME_QUERY = 0x40000000;
		public const uint WA_ACTIVE = 1;
		public const uint WA_CLICKACTIVE = 2;
		public const uint WA_INACTIVE = 0;
		public const int WHEEL_DELTA = 120;
		public const int WS_EX_LAYERED = 0x00080000;

		#region Nested type: PAINTSTRUCT

		[StructLayout(LayoutKind.Sequential)]
		public struct PAINTSTRUCT {
			public IntPtr hdc;
			public bool fErase;
			public RECT rcPaint;
			public bool fRestore;
			public bool fIncUpdate;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)] public byte[] rgbReserved;
		}

		#endregion

		#region Nested type: TRACKMOUSEEVENT

		[StructLayout(LayoutKind.Sequential)]
		public struct TRACKMOUSEEVENT {
			public int cbSize;
			public uint dwFlags;
			public IntPtr hwndTrack;
			public int dwHoverTime;
		}

		#endregion

		#region Nested type: WINDOWPOS

		[StructLayout(LayoutKind.Sequential)]
		public struct WINDOWPOS {
			public IntPtr hwnd;
			public IntPtr hwndInsertAfter;
			public int x;
			public int y;
			public int cx;
			public int cy;
			public int flags;
		}

		#endregion

		#endregion

		#region Delegates

		/// <summary>
		/// The CallWndProc hook procedure is an application-defined or library-defined callback 
		/// function used with the SetWindowsHookEx function. The HOOKPROC type defines a pointer 
		/// to this callback function. CallWndProc is a placeholder for the application-defined 
		/// or library-defined function name.
		/// </summary>
		/// <param name="nCode">
		/// [in] Specifies whether the hook procedure must process the message. 
		/// If nCode is HC_ACTION, the hook procedure must process the message. 
		/// If nCode is less than zero, the hook procedure must pass the message to the 
		/// CallNextHookEx function without further processing and must return the 
		/// value returned by CallNextHookEx.
		/// </param>
		/// <param name="wParam">
		/// [in] Specifies whether the message was sent by the current thread. 
		/// If the message was sent by the current thread, it is nonzero; otherwise, it is zero. 
		/// </param>
		/// <param name="lParam">
		/// [in] Pointer to a CWPSTRUCT structure that contains details about the message. 
		/// </param>
		/// <returns>
		/// If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx. 
		/// If nCode is greater than or equal to zero, it is highly recommended that you call CallNextHookEx 
		/// and return the value it returns; otherwise, other applications that have installed WH_CALLWNDPROC 
		/// hooks will not receive hook notifications and may behave incorrectly as a result. If the hook 
		/// procedure does not call CallNextHookEx, the return value should be zero. 
		/// </returns>
		/// <remarks>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/callwndproc.asp
		/// </remarks>
		public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);

		public delegate int WindowProc(IntPtr hwnd, int uMsg, IntPtr wParam, IntPtr lParam);

		#endregion

		#region enWMACTIVATE enum

		public enum enWMACTIVATE {
			WA_INACTIVE = 0
			,
			WA_ACTIVE
			,
			WA_CLICKACTIVE
		}

		#endregion

		#region HitTest enum

		public enum HitTest {
			HTERROR = -2,
			HTTRANSPARENT = -1,
			HTNOWHERE = 0,
			HTCLIENT = 1,
			HTCAPTION = 2,
			HTSYSMENU = 3,
			HTGROWBOX = 4,
			HTSIZE = HTGROWBOX,
			HTMENU = 5,
			HTHSCROLL = 6,
			HTVSCROLL = 7,
			HTMINBUTTON = 8,
			HTMAXBUTTON = 9,
			HTLEFT = 10,
			HTRIGHT = 11,
			HTTOP = 12,
			HTTOPLEFT = 13,
			HTTOPRIGHT = 14,
			HTBOTTOM = 15,
			HTBOTTOMLEFT = 16,
			HTBOTTOMRIGHT = 17,
			HTBORDER = 18,
			HTREDUCE = HTMINBUTTON,
			HTZOOM = HTMAXBUTTON,
			HTSIZEFIRST = HTLEFT,
			HTSIZELAST = HTBOTTOMRIGHT,
			HTOBJECT = 19,
			HTCLOSE = 20,
			HTHELP = 21,
		}

		#endregion

		#region HookType enum

		public enum HookType {
			WH_JOURNALRECORD,
			WH_JOURNALPLAYBACK,
			WH_KEYBOARD,
			WH_GETMESSAGE,
			WH_CALLWNDPROC,
			WH_CBT,
			WH_SYSMSGFILTER,
			WH_MOUSE,
			WH_HARDWARE,
			WH_DEBUG,
			WH_SHELL,
			WH_FOREGROUNDIDLE,
			WH_CALLWNDPROCRET,
			WH_KEYBOARD_LL,
			WH_MOUSE_LL
		}

		#endregion

		#region ScrollBarDirection enum

		public enum ScrollBarDirection {
			SB_HORZ = 0,
			SB_VERT = 1,
			SB_CTL = 2,
			SB_BOTH = 3
		}

		#endregion

		#region ScrollbarDirections enum

		public enum ScrollbarDirections {
			SBS_HORZ = 0,
			SBS_VERT = 1
		}

		#endregion

		#region ScrollInfoMask enum

		public enum ScrollInfoMask {
			SIF_RANGE = 0x1,
			SIF_PAGE = 0x2,
			SIF_POS = 0x4,
			SIF_DISABLENOSCROLL = 0x8,
			SIF_TRACKPOS = 0x10,
			SIF_ALL = SIF_RANGE + SIF_PAGE + SIF_POS + SIF_TRACKPOS
		}

		#endregion

		#region SystemMetric enum

		/// <summary>
		/// Flags used with the Windows API (User32.dll):GetSystemMetrics(SystemMetric smIndex)
		/// </summary>
		public enum SystemMetric {
			/// <summary>
			///  Width of the screen of the primary display monitor, in pixels. This is the same values obtained by calling GetDeviceCaps as follows: GetDeviceCaps( hdcPrimaryMonitor, HORZRES).
			/// </summary>
			SM_CXSCREEN = 0,
			/// <summary>
			/// Height of the screen of the primary display monitor, in pixels. This is the same values obtained by calling GetDeviceCaps as follows: GetDeviceCaps( hdcPrimaryMonitor, VERTRES).
			/// </summary>
			SM_CYSCREEN = 1,
			/// <summary>
			/// Width of a horizontal scroll bar, in pixels.
			/// </summary>
			SM_CYVSCROLL = 2,
			/// <summary>
			/// Height of a horizontal scroll bar, in pixels.
			/// </summary>
			SM_CXVSCROLL = 3,
			/// <summary>
			/// Height of a caption area, in pixels.
			/// </summary>
			SM_CYCAPTION = 4,
			/// <summary>
			/// Width of a window border, in pixels. This is equivalent to the SM_CXEDGE value for windows with the 3-D look. 
			/// </summary>
			SM_CXBORDER = 5,
			/// <summary>
			/// Height of a window border, in pixels. This is equivalent to the SM_CYEDGE value for windows with the 3-D look. 
			/// </summary>
			SM_CYBORDER = 6,
			/// <summary>
			/// Thickness of the frame around the perimeter of a window that has a caption but is not sizable, in pixels. SM_CXFIXEDFRAME is the height of the horizontal border and SM_CYFIXEDFRAME is the width of the vertical border. 
			/// </summary>
			SM_CXDLGFRAME = 7,
			/// <summary>
			/// Thickness of the frame around the perimeter of a window that has a caption but is not sizable, in pixels. SM_CXFIXEDFRAME is the height of the horizontal border and SM_CYFIXEDFRAME is the width of the vertical border. 
			/// </summary>
			SM_CYDLGFRAME = 8,
			/// <summary>
			/// Height of the thumb box in a vertical scroll bar, in pixels
			/// </summary>
			SM_CYVTHUMB = 9,
			/// <summary>
			/// Width of the thumb box in a horizontal scroll bar, in pixels.
			/// </summary>
			SM_CXHTHUMB = 10,
			/// <summary>
			/// Default width of an icon, in pixels. The LoadIcon function can load only icons with the dimensions specified by SM_CXICON and SM_CYICON
			/// </summary>
			SM_CXICON = 11,
			/// <summary>
			/// Default height of an icon, in pixels. The LoadIcon function can load only icons with the dimensions SM_CXICON and SM_CYICON.
			/// </summary>
			SM_CYICON = 12,
			/// <summary>
			/// Width of a cursor, in pixels. The system cannot create cursors of other sizes.
			/// </summary>
			SM_CXCURSOR = 13,
			/// <summary>
			/// Height of a cursor, in pixels. The system cannot create cursors of other sizes.
			/// </summary>
			SM_CYCURSOR = 14,
			/// <summary>
			/// Height of a single-line menu bar, in pixels.
			/// </summary>
			SM_CYMENU = 15,
			/// <summary>
			/// Width of the client area for a full-screen window on the primary display monitor, in pixels. To get the coordinates of the portion of the screen not obscured by the system taskbar or by application desktop toolbars, call the SystemParametersInfo function with the SPI_GETWORKAREA value.
			/// </summary>
			SM_CXFULLSCREEN = 16,
			/// <summary>
			/// Height of the client area for a full-screen window on the primary display monitor, in pixels. To get the coordinates of the portion of the screen not obscured by the system taskbar or by application desktop toolbars, call the SystemParametersInfo function with the SPI_GETWORKAREA value.
			/// </summary>
			SM_CYFULLSCREEN = 17,
			/// <summary>
			/// For double byte character set versions of the system, this is the height of the Kanji window at the bottom of the screen, in pixels
			/// </summary>
			SM_CYKANJIWINDOW = 18,
			/// <summary>
			/// Nonzero if a mouse with a wheel is installed; zero otherwise
			/// </summary>
			SM_MOUSEWHEELPRESENT = 75,
			/// <summary>
			/// Height of the arrow bitmap on a vertical scroll bar, in pixels.
			/// </summary>
			SM_CYHSCROLL = 20,
			/// <summary>
			/// Width of the arrow bitmap on a horizontal scroll bar, in pixels.
			/// </summary>
			SM_CXHSCROLL = 21,
			/// <summary>
			/// Nonzero if the debug version of User.exe is installed; zero otherwise.
			/// </summary>
			SM_DEBUG = 22,
			/// <summary>
			/// Nonzero if the left and right mouse buttons are reversed; zero otherwise.
			/// </summary>
			SM_SWAPBUTTON = 23,
			/// <summary>
			/// Reserved for future use
			/// </summary>
			SM_RESERVED1 = 24,
			/// <summary>
			/// Reserved for future use
			/// </summary>
			SM_RESERVED2 = 25,
			/// <summary>
			/// Reserved for future use
			/// </summary>
			SM_RESERVED3 = 26,
			/// <summary>
			/// Reserved for future use
			/// </summary>
			SM_RESERVED4 = 27,
			/// <summary>
			/// Minimum width of a window, in pixels.
			/// </summary>
			SM_CXMIN = 28,
			/// <summary>
			/// Minimum height of a window, in pixels.
			/// </summary>
			SM_CYMIN = 29,
			/// <summary>
			/// Width of a button in a window's caption or title bar, in pixels.
			/// </summary>
			SM_CXSIZE = 30,
			/// <summary>
			/// Height of a button in a window's caption or title bar, in pixels.
			/// </summary>
			SM_CYSIZE = 31,
			/// <summary>
			/// Thickness of the sizing border around the perimeter of a window that can be resized, in pixels. SM_CXSIZEFRAME is the width of the horizontal border, and SM_CYSIZEFRAME is the height of the vertical border. 
			/// </summary>
			SM_CXFRAME = 32,
			/// <summary>
			/// Thickness of the sizing border around the perimeter of a window that can be resized, in pixels. SM_CXSIZEFRAME is the width of the horizontal border, and SM_CYSIZEFRAME is the height of the vertical border. 
			/// </summary>
			SM_CYFRAME = 33,
			/// <summary>
			/// Minimum tracking width of a window, in pixels. The user cannot drag the window frame to a size smaller than these dimensions. A window can override this value by processing the WM_GETMINMAXINFO message.
			/// </summary>
			SM_CXMINTRACK = 34,
			/// <summary>
			/// Minimum tracking height of a window, in pixels. The user cannot drag the window frame to a size smaller than these dimensions. A window can override this value by processing the WM_GETMINMAXINFO message
			/// </summary>
			SM_CYMINTRACK = 35,
			/// <summary>
			/// Width of the rectangle around the location of a first click in a double-click sequence, in pixels. The second click must occur within the rectangle defined by SM_CXDOUBLECLK and SM_CYDOUBLECLK for the system to consider the two clicks a double-click
			/// </summary>
			SM_CXDOUBLECLK = 36,
			/// <summary>
			/// Height of the rectangle around the location of a first click in a double-click sequence, in pixels. The second click must occur within the rectangle defined by SM_CXDOUBLECLK and SM_CYDOUBLECLK for the system to consider the two clicks a double-click. (The two clicks must also occur within a specified time.) 
			/// </summary>
			SM_CYDOUBLECLK = 37,
			/// <summary>
			/// Width of a grid cell for items in large icon view, in pixels. Each item fits into a rectangle of size SM_CXICONSPACING by SM_CYICONSPACING when arranged. This value is always greater than or equal to SM_CXICON
			/// </summary>
			SM_CXICONSPACING = 38,
			/// <summary>
			/// Height of a grid cell for items in large icon view, in pixels. Each item fits into a rectangle of size SM_CXICONSPACING by SM_CYICONSPACING when arranged. This value is always greater than or equal to SM_CYICON.
			/// </summary>
			SM_CYICONSPACING = 39,
			/// <summary>
			/// Nonzero if drop-down menus are right-aligned with the corresponding menu-bar item; zero if the menus are left-aligned.
			/// </summary>
			SM_MENUDROPALIGNMENT = 40,
			/// <summary>
			/// Nonzero if the Microsoft Windows for Pen computing extensions are installed; zero otherwise.
			/// </summary>
			SM_PENWINDOWS = 41,
			/// <summary>
			/// Nonzero if User32.dll supports DBCS; zero otherwise. (WinMe/95/98): Unicode
			/// </summary>
			SM_DBCSENABLED = 42,
			/// <summary>
			/// Number of buttons on mouse, or zero if no mouse is installed.
			/// </summary>
			SM_CMOUSEBUTTONS = 43,
			/// <summary>
			/// Identical Values Changed After Windows NT 4.0  
			/// </summary>
			SM_CXFIXEDFRAME = SM_CXDLGFRAME,
			/// <summary>
			/// Identical Values Changed After Windows NT 4.0
			/// </summary>
			SM_CYFIXEDFRAME = SM_CYDLGFRAME,
			/// <summary>
			/// Identical Values Changed After Windows NT 4.0
			/// </summary>
			SM_CXSIZEFRAME = SM_CXFRAME,
			/// <summary>
			/// Identical Values Changed After Windows NT 4.0
			/// </summary>
			SM_CYSIZEFRAME = SM_CYFRAME,
			/// <summary>
			/// Nonzero if security is present; zero otherwise.
			/// </summary>
			SM_SECURE = 44,
			/// <summary>
			/// Width of a 3-D border, in pixels. This is the 3-D counterpart of SM_CXBORDER
			/// </summary>
			SM_CXEDGE = 45,
			/// <summary>
			/// Height of a 3-D border, in pixels. This is the 3-D counterpart of SM_CYBORDER
			/// </summary>
			SM_CYEDGE = 46,
			/// <summary>
			/// Width of a grid cell for a minimized window, in pixels. Each minimized window fits into a rectangle this size when arranged. This value is always greater than or equal to SM_CXMINIMIZED.
			/// </summary>
			SM_CXMINSPACING = 47,
			/// <summary>
			/// Height of a grid cell for a minimized window, in pixels. Each minimized window fits into a rectangle this size when arranged. This value is always greater than or equal to SM_CYMINIMIZED.
			/// </summary>
			SM_CYMINSPACING = 48,
			/// <summary>
			/// Recommended width of a small icon, in pixels. Small icons typically appear in window captions and in small icon view
			/// </summary>
			SM_CXSMICON = 49,
			/// <summary>
			/// Recommended height of a small icon, in pixels. Small icons typically appear in window captions and in small icon view.
			/// </summary>
			SM_CYSMICON = 50,
			/// <summary>
			/// Height of a small caption, in pixels
			/// </summary>
			SM_CYSMCAPTION = 51,
			/// <summary>
			/// Width of small caption buttons, in pixels.
			/// </summary>
			SM_CXSMSIZE = 52,
			/// <summary>
			/// Height of small caption buttons, in pixels.
			/// </summary>
			SM_CYSMSIZE = 53,
			/// <summary>
			/// Width of menu bar buttons, such as the child window close button used in the multiple document interface, in pixels.
			/// </summary>
			SM_CXMENUSIZE = 54,
			/// <summary>
			/// Height of menu bar buttons, such as the child window close button used in the multiple document interface, in pixels.
			/// </summary>
			SM_CYMENUSIZE = 55,
			/// <summary>
			/// Flags specifying how the system arranged minimized windows
			/// </summary>
			SM_ARRANGE = 56,
			/// <summary>
			/// Width of a minimized window, in pixels.
			/// </summary>
			SM_CXMINIMIZED = 57,
			/// <summary>
			/// Height of a minimized window, in pixels.
			/// </summary>
			SM_CYMINIMIZED = 58,
			/// <summary>
			/// Default maximum width of a window that has a caption and sizing borders, in pixels. This metric refers to the entire desktop. The user cannot drag the window frame to a size larger than these dimensions. A window can override this value by processing the WM_GETMINMAXINFO message.
			/// </summary>
			SM_CXMAXTRACK = 59,
			/// <summary>
			/// Default maximum height of a window that has a caption and sizing borders, in pixels. This metric refers to the entire desktop. The user cannot drag the window frame to a size larger than these dimensions. A window can override this value by processing the WM_GETMINMAXINFO message.
			/// </summary>
			SM_CYMAXTRACK = 60,
			/// <summary>
			/// Default width, in pixels, of a maximized top-level window on the primary display monitor.
			/// </summary>
			SM_CXMAXIMIZED = 61,
			/// <summary>
			/// Default height, in pixels, of a maximized top-level window on the primary display monitor.
			/// </summary>
			SM_CYMAXIMIZED = 62,
			/// <summary>
			/// Least significant bit is set if a network is present; otherwise, it is cleared. The other bits are reserved for future use
			/// </summary>
			SM_NETWORK = 63,
			/// <summary>
			/// Value that specifies how the system was started: 0-normal, 1-failsafe, 2-failsafe /w net
			/// </summary>
			SM_CLEANBOOT = 67,
			/// <summary>
			/// Width of a rectangle centered on a drag point to allow for limited movement of the mouse pointer before a drag operation begins, in pixels. 
			/// </summary>
			SM_CXDRAG = 68,
			/// <summary>
			/// Height of a rectangle centered on a drag point to allow for limited movement of the mouse pointer before a drag operation begins. This value is in pixels. It allows the user to click and release the mouse button easily without unintentionally starting a drag operation.
			/// </summary>
			SM_CYDRAG = 69,
			/// <summary>
			/// Nonzero if the user requires an application to present information visually in situations where it would otherwise present the information only in audible form; zero otherwise. 
			/// </summary>
			SM_SHOWSOUNDS = 70,
			/// <summary>
			/// Width of the default menu check-mark bitmap, in pixels.
			/// </summary>
			SM_CXMENUCHECK = 71,
			/// <summary>
			/// Height of the default menu check-mark bitmap, in pixels.
			/// </summary>
			SM_CYMENUCHECK = 72,
			/// <summary>
			/// Nonzero if the computer has a low-end (slow) processor; zero otherwise
			/// </summary>
			SM_SLOWMACHINE = 73,
			/// <summary>
			/// Nonzero if the system is enabled for Hebrew and Arabic languages, zero if not.
			/// </summary>
			SM_MIDEASTENABLED = 74,
			/// <summary>
			/// Nonzero if a mouse is installed; zero otherwise. This value is rarely zero, because of support for virtual mice and because some systems detect the presence of the port instead of the presence of a mouse.
			/// </summary>
			SM_MOUSEPRESENT = 19,
			/// <summary>
			/// Windows 2000 (v5.0+) Coordinate of the top of the virtual screen
			/// </summary>
			SM_XVIRTUALSCREEN = 76,
			/// <summary>
			/// Windows 2000 (v5.0+) Coordinate of the left of the virtual screen
			/// </summary>
			SM_YVIRTUALSCREEN = 77,
			/// <summary>
			/// Windows 2000 (v5.0+) Width of the virtual screen
			/// </summary>
			SM_CXVIRTUALSCREEN = 78,
			/// <summary>
			/// Windows 2000 (v5.0+) Height of the virtual screen
			/// </summary>
			SM_CYVIRTUALSCREEN = 79,
			/// <summary>
			/// Number of display monitors on the desktop
			/// </summary>
			SM_CMONITORS = 80,
			/// <summary>
			/// Windows XP (v5.1+) Nonzero if all the display monitors have the same color format, zero otherwise. Two displays can have the same bit depth, but different color formats. For example, the red, green, and blue pixels can be encoded with different numbers of bits, or those bits can be located in different places in a pixel's color value. 
			/// </summary>
			SM_SAMEDISPLAYFORMAT = 81,
			/// <summary>
			/// Windows XP (v5.1+) Nonzero if Input Method Manager/Input Method Editor features are enabled; zero otherwise
			/// </summary>
			SM_IMMENABLED = 82,
			/// <summary>
			/// Windows XP (v5.1+) Width of the left and right edges of the focus rectangle drawn by DrawFocusRect. This value is in pixels. 
			/// </summary>
			SM_CXFOCUSBORDER = 83,
			/// <summary>
			/// Windows XP (v5.1+) Height of the top and bottom edges of the focus rectangle drawn by DrawFocusRect. This value is in pixels. 
			/// </summary>
			SM_CYFOCUSBORDER = 84,
			/// <summary>
			/// Nonzero if the current operating system is the Windows XP Tablet PC edition, zero if not.
			/// </summary>
			SM_TABLETPC = 86,
			/// <summary>
			/// Nonzero if the current operating system is the Windows XP, Media Center Edition, zero if not.
			/// </summary>
			SM_MEDIACENTER = 87,
			/// <summary>
			/// Metrics Other
			/// </summary>
			SM_CMETRICS_OTHER = 76,
			/// <summary>
			/// Metrics Windows 2000
			/// </summary>
			SM_CMETRICS_2000 = 83,
			/// <summary>
			/// Metrics Windows NT
			/// </summary>
			SM_CMETRICS_NT = 88,
			/// <summary>
			/// Windows XP (v5.1+) This system metric is used in a Terminal Services environment. If the calling process is associated with a Terminal Services client session, the return value is nonzero. If the calling process is associated with the Terminal Server console session, the return value is zero. The console session is not necessarily the physical console - see WTSGetActiveConsoleSessionId for more information. 
			/// </summary>
			SM_REMOTESESSION = 0x1000,
			/// <summary>
			/// Windows XP (v5.1+) Nonzero if the current session is shutting down; zero otherwise
			/// </summary>
			SM_SHUTTINGDOWN = 0x2000,
			/// <summary>
			/// Windows XP (v5.1+) This system metric is used in a Terminal Services environment. Its value is nonzero if the current session is remotely controlled; zero otherwise
			/// </summary>
			SM_REMOTECONTROL = 0x2001,
		}

		#endregion

		public const int DLGC_BUTTON = 0x2000; /* Button item: can be checked      */
		public const int DLGC_DEFPUSHBUTTON = 0x0010; /* Default pushbutton               */
		public const int DLGC_HASSETSEL = 0x0008; /* Understands EM_SETSEL message    */
		public const int DLGC_RADIOBUTTON = 0x0040; /* Radio button                     */
		public const int DLGC_STATIC = 0x0100; /* Static item: don't include       */
		public const int DLGC_UNDEFPUSHBUTTON = 0x0020; /* Non-default pushbutton           */
		public const int DLGC_WANTALLKEYS = 0x0004; /* Control wants all keys           */
		public const int DLGC_WANTARROWS = 0x0001; /* Control wants arrow keys         */
		public const int DLGC_WANTCHARS = 0x0080; /* Want WM_CHAR messages            */
		public const int DLGC_WANTTAB = 0x0002; /* Control wants tab keys           */
		public static int GWL_STYLE = -16;

		public static UInt32 SWP_DRAWFRAME = SWP_FRAMECHANGED;
		public static UInt32 SWP_FRAMECHANGED = 0x0020; /* The frame changed: send WM_NCCALCSIZE */
		public static UInt32 SWP_HIDEWINDOW = 0x0080;
		public static UInt32 SWP_NOACTIVATE = 0x0010;
		public static UInt32 SWP_NOCOPYBITS = 0x0100;
		public static UInt32 SWP_NOMOVE = 0x0002;
		public static UInt32 SWP_NOOWNERZORDER = 0x0200; /* Don't do owner Z ordering */
		public static UInt32 SWP_NOREDRAW = 0x0008;
		public static UInt32 SWP_NOSENDCHANGING = 0x0400; /* Don't send WM_WINDOWPOSCHANGING */
		public static UInt32 SWP_NOSIZE = 0x0001;
		public static UInt32 SWP_NOZORDER = 0x0004;
		public static UInt32 SWP_SHOWWINDOW = 0x0040;

		public static int WM_ERASEBKGND = 20;
		public static int WM_GETDLGCODE = 0x87;

		#region Misc

		//		private static void SendEscape() {
		//			NativeWIN32.INPUT structInput;
		//			structInput = new NativeWIN32.INPUT();
		//			structInput.type = NativeWIN32.INPUT_KEYBOARD;
		//			structInput.ki.wScan = 0;
		//			structInput.ki.time = 0;
		//			structInput.ki.dwExtraInfo = 0;
		//			structInput.ki.wVk = (ushort)NativeWIN32.VK.ESCAPE;
		//
		//			// Key down
		//			structInput.ki.dwFlags = 0;
		//			NativeWIN32.SendInput(1, ref structInput, Marshal.SizeOf(structInput));
		//			// Key up
		//			structInput.ki.dwFlags = NativeWIN32.KEYEVENTF_KEYUP;
		//			NativeWIN32.SendInput(1, ref structInput, Marshal.SizeOf(structInput));
		//		}

		#endregion

		#region Public methods

		public static string GetClassName(IntPtr hWnd) {
			StringBuilder sb = new StringBuilder(100);
			int intLength = GetClassName(hWnd, sb, sb.Capacity);
			if (intLength > 0) {
				return sb.ToString();
			} else {
				return null;
			}
		}

		public static string GetActiveWindowText(IntPtr hWnd) {
			// Allocate correct string length first
			int length = GetWindowTextLength(hWnd);
			StringBuilder sb = new StringBuilder(length + 1);
			GetWindowText(hWnd, sb, sb.Capacity);
			return sb.ToString();
		}

		public static int GET_WHEEL_DELTA_WPARAM(IntPtr wParam) {
			return (short)HIWORD(wParam);
		}

		public static int GET_WHEEL_DELTA_WPARAM(uint wParam) {
			return (short)HIWORD(wParam);
		}

		public static int GET_KEYSTATE_WPARAM(IntPtr wParam) {
			return (short)LOWORD(wParam);
		}

		public static int GET_Y_LPARAM(IntPtr lParam) {
			return (short)HIWORD(lParam);
		}

		public static int GET_X_LPARAM(IntPtr lParam) {
			return (short)LOWORD(lParam);
		}

		public static int MakeLong(int LoWord, int HiWord) {
			return (HiWord << 16) | (LoWord & 0xffff);
		}

		public static IntPtr MakeLParam(int LoWord, int HiWord) {
			return (IntPtr)((HiWord << 16) | (LoWord & 0xffff));
		}

		public static int HiWord(int Number) {
			return (Number >> 16) & 0xffff;
		}

		public static int LoWord(int Number) {
			return Number & 0xffff;
		}

		public static long MAKELPARAM(uint wLow, uint wHigh) {
			return LOWORD(wLow) | (0x10000 * LOWORD(wHigh));
		}

		public static ushort LOWORD(IntPtr l) {
			return (ushort)(((long)l) & 0xffff);
		}

		public static ushort LOWORD(uint l) {
			return (ushort)(l & 0xffff);
		}

		public static ushort HIWORD(IntPtr l) {
			return (ushort)((((long)l) >> 0x10) & 0xffff);
		}

		public static ushort HIWORD(uint l) {
			return (ushort)(l >> 0x10);
		}

		public static bool IsScrollBar(IntPtr hwnd) {
			string className = GetClassName(hwnd);
			return className.Equals("ScrollBar");
		}

		public static bool IsScrollBarVertical(IntPtr hwnd) {
			int styles = GetWindowLong(hwnd, GWL_STYLE);
			return (styles & (int)(ScrollbarDirections.SBS_VERT)) == (int)(ScrollbarDirections.SBS_VERT);
		}

		public static bool IsScrollBarVisible(Control ctrl, bool blnVertical) {
			if (!ctrl.IsHandleCreated) {
				return false;
			}
			return (GetWindowLong(ctrl.Handle, GWL_STYLE) & (int)(blnVertical ? WindowStyles.WS_VSCROLL : WindowStyles.WS_HSCROLL)) != 0;
		}

		public static int GetVerticalScrollBarWidth() {
			return GetSystemMetrics(SystemMetric.SM_CXVSCROLL);
		}

		public static int GetScreenHeight() {
			return GetSystemMetrics(SystemMetric.SM_CYVIRTUALSCREEN);
		}

		public static int ColorToRGB(Color crColor) {
			return crColor.B << 16 | crColor.G << 8 | crColor.R;
		}

		public static bool IsValidWindowHandle(IntPtr hwnd) {
			if (IntPtr.Zero == hwnd || !IsWindow(hwnd)) {
				return false;
			}
			return true;
		}

		#endregion

		#region Dll imports

		#region Delegates

		public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

		#endregion

		[DllImport("user32.dll")]
		public static extern bool ShowCaret(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern bool HideCaret(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern IntPtr GetFocus();

		[DllImport("gdi32.dll")]
		public static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

		[DllImport("user32.dll")]
		public static extern bool IsRectEmpty([In] ref RECT lprc);

		[DllImport("user32.dll")]
		public static extern bool GetUpdateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

		[DllImport("user32.dll")]
		public static extern bool GetUpdateRect(IntPtr hWnd, out RECT lpRect, bool bErase);

		[DllImport("user32.dll")]
		public static extern bool GetUpdateRect(HandleRef hWnd, out RECT rect, bool bErase);

		[DllImport("User32.dll", SetLastError = true)]
		public static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowDC(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool KillTimer(IntPtr hWnd, uint uIDEvent);

		[DllImport("user32.dll")]
		public static extern bool DestroyWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern bool TrackMouseEvent(ref TRACKMOUSEEVENT lpEventTrack);

		[DllImport("user32.dll")]
		public static extern IntPtr LoadCursor(IntPtr hInstance, StockCursors lpCursorName);

		[DllImport("user32.dll")]
		public static extern IntPtr SetCursor(IntPtr hCursor);

		[DllImport("user32.dll")]
		public static extern bool DrawFrameControl(IntPtr hdc, [In] ref RECT lprc, uint uType, uint uState);

		[DllImport("user32.dll")]
		public static extern int FillRect(IntPtr hDC, [In] ref RECT lprc, IntPtr hbr);

		[DllImport("gdi32.dll")]
		public static extern IntPtr GetStockObject(int fnObject);

		[DllImport("gdi32.dll")]
		public static extern uint SetDCBrushColor(IntPtr hdc, uint crColor);

		[DllImport("user32.dll")]
		public static extern IntPtr BeginPaint(IntPtr hwnd, out PAINTSTRUCT lpPaint);

		[DllImport("user32.dll")]
		public static extern bool EndPaint(IntPtr hWnd, [In] ref PAINTSTRUCT lpPaint);

		[DllImport("user32.dll")]
		public static extern bool ValidateRect(IntPtr hWnd, ref RECT lpRect);

		[DllImport("user32.dll")]
		public static extern bool ValidateRect(IntPtr hWnd, IntPtr lpRect);

		[DllImport("user32.dll")]
		public static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

		[DllImport("user32.dll")]
		public static extern bool InvalidateRect(IntPtr hWnd, RECT lpRect, bool bErase);

		[DllImport("user32.dll")]
		public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

		[DllImport("user32.dll")]
		public static extern IntPtr GetDlgItem(IntPtr hDlg, int nIDDlgItem);

		[DllImport("user32.dll")]
		public static extern bool IsWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

		[DllImport("winmm.dll", EntryPoint = "timeGetTime")]
		public static extern uint timeGetTime();

		[DllImport("Kernel32.dll")]
		public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

		[DllImport("Kernel32.dll")]
		public static extern bool QueryPerformanceFrequency(out long lpFrequency);

		[DllImport("user32.dll")]
		public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, Int32 crKey, int bAlpha, uint dwFlags);

		[DllImport("user32.dll")]
		public static extern short GetKeyState(short nVirtKey);

		[DllImport("user32.dll")]
		public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

		[DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
		public static extern IntPtr GetParent(IntPtr hWnd);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

		[DllImport("user32.dll")]
		public static extern bool PostMessage(IntPtr hWnd, uint msg, int wP, int lP);

		[DllImport("user32.dll")]
		public static extern IntPtr SetFocus(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern IntPtr WindowFromPoint(POINT Point);

		[DllImport("user32.dll", SetLastError = true, EntryPoint = "GetScrollBarInfo")]
		public static extern int GetScrollBarInfo(IntPtr hWnd, uint idObject, ref SCROLLBARINFO psbi);

		[DllImport("USER32.DLL")]
		public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		[DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
		public static extern IntPtr SetWindowLong(IntPtr hWnd, int index, [MarshalAs(UnmanagedType.FunctionPtr)] WindowProc newWndProc);

		[DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
		public static extern IntPtr SetWindowLong(IntPtr hWnd, int index, IntPtr newWndProc);

		[DllImport("user32.dll", EntryPoint = "CallWindowProc", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
		public static extern int CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetScrollInfo(IntPtr hwnd, int fnBar, ref SCROLLINFO lpsi);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SystemParametersInfo(SPI uiAction, uint uiParam, IntPtr pvParam, SPIF fWinIni);

		//		public static Point ScreenToClient(IntPtr hWnd, Point point) {
		//			POINTAPI pt = point;
		//			ScreenToClient(hWnd, ref pt);
		//			return pt;
		//		}
		//
		//		[DllImport("user32.dll")]
		//		public static extern bool ScreenToClient(IntPtr hWnd, ref POINTAPI lpPoint);

		[DllImport("user32.dll")]
		public static extern bool ClientToScreen(IntPtr hWnd, ref Point pt);

		[DllImport("user32.dll")]
		public static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

		[DllImport("user32.dll")]
		public static extern bool ScreenToClient(IntPtr hWnd, ref Point lpPoint);

		[DllImport("user32.dll")]
		public static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr LoadLibrary(string lpFileName);

		[DllImport("kernel32.dll")]
		public static extern uint GetCurrentThreadId();

		[DllImport("user32.dll", SetLastError = false)]
		public static extern IntPtr GetMessageExtraInfo();

		[DllImport("user32.dll")]
		public static extern int GetSystemMetrics(SystemMetric smIndex);

		[DllImport("USER32.DLL")]
		public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

		[DllImport("user32.dll", EntryPoint = "FindWindowEx", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
		public static extern IntPtr FindWindowEx(int hWndParent, IntPtr hWndChildAfter, string lpszClass, string lpszWindow);

		[DllImport("user32.dll", EntryPoint = "FindWindowEx", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
		public static extern IntPtr FindWindowEx(IntPtr hWndParent, IntPtr hWndChildAfter, string lpszClass, string lpszWindow);

		[DllImport("user32.dll")]
		public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

		[DllImport("user32.dll")]
		public static extern IntPtr GetActiveWindow();

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int GetWindowText(IntPtr hWnd, out STRINGBUFFER ClassName, int nMaxCount);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
		public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool PostMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern int GetWindowTextLength(IntPtr hWnd);

		/// <summary>
		/// The SetWindowsHookEx function installs an application-defined hook procedure into a hook chain. 
		/// You would install a hook procedure to monitor the system for certain types of events. These events 
		/// are associated either with a specific thread or with all threads in the same desktop as the calling thread. 
		/// </summary>
		/// <param name="idHook">
		/// [in] Specifies the type of hook procedure to be installed. This parameter can be one of the following values.
		/// </param>
		/// <param name="lpfn">
		/// [in] Pointer to the hook procedure. If the dwThreadId parameter is zero or specifies the identifier of a 
		/// thread created by a different process, the lpfn parameter must point to a hook procedure in a dynamic-link 
		/// library (DLL). Otherwise, lpfn can point to a hook procedure in the code associated with the current process.
		/// </param>
		/// <param name="hMod">
		/// [in] Handle to the DLL containing the hook procedure pointed to by the lpfn parameter. 
		/// The hMod parameter must be set to NULL if the dwThreadId parameter specifies a thread created by 
		/// the current process and if the hook procedure is within the code associated with the current process. 
		/// </param>
		/// <param name="dwThreadId">
		/// [in] Specifies the identifier of the thread with which the hook procedure is to be associated. 
		/// If this parameter is zero, the hook procedure is associated with all existing threads running in the 
		/// same desktop as the calling thread. 
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is the handle to the hook procedure.
		/// If the function fails, the return value is NULL. To get extended error information, call GetLastError.
		/// </returns>
		/// <remarks>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/setwindowshookex.asp
		/// </remarks>
		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		public static extern IntPtr SetWindowsHookEx(HookType idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

		//		public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, int dwThreadId);

		/// <summary>
		/// The UnhookWindowsHookEx function removes a hook procedure installed in a hook chain by the SetWindowsHookEx function. 
		/// </summary>
		/// <param name="idHook">
		/// [in] Handle to the hook to be removed. This parameter is a hook handle obtained by a previous call to SetWindowsHookEx. 
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero.
		/// If the function fails, the return value is zero. To get extended error information, call GetLastError.
		/// </returns>
		/// <remarks>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/setwindowshookex.asp
		/// </remarks>
		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		public static extern int UnhookWindowsHookEx(IntPtr idHook);

		//		public static extern int UnhookWindowsHookEx(int idHook);

		/// <summary>
		/// The CallNextHookEx function passes the hook information to the next hook procedure in the current hook chain. 
		/// A hook procedure can call this function either before or after processing the hook information. 
		/// </summary>
		/// <param name="idHook">Ignored.</param>
		/// <param name="nCode">
		/// [in] Specifies the hook code passed to the current hook procedure. 
		/// The next hook procedure uses this code to determine how to process the hook information.
		/// </param>
		/// <param name="wParam">
		/// [in] Specifies the wParam value passed to the current hook procedure. 
		/// The meaning of this parameter depends on the type of hook associated with the current hook chain. 
		/// </param>
		/// <param name="lParam">
		/// [in] Specifies the lParam value passed to the current hook procedure. 
		/// The meaning of this parameter depends on the type of hook associated with the current hook chain. 
		/// </param>
		/// <returns>
		/// This value is returned by the next hook procedure in the chain. 
		/// The current hook procedure must also return this value. The meaning of the return value depends on the hook type. 
		/// For more information, see the descriptions of the individual hook procedures.
		/// </returns>
		/// <remarks>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/setwindowshookex.asp
		/// </remarks>
		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern int CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);

		//		public static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

		/// <summary>
		/// The ToAscii function translates the specified virtual-key code and keyboard 
		/// state to the corresponding character or characters. The function translates the code 
		/// using the input language and physical keyboard layout identified by the keyboard layout handle.
		/// </summary>
		/// <param name="uVirtKey">
		/// [in] Specifies the virtual-key code to be translated. 
		/// </param>
		/// <param name="uScanCode">
		/// [in] Specifies the hardware scan code of the key to be translated. 
		/// The high-order bit of this value is set if the key is up (not pressed). 
		/// </param>
		/// <param name="lpbKeyState">
		/// [in] Pointer to a 256-byte array that contains the current keyboard state. 
		/// Each element (byte) in the array contains the state of one key. 
		/// If the high-order bit of a byte is set, the key is down (pressed). 
		/// The low bit, if set, indicates that the key is toggled on. In this function, 
		/// only the toggle bit of the CAPS LOCK key is relevant. The toggle state 
		/// of the NUM LOCK and SCROLL LOCK keys is ignored.
		/// </param>
		/// <param name="lpwTransKey">
		/// [out] Pointer to the buffer that receives the translated character or characters. 
		/// </param>
		/// <param name="fuState">
		/// [in] Specifies whether a menu is active. This parameter must be 1 if a menu is active, or 0 otherwise. 
		/// </param>
		/// <returns>
		/// If the specified key is a dead key, the return value is negative. Otherwise, it is one of the following values. 
		/// Value Meaning 
		/// 0 The specified virtual key has no translation for the current state of the keyboard. 
		/// 1 One character was copied to the buffer. 
		/// 2 Two characters were copied to the buffer. This usually happens when a dead-key character 
		/// (accent or diacritic) stored in the keyboard layout cannot be composed with the specified 
		/// virtual key to form a single character. 
		/// </returns>
		/// <remarks>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/userinput/keyboardinput/keyboardinputreference/keyboardinputfunctions/toascii.asp
		/// </remarks>
		[DllImport("user32")]
		public static extern int ToAscii(int uVirtKey, int uScanCode, byte[] lpbKeyState, byte[] lpwTransKey, int fuState);

		/// <summary>
		/// The GetKeyboardState function copies the status of the 256 virtual keys to the 
		/// specified buffer. 
		/// </summary>
		/// <param name="pbKeyState">
		/// [in] Pointer to a 256-byte array that contains keyboard key states. 
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero.
		/// If the function fails, the return value is zero. To get extended error information, call GetLastError. 
		/// </returns>
		/// <remarks>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/userinput/keyboardinput/keyboardinputreference/keyboardinputfunctions/toascii.asp
		/// </remarks>
		[DllImport("user32")]
		public static extern int GetKeyboardState(byte[] pbKeyState);

		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern short GetKeyState(int vKey);

		#endregion

		#region Nested type: CREATESTRUCT

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct CREATESTRUCT {
			public IntPtr lpCreateParams;
			public IntPtr hInstance;
			public IntPtr hMenu;
			public IntPtr hwndParent;
			public int cy;
			public int cx;
			public int y;
			public int x;
			public int style;
			public string lpszName;
			public string lpszClass;
			public int dwExStyle;
		}

		#endregion

		#region Nested type: HookEventArgs

		[StructLayout(LayoutKind.Sequential)]
		public struct HookEventArgs {
			public int HookCode;
			public IntPtr wParam;
			public IntPtr lParam;

			public KeyboardHookStruct GetKeyboardHookStruct() {
				return (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof (KeyboardHookStruct));
			}

			public MouseLLHookStruct GetMouseHookStruct() {
				return (MouseLLHookStruct)Marshal.PtrToStructure(lParam, typeof (MouseLLHookStruct));
			}
		}

		#endregion

		#region Nested type: KeyboardHookStruct

		/// <summary>
		/// The KBDLLHOOKSTRUCT structure contains information about a low-level keyboard input event. 
		/// </summary>
		/// <remarks>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookstructures/cwpstruct.asp
		/// </remarks>
		[StructLayout(LayoutKind.Sequential)]
		public class KeyboardHookStruct {
			/// <summary>
			/// Specifies a virtual-key code. The code must be a value in the range 1 to 254. 
			/// </summary>
			public int vkCode;

			/// <summary>
			/// Specifies a hardware scan code for the key. 
			/// </summary>
			public int scanCode;

			/// <summary>
			/// Specifies the extended-key flag, event-injected flag, context code, and transition-state flag.
			/// </summary>
			public int flags;

			/// <summary>
			/// Specifies the time stamp for this message.
			/// </summary>
			public int time;

			/// <summary>
			/// Specifies extra information associated with the message. 
			/// </summary>
			public int dwExtraInfo;
		}

		#endregion

		#region Nested type: MouseLLHookStruct

		/// <summary>
		/// The MSLLHOOKSTRUCT structure contains information about a low-level keyboard input event. 
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public class MouseLLHookStruct {
			/// <summary>
			/// Specifies a POINT structure that contains the x- and y-coordinates of the cursor, in screen coordinates. 
			/// </summary>
			public POINT pt;

			/// <summary>
			/// If the message is WM_MOUSEWHEEL, the high-order word of this member is the wheel delta. 
			/// The low-order word is reserved. A positive value indicates that the wheel was rotated forward, 
			/// away from the user; a negative value indicates that the wheel was rotated backward, toward the user. 
			/// One wheel click is defined as WHEEL_DELTA, which is 120. 
			/// If the message is WM_XBUTTONDOWN, WM_XBUTTONUP, WM_XBUTTONDBLCLK, WM_NCXBUTTONDOWN, WM_NCXBUTTONUP,
			/// or WM_NCXBUTTONDBLCLK, the high-order word specifies which X button was pressed or released, 
			/// and the low-order word is reserved. This value can be one or more of the following values. Otherwise, mouseData is not used. 
			/// XBUTTON1
			/// The first X button was pressed or released.
			/// XBUTTON2
			/// The second X button was pressed or released.
			/// </summary>
			public int mouseData;

			/// <summary>
			/// Specifies the event-injected flag. An application can use the following value to test the mouse flags. Value Purpose 
			/// LLMHF_INJECTED Test the event-injected flag.  
			/// 0
			/// Specifies whether the event was injected. The value is 1 if the event was injected; otherwise, it is 0.
			/// 1-15
			/// Reserved.
			/// </summary>
			public int flags;

			/// <summary>
			/// Specifies the time stamp for this message.
			/// </summary>
			public int time;

			/// <summary>
			/// Specifies extra information associated with the message. 
			/// </summary>
			public int dwExtraInfo;
		}

		#endregion

		#region Nested type: POINT

		/// <summary>
		/// The POINT structure defines the x- and y- coordinates of a point. 
		/// </summary>
		/// <remarks>
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/gdi/rectangl_0tiq.asp
		/// </remarks>
		[StructLayout(LayoutKind.Sequential)]
		public struct POINT {
			public int x;
			public int y;

			public POINT(int x, int y) {
				this.x = x;
				this.y = y;
			}

			public static implicit operator Point(POINT p) {
				return new Point(p.y, p.y);
			}

			public static implicit operator POINT(Point p) {
				return new POINT(p.X, p.Y);
			}
		}

		#endregion

		#region Nested type: RECT

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT {
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}

		#endregion

		#region Nested type: SCROLLBARINFO

		[StructLayout(LayoutKind.Sequential)]
		public struct SCROLLBARINFO {
			public int cbSize;
			public RECT rcScrollBar;
			public int dxyLineButton;
			public int xyThumbTop;
			public int xyThumbBottom;
			public int reserved;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)] public int[] rgstate;
		}

		#endregion

		#region Nested type: SCROLLINFO

		[StructLayout(LayoutKind.Sequential)]
		public struct SCROLLINFO {
			public uint cbSize;
			public uint fMask;
			public int nMin;
			public int nMax;
			public uint nPage;
			public int nPos;
			public int nTrackPos;
		}

		#endregion

		#region Nested type: STRINGBUFFER

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct STRINGBUFFER {
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)] public string szText;
		}

		#endregion
	}
}