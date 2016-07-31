Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream

Public Module DocumentExtensions

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="cols"><see cref="File.Columns"/> filtering results.</param>
    ''' <returns></returns>
    <Extension>
    Public Function JoinColumns(cols As IEnumerable(Of String())) As DocumentStream.File
        Dim array As String()() = cols.ToArray
        Dim out As New DocumentStream.File

        For i As Integer = 0 To array.First.Length - 1
            Dim ind As Integer = i
            out += New RowObject(array.Select(Function(x) x(ind)))
        Next

        Return out
    End Function
End Module
