Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Data

Public Module SimpleBuilder

    <Extension>
    Public Function GraphQueryByCompoundList(compoundIds As IEnumerable(Of String), reactions As ReactionClassifier, maps As MapRepository) As NetworkGraph
        Dim g As New NetworkGraph
        Dim usedLinks As New Index(Of String)
        Dim compoundIndex As Index(Of String) = compoundIds.Indexing

        For Each cid As String In compoundIndex.Objects
            g.CreateNode(cid)
        Next

        For Each cid As String In compoundIndex.Objects
            Dim links As ReactionClass() = reactions.QueryByCompoundId(cid)

            For Each link As ReactionClass In links
                For Each transform In link.reactantPairs
                    If transform.from Like compoundIndex AndAlso transform.to Like compoundIndex Then
                        If Not g.GetEdges(g.GetElementByID(transform.from), g.GetElementByID(transform.to)).Any Then
                            Dim tuple As String() = {transform.from, transform.to}

                            g.CreateEdge(transform.from, transform.to, data:=New EdgeData With {.label = link.definition})

                            For Each map As MapIndex In maps.QueryMapsByMembers(tuple)
                                If map.FilterAll(tuple) Then
                                    Dim KO = link.orthology.Where(Function(KOid) map.hasAny(KOid.name)).Select(Function(k) k.name).ToArray

                                    If g.GetElementByID(map.id) Is Nothing Then
                                        g.CreateNode(map.id)
                                    End If

                                    For Each id As String In KO
                                        If g.GetElementByID(id) Is Nothing Then
                                            g.CreateNode(id)
                                        End If

                                        If Not g.GetEdges(g.GetElementByID(map.id), g.GetElementByID(id)).Any Then
                                            g.CreateEdge(map.id, id)
                                        End If
                                    Next

                                    If Not g.GetEdges(g.GetElementByID(map.id), g.GetElementByID(transform.from)).Any Then
                                        g.CreateEdge(map.id, transform.from)
                                    End If

                                    If Not g.GetEdges(g.GetElementByID(map.id), g.GetElementByID(transform.to)).Any Then
                                        g.CreateEdge(map.id, transform.to)
                                    End If
                                End If
                            Next
                        End If
                    End If
                Next
            Next
        Next

        Return g
    End Function
End Module
