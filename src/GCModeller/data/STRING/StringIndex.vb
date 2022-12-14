Imports Microsoft.VisualBasic.Math.DataFrame
Imports SMRUCC.genomics.Data.STRING.StringDB.Tsv

''' <summary>
''' network index for the <see cref="linksDetail"/>
''' </summary>
Public Class StringIndex

    ''' <summary>
    ''' protein1 => [protein2 => linkdata]
    ''' </summary>
    ReadOnly matrix As Dictionary(Of String, Dictionary(Of String, linksDetail))

    Sub New(links As IEnumerable(Of linksDetail))
        matrix = links _
            .GroupBy(Function(l) l.protein1) _
            .ToDictionary(Function(prot1) prot1.Key,
                          Function(group)
                              Return group.ToDictionary(Function(l) l.protein2)
                          End Function)
    End Sub

    Public Function QueryLinkDetails(prot1 As String, prot2 As String, Optional ignoreDirection As Boolean = True) As linksDetail
        If matrix.ContainsKey(prot1) Then
            If matrix(prot1).ContainsKey(prot2) Then
                Return matrix(prot1)(prot2)
            Else
                Return Nothing
            End If
        ElseIf ignoreDirection AndAlso matrix.ContainsKey(prot2) Then
            If matrix(prot2).ContainsKey(prot1) Then
                Return matrix(prot2)(prot1)
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    Public Shared Iterator Function RemoveTaxonomyIdPrefix(links As IEnumerable(Of linksDetail)) As IEnumerable(Of linksDetail)
        For Each link As linksDetail In links
            link.protein1 = link.protein1.Split("."c)(1)
            link.protein2 = link.protein2.Split("."c)(1)

            Yield link
        Next
    End Function

    Public Function Intersect(cor As CorrelationMatrix) As CorrelationMatrix

    End Function
End Class
