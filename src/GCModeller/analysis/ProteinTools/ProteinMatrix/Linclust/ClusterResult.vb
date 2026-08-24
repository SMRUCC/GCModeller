#Region "Microsoft.VisualBasic::5574b8949c1461e0aafcb7243c1d9a5b, analysis\ProteinTools\ProteinMatrix\Linclust\ClusterResult.vb"

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

    '   Total Lines: 63
    '    Code Lines: 29 (46.03%)
    ' Comment Lines: 30 (47.62%)
    '    - Xml Docs: 96.67%
    ' 
    '   Blank Lines: 4 (6.35%)
    '     File Size: 2.98 KB


    '     Class LinclustOptions
    ' 
    '         Properties: Aeff, coverage, evalue, fastFilterCoverage, fastFilterSeqid
    '                     m, seqidThreshold
    ' 
    '     Class Cluster
    ' 
    '         Properties: members, memberScores, representative
    ' 
    '         Function: ToString
    ' 
    '     Class ClusterResult
    ' 
    '         Properties: clusters, k, nClusters, nSeq
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' Linclust 聚类结果模型与配置项
Namespace Linclust

    ''' <summary>
    ''' Linclust 运行配置
    ''' </summary>
    Public Class LinclustOptions
        ''' <summary>每序列保留的最小哈希 k-mer 个数(默认 20)</summary>
        Public Property m As Integer = 20
        ''' <summary>一致性(identity)阈值,0-1。>=0.9 时 k_seqid=14,否则 10</summary>
        Public Property seqidThreshold As Double = 0.9
        ''' <summary>覆盖率(coverage)阈值,0-1</summary>
        Public Property coverage As Double = 0.8
        ''' <summary>E-value 阈值(预留,当前判据以一致性+覆盖率为主)</summary>
        Public Property evalue As Double = 0.001
        ''' <summary>阶段三快速过滤的一致性阈值(通常等于 seqidThreshold)</summary>
        Public Property fastFilterSeqid As Double = 0.9
        ''' <summary>阶段三快速过滤的覆盖率阈值(通常等于 coverage)</summary>
        Public Property fastFilterCoverage As Double = 0.8
        ''' <summary>缩减字母表有效大小 A_eff(用于 k 长度自动选择)</summary>
        Public Property Aeff As Double = 8.7
    End Class

    ''' <summary>
    ''' 单个聚类簇
    ''' </summary>
    Public Class Cluster
        ''' <summary>代表序列 ID(簇中最长成员)</summary>
        Public Property representative As Integer
        ''' <summary>簇内全部成员序列 ID(含代表)</summary>
        Public Property members As List(Of Integer)
        ''' <summary>
        ''' 成员序列 ID -> 相对代表序列的 Smith-Waterman 比对 score。
        ''' 代表序列自身记为该簇内成员比对 score 的最大值(语义上代表其家族内最优同源强度)。
        ''' 该字典由阶段五 <see cref="GreedyCover.Cluster"/> 依据携带 score 的有向边填充,
        ''' 供结果导出模块(<c>ClusterExporter</c>)读取,作为 <see cref="SequenceCluster.score"/> 的数据来源。
        ''' 旧调用方(如仅读取 representative/members)可忽略此字段。
        ''' </summary>
        Public Property memberScores As Dictionary(Of Integer, Double)

        Public Overrides Function ToString() As String
            Return $"repr={representative}, size={If(members Is Nothing, 0, members.Count)}"
        End Function
    End Class

    ''' <summary>
    ''' 聚类结果
    ''' </summary>
    Public Class ClusterResult
        ''' <summary>所有簇</summary>
        Public Property clusters As List(Of Cluster)
        ''' <summary>实际使用的 k-mer 长度</summary>
        Public Property k As Integer
        ''' <summary>序列总数</summary>
        Public Property nSeq As Integer
        ''' <summary>簇数</summary>
        Public ReadOnly Property nClusters As Integer
            Get
                Return If(clusters Is Nothing, 0, clusters.Count)
            End Get
        End Property
    End Class
End Namespace

