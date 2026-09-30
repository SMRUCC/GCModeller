Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

''' <summary>
''' The sliding window delta* profile data row.
''' (CSV column names kept compatible with the legacy ``Site/Sigma/Similarity`` schema)
''' </summary>
''' <remarks></remarks>
Public Class SiteSigma
    <Column("Site")> Public Property Site As Integer
    ''' <summary>
    ''' the delta* value of the window (multiplied by 1000 when reported)
    ''' </summary>
    <Column("Sigma")> Public Property DeltaStar As Double
    ''' <summary>
    ''' the delta-difference similarity level (see <see cref="DeltaSimilarity1998.DeltaStarLevels"/>)
    ''' </summary>
    <Column("Similarity")> Public Property Level As DeltaSimilarity1998.DeltaStarLevels
End Class
