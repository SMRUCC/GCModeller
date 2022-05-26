Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports ColorPalette = Microsoft.VisualBasic.Imaging.Drawing2D.Colors.Designer
Imports pathwayBrite = SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry.Pathway
Imports stdVec = Microsoft.VisualBasic.Math.LinearAlgebra.Vector

Namespace CatalogProfiling

    Public Class BubbleTerm

        ''' <summary>
        ''' [X]
        ''' </summary>
        ''' <returns></returns>
        Public Property Factor As Double
        ''' <summary>
        ''' [Y] -log10(p-value)
        ''' </summary>
        ''' <returns></returns>
        Public Property PValue As Double
        ''' <summary>
        ''' bubble radius
        ''' </summary>
        ''' <returns></returns>
        Public Property data As Double
        Public Property termId As String

        Public Shared Function CreateBubbles(logP As stdVec,
                                             Impact As stdVec,
                                             values As stdVec,
                                             pathwayList As String()) As Dictionary(Of String, BubbleTerm())

            Dim bubbleData As New Dictionary(Of String, List(Of BubbleTerm))
            Dim KOmap = pathwayBrite.LoadFromResource.ToDictionary(Function(map) map.EntryId)

            For i As Integer = 0 To pathwayList.Length - 1
                Dim map As pathwayBrite = KOmap(pathwayList(i).Match("\d+"))

                If Not bubbleData.ContainsKey(map.class) Then
                    bubbleData.Add(map.class, New List(Of BubbleTerm))
                End If

                bubbleData(map.class).Add(New BubbleTerm With {
                    .data = values(i),
                    .Factor = Impact(i),
                    .PValue = logP(i),
                    .termId = map.entry.text
                })
            Next

            Return bubbleData _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return a.Value.ToArray
                              End Function)
        End Function

        Public Shared Function CreateEnrichColors(bubbleData As Dictionary(Of String, BubbleTerm()),
                                                  Optional theme As String = "Set1:c8",
                                                  Optional unenrichColor As String = NameOf(Color.LightGray)) As Dictionary(Of String, Color())

            Dim enrichColors As New Dictionary(Of String, Color())
            Dim colorSet As Color() = ColorPalette.GetColors(theme)
            Dim keys As String() = bubbleData.Keys.ToArray
            Dim baseColor As Color = unenrichColor.TranslateColor
            Dim middle As Color

            For i As Integer = 0 To keys.Length - 1
                middle = Color.FromArgb(
                    red:=(baseColor.R + colorSet(i).R) / 2,
                    green:=(baseColor.G + colorSet(i).G) / 2,
                    blue:=(baseColor.B + colorSet(i).B) / 2
                )
                enrichColors(keys(i)) = ColorPalette.CubicSpline({baseColor, middle, colorSet(i)}, 25)
            Next

            Return enrichColors
        End Function

    End Class
End Namespace