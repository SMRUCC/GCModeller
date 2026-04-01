Namespace MetabolicThermodynamics

    ''' <summary>
    ''' 参与反应的代谢物热力学信息
    ''' </summary>
    Public Class MetaboliteThermoData
        ''' <summary>代谢物名称或ID (如 "atp_c")</summary>
        Public Property Id As String

        ''' <summary>化学计量数 (产物为正，反应物为负。如 ATP->ADP，ATP填-1，ADP填1)</summary>
        Public Property Stoichiometry As Double

        ''' <summary>标准生成吉布斯自由能 ΔGf'0 (单位: kJ/mol，需确保是特定pH和离子强度下的校正值)</summary>
        Public Property DeltaGf0 As Double

        ''' <summary>实际胞内浓度 (单位: M，摩尔/升。如 1mM 则填 0.001)</summary>
        Public Property Concentration As Double
    End Class

End Namespace