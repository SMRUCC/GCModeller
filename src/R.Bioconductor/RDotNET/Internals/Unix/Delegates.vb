Imports System
Imports System.Runtime.InteropServices
Imports System.Text

Namespace Internals.Unix
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub ptr_R_Suicide(
    <[In]>
    <MarshalAs(UnmanagedType.LPStr)> ByVal message As String)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub ptr_R_ShowMessage(
    <[In]>
    <MarshalAs(UnmanagedType.LPStr)> ByVal message As String)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function ptr_R_ReadConsole(
    <[In]>
    <MarshalAs(UnmanagedType.LPStr)> ByVal prompt As String,
    <MarshalAs(UnmanagedType.LPStr)> ByVal buffer As StringBuilder, ByVal length As Integer,
    <MarshalAs(UnmanagedType.Bool)> ByVal history As Boolean) As <MarshalAs(UnmanagedType.Bool)> Boolean
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub ptr_R_WriteConsole(
    <[In]>
    <MarshalAs(UnmanagedType.LPStr)> ByVal buffer As String, ByVal length As Integer)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub ptr_R_WriteConsoleEx(
    <[In]>
    <MarshalAs(UnmanagedType.LPStr)> ByVal buffer As String, ByVal length As Integer, ByVal outputType As ConsoleOutputType)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub ptr_R_ResetConsole()
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub ptr_R_FlushConsole()
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub ptr_R_ClearerrConsole()
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub ptr_R_Busy(ByVal which As BusyType)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub ptr_R_CleanUp(ByVal saveAction As StartupSaveAction, ByVal status As Integer,
    <MarshalAs(UnmanagedType.Bool)> ByVal runLast As Boolean)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function ptr_R_ShowFiles(ByVal count As Integer,
    <[In]>
    <MarshalAs(UnmanagedType.LPArray, ArraySubType:=UnmanagedType.LPStr)> ByVal files As String(),
    <[In]>
    <MarshalAs(UnmanagedType.LPArray, ArraySubType:=UnmanagedType.LPStr)> ByVal headers As String(),
    <[In]>
    <MarshalAs(UnmanagedType.LPStr)> ByVal title As String,
    <MarshalAs(UnmanagedType.Bool)> ByVal delete As Boolean,
    <[In]>
    <MarshalAs(UnmanagedType.LPStr)> ByVal pager As String) As <MarshalAs(UnmanagedType.Bool)> Boolean
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function ptr_R_ChooseFile(
    <MarshalAs(UnmanagedType.Bool)> ByVal create As Boolean, ByVal buffer As StringBuilder, ByVal length As Integer) As <MarshalAs(UnmanagedType.Bool)> Integer
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub ptr_R_EditFile(
    <MarshalAs(UnmanagedType.LPStr)> ByVal file As String)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function ptr_R_loadhistory(ByVal [call] As IntPtr, ByVal operation As IntPtr, ByVal args As IntPtr, ByVal environment As IntPtr) As IntPtr
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function ptr_R_savehistory(ByVal [call] As IntPtr, ByVal operation As IntPtr, ByVal args As IntPtr, ByVal environment As IntPtr) As IntPtr
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function ptr_R_addhistory(ByVal [call] As IntPtr, ByVal operation As IntPtr, ByVal args As IntPtr, ByVal environment As IntPtr) As IntPtr
End Namespace
