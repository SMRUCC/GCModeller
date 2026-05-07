' ============================================================================
' V2reportsAdditionalSubmitter.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsAdditionalSubmitter
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsAdditionalSubmitter

        ''' <summary>
        ''' genbank_accession 属性
        ''' </summary>
        <Field("genbank_accession")>
        Public Property GenbankAccession As String

        ''' <summary>
        ''' refseq_accession 属性
        ''' </summary>
        <Field("refseq_accession")>
        Public Property RefseqAccession As String

        ''' <summary>
        ''' chr_name 属性
        ''' </summary>
        <Field("chr_name")>
        Public Property ChrName As String

        ''' <summary>
        ''' molecule_type 属性
        ''' </summary>
        <Field("molecule_type")>
        Public Property MoleculeType As String

        ''' <summary>
        ''' submitter 属性
        ''' </summary>
        <Field("submitter")>
        Public Property Submitter As String

        ''' <summary>
        ''' bioproject_accession 属性
        ''' </summary>
        <Field("bioproject_accession")>
        Public Property BioprojectAccession As String

    End Class

End Namespace
