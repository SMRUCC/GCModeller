#Region "Microsoft.VisualBasic::23c2f958dbf7cfe76caf54917d1443ed, analysis\SequenceToolkit\SequenceAlignment\Diamond\ParallelScheduler.vb"

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

    '   Total Lines: 40
    '    Code Lines: 17 (42.50%)
    ' Comment Lines: 15 (37.50%)
    '    - Xml Docs: 26.67%
    ' 
    '   Blank Lines: 8 (20.00%)
    '     File Size: 1.79 KB


    '     Class ParallelScheduler
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Run
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' 单机并行调度器 (PLINQ)
'
' 基于 PLINQ (.AsParallel) 对查询集合分块并行,每个查询独立完成单查询流水线。
' 参照本项目 <see cref="CDHit"/> 的 .AsParallel 并行风格。
'
' 线程安全约定(由 <see cref="DiamondBlastp"/> 保证):
'   - 参考蛋白库与按形状缓存的 ReferenceIndex 为只读共享;
'   - 每个查询线程独立构建自己的查询侧索引,互不干扰;
'   - ReferenceIndex 的懒加载写入已在 DiamondBlastp 内用 SyncLock 保护。
' 因此本调度器无需额外加锁即可安全并行。

Imports System.Linq
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace DIAMOND

    ''' <summary>
    ''' 单机 PLINQ 并行调度器(默认多查询调度策略)。
    ''' </summary>
    Public Class ParallelScheduler : Implements IDiamondScheduler

        ''' <summary>并行度;为 0 表示由运行时自动选择(Environment.ProcessorCount)。</summary>
        Public ReadOnly DegreeOfParallelism As Integer

        Sub New(Optional degreeOfParallelism As Integer = 0)
            Me.DegreeOfParallelism = degreeOfParallelism
        End Sub

        Public Function Run(queries As FastaSeq(), subjectDb As IList(Of FastaSeq), perQuery As Func(Of FastaSeq, IEnumerable(Of DiamondHit))) As IEnumerable(Of DiamondHit) Implements IDiamondScheduler.Run
            Dim q = queries.AsParallel()

            If DegreeOfParallelism > 0 Then
                q = q.WithDegreeOfParallelism(DegreeOfParallelism)
            End If

            ' 保留顺序非必需,使用无序执行以获得更好吞吐;结果按查询顺序聚合由调用方决定
            Return q.SelectMany(Function(query) perQuery(query)).ToArray
        End Function
    End Class
End Namespace

