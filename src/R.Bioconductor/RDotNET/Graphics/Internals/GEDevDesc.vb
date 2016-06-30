Imports System.Runtime.InteropServices

Namespace Graphics.Internals
    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Ansi)> _
    Friend Structure GEDevDesc
        Friend dev As IntPtr

        <MarshalAs(UnmanagedType.Bool)> _
        Friend displayListOn As Boolean

        Friend displayList As IntPtr
        Friend DLlastElt As IntPtr
        Friend savedSnapshot As IntPtr

        <MarshalAs(UnmanagedType.Bool)> _
        Friend dirty As Boolean

        <MarshalAs(UnmanagedType.Bool)> _
        Friend recordGraphics As Boolean

        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=24)>
        Friend gesd As IntPtr()

        <MarshalAs(UnmanagedType.Bool)> _
        Friend ask As Boolean
    End Structure
End Namespace
