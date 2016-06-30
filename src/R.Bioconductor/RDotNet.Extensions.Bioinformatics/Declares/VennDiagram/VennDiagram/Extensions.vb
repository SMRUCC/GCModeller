Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic

Namespace VennDiagram.ModelAPI

    Public Module Extensions

        ''' <summary>
        ''' 尝试着从一个字符串集合中猜测出可能的名称
        ''' </summary>
        ''' <param name="source">基因号列表</param>
        ''' <returns>猜测出基因号的物种前缀，例如XC_1184 -> XC_</returns>
        ''' <remarks></remarks>
        <Extension> Public Function ParseName(source As Generic.IEnumerable(Of String), Serial As Integer) As String
            Dim LCollection = (From s As String In source.AsParallel Where Not String.IsNullOrEmpty(s) Select s Distinct).ToArray
            Dim Name As List(Of Char) = New List(Of Char)
            For i As Integer = 0 To (From s As String In LCollection.AsParallel Select Len(s)).Min - 1
                Dim p As Integer = i
                Dim LQuery = (From s As String In LCollection Select s(p) Distinct).ToArray '

                If LQuery.Length = 1 Then
                    Name += LQuery.First
                Else
                    Exit For
                End If
            Next

            If Name.Count > 0 Then
                Return New String(Name.ToArray)
            Else
                Return "Serial_" & Serial
            End If
        End Function
    End Module
End Namespace