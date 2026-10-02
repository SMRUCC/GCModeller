Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

''' <summary>
''' The sliding window delta* profile data row.
''' (CSV column names kept compatible with the legacy ``Site/Sigma/Similarity`` schema)
''' </summary>
''' <remarks>
''' One point of the sliding window delta* profile: the delta-difference between 
''' the genome signature of the local 50kb window and the global average 
''' genome signature.
''' </remarks>
Public Class WindowDelta

    ''' <summary>
    ''' the 0-based start position of the window on the genome
    ''' </summary>
    Public Property site As Integer

    ''' <summary>
    ''' the delta* value of the window (multiplied by 1000 when reported)
    ''' </summary>
    <Column("delta*")> Public Property deltaStar As Double
    ''' <summary>
    ''' the delta-difference similarity level (see <see cref="DeltaSimilarity1998.DeltaStarLevels"/>)
    ''' </summary>
    <Column("similarity")> Public Property level As DeltaSimilarity1998.DeltaStarLevels

    Public Overrides Function ToString() As String
        Return $"[{site}] delta*={deltaStar:F2} ({level.Description})"
    End Function

End Class
