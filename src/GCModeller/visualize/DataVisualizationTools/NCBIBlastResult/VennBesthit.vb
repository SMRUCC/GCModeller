#Region "Microsoft.VisualBasic::05490ad9afc424307f69147e8636565d, visualize\DataVisualizationTools\NCBIBlastResult\VennBesthit.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:

    '     Module VennBesthit
    ' 
    '         Function: CreateAlphabetTagSerials, InvokeDrawing
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Tasks.Models

Namespace NCBIBlastResult

    <Package("Venn.Besthit", Publisher:="xie.guigang@live.com")>
    Public Module VennBesthit

        Const ALPHABET As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="dat">数据必须是已经去除掉了重复的</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CreateAlphabetTagSerials(dat As String()) As Dictionary(Of String, String)
            If dat.Count <= 26 Then
                Return (From I As Integer In dat.Sequence Let s As String = dat(I) Select Tag = ALPHABET(I).ToString, s).ToArray.ToDictionary(Function(item) item.s, Function(item) item.Tag)
            Else
                Dim ChunkBuffer = dat.CreateSlideWindows(26)
                Dim list = New List(Of KeyValuePair(Of String, String))
                Dim prefix As String = ""

                For i As Integer = 0 To ChunkBuffer.Count - 1
                    dat = ChunkBuffer(i).Items
                    Call list.AddRange((From j As Integer In dat.Sequence Let s As String = dat(j) Select New KeyValuePair(Of String, String)(s, prefix & ALPHABET(j).ToString)).ToArray)
                    prefix &= ALPHABET(i)
                Next

                Return list.ToDictionary(Function(item) item.Key, elementSelector:=Function(item) item.Value)
            End If
        End Function

        Dim Margin As Integer = 100

        <ExportAPI("Invoke.Drawing")>
        <Extension>
        Public Function InvokeDrawing(bh As SpeciesBesthit,
                                      Optional range_start$ = "",
                                      Optional range_ends$ = "",
                                      Optional custom_orders$() = Nothing,
                                      Optional p# = 0.1,
                                      Optional selectList$() = Nothing,
                                      Optional removeList$() = Nothing,
                                      Optional tagFontCSS$ = CSSFont.UbuntuBold,
                                      Optional title$ = "Orthologs BBH Matrix") As Image

            If custom_orders.IsNullOrEmpty Then
                bh.hits = LinqAPI.Exec(Of HitCollection) <=
                    From hit As HitCollection
                    In bh.hits
                    Select hit
                    Order By hit.QueryName Ascending
            End If

            bh = bh.TrimEmpty(p)

            If String.IsNullOrEmpty(range_start) Then
                range_start = bh.hits.First.QueryName
            End If
            If String.IsNullOrEmpty(range_ends) Then
                range_ends = bh.hits.Last.QueryName
            End If

            Dim list As New List(Of HitCollection)
            Dim start = LinqAPI.DefaultFirst(Of HitCollection) _
 _
                () <= From hit As HitCollection
                      In bh.hits
                      Where String.Equals(hit.QueryName, range_start, StringComparison.OrdinalIgnoreCase)
                      Select hit

            Dim i% = Array.IndexOf(bh.hits, start)

            For i = i To bh.hits.Length - 1
                Call list.Add(bh.hits(i))
            Next

            Dim TagFont As Font = CSSFont.TryParse(tagFontCSS).GDIObject
            Dim table = CreateAlphabetTagSerials(bh.hits.First.hits.Select(Function(h) h.tag).ToArray)
            Dim maxIdLength = (From hits As HitCollection
                               In list
                               Let mat = {
                                   New String() {hits.QueryName},
                                   (From nnnnn In hits.hits Select nnnnn.hitName).ToArray
                               }
                               Let id_cols As String() = mat.ToVector
                               Select id_cols).ToVector _
                                              .MaxLengthString _
                                              .MeasureSize(New Size(1, 1).CreateGDIDevice, TagFont)

            Dim dotSize As New Size(maxIdLength.Width + 5, maxIdLength.Height + 10)
            Dim devSize As New Size(
                (list.First.hits.Count + 2) * dotSize.Width + 4 * Margin,
                (list.Count + 8) * dotSize.Height + 2 * Margin + list.First.hits.Length * (maxIdLength.Height + 3))

            Using g As Graphics2D = devSize.CreateGDIDevice()
                Dim X As Integer = Margin + maxIdLength.Width, Y As Integer = Margin * 1.3
                Dim Colors = NCBIBlastResult.ColorSchema.IdentitiesChromatic

                Call g.DrawString(title, New Font(FontFace.Ubuntu, 24), Brushes.Black, New Point(g.Width / 2, Margin * 0.5))

                For Each item In table
                    X += dotSize.Width + 2

                    Call g.DrawString(item.Value, TagFont, Brushes.Black, New Point(X, Y))
                Next

                Y += 20

                For Each item In list
                    Y += dotSize.Height + 3
                    X = Margin + maxIdLength.Width

                    Call g.DrawString(item.QueryName, TagFont, Brushes.Black, New Point(Margin, Y))

                    For Each sp In item.hits
                        X += dotSize.Width + 2

                        If Not String.IsNullOrEmpty(sp.hitName) Then
                            Dim cl As Color = Colors.GetColor(sp.identities)
                            Call g.FillRectangle(New SolidBrush(cl), New Rectangle(New Point(X, Y), dotSize))
                            Call g.DrawString(sp.hitName, TagFont, If(cl = Color.Black, Brushes.White, Brushes.Black), New Point(X, Y))
                        End If
                    Next
                Next

                X = Margin + maxIdLength.Width * 2
                Y += Margin

                For Each item In table
                    Call g.DrawString(item.Value & "  --> " & item.Key, TagFont, Brushes.Black, New Point(X, Y))
                    Y += maxIdLength.Height + 2
                Next

                X = Margin
                Y = g.Height - 12 * maxIdLength.Height

                Call g.DrawString("Color key for alignment identities", TagFont, Brushes.Black, New Point(Margin, Y - maxIdLength.Height * 2))

                For Each Line In Colors.Values
                    Call g.FillRectangle(New SolidBrush(Line.Value), New Rectangle(New Point(X, Y), dotSize))
                    Call g.DrawString(Line.Name, TagFont, Brushes.Black, X + dotSize.Width + 10, Y + 3) : Y += dotSize.Height + 5
                Next

                Return g.ImageResource
            End Using
        End Function
    End Module
End Namespace
