#Region "Microsoft.VisualBasic::da75c0205d4064cc6ccf0580e6a993b8, ..\GNUplot\GNUplot\Data.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

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

