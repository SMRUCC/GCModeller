#Region "Microsoft.VisualBasic::5c8f56ff14b8c1bdd2229dcb5a986f44, modules\ExperimentDesigner\Templates\SampleGroup.vb"

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

    '   Total Lines: 42
    '    Code Lines: 15 (35.71%)
    ' Comment Lines: 21 (50.00%)
    '    - Xml Docs: 95.24%
    ' 
    '   Blank Lines: 6 (14.29%)
    '     File Size: 1.36 KB


    ' Class SampleGroup
    ' 
    '     Properties: color, sample_info, sample_name, shape
    ' 
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
