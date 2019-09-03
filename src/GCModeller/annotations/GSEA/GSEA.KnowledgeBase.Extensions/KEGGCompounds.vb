Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Organism

''' <summary>
''' Create background model for KEGG pathway enrichment based on the kegg metabolites, used for LC-MS metabolism data analysis.
''' </summary>
Public Module KEGGCompounds

    ''' <summary>
    ''' Create GSEA background model from LC-MS metabolism analysis result.
    ''' </summary>
    ''' <param name="maps"></param>
    ''' <returns></returns>
    <Extension>
    Public Function CreateBackground(org As OrganismInfo, maps As IEnumerable(Of Pathway), Optional outputAll As Boolean = True) As Background
        ' The total number of metabolites in background genome. 
        Dim backgroundSize% = 0
        Dim clusters As New List(Of Cluster)

        For Each map As Pathway In maps
            clusters += New Cluster With {
                .description = map.description,
                .ID = map.briteID,
                .names = map.name,
                .members = map.compound _
                    .Select(Function(c)
                                Return New BackgroundGene With {
                                    .name = c.text,
                                    .accessionID = c.name,
                                    .[alias] = {c.name},
                                    .locus_tag = c,
                                    .term_id = {c.name}
                                }
                            End Function) _
                    .ToArray
            }
        Next

        backgroundSize = clusters _
            .Select(Function(c) c.members) _
            .IteratesALL _
            .Select(Function(c) c.accessionID) _
            .Distinct _
            .Count

        Return New Background With {
            .build = Now,
            .clusters = clusters,
            .comments = "Background model apply for GSEA of LC-MS metabolism analysis, created by GCModeller.",
            .name = org.FullName,
            .size = backgroundSize
        }
    End Function
End Module
