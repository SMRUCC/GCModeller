Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language

Public Module Extensions

    Const COGNotAssign As String = "COG_NOT_ASSIGN"

    ''' <summary>
    ''' 绘制COG分类的颜色，请注意，对于没有COG颜色分类的，情使用空字符串来替代
    ''' </summary>
    ''' <param name="gdi"></param>
    ''' <remarks></remarks>
    ''' 
    <Extension> Public Sub DrawingCOGColors(ByRef gdi As Graphics,
                                            COGsColor As Dictionary(Of String, Brush),
                                            ref As Point,
                                            legendFont As Font,
                                            width As Integer,
                                            margin As Integer)

        Dim top As Integer = ref.Y - 100
        Dim left As Integer = ref.X
        Dim legendHeight As Integer = 20
        Dim FontHeight As Single = gdi.MeasureString("COG", legendFont).Height
        Dim d As Single = (legendHeight - FontHeight) / 2
        Dim colors As List(Of KeyValuePair(Of String, Brush)) =
            LinqAPI.MakeList(Of KeyValuePair(Of String, Brush)) <=
                From x As KeyValuePair(Of String, Brush)
                In COGsColor
                Where Not String.IsNullOrEmpty(x.Key)
                Select x
                Order By x.Key Ascending

        If COGsColor.ContainsKey("") Then
            Dim Cl As Brush = COGsColor("")
            Call colors.Add(COGNotAssign, Cl)
        End If

        Dim regn As Region
        Dim rect As Rectangle

        For Each color As KeyValuePair(Of String, Brush) In colors

            rect = New Rectangle(New Point(left, top),
                                 New Size(100, legendHeight))
            regn = New Region(rect)

            Call gdi.FillRegion(color.Value, regn)
            Call gdi.DrawString(color.Key,
                                legendFont,
                                Brushes.Black,
                                New Point(left + 110, top + d))

            left += 120 + gdi.MeasureString(color.Key, legendFont).Width

            If left >= width - margin Then
                left = margin
                top += 2 * legendHeight
            End If
        Next
    End Sub
End Module
