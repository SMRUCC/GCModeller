Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.KEGG.WebServices.KGML

    ''' <summary>
    ''' + pathway map: http://www.kegg.jp/kegg-bin/download?entry=xcb00280&amp;format=kgml
    ''' </summary>
    Public Class pathway

        <XmlAttribute> Public Property name As String
        <XmlAttribute> Public Property org As String
        <XmlAttribute> Public Property number As String
        <XmlAttribute> Public Property title As String
        <XmlAttribute> Public Property image As String
        <XmlAttribute> Public Property link As String

#Region "pathway network"
        <XmlElement(NameOf(entry))> Public Property items As entry()
        <XmlElement(NameOf(relation))> Public Property relations As relation()
        <XmlElement(NameOf(reaction))> Public Property reactions As reaction()
#End Region

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function ResourceURL(entry As String) As String
            Return $"http://www.kegg.jp/kegg-bin/download?entry={entry}&format=kgml"
        End Function

        Public Overrides Function ToString() As String
            Return $"[{name}] {title}"
        End Function
    End Class

    ''' <summary>
    ''' Network edges
    ''' </summary>
    Public Class Link

        <XmlAttribute> Public Property entry1 As String
        <XmlAttribute> Public Property entry2 As String
        <XmlAttribute> Public Property type As String

    End Class

    Public Class relation : Inherits Link
        Public Property subtype As subtype
    End Class

    Public Class subtype
        <XmlAttribute> Public Property name As String
        <XmlAttribute> Public Property value As String
    End Class

    Public Class compound
        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property name As String

        Public Overrides Function ToString() As String
            Return name
        End Function
    End Class

    Public Class reaction : Inherits Link
        <XmlElement("substrate")>
        Public Property substrates As compound()
        <XmlElement("product")>
        Public Property products As compound()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    ''' <summary>
    ''' Network nodes
    ''' </summary>
    Public Class entry

        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property name As String
        <XmlAttribute> Public Property type As String
        <XmlAttribute> Public Property link As String

        Public Property graphics As graphics

        Public Overrides Function ToString() As String
            Return name
        End Function
    End Class

    Public Class graphics

        <XmlAttribute> Public Property name As String
        <XmlAttribute> Public Property fgcolor As String
        <XmlAttribute> Public Property bgcolor As String
        <XmlAttribute> Public Property type As String
        <XmlAttribute> Public Property x As String
        <XmlAttribute> Public Property y As String
        <XmlAttribute> Public Property width As String
        <XmlAttribute> Public Property height As String

        Public Overrides Function ToString() As String
            Return name
        End Function
    End Class
End Namespace