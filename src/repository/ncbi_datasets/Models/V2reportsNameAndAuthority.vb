#Region "Microsoft.VisualBasic::94bb8f4b892b1f93cd8a06fb4854dd79, ncbi_datasets\Models\V2reportsNameAndAuthority.vb"

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

    '   Total Lines: 89
    '    Code Lines: 31 (34.83%)
    ' Comment Lines: 41 (46.07%)
    '    - Xml Docs: 87.80%
    ' 
    '   Blank Lines: 17 (19.10%)
    '     File Size: 2.53 KB


    '     Class V2reportsNameAndAuthority
    ' 
    '         Properties: Authority, Basionym, CuratorSynonym, Formal, HeterotypicSynonyms
    '                     HomotypicSynonyms, InformalNames, Name, Notes, OtherSynonyms
    '                     Publications, TypeStrains
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsNameAndAuthority.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsNameAndAuthority
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsNameAndAuthority

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <Field("name")>
        Public Property Name As String

        ''' <summary>
        ''' authority 属性
        ''' </summary>
        <Field("authority")>
        Public Property Authority As String

        ''' <summary>
        ''' type_strains 属性
        ''' </summary>
        <Field("type_strains")>
        Public Property TypeStrains As List(Of Object)

        ''' <summary>
        ''' curator_synonym 属性
        ''' </summary>
        <Field("curator_synonym")>
        Public Property CuratorSynonym As String

        ''' <summary>
        ''' homotypic_synonyms 属性
        ''' </summary>
        <Field("homotypic_synonyms")>
        Public Property HomotypicSynonyms As List(Of Object)

        ''' <summary>
        ''' heterotypic_synonyms 属性
        ''' </summary>
        <Field("heterotypic_synonyms")>
        Public Property HeterotypicSynonyms As List(Of Object)

        ''' <summary>
        ''' other_synonyms 属性
        ''' </summary>
        <Field("other_synonyms")>
        Public Property OtherSynonyms As List(Of Object)

        ''' <summary>
        ''' informal_names 属性
        ''' </summary>
        <Field("informal_names")>
        Public Property InformalNames As List(Of String)

        ''' <summary>
        ''' basionym 属性
        ''' </summary>
        <Field("basionym")>
        Public Property Basionym As Object

        ''' <summary>
        ''' publications 属性
        ''' </summary>
        <Field("publications")>
        Public Property Publications As List(Of Object)

        ''' <summary>
        ''' notes 属性
        ''' </summary>
        <Field("notes")>
        Public Property Notes As List(Of Object)

        ''' <summary>
        ''' formal 属性
        ''' </summary>
        <Field("formal")>
        Public Property Formal As Boolean?

    End Class

End Namespace

