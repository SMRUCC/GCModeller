#Region "Microsoft.VisualBasic::8267c673c0d7e861f7051020c22e60c6, analysis\SequenceToolkit\SequenceAlignment\Diamond\DistributedScheduler.vb"

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
    '    Code Lines: 22 (44.00%)
    ' Comment Lines: 18 (36.00%)
    '    - Xml Docs: 44.44%
    ' 
    '   Blank Lines: 10 (20.00%)
    '     File Size: 2.55 KB


    '     Class DistributedScheduler
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: DispatchCore, Run
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' 分布式调度骨架 (Distributed Scheduler Skeleton)
'
' 提供跨节点分发多查询的接口占位与基于本地并行回退的骨架实现。
' 本阶段不引入任何第三方分布式依赖;实际生产部署时,可继承本类并重写
' <see cref="DispatchCore"/> 将查询分片发往远程节点(例如通过消息队列、
' gRPC / REST 远程调用 DiamondBlastp 服务),再聚合各节点返回的命中。
'
' 默认行为:在本地以 PLINQ 并行执行(与 <see cref="ParallelScheduler"/> 等价),
' 保证即使不接外部集群也可直接运行;子类仅需替换分发逻辑即可扩展到集群。

Imports System.Linq
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace DIAMOND

    ''' <summary>
    ''' 分布式调度骨架。重写 <see cref="DispatchCore"/> 以接入真实集群。
    ''' </summary>
    Public Class DistributedScheduler : Implements IDiamondScheduler

        ''' <summary>节点标识列表(占位;真实环境由部署配置注入)。</summary>
        Public ReadOnly Nodes As String()

        ''' <summary>本地并行度(无集群时作为回退并行度)。</summary>
        Public ReadOnly LocalDegreeOfParallelism As Integer

        Sub New(Optional nodes As String() = Nothing, Optional localDegreeOfParallelism As Integer = 0)
            Me.Nodes = If(nodes, New String() {"local"})
            Me.LocalDegreeOfParallelism = localDegreeOfParallelism
        End Sub

        Public Function Run(queries As FastaSeq(), subjectDb As IList(Of FastaSeq), perQuery As Func(Of FastaSeq, IEnumerable(Of DiamondHit))) As IEnumerable(Of DiamondHit) Implements IDiamondScheduler.Run
            ' 默认在本地并行执行;真实分布式部署应重写 DispatchCore 将查询切片分发到 Nodes。
            Return DispatchCore(queries, subjectDb, perQuery)
        End Function

        ''' <summary>
        ''' 分发核心(可被子类重写以接入集群)。默认实现为本地 PLINQ 并行回退。
        ''' </summary>
        Protected Overridable Function DispatchCore(queries As FastaSeq(), subjectDb As IList(Of FastaSeq), perQuery As Func(Of FastaSeq, IEnumerable(Of DiamondHit))) As IEnumerable(Of DiamondHit)
            Dim q = queries.AsParallel()

            If LocalDegreeOfParallelism > 0 Then
                q = q.WithDegreeOfParallelism(LocalDegreeOfParallelism)
            End If

            Return q.SelectMany(Function(query) perQuery(query)).ToArray
        End Function
    End Class
End Namespace

