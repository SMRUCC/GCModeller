Imports System
Imports System.Runtime.InteropServices

Namespace Graphics.Internals
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub R_GE_checkVersionOrDie(ByVal version As Integer)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub R_CheckDeviceAvailable()
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub Rf_onintr()
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function GEcreateDevDesc(ByVal dev As IntPtr) As IntPtr
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub GEaddDevice2(ByVal dev As IntPtr, ByVal name As String)
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub GEkillDevice(ByVal dev As IntPtr)
End Namespace
