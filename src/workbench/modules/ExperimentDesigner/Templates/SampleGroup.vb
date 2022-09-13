#Region "Microsoft.VisualBasic::b676d372a522f480d1613a631d4b4a5c, modules\ExperimentDesigner\Templates\SampleGroup.vb"

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

    '   Total Lines: 30
    '    Code Lines: 13
    ' Comment Lines: 13
    '   Blank Lines: 4
    '     File Size: 1.03 KB


    ' Class SampleGroup
    ' 
    '     Properties: sample_info, sample_name
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

