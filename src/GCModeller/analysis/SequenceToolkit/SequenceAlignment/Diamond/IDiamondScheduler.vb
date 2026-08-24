#Region "Microsoft.VisualBasic::2b6ae39a15062c2f72ef323a1f86dec2, analysis\SequenceToolkit\SequenceAlignment\Diamond\IDiamondScheduler.vb"

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

    '   Total Lines: 23
    '    Code Lines: 6 (26.09%)
    ' Comment Lines: 14 (60.87%)
    '    - Xml Docs: 50.00%
    ' 
    '   Blank Lines: 3 (13.04%)
    '     File Size: 1.14 KB


    '     Interface IDiamondScheduler
    ' 
    '         Function: Run
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' DIAMOND 多查询调度接口 (Scheduler Boundary)
'
' 将"多查询集合如何被分发执行"抽象为接口边界,使 <see cref="DiamondBlastp"/> 的
' 多查询重载不依赖具体并行/分布式策略。默认实现为 <see cref="ParallelScheduler"/>
' (单机 PLINQ 并行);<see cref="DistributedScheduler"/> 提供跨节点分发的骨架接口,
' 便于后续接入真实分布式计算框架(如通过消息队列 / 远程过程调用),而不修改
' 比对算法本身。

Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace DIAMOND

    ''' <summary>
    ''' 多查询调度边界。
    ''' queries: 查询序列数组。
    ''' subjectDb: 参考蛋白库(只读共享)。
    ''' perQuery: 对单条查询执行 DIAMOND 流水线并返回命中的函数(由 DiamondBlastp 提供)。
    ''' 返回: 所有查询命中的聚合结果。
    ''' </summary>
    Public Interface IDiamondScheduler
        Function Run(queries As FastaSeq(), subjectDb As IList(Of FastaSeq), perQuery As Func(Of FastaSeq, IEnumerable(Of DiamondHit))) As IEnumerable(Of DiamondHit)
    End Interface
End Namespace

