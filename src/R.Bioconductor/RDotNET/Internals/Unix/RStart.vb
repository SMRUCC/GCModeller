Imports System
Imports System.Runtime.InteropServices

Namespace Internals.Unix
    <StructLayout(LayoutKind.Sequential)>
    Friend Structure RStart
        <MarshalAs(UnmanagedType.Bool)>
        Public R_Quiet As Boolean
        <MarshalAs(UnmanagedType.Bool)>
        Public R_Slave As Boolean
        <MarshalAs(UnmanagedType.Bool)>
        Public R_Interactive As Boolean
        <MarshalAs(UnmanagedType.Bool)>
        Public R_Verbose As Boolean
        <MarshalAs(UnmanagedType.Bool)>
        Public LoadSiteFile As Boolean
        <MarshalAs(UnmanagedType.Bool)>
        Public LoadInitFile As Boolean
        <MarshalAs(UnmanagedType.Bool)>
        Public DebugInitFile As Boolean
        Public RestoreAction As StartupRestoreAction
        Public SaveAction As StartupSaveAction
        Friend vsize As UIntPtr
        Friend nsize As UIntPtr
        Friend max_vsize As UIntPtr
        Friend max_nsize As UIntPtr
        Friend ppsize As UIntPtr
        <MarshalAs(UnmanagedType.Bool)>
        Public NoRenviron As Boolean
    End Structure
End Namespace
