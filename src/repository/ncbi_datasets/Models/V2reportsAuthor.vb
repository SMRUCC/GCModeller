#Region "Microsoft.VisualBasic::b1d7c6233c4d71bc8bf1f0b41961a1a5, ncbi_datasets\Models\V2reportsAuthor.vb"

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

    '   Total Lines: 29
    '    Code Lines: 11 (37.93%)
    ' Comment Lines: 11 (37.93%)
    '    - Xml Docs: 54.55%
    ' 
    '   Blank Lines: 7 (24.14%)
    '     File Size: 801 B


    '     Class V2reportsAuthor
    ' 
    '         Properties: Affiliation, Name
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsAuthor.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsAuthor
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsAuthor

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <Field("name")>
        Public Property Name As String

        ''' <summary>
        ''' affiliation 属性
        ''' </summary>
        <Field("affiliation")>
        Public Property Affiliation As String

    End Class

End Namespace

