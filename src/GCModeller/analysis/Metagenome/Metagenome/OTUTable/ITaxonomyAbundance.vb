Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.ComponentModel

''' <summary>
''' [id, ncbi_taxid, expression_value]
''' </summary>
Public Interface ITaxonomyAbundance : Inherits IExpressionValue

    Property ncbi_taxid As UInteger

End Interface

Public Class ContigResult : Implements ITaxonomyAbundance

    Public Property contig_id As String Implements IReadOnlyId.Identity
    Public Property ncbi_taxid As UInteger Implements ITaxonomyAbundance.ncbi_taxid
    Public Property expression_value As Double Implements IExpressionValue.ExpressionValue

End Class

Public Interface ITaxonomy

    Property ncbi_taxid As UInteger
    Property taxonomy_string As String

End Interface
