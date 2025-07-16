Imports System.Xml.Serialization

Namespace v2

    Public Class protein

        <XmlAttribute> Public Property protein_id As String

        <XmlAttribute> Public Property name As String

        Public Property ligand As String()
        Public Property peptide_chains As String()

        Public Property note As String

    End Class
End Namespace