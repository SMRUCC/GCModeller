#Region "Microsoft.VisualBasic::a9c58a8fce8e068870f2b00d95b80bcf, GCModeller\core\Bio.Assembly\SequenceModel\NucleicAcid\Assembly.vb"

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

    '   Total Lines: 66
    '    Code Lines: 41
    ' Comment Lines: 13
    '   Blank Lines: 12
    '     File Size: 2.06 KB


    '     Class Assembly
    ' 
    '         Function: AssembleOriginal, ToString
    ' 
    '         Sub: Add, Replace
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace SequenceModel.NucleotideModels

    Public Class Assembly

        ReadOnly bits As New List(Of Char)

        Public Sub Add(contig As SimpleSegment)
            Dim reads As Char()
            Dim left% = contig.Start

            If contig.Strand.GetStrand = Strands.Forward Then
                reads = contig.SequenceData.ToArray
            Else
                ' reverse
                reads = contig _
                    .ReadComplement(1, contig.Length) _
                    .SequenceData _
                    .ToArray
            End If

            For i As Integer = 0 To contig.Length - 1
                Call Replace(left + i, reads(i))
            Next
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="location">以1为底的</param>
        ''' <param name="base"></param>
        Public Sub Replace(location As Integer, base As Char)
            If location >= bits.Count Then
                bits.AddRange("-"c.Replicate(10000 + (location - bits.Count)))
            End If

            bits(location) = base
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 不可以trim序列的起始，否则会乱码
        ''' </remarks>
        Public Overrides Function ToString() As String
            Return bits.CharString.TrimEnd("-"c).Replace("-"c, "N"c)
        End Function

        Public Shared Function AssembleOriginal(contigs As IEnumerable(Of SimpleSegment), Optional title$ = "Unknown") As FastaSeq
            Dim nt As New Assembly

            For Each read As SimpleSegment In contigs
                Call nt.Add(read)
            Next

            Return New FastaSeq With {
                .Headers = {title},
                .SequenceData = nt.ToString
            }
        End Function
    End Class
End Namespace
