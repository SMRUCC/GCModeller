#Region "Microsoft.VisualBasic::80e5493df320fc967df2352c886b12c1, visualize\Circos\Circos\Karyotype\Adapters\ChromosomeGenerator.vb"

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

    '     Module ChromosomeGenerator
    ' 
    '         Function: chromosomes, FromNts
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Option Strict Off

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.Visualize.Circos.Colors
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

Namespace Karyotype

    Public Module ChromosomeGenerator

        ReadOnly shuffleCircosColors As New [Default](Of String())(Function() CircosColor.AllCircosColors.Shuffles, isLazy:=False)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="chrs"></param>
        ''' <param name="colors">
        ''' 如果这个参数为空值的话，则默认使用随机的<see cref="CircosColor.AllCircosColors"/>
        ''' </param>
        ''' <returns></returns>
        Public Function FromNts(chrs As IEnumerable(Of FastaSeq), Optional colors As String() = Nothing) As KaryotypeChromosomes
            Dim chrVector As FastaSeq() = chrs.ToArray
            Dim ks As Karyotype() = chrVector.chromosomes(colors Or shuffleCircosColors).ToArray

            With ks.VectorShadows
                .nt = chrVector
            End With

            Return New KaryotypeChromosomes(ks)
        End Function

        <Extension>
        Private Function chromosomes(chrVector As FastaSeq(), colors$()) As IEnumerable(Of Karyotype)
            Return From nt As SeqValue(Of FastaSeq)
                   In chrVector.SeqIterator(offset:=1)
                   Let fasta = nt.value
                   Let name As String = fasta.Title _
                       .Split("."c) _
                       .First _
                       .NormalizePathString(True) _
                       .Replace(" ", "_")
                   Let clInd As Integer = randf.NextInteger(colors.Length)
                   Select New Karyotype With {
                       .chrName = "chr" & nt.i,
                       .chrLabel = name,
                       .color = colors(clInd),
                       .start = 0,
                       .end = fasta.Length
                   }
        End Function
    End Module
End Namespace
