#Region "Microsoft.VisualBasic::cca929452e2011a8aec74752186a98e5, annotations\Bifrost\Prodigal\Utils\GeneScore.vb"

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

    '   Total Lines: 59
    '    Code Lines: 51 (86.44%)
    ' Comment Lines: 3 (5.08%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 5 (8.47%)
    '     File Size: 2.25 KB


    ' Class GeneScore
    ' 
    '     Properties: [end], coding_score, frame, gene_index, length
    '                 partial_type, rbs_motif, rbs_score, rbs_spacing, seq_id
    '                 start, start_codon, start_score, stop_codon, strand
    '                 total_score, type_score, upstream_score
    ' 
    '     Function: ScoreTable
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.ComponentModel.Loci
Imports std = System.Math

''' <summary>
''' gene prediction score table
''' </summary>
Public Class GeneScore

    Public Property seq_id As String
    Public Property gene_index As Integer
    Public Property start As Integer
    Public Property [end] As Integer
    Public Property strand As Strands
    Public Property frame As Integer
    Public Property start_codon As String
    Public Property stop_codon As String
    Public Property rbs_motif As String
    Public Property total_score As Double
    Public Property coding_score As Double
    Public Property start_score As Double
    Public Property rbs_score As Double
    Public Property type_score As Double
    Public Property upstream_score As Double
    Public Property rbs_spacing As Integer
    Public Property partial_type As String

    Public ReadOnly Property length As Integer
        Get
            Return std.Abs(start - [end])
        End Get
    End Property

    Public Shared Iterator Function ScoreTable(results As IReadOnlyCollection(Of PredictionResult)) As IEnumerable(Of GeneScore)
        For Each result As PredictionResult In results
            For Each gene As PredictedGene In result.Genes
                Yield New GeneScore With {
                    .seq_id = result.SeqId,
                    .gene_index = gene.GeneIndex,
                    .start = gene.Start,
                    .[end] = gene.End,
                    .strand = gene.Strand.GetStrands,
                    .frame = gene.Frame + 1,
                    .start_codon = gene.StartCodon,
                    .stop_codon = gene.StopCodon,
                    .rbs_motif = gene.RbsMotif,
                    .total_score = gene.TotalScore,
                    .coding_score = gene.CodingScore,
                    .start_score = gene.StartScore,
                    .rbs_score = gene.RbsScore,
                    .type_score = gene.TypeScore,
                    .upstream_score = gene.UpstreamScore,
                    .rbs_spacing = gene.RbsSpacing,
                    .partial_type = gene.PartialType
                }
            Next
        Next
    End Function

End Class

