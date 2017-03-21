Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' Shotgun data reader
''' </summary>
Public Class ShotgunData

    Public Property Accession As String
    Public Property Description As String
    Public Property Score As String
    Public Property Coverage As String
    <Column("# Proteins")> Public Property Proteins As String
    <Column("# Unique Peptides")> Public Property UniquePeptides As String
    <Column("# Peptides")> Public Property Peptides As String
    <Column("# PSMs")> Public Property PSMs As String
    <Column("# AAs")> Public Property AAs As String
    <Column("MW [kDa]")> Public Property MW As String
    <Column("calc. pI")> Public Property calcPI As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class
