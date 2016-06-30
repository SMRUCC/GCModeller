Imports System.Drawing
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Imaging

Namespace CytoscapeGraphView.XGMML

    Public Class EdgeGraphics : Inherits AttributeDictionary

        <XmlAttribute("width")> Public Property Width As Double
        <XmlAttribute("fill")> Public Property Fill As String

        Public ReadOnly Property LineColor As Color
            Get
                Dim Hex As String = Mid(Fill, 2)
                Dim alpha = Me("EDGE_TRANSPARENCY")
                Dim r = CytoscapeColor.HexToARGB(Hex, If(alpha Is Nothing, 255, Val(alpha.Value)))
                Return r
            End Get
        End Property

        Public ReadOnly Property LabelSize As Integer
            Get
                Dim attr = Me("EDGE_LABEL_FONT_SIZE")
                If attr Is Nothing Then
                    Return 10
                Else
                    Return Val(attr.Value)
                End If
            End Get
        End Property

        Public Function GetLabelFont(Scale As Double) As Font
            Dim size = Scale * LabelSize
            Dim Font = Me("EDGE_LABEL_FONT_FACE")
            If Font Is Nothing Then
                Return New Font(FontFace.MicrosoftYaHei, size)
            Else
                Return New Font(Font.Value.Split("."c).First, size)
            End If
        End Function

        Public ReadOnly Property FontColor As Color
            Get
                Dim clattr = Me("EDGE_LABEL_COLOR")
                Dim alpha = Me("EDGE_LABEL_TRANSPARENCY")

                If clattr Is Nothing Then
                    Return Color.Black
                End If

                Return CytoscapeColor.HexToARGB(Mid(clattr.Value, 2), If(alpha IsNot Nothing, Val(alpha.Value), 255))
            End Get
        End Property

        Public ReadOnly Property EdgeLabel As String
            Get
                Dim attr = Me("EDGE_LABEL")
                If attr Is Nothing Then
                    Return ""
                Else
                    Return attr.Value
                End If
            End Get
        End Property
    End Class
End Namespace