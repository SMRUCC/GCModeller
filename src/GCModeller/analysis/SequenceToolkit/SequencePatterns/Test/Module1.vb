Imports LANS.SystemsBiology.AnalysisTools.SequenceTools.SequencePatterns
Imports LANS.SystemsBiology.AnalysisTools.SequenceTools.SequencePatterns.Motif
Imports LANS.SystemsBiology.AnalysisTools.SequenceTools.SequencePatterns.Motif.Patterns
Imports LANS.SystemsBiology.SequenceModel.FASTA

Module Module1

    Sub Main()

        Dim s = "[AG]CGTT[AC]G[ATC]"
        Dim st = PatternParser.SimpleTokens(s)




        Dim scan As New Scanner(New FastaToken("F:\Xanthomonas_campestris_8004_uid15\CP000050.fna"))
        Dim result = scan.Scan(s)

        Dim motif As String = "[AG]{2,7}at*g+G4A{3,5}G{29}N?N{x}-(aa{x}TGA{b}){3,7}~x={2,5};b={x+2}"
        Dim tokens = PatternParser.ExpressionParser(motif)
    End Sub

End Module
