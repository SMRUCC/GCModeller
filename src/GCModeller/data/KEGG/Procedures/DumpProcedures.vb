Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Oracle.LinuxCompatibility.MySQL
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry

Public Module DumpProcedures

    Public Sub DumpReferencePathwayMap(DIR$, save$)
        Dim data_pathways As New List(Of mysql.data_pathway)
        Dim references As New Dictionary(Of Long, mysql.data_references)
        Dim pathwayReferences As New List(Of mysql.xref_pathway_references)
        Dim pathwayModules As New List(Of mysql.xref_pathway_modules)
        Dim pathwayKOgenes As New List(Of mysql.xref_pathway_genes)
        Dim pathwayClass As New List(Of mysql.class_ko00001_pathway)
        Dim orthologyClass As New List(Of mysql.class_ko00001_orthology)
        Dim br As Dictionary(Of String, BriteHText) = htext.ko00001 _
            .Hierarchical _
            .EnumerateEntries _
            .Select(Function(x) x.Parent) _
            .Where(Function(x)
                       Return Not x Is Nothing AndAlso
                            x.CategoryLevel = "C"c AndAlso
                            Not x.ClassLabel.StringEmpty
                   End Function) _
            .GroupBy(Function(pathway) pathway.ClassLabel.Split.First) _
            .ToDictionary(Function(x) x.Key,
                          Function(g) g.First)

        For Each xml As String In ls - l - r - "*.XML" <= DIR
            Dim map As PathwayMap = xml.LoadXml(Of PathwayMap)
            Dim image$ = (xml.TrimSuffix & ".png").Open.ZipAsBase64

            data_pathways += New mysql.data_pathway With {
                .map = image,
                .KO = "ko" & map.BriteId,
                .name = map.Name,
                .uid = Val(map.BriteId),
                .description = map.Description
            }

            Dim pathway& = data_pathways.Last.uid
            Dim KO = data_pathways.Last.KO

            For Each m In map.Modules.SafeQuery
                pathwayModules += New mysql.xref_pathway_modules With {
                    .pathway = pathway,
                    .module = Val(m.Key.Trim("M"c)),
                    .name = MySqlEscaping(m.Value),
                    .KO = KO
                }
            Next

            Dim h As BriteHText = br.TryGetValue(map.BriteId)

            For Each O In map.KEGGOrthology.SafeQuery
                orthologyClass += New mysql.class_ko00001_orthology With {
                    .function = O.Value,
                    .KEGG = O.Key,
                    .Orthology = O.Key.Trim("K"c),
                    .name = .function.Split(";"c).First,
                    .level_C = h?.ClassLabel,
                    .level_B = h?.Parent?.ClassLabel,
                    .level_A = h?.Parent?.Parent?.ClassLabel
                }
            Next
        Next

        Call data_pathways.DumpTransaction(save)
        Call references.Values.DumpTransaction(save)
        Call pathwayReferences.DumpTransaction(save)
        Call orthologyClass.DumpTransaction(save)
        Call pathwayModules.DumpTransaction(save)

        Call pathwayModules.DumpToTable(save)
        Call orthologyClass.DumpToTable(save)
        Call data_pathways.DumpToTable(save)
        Call references.Values.DumpToTable(save)
        Call pathwayReferences.DumpToTable(save)
    End Sub

    Public Sub DumpKO(DIR$, save$)

    End Sub
End Module
