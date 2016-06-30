Imports LANS.SystemsBiology
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports LANS.SystemsBiology.Toolkits.RNA_Seq.TSSsTools.DocumentFormat
Imports LANS.SystemsBiology.Toolkits.RNA_Seq.TSSsTools

Partial Module CLI

    ''' <summary>
    ''' 数据视图
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Views", Usage:="/Views /in <inTSSs.csv> /genome <genome.fasta> [/out <outDIR> /TSS-len 5 /upstram 150]")>
    Public Function Views(args As CommandLine.CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim out As String = args.GetValue("/out", inFile.TrimFileExt & ".Views/")
        Dim TSSsLen As Integer = args.GetValue("/TSS-len", 5)
        Dim upstreamLen As Integer = args.GetValue("/upstream", 150)
        Dim genome As New SequenceModel.FASTA.FastaToken(args("/genome"))
        Dim inData = inFile.LoadCsv(Of Transcript)

        Call TSSsDataViews.UpStream(inData, genome, upstreamLen).Save(out & "/TSSs.UpStream.fasta")
        Call TSSsDataViews._5UTR(inData, genome).Save(out & "/5'UTR.fasta")
        Call TSSsDataViews._5UTRLength(inData).Save(out & "/5'UTR.csv", Encoding:=System.Text.Encoding.ASCII)
        Call TSSsDataViews.TSSs(inData, genome, TSSsLen).Save(out & "/TSSs.fasta")

        Return 0
    End Function

    <ExportAPI("/Views.TSSs.NTFreq", Usage:="/Views.TSSs.NTFreq /in <inTSSs.csv> /reads <reads.dat> [/out <out.csv>]")>
    Public Function TSSsNTFreq(args As CommandLine.CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim readsFile As String = args("/reads")
        Dim out As String = args.GetValue("/out", inFile.TrimFileExt & ".TSSs.NTFreq.csv")
        Dim inData = inFile.LoadCsv(Of Transcript)
        Dim RawReads = ReadsCount.LoadDb(readsFile).ToDictionary(Function(x) x.Index)
        Dim source = (From x In inData Where Not String.IsNullOrEmpty(x.Synonym) Select x).ToArray
        Dim nts = (From x In source Where RawReads.ContainsKey(x.TSSs) Select RawReads(x.TSSs).NT).ToArray
        Dim Freq = (From nt In nts Select nt Group nt By nt Into Count).ToDictionary(Function(x) x.nt, Function(x) x.Count / nts.Length)
        Dim Freq2 = (From x In Freq Select nt = x.Key, ntFreq = x.Value).ToArray
        Return Freq2.SaveTo(out).CLICode
    End Function
End Module