#Region "Microsoft.VisualBasic::2888c8d68fe61ac860835c170e8026c8, ncbi_datasets\Models\V2reportsProteinFamily.vb"

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

    '   Total Lines: 47
    '    Code Lines: 17 (36.17%)
    ' Comment Lines: 20 (42.55%)
    '    - Xml Docs: 75.00%
    ' 
    '   Blank Lines: 10 (21.28%)
    '     File Size: 1.26 KB


    '     Class V2reportsProteinFamily
    ' 
    '         Properties: Description, Identifier, Method, Name, Type
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsProteinFamily.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsProteinFamily
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsProteinFamily

        ''' <summary>
        ''' method 属性
        ''' </summary>
        <Field("method")>
        Public Property Method As String

        ''' <summary>
        ''' identifier 属性
        ''' </summary>
        <Field("identifier")>
        Public Property Identifier As Integer?

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <Field("name")>
        Public Property Name As String

        ''' <summary>
        ''' description 属性
        ''' </summary>
        <Field("description")>
        Public Property Description As String

        ''' <summary>
        ''' type 属性
        ''' </summary>
        <Field("type")>
        Public Property Type As String

    End Class

End Namespace

