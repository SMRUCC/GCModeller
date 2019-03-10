Imports System.Runtime.InteropServices

Namespace Graphics.Internals
    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub R_GE_checkVersionOrDie(version As Integer)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub R_CheckDeviceAvailable()

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub Rf_onintr()

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Function GEcreateDevDesc(dev As IntPtr) As IntPtr

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub GEaddDevice2(dev As IntPtr, name As String)

    <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
    Friend Delegate Sub GEkillDevice(dev As IntPtr)
End Namespace
