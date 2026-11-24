#Region "Microsoft.VisualBasic::a0697dff95e2c7dbf2e87f90831519f0, sub-system\BNLearn\Intervention\InterventionSpec.vb"

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

    '   Total Lines: 38
    '    Code Lines: 22 (57.89%)
    ' Comment Lines: 8 (21.05%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 8 (21.05%)
    '     File Size: 1.35 KB


    '     Class InterventionSpec
    ' 
    '         Properties: GeneIndex, GeneName, Mode, Value
    ' 
    '         Function: GetInterventionValue
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Intervention

    ''' <summary>
    ''' 单次干预定义
    ''' </summary>
    Public Class InterventionSpec

        ''' <summary>目标基因索引</summary>
        Public Property GeneIndex As Integer = -1

        ''' <summary>目标基因名称</summary>
        Public Property GeneName As String = ""

        ''' <summary>干预模式</summary>
        Public Property Mode As InterventionMode = InterventionMode.Knockout

        ''' <summary>干预值（Custom 模式下使用）</summary>
        Public Property Value As Double = 0.0

        ''' <summary>根据干预模式获取实际干预值</summary>
        Public Function GetInterventionValue(wildtypeMean As Double, wildtypeSD As Double) As Double
            Select Case Mode
                Case InterventionMode.Knockout
                    Return 0.0
                Case InterventionMode.Overexpression
                    Return wildtypeMean + 3.0 * wildtypeSD  ' 3倍标准差过表达
                Case InterventionMode.Knockdown
                    Return wildtypeMean - 2.0 * wildtypeSD  ' 2倍标准差下调
                Case InterventionMode.Custom
                    Return Value
                Case Else
                    Return Value
            End Select
        End Function

        Public Overrides Function ToString() As String
            Return $"[{GeneIndex}] {GeneName} as {Mode.Description} = {GetInterventionValue(10, 1.25)}"
        End Function

    End Class

End Namespace
