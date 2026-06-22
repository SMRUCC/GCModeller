''' <summary>
''' 测量模型载荷与权重表
''' 
''' 本表格展示了PLS-PM测量模型（外模型）中显变量（观测指标）与其所属潜变量之间的从属关系及载荷权重。通过该表格，可以了解到具体的生物学测量指标（如具体的理化指标、基因模块等）是如何聚合映射为高维潜变量的，以及这些显变量对潜变量构建的相对贡献大小。
''' </summary>
Public Class MeasurementModel

    ''' <summary>
    ''' latentName表示所属潜变量名称
    ''' </summary>
    ''' <returns></returns>
    Public Property latentName As String
    ''' <summary>
    ''' mode表示测量模式（A表示反映型模式，即显变量由潜变量反映产生）
    ''' </summary>
    ''' <returns></returns>
    Public Property mode As MeasurementModels
    ''' <summary>
    ''' manifest_variable表示实际观测的显变量名称
    ''' </summary>
    ''' <returns></returns>
    Public Property manifest_variable As String
    ''' <summary>
    ''' loading表示载荷系数，反映显变量与潜变量之间的相关程度
    ''' </summary>
    ''' <returns></returns>
    Public Property loading As Double
    ''' <summary>
    ''' w表示外模型估计中的权重系数
    ''' </summary>
    ''' <returns></returns>
    Public Property w As Double
    ''' <summary>
    ''' communality表示该显变量的共同度
    ''' </summary>
    ''' <returns></returns>
    Public Property communality As Double
    ''' <summary>
    ''' block_communality表示该潜变量模块下所有显变量的平均共同度。
    ''' </summary>
    ''' <returns></returns>
    Public Property block_communality As Double

    Public Shared Iterator Function FromResult(result As PLSPMResult) As IEnumerable(Of MeasurementModel)
        For j = 0 To result.NumLatents - 1
            Dim lv = result.LatentDefs(j)
            Dim block = result.Communalities(j)

            For k = 0 To lv.featureIDs.Length - 1
                Dim mvName = lv.featureIDs(k)
                Dim load = result.Loadings(j)(k)
                Dim w = result.FinalOuterWeights(j)(k)
                Dim comm = load * load

                Yield New MeasurementModel With {
                    .block_communality = block,
                    .communality = comm,
                    .latentName = lv.varName,
                    .mode = lv.mode,
                    .loading = load,
                    .manifest_variable = mvName,
                    .w = w
                }
            Next
        Next
    End Function

End Class

''' <summary>
''' 内生潜变量模型拟合优度表
''' 
''' 本表格展示了PLS-PM结构模型中所有内生潜变量（即被其他潜变量预测的变量）的模型拟合优度与解释力度指标。通过该表格，可以了解到上游组学变量对下游靶变量（如各类黄酮物质、淀粉表型等）变异的整体解释能力，以及测量模型中显变量对潜变量的代表程度，用于评估整体网络模型的可靠性与解释力。
''' </summary>
Public Class EndogenousLatentVariable

    ''' <summary>
    ''' latentName表示内生潜变量的名称
    ''' </summary>
    ''' <returns></returns>
    Public Property latentName As String
    ''' <summary>
    ''' r2表示决定系数，代表该潜变量的方差能被其上游预测变量解释的比例
    ''' </summary>
    ''' <returns></returns>
    Public Property r2 As Double
    ''' <summary>
    ''' communality表示共同度，代表该潜变量的显变量能解释该潜变量的方差比例（测量模型的效度）
    ''' </summary>
    ''' <returns></returns>
    Public Property communality As Double
    ''' <summary>
    ''' redundancy表示冗余度，代表显变量能被上游潜变量解释的方差比例。
    ''' </summary>
    ''' <returns></returns>
    Public Property redundancy As Double

    Public Overrides Function ToString() As String
        Return $"{latentName,-25}{r2,12:F4}{communality,12:F4}{redundancy,12:F4}"
    End Function

    Public Shared Iterator Function FromResult(result As PLSPMResult) As IEnumerable(Of EndogenousLatentVariable)
        For j = 0 To result.NumLatents - 1
            Dim r2 = If(result.RSquared.ContainsKey(j), result.RSquared(j), 0.0)
            Dim comm = result.Communalities(j)
            Dim red = result.Redundancies(j)

            Yield New EndogenousLatentVariable With {
                .latentName = result.LatentNames(j),
                .r2 = r2,
                .communality = comm,
                .redundancy = red
            }
        Next
    End Function

End Class