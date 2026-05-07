' ============================================================================
' V2reportsANIMatch.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsANIMatch
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsANIMatch

        ''' <summary>
        ''' assembly 属性
        ''' </summary>
        <Field("assembly")>
        Public Property Assembly As String

        ''' <summary>
        ''' organism_name 属性
        ''' </summary>
        <Field("organism_name")>
        Public Property OrganismName As String

        ''' <summary>
        ''' category 属性
        ''' </summary>
        <Field("category")>
        Public Property Category As Object

        ''' <summary>
        ''' ani 属性
        ''' </summary>
        <Field("ani")>
        Public Property Ani As Single?

        ''' <summary>
        ''' assembly_coverage 属性
        ''' </summary>
        <Field("assembly_coverage")>
        Public Property AssemblyCoverage As Single?

        ''' <summary>
        ''' type_assembly_coverage 属性
        ''' </summary>
        <Field("type_assembly_coverage")>
        Public Property TypeAssemblyCoverage As Single?

    End Class

End Namespace
