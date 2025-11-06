Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Statistics
Imports Microsoft.VisualBasic.Math.Statistics.Linq
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

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
    Public Iterator Function LmFit(x As Matrix, design As DataAnalysis) As IEnumerable(Of DEGModel)
        Dim control As Integer() = x.IndexOf(design.control)  ' 0
        Dim treat As Integer() = x.IndexOf(design.experiment) ' 1
        Dim xi As Double() = Replicate(0.0, control.Length) _
            .JoinIterates(Replicate(1.0, treat.Length)) _
            .ToArray

        ' transform the expression data to log2 scale
        x = x.log(2)
        design.ValidateSampleIDsHelper(control, treat)

        Dim logFC As Double() = New Double(x.size - 1) {}
        Dim residuals As Double()() = RectangularArray.Matrix(Of Double)(x.size, control.Length + treat.Length)
        Dim i As Integer = 0

        ' 拟合线性模型（逐基因）
        For Each gene As DataFrameRow In x.expression
            Dim y As Double() = gene(control).JoinIterates(gene(treat)).ToArray
            Dim lm As FitResult = LeastSquares.LinearFit(xi, y)

            logFC(i) = lm.Slope
            residuals(i) = lm.Residuals

            i += 1
        Next

        ' 经验贝叶斯方差调整
        ' a vs b
        Dim p As Integer = design.size
        Dim df_residual = (control.Length + treat.Length) - p
        Dim gene_vars As Double() = residuals _
            .Select(Function(e) (New Vector(e) ^ 2).Sum / df_residual) _
            .ToArray

        Dim prior_var As Double = gene_vars.Median
        Dim prior_df = 4  ' limma默认先验自由度
        Dim shrunk_vars = (prior_df * prior_var + df_residual * New Vector(gene_vars)) / (prior_df + df_residual)
        Dim shrunk_se = Vector.Sqrt(shrunk_vars)

        ' t检验与p值
        Dim t_stats = logFC / shrunk_se
        Dim df_total = prior_df + df_residual
        Dim t As Vector = t_stats.Abs
        Dim p_values = Hypothesis.t.Pvalue(t, df:=df_total, Hypothesis.Hypothesis.TwoSided)

        For offset As Integer = 0 To logFC.Length - 1
            Yield New DEGModel With {
                .logFC = logFC(offset),
                .label = x(offset).geneID,
                .pvalue = p_values(offset),
                .[class] = If(.pvalue < 0.05, If(.logFC > 0, "up", "down"), "not sig"),
                .t = t(offset)
            }
        Next
    End Function
End Module
