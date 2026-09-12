#Region "Microsoft.VisualBasic::806b5bff00352fec7bbcedb13355c686, localblast\PanGenome\Output\CollinearBlock.vb"

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

    '   Total Lines: 33
    '    Code Lines: 18 (54.55%)
    ' Comment Lines: 11 (33.33%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 4 (12.12%)
    '     File Size: 884 B


    ' Class CollinearBlock
    ' 
    '     Properties: Chr1, Chr2, Genome1, Genome2, OrthologyLinks
    '                 Score
    ' 
    '     Constructor: (+2 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' 共线性区块定义
''' </summary>
Public Class CollinearBlock

    Public Property Genome1 As String
    Public Property Genome2 As String
    Public Property Chr1 As String
    Public Property Chr2 As String
    ''' <summary>
    ''' 区块包含的基因对
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' 在基因组数量非常多的情况下（例如上百个基因组的两两比较），
    ''' 逐基因的共线性配对数据会占用非常巨大的内存，这个时候会通过
    ''' <see cref="GenomeAnalyzer.RetainOrthologyLinks"/> 关闭掉这个属性的输出，
    ''' 只保留 <see cref="LinkCount"/> 统计数值。
    ''' </remarks>
    Public Property OrthologyLinks As OrthologyLink()

    ''' <summary>
    ''' 区块内的同源基因对数量，当 <see cref="OrthologyLinks"/> 为 Nothing 的时候使用这个属性做统计
    ''' </summary>
    ''' <returns></returns>
    Public Property LinkCount As Integer

    ''' <summary>
    ''' TODO: 评估指标：得分或E-value
    ''' </summary>
    ''' <returns></returns>
    Public Property Score As Double

    ''' <summary>
    ''' 区块内的同源基因对数量（自动兼容摘要模式）
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property GenePairCount As Integer
        Get
            If OrthologyLinks Is Nothing Then
                Return LinkCount
            Else
                Return OrthologyLinks.Length
            End If
        End Get
    End Property

    Sub New()
    End Sub

    Friend Sub New(source As CollinearBlock, links As IEnumerable(Of OrthologyLink))
        Genome1 = source.Genome1
        Genome2 = source.Genome2
        Chr1 = source.Chr1
        Chr2 = source.Chr2
        OrthologyLinks = links.ToArray
        LinkCount = OrthologyLinks.Length
        Score = source.Score
    End Sub

    ''' <summary>
    ''' 构造一个仅包含统计信息的共线性区块摘要（不保存逐基因的同源配对数据）
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="linkCount">区块内的同源基因对数量</param>
    Friend Sub New(source As CollinearBlock, linkCount As Integer)
        Genome1 = source.Genome1
        Genome2 = source.Genome2
        Chr1 = source.Chr1
        Chr2 = source.Chr2
        OrthologyLinks = Nothing
        LinkCount = linkCount
        Score = source.Score
    End Sub

End Class
