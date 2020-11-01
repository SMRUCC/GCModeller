Imports System
Imports System.Runtime.InteropServices
Imports UnixRStruct = RDotNet.Internals.Unix.RStart

Namespace Internals.Windows
    <StructLayout(LayoutKind.Sequential)>
    Friend Structure RStart
        Friend Common As UnixRStruct
        Public rhome As IntPtr
        Public home As IntPtr
        <MarshalAs(UnmanagedType.FunctionPtr)>
        Public ReadConsole As blah1
        <MarshalAs(UnmanagedType.FunctionPtr)>
        Public WriteConsole As blah2
        <MarshalAs(UnmanagedType.FunctionPtr)>
        Public CallBack As blah3
        <MarshalAs(UnmanagedType.FunctionPtr)>
        Public ShowMessage As blah4
        <MarshalAs(UnmanagedType.FunctionPtr)>
        Public YesNoCancel As blah5
        <MarshalAs(UnmanagedType.FunctionPtr)>
        Public Busy As blah6
        Public CharacterMode As UiMode
        <MarshalAs(UnmanagedType.FunctionPtr)>
        Public WriteConsoleEx As blah7
    End Structure
End Namespace
