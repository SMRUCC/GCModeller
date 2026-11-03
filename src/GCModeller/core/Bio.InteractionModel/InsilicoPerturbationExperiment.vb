Imports SMRUCC.genomics.BioSystem.Intervention

Public Interface InsilicoPerturbationExperiment

    ''' <summary>
    ''' set the context alignment mode, which is used to align the context status of the perturbation experiment with the training data space.
    ''' </summary>
    ''' <param name="mode"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 在当前的虚拟扰动容器中已经默认设置了最佳的上下文对齐模式，通过这个函数可以将默认的最佳对齐模式修改为指定的对齐模式。
    ''' </remarks>
    Function SetContextAlignmentMode(mode As ContextAlignmentMode) As InsilicoPerturbationExperiment

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="context">
    ''' a tuple of (geneName, expressionValue) pairs, which is used to set the context status of the perturbation experiment.
    ''' </param>
    ''' <returns></returns>
    Function SetContext(context As Dictionary(Of String, Double)) As InsilicoPerturbationExperiment

    Function KnockoutGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult
    Function OverexpressGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult
    Function KnockDownGene(geneName As String, Optional nSamples As Integer = 0) As InterventionResult
End Interface

Public Enum ContextAlignmentMode
    ''' <summary>
    ''' 原值直灌（同管线、已配对）
    ''' </summary>
    None
    ''' <summary>
    ''' 逐基因 (x-μ)/σ（线性/高斯/幂律模型）
    ''' </summary>
    Affine
    ''' <summary>
    ''' log2(max(x,0)+pseudo)+δ（PLAS.NET 口径）
    ''' </summary>
    LogAffine
    ''' <summary>
    ''' 秩映射（树模型，现 GeneTree 实现）
    ''' </summary>
    Quantile
    ''' <summary>
    ''' 保倍数的分位偏移
    ''' </summary>
    QuantileDelta
End Enum
