Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.Uniprot

Public Module Shotgun_csvReader

    Public Function StripCsv(path$, Optional headers% = 2) As File
        Dim [in] As File = File.Load(path)
        Dim headerRows As RowObject() = [in].Take(headers).ToArray
        Dim proteins As New List(Of RowObject)
        Dim row As New Value(Of RowObject)
        Dim i As int = headers

        Do While Not (row = [in](++i)).IsNullOrEmpty
            If Not row.value.First.StringEmpty Then
                Dim h = UniprotFasta.SimpleHeaderParser(header:=(+row)(1))
                proteins += (+row)
                Call (+row).Insert(1, h("UniprotID"))
            End If
        Loop

        Dim out As New File
        out += headerRows.__mergeHeaders
        out += proteins.ToArray
        Return out
    End Function

    <Extension>
    Private Function __mergeHeaders(headers As RowObject()) As RowObject
        Dim out As New RowObject From {"ID", "UniprotID"}

        For i As Integer = 1 To headers(Scan0).NumbersOfColumn - 1
            Dim index% = i
            Dim t$() = headers.Select(Function(r) r(index)) _
                .Where(Function(str) Not str.StringEmpty) _
                .ToArray
            Dim s$ = t.JoinBy(" ")

            out += s
        Next

        Return out
    End Function
End Module
