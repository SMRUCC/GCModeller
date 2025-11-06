#Region "Microsoft.VisualBasic::658684443107ff40b24b5e2b1c5d24ef, modules\ExperimentDesigner\Templates\SampleInfo.vb"

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

    '   Total Lines: 59
    '    Code Lines: 35 (59.32%)
    ' Comment Lines: 17 (28.81%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (11.86%)
    '     File Size: 2.03 KB


    ' Class SampleInfo
    ' 
    '     Properties: batch, ID, injectionOrder
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: FromTagGroup
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

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
    ''' <summary>
    ''' the experiment batch id
    ''' </summary>
    ''' <returns></returns>
    Public Property batch As Integer

    Sub New()
    End Sub

    Sub New(copy As SampleInfo)
        Me.ID = copy.ID
        Me.injectionOrder = copy.injectionOrder
        Me.batch = copy.batch
        Me.color = copy.color
        Me.sample_info = copy.sample_info
        Me.sample_name = copy.sample_name
        Me.shape = copy.shape
    End Sub

    Public Overrides Function ToString() As String
        Return $"[{ID} - {sample_info}({color})] {sample_name}"
    End Function

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
