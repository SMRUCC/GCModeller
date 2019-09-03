Imports System.Runtime.CompilerServices

''' <summary>
''' Create background model for KEGG pathway enrichment based on the kegg metabolites, used for LC-MS metabolism data analysis.
''' </summary>
Public Module KEGGCompounds

    ''' <summary>
    ''' Create GSEA background model from bbh annotation result.
    ''' </summary>
    ''' <param name="maps"></param>
    ''' <param name="backgroundSize">
    ''' The total number of genes in background genome. 
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function CreateBackground(maps As IEnumerable(Of map),
                                     Optional backgroundSize% = -1,
                                     Optional outputAll As Boolean = True,
                                     Optional genomeName$ = "Unknown") As Background

    End Function
End Module
