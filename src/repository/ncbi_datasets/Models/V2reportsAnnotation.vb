#Region "Microsoft.VisualBasic::65daec560ae72013e006b61fece3a9af, ncbi_datasets\Models\V2reportsAnnotation.vb"

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
    '     File Size: 1.41 KB


    '     Class V2reportsAnnotation
    ' 
    '         Properties: AnnotationName, AnnotationReleaseDate, AssemblyAccession, AssemblyName, GenomicLocations
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsAnnotation.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsAnnotation
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsAnnotation

        ''' <summary>
        ''' assembly_accession 属性
        ''' </summary>
        <Field("assembly_accession")>
        Public Property AssemblyAccession As String

        ''' <summary>
        ''' assembly_name 属性
        ''' </summary>
        <Field("assembly_name")>
        Public Property AssemblyName As String

        ''' <summary>
        ''' annotation_name 属性
        ''' </summary>
        <Field("annotation_name")>
        Public Property AnnotationName As String

        ''' <summary>
        ''' annotation_release_date 属性
        ''' </summary>
        <Field("annotation_release_date")>
        Public Property AnnotationReleaseDate As String

        ''' <summary>
        ''' genomic_locations 属性
        ''' </summary>
        <Field("genomic_locations")>
        Public Property GenomicLocations As List(Of Object)

    End Class

End Namespace

