Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Namespace ReactionNetwork

    <HideModuleName>
    Module GetGeneSymbols

        <Extension>
        Public Function GetGeneSymbols(reactions As IEnumerable(Of ReactionTable)) As (label As String, KO As String(), EC As String(), keggRid As String(), geneSymbols As String())
            Dim models As String() = reactions _
                .Select(Function(r)
                            Return r.geneNames.JoinIterates(r.KO).JoinIterates(r.EC)
                        End Function) _
                .IteratesALL _
                .Select(Function(s) s.StringSplit("[;,]")) _
                .IteratesALL _
                .Select(AddressOf Strings.Trim) _
                .Where(Function(s) Not s.StringEmpty) _
                .ToArray
            Dim KO = models.Where(Function(id) id.IsPattern("K\d+")).ToArray
            Dim EC = models.Select(Function(id) id.Match("\d+\.([-]|(\d+))(\.([-]|(\d+))){3}")).Where(Function(id) Not id.StringEmpty).ToArray
            Dim keggRid = models.Select(Function(id) id.Match("R\d+")).Where(Function(id) Not id.StringEmpty).ToArray
            Dim allId As String() = KO.JoinIterates(EC).JoinIterates(keggRid).ToArray
            Dim geneSymbols = models _
                .AsParallel _
                .Where(Function(line) line.InStrAny(allId) = -1) _
                .Where(Function(id) id.Match("\d+\.([-]|(\d+))(\.([-]|(\d+)))*", RegexICSng).StringEmpty) _
                .ToArray
            Dim middleNode As String

            If models.Length = 1 Then
                middleNode = models(Scan0)
            Else
                If geneSymbols.IsNullOrEmpty Then
                    If EC.IsNullOrEmpty Then
                        If KO.IsNullOrEmpty Then
                            middleNode = keggRid.GroupBy(Function(id) id).OrderByDescending(Function(g) g.Count).First.Key
                        Else
                            middleNode = KO.GroupBy(Function(id) id).OrderByDescending(Function(g) g.Count).First.Key
                        End If
                    Else
                        middleNode = EC.GroupBy(Function(id) id.Split("."c).Take(2).JoinBy(".")).OrderByDescending(Function(g) g.Count).First.Key & ".-.-"
                    End If
                Else
                    middleNode = geneSymbols.GroupBy(Function(name) name.ToLower).OrderByDescending(Function(g) g.Count).First.First
                End If
            End If

            Return (middleNode, KO, EC, keggRid, geneSymbols)
        End Function
    End Module
End Namespace