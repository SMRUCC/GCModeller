#Region "Microsoft.VisualBasic::febdc956a211897f52d74f6d2a39ca9c, analysis\SequenceToolkit\MotifFinder\Gibbs\RunSample.vb"

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

    '   Total Lines: 50
    '    Code Lines: 41 (82.00%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 9 (18.00%)
    '     File Size: 1.85 KB


    ' Class RunSample
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Sub: RunOne
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language

''' <summary>
''' 单次「多重启吉布斯采样」的执行器：从多个随机初始状态独立运行采样，
''' 并保留其中信息含量最高的那一份位点与 motif 序列。
''' </summary>
Friend Class RunSample

    ''' <summary>
    ''' 判定信息含量是否已达理论上限时所使用的浮点容差
    ''' </summary>
    Const ICPC_EPSILON As Double = 0.0000001

    Friend ReadOnly sampler As GibbsSampler
    Friend ReadOnly sequences As String()

    ''' <summary>
    ''' 下列三个字段是各个并行重启之间共享的可变状态，必须统一由这一把锁保护。
    ''' </summary>
    Private ReadOnly syncRoot As New Object()

    Private bestInformationContent As Double = Double.NegativeInfinity
    Friend ReadOnly predictedMotifs As New List(Of String)
    Friend ReadOnly predictedSites As New List(Of Integer)

    ''' <summary>
    ''' 当前所有重启之中找到的最大信息含量
    ''' </summary>
    ''' <returns></returns>
    Friend ReadOnly Property InformationContent As Double
        Get
            SyncLock syncRoot
                Return bestInformationContent
            End SyncLock
        End Get
    End Property

    ''' <summary>
    ''' 单位列信息含量(bits/column)，其理论上界为 log2(4) = 2
    ''' </summary>
    ''' <returns></returns>
    Friend ReadOnly Property ICPC As Double
        Get
            Return InformationContent / sampler.m_motifLength
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="gibbs"></param>
    ''' <param name="work">
    ''' 本轮采样所使用的工作序列集合。在迭代式屏蔽发现(findTopN)之中，
    ''' 这里传入的是上一轮把已发现位点屏蔽为 N 之后的序列；
    ''' 为空时表示直接使用采样器之中的原始序列。
    ''' </param>
    Sub New(gibbs As GibbsSampler, Optional work As String() = Nothing)
        sampler = gibbs
        sequences = If(work Is Nothing, gibbs.Sequences.ToArray, work)
    End Sub

    Public Sub RunOne(maxIterations As Integer)
        ' 已经收敛到信息含量的理论上限，再多的重启也不会更好，直接跳过
        SyncLock syncRoot
            If bestInformationContent / sampler.m_motifLength >= GibbsSampler.MAX_ICPC - ICPC_EPSILON Then
                Return
            End If
        End SyncLock

        Dim sites As List(Of Integer) = sampler.gibbsSample(maxIterations, New List(Of String)(sequences))
        Dim motifs As List(Of String) = sampler.getMotifStrings(sequences, sites)
        Dim informationContent As Double = sampler.informationContent(motifs)
        Dim newMax As Boolean

        ' 比较与更新必须在同一把锁之内原子完成。
        ' 原实现把「判定」与「写入」拆成了两段，并且对三个字段分别使用不同的锁对象，
        ' 并发之下会交错产生「位点来自线程 A、而 motif 序列来自线程 B」的静默数据损坏。
        SyncLock syncRoot
            newMax = informationContent >= bestInformationContent

            If newMax Then
                bestInformationContent = informationContent

                predictedSites.Clear()
                predictedSites.AddRange(sites)

                predictedMotifs.Clear()
                predictedMotifs.AddRange(motifs)

                Call VBDebugger.EchoLine(
                    informationContent.ToString() & " :: " &
                    sites.Select(Function(k) k.ToString).JoinBy(" "))
            End If
        End SyncLock
    End Sub
End Class
