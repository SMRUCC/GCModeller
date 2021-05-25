#Region "Microsoft.VisualBasic::b209dea9c1a67cd3f508d5dc15caca64, visualize\DataVisualizationTools\GC.vb"

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

' Module GCPlot
' 
'     Function: (+2 Overloads) PlotGC, PlotGCContent, PlotGCSkew
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcidStaticsProperty
Imports SMRUCC.genomics.SequenceModel.Patterns
Imports ColorPattern = Microsoft.VisualBasic.Imaging.ColorMap

Public Module GCPlot

    <Extension>
    Public Function PlotGCSkew(mal As FastaFile, cal As NtProperty, winSize%, steps%, Optional isCircle? As Boolean = True) As GraphicsData
        Return mal.PlotGC(AddressOf NucleicAcidStaticsProperty.GCSkew, winSize, steps, isCircle)
    End Function

    <Extension>
    Public Function PlotGCContent(mal As FastaFile, cal As NtProperty, winSize%, steps%, Optional isCircle? As Boolean = True) As GraphicsData
        Return mal.PlotGC(AddressOf GCContent, winSize, steps, isCircle)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="mal"></param>
    ''' <param name="plot$">gcskew/gccontent</param>
    ''' <param name="winSize%"></param>
    ''' <param name="steps%"></param>
    ''' <param name="isCircle"></param>
    ''' <param name="size"></param>
    ''' <param name="margin"></param>
    ''' <param name="bg$"></param>
    ''' <param name="colors$"></param>
    ''' <param name="levels%"></param>
    ''' <returns></returns>
    <Extension>
    Public Function PlotGC(mal As FastaFile, plot$,
                           winSize%,
                           steps%,
                           Optional isCircle? As Boolean = True,
                           Optional size As Size = Nothing,
                           Optional margin As Size = Nothing,
                           Optional bg$ = "white",
                           Optional colors$ = "Jet",
                           Optional levels% = 100,
                           Optional ref$ = "0") As GraphicsData

        If plot.TextEquals("gcskew") Then
            Return mal.PlotGC(AddressOf NucleicAcidStaticsProperty.GCSkew, winSize, steps, isCircle)
        ElseIf plot.TextEquals("variation") Then
            Dim refIndex = mal.Index(ref)
            Dim v As New Variation(mal(refIndex)) With {
                .Strict = False
            }
            Call mal.RemoveAt(refIndex)
            Return mal.PlotGC(AddressOf v.NtVariation, winSize, steps, isCircle, base:="white")
        Else
            Return mal.PlotGC(AddressOf GCContent, winSize, steps, isCircle)
        End If
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="mal"></param>
    ''' <param name="cal"></param>
    ''' <param name="winSize%"></param>
    ''' <param name="steps%"></param>
    ''' <param name="isCircle"></param>
    ''' <param name="size"></param>
    ''' <param name="bg$"></param>
    ''' <param name="colors$"></param>
    ''' <param name="levels%"></param>
    ''' <param name="base$">color code for ZERO level</param>
    ''' <returns></returns>
    <Extension>
    Public Function PlotGC(mal As FastaFile, cal As NtProperty,
                           winSize%,
                           steps%,
                           Optional isCircle? As Boolean = True,
                           Optional size As Size = Nothing,
                           Optional padding$ = "padding: 100 350 100 350",
                           Optional bg$ = "white",
                           Optional colors$ = "Jet",
                           Optional levels% = 100,
                           Optional base$ = Nothing) As GraphicsData

        Dim ntArray As NamedValue(Of Double())() = LinqAPI.Exec(Of NamedValue(Of Double())) <=
 _
            From seq As FastaSeq
            In mal
            Select New NamedValue(Of Double()) With {
                .Name = seq.Title,
                .Value = cal(seq, winSize, steps, isCircle)
            }

        Dim tick As New Font(FontFace.SegoeUI, 12)
        Dim label As New Font(FontFace.MicrosoftYaHei, 8)
        Dim v%() = ntArray _
            .Select(Function(s) s.Value) _
            .IteratesALL _
            .GenerateMapping(levels, offset:=0)
        Dim lvMAT As NamedValue(Of Integer())() = v _
            .Split(ntArray(Scan0).Value.Length) _
            .SeqIterator _
            .Select(Function(i) New NamedValue(Of Integer()) With {
                .Name = ntArray(i).Name,
                .Value = i.value
            })
        Dim mapColors As Color() = New ColorPattern(levels * 2 + 1).ColorSequence(colors)
        Dim margin As Padding = padding

        If size.IsEmpty Then
            size = New Size(30000, 15000)
        End If

        If Not String.IsNullOrEmpty(base) Then
            mapColors(Scan0) = base.ToColor
        End If

        Call $"max:={v.Max}, min:={v.Min}".__DEBUG_ECHO

        Dim plotInternal =
            Sub(ByRef g As IGraphics, region As GraphicsRegion)
                Dim plotWidth = region.PlotRegion.Width
                Dim y! = margin.Top
                Dim deltaX! = plotWidth / lvMAT(Scan0).Value.Length
                Dim deltaY! = region.PlotRegion.Height / lvMAT.Length
                Dim plotTick As Boolean = True

                For Each line As NamedValue(Of Integer()) In lvMAT

                    Dim x! = margin.Left
                    Dim bp% = 1

                    For Each d As Integer In line.Value
                        If d > mapColors.Length - 1 Then
                            d = mapColors.Length - 1
                        End If

                        Dim b As New SolidBrush(mapColors(d))
                        Dim rect As New RectangleF(New PointF(x!, y!), New SizeF(deltaX, deltaY))
                        Call g.FillRectangle(b, rect)

                        If plotTick Then
                            Call g.DrawString(bp, tick, Brushes.Black, New Point(x, margin.Top - 20))
                            bp += steps
                        End If

                        x += deltaX
                    Next

                    Call g.DrawString(line.Name, label, Brushes.Black, New PointF(1, y))

                    plotTick = False
                    y += deltaY
                Next
            End Sub

        Return g.GraphicsPlots(size, margin, bg, plotInternal)
    End Function
End Module
