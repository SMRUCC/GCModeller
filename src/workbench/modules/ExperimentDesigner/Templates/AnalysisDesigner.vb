#Region "Microsoft.VisualBasic::546150f5409f0531110dfd4779c34e26, modules\ExperimentDesigner\Templates\AnalysisDesigner.vb"

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

    '   Total Lines: 75
    '    Code Lines: 35
    ' Comment Lines: 30
    '   Blank Lines: 10
    '     File Size: 2.24 KB


    ' Class AnalysisDesigner
    ' 
    '     Properties: controls, note, title, treatment
    ' 
    '     Function: CreateTitle, Swap, (+2 Overloads) ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

' 数据对比类型

' □ 对照 vs 处理         
' □ 时间序列 

' 对照组 vs 处理组： 计算的时候为 处理组/对照组
'
' 时间序列： 时间2/时间1  时间3/时间2
'

''' <summary>
''' ```
''' <see cref="Controls"/> / <see cref="Treatment"/>
''' ```
''' 这个对象描述了如何设计一个比对计算实验分析
''' </summary>
<Template(ExperimentDesigner)>
Public Class AnalysisDesigner

    ''' <summary>
    ''' 对照组
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute("control")>
    Public Property controls As String
    ''' <summary>
    ''' 处理组
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute("treatment")>
    Public Property treatment As String
    ''' <summary>
    ''' 用户备注信息，这个属性不会被使用
    ''' </summary>
    ''' <returns></returns>
    <XmlText>
    Public Property note As String

    Public ReadOnly Property title As String
        Get
            Return $"{controls} vs {treatment}"
        End Get
    End Property

    ''' <summary>
    ''' 对于iTraq实验数据而言，这里是具体的样品的编号的比对
    ''' 对于LabelFree实验数据而言，由于需要手工计算FC值，所以在这里比对的是样品的组别名称
    ''' </summary>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return $"{treatment}/{controls}"
    End Function

    Public Overloads Function ToString(translation As Dictionary(Of String, String)) As String
        Return $"{translation(treatment)}/{translation(controls)}"
    End Function

    Public Function Swap() As AnalysisDesigner
        Return New AnalysisDesigner With {
            .controls = treatment,
            .treatment = controls
        }
    End Function

    Public Shared Function CreateTitle(label As String) As String
        With label.Split("/"c)
            Return $"{ .Last} vs { .First}"
        End With
    End Function
End Class
