Imports System.Runtime.CompilerServices

Public Module mappingDecode

    <Extension>
    Public Iterator Function decodeMappings(map As sourceMap) As IEnumerable(Of mappingLine)
        Dim lines As String() = map.mappings.Split(";"c)

        For Each line As String In lines
            If line.StringEmpty Then
                Yield New mappingLine
            Else
                Yield decodeVLQ(encode:=line)
            End If
        Next
    End Function

    Private Function decodeVLQ(encode As String) As mappingLine

    End Function
End Module
