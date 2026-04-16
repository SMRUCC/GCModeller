#Region "Microsoft.VisualBasic::2ba42983ba9b2dba79c39165c9940213, engine\IO\GCMarkupLanguage\v2\Xml\Metabolism\Enzyme.vb"

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

    '   Total Lines: 110
    '    Code Lines: 52 (47.27%)
    ' Comment Lines: 39 (35.45%)
    '    - Xml Docs: 97.44%
    ' 
    '   Blank Lines: 19 (17.27%)
    '     File Size: 3.33 KB


    '     Class Enzyme
    ' 
    '         Properties: catalysis, ECNumber, KO, proteinID
    ' 
    '         Function: ToString
    ' 
    '     Class Catalysis
    ' 
    '         Properties: formula, parameter, PH, reaction, temperature
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ToString
    ' 
    '     Class KineticsParameter
    ' 
    '         Properties: isModifier, name, target, value
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Math.Scripting.MathExpression

Namespace v2

    ''' <summary>
    ''' 酶分子对象
    ''' </summary>
    <XmlType("enzyme", [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class Enzyme : Implements INamedValue

        ''' <summary>
        ''' the protein id of this enzyme
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property proteinID As String Implements IKeyedEntity(Of String).Key
        <XmlAttribute> Public Property KO As String
        <XmlAttribute> Public Property ECNumber As String

        ''' <summary>
        ''' 酶分子所催化的反应列表
        ''' </summary>
        ''' <returns></returns>
        <XmlElement>
        Public Property catalysis As Catalysis()

        Public Overrides Function ToString() As String
            Return proteinID
        End Function

    End Class

    ''' <summary>
    ''' 酶分子对目标反应过程的催化动力学函数
    ''' </summary>
    <XmlType("catalysis", [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class Catalysis

        ''' <summary>
        ''' The reaction id
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property reaction As String
        <XmlAttribute> Public Property PH As Double
        ''' <summary>
        ''' 单位为摄氏度的温度参数值
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property temperature As Double

        ''' <summary>
        ''' 通过sabio-rk数据库得到的动力学函数方程
        ''' </summary>
        ''' <returns></returns>
        Public Property formula As FunctionElement

        ''' <summary>
        ''' 动力学方程的参数列表
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <XmlElement>
        Public Property parameter As KineticsParameter()

        Sub New()
        End Sub

        Sub New(id As String)
            reaction = id
        End Sub

        Public Overrides Function ToString() As String
            If formula Is Nothing Then
                Return "null"
            Else
                Return formula.lambda
            End If
        End Function

    End Class

    Public Class KineticsParameter

        <XmlAttribute> Public Property name As String
        ''' <summary>
        ''' kegg compound id
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property target As String
        ''' <summary>
        ''' true means current parameter is enzyme concentration
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property isModifier As Boolean

        <XmlText>
        Public Property value As Double

        Public Overrides Function ToString() As String
            If Not target.StringEmpty Then
                Return $"Dim {name} As {target} = {value}"
            Else
                Return $"Dim {name} = {value}"
            End If
        End Function

    End Class
End Namespace
