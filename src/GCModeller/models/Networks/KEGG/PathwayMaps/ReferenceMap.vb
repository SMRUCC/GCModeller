Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Namespace PathwayMaps

    Public Module ReferenceMap

        Private Function getCompoundsInMap(map As Map) As IEnumerable(Of NamedValue(Of String))
            Return map.shapes _
                .Select(Function(a) a.Names) _
                .IteratesALL _
                .Where(Function(term)
                           Return term.Name.IsPattern("C\d+")
                       End Function) _
                .Select(Function(c)
                            Return New NamedValue(Of String) With {
                                .Name = c.Name,
                                .Value = c.Value,
                                .Description = map.id
                            }
                        End Function)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Private Function getCompoundIndex(reactionVector As ReactionTable(), getIds As Func(Of ReactionTable, String())) As Dictionary(Of String, ReactionTable())
            Return reactionVector _
                .Select(Function(r)
                            Return getIds(r).Select(Function(cid) (cid, r))
                        End Function) _
                .IteratesALL _
                .GroupBy(Function(t) t.cid) _
                .ToDictionary(Function(c) c.Key,
                              Function(g)
                                  Return g.Select(Function(r) r.r).ToArray
                              End Function)
        End Function

        <Extension>
        Private Function createNodeTable(compounds As IGrouping(Of String, NamedValue(Of String))(), compoundsWithBiologicalRoles As Dictionary(Of String, String)) As Dictionary(Of String, Node)
            Dim nodes As New Dictionary(Of String, Node)
            Dim node As Node
            Dim mapList$

            For Each compound In compounds
                mapList = compound.Select(Function(map) map.Description).JoinBy("|")
                node = New Node With {
                    .ID = compound.Key,
                    .NodeType = "compound",
                    .Properties = New Dictionary(Of String, String) From {
                        {"label", compound.First.Value},
                        {"maps", mapList},
                        {"class", compoundsWithBiologicalRoles.TryGetValue(compound.Key, [default]:="NA")}
                    }
                }

                Call nodes.Add(compound.Key, node)
            Next

            Return nodes
        End Function

        ReadOnly ignores As Index(Of String) = {
            "C00001", ' H2O
            "C00007", ' O2
            "C00011",  ' CO2
            "C00012", ' Peptide
            "C00014",' Ammonia
            "C11481", ' HSO3-
            "C00283", 'Hydrogen sulfide
            "C00094", ' Sulfite
            "C00080" ' H+
        }

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="maps">
        ''' <see cref="MapRepository.ScanMaps(String)"/>
        ''' </param>
        ''' <param name="reactions">
        ''' <see cref="ReactionTable.Load(String)"/>
        ''' </param>
        ''' <returns></returns>
        Public Function BuildNetworkModel(maps As IEnumerable(Of Map), reactions As IEnumerable(Of ReactionTable), Optional classFilter As Boolean = True) As NetworkTables
            Dim mapsVector = maps.ToArray
            Dim reactionVector As ReactionTable() = reactions.ToArray
            Dim compoundsWithBiologicalRoles = CompoundBrite _
                .CompoundsWithBiologicalRoles _
                .GroupBy(Function(c) c.entry.Key) _
                .ToDictionary(Function(c) c.Key,
                              Function(c)
                                  Return c.First.class
                              End Function)
            Dim compounds = mapsVector _
                .Select(AddressOf getCompoundsInMap) _
                .IteratesALL _
                .GroupBy(Function(c) c.Name) _
                .Where(Function(c) Not c.Key Like ignores) _
                .Where(Function(c)
                           If classFilter Then
                               Return compoundsWithBiologicalRoles.ContainsKey(c.Key)
                           Else
                               Return True
                           End If
                       End Function) _
                .ToArray

            If classFilter Then
                reactionVector = reactionVector _
                    .Where(Function(r)
                               ' reaction show have EC class value
                               If r.EC.IsNullOrEmpty Then
                                   Return False
                               Else
                                   Return r.EC _
                                       .Any(Function([class])
                                                Return [class] _
                                                    .Split("."c) _
                                                    .First _
                                                    .ParseInteger > 0
                                            End Function)
                               End If
                           End Function) _
                    .ToArray
            End If

            Dim reactantIndex = reactionVector.getCompoundIndex(Function(r) r.substrates)
            Dim productIndex = reactionVector.getCompoundIndex(Function(r) r.products)

            Dim nodes As Dictionary(Of String, Node) = compounds.createNodeTable(compoundsWithBiologicalRoles)
            Dim edges As New List(Of NetworkEdge)
            Dim edge1 As NetworkEdge
            Dim edge2 As NetworkEdge
            Dim fluxNode As Node

            ' 下面的两个for循环产生的是从reactant a到products b的反应过程边连接
            For Each a In compounds
                Dim forwards = reactantIndex.TryGetValue(a.Key)

                If forwards.IsNullOrEmpty Then
                    Continue For
                End If

                Dim producs As Dictionary(Of String, ReactionTable()) = forwards _
                    .Select(Function(r) r.products.Select(Function(cid) (cid, r))) _
                    .IteratesALL _
                    .GroupBy(Function(cid) cid.cid) _
                    .ToDictionary(Function(c) c.Key,
                                  Function(group)
                                      Return group.Select(Function(r) r.r).ToArray
                                  End Function)

                For Each b In compounds.Where(Function(c) c.Key <> a.Key AndAlso producs.ContainsKey(c.Key))
                    ' reactant -> reaction
                    ' reaction -> product
                    For Each flux In producs(b.Key)
                        edge1 = New NetworkEdge With {
                            .fromNode = a.Key,
                            .toNode = flux.entry,
                            .interaction = "substrate"
                        }
                        edge2 = New NetworkEdge With {
                            .fromNode = flux.entry,
                            .toNode = b.Key,
                            .interaction = "product"
                        }

                        edges = edges + edge1 + edge2

                        If Not nodes.ContainsKey(flux.entry) Then
                            fluxNode = New Node With {
                                .ID = flux.entry,
                                .NodeType = "flux",
                                .Properties = New Dictionary(Of String, String) From {
                                    {"label", flux.EC.JoinBy("+") Or flux.name.AsDefault}
                                }
                            }

                            Call nodes.Add(flux.entry, fluxNode)
                        End If
                    Next
                Next
            Next

            Dim g As New NetworkTables(nodes.Values, edges)

            Call g.ComputeNodeDegrees

            Return g
        End Function
    End Module
End Namespace