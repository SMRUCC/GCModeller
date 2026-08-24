#Region "Microsoft.VisualBasic::4a74d1d6cadb04e7305c79d51443e502, ncbi_datasets\Models\V2reportsSequenceFeature.vb"

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
    '     File Size: 2.47 KB


    '     Class V2reportsSequenceFeature
    ' 
    '         Properties: CodedProteinInfo, DataProvenance, EcNumber, GeneId, Location
    '                     LocusTag, Name, NestedFeatures, OtherNames, PredictionSource
    '                     SignalSequence, Type
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsSequenceFeature.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsSequenceFeature
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsSequenceFeature

        ''' <summary>
        ''' type 属性
        ''' </summary>
        <Field("type")>
        Public Property Type As String

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <Field("name")>
        Public Property Name As String

        ''' <summary>
        ''' locus_tag 属性
        ''' </summary>
        <Field("locus_tag")>
        Public Property LocusTag As String

        ''' <summary>
        ''' gene_id 属性
        ''' </summary>
        <Field("gene_id")>
        Public Property GeneId As String

        ''' <summary>
        ''' location 属性
        ''' </summary>
        <Field("location")>
        Public Property Location As Object

        ''' <summary>
        ''' other_names 属性
        ''' </summary>
        <Field("other_names")>
        Public Property OtherNames As List(Of String)

        ''' <summary>
        ''' ec_number 属性
        ''' </summary>
        <Field("ec_number")>
        Public Property EcNumber As List(Of String)

        ''' <summary>
        ''' coded_protein_info 属性
        ''' </summary>
        <Field("coded_protein_info")>
        Public Property CodedProteinInfo As Object

        ''' <summary>
        ''' prediction_source 属性
        ''' </summary>
        <Field("prediction_source")>
        Public Property PredictionSource As Object

        ''' <summary>
        ''' data_provenance 属性
        ''' </summary>
        <Field("data_provenance")>
        Public Property DataProvenance As Object

        ''' <summary>
        ''' signal_sequence 属性
        ''' </summary>
        <Field("signal_sequence")>
        Public Property SignalSequence As String

        ''' <summary>
        ''' nested_features 属性
        ''' </summary>
        <Field("nested_features")>
        Public Property NestedFeatures As List(Of Object)

    End Class

End Namespace

