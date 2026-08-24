#Region "Microsoft.VisualBasic::eb7ec9b6aabdc5916040dac25e1f7f07, ncbi_datasets\Models\V2reportsError.vb"

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

    '   Total Lines: 65
    '    Code Lines: 23 (35.38%)
    ' Comment Lines: 29 (44.62%)
    '    - Xml Docs: 82.76%
    ' 
    '   Blank Lines: 13 (20.00%)
    '     File Size: 1.88 KB


    '     Class V2reportsError
    ' 
    '         Properties: AssemblyErrorCode, GeneErrorCode, InvalidIdentifiers, Message, OrganelleErrorCode
    '                     Reason, TaxonomyErrorCode, VirusErrorCode
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsError.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsError
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsError

        ''' <summary>
        ''' assembly_error_code 属性
        ''' </summary>
        <Field("assembly_error_code")>
        Public Property AssemblyErrorCode As Object

        ''' <summary>
        ''' gene_error_code 属性
        ''' </summary>
        <Field("gene_error_code")>
        Public Property GeneErrorCode As Object

        ''' <summary>
        ''' organelle_error_code 属性
        ''' </summary>
        <Field("organelle_error_code")>
        Public Property OrganelleErrorCode As Object

        ''' <summary>
        ''' virus_error_code 属性
        ''' </summary>
        <Field("virus_error_code")>
        Public Property VirusErrorCode As Object

        ''' <summary>
        ''' taxonomy_error_code 属性
        ''' </summary>
        <Field("taxonomy_error_code")>
        Public Property TaxonomyErrorCode As Object

        ''' <summary>
        ''' reason 属性
        ''' </summary>
        <Field("reason")>
        Public Property Reason As String

        ''' <summary>
        ''' message 属性
        ''' </summary>
        <Field("message")>
        Public Property Message As String

        ''' <summary>
        ''' invalid_identifiers 属性
        ''' </summary>
        <Field("invalid_identifiers")>
        Public Property InvalidIdentifiers As List(Of String)

    End Class

End Namespace

