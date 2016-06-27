Imports LANS.SystemsBiology.AnalysisTools.SequenceTools.SequencePatterns
Imports LANS.SystemsBiology.AnalysisTools.SequenceTools.SequencePatterns.Topologically
Imports LANS.SystemsBiology.SequenceModel.FASTA
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports LANS.SystemsBiology.SequenceModel.Polypeptides

Partial Module Utilities

    <ExportAPI("Search.Batch",
               Info:="Batch search for repeats.",
               Usage:="Search.Batch /aln <alignment.fasta> [/min 3 /max 20 /min-rep 2 /out <./>]")>
    <ParameterInfo("/aln", False,
                   Description:="The input fasta file should be the output of the clustal multiple alignment fasta output.")>
    Public Function BatchSearch(args As CommandLine.CommandLine) As Integer
        Dim Mla As FastaFile = args.GetObject("/aln", AddressOf FastaFile.Read)
        Dim Min As Integer = args.GetValue("/min", 3)
        Dim Max As Integer = args.GetValue("/max", 20)
        Dim MinAppeared As Integer = args.GetValue("/min-rep", 2)
        Dim SaveDir As String = args.GetValue("/out", "./")

        Call Topologically.BatchSearch(Mla, Min, Max, MinAppeared, SaveDir)

        Return 0
    End Function

    <ExportAPI("Repeats.Density",
               Usage:="Repeats.Density /dir <dir> /size <size> /ref <refName> [/out <out.csv> /cutoff <default:=0>]")>
    Public Function RepeatsDensity(args As CommandLine.CommandLine) As Integer
        Dim DIR As String = args("/dir")
        Dim size As Integer = args.GetInt32("/size")
        Dim out As String = args.GetValue("/out", DIR & "/Repeats.Density.vector.txt")
        Dim vector As Double() = Topologically.RepeatsDensity(DIR, size, ref:=args("/ref"), cutoff:=args.GetValue("/cutoff", 0R))
        Return vector.FlushAllLines(out).CLICode
    End Function

    <ExportAPI("rev-Repeats.Density", Usage:="rev-Repeats.Density /dir <dir> /size <size> /ref <refName> [/out <out.csv> /cutoff <default:=0>]")>
    Public Function revRepeatsDensity(args As CommandLine.CommandLine) As Integer
        Dim DIR As String = args("/dir")
        Dim size As Integer = args.GetInt32("/size")
        Dim out As String = args.GetValue("/out", DIR & "/rev-Repeats.Density.vector.txt")
        Dim vector As Double() = Topologically.RevRepeatsDensity(DIR, size, ref:=args("/ref"), cutoff:=args.GetValue("/cutoff", 0R))
        Return vector.FlushAllLines(out).CLICode
    End Function

    <ExportAPI("/Write.Seeds",
               Usage:="/Write.Seeds /out <out.dat> [/prot /max <20>]")>
    Public Function WriteSeeds(args As CommandLine.CommandLine) As Integer
        Dim isProt As Boolean = args.GetBoolean("/prot")
        Dim out As String = args("/out")
        Dim max As Integer = args.GetValue("/max", 20)
        Dim chars As Char() = If(isProt, ToChar.Values.Distinct.ToArray, {"A"c, "T"c, "G"c, "C"c})
        Dim seeds As SeedData = SeedData.Initialize(chars, max)

        Return seeds.Save(out)
    End Function
End Module