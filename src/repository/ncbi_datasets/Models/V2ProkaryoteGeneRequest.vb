#Region "Microsoft.VisualBasic::f1940d903386dfac414c72859e273fab, ncbi_datasets\Models\V2ProkaryoteGeneRequest.vb"

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

    '   Total Lines: 41
    '    Code Lines: 15 (36.59%)
    ' Comment Lines: 17 (41.46%)
    '    - Xml Docs: 70.59%
    ' 
    '   Blank Lines: 9 (21.95%)
    '     File Size: 1.21 KB


    '     Class V2ProkaryoteGeneRequest
    ' 
    '         Properties: Accessions, GeneFlankConfig, IncludeAnnotationType, Taxon
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2ProkaryoteGeneRequest.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2ProkaryoteGeneRequest
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2ProkaryoteGeneRequest

        ''' <summary>
        ''' accessions 属性
        ''' </summary>
        <Field("accessions")>
        Public Property Accessions As List(Of String)

        ''' <summary>
        ''' include_annotation_type 属性
        ''' </summary>
        <Field("include_annotation_type")>
        Public Property IncludeAnnotationType As List(Of Object)

        ''' <summary>
        ''' gene_flank_config 属性
        ''' </summary>
        <Field("gene_flank_config")>
        Public Property GeneFlankConfig As Object

        ''' <summary>
        ''' taxon 属性
        ''' </summary>
        <Field("taxon")>
        Public Property Taxon As String

    End Class

End Namespace

