
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

<Xref("regulation.dat")>
Public Class regulation : Inherits Model

    <AttributeField("MODE")>
    Public Property mode As String
    <AttributeField("REGULATED-ENTITY")>
    Public Property regulatedEntity As String
    <AttributeField("REGULATOR")>
    Public Property regulator As String

End Class
