
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

<Xref("genes.dat")>
Public Class genes : Inherits Model

    <AttributeField("ACCESSION-1")>
    Public Property accession1 As String
    <AttributeField("ACCESSION-2")>
    Public Property accession2 As String
    <AttributeField("DBLINKS")>
    Public Property db_xrefs As String()
    <AttributeField("PRODUCT")>
    Public Property product As String

End Class
