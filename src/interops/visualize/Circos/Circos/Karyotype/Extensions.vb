#Region "Microsoft.VisualBasic::181ab7408e5ee0363b1406032ece724d, Circos\Karyotype\Extensions.vb"

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

    '     Module KaryotypeExtensions
    ' 
    '         Function: LoopHole, MapsRaw, nt
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Karyotype

    <HideModuleName>
    Public Module KaryotypeExtensions

        ''' <summary>
        ''' 缺口的大小，这个仅仅在单个染色体的基因组绘图模型之中有效
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        <Extension>
        Public Function LoopHole(x As SkeletonInfo) As PropertyValue(Of Integer)
            Return PropertyValue(Of Integer).Read(Of SkeletonInfo)(x, NameOf(LoopHole))
        End Function

        ''' <summary>
        ''' nt核苷酸基因组序列拓展属性
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        <Extension>
        Public Function nt(x As Karyotype) As PropertyValue(Of FastaSeq)
            Return PropertyValue(Of FastaSeq).Read(Of Karyotype)(x, NameOf(nt))
        End Function

        <Extension>
        Public Function MapsRaw(x As Band) As PropertyValue(Of BlastnMapping)
            Return PropertyValue(Of BlastnMapping).Read(Of Band)(x, NameOf(MapsRaw))
        End Function
    End Module
End Namespace
