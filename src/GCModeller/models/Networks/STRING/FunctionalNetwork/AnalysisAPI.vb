#Region "Microsoft.VisualBasic::ecefcc0d7fe3809a13b436994558b356, ..\GCModeller\models\Networks\STRING\FunctionalNetwork\AnalysisAPI.vb"

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

Imports System.Drawing
Imports System.Runtime.CompilerServices
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
                                     Optional radius$ = "5,30") As (model As NetworkTables, image As Image)

        Dim model = stringNetwork _
            .BuildModel(uniprot:=annotations,
                        groupValues:=FunctionalNetwork.KOGroupTable)
        Call model.ComputeNodeDegrees
        Call model.RenderDEGsColorSchema(DEGs, (up:=ColorBrewer.SequentialSchemes.RdPu9, down:=ColorBrewer.SequentialSchemes.YlGnBu9),)

        With model.VisualizeKEGG(
            layouts.ToArray,
            size:="4000,3000",
            scale:=2.5,
            radius:=radius,
            groupLowerBounds:=4)

            Return (model, .ref)
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
                                     .Xrefs(InteractExports.STRING) _
                                     .Select(Function(link) link.id) _
                                     .ToArray
                             End Function)
        Return Function(list As Dictionary(Of String, Double))
                   Return list _
                       .Where(Function(id) uniprotSTRING.ContainsKey(id.Key)) _
                       .Select(Function(id) uniprotSTRING(id.Key).Select(Function(id2) (id2:=id2, log2FC:=id.Value))) _
                       .IteratesALL _
                       .GroupBy(Function(x) x.id2) _
                       .ToDictionary(Function(x) x.Key,
                                     Function(x) Aggregate n In x Into Average(n.log2FC))
               End Function
    End Function
End Module

