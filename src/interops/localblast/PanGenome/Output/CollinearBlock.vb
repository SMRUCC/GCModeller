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
    '     Properties: Chr1, Chr2, End1, End2, GenePairCount, Genome1, Genome2
    '                 Length1, Length2, LinkCount, OrthologyLinks, Score
    '                 Start1, Start2
    ' 
    '     Constructor: (+4 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' 共线性区块定义
''' </summary>
''' <remarks>
''' 区块除了记录参与共线性的两个基因组/染色体之外，还会记录区块在这两条染色体之上
''' 所覆盖的起止坐标(<see cref="Start1"/> / <see cref="End1"/> / <see cref="Start2"/> / <see cref="End2"/>，
''' 分别取区块内所有基因的最小起始位点与最大终止位点)，这样子就可以直接用于共线性绘图。
''' </remarks>
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

    ''' <summary>
    ''' 区块在基因组1的 <see cref="Chr1"/> 之上的起始位点（区块内所有基因的最小起始位点）
    ''' </summary>
    ''' <returns></returns>
    Public Property Start1 As Integer
    ''' <summary>
    ''' 区块在基因组1的 <see cref="Chr1"/> 之上的终止位点（区块内所有基因的最大终止位点）
    ''' </summary>
    ''' <returns></returns>
    Public Property End1 As Integer
    ''' <summary>
    ''' 区块在基因组2的 <see cref="Chr2"/> 之上的起始位点（区块内所有基因的最小起始位点）
    ''' </summary>
    ''' <returns></returns>
    Public Property Start2 As Integer
    ''' <summary>
    ''' 区块在基因组2的 <see cref="Chr2"/> 之上的终止位点（区块内所有基因的最大终止位点）
    ''' </summary>
    ''' <returns></returns>
    Public Property End2 As Integer

    ''' <summary>
    ''' 区块在基因组1之上的长度（bp），没有坐标信息的时候返回0
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Length1 As Integer
        Get
            Return Math.Max(0, End1 - Start1 + 1)
        End Get
    End Property

    ''' <summary>
    ''' 区块在基因组2之上的长度（bp），没有坐标信息的时候返回0
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Length2 As Integer
        Get
            Return Math.Max(0, End2 - Start2 + 1)
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

    ''' <summary>
    ''' 构造一个包含起止坐标信息的共线性区块
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="links">区块之内的同源基因对</param>
    ''' <param name="range">区块在两个基因组之上的坐标范围</param>
    ''' <remarks>
    ''' 坐标信息(两个方向上基因的最小起始位点与最大终止位点)是共线性绘图
    ''' (点图、带状图)所必须的；在基因组数量很多、逐基因配对数据被关闭的
    ''' 摘要模式下面，坐标信息依然会被完整保留下来。
    ''' </remarks>
    Friend Sub New(source As CollinearBlock, links As IEnumerable(Of OrthologyLink), range As CollinearRange)
        Call Me.New(source, links)

        Start1 = range.Start1
        End1 = range.End1
        Start2 = range.Start2
        End2 = range.End2
    End Sub

    ''' <summary>
    ''' 构造一个仅包含统计信息与坐标范围的共线性区块摘要
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="linkCount">区块内的同源基因对数量</param>
    ''' <param name="range">区块在两个基因组之上的坐标范围</param>
    Friend Sub New(source As CollinearBlock, linkCount As Integer, range As CollinearRange)
        Call Me.New(source, linkCount)

        Start1 = range.Start1
        End1 = range.End1
        Start2 = range.Start2
        End2 = range.End2
    End Sub

End Class

''' <summary>
''' 共线性区块在两个基因组之上所覆盖的坐标范围
''' </summary>
Public Structure CollinearRange

    Public Property Start1 As Integer
    Public Property End1 As Integer
    Public Property Start2 As Integer
    Public Property End2 As Integer

End Structure
