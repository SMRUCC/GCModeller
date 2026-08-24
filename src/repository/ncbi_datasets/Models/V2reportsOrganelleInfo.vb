#Region "Microsoft.VisualBasic::15a0e30165d7b78acb9ecd2848eae6ea, ncbi_datasets\Models\V2reportsOrganelleInfo.vb"

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
    '     File Size: 1.52 KB


    '     Class V2reportsOrganelleInfo
    ' 
    '         Properties: AssemblyName, Bioproject, Description, InfraspecificName, Submitter
    '                     TotalSeqLength
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsOrganelleInfo.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsOrganelleInfo
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsOrganelleInfo

        ''' <summary>
        ''' assembly_name 属性
        ''' </summary>
        <Field("assembly_name")>
        Public Property AssemblyName As String

        ''' <summary>
        ''' infraspecific_name 属性
        ''' </summary>
        <Field("infraspecific_name")>
        Public Property InfraspecificName As String

        ''' <summary>
        ''' bioproject 属性
        ''' </summary>
        <Field("bioproject")>
        Public Property Bioproject As List(Of String)

        ''' <summary>
        ''' description 属性
        ''' </summary>
        <Field("description")>
        Public Property Description As String

        ''' <summary>
        ''' total_seq_length 属性
        ''' </summary>
        <Field("total_seq_length")>
        Public Property TotalSeqLength As String

        ''' <summary>
        ''' submitter 属性
        ''' </summary>
        <Field("submitter")>
        Public Property Submitter As String

    End Class

End Namespace

