#Region "Microsoft.VisualBasic::cb4ba5ef2714132b227fb3954c8e9541, ncbi_datasets\Models\V2reportsOrganelle.vb"

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

    '   Total Lines: 77
    '    Code Lines: 27 (35.06%)
    ' Comment Lines: 35 (45.45%)
    '    - Xml Docs: 85.71%
    ' 
    '   Blank Lines: 15 (19.48%)
    '     File Size: 2.05 KB


    '     Class V2reportsOrganelle
    ' 
    '         Properties: Bioprojects, Biosample, Description, Genbank, GeneCount
    '                     GeneCounts, Length, Organism, Refseq, Topology
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsOrganelle.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsOrganelle
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsOrganelle

        ''' <summary>
        ''' description 属性
        ''' </summary>
        <Field("description")>
        Public Property Description As Object

        ''' <summary>
        ''' genbank 属性
        ''' </summary>
        <Field("genbank")>
        Public Property Genbank As Object

        ''' <summary>
        ''' refseq 属性
        ''' </summary>
        <Field("refseq")>
        Public Property Refseq As Object

        ''' <summary>
        ''' organism 属性
        ''' </summary>
        <Field("organism")>
        Public Property Organism As Object

        ''' <summary>
        ''' bioprojects 属性
        ''' </summary>
        <Field("bioprojects")>
        Public Property Bioprojects As List(Of Object)

        ''' <summary>
        ''' biosample 属性
        ''' </summary>
        <Field("biosample")>
        Public Property Biosample As Object

        ''' <summary>
        ''' gene_counts 属性
        ''' </summary>
        <Field("gene_counts")>
        Public Property GeneCounts As Object

        ''' <summary>
        ''' length 属性
        ''' </summary>
        <Field("length")>
        Public Property Length As Integer?

        ''' <summary>
        ''' topology 属性
        ''' </summary>
        <Field("topology")>
        Public Property Topology As Object

        ''' <summary>
        ''' gene_count 属性
        ''' </summary>
        <Field("gene_count")>
        Public Property GeneCount As Integer?

    End Class

End Namespace

