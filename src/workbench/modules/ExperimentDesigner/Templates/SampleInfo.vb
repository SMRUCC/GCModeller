#Region "Microsoft.VisualBasic::3a82dc3f6028babfdb15027c9194ca84, modules\ExperimentDesigner\Templates\SampleInfo.vb"

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

    '   Total Lines: 40
    '    Code Lines: 13
    ' Comment Lines: 21
    '   Blank Lines: 6
    '     File Size: 1.34 KB


    ' Class SampleInfo
    ' 
    '     Properties: batch, color, ID, injectionOrder, shape
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Language

''' <summary>
''' 一般而言，对于实验数据的分析而言，在进行数据存储的时候使用的是<see cref="ID"/>属性，
''' 而在进行数据可视化或者数据报告输出的时候，则是使用的<see cref="sample_name"/>属性
''' 作为显示的label
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

    Public Shared Iterator Function FromTagGroup(tags As NamedCollection(Of String),
                                                 Optional color As String = "#FF0000",
                                                 Optional shape As String = "21") As IEnumerable(Of SampleInfo)
        For Each tag As String In tags
            Yield New SampleInfo With {
                .batch = 1,
                .ID = tag,
                .injectionOrder = 1,
                .sample_info = tags.name,
                .sample_name = tag,
                .color = color,
                .shape = shape
            }
        Next
    End Function

End Class
