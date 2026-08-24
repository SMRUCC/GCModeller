#Region "Microsoft.VisualBasic::bfc3e6376b135302891aa56c6d648842, ncbi_datasets\Models\V2reportsANIMatch.vb"

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

    '   Total Lines: 53
    '    Code Lines: 19 (35.85%)
    ' Comment Lines: 23 (43.40%)
    '    - Xml Docs: 78.26%
    ' 
    '   Blank Lines: 11 (20.75%)
    '     File Size: 1.48 KB


    '     Class V2reportsANIMatch
    ' 
    '         Properties: Ani, Assembly, AssemblyCoverage, Category, OrganismName
    '                     TypeAssemblyCoverage
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsANIMatch.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsANIMatch
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsANIMatch

        ''' <summary>
        ''' assembly 属性
        ''' </summary>
        <Field("assembly")>
        Public Property Assembly As String

        ''' <summary>
        ''' organism_name 属性
        ''' </summary>
        <Field("organism_name")>
        Public Property OrganismName As String

        ''' <summary>
        ''' category 属性
        ''' </summary>
        <Field("category")>
        Public Property Category As Object

        ''' <summary>
        ''' ani 属性
        ''' </summary>
        <Field("ani")>
        Public Property Ani As Single?

        ''' <summary>
        ''' assembly_coverage 属性
        ''' </summary>
        <Field("assembly_coverage")>
        Public Property AssemblyCoverage As Single?

        ''' <summary>
        ''' type_assembly_coverage 属性
        ''' </summary>
        <Field("type_assembly_coverage")>
        Public Property TypeAssemblyCoverage As Single?

    End Class

End Namespace

