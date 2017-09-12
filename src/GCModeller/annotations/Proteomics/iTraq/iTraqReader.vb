Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection

''' <summary>
''' iTraq data reader
''' </summary>
Public Class iTraqReader : Inherits DataSet
    Implements INamedValue

    <Column("Accession")>
    Public Overrides Property ID As String Implements IKeyedEntity(Of String).Key
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
        Return $"{ID} {Description}"
    End Function
End Class
