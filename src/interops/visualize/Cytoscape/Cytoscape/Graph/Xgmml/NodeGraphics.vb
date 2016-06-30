Imports System.Drawing
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Imaging

Namespace CytoscapeGraphView.XGMML

    Public Class NodeGraphics : Inherits AttributeDictionary

        <XmlAttribute("outline")> Public Property Outline As String
        <XmlAttribute> Public Property z As String
        <XmlAttribute("type")> Public Property Type As String
        <XmlAttribute("fill")> Public Property Fill As String
        <XmlAttribute> Public Property x As Double
        <XmlAttribute("width")> Public Property Width As Double
        <XmlAttribute> Public Property w As Double
        <XmlAttribute> Public Property h As Double
        <XmlAttribute> Public Property y As Double

        Public ReadOnly Property NODE_LABEL_FONT_SIZE As Integer
            Get
                Dim attr = Me.Value("NODE_LABEL_FONT_SIZE")
                If attr Is Nothing OrElse String.IsNullOrEmpty(attr.Value) Then
                    Return Math.Min(w, h)
                Else
                    Return Val(attr.Value)
                End If
            End Get
        End Property

        Public ReadOnly Property FillColor As Color
            Get
                Dim Hex As String = Mid(Fill, 2)
                Dim alpha = Me("NODE_TRANSPARENCY")
                Dim r = CytoscapeColor.HexToARGB(Hex, If(alpha Is Nothing, 255, Val(alpha.Value)))
                Return r
            End Get
        End Property

        Public Function GetLabelFont(Scale As Double) As Font
            Dim fName = Me("NODE_LABEL_FONT_FACE")
            Dim size As Integer = NODE_LABEL_FONT_SIZE * Scale

            If Not fName Is Nothing Then
                Return New Font(fName.Value.Split("."c).FirstOrDefault, size)
            Else
                Return New Font(FontFace.MicrosoftYaHei, size)
            End If
        End Function

        Public ReadOnly Property radius As Single
            Get
                Return Math.Sqrt(w ^ 2 + h ^ 2) / 2
            End Get
        End Property

        Public ReadOnly Property LabelColor As Color
            Get
                Dim clattr = Me("NODE_LABEL_COLOR")
                Dim clattrAlpha = Me("NODE_LABEL_TRANSPARENCY")

                If clattr Is Nothing Then
                    Return Color.Black
                End If

                Return CytoscapeColor.HexToARGB(Mid(clattr.Value, 2), If(clattrAlpha Is Nothing, 255, Val(clattrAlpha.Value)))
            End Get
        End Property
    End Class
End Namespace