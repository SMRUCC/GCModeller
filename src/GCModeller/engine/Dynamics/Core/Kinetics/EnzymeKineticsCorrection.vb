Namespace Kinetics

    ''' <summary>
    ''' Km = Km[0] + E(X)
    ''' 
    ''' Km[0]为数据库中记录的常数值
    ''' E(X)为基于当前溶液环境条件下的修正项
    ''' </summary>
    ''' <remarks>
    ''' E(X) = a × (F_total - 1)
    ''' 
    ''' 其中 F_total = F_temp × F_pH × F_ion × F_sub × F_prod
    ''' 
    ''' 各分项函数如下：
    ''' 
    ''' 温度因子（Arrhenius型）：F_temp = exp[(Ea/R) × (1/T_ref - 1/T)]
    ''' pH因子（钟形曲线）：F_pH = exp[-(pH - pH_opt)² / (2 × σ_pH²)]
    ''' 离子强度因子：F_ion = exp[-k_ion × |I - I_opt|]
    ''' 底物抑制因子：F_sub = (Km_ref + [S]) / (Km_ref + [S] + [S]²/Ki_sub)
    ''' 产物抑制因子：F_prod = (Km_ref + [S]) / [Km_ref × (1 + [P]/Ki_prod) + [S]]
    ''' </remarks>
    Public Class AdvancedEnzymeKineticsCorrection
        ' 物理常数
        Private Const R As Double = 8.31446261815324      ' 气体常数 J/(mol·K)
        Private Const F As Double = 96485.33212           ' 法拉第常数 C/mol

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
                Return True
            End Function
        End Class

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
            Public Property Ka_cofactor As Double = 0.001      ' 辅因子激活常数 (M)
            Public Property n_cofactor As Double = 1.0         ' 辅因子结合协同性 (Hill系数)

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
                If Ka_cofactor <= 0 Then Return False
                Return True
            End Function
        End Class

        ''' <summary>
        ''' 计算修正后的 Km 值
        ''' Km_corrected = Km_ref × F_total
        ''' </summary>
        Public Shared Function CalculateCorrectedKm(
        Km_ref As Double,
        env As EnvironmentState,
        enzyme As EnzymeCharacteristics
    ) As Double

            ' 参数验证
            If Not env.Validate() Or Not enzyme.Validate() Then
                Throw New ArgumentException("环境参数或酶特征参数无效")
            End If

            If Km_ref <= 0 Then
                Throw New ArgumentException("Km_ref必须大于0")
            End If

            ' 1. 温度因子 (基于结合焓)
            Dim F_temp As Double = CalculateTemperatureFactorKm(env.Temperature, enzyme)

            ' 2. pH因子 (双pKa机理模型)
            Dim F_pH As Double = CalculatepHFactor(env.pH, enzyme)

            ' 3. 离子强度因子 (考虑电荷效应)
            Dim F_ion As Double = CalculateIonStrengthFactor(env.IonicStrength, enzyme)

            ' 4. 底物抑制因子
            Dim F_sub_inh As Double = CalculateSubstrateInhibitionFactor(env.SubstrateConc, Km_ref, enzyme)

            ' 5. 产物竞争抑制因子
            Dim F_prod_comp As Double = CalculateProductCompetitiveInhibitionFactor(env.ProductConc, enzyme)

            ' 6. 拥挤效应
            Dim F_crowding As Double = CalculateCrowdingEffect(env.CrowdingFactor)

            ' 7. 温度-pH交互作用
            Dim F_temp_pH_interaction As Double = CalculateTemperaturepHInteraction(
            env.Temperature, env.pH, enzyme)

            ' 8. 动态适应效应
            Dim F_dynamic As Double = CalculateDynamicAdaptationEffect(env, enzyme)

            ' 合成总修正因子
            Dim F_total As Double = F_temp * F_pH * F_ion * F_sub_inh *
                               F_prod_comp * F_crowding * F_temp_pH_interaction * F_dynamic

            ' 边界检查
            F_total = Math.Max(0.01, Math.Min(100.0, F_total))

            Return Km_ref * F_total
        End Function

        ''' <summary>
        ''' 计算修正后的 kcat 值
        ''' kcat_corrected = kcat_ref × F_total
        ''' </summary>
        Public Shared Function CalculateCorrectedKcat(
        kcat_ref As Double,
        env As EnvironmentState,
        enzyme As EnzymeCharacteristics
    ) As Double

            ' 参数验证
            If Not env.Validate() Or Not enzyme.Validate() Then
                Throw New ArgumentException("环境参数或酶特征参数无效")
            End If

            If kcat_ref <= 0 Then
                Throw New ArgumentException("kcat_ref必须大于0")
            End If

            ' 1. 温度因子 (Arrhenius方程)
            Dim F_temp As Double = CalculateTemperatureFactorKcat(env.Temperature, enzyme)

            ' 2. 高温失活因子
            Dim F_denat As Double = CalculateThermalDenaturationFactor(env.Temperature, enzyme)

            ' 3. pH因子
            Dim F_pH As Double = CalculatepHFactor(env.pH, enzyme)

            ' 4. 离子强度因子
            Dim F_ion As Double = CalculateIonStrengthFactor(env.IonicStrength, enzyme)

            ' 5. 产物非竞争抑制因子
            Dim F_prod_uncomp As Double = CalculateProductNoncompetitiveInhibitionFactor(env.ProductConc, enzyme)

            ' 6. 辅因子激活 (Hill方程)
            Dim F_cofactor As Double = CalculateCofactorActivationFactor(env.CofactorConc, enzyme)

            ' 7. 粘度效应
            Dim F_viscosity As Double = CalculateViscosityEffect(env.Viscosity)

            ' 8. 温度-pH交互作用
            Dim F_temp_pH_interaction As Double = CalculateTemperaturepHInteraction(
            env.Temperature, env.pH, enzyme)

            ' 合成总修正因子
            Dim F_total As Double = F_temp * F_denat * F_pH * F_ion *
                               F_prod_uncomp * F_cofactor * F_viscosity * F_temp_pH_interaction

            ' 边界检查
            F_total = Math.Max(0.01, Math.Min(100.0, F_total))

            Return kcat_ref * F_total
        End Function

        ' ============== 各修正因子的具体实现 ==============

        ''' <summary>
        ''' 温度因子 (Km) - 基于结合焓
        ''' </summary>
        Private Shared Function CalculateTemperatureFactorKm(T As Double, enzyme As EnzymeCharacteristics) As Double
            ' Van't Hoff方程: d(lnK)/dT = ΔH/(RT²)
            ' 积分得: K(T)/K(T_ref) = exp[-ΔH/R * (1/T - 1/T_ref)]
            Dim exponent As Double = -enzyme.DeltaH_bind / R * (1.0 / T - 1.0 / enzyme.T_ref)
            Return SafeExp(exponent)
        End Function

        ''' <summary>
        ''' 温度因子 (kcat) - Arrhenius方程
        ''' </summary>
        Private Shared Function CalculateTemperatureFactorKcat(T As Double, enzyme As EnzymeCharacteristics) As Double
            ' Arrhenius方程: k = A * exp(-Ea/RT)
            ' k(T)/k(T_ref) = exp[Ea/R * (1/T_ref - 1/T)]
            Dim exponent As Double = enzyme.Ea / R * (1.0 / enzyme.T_ref - 1.0 / T)
            Return SafeExp(exponent)
        End Function

        ''' <summary>
        ''' 高温失活因子 - Sigmoid函数
        ''' </summary>
        Private Shared Function CalculateThermalDenaturationFactor(T As Double, enzyme As EnzymeCharacteristics) As Double
            ' Sigmoid函数模拟温度诱导的酶变性
            Dim x As Double = enzyme.k_denat * (T - enzyme.T_melt)
            Return 1.0 / (1.0 + SafeExp(x))
        End Function

        ''' <summary>
        ''' pH因子 - 双pKa机理模型
        ''' </summary>
        Private Shared Function CalculatepHFactor(pH As Double, enzyme As EnzymeCharacteristics) As Double
            ' 基于酶活性位点质子化状态的双pKa模型
            ' 活性形式比例 = 1 / [1 + 10^(pKa1-pH) + 10^(pH-pKa2)]
            Dim term1 As Double = Math.Pow(10, enzyme.pKa1 - pH)
            Dim term2 As Double = Math.Pow(10, pH - enzyme.pKa2)
            Return 1.0 / (1.0 + term1 + term2)
        End Function

        ''' <summary>
        ''' 离子强度因子 - 考虑电荷效应
        ''' </summary>
        Private Shared Function CalculateIonStrengthFactor(I As Double, enzyme As EnzymeCharacteristics) As Double
            ' 方法1: 简单指数衰减
            Dim simple_factor As Double = Math.Exp(-enzyme.k_ion * Math.Abs(I - enzyme.I_opt))

            ' 方法2: 基于德拜-休克尔理论 (可选)
            If Math.Abs(enzyme.z_enzyme) > 0.1 And Math.Abs(enzyme.z_substrate) > 0.1 Then
                Dim sqrtI As Double = Math.Sqrt(I)
                Dim denominator As Double = 1.0 + sqrtI
                Dim charge_effect As Double = Math.Exp(enzyme.z_enzyme * enzyme.z_substrate * sqrtI / denominator)

                ' 结合两种效应
                Return 0.7 * simple_factor + 0.3 * charge_effect
            End If

            Return simple_factor
        End Function

        ''' <summary>
        ''' 底物抑制因子
        ''' </summary>
        Private Shared Function CalculateSubstrateInhibitionFactor(
        S As Double, Km_ref As Double, enzyme As EnzymeCharacteristics
    ) As Double
            If S <= 0 Or enzyme.Ki_sub <= 0 Then Return 1.0

            ' 基于修正的米氏方程: v = Vmax[S]/(Km + [S] + [S]²/Ki)
            ' 表观Km = Km + [S]²/Ki
            Dim apparent_Km As Double = Km_ref + (S * S) / enzyme.Ki_sub
            Return apparent_Km / Km_ref
        End Function

        ''' <summary>
        ''' 产物竞争抑制因子
        ''' </summary>
        Private Shared Function CalculateProductCompetitiveInhibitionFactor(P As Double, enzyme As EnzymeCharacteristics) As Double
            If P <= 0 Or enzyme.Ki_prod_comp <= 0 Then Return 1.0

            ' 竞争性抑制: Km_app = Km * (1 + [P]/Ki)
            Return 1.0 + P / enzyme.Ki_prod_comp
        End Function

        ''' <summary>
        ''' 产物非竞争抑制因子
        ''' </summary>
        Private Shared Function CalculateProductNoncompetitiveInhibitionFactor(P As Double, enzyme As EnzymeCharacteristics) As Double
            If P <= 0 Or enzyme.Ki_prod_uncomp <= 0 Then Return 1.0

            ' 非竞争性抑制: kcat_app = kcat / (1 + [P]/Ki)
            Return 1.0 / (1.0 + P / enzyme.Ki_prod_uncomp)
        End Function

        ''' <summary>
        ''' 辅因子激活因子 (Hill方程)
        ''' </summary>
        Private Shared Function CalculateCofactorActivationFactor(C As Double, enzyme As EnzymeCharacteristics) As Double
            If C <= 0 Or enzyme.Ka_cofactor <= 0 Then Return 0.0

            ' Hill方程: f = [C]^n / (Ka^n + [C]^n)
            Dim Cn As Double = Math.Pow(C, enzyme.n_cofactor)
            Dim Kan As Double = Math.Pow(enzyme.Ka_cofactor, enzyme.n_cofactor)

            Return Cn / (Kan + Cn)
        End Function

        ''' <summary>
        ''' 拥挤效应因子
        ''' </summary>
        Private Shared Function CalculateCrowdingEffect(crowding As Double) As Double
            ' 大分子拥挤通常增加表观亲和力 (降低Km)
            ' 简单模型: 亲和力与可用体积成反比
            Return 1.0 / crowding
        End Function

        ''' <summary>
        ''' 粘度效应因子
        ''' </summary>
        Private Shared Function CalculateViscosityEffect(viscosity As Double) As Double
            ' 扩散控制反应的速率与粘度成反比
            ' Stokes-Einstein关系: D ∝ 1/η
            Return 1.0 / viscosity
        End Function

        ''' <summary>
        ''' 温度-pH交互作用因子
        ''' </summary>
        Private Shared Function CalculateTemperaturepHInteraction(T As Double, pH As Double, enzyme As EnzymeCharacteristics) As Double
            ' 交互作用项: 温度偏离最适温度 × pH偏离最适pH
            Dim temp_dev As Double = Math.Abs(T - enzyme.T_opt) / enzyme.T_opt
            Dim pH_dev As Double = Math.Abs(pH - enzyme.pH_opt) / 7.0 ' 归一化

            Dim interaction As Double = enzyme.alpha_temp_pH * temp_dev * pH_dev
            Return Math.Exp(-interaction)
        End Function

        ''' <summary>
        ''' 动态适应效应因子
        ''' </summary>
        Private Shared Function CalculateDynamicAdaptationEffect(env As EnvironmentState, enzyme As EnzymeCharacteristics) As Double
            ' 如果无历史数据，返回1.0 (无动态效应)
            If env.TemperatureHistory Is Nothing Or env.pHHistory Is Nothing Then
                Return 1.0
            End If

            ' 计算环境变化速率
            Dim temp_rate As Double = CalculateChangeRate(env.TemperatureHistory, env.Temperature)
            Dim pH_rate As Double = CalculateChangeRate(env.pHHistory, env.pH)

            ' 动态适应因子: 快速变化时酶需要时间适应
            Dim adaptation As Double = 1.0

            If enzyme.tau_temp > 0 Then
                adaptation *= 1.0 / (1.0 + enzyme.tau_temp * Math.Abs(temp_rate))
            End If

            If enzyme.tau_pH > 0 Then
                adaptation *= 1.0 / (1.0 + enzyme.tau_pH * Math.Abs(pH_rate))
            End If

            Return adaptation
        End Function

        ''' <summary>
        ''' 计算变化速率
        ''' </summary>
        Private Shared Function CalculateChangeRate(history As List(Of Double), current As Double) As Double
            If history Is Nothing Or history.Count < 2 Then Return 0.0

            ' 简单线性回归计算变化率
            Dim sum_x As Double = 0, sum_y As Double = 0, sum_xy As Double = 0, sum_xx As Double = 0
            Dim n As Integer = Math.Min(history.Count, 10) ' 最近10个点

            For i As Integer = 0 To n - 1
                Dim x As Double = i
                Dim y As Double = history(history.Count - n + i)
                sum_x += x
                sum_y += y
                sum_xy += x * y
                sum_xx += x * x
            Next

            Dim slope As Double = (n * sum_xy - sum_x * sum_y) / (n * sum_xx - sum_x * sum_x)
            Return slope
        End Function

        ''' <summary>
        ''' 安全的指数计算，防止溢出
        ''' </summary>
        Private Shared Function SafeExp(x As Double) As Double
            If x > 100 Then Return Double.MaxValue
            If x < -100 Then Return 0.0
            Return Math.Exp(x)
        End Function

        ' ============== 辅助工具函数 ==============

        ''' <summary>
        ''' 参数敏感性分析
        ''' </summary>
        Public Shared Function SensitivityAnalysis(
        baseValue As Double,
        env As EnvironmentState,
        enzyme As EnzymeCharacteristics,
        paramName As String,
        variation As Double
    ) As Dictionary(Of String, Double)

            Dim results As New Dictionary(Of String, Double)()

            ' 备份原始值
            Dim originalEnv As EnvironmentState = CloneEnvironment(env)
            Dim originalEnzyme As EnzymeCharacteristics = CloneEnzyme(enzyme)

            ' 分析Km敏感性
            Dim baseKm As Double = CalculateCorrectedKm(baseValue, env, enzyme)

            ' 测试各环境参数变化
            Dim testCases As New Dictionary(Of String, Action)()

            ' 温度变化
            testCases("Temperature +10%") = Sub() env.Temperature *= 1.1
            testCases("Temperature -10%") = Sub() env.Temperature *= 0.9

            ' pH变化
            testCases("pH +0.5") = Sub() env.pH += 0.5
            testCases("pH -0.5") = Sub() env.pH -= 0.5

            ' 离子强度变化
            testCases("IonicStrength +50%") = Sub() env.IonicStrength *= 1.5
            testCases("IonicStrength -50%") = Sub() env.IonicStrength *= 0.5

            For Each kvp In testCases
                ' 恢复原始环境
                RestoreEnvironment(env, originalEnv)

                ' 应用变化
                kvp.Value.Invoke()

                ' 计算新值
                Dim newKm As Double = CalculateCorrectedKm(baseValue, env, enzyme)
                Dim sensitivity As Double = (newKm - baseKm) / baseKm * 100.0

                results.Add(kvp.Key, sensitivity)
            Next

            ' 恢复原始环境
            RestoreEnvironment(env, originalEnv)

            Return results
        End Function

        ''' <summary>
        ''' 克隆环境状态
        ''' </summary>
        Private Shared Function CloneEnvironment(env As EnvironmentState) As EnvironmentState
            Return New EnvironmentState With {
            .Temperature = env.Temperature,
            .pH = env.pH,
            .IonicStrength = env.IonicStrength,
            .SubstrateConc = env.SubstrateConc,
            .ProductConc = env.ProductConc,
            .CofactorConc = env.CofactorConc,
            .CrowdingFactor = env.CrowdingFactor,
            .Viscosity = env.Viscosity
        }
        End Function

        ''' <summary>
        ''' 克隆酶特征
        ''' </summary>
        Private Shared Function CloneEnzyme(enzyme As EnzymeCharacteristics) As EnzymeCharacteristics
            Return New EnzymeCharacteristics With {
            .T_ref = enzyme.T_ref,
            .T_opt = enzyme.T_opt,
            .Ea = enzyme.Ea,
            .DeltaH_bind = enzyme.DeltaH_bind,
            .T_melt = enzyme.T_melt,
            .k_denat = enzyme.k_denat,
            .pH_opt = enzyme.pH_opt,
            .pKa1 = enzyme.pKa1,
            .pKa2 = enzyme.pKa2,
            .pH_width = enzyme.pH_width,
            .I_opt = enzyme.I_opt,
            .k_ion = enzyme.k_ion,
            .z_enzyme = enzyme.z_enzyme,
            .z_substrate = enzyme.z_substrate,
            .Ki_sub = enzyme.Ki_sub,
            .Ki_prod_comp = enzyme.Ki_prod_comp,
            .Ki_prod_uncomp = enzyme.Ki_prod_uncomp,
            .Ka_cofactor = enzyme.Ka_cofactor,
            .n_cofactor = enzyme.n_cofactor,
            .alpha_temp_pH = enzyme.alpha_temp_pH,
            .beta_ion_pH = enzyme.beta_ion_pH,
            .tau_temp = enzyme.tau_temp,
            .tau_pH = enzyme.tau_pH
        }
        End Function

        ''' <summary>
        ''' 恢复环境状态
        ''' </summary>
        Private Shared Sub RestoreEnvironment(target As EnvironmentState, source As EnvironmentState)
            target.Temperature = source.Temperature
            target.pH = source.pH
            target.IonicStrength = source.IonicStrength
            target.SubstrateConc = source.SubstrateConc
            target.ProductConc = source.ProductConc
            target.CofactorConc = source.CofactorConc
            target.CrowdingFactor = source.CrowdingFactor
            target.Viscosity = source.Viscosity
        End Sub

        ''' <summary>
        ''' 获取默认酶特征参数
        ''' </summary>
        Public Shared Function GetDefaultEnzymeCharacteristics() As EnzymeCharacteristics
            Return New EnzymeCharacteristics With {
                .T_ref = 298.15,  ' 温度参数
                .T_opt = 310.15,
                .Ea = 50000.0,
                .DeltaH_bind = -30000.0,
                .T_melt = 330.0,
                .k_denat = 10.0,
                .pH_opt = 7.0,  ' pH参数
                .pKa1 = 6.5,
                .pKa2 = 8.5,
                .pH_width = 1.5,
                .I_opt = 0.15,  ' 离子强度参数
                .k_ion = 2.0,
                .z_enzyme = -2.0,
                .z_substrate = -1.0,
                .Ki_sub = 0.01,  ' 抑制与激活参数
                .Ki_prod_comp = 0.005,
                .Ki_prod_uncomp = 0.005,
                .Ka_cofactor = 0.001,
                .n_cofactor = 1.0,
                .alpha_temp_pH = 0.1,    ' 交互作用参数
                .beta_ion_pH = 0.05,
                .tau_temp = 60.0,   ' 动态效应参数
                .tau_pH = 30.0
            }
        End Function
    End Class

End Namespace