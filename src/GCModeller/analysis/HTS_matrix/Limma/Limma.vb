#Region "Microsoft.VisualBasic::7fe149c98f599a4c960e712ac1a7a0e7, analysis\HTS_matrix\Limma\Limma.vb"

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

    '   Total Lines: 234
    '    Code Lines: 107 (45.73%)
    ' Comment Lines: 105 (44.87%)
    '    - Xml Docs: 57.14%
    ' 
    '   Blank Lines: 22 (9.40%)
    '     File Size: 10.59 KB


    ' Module Limma
    ' 
    '     Function: LmFit, LmFitInternal
    ' 
    '     Sub: ValidateSampleIDsHelper
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Statistics
Imports Microsoft.VisualBasic.Math.Statistics.Linq
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports std = System.Math

''' <summary>
''' The **limma algorithm** (Linear Models for Microarray Data) is a widely used statistical framework in 
''' R/Bioconductor for differential expression (DE) analysis of RNA-seq data. Originally designed for 
''' microarray studies, its flexibility and robustness have extended its utility to RNA-seq through the 
''' `voom` transformation. Below is a comprehensive overview of its workflow, features, and applications:  
'''
''' ---
'''
''' ### **1. Core Philosophy**  
''' 
''' - **Linear Modeling**: Uses linear models to relate gene expression to experimental conditions, covariates, or interactions .  
''' - **Empirical Bayes Moderation**: Borrows information across genes to stabilize variance estimates, especially effective for small sample sizes .  
''' - **Adaptability**: Processes data from microarrays, RNA-seq, PCR, and other platforms with a unified pipeline after preprocessing .  
'''
''' ### **2. Key Steps in RNA-seq Analysis with limma**  
''' 
''' #### **A. Preprocessing**  
''' 
''' - **Normalization**:  
''' - Applies methods like **TMM** (edgeR), **quantile normalization**, or **voom transformation** to correct for library size, sequencing depth, and technical biases .  
''' - `voom` converts count data into log2 counts per million (CPM) and calculates precision weights for each observation based on the mean-variance trend .  
''' - **Batch Effect Correction**: Uses `removeBatchEffect()` or incorporates batch covariates into the design matrix .  
'''
''' #### **B. Linear Modeling**  
''' 
''' - **Design Matrix**: Constructed using `model.matrix()` to encode experimental factors (e.g., treatment vs. control, time points) .  
''' - **Model Fitting**:  
''' - `lmFit()` fits a linear model to normalized expression data.  
''' - `eBayes()` applies empirical Bayes moderation to t-statistics, enhancing DE detection reliability .  
'''
''' #### **C. Differential Expression Testing**  
''' 
''' - **Contrasts**: Define comparisons (e.g., `treatment - control`) with `makeContrasts()` .  
''' - **DE Gene Extraction**:  
''' - `topTable()` outputs DE genes ranked by statistical significance (adjusted *p*-values) and log-fold change (logFC) .  
''' - Thresholds: Commonly use **|logFC| > 1** and **adj. *p*-value &lt; 0.05** .  
'''
''' #### **D. Visualization**  
''' 
''' - **Volcano Plots**: Highlight DE genes (up/downregulated) using `ggplot2` or `ggVolcano` .  
''' - **Heatmaps**: Display expression patterns of DE genes across samples .  
'''
''' ### **3. Advanced Features**  
''' 
''' - **Complex Designs**: Handles multi-factor experiments (e.g., interactions, time series) and repeated measurements .  
''' - **Differential Splicing**: Detects alternative splicing events in RNA-seq data .  
''' - **Gene Set Analysis**: Integrates with tools like *camera* or *romer* to test co-regulated gene sets or pathways .  
'''
'''
''' ### **4. Strengths &amp; Limitations**  
''' 
''' - **Strengths**:  
''' - Flexibility for diverse experimental designs.  
''' - Superior performance in small-sample studies via information borrowing .  
''' - Seamless integration with Bioconductor ecosystem (e.g., edgeR, Glimma) .  
''' - **Limitations**:  
''' - Sensitive to normalization methods.  
''' - Requires biological replicates for stable variance estimation .  
'''
'''
''' ### **5. Practical Applications**  
''' 
''' - Identifies disease biomarkers (e.g., schizophrenia, Parkinson’s) from blood or tissue transcriptomes .  
''' - Validated in studies integrating RNA-seq with clinical data or multi-omics approaches .  
'''
''' ---
'''
''' ### **Example R Code Snippet**  
''' 
''' ```r
''' library(limma)
''' library(edgeR)
'''
''' # Step 1: Preprocessing with voom
''' dge &lt;- DGEList(counts = count_matrix)
''' dge &lt;- calcNormFactors(dge, method = "TMM")
''' v &lt;- voom(dge, design = design_matrix, plot = TRUE)  # Converts counts + weights
'''
''' # Step 2: Fit linear model
''' fit &lt;- lmFit(v, design_matrix)
''' fit &lt;- eBayes(fit)
'''
''' # Step 3: Extract DE genes (e.g., treatment vs. control)
''' de_genes &lt;- topTable(fit, coef = 2, adjust = "BH", number = Inf, sort.by = "P")
''' ```
''' </summary>
Public Module Limma

    <Extension>
    Private Sub ValidateSampleIDsHelper(design As DataAnalysis, control As Integer(), treat As Integer())
        Dim control_missing As New List(Of String)
        Dim treat_missing As New List(Of String)
        Dim err As String = ""

        If control.Any(Function(ordinal) ordinal = -1) Then
            For i As Integer = 0 To control.Length - 1
                If control(i) < 0 Then
                    control_missing.Add(design.control(i))
                End If
            Next

            err = $"missing control samples: " & control_missing.JoinBy(", ")
        End If
        If treat.Any(Function(ordinal) ordinal = -1) Then
            For i As Integer = 0 To treat.Length - 1
                If treat(i) < 0 Then
                    treat_missing.Add(design.experiment(i))
                End If
            Next

            If err.Length > 0 Then
                err &= "; "
            End If

            err &= $"missing treatment samples: " & treat_missing.JoinBy(", ")
        End If

        If err.Length > 0 Then
            Throw New MissingMemberException($"Missing sample id in expression matrix input: {err}")
        End If
    End Sub

    <Extension>
    Public Function LmFit(x As Matrix, design As DataAnalysis) As IEnumerable(Of LimmaTable)
        Return x.LmFitInternal(design).OrderBy(Function(i) i.adj_P_Val)
    End Function

    ''' <summary>
    ''' limma 的 <c>lmFit</c> + <c>eBayes</c>（两组比较）。
    ''' </summary>
    ''' <param name="x">表达矩阵（基因在行，样本在列）</param>
    ''' <param name="design">
    ''' 两组比较的设计：<see cref="DataAnalysis.experiment"/> 为处理组，
    ''' <see cref="DataAnalysis.control"/> 为对照组，<c>logFC = 处理组 − 对照组</c>
    ''' </param>
    ''' <remarks>
    ''' <para>
    ''' 计算流程与 R 端 <c>lmFit</c> + <c>contrasts.fit</c> + <c>eBayes</c> 一一对应：
    ''' </para>
    ''' <list type="number">
    ''' <item>表达值先做 log2 变换（与 limma 的 <c>Amean</c> 相同，都是在**变换后**的矩阵上取行均值）；</item>
    ''' <item><c>logFC = mean(treat) − mean(control)</c>（两组均值差的 OLS 估计）；</item>
    ''' <item>残差方差 <c>s² = (SSE_control + SSE_treat) / (n − 2)</c>，自由度取设计矩阵的秩 2；</item>
    ''' <item>经验贝叶斯收缩：<see cref="LimmaStatistics.SqueezeVar"/>（limma 的 <c>fitFDist</c> + <c>squeezeVar</c>）；</item>
    ''' <item>
    ''' 标准误必须是**对比**的标准误：<c>se = sqrt(s²_post × (1/n₁ + 1/n₂))</c>。
    ''' 旧实现用的是 <c>sd / sqrt(n)</c>，在 n₁ = n₂ 时把标准误低估了一半，
    ''' 于是 |t| 被高估一倍、p 值系统性偏小 —— 这是本次修复的核心数学错误；
    ''' </item>
    ''' <item>p 值用正则化不完全贝塔函数精确计算（见 <see cref="LimmaStatistics.TwoSidedPvalue"/>）；</item>
    ''' <item><c>B</c>（lods）按 limma 的 <c>tmixture</c> + 后验几率公式计算；</item>
    ''' <item><c>adj.P.Val</c> 使用带单调化的 BH 校正（等价于 R 的 <c>p.adjust(..., "BH")</c>）。</item>
    ''' </list>
    ''' </remarks>
    <Extension>
    Private Function LmFitInternal(x As Matrix, design As DataAnalysis) As LimmaTable()
        ' extract of the sample ordinals
        Dim control As Integer() = x.IndexOf(design.control)   ' 0
        Dim treat As Integer() = x.IndexOf(design.experiment)  ' 1

        design.ValidateSampleIDsHelper(control, treat)

        ' transform the expression data to log2 scale
        Dim logX As Matrix = x.log(2)
        Dim nControl As Integer = control.Length
        Dim nTreat As Integer = treat.Length
        Dim n As Integer = nControl + nTreat
        ' 两组比较的设计矩阵秩为 2
        Dim dfResidual As Double = n - 2
        ' 对比系数为 +1/-1 时，未缩放的标准误是 sqrt(1/n1 + 1/n2)
        Dim seUnscaled As Double = std.Sqrt(1.0 / nControl + 1.0 / nTreat)

        Dim nGene As Integer = logX.size
        Dim logFC As Double() = New Double(nGene - 1) {}
        Dim aveExpr As Double() = New Double(nGene - 1) {}
        Dim geneVars As Double() = New Double(nGene - 1) {}

        For i As Integer = 0 To nGene - 1
            Dim row As Double() = logX.expression(i).experiments
            Dim yc As Double() = control.Select(Function(c) row(c)).ToArray
            Dim yt As Double() = treat.Select(Function(c) row(c)).ToArray
            Dim meanC As Double = yc.Average
            Dim meanT As Double = yt.Average

            logFC(i) = meanT - meanC
            ' limma 的 AveExpr(Amean) 是在**全部样本**的 log 值上取平均
            aveExpr(i) = row.Average
            geneVars(i) = (sumOfSquares(yc, meanC) + sumOfSquares(yt, meanT)) / dfResidual
        Next

        ' 经验贝叶斯方差收缩（limma: fitFDist -> squeezeVar）
        Dim prior = LimmaStatistics.SqueezeVar(geneVars, dfResidual)
        Dim dfPooled As Double = dfResidual * nGene
        Dim dfTotal As Double = std.Min(dfResidual + prior.dfPrior, dfPooled)
        Dim tstat As Double() = New Double(nGene - 1) {}
        Dim pvalue As Double() = New Double(nGene - 1) {}

        For i As Integer = 0 To nGene - 1
            Dim s2post As Double

            If Double.IsInfinity(prior.dfPrior) Then
                s2post = prior.s2Prior
            Else
                s2post = (dfResidual * geneVars(i) + prior.dfPrior * prior.s2Prior) / (dfResidual + prior.dfPrior)
            End If

            Dim se As Double = std.Sqrt(s2post) * seUnscaled

            If se > 0 Then
                tstat(i) = logFC(i) / se
            ElseIf logFC(i) > 0 Then
                tstat(i) = Double.PositiveInfinity
            ElseIf logFC(i) < 0 Then
                tstat(i) = Double.NegativeInfinity
            Else
                tstat(i) = 0
            End If

            pvalue(i) = LimmaStatistics.TwoSidedPvalue(tstat(i), dfTotal)
        Next

        ' B 统计量（limma 的 lods）与 BH 校正
        Dim lods As Double() = LimmaStatistics.Lods(tstat, dfTotal, seUnscaled, prior.s2Prior, prior.dfPrior)
        Dim adjP As Double() = LimmaStatistics.BHAdjust(pvalue)

        Dim tables As LimmaTable() = New LimmaTable(nGene - 1) {}

        For i As Integer = 0 To nGene - 1
            tables(i) = New LimmaTable With {
                .logFC = logFC(i),
                .id = logX.expression(i).geneID,
                .P_Value = pvalue(i),
                .adj_P_Val = adjP(i),
                .t = tstat(i),
                .AveExpr = aveExpr(i),
                .B = lods(i)
            }

            With tables(i)
                .[class] = If(.P_Value < 0.05, If(.logFC > 0, "up", "down"), "not sig")
            End With
        Next

        Return tables
    End Function

    ''' <summary>
    ''' 离差平方和 Σ(x − mean)²
    ''' </summary>
    Private Function sumOfSquares(x As Double(), mean As Double) As Double
        Dim sum As Double = 0

        For i As Integer = 0 To x.Length - 1
            sum += (x(i) - mean) ^ 2
        Next

        Return sum
    End Function
End Module
