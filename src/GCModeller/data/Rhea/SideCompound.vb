
Imports System.Xml.Serialization

Public Class SideCompound

    <XmlAttribute>
    Public Property side As String
    Public Property compound As Compound

    Sub New(side As String, compound As Compound)
        _side = side
        _compound = compound
    End Sub

    Sub New()
    End Sub

    Public Overrides Function ToString() As String
        Return compound.ToString
    End Function

End Class