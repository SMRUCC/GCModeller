#Region "Microsoft.VisualBasic::9df71055f405b254f8e527a776c365b6, ncbi_datasets\Models\V2reportsProcessMetadata.vb"

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
    '     File Size: 1.28 KB


    '     Class V2reportsProcessMetadata
    ' 
    '         Properties: EvidenceCode, GoId, Name, Qualifier, Reference
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsProcessMetadata.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsProcessMetadata
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsProcessMetadata

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <Field("name")>
        Public Property Name As String

        ''' <summary>
        ''' go_id 属性
        ''' </summary>
        <Field("go_id")>
        Public Property GoId As String

        ''' <summary>
        ''' evidence_code 属性
        ''' </summary>
        <Field("evidence_code")>
        Public Property EvidenceCode As String

        ''' <summary>
        ''' qualifier 属性
        ''' </summary>
        <Field("qualifier")>
        Public Property Qualifier As String

        ''' <summary>
        ''' reference 属性
        ''' </summary>
        <Field("reference")>
        Public Property Reference As Object

    End Class

End Namespace

