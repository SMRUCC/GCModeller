#Region "Microsoft.VisualBasic::e31e5a1240c580fa1c063c4106361ac3, modules\ExperimentDesigner\Templates\SampleGroup.vb"

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

    '   Total Lines: 58
    '    Code Lines: 23 (39.66%)
    ' Comment Lines: 27 (46.55%)
    '    - Xml Docs: 96.30%
    ' 
    '   Blank Lines: 8 (13.79%)
    '     File Size: 1.78 KB


    ' Class SampleGroup
    ' 
    '     Properties: color, sample_info, sample_name, shape
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Language

''' <summary>
''' the analysis property of the sample data group.(样品的分组信息)
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

    Sub New()
    End Sub

    ''' <summary>
    ''' copy the sample group information
    ''' </summary>
    ''' <param name="info">
    ''' could be copy from the information of a sample: <see cref="SampleInfo"/>
    ''' </param>
    Sub New(info As SampleGroup)
        sample_name = info.sample_name
        sample_info = info.sample_info
        color = info.color
        shape = info.shape
    End Sub

    Public Overrides Function ToString() As String
        Return $"[{sample_info}] {sample_name}"
    End Function
End Class
