Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace metaTraits

    Public Class NCBI2GTDB_lineage

        <Column("taxonID NCBI")> Public Property taxonID_NCBI As UInteger

        <Column("taxonID GTDB")> Public Property taxonID_GTDB As UInteger

    End Class
End Namespace