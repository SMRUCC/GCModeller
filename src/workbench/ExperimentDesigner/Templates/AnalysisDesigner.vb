#Region "Microsoft.VisualBasic::3c8416c9e97414e16c614ac2fa8e5f1b, ..\workbench\ExperimentDesigner\Templates\AnalysisDesigner.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Data.csv

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
    Public Property Controls As String
    ''' <summary>
    ''' 处理组
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute("treatment")>
    Public Property Treatment As String
    ''' <summary>
    ''' 用户备注信息，这个属性不会被使用
    ''' </summary>
    ''' <returns></returns>
    <XmlText>
    Public Property Note As String

    ''' <summary>
    ''' 对于iTraq实验数据而言，这里是具体的样品的编号的比对
    ''' 对于LabelFree实验数据而言，由于需要手工计算FC值，所以在这里比对的是样品的组别名称
    ''' </summary>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return $"{Treatment}/{Controls}"
    End Function

    Public Function Swap() As AnalysisDesigner
        Return New AnalysisDesigner With {
            .Controls = Treatment,
            .Treatment = Controls
        }
    End Function

    Public Shared Function CreateTitle(label As String) As String
        With label.Split("/"c)
            Return $"{ .Last} vs { .First}"
        End With
    End Function
End Class
