Imports System.Runtime.InteropServices

Namespace Graphics

    Friend NotInheritable Class Utility
        Private Sub New()
        End Sub
        Public Shared Function ReadDouble(pointer As IntPtr, offset As Integer) As Double
            Dim value = New Double(0) {}
            Marshal.Copy(IntPtr.Add(pointer, offset), value, 0, value.Length)
            Return value(0)
        End Function
    End Class
End Namespace