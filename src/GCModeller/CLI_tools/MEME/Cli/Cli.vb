Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis
Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite.Workflows.PromoterParser
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.GenomeMotifFootPrints
Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.MotifScans
Imports SMRUCC.genomics.Toolkits.RNA_Seq
Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions
Imports SMRUCC.genomics.DatabaseServices.Regprecise

<PackageNamespace("MEME.CLI",
                  Description:="A wrapper tools for the NCBR meme tools, this is a powerfull tools for reconstruct the regulation in the bacterial genome.",
                  Category:=APICategories.CLI_MAN,
                  Publisher:="xie.guigang@gcmodeller.org")>
Module CLI

    <ExportAPI("VirtualFootprint.DIP",
               Info:="Associate the dip information with the Sigma 70 virtual footprints.",
               Usage:="VirtualFootprint.DIP vf.csv <csv> dip.csv <csv>")>
    Public Function VirtualFootprintDIP(argvs As CommandLine.CommandLine) As Integer
        Dim dipCsv As String = argvs("dip.csv")
        Dim vfCsv As String = argvs("vf.csv")

        Return IntergenicSigma70.VirtualFootprintDIP(vfCsv, DIPCsv:=dipCsv)
    End Function

    <ExportAPI("Motif.Locates",
               Info:="",
               Usage:="Motif.Locates -ptt <bacterial_genome.ptt> -meme <meme.txt> [/out <out.csv>]")>
    Public Function MotifLocites(args As CommandLine.CommandLine) As Integer
        Dim PTTfile As String = args("-ptt")
        Dim MEMEText As String = args("-meme")
        Dim OutCsv As String = args("/out")

        If String.IsNullOrEmpty(OutCsv) Then
            OutCsv = MEMEText & "_Located.csv"
        End If

        Dim Length = MotifLoci.GetTraningSet(MEMEText)
        Dim Result = MotifLoci.Located(
            PTT.Load(PTTfile),
            MEME_Suite.DocumentFormat.MEME.Text.Load(MEMEText),
            Length)
        Dim b As Boolean = Result.SaveTo(OutCsv, False)

        Return If(b, 0, -100)
    End Function

    ''' <summary>
    ''' 求query - subject得到的差集
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("--hits.diff", Usage:="--hits.diff /query <bbhh.csv> /subject <bbhh.csv> [/reverse]")>
    Public Function DiffHits(args As CommandLine.CommandLine) As Integer
        Dim reverse As Boolean = args.GetBoolean("/reverse")
        Dim query = (From x In args("/query").LoadCsv(Of bbhMappings) Select x Group x By x.hit_name Into Group).ToDictionary(Function(x) x.hit_name, elementSelector:=Function(x) x.Group.ToArray)
        Dim subject = (From x In args("/subject").LoadCsv(Of bbhMappings) Select x Group x By x.hit_name Into Group).ToDictionary(Function(x) x.hit_name, elementSelector:=Function(x) x.Group.ToArray)
        Dim LQuery = (From x In query Where subject.ContainsKey(x.Key) Select __diff(x.Value, subject(x.Key))).ToArray.MatrixToList
        Dim path As String = args("/query").TrimFileExt & $".diff__{IO.Path.GetFileNameWithoutExtension(args("/subject"))}.csv"
        Dim exclude = (From x In query Where Not subject.ContainsKey(x.Key) Select x.Value).ToArray.MatrixToList
        Call LQuery.SaveTo(path)
        path = args("/query").TrimFileExt & $".excludes__{IO.Path.GetFileNameWithoutExtension(args("/subject"))}.csv"
        Return exclude.SaveTo(path).CLICode
    End Function

    Private Function __diff(query As bbhMappings(), subject As bbhMappings()) As bbhMappings()
        Dim s2 = New [Set](subject.ToArray(Function(xx) xx.query_name))
        Dim expect = (New [Set](query.ToArray(Function(xx) xx.query_name)) - s2).ToArray(Function(xx) DirectCast(xx, String))
        Dim xDict = query.ToDictionary(Function(xx) xx.query_name)
        Dim value = expect.ToArray(Function(xx) xDict(xx))
        Return value
    End Function

    <ExportAPI("--Intersect.Max", Usage:="--Intersect.Max /query <bbhh.csv> /subject <bbhh.csv>")>
    Public Function MaxIntersection(args As CommandLine.CommandLine) As Integer
        Dim query = (From x In args("/query").LoadCsv(Of bbhMappings) Select x Group x By x.hit_name Into Group).ToDictionary(Function(x) x.hit_name, elementSelector:=Function(x) x.Group.ToArray)
        Dim subject = (From x In args("/subject").LoadCsv(Of bbhMappings) Select x Group x By x.hit_name Into Group).ToDictionary(Function(x) x.hit_name, elementSelector:=Function(x) x.Group.ToArray)
        Dim LQuery = (From x In query
                      Where subject.ContainsKey(x.Key)
                      Let intr = Interacts(x.Value, subject(x.Key))
                      Select intr
                      Order By intr.Length Descending).ToArray
        Dim path As String = args("/query").TrimFileExt & $".Intersect.Max.{IO.Path.GetFileNameWithoutExtension(args("/subject"))}.csv"
        Return LQuery.First.SaveTo(path).CLICode
    End Function

    Private Function Interacts(query As bbhMappings(), subject As bbhMappings()) As bbhMappings()
        Dim s1 = New [Set](query.ToArray(Function(x) x.query_name))
        Dim s2 = New [Set](subject.ToArray(Function(x) x.query_name))
        Dim inters = (s1 And s2).ToArray(Of String)
        Dim LQuery = (From x In query Where Array.IndexOf(inters, x.query_name) > -1 Select x).ToArray
        Return LQuery
    End Function

    <ExportAPI("--GetFasta", Usage:="--GetFasta /bbh <bbhh.csv> /id <subject_id> /regprecise <regprecise.fasta>")>
    Public Function GetFasta(args As CommandLine.CommandLine) As Integer
        Dim bbh = args("/bbh").LoadCsv(Of bbhMappings)
        Dim id As String = args("/id")
        Dim query = (From x In bbh.AsParallel Where String.Equals(id, x.hit_name, StringComparison.OrdinalIgnoreCase) Select x.query_name).ToArray
        Dim fasta = New SMRUCC.genomics.SequenceModel.FASTA.FastaFile(args("/regprecise")).ToDictionary(Function(x) x.Title.Split.First)
        Dim LQuery = (From sId As String In query Where fasta.ContainsKey(sId) Select fasta(sId)).ToArray
        Dim path As String = args("/bbh") & $"_{id}.fasta"
        Return New SMRUCC.genomics.SequenceModel.FASTA.FastaFile(LQuery).Save(path).CLICode
    End Function

    <ExportAPI("/Trim.MastSite",
               Usage:="/Trim.MastSite /in <mastSite.Csv> /locus <locus_tag> /correlations <DIR/name> [/out <out.csv> /cut <0.65>]")>
    Public Function Trim(args As CommandLine.CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim locus As String = args("/locus")
        Dim correlations As String = args("/correlations")
        Dim cut As Double = args.GetValue("/cut", 0.65)
        Dim out As String = args.GetValue("/out", inFile.TrimFileExt & "-" & locus & $"-{cut}/")
        Dim mastSites = inFile.LoadCsv(Of MastSites)
        Dim weights = Correlation2.LoadAuto(correlations)
        Dim list As String() = locus.Split(","c)

        For i As Integer = 0 To list.Length - 1
            Dim id As String = list(i)
            Dim LQuery = (From x As MastSites In mastSites.AsParallel
                          Let pcc = weights.GetPcc(id, x.Gene),
                              spcc = weights.GetSPcc(id, x.Gene)
                          Where Math.Abs(pcc) >= cut OrElse
                              Math.Abs(spcc) >= cut
                          Select x).ToArray
            Call LQuery.SaveTo(out & id & ".csv")
        Next

        Return 0
    End Function
End Module
