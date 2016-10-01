Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Threading

''' <summary>
''' GNUplot data services
''' </summary>
Public Module Data

    Public Function waitForFile(filename$, Optional timeout% = 10000) As Boolean
        Dim attempts As Integer = timeout \ 100
        Dim file As StreamReader = Nothing

        Call Thread.Sleep(20)

        While file Is Nothing
            Try
                file = New StreamReader(filename)
            Catch
                If Math.Max(Interlocked.Decrement(attempts), attempts + 1) > 0 Then
                    Thread.Sleep(100)
                Else
                    Return False
                End If
            Finally
                If Not file Is Nothing Then
                    Call file.Close()
                End If
            End Try
        End While

        Return True
    End Function

    <Extension>
    Public Sub WriteData(stream As StreamWriter, ySize As Integer, z As Double(), Optional flush As Boolean = True)
        For i As Integer = 0 To z.Length - 1
            If i > 0 AndAlso i Mod ySize = 0 Then
                stream.WriteLine()
            End If
            stream.WriteLine(z(i).ToString())
        Next

        If flush Then
            stream.Flush()
        End If
    End Sub

    <Extension>
    Public Sub WriteData(stream As StreamWriter, zz As Double(,), Optional flush As Boolean = True)
        Dim m As Integer = zz.GetLength(0)
        Dim n As Integer = zz.GetLength(1)
        Dim line As String
        For i As Integer = 0 To m - 1
            line = ""
            For j As Integer = 0 To n - 1
                line += zz(i, j).ToString() & " "
            Next
            stream.WriteLine(line.TrimEnd())
        Next

        If flush Then
            stream.Flush()
        End If
    End Sub

    <Extension>
    Public Sub WriteData(stream As StreamWriter, x As Double(), y As Double(), z As Double(), Optional flush As Boolean = True)
        Dim m As Integer = Math.Min(x.Length, y.Length)
        m = Math.Min(m, z.Length)
        For i As Integer = 0 To m - 1
            If i > 0 AndAlso x(i) <> x(i - 1) Then
                stream.WriteLine("")
            End If
            stream.WriteLine(x(i) & " " & y(i) & " " & z(i))
        Next

        If flush Then
            stream.Flush()
        End If
    End Sub

    <Extension>
    Public Sub WriteData(stream As StreamWriter, y As Double(), Optional flush As Boolean = True)
        For i As Integer = 0 To y.Length - 1
            stream.WriteLine(y(i).ToString())
        Next

        If flush Then
            stream.Flush()
        End If
    End Sub

    <Extension>
    Public Sub WriteData(stream As StreamWriter, x As Double(), y As Double(), Optional flush As Boolean = True)
        For i As Integer = 0 To y.Length - 1
            stream.WriteLine(x(i).ToString() & " " & y(i).ToString())
        Next

        If flush Then
            stream.Flush()
        End If
    End Sub
End Module
