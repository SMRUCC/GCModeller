#Region "Microsoft.VisualBasic::ad5b15642a7eb60c381879eed11e6512, GCModeller\analysis\Metagenome\Metagenome\OTUTable\OTU.vb"

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

    '   Total Lines: 146
    '    Code Lines: 125
    ' Comment Lines: 9
    '   Blank Lines: 12
    '     File Size: 6.05 KB


    ' Module OTU
    ' 
    '     Function: BuildOTUClusters, CreateGastCountTabel, LoadOTU_taxa_table
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.SequenceTools
Imports SMRUCC.genomics.Metagenomics
Imports SMRUCC.genomics.Metagenomics.BIOMTaxonomy
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports Table = Microsoft.VisualBasic.Data.csv.IO.File

Public Module OTU

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function CreateGastCountTabel(table As IEnumerable(Of OTUTable), sampleName$) As IEnumerable(Of gast.gastOUT)
        Return table _
            .Select(Function(OTU)
                        Return New gast.gastOUT With {
                            .counts = OTU(sampleName),
                            .distance = 0,
                            .max_pcts = 1,
                            .minrank = 1,
                            .na_pcts = 0,
                            .rank = 1,
                            .read_id = OTU.ID,
                            .refhvr_ids = 1,
                            .refssu_count = 1,
                            .taxa_counts = 1,
                            .taxonomy = OTU.taxonomy.ToString(BIOMstyle:=True),
                            .vote = 1
                        }
                    End Function)
    End Function

    ''' <summary>
    ''' Only works on the 16S/18S data
    ''' </summary>
    ''' <param name="contigs"></param>
    ''' <param name="similarity">
    ''' Two sequence at least have 97% percentage similarity that can be cluster into one OTU
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function BuildOTUClusters(contigs As IEnumerable(Of FastaSeq), output As StreamWriter, Optional similarity# = 97%) As NamedValue(Of String())()
        Dim ref As FastaSeq = contigs.First
        Dim OTUs As New List(Of (ref As FastaSeq, fullEquals#, cluster As NamedValue(Of List(Of String))))
        Dim n As i32 = 1

        OTUs += (ref,
            fullEquals:=RunNeedlemanWunsch.RunAlign(
                ref, ref,
                output),
            cluster:=New NamedValue(Of List(Of String)) With {
                .Name = "OTU_" & ++n,
                .Value = New List(Of String) From {
                    ref.Title
                }
        })

        For Each seq As FastaSeq In contigs.Skip(1)
            Dim matched = LinqAPI.DefaultFirst(Of (ref As FastaSeq, fullEquals#, cluster As NamedValue(Of List(Of String)))) <=
                From OTU As (ref As FastaSeq, fullEquals#, cluster As NamedValue(Of List(Of String)))
                In OTUs.AsParallel
                Let score As Double = RunNeedlemanWunsch.RunAlign(
                    seq, OTU.ref,
                    output)
                Let is_matched As Double = 100 * score / OTU.fullEquals
                Where is_matched >= similarity
                Select OTU

            If matched.ref Is Nothing Then
                ' 没有找到匹配的，则是新的cluster
                OTUs += (seq,
                    fullEquals:=RunNeedlemanWunsch.RunAlign(
                        seq, seq,
                        output),
                    cluster:=New NamedValue(Of List(Of String)) With {
                        .Name = "OTU_" & ++n,
                        .Value = New List(Of String) From {
                            seq.Title
                        }
                    })
            Else
                matched.cluster.Value.Add(seq.Title) ' 是当前的找到的这个OTU的
            End If
        Next

        Return LinqAPI.Exec(Of NamedValue(Of String())) <=
 _
            From OTU As (ref As FastaSeq, fullEquals#, cluster As NamedValue(Of List(Of String)))
            In OTUs
            Let refSeq As FastaSeq = New FastaSeq With {
                .SequenceData = OTU.ref.SequenceData,
                .Headers = {
                    OTU.cluster.Name & " " & OTU.ref.Title
                }
            }
            Let OTUseq As String = refSeq.GenerateDocument(-1)
            Let cluster As NamedValue(Of List(Of String)) = OTU.cluster
            Select New NamedValue(Of String()) With {
                .Name = cluster.Name,
                .Value = cluster.Value,
                .Description = OTUseq
            }
    End Function

    Public Iterator Function LoadOTU_taxa_table(tableFile$, Optional tsv As Boolean = True, Optional brief As Boolean = True) As IEnumerable(Of OTUTable)
        Dim csv As Table = If(tsv, Table.LoadTsv(tableFile, Encodings.UTF8), Table.Load(tableFile))
        Dim parser As TaxonomyLineageParser = If(brief, BriefParser, CompleteParser)
        Dim taxonomyIndex%

        With csv.Headers.Indexing
            If .IndexOf("taxonomy") > -1 Then
                taxonomyIndex = .IndexOf("taxonomy")
            ElseIf .IndexOf("Taxonomy") > -1 Then
                taxonomyIndex = .IndexOf("Taxonomy")
            Else
                taxonomyIndex = -1
            End If
        End With

        Dim title() = csv.Headers _
            .SeqIterator _
            .Where(Function(i)
                       Return i <> 0 AndAlso i <> taxonomyIndex
                   End Function) _
            .ToArray

        For Each row As RowObject In csv.Skip(1)
            Dim ID$ = row.First
            Dim taxonomy$ = If(taxonomyIndex > -1, row(taxonomyIndex), "")
            Dim data As Dictionary(Of String, Double) = title _
                .ToDictionary(Function(s) s.value,
                              Function(i) Val(row(i)))

            Yield New OTUTable With {
                .ID = ID,
                .Properties = data,
                .taxonomy = New Taxonomy(parser(taxonomy))
            }
        Next
    End Function
End Module
