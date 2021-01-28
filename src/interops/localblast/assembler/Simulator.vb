#Region "Microsoft.VisualBasic::9a561721252f848518b47577d78d9aea, assembler\Simulator.vb"

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

    ' Module Simulator
    ' 
    '     Function: MakeFragments, MakeReads
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

''' <summary>
''' 测序数据的模拟生成模块
''' </summary>
Public Module Simulator

    ''' <summary>
    ''' 模拟基因组测序, 基因组序列上均匀的随机断裂
    ''' </summary>
    ''' <param name="nt"></param>
    ''' <param name="fragmentSize">碎片的平均大小</param>
    ''' <param name="totalFragments">产生的碎片总数量</param>
    ''' <returns></returns>
    Public Iterator Function MakeFragments(nt As FastaSeq,
                                           Optional fragmentSize% = 200,
                                           Optional variantSize% = 10,
                                           Optional totalFragments% = 50000) As IEnumerable(Of FastaSeq)
        Dim left As Integer
        Dim size As Integer
        Dim ntSize As Integer = nt.Length - fragmentSize
        Dim [variant] As New IntRange(fragmentSize - [variantSize], fragmentSize)
        Dim title$ = nt.Title

        For i As Integer = 0 To totalFragments - 1
            left = randf.NextInteger(ntSize)
            size = randf.GetRandomValue([variant])

            Yield New FastaSeq With {
                .Headers = {title, $"left={left}"},
                .SequenceData = nt _
                    .CutSequenceLinear(left, left + size) _
                    .SequenceData
            }
        Next
    End Function

    ''' <summary>
    ''' 模拟mRNA测序结果, 用来测试基因表达量的估算程序
    ''' </summary>
    ''' <param name="nt"></param>
    ''' <param name="context"></param>
    ''' <returns></returns>
    Public Iterator Function MakeReads(nt As FastaSeq, context As GFFTable) As IEnumerable(Of FastaSeq)

    End Function
End Module
