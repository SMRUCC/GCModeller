Namespace Simulation.ExpressionRegulationNetwork

    Public Class Configs

        Public Interface I_Configurable
            Function SetConfigs(conf As Configs) As Integer
        End Interface

        Public Property Regulator_Decays As Double
        Public Property Enzyme_Decays As Double

        ''' <summary>
        ''' 这个参数值调整调控事件的发生概率阈值的高低，则阈值越低，即调控事件越容易发生
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SiteSpecificDynamicsRegulations As Double
        ''' <summary>
        ''' 本底表达水平，数值越高，则表达量越高
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property BasalThreshold As Double

        ''' <summary>
        ''' 没有在模型之中找到代谢物的合成的代谢途径，则可能为第二信使或者其他未知的原因，则在模型之中以很低的概率产生调控效应，这个参数配置产生火星的概率的高低
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property OCS_NONE_Effector As Double

        ''' <summary>
        ''' 默认的调控值
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property OCS_Default_EffectValue As Double

    End Class
End Namespace