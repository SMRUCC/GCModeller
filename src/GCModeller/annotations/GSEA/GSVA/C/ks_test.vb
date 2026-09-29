#Region "Microsoft.VisualBasic::68095f7ec6c1b6cdb1c44d21a926ea63, annotations\GSEA\GSVA\C\ks_test.vb"

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

    '   Total Lines: 100
    '    Code Lines: 66 (66.00%)
    ' Comment Lines: 16 (16.00%)
    '    - Xml Docs: 75.00%
    ' 
    '   Blank Lines: 18 (18.00%)
    '     File Size: 3.77 KB


    '     Module ks_test
    ' 
    '         Function: ks_matrix_R, ks_sample
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports std = System.Math

Namespace C

    ''' <summary>
    ''' GSVA 的 KS 随机游走统计量
    ''' </summary>
    ''' <remarks>
    ''' 本模块是 R 包 GSVA 中 ``src/ks_test.c`` 的 ``gsva_rnd_walk()`` 与
    ''' ``gsva_score_genesets_R()`` 的 VB 移植，数值语义与官方实现逐点对齐。
    ''' </remarks>
    Module ks_test

        ''' <summary>
        ''' 计算单个基因集在全部样本上的富集分数
        ''' </summary>
        ''' <param name="symrnkstat">
        ''' 对称秩统计量，按样本组织：``symrnkstat(样本)(基因) = |p / 2 - rank|``
        ''' </param>
        ''' <param name="decordstat">
        ''' 降序序数，按样本组织：``decordstat(样本)(基因)`` 取值 1..p，1 表示该样本中表达最高的基因
        ''' </param>
        ''' <param name="n_genes">基因（特征）总数 p</param>
        ''' <param name="geneset_idxs">当前基因集所含基因的下标（0 基）</param>
        ''' <param name="n_geneset">当前基因集的基因数 k</param>
        ''' <param name="tau">随机游走尾部的权重指数，默认为 1</param>
        ''' <param name="n_samples">样本数</param>
        ''' <param name="mx_diff">
        ''' TRUE 时取最大正负偏差之差（默认，修正 Kuiper 统计量）；
        ''' FALSE 时取偏离 0 最远的单个偏差
        ''' </param>
        ''' <param name="abs_rnk">
        ''' 仅当 <paramref name="mx_diff"/> 为 TRUE 时生效：TRUE 使用原始 Kuiper 统计量
        ''' （最大正偏差减去最大负偏差），FALSE 使用修正 Kuiper 统计量（两者相加）
        ''' </param>
        ''' <returns>
        ''' 长度为 <paramref name="n_samples"/> 的富集分数向量，取值落在 [-1, 1]；
        ''' 退化情形（基因集权重和为零、或基因集覆盖了全部基因）下对应样本返回 <see cref="Double.NaN"/>
        ''' </returns>
        Friend Function ks_matrix_R(symrnkstat As Double()(),
                                    decordstat As Integer()(),
                                    n_genes As Integer,
                                    geneset_idxs As Integer(),
                                    n_geneset As Integer,
                                    tau As Double,
                                    n_samples As Integer,
                                    mx_diff As Boolean,
                                    abs_rnk As Boolean) As Double()

            Dim R As Double() = New Double(n_samples - 1) {}
            ' 两个游走缓冲在同一次调用内复用，避免在 基因数 x 样本数 量级上反复分配
            Dim stepIn As Double() = New Double(n_genes - 1) {}
            Dim stepOut As Double() = New Double(n_genes - 1) {}
            Dim noTau As Boolean = (tau = 1.0)

            For j As Integer = 0 To n_samples - 1
                R(j) = ks_sample(
                    symrnkstat:=symrnkstat(j),
                    decordstat:=decordstat(j),
                    n_genes:=n_genes,
                    geneset_idxs:=geneset_idxs,
                    n_geneset:=n_geneset,
                    tau:=tau,
                    noTau:=noTau,
                    mx_diff:=mx_diff,
                    abs_rnk:=abs_rnk,
                    stepIn:=stepIn,
                    stepOut:=stepOut
                )
            Next

            Return R
        End Function

        ''' <summary>
        ''' 单个样本上的 KS 随机游走
        ''' </summary>
        ''' <remarks>
        ''' 对应官方 C 实现的 ``gsva_rnd_walk()``：
        ''' 
        ''' 1. 在「表达由高到低」的遍历序（即 <paramref name="decordstat"/> 给出的位置）上，
        '''    集内基因贡献 <paramref name="symrnkstat"/>`^tau 的步长，集外基因贡献步长 1
        ''' 2. 两条步长序列各自做前缀和并按末元素归一
        ''' 3. 游走统计量 = 集内归一前缀和 - 集外归一前缀和
        ''' 4. 由最大正偏差 <paramref name="mx_pos"/> 与最大负偏差 <paramref name="mx_neg"/> 聚合出富集分数
        ''' </remarks>
        Private Function ks_sample(symrnkstat As Double(),
                                   decordstat As Integer(),
                                   n_genes As Integer,
                                   geneset_idxs As Integer(),
                                   n_geneset As Integer,
                                   tau As Double,
                                   noTau As Boolean,
                                   mx_diff As Boolean,
                                   abs_rnk As Boolean,
                                   stepIn As Double(),
                                   stepOut As Double()) As Double

            Dim i As Integer
            Dim pos As Integer
            Dim gene As Integer
            Dim stat As Double
            Dim mx_pos As Double
            Dim mx_neg As Double
            Dim wlkstat As Double
            Dim totalIn As Double
            Dim totalOut As Double

            Array.Clear(stepIn, 0, n_genes)

            For i = 0 To n_genes - 1
                stepOut(i) = 1
            Next

            For i = 0 To n_geneset - 1
                gene = geneset_idxs(i)
                pos = decordstat(gene) - 1
                stat = symrnkstat(gene)

                stepIn(pos) = If(noTau, stat, std.Pow(stat, tau))
                stepOut(pos) = 0
            Next

            For i = 1 To n_genes - 1
                stepIn(i) += stepIn(i - 1)
                stepOut(i) += stepOut(i - 1)
            Next

            totalIn = stepIn(n_genes - 1)
            totalOut = stepOut(n_genes - 1)

            ' 官方实现在此处要求两条累积曲线的总和都严格为正，否则随机游走无法归一化，
            ' 对应情形（例如基因集权重和为零、基因集覆盖全部基因）应记为 NA
            If totalIn <= 0 OrElse totalOut <= 0 Then
                Return Double.NaN
            End If

            mx_pos = 0
            mx_neg = 0

            For i = 0 To n_genes - 1
                wlkstat = stepIn(i) / totalIn - stepOut(i) / totalOut

                If wlkstat > mx_pos Then
                    mx_pos = wlkstat
                End If
                If wlkstat < mx_neg Then
                    mx_neg = wlkstat
                End If
            Next

            If mx_diff Then
                If abs_rnk Then
                    ' 原始 Kuiper 统计量
                    Return mx_pos - mx_neg
                Else
                    ' 修正 Kuiper 统计量（默认）
                    Return mx_pos + mx_neg
                End If
            Else
                Return If(mx_pos > std.Abs(mx_neg), mx_pos, mx_neg)
            End If
        End Function
    End Module
End Namespace
