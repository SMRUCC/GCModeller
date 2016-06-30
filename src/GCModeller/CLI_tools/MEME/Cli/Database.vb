Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Workflows.PromoterParser
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat
Imports LANS.SystemsBiology.DatabaseServices.ComparativeGenomics.AnnotationTools.RegpreciseRegulations
Imports LANS.SystemsBiology.DatabaseServices.Regprecise
Imports LANS.SystemsBiology.SequenceModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DataSetHandler

Partial Module CLI

    ''' <summary>
    ''' 初始化应用程序模块的时候自动执行初始化代码
    ''' </summary>
    Sub New()
        Call Settings.Session.Initialize()
        Call InitHandle()
    End Sub

    <ExportAPI("--Get.Intergenic",
               Usage:="--Get.Intergenic /PTT <genome.ptt> /nt <genome.fasta> [/o <out.fasta> /len 100 /strict]")>
    Public Function GetIntergenic(args As CommandLine) As Integer
        Dim len As Integer = args.GetValue("/len", 100)
        Dim strict As Boolean = args.GetBoolean("/strict")
        Dim PTT As PTT = TabularFormat.PTT.Read(args("/ptt"))
        Dim outDIR As String = args.GetValue("/o", args("/nt").TrimFileExt & $".intergenic.{len}bp.{If(strict, "strict", "")}.fasta")
        Dim NT As New FASTA.FastaToken(args("/nt"))
        Dim fa As FASTA.FastaFile =
            IntergenicSigma70.Sigma70Parser(NT, PTT, Length:=len, StrictOverlap:=strict)
        Return fa.Save(outDIR).CLICode
    End Function

    <ExportAPI("/Export.Regprecise.Motifs")>
    Public Function ExportRegpreciseMotifs(args As CommandLine) As Integer
        Call Compiler.SitesFamilyCategory(GCModeller.FileSystem.RegpreciseRoot)
        Return 0
    End Function

    <ExportAPI("/MAST_LDM.Build", Usage:="/MAST_LDM.Build /source <sourceDIR> [/out <exportDIR:=./> /evalue <1e-3>]")>
    Public Function BuildPWMDb(args As CommandLine) As Integer
        Dim inDIR As String = args("/source")
        Dim out As String = args.GetValue("/out", App.CurrentDirectory & "/MAST_LDM." & FileIO.FileSystem.GetDirectoryInfo(inDIR).Name)
        Dim evalue As Double = args.GetValue("/evalue", 0.001)
        Call RegpreciseShellScriptAPI.BuildPWM(inDIR, out, evalue)
        Return 0
    End Function
End Module