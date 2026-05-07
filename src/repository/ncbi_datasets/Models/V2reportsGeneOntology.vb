' ============================================================================
' V2reportsGeneOntology.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsGeneOntology
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsGeneOntology

        ''' <summary>
        ''' assigned_by 属性
        ''' </summary>
        <Field("assigned_by")>
        Public Property AssignedBy As String

        ''' <summary>
        ''' molecular_functions 属性
        ''' </summary>
        <Field("molecular_functions")>
        Public Property MolecularFunctions As List(Of Object)

        ''' <summary>
        ''' biological_processes 属性
        ''' </summary>
        <Field("biological_processes")>
        Public Property BiologicalProcesses As List(Of Object)

        ''' <summary>
        ''' cellular_components 属性
        ''' </summary>
        <Field("cellular_components")>
        Public Property CellularComponents As List(Of Object)

    End Class

End Namespace
