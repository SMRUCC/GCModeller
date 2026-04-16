#Region "Microsoft.VisualBasic::4922e3e843d3ad6650337bd551176180, engine\Model\MetabolicThermodynamics\ThermoFluxCalculator.vb"

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

    '   Total Lines: 81
    '    Code Lines: 45 (55.56%)
    ' Comment Lines: 24 (29.63%)
    '    - Xml Docs: 41.67%
    ' 
    '   Blank Lines: 12 (14.81%)
    '     File Size: 3.63 KB


    '     Class ThermoFluxCalculator
    ' 
    '         Function: CalculateBounds
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace MetabolicThermodynamics

    ''' <summary>
    ''' 热力学通量边界计算器
    ''' </summary>
    Public Class ThermoFluxCalculator
        ' 理想气体常数 R = 8.314 J/(mol·K)
        Private Const R_JOULES As Double = 8.314

        ' FBA中的常规默认最大通量 (单位通常为 mmol/gDW/h)
        Private Const DEFAULT_MAX_FLUX As Double = 1000.0

        ''' <summary>
        ''' 计算单个反应的热力学约束边界
        ''' </summary>
        ''' <param name="metabolites">该反应包含的所有代谢物列表</param>
        ''' <param name="temperatureK">绝对温度 (开尔文，如298.15)</param>
        ''' <param name="threshold">热力学阈值，防止刚好为0时的数值不稳定，通常设为 1.0 kJ/mol</param>
        ''' <returns>包含计算结果和FBA边界的对象</returns>
        Public Function CalculateBounds(metabolites As List(Of MetaboliteThermoData),
                                        temperatureK As Double,
                                        Optional threshold As Double = 1.0) As ThermoBoundResult

            If metabolites Is Nothing OrElse metabolites.Count = 0 Then
                Throw New ArgumentException("代谢物列表不能为空")
            End If

            ' --- 步骤 1: 计算 ΔG'0 ---
            Dim deltaG0 As Double = 0
            For Each met In metabolites
                deltaG0 += met.Stoichiometry * met.DeltaGf0
            Next

            ' --- 步骤 2: 计算反应商 Q 的自然对数 ln(Q) ---
            ' 公式: ln(Q) = Σ (ν_i * ln([M_i]))
            Dim logQ As Double = 0
            For Each met In metabolites
                Dim conc As Double = met.Concentration

                ' 处理极端情况：如果浓度未测量或为0，赋予极小值(1e-9 M)避免对数报错
                ' 在热力学上，极低浓度会驱动反应向生成该物质的方向进行
                If conc <= 0 Then
                    conc = 0.000000001
                End If

                logQ += met.Stoichiometry * Math.Log(conc)
            Next

            ' --- 步骤 3: 计算实际 ΔG' ---
            ' 注意单位换算：R 的单位是 J/(mol·K)，算出来的结果是 J，需要除以1000转为 kJ/mol
            Dim deltaGActual As Double = deltaG0 + (R_JOULES * temperatureK * logQ) / 1000.0

            ' --- 步骤 4: 根据 ΔG' 判定方向并分配 FBA 边界 ---
            Dim lb As Double = -DEFAULT_MAX_FLUX
            Dim ub As Double = DEFAULT_MAX_FLUX
            Dim direction As String = "Reversible (接近平衡态)"

            If deltaGActual <= -threshold Then
                ' ΔG' < 0，正向自发（不可逆正向）
                lb = 0
                ub = DEFAULT_MAX_FLUX
                direction = "Irreversible Forward (严格正向)"
            ElseIf deltaGActual >= threshold Then
                ' ΔG' > 0，逆向自发（不可逆反向）
                lb = -DEFAULT_MAX_FLUX
                ub = 0
                direction = "Irreversible Reverse (严格反向)"
            End If
            ' 如果在阈值之间，保持默认的 -1000 到 1000 (可逆)

            ' --- 返回结果 ---
            Return New ThermoBoundResult With {
                .DeltaG0 = Math.Round(deltaG0, 2),
                .DeltaGActual = Math.Round(deltaGActual, 2),
                .Direction = direction,
                .LowerBound = lb,
                .UpperBound = ub
            }
        End Function
    End Class
End Namespace
