Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging.d3js.Layout
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports np = Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Numpy

Namespace Drawing2D.Text.Nudge

    Public Module Adjustment

        ''' <summary>
        ''' function to create text rectangle from xy coordonnee and text to print.
        ''' Added a coef to balance output for some reason
        ''' </summary>
        ''' <param name="xy"></param>
        ''' <param name="text"></param>
        ''' <param name="marge"></param>
        ''' <param name="ax"></param>
        ''' <returns></returns>
        Public Function make_text_rectangle(xy As Double(), text As String, marge As Double(), ax As GraphicsTextHandle) As TextRectangle
            Dim x_scope = ax.get_xlim()(1) - ax.get_xlim()(0)
            Dim y_scope = ax.get_ylim()(1) - ax.get_ylim()(0)
            Dim figwidth = ax.get_figwidth()
            Dim figheight = ax.get_figheight()
            Dim sizing_dict As New Dictionary(Of String, Double) From {
                {"default", 1.1},
                {"a", 1.1},
                {"b", 1.1},
                {"c", 1},
                {"d", 1.1},
                {"e", 1.1},
                {"f", 0.65},
                {"g", 1.1},
                {"h", 1.1},
                {"i", 0.55},
                {"j", 0.55},
                {"k", 1.1},
                {"l", 0.55},
                {"m", 1.54},
                {"n", 1.1},
                {"o", 1.1},
                {"p", 1.1},
                {"q", 1.1},
                {"r", 0.825},
                {"s", 1},
                {"t", 0.65},
                {"u", 1.1},
                {"v", 1.1},
                {"w", 1.3},
                {"x", 1.1},
                {"y", 1.1},
                {"z", 1},
                {"_", 0.9},
                {"'", 0.55},
                {"1", 1.15},
                {"2", 1.15},
                {"3", 1.15},
                {"4", 1.15},
                {"5", 1.15},
                {"6", 1.15},
                {"7", 1.15},
                {"8", 1.15},
                {"9", 1.15},
                {"0", 1.15}
            }

            ' as we compute character size in a x_scope of length 16 and figwidth of 10
            ', we have to rescale the character's size to the targeted figure scale.
            For Each key In sizing_dict.Keys
                sizing_dict(key) = sizing_dict(key) * x_scope / (figwidth * 10)
            Next

            Dim h = 1.5 * x_scope / (figwidth * 10)
            Dim l As Double = 0
            For Each c As Char In text.ToLower
                If Not sizing_dict.ContainsKey(c) Then
                    'raise ValueError("character {} in text not recognize by sizing\
                    'dict. Only lower character without accent are accepted, and only _ and ' are\
                    'accepted for punctuation".format(c))
                    l += sizing_dict("default")
                Else
                    l += sizing_dict(c)
                End If
            Next

            ' print("text : {0}, size : {1}".format(text, str(l)))
            Return New TextRectangle(New PlateRectangle(xy + np.array(marge), l, h), 1, marge)
        End Function

        <Extension>
        Public Function adjust_text(ax As GraphicsTextHandle, Optional add_marge As Boolean = True, Optional arrows As Boolean = False) As Boolean
            If ax.texts.IsNullOrEmpty Then
                Return False
            End If
            'If the Then axis aspect Is Set To equal To keep proportion (For cercle) For example, the x_scope Is going To be multiply by two When we will enlarge 
            'If ax Then.get_aspect() == "equal":
            '	x_scope *= 2
            Dim x_scope = ax.get_xlim()(1) - ax.get_xlim()(0)
            Dim y_scope = ax.get_ylim()(1) - ax.get_ylim()(0)
            Dim figwidth = ax.get_figwidth()
            Dim figheight = ax.get_figheight()
            Dim marge As Vector

            If add_marge Then
                marge = np.array({x_scope / figwidth, y_scope / figheight}) / 15
            Else
                marge = np.array({0, 0})
            End If

            Dim list_tr As New List(Of TextRectangle)

            For Each text As Label In ax.texts
                Call list_tr.Add(make_text_rectangle({text.X, text.Y}, text.text, marge, ax))
            Next

            Dim cloud As New CloudOfTextRectangle(list_tr)
            cloud.arrange_text(arrows)
            For i As Integer = 0 To ax.texts.Length - 1
                Dim tr = cloud.list_tr(i)
                ax.texts(i).X = tr.r.x1(0)
                ax.texts(i).Y = tr.r.x1(1)
            Next

            Return True
        End Function
    End Module
End Namespace