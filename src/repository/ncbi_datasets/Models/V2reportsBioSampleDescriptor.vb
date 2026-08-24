#Region "Microsoft.VisualBasic::b9f46cafc9969a4f95eb41e6336ddab1, ncbi_datasets\Models\V2reportsBioSampleDescriptor.vb"

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

    '   Total Lines: 245
    '    Code Lines: 83 (33.88%)
    ' Comment Lines: 119 (48.57%)
    '    - Xml Docs: 95.80%
    ' 
    '   Blank Lines: 43 (17.55%)
    '     File Size: 6.51 KB


    '     Class V2reportsBioSampleDescriptor
    ' 
    '         Properties: Accession, Age, Attributes, BiomaterialProvider, Bioprojects
    '                     Breed, CollectedBy, CollectionDate, Cultivar, Description
    '                     DevStage, Ecotype, GeoLocName, Host, HostDisease
    '                     IdentifiedBy, IfsacCategory, Isolate, IsolateNameAlias, IsolationSource
    '                     LastUpdated, LatLon, Models, Owner, Package
    '                     ProjectName, PublicationDate, SampleIds, SampleName, Serotype
    '                     Serovar, Sex, SourceType, Status, Strain
    '                     SubmissionDate, SubSpecies, Tissue
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsBioSampleDescriptor.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsBioSampleDescriptor
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsBioSampleDescriptor

        ''' <summary>
        ''' accession 属性
        ''' </summary>
        <Field("accession")>
        Public Property Accession As String

        ''' <summary>
        ''' last_updated 属性
        ''' </summary>
        <Field("last_updated")>
        Public Property LastUpdated As String

        ''' <summary>
        ''' publication_date 属性
        ''' </summary>
        <Field("publication_date")>
        Public Property PublicationDate As String

        ''' <summary>
        ''' submission_date 属性
        ''' </summary>
        <Field("submission_date")>
        Public Property SubmissionDate As String

        ''' <summary>
        ''' sample_ids 属性
        ''' </summary>
        <Field("sample_ids")>
        Public Property SampleIds As List(Of Object)

        ''' <summary>
        ''' description 属性
        ''' </summary>
        <Field("description")>
        Public Property Description As Object

        ''' <summary>
        ''' owner 属性
        ''' </summary>
        <Field("owner")>
        Public Property Owner As Object

        ''' <summary>
        ''' models 属性
        ''' </summary>
        <Field("models")>
        Public Property Models As List(Of String)

        ''' <summary>
        ''' bioprojects 属性
        ''' </summary>
        <Field("bioprojects")>
        Public Property Bioprojects As List(Of Object)

        ''' <summary>
        ''' package 属性
        ''' </summary>
        <Field("package")>
        Public Property Package As String

        ''' <summary>
        ''' attributes 属性
        ''' </summary>
        <Field("attributes")>
        Public Property Attributes As List(Of Object)

        ''' <summary>
        ''' status 属性
        ''' </summary>
        <Field("status")>
        Public Property Status As Object

        ''' <summary>
        ''' age 属性
        ''' </summary>
        <Field("age")>
        Public Property Age As String

        ''' <summary>
        ''' biomaterial_provider 属性
        ''' </summary>
        <Field("biomaterial_provider")>
        Public Property BiomaterialProvider As String

        ''' <summary>
        ''' breed 属性
        ''' </summary>
        <Field("breed")>
        Public Property Breed As String

        ''' <summary>
        ''' collected_by 属性
        ''' </summary>
        <Field("collected_by")>
        Public Property CollectedBy As String

        ''' <summary>
        ''' collection_date 属性
        ''' </summary>
        <Field("collection_date")>
        Public Property CollectionDate As String

        ''' <summary>
        ''' cultivar 属性
        ''' </summary>
        <Field("cultivar")>
        Public Property Cultivar As String

        ''' <summary>
        ''' dev_stage 属性
        ''' </summary>
        <Field("dev_stage")>
        Public Property DevStage As String

        ''' <summary>
        ''' ecotype 属性
        ''' </summary>
        <Field("ecotype")>
        Public Property Ecotype As String

        ''' <summary>
        ''' geo_loc_name 属性
        ''' </summary>
        <Field("geo_loc_name")>
        Public Property GeoLocName As String

        ''' <summary>
        ''' host 属性
        ''' </summary>
        <Field("host")>
        Public Property Host As String

        ''' <summary>
        ''' host_disease 属性
        ''' </summary>
        <Field("host_disease")>
        Public Property HostDisease As String

        ''' <summary>
        ''' identified_by 属性
        ''' </summary>
        <Field("identified_by")>
        Public Property IdentifiedBy As String

        ''' <summary>
        ''' ifsac_category 属性
        ''' </summary>
        <Field("ifsac_category")>
        Public Property IfsacCategory As String

        ''' <summary>
        ''' isolate 属性
        ''' </summary>
        <Field("isolate")>
        Public Property Isolate As String

        ''' <summary>
        ''' isolate_name_alias 属性
        ''' </summary>
        <Field("isolate_name_alias")>
        Public Property IsolateNameAlias As String

        ''' <summary>
        ''' isolation_source 属性
        ''' </summary>
        <Field("isolation_source")>
        Public Property IsolationSource As String

        ''' <summary>
        ''' lat_lon 属性
        ''' </summary>
        <Field("lat_lon")>
        Public Property LatLon As String

        ''' <summary>
        ''' project_name 属性
        ''' </summary>
        <Field("project_name")>
        Public Property ProjectName As String

        ''' <summary>
        ''' sample_name 属性
        ''' </summary>
        <Field("sample_name")>
        Public Property SampleName As String

        ''' <summary>
        ''' serovar 属性
        ''' </summary>
        <Field("serovar")>
        Public Property Serovar As String

        ''' <summary>
        ''' sex 属性
        ''' </summary>
        <Field("sex")>
        Public Property Sex As String

        ''' <summary>
        ''' source_type 属性
        ''' </summary>
        <Field("source_type")>
        Public Property SourceType As String

        ''' <summary>
        ''' strain 属性
        ''' </summary>
        <Field("strain")>
        Public Property Strain As String

        ''' <summary>
        ''' sub_species 属性
        ''' </summary>
        <Field("sub_species")>
        Public Property SubSpecies As String

        ''' <summary>
        ''' tissue 属性
        ''' </summary>
        <Field("tissue")>
        Public Property Tissue As String

        ''' <summary>
        ''' serotype 属性
        ''' </summary>
        <Field("serotype")>
        Public Property Serotype As String

    End Class

End Namespace

