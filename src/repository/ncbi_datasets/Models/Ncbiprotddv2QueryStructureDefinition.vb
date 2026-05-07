' ============================================================================
' Ncbiprotddv2QueryStructureDefinition.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: ncbiprotddv2QueryStructureDefinition
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class Ncbiprotddv2QueryStructureDefinition

        ''' <summary>
        ''' sdid 属性
        ''' </summary>
        <Field("sdid")>
        Public Property Sdid As Integer?

        ''' <summary>
        ''' mmdb_id 属性
        ''' </summary>
        <Field("mmdb_id")>
        Public Property MmdbId As Integer?

        ''' <summary>
        ''' pdb_id 属性
        ''' </summary>
        <Field("pdb_id")>
        Public Property PdbId As String

        ''' <summary>
        ''' description 属性
        ''' </summary>
        <Field("description")>
        Public Property Description As String

        ''' <summary>
        ''' chain_id 属性
        ''' </summary>
        <Field("chain_id")>
        Public Property ChainId As String

        ''' <summary>
        ''' domain_number 属性
        ''' </summary>
        <Field("domain_number")>
        Public Property DomainNumber As Integer?

        ''' <summary>
        ''' from 属性
        ''' </summary>
        <Field("from")>
        Public Property From As Integer?

        ''' <summary>
        ''' to 属性
        ''' </summary>
        <Field("to")>
        Public Property To As Integer?

    End Class

End Namespace
