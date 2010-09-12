// ---------------------------------
// SmarterSql (c) Johan Sassner 2008
// ---------------------------------
using System;
using System.Runtime.InteropServices;
using Sassner.SmarterSql.PInvoke;

namespace Microsoft.VisualStudio {
	//   We sacrifice performance for security as this is a serious fxcop bug.   
	//	 [System.Security.SuppressUnmanagedCodeSecurityAttribute()]
	internal static class SafeNativeMethods {
		[DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
		internal static extern bool InvalidateRect(IntPtr hWnd, ref NativeMethods.RECT rect, bool erase);

		[DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
		internal static extern bool InvalidateRect(IntPtr hWnd, [MarshalAs(UnmanagedType.Interface)] object rect, bool erase);

		[DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
		internal static extern bool IsChild(IntPtr parent, IntPtr child);

		[DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
		internal static extern bool IsWindowVisible(IntPtr hWnd);

		[DllImport(ExternDll.Kernel32, ExactSpelling = true, CharSet = CharSet.Auto)]
		internal static extern int GetCurrentThreadId();

		[DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
		internal static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, [In, Out] ref NativeMethods.RECT rect, int cPoints);

		[DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
		internal static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, [In, Out] NativeMethods.POINT pt, int cPoints);

		[DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
		internal static extern int RegisterWindowMessage(string msg);

		[DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
		internal static extern bool GetWindowRect(IntPtr hWnd, [In, Out] ref NativeMethods.RECT rect);

		[DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
		internal static extern int DrawText(IntPtr hDC, string lpszString, int nCount, ref NativeMethods.RECT lpRect, int nFormat);

		[DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
		internal static extern bool OffsetRect([In, Out] ref NativeMethods.RECT lpRect, int dx, int dy);

		[DllImport(ExternDll.Gdi32, SetLastError = true, CharSet = CharSet.Auto)]
		internal static extern int GetTextExtentPoint32(IntPtr hDC, string str, int len, [In, Out] NativeMethods.POINT ptSize);

		[DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
		internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr gdiObj);

		[DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
		internal static extern void DeleteObject(IntPtr gdiObj);

		[DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
		internal static extern IntPtr CreateSolidBrush(int crColor);

		[DllImport(ExternDll.Gdi32, SetLastError = true, CharSet = CharSet.Auto)]
		internal static extern IntPtr CreateFontIndirect([In, Out, MarshalAs(UnmanagedType.AsAny)] object lf);

		[DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
		internal static extern int SetTextColor(IntPtr hdc, int crColor);

		[DllImport(ExternDll.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
		internal static extern int SetBkMode(IntPtr hdc, int nBkMode);

		[DllImport(ExternDll.Oleaut32, PreserveSig = false)]
		internal static extern void VariantInit(IntPtr pObject);

		[DllImport(ExternDll.Oleaut32, PreserveSig = false)]
		internal static extern void VariantClear(IntPtr pObject);
	}
}