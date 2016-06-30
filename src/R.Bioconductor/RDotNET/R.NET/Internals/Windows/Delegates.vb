Imports System.Runtime.InteropServices
Imports System.Text

Namespace Internals.Windows
	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function blah1(<[In]> <MarshalAs(UnmanagedType.LPStr)> prompt As String, <MarshalAs(UnmanagedType.LPStr)> buffer As StringBuilder, length As Integer, <MarshalAs(UnmanagedType.Bool)> history As Boolean) As <MarshalAs(UnmanagedType.Bool)> Boolean

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub blah2(<[In]> <MarshalAs(UnmanagedType.LPStr)> buffer As String, length As Integer)

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub blah3()

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub blah4(<[In]> <MarshalAs(UnmanagedType.LPStr)> message As String)

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Function blah5(<[In]> <MarshalAs(UnmanagedType.LPStr)> question As String) As YesNoCancel

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub blah6(which As BusyType)

	<UnmanagedFunctionPointer(CallingConvention.Cdecl)> _
	Friend Delegate Sub blah7(<[In]> <MarshalAs(UnmanagedType.LPStr)> buffer As String, length As Integer, outputType As ConsoleOutputType)
End Namespace
