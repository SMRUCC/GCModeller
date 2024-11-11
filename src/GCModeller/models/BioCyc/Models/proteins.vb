Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

<Xref("proteins.dat")>
Public Class proteins : Inherits Model

    <AttributeField("DBLINKS")>
    Public Property db_xrefs As String()
    <AttributeField("GENE")>
    Public Property gene As String
    <AttributeField("LOCATIONS")>
    Public Property locations As String()

    Public ReadOnly Property db_links As DBLink()
        Get
            Return GetDbLinks(db_xrefs).ToArray
        End Get
    End Property


End Class
