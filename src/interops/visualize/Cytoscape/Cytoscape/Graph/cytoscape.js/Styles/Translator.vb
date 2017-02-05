Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace CytoscapeGraphView.Cyjs.style

    Public Class CSSTranslator

        Dim css As Dictionary(Of String, String)

        Public Property TextOpacity As Integer
            Get
                Return CInt(Val(css.TryGetValue("text-opacity")) * 255)
            End Get
            Set(value As Integer)
                css("text-opacity") = value / 255
            End Set
        End Property

        Public Property BackgroundColor As Color
            Get
                Return css.TryGetValue("background-color").TranslateColor
            End Get
            Set(value As Color)
                css("background-color") = value.RGBExpression
            End Set
        End Property

        Public Property FontSize As Single
            Get
                Return Val(css.TryGetValue("font-size"))
            End Get
            Set(value As Single)
                css("font-size") = value
            End Set
        End Property

        Public Property BackgroundOpacity As Integer
            Get
                Return CInt(Val(css.TryGetValue("background-opacity")) * 255)
            End Get
            Set(value As Integer)
                css("background-opacity") = value / 255
            End Set
        End Property

        Public Property Shape As String
            Get
                Return css("shape")
            End Get
            Set(value As String)
                css("shape") = value
            End Set
        End Property

        Public Property Width As Single
            Get
                Return Val(css("width"))
            End Get
            Set(value As Single)
                css("width") = value
            End Set
        End Property

        Public Property Color As Color
            Get
                Return css.TryGetValue("color").TranslateColor
            End Get
            Set(value As Color)
                css("color") = value.RGBExpression
            End Set
        End Property

        Public Property BorderOpacity As Integer
            Get
                Return Val(css.TryGetValue("border-opacity")) * 255
            End Get
            Set(value As Integer)
                css("border-opacity") = value / 255
            End Set
        End Property

        Sub New(css As Dictionary(Of String, String))
            Me.css = css
        End Sub

        Public Overrides Function ToString() As String
            Return css.GetJson
        End Function
    End Class
End Namespace