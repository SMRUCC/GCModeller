Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports Microsoft.VisualBasic.Linq

<Package("KOBAS")>
Module KOBAS

    <ExportAPI("read.KEGGpathway")>
    Public Function KEGGPathway(file As String) As EnrichmentTerm()
        Return file.SplitTable _
            .Where(Function(part) part.name = "KEGG PATHWAY") _
            .FirstOrDefault _
            .SafeQuery _
            .ToArray
    End Function
End Module
