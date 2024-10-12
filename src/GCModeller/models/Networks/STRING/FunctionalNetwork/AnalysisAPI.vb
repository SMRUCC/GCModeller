﻿#Region "Microsoft.VisualBasic::781bafc2bc46ca2356d037cdeb6a8a18, models\Networks\STRING\FunctionalNetwork\AnalysisAPI.vb"

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


    ' Code Statistics:

    '   Total Lines: 76
    '    Code Lines: 67 (88.16%)
    ' Comment Lines: 3 (3.95%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 6 (7.89%)
    '     File Size: 3.59 KB


    ' Module AnalysisAPI
    ' 
    '     Function: NetworkVisualize, Uniprot2STRING
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.STRING
Imports SMRUCC.genomics.Model.Network.KEGG
Imports SMRUCC.genomics.Model.Network.KEGG.GraphVisualizer

#If NET48 Then
Imports Pen = System.Drawing.Pen
Imports Pens = System.Drawing.Pens
Imports Brush = System.Drawing.Brush
Imports Font = System.Drawing.Font
Imports Brushes = System.Drawing.Brushes
Imports SolidBrush = System.Drawing.SolidBrush
Imports DashStyle = System.Drawing.Drawing2D.DashStyle
Imports Image = System.Drawing.Image
Imports Bitmap = System.Drawing.Bitmap
Imports GraphicsPath = System.Drawing.Drawing2D.GraphicsPath
Imports FontStyle = System.Drawing.FontStyle
Imports LineCap = System.Drawing.Drawing2D.LineCap
#Else
Imports Pen = Microsoft.VisualBasic.Imaging.Pen
Imports Pens = Microsoft.VisualBasic.Imaging.Pens
Imports Brush = Microsoft.VisualBasic.Imaging.Brush
Imports Font = Microsoft.VisualBasic.Imaging.Font
Imports Brushes = Microsoft.VisualBasic.Imaging.Brushes
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush
Imports DashStyle = Microsoft.VisualBasic.Imaging.DashStyle
Imports Image = Microsoft.VisualBasic.Imaging.Image
Imports Bitmap = Microsoft.VisualBasic.Imaging.Bitmap
Imports GraphicsPath = Microsoft.VisualBasic.Imaging.GraphicsPath
Imports FontStyle = Microsoft.VisualBasic.Imaging.FontStyle
Imports LineCap = Microsoft.VisualBasic.Imaging.LineCap
#End If


''' <summary>
''' Functional network analysis based on the ``STRING-db``.
''' </summary>
Public Module AnalysisAPI

    <Extension>
    Public Function NetworkVisualize(stringNetwork As IEnumerable(Of InteractExports),
                                     annotations As Dictionary(Of String, entry),
                                     DEGs As (UP As Dictionary(Of String, Double), down As Dictionary(Of String, Double)),
                                     Optional layouts As IEnumerable(Of Coordinates) = Nothing,
                                     Optional radius$ = "5,30",
                                     Optional canvasSize$ = "1920,1080") As (model As NetworkGraph, image As Image)

        Dim colorLevels = (up:=ColorBrewer.SequentialSchemes.RdPu9, down:=ColorBrewer.SequentialSchemes.YlGnBu9)
        Dim model As NetworkGraph = stringNetwork _
            .BuildModel(uniprot:=annotations,
                        groupValues:=SimpleBuilder.KOGroupTable
            )
        Call model.ComputeNodeDegrees
        Call model.RenderDEGsColorSchema(DEGs, colorLevels,)

        With model.VisualizeKEGG(
            layouts.ToArray,
            size:=canvasSize,
            radius:=radius,
            groupLowerBounds:=4
        )

            Return (model, .ByRef)
        End With
    End Function

    <Extension>
    Public Function Uniprot2STRING(annotations As Dictionary(Of String, entry)) As Func(Of Dictionary(Of String, Double), Dictionary(Of String, Double))
        Dim uniprotSTRING = annotations.Values _
               .Distinct _
               .Select(Function(protein)
                           Return protein.accessions.Select(Function(unid) (unid, protein))
                       End Function) _
               .IteratesALL _
               .GroupBy(Function(x) x.Item1) _
               .ToDictionary(Function(x) x.Key,
                             Function(x)
                                 Return x.First.Item2 _
                                     .xrefs(InteractExports.STRING) _
                                     .Select(Function(link) link.id) _
                                     .ToArray
                             End Function)
        Return Function(list As Dictionary(Of String, Double))
                   Return list _
                       .Where(Function(id) uniprotSTRING.ContainsKey(id.Key)) _
                       .Select(Function(id)
                                   Return uniprotSTRING(id.Key).Select(Function(id2) (id2:=id2, log2FC:=id.Value))
                               End Function) _
                       .IteratesALL _
                       .GroupBy(Function(x) x.id2) _
                       .ToDictionary(Function(x) x.Key,
                                     Function(x)
                                         Return Aggregate n In x Into Average(n.log2FC)
                                     End Function)
               End Function
    End Function
End Module
