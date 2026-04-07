#Region "Microsoft.VisualBasic::e3c2f379f22b296ced70963fc79da83c, engine\Model\MetabolicThermodynamics\MetaboliteThermoData.vb"

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

    '   Total Lines: 20
    '    Code Lines: 8 (40.00%)
    ' Comment Lines: 7 (35.00%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 5 (25.00%)
    '     File Size: 803 B


    '     Class MetaboliteThermoData
    ' 
    '         Properties: Concentration, DeltaGf0, Id, Stoichiometry
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
