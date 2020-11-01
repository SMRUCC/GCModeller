Imports System
Imports System.Runtime.InteropServices

Namespace Graphics
    Friend Module Utility
        Public Function ReadDouble(ByVal pointer As IntPtr, ByVal offset As Integer) As Double
            Dim value = New Double(0) {}
            Marshal.Copy(IntPtr.Add(pointer, offset), value, 0, value.Length)
            Return value(0)
        End Function
    End Module
End Namespace
