Namespace Kinetics

    ''' <summary>
    ''' 酶的物理化学特征参数
    ''' </summary>
    Public Class EnzymeCharacteristics
        ' 温度相关参数
        Public Property T_ref As Double = 298.15           ' 参考温度 (K)
        Public Property T_opt As Double = 310.15           ' 最适温度 (K)
        Public Property Ea As Double = 50000.0             ' kcat 活化能 (J/mol)
        Public Property DeltaH_bind As Double = -30000.0   ' Km 结合焓 (J/mol)
        Public Property T_melt As Double = 330.0           ' 酶熔化温度 (K)
        Public Property k_denat As Double = 10.0           ' 失活陡峭度系数

        ' pH 相关参数
        Public Property pH_opt As Double = 7.0             ' 最适 pH
        Public Property pKa1 As Double = 6.5               ' 催化残基质子解离常数
        Public Property pKa2 As Double = 8.5               ' 催化残基去质子解离常数
        Public Property pH_width As Double = 1.5           ' pH活性曲线宽度

        ' 离子强度参数
        Public Property I_opt As Double = 0.15             ' 最适离子强度 (M)
        Public Property k_ion As Double = 2.0              ' 离子强度影响系数
        Public Property z_enzyme As Double = -2.0          ' 酶净电荷数
        Public Property z_substrate As Double = -1.0       ' 底物净电荷数

        ' 抑制与激活参数
        Public Property Ki_sub As Double = 0.01            ' 底物抑制常数 (M)
        Public Property Ki_prod_comp As Double = 0.005     ' 产物竞争抑制常数 (M)
        Public Property Ki_prod_uncomp As Double = 0.005   ' 产物非竞争抑制常数 (M)
        ''' <summary>
        ''' 是否需要辅因子
        ''' </summary>
        ''' <returns></returns>
        Public Property RequiresCofactor As Boolean = False
        Public Property Ka_cofactor As Double = 0.001      ' 辅因子激活常数 (M)
        Public Property n_cofactor As Double = 1.0         ' 辅因子结合协同性 (Hill系数)

        ' 环境依赖度参数 
        Public Property crowding_sensitivity As Double = 0.2 ' 拥挤敏感度 (0~1)
        Public Property viscosity_sensitivity As Double = 0.1 ' 粘度敏感度 (0~1), 1为纯扩散控制

        ' 交互作用参数
        Public Property alpha_temp_pH As Double = 0.1      ' 温度-pH交互作用系数
        Public Property beta_ion_pH As Double = 0.05       ' 离子强度-pH交互作用系数

        ' 动态效应参数
        Public Property tau_temp As Double = 60.0          ' 温度适应时间常数 (秒)
        Public Property tau_pH As Double = 30.0            ' pH适应时间常数 (秒)

        ''' <summary>
        ''' 验证酶特征参数的有效性
        ''' </summary>
        Public Function Validate() As Boolean
            If T_ref <= 0 Or T_opt <= 0 Or T_melt <= 0 Then Return False
            If Ea <= 0 Or Ea > 200000 Then Return False ' 合理活化能范围
            If pKa1 >= pKa2 Then Return False
            If Ki_sub <= 0 Or Ki_prod_comp <= 0 Or Ki_prod_uncomp <= 0 Then Return False
            If RequiresCofactor AndAlso Ka_cofactor <= 0 Then Return False
            Return True
        End Function
    End Class
End Namespace