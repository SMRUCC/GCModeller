Imports System.Runtime.InteropServices

Namespace Graphics

    Module Utility

        Public Function ReadDouble(pointer As IntPtr, offset As Integer) As Double
            Dim value = New Double(0) {}
            Marshal.Copy(IntPtr.Add(pointer, offset), value, 0, value.Length)
            Return value(0)
        End Function
    End Module
End Namespace