Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.STRING
Imports NetGraph = Microsoft.VisualBasic.Data.visualize.Network.FileStream.Network

Public Module FunctionalEnrichmentPlot

    <Extension>
    Public Function BuildModel(interacts As IEnumerable(Of InteractExports), uniprot As UniprotXML) As NetGraph
        Dim KOCatagory = PathwayMapping.DefaultKOTable
        Dim stringUniprot =
            uniprot _
            .entries _
            .Select(Function(protein)
                        Return protein.dbReferences _
                            .SafeQuery _
                            .Where(Function(link) link.type = InteractExports.STRING) _
                            .Select(Function(link)
                                        Dim KO$() = protein _
                                            .Xrefs _
                                            .TryGetValue("KO") _
                                            .SafeQuery _
                                            .Select(Function(k) k.id) _
                                            .ToArray
                                        Return (
                                            stringID:=link.id,
                                            uniprotID:=protein.accessions,
                                            KO:=KO)
                                    End Function) _
                            .ToArray
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(l) l.stringID) _
            .ToDictionary(Function(prot) prot.Key,
                          Function(uniprots)
                              Dim uid = uniprots _
                                 .Select(Function(g) g.uniprotID) _
                                 .IteratesALL _
                                 .Distinct _
                                 .ToArray
                              Dim kid = uniprots _
                                 .Select(Function(g) g.KO) _
                                 .IteratesALL _
                                 .Distinct _
                                 .ToArray
                              Return (uniprotID:=uid, KO:=kid)
                          End Function)
        Dim nodes = stringUniprot _
            .Select(Function(protein)
                        Dim type = KOCatagory _
                            .TryGetValue(protein.Value.KO.FirstOrDefault, [default]:=Nothing) _
                            ?.Parent _
                            ?.Description
                        Dim values As New Dictionary(Of String, String)

                        Return New Node With {
                            .ID = protein.Key,
                            .NodeType = type,
                            .Properties = values
                        }
                    End Function) _
            .ToArray
        Dim links = interacts _
            .Select(Function(l)
                        Return New NetworkEdge With {
                            .FromNode = l.node1_external_id,
                            .ToNode = l.node2_external_id,
                            .value = l.combined_score
                        }
                    End Function).ToArray

        Return New NetGraph(links, nodes)
    End Function
End Module
