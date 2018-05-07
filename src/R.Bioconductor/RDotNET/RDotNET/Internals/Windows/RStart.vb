Imports System.Runtime.InteropServices
Imports UnixRStruct = RDotNet.Internals.Unix.RStart

Namespace Internals.Windows
	<StructLayout(LayoutKind.Sequential)> _
	Friend Structure RStart
		Friend Common As UnixRStruct
		Public rhome As IntPtr
		Public home As IntPtr

		<MarshalAs(UnmanagedType.FunctionPtr)> _
		Public ReadConsole As blah1

		<MarshalAs(UnmanagedType.FunctionPtr)> _
		Public WriteConsole As blah2

		<MarshalAs(UnmanagedType.FunctionPtr)> _
		Public CallBack As blah3

		<MarshalAs(UnmanagedType.FunctionPtr)> _
		Public ShowMessage As blah4

		<MarshalAs(UnmanagedType.FunctionPtr)> _
		Public YesNoCancel As blah5

		<MarshalAs(UnmanagedType.FunctionPtr)> _
		Public Busy As blah6

		Public CharacterMode As UiMode

		<MarshalAs(UnmanagedType.FunctionPtr)> _
		Public WriteConsoleEx As blah7
	End Structure
End Namespace
