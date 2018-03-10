Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language

''' <summary>
''' 只适合显示短序列上面的Feature
''' </summary>
Public Module ASCIIViewer

    '                           ProtK 
    'Ch_lo_Pn1.3_Pn2_ProtK_Therm| 
    '           Glu_ProtK_Staph|| 
    '            AspGluN_ProtK||| 
    '  ArgC_Clost_Therm_Tryps|||| 
    '        Glu_ProtK_Staph||||| 
    '               AspGluN|||||| 

    '                     ||||||| 
    '                     SERVELAT
    '                 1   --------   8

    <Extension>
    Public Sub DisplayOn(sites As IEnumerable(Of Site), seq$, Optional dev As TextWriter = Nothing, Optional deli$ = ", ")
        ' 先按照位点的位置进行分组
        Dim groups = sites _
            .GroupBy(Function(site) site.Left) _
            .OrderByDescending(Function(g) g.Key) _
            .ToArray
        Dim labels = groups _
            .Select(Function(g)
                        Return g.Select(Function(site) site.Name) _
                                .OrderBy(Function(s) s) _
                                .Distinct _
                                .JoinBy(deli)
                    End Function) _
            .ToArray
        Dim lens%() = labels.Select(AddressOf Len).ToArray
        Dim lefts%() = groups.Select(Function(g) g.Key).ToArray
        Dim maxOffset% = lens.Max

        With dev Or App.StdOut
            For i As Integer = 0 To labels.Length - 1
                Dim labeList$ = labels(i)
                Dim left% = lefts(i)
                Dim labelLength = lens(i)
                Dim offset = maxOffset - (1 + labeList.Length)

                Call .Write(New String(" "c, offset))
                Call .Write(labeList)
                Call .Write("|"c)

                For j As Integer = i To 0 Step -1
                    Dim delta = lefts(j) - left
                    .Write(New String(" "c, delta - 1) & "|")
                    left = lefts(j)
                Next
            Next

            Dim l As New List(Of Char)

            With lefts.Indexing
                For j As Integer = 1 To seq.Length
                    If .IndexOf(j) > -1 Then
                        l += "|"c
                    Else
                        l += " "c
                    End If
                Next
            End With

            Call .WriteLine()
            Call .WriteLine(l.CharString)
            Call .WriteLine(seq)
            Call .WriteLine($"1   {New String("-"c, seq.Length)}   {seq.Length}")

            Call .Flush()
        End With
    End Sub
End Module

Public Class Site
    Public Property Name As String
    Public Property Left As Integer
End Class