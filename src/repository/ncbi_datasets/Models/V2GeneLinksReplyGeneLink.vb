#Region "Microsoft.VisualBasic::dae233275baf491654f55194530363de, ncbi_datasets\Models\V2GeneLinksReplyGeneLink.vb"

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
    '     File Size: 1.17 KB


    '     Class V2GeneLinksReplyGeneLink
    ' 
    '         Properties: GeneId, GeneLinkType, ResourceId, ResourceLink
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2GeneLinksReplyGeneLink.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2GeneLinksReplyGeneLink
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2GeneLinksReplyGeneLink

        ''' <summary>
        ''' gene_id 属性
        ''' </summary>
        <Field("gene_id")>
        Public Property GeneId As Integer?

        ''' <summary>
        ''' gene_link_type 属性
        ''' </summary>
        <Field("gene_link_type")>
        Public Property GeneLinkType As Object

        ''' <summary>
        ''' resource_link 属性
        ''' </summary>
        <Field("resource_link")>
        Public Property ResourceLink As String

        ''' <summary>
        ''' resource_id 属性
        ''' </summary>
        <Field("resource_id")>
        Public Property ResourceId As String

    End Class

End Namespace

