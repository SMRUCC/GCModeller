Imports Field = Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps.DataFrameColumnAttribute

Namespace ARDB

    ''' <summary>
    ''' ``origin_type.tab``
    ''' </summary>
    Public Class origin_type

        <Field(1)> Public Property type_name As String
        <Field(2)> Public Property NCBI_acc As String
        <Field(3)> Public Property resistance_type As String
        <Field(4)> Public Property similarity As String

    End Class

    ''' <summary>
    ''' ``ar_genes.tab``
    ''' </summary>
    Public Class ar_genes

        Public Property NCBI_acc_prot As String
        Public Property type_name As String
        Public Property NCBI_taxon As String
        Public Property NCBI_acc_nucl As String
        Public Property non_redundant As String
        Public Property complete As String

    End Class

    ''' <summary>
    ''' ``resistance_profile.tab``
    ''' </summary>
    Public Class resistance_profile

        Public Property gene_name As String
        Public Property antibiotic As String

    End Class

    ''' <summary>
    ''' ``type2cdd.tab``
    ''' </summary>
    Public Class type2cdd

        Public Property resistance_type As String
        Public Property CDD_ID As String

    End Class

    ''' <summary>
    ''' ``type2cog.tab``
    ''' </summary>
    Public Class type2cog

        Public Property resistance_type As String
        Public Property COG_ID As String

    End Class

    ''' <summary>
    ''' ``type2genus.tab``
    ''' </summary>
    Public Class type2genus

        Public Property resistance_type As String
        Public Property taxon_id As String
        Public Property resistance_genes As String

    End Class

    ''' <summary>
    ''' ``type2ref.tab``
    ''' </summary>
    Public Class type2ref

        Public Property resistance_type As String
        Public Property PubMed_ID As String

    End Class

    ''' <summary>
    ''' ``type2rq.tab``
    ''' </summary>
    Public Class type2rq

        Public Property resistance_type As String
        Public Property confer_name As String

    End Class

    ''' <summary>
    ''' ``class2go.tab``
    ''' </summary>
    Public Class class2go
        Public Property class_name As String
        Public Property GO_ID As String
    End Class

End Namespace