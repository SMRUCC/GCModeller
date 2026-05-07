' ============================================================================
' V2reportsSequenceInfo.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsSequenceInfo
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsSequenceInfo

        ''' <summary>
        ''' assembly_accession 属性
        ''' </summary>
        <Field("assembly_accession")>
        Public Property AssemblyAccession As String

        ''' <summary>
        ''' chr_name 属性
        ''' </summary>
        <Field("chr_name")>
        Public Property ChrName As String

        ''' <summary>
        ''' ucsc_style_name 属性
        ''' </summary>
        <Field("ucsc_style_name")>
        Public Property UcscStyleName As String

        ''' <summary>
        ''' sort_order 属性
        ''' </summary>
        <Field("sort_order")>
        Public Property SortOrder As Integer?

        ''' <summary>
        ''' assigned_molecule_location_type 属性
        ''' </summary>
        <Field("assigned_molecule_location_type")>
        Public Property AssignedMoleculeLocationType As String

        ''' <summary>
        ''' refseq_accession 属性
        ''' </summary>
        <Field("refseq_accession")>
        Public Property RefseqAccession As String

        ''' <summary>
        ''' assembly_unit 属性
        ''' </summary>
        <Field("assembly_unit")>
        Public Property AssemblyUnit As String

        ''' <summary>
        ''' length 属性
        ''' </summary>
        <Field("length")>
        Public Property Length As Integer?

        ''' <summary>
        ''' genbank_accession 属性
        ''' </summary>
        <Field("genbank_accession")>
        Public Property GenbankAccession As String

        ''' <summary>
        ''' gc_count 属性
        ''' </summary>
        <Field("gc_count")>
        Public Property GcCount As String

        ''' <summary>
        ''' gc_percent 属性
        ''' </summary>
        <Field("gc_percent")>
        Public Property GcPercent As Single?

        ''' <summary>
        ''' unlocalized_count 属性
        ''' </summary>
        <Field("unlocalized_count")>
        Public Property UnlocalizedCount As Integer?

        ''' <summary>
        ''' assembly_unplaced_count 属性
        ''' </summary>
        <Field("assembly_unplaced_count")>
        Public Property AssemblyUnplacedCount As Integer?

        ''' <summary>
        ''' role 属性
        ''' </summary>
        <Field("role")>
        Public Property Role As String

        ''' <summary>
        ''' sequence_name 属性
        ''' </summary>
        <Field("sequence_name")>
        Public Property SequenceName As String

    End Class

End Namespace
