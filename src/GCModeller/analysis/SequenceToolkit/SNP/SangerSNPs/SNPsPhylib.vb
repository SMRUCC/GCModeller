#Region "Microsoft.VisualBasic::15019a0e6e7e374fd2476a5667a2e378, GCModeller\analysis\SequenceToolkit\SNP\SangerSNPs\SNPsPhylib.vb"

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

    '   Total Lines: 108
    '    Code Lines: 59
    ' Comment Lines: 30
    '   Blank Lines: 19
    '     File Size: 4.19 KB


    '     Module SNPsPhylib
    ' 
    '         Function: PhylibOfSNPSites
    ' 
    '     Structure Phylip
    ' 
    '         Function: Doc, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports FILE = System.IO.StreamWriter

Namespace SangerSNPs

    Public Module SNPsPhylib

        '	 *  Wellcome Trust Sanger Institute
        '	 *  Copyright (C) 2013  Wellcome Trust Sanger Institute
        '	 *  
        '	 *  This program is free software; you can redistribute it and/or
        '	 *  modify it under the terms of the GNU General Public License
        '	 *  as published by the Free Software Foundation; either version 3
        '	 *  of the License, or (at your option) any later version.
        '	 *  
        '	 *  This program is distributed in the hope that it will be useful,
        '	 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
        '	 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
        '	 *  GNU General Public License for more details.
        '	 *  
        '	 *  You should have received a copy of the GNU General Public License
        '	 *  along with this program; if not, write to the Free Software
        '	 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
        '	 

        ''' <summary>
        ''' Phylib Of SNP Sites
        ''' </summary>
        ''' <param name="number_of_snps"></param>
        ''' <param name="bases_for_snps"></param>
        ''' <param name="sequence_names"></param>
        ''' <param name="number_of_samples"></param>
        ''' <param name="output_reference"></param>
        ''' <param name="pseudo_reference_sequence"></param>
        ''' <param name="snp_locations"></param>
        Public Function PhylibOfSNPSites(number_of_snps As Integer,
                                         ByRef bases_for_snps As String(),
                                         ByRef sequence_names As String(),
                                         number_of_samples As Integer,
                                         output_reference As Integer,
                                         ByRef pseudo_reference_sequence As String,
                                         snp_locations As Integer()) As Phylip

            Dim buf As New List(Of FastaSeq)
            Dim seq As New List(Of Char)

            If output_reference = 1 Then
                For snp_counter As Integer = 0 To number_of_snps - 1
                    seq.Add(pseudo_reference_sequence(snp_locations(snp_counter)))
                Next

                buf += New FastaSeq({SNPsFasta.pseudo_reference_sequence & vbTab}, New String(seq.ToArray))
            End If

            For sample_counter = 0 To number_of_samples - 1
                Dim name As String = sequence_names(sample_counter) & vbTab

                Call seq.Clear()

                For snp_counter = 0 To number_of_snps - 1
                    seq.Add(bases_for_snps(snp_counter)(sample_counter))
                Next

                buf += New FastaSeq({name}, New String(seq.ToArray))
            Next

            Return New Phylip With {
                .numSamples = number_of_samples,
                .SNPs = buf.ToArray,
                .numSNPs = number_of_snps
            }
        End Function
    End Module

    ''' <summary>
    ''' 用来绘制SNP位点的进化树
    ''' </summary>
    Public Structure Phylip

        Public numSamples As Integer
        Public numSNPs As Integer
        Public SNPs As FastaSeq()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Function Doc() As String
            Dim sb As New StringBuilder

            Call sb.AppendLine($"{numSamples} {numSNPs}")

            For Each x In SNPs
                Call sb.AppendLine(x.Title)
                Call sb.AppendLine(x.SequenceData)
            Next

            Return sb.ToString
        End Function
    End Structure
End Namespace
