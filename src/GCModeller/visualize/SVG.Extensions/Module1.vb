Module Module1

    Sub Main()



        Dim doc As New SVG.SvgDocument

        Dim box As New SVG.SvgRectangle() With {.X = New SvgUnit(100), .Y = New SvgUnit(100), .Width = New SvgUnit(100), .Height = New SvgUnit(100)}

        Call doc.Children.Add(box)


        Call doc.Write("x:\sfsdf.svg")

    End Sub

End Module
