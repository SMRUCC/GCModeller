#Region "Microsoft.VisualBasic::05618b5e5eda8a9b602106a1004ece85, G:/GCModeller/src/GCModeller/data/RegulonDatabase//Regprecise/WebServices/WebParser/Parser/RegulateGraph.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 105
    '    Code Lines: 86
    ' Comment Lines: 7
    '   Blank Lines: 12
    '     File Size: 3.87 KB


    '     Module RegulateGraph
    ' 
    '         Function: ParseMotifSites, ParseStream
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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

        ''' <summary>
        ''' Parse the motif site fasta data
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
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
                        type = .Name.Trim(" "c, "#"c)

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
