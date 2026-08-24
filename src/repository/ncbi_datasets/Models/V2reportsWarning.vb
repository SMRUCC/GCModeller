#Region "Microsoft.VisualBasic::08675736a1b80e5d27d4926c645024b0, ncbi_datasets\Models\V2reportsWarning.vb"

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
    '     File Size: 1.33 KB


    '     Class V2reportsWarning
    ' 
    '         Properties: GeneWarningCode, Message, Reason, ReplacedId, UnrecognizedIdentifier
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsWarning.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsWarning
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsWarning

        ''' <summary>
        ''' gene_warning_code 属性
        ''' </summary>
        <Field("gene_warning_code")>
        Public Property GeneWarningCode As Object

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
        ''' replaced_id 属性
        ''' </summary>
        <Field("replaced_id")>
        Public Property ReplacedId As Object

        ''' <summary>
        ''' unrecognized_identifier 属性
        ''' </summary>
        <Field("unrecognized_identifier")>
        Public Property UnrecognizedIdentifier As String

    End Class

End Namespace

