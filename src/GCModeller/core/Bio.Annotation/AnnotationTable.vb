Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

Public Class AnnotationTable : Inherits DynamicPropertyBase(Of String)
    Implements INamedValue

    Public Property ID As String Implements IKeyedEntity(Of String).Key
    Public Property geneName As String
    Public Property ORF As String
    Public Property Entrez As String()
    Public Property fullName As String()
    Public Property uniprot As String()
    Public Property GO As String()
    Public Property EC As String()
    Public Property KO As String()
    Public Property pfam As String
    Public Property organism As String

End Class
