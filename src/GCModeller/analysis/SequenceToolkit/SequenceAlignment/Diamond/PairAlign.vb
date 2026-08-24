#Region "Microsoft.VisualBasic::232f65a7e459c3058d06cd57f3eed131, analysis\SequenceToolkit\SequenceAlignment\Diamond\PairAlign.vb"

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

    '   Total Lines: 94
    '    Code Lines: 35 (37.23%)
    ' Comment Lines: 49 (52.13%)
    '    - Xml Docs: 97.96%
    ' 
    '   Blank Lines: 10 (10.64%)
    '     File Size: 5.36 KB


    '     Class PairAlign
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: (+2 Overloads) AlignBestHSP, AlignDetailed, ComputeStats
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Analysis.SequenceAlignment
Imports SMRUCC.genomics.Analysis.SequenceAlignment.BestLocalAlignment
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace DIAMOND

    ''' <summary>
    ''' 轻量单对单局部比对判定器,面向序列聚类分析(如 CD-HIT 风格的成对同源判定)。
    ''' </summary>
    ''' <remarks>
    ''' 与 <see cref="DiamondBlastp"/> 的区别:DiamondBlastp 针对“一条查询 vs 一个库”
    ''' 做加速(双索引 / SIMD / 调度),而本类只解决“两条序列之间的最佳比对”这一原子操作,
    ''' 在聚类分析需要两两序列做判定的场景下更合适——避免为每条 pair 重复建立库索引。
    ''' 底层直接调用 GCModeller 标准的 <see cref="SmithWaterman"/> 局部比对,
    ''' 返回 <see cref="HSP"/> / <see cref="Output"/> 对象,供上层聚类流程直接使用。
    ''' </remarks>
    Public Class PairAlign

        ''' <summary>局部比对使用的替换矩阵,默认 BLOSUM62。</summary>
        Public ReadOnly Matrix As Blosum

        Sub New(Optional blosum As Blosum = Nothing)
            Me.Matrix = If(blosum, Blosum.FromInnerBlosum62)
        End Sub

        ''' <summary>
        ''' 对两条蛋白序列做最佳局部比对,返回得分最高的单条 <see cref="HSP"/>。
        ''' </summary>
        ''' <param name="query">查询序列(字符形式)。</param>
        ''' <param name="subject">参考(主题)序列(字符形式)。</param>
        ''' <param name="minWidth">最短 HSP 片段长度过滤(0 表示不限制)。</param>
        ''' <returns>得分最高的 HSP;若无正分比对则返回 Nothing。</returns>
        ''' <remarks>
        ''' 这里走的是 <see cref="SmithWaterman.GetBestHSP"/> 轻量级路径,而**不是**
        ''' <see cref="SmithWaterman.GetOutput"/>。后者会额外构建完整的 <see cref="Output"/>:
        ''' 复制一份动态规划得分矩阵、为方向矩阵逐行建立视图、并计算完整回溯路径,
        ''' 单次比对的额外开销与 query*subject 成正比(千级长度蛋白序列可达数十 MB),
        ''' 且这些数组会进入大对象堆(LOH)。
        ''' 
        ''' 由于本方法只需要一条最佳 HSP,在聚类分析的两两比对循环(O(n^2))中若使用
        ''' 完整路径,将造成进程常驻内存持续增长且无法回收。
        ''' </remarks>
        Public Function AlignBestHSP(query As String, subject As String, Optional minWidth As Integer = 0) As HSP
            Using sw As New SmithWaterman(query, subject, Matrix)
                Call sw.BuildMatrix()

                Return sw.GetBestHSP(cutoff:=0, minW:=minWidth)
            End Using
        End Function

        ''' <summary>
        ''' <see cref="FastaSeq"/> 重载,直接传入序列对象,内部使用其 <see cref="FastaSeq.SequenceData"/>。
        ''' </summary>
        Public Function AlignBestHSP(query As FastaSeq, subject As FastaSeq, Optional minWidth As Integer = 0) As HSP
            Return AlignBestHSP(query.SequenceData, subject.SequenceData, minWidth)
        End Function

        ''' <summary>
        ''' 完整局部比对,返回 <see cref="Output"/>,包含全部 HSP 链、DP 矩阵与回溯路径,
        ''' 供需要多条 HSP(如重叠高分区)的聚类分析使用。
        ''' </summary>
        ''' <param name="cutoff">收集 HSP 的得分阈值(占最高分比例,0-1;0 表示收集所有正分 HSP)。</param>
        ''' <remarks>
        ''' 返回的 <see cref="Output"/> 持有动态规划矩阵(其方向矩阵与 
        ''' <see cref="SmithWaterman"/> 共享行数组),属于重量级对象。
        ''' 调用方在用完之后应当及时 Dispose(<see cref="Output"/> 已实现 
        ''' <see cref="IDisposable"/>),否则在循环调用场景下会造成内存持续增长。
        ''' 若只需要最佳的那一条比对,请改用 <see cref="AlignBestHSP"/>。
        ''' </remarks>
        Public Function AlignDetailed(query As FastaSeq, subject As FastaSeq, Optional cutoff As Double = 0, Optional minWidth As Integer = 0) As Output
            Using sw As New SmithWaterman(query.SequenceData, subject.SequenceData, Matrix)
                Call sw.BuildMatrix()

                Return sw.GetOutput(cutoff, minW:=minWidth)
            End Using
        End Function

        ''' <summary>
        ''' 计算给定 HSP 的 BitScore 与 E-value(Karlin-Altschul 模型,BLOSUM62 统计量)。
        ''' </summary>
        ''' <param name="hsp">由 <see cref="AlignBestHSP"/> / <see cref="AlignDetailed"/> 产出的比对。</param>
        ''' <param name="queryLength">查询序列全长。</param>
        ''' <param name="subjectLength">参考序列全长。</param>
        Public Shared Function ComputeStats(hsp As HSP, queryLength As Integer, subjectLength As Integer) As (BitScore As Double, EValue As Double)
            If hsp Is Nothing Then
                Return (0, 1)
            End If
            Dim raw = hsp.score
            Dim bitScore = (0.267 * raw - Math.Log(0.041)) / Math.Log(2)
            Dim eval = EValue.Compute(raw, queryLength, subjectLength)
            Return (bitScore, eval)
        End Function
    End Class
End Namespace

