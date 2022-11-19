#Region "Microsoft.VisualBasic::fc7a81266f37283722300d45f10d630c, GCModeller\analysis\SequenceToolkit\SNP\SangerSNPs\SNPsFasta.vb"

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

    '   Total Lines: 76
    '    Code Lines: 34
    ' Comment Lines: 29
    '   Blank Lines: 13
    '     File Size: 3.23 KB


    '     Module SNPsFasta
    ' 
    '         Function: SNPSitesFasta
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports FILE = System.IO.StreamWriter
Imports Microsoft.VisualBasic

Namespace SangerSNPs

    Public Module SNPsFasta

        '	 *  Wellcome Trust Sanger Institute
        '	 *  Copyright (C) 2011  Wellcome Trust Sanger Institute
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

        ''' <summary>
        ''' pseudo_reference_sequence fasta title
        ''' </summary>
        Public Const pseudo_reference_sequence As String = "pseudo_reference_sequence"

        ''' <summary>
        ''' Create fasta file of snps sites.
        ''' </summary>
        ''' <param name="number_of_snps"></param>
        ''' <param name="bases_for_snps"></param>
        ''' <param name="sequence_names"></param>
        ''' <param name="number_of_samples"></param>
        ''' <param name="output_reference"></param>
        ''' <param name="pseudo_reference_sequence"></param>
        ''' <param name="snp_locations"></param>
        Public Function SNPSitesFasta(number_of_snps As Integer,
                                      ByRef bases_for_snps As String(),
                                      ByRef sequence_names As String(),
                                      number_of_samples As Integer,
                                      output_reference As Integer,
                                      ByRef pseudo_reference_sequence As String,
                                      snp_locations As Integer()) As FastaFile

            Dim fasta As New FastaFile
            Dim seq As New List(Of Char)

            If output_reference = 1 Then
                For snp_counter = 0 To number_of_snps - 1
                    seq.Add(pseudo_reference_sequence(snp_locations(snp_counter)))
                Next

                fasta += New FastaSeq({SNPsFasta.pseudo_reference_sequence}, New String(seq.ToArray))
            End If

            For sample_counter As Integer = 0 To number_of_samples - 1
                Dim atrs As String = sequence_names(sample_counter)

                Call seq.Clear()

                For snp_counter As Integer = 0 To number_of_snps - 1
                    Call seq.Add(bases_for_snps(snp_counter)(sample_counter))
                Next

                fasta += New FastaSeq({atrs}, New String(seq.ToArray))
            Next

            Return fasta
        End Function
    End Module
End Namespace
