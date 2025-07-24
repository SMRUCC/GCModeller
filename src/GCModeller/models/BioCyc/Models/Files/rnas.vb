
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

<Xref("rnas.dat")>
Public Class rnas : Inherits Model

    <AttributeField("GENE")>
    Public Property gene As String
    <AttributeField("LOCATIONS")>
    Public Property locations As String()
    <AttributeField("REGULATES")>
    Public Property regulates As String()

End Class
