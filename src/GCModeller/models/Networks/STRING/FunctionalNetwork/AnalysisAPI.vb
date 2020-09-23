#Region "Microsoft.VisualBasic::d0bb8ea6a94cfe2c28626746425f61a8, models\Networks\STRING\FunctionalNetwork\AnalysisAPI.vb"

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
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.STRING
Imports SMRUCC.genomics.Model.Network.KEGG
Imports NetworkTables = Microsoft.VisualBasic.Data.visualize.Network.FileStream.NetworkTables

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
                                     Optional canvasSize$ = "1920,1080") As (model As NetworkTables, image As Image)

        Dim colorLevels = (up:=ColorBrewer.SequentialSchemes.RdPu9, down:=ColorBrewer.SequentialSchemes.YlGnBu9)
        Dim model = stringNetwork _
            .BuildModel(uniprot:=annotations,
                        groupValues:=FunctionalNetwork.KOGroupTable
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
