#Region "Microsoft.VisualBasic::6ff247b31bfd25942f2319ce45999372, engine\Dynamics\Core\Kinetics\EnvironmentState.vb"

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

    '   Total Lines: 41
    '    Code Lines: 25 (60.98%)
    ' Comment Lines: 10 (24.39%)
    '    - Xml Docs: 60.00%
    ' 
    '   Blank Lines: 6 (14.63%)
    '     File Size: 1.93 KB


    '     Class EnvironmentState
    ' 
    '         Properties: CofactorConc, CrowdingFactor, IonicStrength, OsmoticPressure, pH
    '                     pHHistory, ProductConc, RedoxPotential, SubstrateConc, Temperature
    '                     TemperatureHistory, Viscosity
    ' 
    '         Function: Validate
    ' 
    ' 
    ' /********************************************************************************/

#End Region


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
