Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline

Public Module TermStreamAssignment

    ReadOnly rank As Func(Of DiamondAnnotation, Double) = Function(hit) (hit.Pident + 1) * (hit.BitScore + 1)

    ''' <summary>
    ''' Processing term assignment via stream processing of the large diamond annotation result output
    ''' </summary>
    ''' <param name="m8"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function MakeTerms(m8 As IEnumerable(Of DiamondAnnotation), Optional topBest As Boolean = True) As IEnumerable(Of RankTerm)
        Dim group As New List(Of DiamondAnnotation)
        Dim query As String = ""

        For Each hit As DiamondAnnotation In m8.SafeQuery
            If hit.QseqId <> query Then
                If group.Count > 0 Then
                    For Each term As RankTerm In MakeTerms(query, group, topBest)
                        Yield term
                    Next

                    Call group.Clear()
                End If

                query = hit.QseqId
            End If

            Call group.Add(hit)
        Next

        If group.Count > 0 Then
            For Each term As RankTerm In MakeTerms(query, group, topBest)
                Yield term
            Next
        End If
    End Function

    Private Iterator Function MakeTerms(query As String, group As IEnumerable(Of DiamondAnnotation), topBest As Boolean) As IEnumerable(Of RankTerm)
        Dim data As New NamedCollection(Of DiamondAnnotation)(query, group)
        Dim terms = RankTerm.MeasureTopTerm(data, rank, Nothing)

        If topBest AndAlso terms.Any Then
            Yield terms.First
        Else
            For Each term As RankTerm In terms
                Yield term
            Next
        End If
    End Function
End Module
