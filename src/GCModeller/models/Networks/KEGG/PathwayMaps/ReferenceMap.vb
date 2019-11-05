Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Data

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

        Private Function getCompoundsInMap(map As bGetObject.Pathway) As IEnumerable(Of NamedValue(Of String))
            Return map.compound _
                .Where(Function(term)
                           Return term.name.IsPattern("C\d+")
                       End Function) _
                .Select(Function(a)
                            Return New NamedValue(Of String) With {
                                .Name = a.name,
                                .Value = a.text,
                                .Description = map.EntryId
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

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function getCompoundClassCategory() As Dictionary(Of String, String)
            Return CompoundBrite _
                .CompoundsWithBiologicalRoles _
                .GroupBy(Function(c) c.entry.Key) _
                .ToDictionary(Function(c) c.Key,
                              Function(c)
                                  Return c.First.class
                              End Function)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="maps">
        ''' <see cref="OrganismModel.EnumerateModules(String)"/>
        ''' </param>
        ''' <param name="reactions"></param>
        ''' <returns></returns>
        Public Function BuildNetworkModel(maps As IEnumerable(Of bGetObject.Pathway), reactions As IEnumerable(Of ReactionTable)) As NetworkTables
            Dim mapsVector = maps.ToArray
            Dim reactionVector As ReactionTable() = reactions.ToArray
            Dim compounds = mapsVector _
                .Select(AddressOf getCompoundsInMap) _
                .IteratesALL _
                .GroupBy(Function(c) c.Name) _
                .Where(Function(c)
                           ' 因为在这里是物种特定的代谢途径模型数据
                           ' 所以在这里就不进行生物学功能分类了
                           ' 直接全部使用该物种的pathway map里面的所有的代谢物来进行作图
                           Return Not c.Key Like ignores
                       End Function) _
                .ToArray

            Return compounds.buildNetworkModelInternal(reactionVector)
        End Function

        <Extension>
        Private Function buildNetworkModelInternal(compounds As IGrouping(Of String, NamedValue(Of String))(), reactionVector As ReactionTable()) As NetworkTables
            Dim reactantIndex = reactionVector.getCompoundIndex(Function(r) r.substrates)
            Dim productIndex = reactionVector.getCompoundIndex(Function(r) r.products)
            Dim compoundsWithBiologicalRoles = getCompoundClassCategory()
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

                Dim aName$ = a.First.Value
                Dim producs As Dictionary(Of String, ReactionTable()) = forwards _
                    .Select(Function(r) r.products.Select(Function(cid) (cid, r))) _
                    .IteratesALL _
                    .GroupBy(Function(cid) cid.cid) _
                    .ToDictionary(Function(c) c.Key,
                                  Function(group)
                                      Return group.Select(Function(r) r.r).ToArray
                                  End Function)

                For Each b In compounds.Where(Function(c) c.Key <> a.Key AndAlso producs.ContainsKey(c.Key))
                    Dim bName$ = b.First.Value

                    ' reactant -> reaction
                    ' reaction -> product
                    For Each flux As ReactionTable In producs(b.Key)
                        edge1 = New NetworkEdge With {
                            .fromNode = a.Key,
                            .toNode = flux.entry,
                            .interaction = "substrate",
                            .Properties = New Dictionary(Of String, String) From {
                                {"compound.name", aName},
                                {"flux.name", flux.EC.First}
                            }
                        }
                        edge2 = New NetworkEdge With {
                            .fromNode = flux.entry,
                            .toNode = b.Key,
                            .interaction = "product",
                            .Properties = New Dictionary(Of String, String) From {
                                {"compound.name", bName},
                                {"flux.name", flux.EC.First}
                            }
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
            Dim compoundsWithBiologicalRoles = getCompoundClassCategory()
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

            Return compounds.buildNetworkModelInternal(reactionVector)
        End Function
    End Module
End Namespace