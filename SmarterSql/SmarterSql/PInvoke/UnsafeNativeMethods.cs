// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio.OLE.Interop;
using Sassner.SmarterSql.PInvoke;

namespace Microsoft.VisualStudio {
	//   We sacrifice performance for security as this is a serious fxcop bug.   
	//	 [System.Security.SuppressUnmanagedCodeSecurityAttribute()]
	internal static class UnsafeNativeMethods {
		// APIS

		[DllImport(ExternDll.Kernel32, CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern int GetFileAttributes(String name);

		[DllImport(ExternDll.Kernel32, CharSet = CharSet.Auto)]
		public static extern void GetTempFileName(string tempDirName, string prefixName, int unique, StringBuilder sb);

		[DllImport(ExternDll.Kernel32, ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool CloseHandle(HandleRef handle);

		[DllImport(ExternDll.User32, SetLastError = true, CharSet = CharSet.Auto)]
		public static extern bool LoadString(HandleRef hInstance, int uID, StringBuilder lpBuffer, int nBufferMax);

		//GetWindowLong won't work correctly for 64-bit: we should use GetWindowLongPtr instead.  On
		//32-bit, GetWindowLongPtr is just #defined as GetWindowLong.  GetWindowLong really should 
		//take/return int instead of IntPtr/HandleRef, but since we're running this only for 32-bit
		//it'll be OK.
		public static IntPtr GetWindowLong(IntPtr hWnd, int nIndex) {
			if (IntPtr.Size == 4) {
				return GetWindowLong32(hWnd, nIndex);
			}
			return GetWindowLongPtr64(hWnd, nIndex);
		}

		[DllImport(ExternDll.User32, CharSet = CharSet.Auto, EntryPoint = "GetWindowLong")]
		public static extern IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);

		[SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist")]
		[DllImport(ExternDll.User32, CharSet = CharSet.Auto, EntryPoint = "GetWindowLongPtr")]
		public static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

		[DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
		internal static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

		[DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
		internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

		[DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
		[SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable")]
		public static extern IntPtr SendMessage(IntPtr hwnd, int msg, bool wparam, int lparam);

		[DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
		[SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable")]
		public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);

		[DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
		internal static extern IntPtr SetParent(IntPtr hWnd, IntPtr hWndParent);

		//SetWindowLong won't work correctly for 64-bit: we should use SetWindowLongPtr instead.  On
		//32-bit, SetWindowLongPtr is just #defined as SetWindowLong.  SetWindowLong really should 
		//take/return int instead of IntPtr/HandleRef, but since we're running this only for 32-bit
		//it'll be OK.
		public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong) {
			if (IntPtr.Size == 4) {
				return SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
			}
			return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
		}

		[SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable")]
		[DllImport(ExternDll.User32, CharSet = CharSet.Auto, EntryPoint = "SetWindowLong")]
		public static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		[SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist")]
		[DllImport(ExternDll.User32, CharSet = CharSet.Auto, EntryPoint = "SetWindowLongPtr")]
		public static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		public static IntPtr SetWindowLong(IntPtr hWnd, short nIndex, IntPtr dwNewLong) {
			if (IntPtr.Size == 4) {
				return SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
			}
			return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
		}

		[SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable")]
		[DllImport(ExternDll.User32, CharSet = CharSet.Auto, EntryPoint = "SetWindowLong")]
		public static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, short nIndex, IntPtr dwNewLong);

		[DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
		internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
			int x, int y, int cx, int cy, int flags);

		[DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
		internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		/// IDataObject stuff
		[DllImport(ExternDll.Kernel32, ExactSpelling = true, CharSet = CharSet.Auto)]
		internal static extern IntPtr GlobalAlloc(int uFlags, int dwBytes);

		[DllImport(ExternDll.Kernel32, ExactSpelling = true, CharSet = CharSet.Auto)]
		internal static extern IntPtr GlobalReAlloc(HandleRef handle, int bytes, int flags);

		[DllImport(ExternDll.Kernel32, ExactSpelling = true, CharSet = CharSet.Auto)]
		internal static extern IntPtr GlobalLock(HandleRef handle);

		[DllImport(ExternDll.Kernel32, ExactSpelling = true, CharSet = CharSet.Auto)]
		internal static extern bool GlobalUnlock(HandleRef handle);

		[DllImport(ExternDll.Kernel32, ExactSpelling = true, CharSet = CharSet.Auto)]
		internal static extern IntPtr GlobalFree(HandleRef handle);

		[DllImport(ExternDll.Kernel32, ExactSpelling = true, CharSet = CharSet.Auto)]
		internal static extern int GlobalSize(HandleRef handle);

		// Beats me why this isn't in the Marshal class.
		[DllImport(ExternDll.Kernel32, EntryPoint = "GlobalLock", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		internal static extern IntPtr GlobalLock(IntPtr h);

		[DllImport(ExternDll.Kernel32, EntryPoint = "GlobalUnlock", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		internal static extern bool GlobalUnLock(IntPtr h);

		[DllImport(ExternDll.Kernel32, EntryPoint = "GlobalSize", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		internal static extern int GlobalSize(IntPtr h);

		[DllImport(ExternDll.Kernel32, ExactSpelling = true, EntryPoint = "RtlMoveMemory", CharSet = CharSet.Unicode)]
		internal static extern void CopyMemoryW(IntPtr pdst, string psrc, int cb);

		[DllImport(ExternDll.Kernel32, ExactSpelling = true, EntryPoint = "RtlMoveMemory", CharSet = CharSet.Unicode)]
		internal static extern void CopyMemoryW(IntPtr pdst, char[] psrc, int cb);

		[DllImport(ExternDll.Kernel32, ExactSpelling = true, EntryPoint = "RtlMoveMemory", CharSet = CharSet.Unicode)]
		internal static extern void CopyMemoryW(StringBuilder pdst, HandleRef psrc, int cb);

		[DllImport(ExternDll.Kernel32, ExactSpelling = true, EntryPoint = "RtlMoveMemory", CharSet = CharSet.Unicode)]
		internal static extern void CopyMemoryW(char[] pdst, HandleRef psrc, int cb);

		[DllImport(ExternDll.Kernel32, ExactSpelling = true, EntryPoint = "RtlMoveMemory")]
		internal static extern void CopyMemory(IntPtr pdst, byte[] psrc, int cb);

		[DllImport(ExternDll.Kernel32, ExactSpelling = true, EntryPoint = "RtlMoveMemory")]
		internal static extern void CopyMemory(byte[] pdst, HandleRef psrc, int cb);

		[DllImport(ExternDll.Kernel32, ExactSpelling = true, EntryPoint = "RtlMoveMemory")]
		internal static extern void CopyMemory(IntPtr pdst, HandleRef psrc, int cb);

		[DllImport(ExternDll.Kernel32, ExactSpelling = true, EntryPoint = "RtlMoveMemory")]
		internal static extern void CopyMemory(IntPtr pdst, string psrc, int cb);

		[DllImport(ExternDll.Kernel32, ExactSpelling = true, CharSet = CharSet.Unicode)]
		internal static extern int WideCharToMultiByte(int codePage, int flags, [MarshalAs(UnmanagedType.LPWStr)] string wideStr, int chars, [In, Out] byte[] pOutBytes, int bufferBytes, IntPtr defaultChar, IntPtr pDefaultUsed);

		[DllImport(ExternDll.Ole32, ExactSpelling = true, CharSet = CharSet.Unicode)]
		internal static extern int OleSetClipboard(IDataObject dataObject);

		[DllImport(ExternDll.Ole32, ExactSpelling = true, CharSet = CharSet.Unicode)]
		internal static extern int OleGetClipboard(out IDataObject dataObject);

		[DllImport(ExternDll.Ole32, ExactSpelling = true, CharSet = CharSet.Unicode)]
		internal static extern int OleFlushClipboard();

		[DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Unicode)]
		internal static extern int OpenClipboard(IntPtr newOwner);

		[DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Unicode)]
		internal static extern int EmptyClipboard();

		[DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Unicode)]
		internal static extern int CloseClipboard();

		[DllImport(ExternDll.Comctl32, CharSet = CharSet.Auto)]
		internal static extern int ImageList_GetImageCount(HandleRef himl);

		[DllImport(ExternDll.Comctl32, CharSet = CharSet.Auto)]
		internal static extern bool ImageList_Draw(HandleRef himl, int i, HandleRef hdcDst, int x, int y, int fStyle);

		[DllImport(ExternDll.Shell32, EntryPoint = "DragQueryFileW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		internal static extern uint DragQueryFile(IntPtr hDrop, uint iFile, char[] lpszFile, uint cch);

		[DllImport(ExternDll.User32, EntryPoint = "RegisterClipboardFormatW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		internal static extern ushort RegisterClipboardFormat(string format);

		///////////// UNUSED

#if false
        [DllImport(ExternDll.Oleaut32, PreserveSig=false)]
        internal static extern UCOMITypeLib LoadRegTypeLib(ref Guid clsid, int majorVersion, int minorVersion, int lcid);

#endif
	}
}