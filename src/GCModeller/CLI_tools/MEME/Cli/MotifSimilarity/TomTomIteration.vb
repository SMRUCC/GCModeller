Imports System.Text.RegularExpressions
Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.DocumentFormat.MEME
Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.DocumentFormat.MEME.Text
Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Programs
Imports LANS.SystemsBiology.SequenceModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Partial Module CLI

    <ExportAPI("/TomTom.Sites.Groups",
               Usage:="/TomTom.Sites.Groups /in <similarity.csv> /meme <meme.DIR> [/grep <regex> /out <out.DIR>]")>
    Public Function ExportTOMSites(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim memeDIR As String = args("/meme")
        Dim out As String = args.GetValue("/out", [in] & "-" & memeDIR.BaseName)
        Dim memeText As IEnumerable(Of String) = ls - l - r - wildcards("*.txt") <= memeDIR
        Dim hits As IEnumerable(Of MotifMatch) = [in].LoadCsv(Of MotifMatch)
        Dim grep As String = args("/grep")
        Dim trimName As Func(Of String, String) =
            If(String.IsNullOrEmpty(grep),
            Function(s) s,
            Function(s) Regex.Match(s, grep, RegexICSng).Value)
        Dim memeHash = (From x As LDM.Motif()
                        In memeText.Select(AddressOf MEME_TEXT.SafelyLoad)
                        Where Not x.IsNullOrEmpty
                        Select x) _
                              .MatrixAsIterator _
                              .OrderBy(Function(x) x.uid) _
                              .ToDictionary(Function(x) trimName(x.uid) & "." & x.Id)
#If DEBUG Then
        Call memeHash.GetJson.SaveTo([in].ParentPath & "/motifs.json")
#End If
        Dim Groups = (From x As MotifMatch
                      In hits
                      Select x
                      Group x By modName = trimName(x.Module) & "." & x.Query Into Group)

        Dim sites As New List(Of NamedValue(Of FASTA.FastaFile))

        For Each modX In Groups
            Dim fa As FASTA.FastaFile = __exportSites(modX.modName, modX.Group.ToArray, memeHash, trimName)
            Dim path As String = $"{out}/{trimName(modX.modName)}.fasta"
            sites += New NamedValue(Of FASTA.FastaFile)(path, fa)
        Next

        Dim getUids = (From x In sites
                       Let array As String() = x.x.Select(Function(o) o.Attributes(Scan0)).OrderBy(Function(s) s).ToArray
                       Let uid As String = String.Join("+", array)
                       Select uid,
                           x
                       Group By uid Into Group)

        For Each Group In getUids
            Dim unique = Group.Group.First
            Dim fasta As FASTA.FastaFile = unique.x.x
            Call fasta.Save(unique.x.Name, Encodings.ASCII)
        Next

        Return 0
    End Function

    Private Function __exportSites([mod] As String,
                                   hits As MotifMatch(),
                                   memeHash As Dictionary(Of String, LDM.Motif),
                                   grep As Func(Of String, String)) As FASTA.FastaFile
        Dim query As IEnumerable(Of FASTA.FastaToken) =
            memeHash([mod]).Sites.Select(Function(x) x.ToFasta([mod]))
        Dim hitSites = From hit As MotifMatch
                       In hits
                       Let uid As String = grep(hit.Family) & "." & hit.Target
                       Where memeHash.ContainsKey(uid)
                       Select memeHash(uid).Sites.Select(Function(x) x.ToFasta(uid))
        Dim fasta As New FASTA.FastaFile(query.ToList + hitSites.MatrixAsIterator)
        Return fasta
    End Function
End Module
