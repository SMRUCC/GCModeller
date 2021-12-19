Imports SMRUCC.genomics.Metagenomics

Namespace PICRUSt

    Public Class MetaBinaryWriter

        ''' <summary>
        ''' greengenes id -> bytes offset
        ''' </summary>
        Dim ggIdIndex As Dictionary(Of String, Long)
        ''' <summary>
        ''' biom taxonomy string -> bytes offset
        ''' </summary>
        Dim taxIndex As Dictionary(Of String, Long)

        Sub New(ggTax As Dictionary(Of String, Taxonomy))

        End Sub

    End Class
End Namespace