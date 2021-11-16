#Region "Microsoft.VisualBasic::2b31479faf1ec8a76c6b386e8571f536, modules\ExperimentDesigner\Templates\SampleInfo.vb"

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

    ' Class SampleInfo
    ' 
    '     Properties: batch, color, ID, injectionOrder, shape
    ' 
    ' Class SampleGroup
    ' 
    '     Properties: sample_info, sample_name
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language

''' <summary>
''' 一般而言，对于实验数据的分析而言，在进行数据存储的时候使用的是<see cref="ID"/>属性，而在进行数据可视化或者数据报告输出的时候，则是使用的<see cref="sample_name"/>属性作为显示的label
''' </summary>
<Template(ExperimentDesigner)>
Public Class SampleInfo : Inherits SampleGroup
    Implements INamedValue

    ''' <summary>
    ''' 样品的标记符号，符合VisualBasic标识符语法的目标样品标识符
    ''' </summary>
    ''' <returns></returns>
    Public Property ID As String Implements INamedValue.Key

    ''' <summary>
    ''' index编号
    ''' </summary>
    ''' <returns></returns>
    Public Property injectionOrder As Integer
    Public Property batch As Integer

    ''' <summary>
    ''' 绘图可视化的时候的颜色
    ''' </summary>
    ''' <returns></returns>
    Public Property color As String

    ''' <summary>
    ''' legend的形状
    ''' </summary>
    ''' <returns></returns>
    Public Property shape As String

End Class

''' <summary>
''' 样品的分组信息
''' </summary>
<Template(ExperimentDesigner)> Public Class SampleGroup
    Implements INamedValue
    Implements Value(Of String).IValueOf

    ''' <summary>
    ''' 在报告之中的显示名称，可能会含有一些奇怪的符号
    ''' </summary>
    ''' <returns></returns>
    Public Property sample_name As String Implements IKeyedEntity(Of String).Key

    ''' <summary>
    ''' the sample info.
    ''' 
    ''' (样品的实验设计分组信息)
    ''' </summary>
    ''' <returns></returns>
    Public Property sample_info As String Implements Value(Of String).IValueOf.Value

    Public Overrides Function ToString() As String
        Return $"[{sample_info}] {sample_name}"
    End Function
End Class
