Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Data.Regtransbase.WebServices
Imports SMRUCC.genomics.SequenceModel.FASTA

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

        <Extension>
        Public Iterator Function ParseMotifSites(data As IEnumerable(Of String)) As IEnumerable(Of Regulator)
            Dim type As String = Nothing
            Dim family As String = Nothing
            Dim regulator_id As String = Nothing
            Dim sites As New List(Of MotifFasta)
            Dim fa_title As String = Nothing

            For Each line As String In data
                If line.StringEmpty Then
                    Continue For
                End If

                If line.First = "#"c Then
                    If sites > 0 Then
                        Yield New Regulator With {
                            .family = family,
                            .type = If(type = "TF", Types.TF, Types.RNA),
                            .regulator = New NamedValue(regulator_id, .family),
                            .regulatorySites = sites.PopAll
                        }
                    End If

                    With line.GetTagValue("-", trim:=True)
                        type = .Name
                        With .Value.GetTagValue(":", trim:=True)
                            family = .Name
                            regulator_id = .Value
                        End With
                    End With
                ElseIf line.First = ">"c Then
                    fa_title = line
                Else
                    sites += FastaSeq.ParseFromStream({fa_title, line}, {" "c}).DoCall(AddressOf MotifFasta.[New])
                End If
            Next

            If sites > 0 Then
                Yield New Regulator With {
                    .family = family,
                    .type = If(type = "TF", Types.TF, Types.RNA),
                    .regulator = New NamedValue(regulator_id, .family),
                    .regulatorySites = sites.PopAll
                }
            End If
        End Function
    End Module
End Namespace