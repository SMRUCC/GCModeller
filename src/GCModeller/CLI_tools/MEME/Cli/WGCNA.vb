Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Workflows.PromoterParser
Imports LANS.SystemsBiology.Assembly.NCBI
Imports LANS.SystemsBiology.Toolkits.RNA_Seq
Imports LANS.SystemsBiology.Toolkits.RNA_Seq.RTools
Imports Microsoft.VisualBasic.CommandLine.Reflection

Partial Module CLI

    <ExportAPI("--CExpr.WGCNA", Usage:="--CExpr.WGCNA /mods <CytoscapeNodes.txt> /genome <genome.DIR|*.PTT;*.fna> /out <DIR.out>")>
    Public Function WGCNAModsCExpr(args As CommandLine.CommandLine) As Integer
        Dim mods = WGCNA.ModsView(WGCNA.LoadModules(args("/mods")))
        Dim gb As New GenBank.TabularFormat.PTTDbLoader(args("/genome"))
        Dim geneParser As New GenePromoterParser(gb)
        Dim ExportDir As String = args("/out")

        For Each Length As Integer In GenePromoterParser.PrefixLength

            For Each profile In mods
                Dim path As String = $"{ExportDir}/{Length}/{profile.Key}.fasta"
                Dim fasta = geneParser.GetSequenceById(lstId:=profile.Value.Join(profile.Key).Distinct.ToArray, Length:=Length)
                Call fasta.Save(path)
            Next
        Next

        Return 0
    End Function
End Module