Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis
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
                                     DEGs As (UP As String(), down As String()),
                                     Optional layouts As IEnumerable(Of Coordinates) = Nothing,
                                     Optional radius$ = "5,30") As (model As NetworkTables, image As Image)

        Dim model = stringNetwork _
            .BuildModel(uniprot:=annotations,
                        groupValues:=FunctionalNetwork.KOGroupTable)
        Call model.ComputeNodeDegrees
        Call model.RenderDEGsColor(DEGs, (up:="brown", down:="skyblue"),)

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
    Public Function Uniprot2STRING(annotations As Dictionary(Of String, entry)) As Func(Of String(), String())
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
        Return Function(list As String())
                   Return list _
                       .Where(Function(id) uniprotSTRING.ContainsKey(id)) _
                       .Select(Function(id) uniprotSTRING(id)) _
                       .IteratesALL _
                       .Distinct _
                       .ToArray
               End Function
    End Function
End Module
