Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection

''' <summary>
''' Tsv table reader for string-db export result ``string_network_coordinates.txt``
''' </summary>
Public Class Coordinates

    <Column("#node")> Public Property node As String
    Public Property x_position As Double
    Public Property y_position As Double
    Public Property color As String
    Public Property annotation As String

    Public Overrides Function ToString() As String
        Return annotation
    End Function
End Class
