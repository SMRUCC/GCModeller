Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy

Public Class ncbi_taxomony

    ''' <summary>
    ''' NCBI taxonomy id, can be using for build taxonomy lineage from <see cref="NcbiTaxonomyTree"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property Accession As String
    ''' <summary>
    ''' <see cref="SeqHeader.species"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property Name As String
    Public Property Description As String
End Class