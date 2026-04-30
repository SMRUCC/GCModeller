
Namespace Kinetics

    ''' <summary>
    ''' 环境状态向量 X - 包含所有影响酶活性的环境因素
    ''' </summary>
    Public Class EnvironmentState
        ' 基本物理化学参数
        Public Property Temperature As Double = 310.15     ' 当前温度 (K)
        Public Property pH As Double = 7.4                 ' 当前 pH
        Public Property IonicStrength As Double = 0.15     ' 离子强度 (M)
        Public Property RedoxPotential As Double = 0.0     ' 氧化还原电位 (mV)，可选

        ' 底物与产物浓度
        Public Property SubstrateConc As Double = 0.001    ' 底物浓度 (M)
        Public Property ProductConc As Double = 0.0001     ' 产物浓度 (M)
        Public Property CofactorConc As Double = 0.002     ' 辅因子/激活剂浓度 (M)

        ' 细胞环境因素
        Public Property CrowdingFactor As Double = 1.0     ' 大分子拥挤因子 (1.0=无拥挤)
        Public Property OsmoticPressure As Double = 0.3    ' 渗透压 (MPa)，可选
        Public Property Viscosity As Double = 1.0          ' 粘度因子 (相对水)

        ' 时间序列数据（用于动态效应分析）
        Public Property TemperatureHistory As List(Of Double) = Nothing
        Public Property pHHistory As List(Of Double) = Nothing

        ''' <summary>
        ''' 验证环境参数的有效性
        ''' </summary>
        Public Function Validate() As Boolean
            If Temperature <= 0 Or Temperature > 373 Then Return False ' 0-100°C范围
            If pH < 0 Or pH > 14 Then Return False
            If IonicStrength < 0 Then Return False
            If SubstrateConc < 0 Or ProductConc < 0 Or CofactorConc < 0 Then Return False
            If CrowdingFactor <= 0 Then Return False
            If Viscosity <= 0 Then Return False
            Return True
        End Function
    End Class
End Namespace