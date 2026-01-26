Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy
Imports SMRUCC.genomics.ComponentModel

Public Interface ITaxonomyAbundance : Inherits IExpressionValue

    Property ncbi_taxid As UInteger

End Interface

Public Module OTUTableBuilder

    <Extension>
    Public Iterator Function MakeOUTTable(Of T As ITaxonomyAbundance)(samples As IEnumerable(Of NamedCollection(Of T)), taxonomyTree As NcbiTaxonomyTree) As IEnumerable(Of OTUTable)

    End Function

End Module