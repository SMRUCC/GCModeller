Imports System.Xml.Serialization

Public Class OrthologyLink

    <XmlAttribute>
    Public Property Tuple As String()

    Sub New()
    End Sub

    Sub New(ParamArray geneSet As String())
        Tuple = geneSet
    End Sub

    Public Overrides Function ToString() As String
        Return $"[{Tuple.JoinBy(", ")}]"
    End Function
End Class