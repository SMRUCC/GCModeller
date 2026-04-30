Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Kinetics.AdvancedEnzymeKineticsCorrection

Namespace Kinetics

    ''' <summary>
    ''' ============== 辅助工具函数 ==============
    ''' </summary>
    Public Class KineticsTest

        ''' <summary>
        ''' 参数敏感性分析
        ''' </summary>
        Public Shared Function SensitivityAnalysis(baseValue As Double, env As EnvironmentState, enzyme As EnzymeCharacteristics, paramName As String, variation As Double) As Dictionary(Of String, Double)
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