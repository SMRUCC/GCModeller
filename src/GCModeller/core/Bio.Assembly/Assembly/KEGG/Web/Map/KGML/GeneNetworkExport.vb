Imports Microsoft.VisualBasic.Linq

Namespace Assembly.KEGG.WebServices.KGML

    ''' <summary>
    ''' A network of gene1 -> compound -> gene2
    ''' </summary>
    Public Class GeneNetworkExport

        Public Property gene1 As String()
        Public Property gene2 As String()
        Public Property compound As String()
        Public Property ko1Name As String
        Public Property ko2Name As String
        Public Property compoundName As String
        Public Property mapId As String
        Public Property mapName As String

        Const Missing As String = "#FFFFFF"

        Public Overrides Function GetHashCode() As Integer
            Return $"{gene1.JoinBy(",")}+{gene2.JoinBy(",")}+{compound.JoinBy(",")}".GetHashCode
        End Function

        Shared ReadOnly koNames As Dictionary(Of String, String) = My.Resources.KEGG.ko.LineTokens.Select(Function(a) a.GetTagValue(vbTab)).ToDictionary(Function(a) a.Name, Function(a) a.Value)
        Shared ReadOnly cpdNames As Dictionary(Of String, String) = My.Resources.KEGGCompounds.compound.LineTokens.Select(Function(a) a.GetTagValue(vbTab)).ToDictionary(Function(a) a.Name, Function(a) a.Value)

        Public Shared Iterator Function ExtractFromKGML(kgml As pathway) As IEnumerable(Of GeneNetworkExport)
            Dim entryIndex = kgml.entries.ToDictionary(Function(e) e.id)
            Dim rels = kgml.relations _
                .SafeQuery _
                .Where(Function(r)
                           Return r.type = "ECrel" AndAlso
                               r.subtype IsNot Nothing AndAlso
                               r.subtype.name = "compound"
                       End Function) _
                .ToArray
            Dim entry1 = rels.GroupBy(Function(a) a.entry1).ToDictionary(Function(a) a.Key, Function(a) a.ToArray)
            Dim entry2 = rels.GroupBy(Function(a) a.entry2).ToDictionary(Function(a) a.Key, Function(a) a.ToArray)
            Dim koIndex = kgml.entries _
                .Where(Function(a) a.type = "ortholog") _
                .Select(Function(k)
                            Return k.reaction.StringSplit(" ").Select(Function(rid) (rid, k))
                        End Function) _
                .IteratesALL _
                .GroupBy(Function(r) r.rid) _
                .ToDictionary(Function(r) r.Key,
                              Function(r)
                                  Return r.Select(Function(a) a.k).ToArray
                              End Function)

            For Each gene As entry In kgml.entries.Where(Function(e) e.type = "gene")
                If gene.graphics Is Nothing Then
                    Continue For
                End If
                If gene.graphics.bgcolor = Missing Then
                    Continue For
                End If

                Dim entry1Matches = entry1.TryGetValue(gene.id)
                Dim entry2Matches = entry2.TryGetValue(gene.id)

                For Each rel As relation In c(entry1Matches, entry2Matches)
                    Dim ko1 = entryIndex(rel.entry1).reaction.StringSplit(" ").Where(Function(rid) koIndex.ContainsKey(rid)).Select(Function(rid) koIndex(rid).Select(Function(e) e.name)).IteratesALL.IteratesALL.Distinct.ToArray
                    Dim ko2 = entryIndex(rel.entry2).reaction.StringSplit(" ").Where(Function(rid) koIndex.ContainsKey(rid)).Select(Function(rid) koIndex(rid).Select(Function(e) e.name)).IteratesALL.IteratesALL.Distinct.ToArray
                    Dim compound = entryIndex(rel.subtype.value)

                    If Not (ko1.Any AndAlso ko2.Any) Then
                        Continue For
                    End If

                    Yield New GeneNetworkExport With {
                        .mapId = kgml.name,
                        .mapName = kgml.title,
                        .gene1 = ko1,
                        .gene2 = ko2,
                        .compound = compound.name,
                        .compoundName = .compound _
                            .Select(Function(cid)
                                        Return cpdNames(cid.Split(":"c).Last)
                                    End Function) _
                            .JoinBy("; "),
                        .ko1Name = ko1.Select(Function(kid) koNames(kid)).JoinBy("; "),
                        .ko2Name = ko2.Select(Function(kid) koNames(kid)).JoinBy("; ")
                    }
                Next
            Next
        End Function

    End Class
End Namespace