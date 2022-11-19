#Region "Microsoft.VisualBasic::10b65371bb779bce3635f912960fac76, GCModeller\core\Bio.Assembly\ContextModel\Extensions.vb"

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

    '   Total Lines: 45
    '    Code Lines: 34
    ' Comment Lines: 6
    '   Blank Lines: 5
    '     File Size: 1.88 KB


    '     Module Extensions
    ' 
    '         Function: Offsets, RangeSelection
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace ContextModel

    Public Module Extensions

        ''' <summary>
        ''' 使用一个范围选出所有落在该范围内的基因
        ''' </summary>
        ''' <param name="genome"></param>
        ''' <param name="region"></param>
        ''' <returns></returns>
        <Extension>
        Public Function RangeSelection(genome As PTT, region As Location, Optional offset As Boolean = False) As PTT
            Dim genes As GeneBrief() = genome _
                .GeneObjects _
                .Where(Function(gene) region.IsOverlapping(gene.Location)) _
                .ToArray
            Dim title$ = genome.Title & " - region of " & region.ToString
            Dim data As GeneBrief() = If(offset, genes.Offsets(region.Left), genes)
            Dim subset As New PTT(data, title, region.Length)
            Return subset
        End Function

        <Extension>
        Public Function Offsets(genes As IEnumerable(Of GeneBrief), left%) As GeneBrief()
            Dim out As GeneBrief() = genes.Select(Function(gene) gene.Clone).ToArray
            Call out.ForEach(Sub(gene, i)
                                 With gene
                                     Dim l = .Location.Left
                                     Dim r = .Location.Right

                                     .Location = New NucleotideLocation(
                                         l - left, 
                                         r - left, 
                                         .Location.Strand)
                                 End With
                             End Sub)
            Return out
        End Function
    End Module
End Namespace
