Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline

Public Module TermStreamAssignment

    ''' <summary>
    ''' Processing term assignment via stream processing of the large diamond annotation result output
    ''' </summary>
    ''' <param name="m8"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function MakeTerms(m8 As IEnumerable(Of DiamondAnnotation), Optional topBest As Boolean = True) As IEnumerable(Of RankTerm)
        Dim group As New List(Of DiamondAnnotation)
        Dim query As String = ""
        Dim rank As Func(Of DiamondAnnotation, Double) = Function(hit) (hit.Pident + 1) * (hit.BitScore + 1)

        For Each hit As DiamondAnnotation In m8.SafeQuery
            If hit.QseqId <> query Then
                If group.Count > 0 Then
                    Dim data As New NamedCollection(Of DiamondAnnotation)(query, group)
                    Dim terms = RankTerm.MeasureTopTerm(data, rank, Nothing)

                    Call group.Clear()

                    If topBest AndAlso terms.Any Then
                        Yield terms.First
                    Else
                        For Each term As RankTerm In terms
                            Yield term
                        Next
                    End If
                End If

                query = hit.QseqId
            End If

            Call group.Add(hit)
        Next
    End Function
End Module
