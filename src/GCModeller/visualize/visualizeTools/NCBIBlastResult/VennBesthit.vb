#Region "Microsoft.VisualBasic::eee10baa07a213b3ce0f6d9dea071a7e, ..\visualize\visualizeTools\NCBIBlastResult\VennBesthit.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Analysis

Namespace NCBIBlastResult

    <PackageNamespace("Venn.Besthit", Publisher:="xie.guigang@live.com")>
    Public Module VennBesthit

        Dim Margin As Integer = 100

        <ExportAPI("Invoke.Drawing")>
        Public Function InvokeDrawing(bh As BestHit,
                                  Optional range_start As String = "",
                                  Optional range_ends As String = "",
                                  Optional custom_orders As String() = Nothing, Optional p As Double = 0.1,
                                  Optional selectList As String() = Nothing,
                                  Optional removeList As String() = Nothing) As Image

            If custom_orders.IsNullOrEmpty Then
                bh.hits = (From item In bh.hits Select item Order By item.QueryName Ascending).ToArray
            End If

            bh = bh.TrimEmpty(p)

            If String.IsNullOrEmpty(range_start) Then
                range_start = bh.hits.First.QueryName
            End If
            If String.IsNullOrEmpty(range_ends) Then
                range_ends = bh.hits.Last.QueryName
            End If

            Dim List As New List(Of HitCollection)
            Dim Start = (From item In bh.hits Where String.Equals(item.QueryName, range_start, StringComparison.OrdinalIgnoreCase) Select item).ToArray
            Dim i As Integer = Array.IndexOf(bh.hits, Start.First)
            For i = i To bh.hits.Length - 1
                Call List.Add(bh.hits(i))
            Next

            Dim TagFont As New Font(FontFace.Ubuntu, 12, FontStyle.Bold)
            Dim Dict = ExpressionMatrix.MatrixDrawing.CreateAlphabetTagSerials((From item In bh.hits.First.Hits Select item.tag).ToArray)
            Dim MaxIdLength = (From s As String In (From item In List Let mat = New String()() {New String() {item.QueryName}, (From nnnnn In item.Hits Select nnnnn.HitName).ToArray} Let id_cols As String() = mat.ToVector Select id_cols).ToArray.ToVector Select s Order By Len(s) Descending).First.MeasureString(TagFont)
            Dim DotSize = New Size(MaxIdLength.Width + 5, MaxIdLength.Height + 10)
            Dim Gr = (New Size((List.First.Hits.Count + 2) * DotSize.Width + 4 * Margin, (List.Count + 8) * DotSize.Height + 2 * Margin + List.First.Hits.Length * (MaxIdLength.Height + 3))).CreateGDIDevice()
            Dim X As Integer = Margin + MaxIdLength.Width, Y As Integer = Margin * 1.3
            Dim Colors = NCBIBlastResult.ColorSchema.IdentitiesColors

            Call Gr.Graphics.DrawString("Orthologs BBH Matrix", New Font(FontFace.Ubuntu, 24), Brushes.Black, New Point(Gr.Width / 2, Margin * 0.5))

            For Each item In Dict
                X += DotSize.Width + 2

                Call Gr.Graphics.DrawString(item.Value, TagFont, Brushes.Black, New Point(X, Y))
            Next

            Y += 20

            For Each item In List
                Y += DotSize.Height + 3
                X = Margin + MaxIdLength.Width

                Call Gr.Graphics.DrawString(item.QueryName, TagFont, Brushes.Black, New Point(Margin, Y))

                For Each sp In item.Hits
                    X += DotSize.Width + 2

                    If Not String.IsNullOrEmpty(sp.HitName) Then
                        Dim cl As Color = Colors.GetColor(sp.Identities)
                        Call Gr.Graphics.FillRectangle(New SolidBrush(cl), New Rectangle(New Point(X, Y), DotSize))
                        Call Gr.Graphics.DrawString(sp.HitName, TagFont, If(cl = Color.Black, Brushes.White, Brushes.Black), New Point(X, Y))
                    End If
                Next
            Next

            X = Margin + MaxIdLength.Width * 2
            Y += Margin

            For Each item In Dict
                Call Gr.Graphics.DrawString(item.Value & "  --> " & item.Key, TagFont, Brushes.Black, New Point(X, Y))
                Y += MaxIdLength.Height + 2
            Next

            X = Margin
            Y = Gr.Height - 12 * MaxIdLength.Height

            Call Gr.Graphics.DrawString("Color key for alignment identities", TagFont, Brushes.Black, New Point(Margin, Y - MaxIdLength.Height * 2))

            For Each Line In Colors.Values
                Call Gr.Graphics.FillRectangle(New SolidBrush(Line.Value), New Rectangle(New Point(X, Y), DotSize))
                Call Gr.Graphics.DrawString(Line.Name, TagFont, Brushes.Black, X + DotSize.Width + 10, Y + 3) : Y += DotSize.Height + 5
            Next

            Return Gr.ImageResource
        End Function
    End Module
End Namespace
