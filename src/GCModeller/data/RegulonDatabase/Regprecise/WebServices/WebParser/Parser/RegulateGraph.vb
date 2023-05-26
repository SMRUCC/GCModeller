Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language

Namespace Regprecise

    Public Module RegulateGraph

        <Extension>
        Public Iterator Function ParseStream(data As IEnumerable(Of String)) As IEnumerable(Of Operon)
            Dim genes As New List(Of RegulatedGene)
            Dim note As String = Nothing

            For Each line As String In data
                If line.StringEmpty Then
                    ' break the operon
                    If genes > 0 Then
                        Yield New Operon With {
                            .ID = "",
                            .members = genes.PopAll,
                            .note = note
                        }
                    End If
                ElseIf line.First = "#"c Then
                    ' break the regulation
                    If genes > 0 Then
                        Yield New Operon With {
                            .ID = "",
                            .members = genes.PopAll,
                            .note = note
                        }
                    End If

                    note = line
                Else
                    genes.Add(RegulatedGene.__parser(line))
                End If
            Next

            If genes > 0 Then
                Yield New Operon With {
                    .ID = "",
                    .members = genes.PopAll,
                    .note = note
                }
            End If
        End Function
    End Module
End Namespace