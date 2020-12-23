Imports System.Xml.Serialization

Namespace SVG.CSS

    Public Class feMorphology
        <XmlAttribute> Public Property [operator] As String
        <XmlAttribute> Public Property radius As String
        <XmlAttribute> Public Property [in] As String
        <XmlAttribute> Public Property result As String
    End Class

    Public Class feGaussianBlur
        <XmlAttribute> Public Property [in] As String
        <XmlAttribute> Public Property stdDeviation As String
        <XmlAttribute> Public Property result As String
    End Class

    Public Class feFlood
        <XmlAttribute("flood-color")> Public Property floodColor As String
        <XmlAttribute> Public Property result As String
    End Class

    Public Class feComposite
        <XmlAttribute> Public Property [in] As String
        <XmlAttribute> Public Property in2 As String
        <XmlAttribute> Public Property [operator] As String
        <XmlAttribute> Public Property result As String
    End Class

    Public Class feOffset
        <XmlAttribute> Public Property dx As String
        <XmlAttribute> Public Property dy As String
    End Class

    Public Class feMergeNode
        <XmlAttribute> Public Property [in] As String
    End Class
End Namespace