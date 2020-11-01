Imports System.Runtime.InteropServices

Namespace Graphics.Internals
    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Ansi)>
    Friend Structure GEcontext
        Friend col As Color
        Friend fill As Color
        Friend gamma As Double
        Friend lwd As Double
        Friend lty As LineType
        Friend lend As LineEnd
        Friend ljoin As LineJoin
        Friend lmitre As Double
        Friend cex As Double
        Friend ps As Double
        Friend lineheight As Double
        Friend fontface As FontFace
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=201)>
        Friend fontfamily As String
    End Structure
End Namespace
