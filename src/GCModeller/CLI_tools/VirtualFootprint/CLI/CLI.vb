Imports LANS.SystemsBiology.AnalysisTools.SequenceTools
Imports LANS.SystemsBiology.AnalysisTools.SequenceTools.SequencePatterns
Imports LANS.SystemsBiology.AnalysisTools.SequenceTools.SequencePatterns.Motif
Imports LANS.SystemsBiology.DatabaseServices.ComparativeGenomics
Imports LANS.SystemsBiology.DatabaseServices.Regprecise
Imports LANS.SystemsBiology.InteractionModel.Network.VirtualFootprint
Imports LANS.SystemsBiology.SequenceModel
Imports LANS.SystemsBiology.Toolkits.RNA_Seq
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DataVisualization.Network.FileStream
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Scripting.MetaData

<PackageNamespace("VirtualFootprint.CLI", Category:=APICategories.CLI_MAN)>
Module CLI

    ''' <summary>
    ''' 合并bbh得到的regulon，得到可能的完整的regulon
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Merge.Regulons", Usage:="/Merge.Regulons /in <regulons.bbh.inDIR> [/out <out.csv>]")>
    Public Function MergeRegulonsExport(args As CommandLine.CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim out As String = args.GetValue("/out", inDIR & ".RegulonsMerged.Csv")
        Dim Merges As RegPreciseRegulon() = RegPreciseRegulon.Merges(inDIR)
        Return Merges.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Trim.Regulons", Usage:="/Trim.Regulons /in <regulons.csv> /pcc <pccDIR/sp_code> [/out <out.csv> /cut 0.65]")>
    Public Function TrimRegulon(args As CommandLine.CommandLine) As Integer
        Dim inRegulons As String = args("/in")
        Dim out As String = args.GetValue("/out", inRegulons.TrimFileExt & ".Trim.Csv")
        Dim pcc As String = args("/pcc")
        Dim PccDb As Correlation2
        Dim cut As Double = args.GetValue("/cut", 0.65)

        If pcc.DirectoryExists Then
            PccDb = New Correlation2(pcc)
        Else
            PccDb = Correlation2.CreateFromName(pcc)
        End If

        Dim source = inRegulons.LoadCsv(Of RegPreciseRegulon)

        For Each x As RegPreciseRegulon In source
            x.Members = (From sId As String
                         In x.Members.AsParallel
                         Let pccn As Double = PccDb.GetPcc(x.Regulator, sId),
                             spcc As Double = PccDb.GetSPcc(x.Regulator, sId)
                         Where Math.Abs(pccn) >= cut OrElse
                             Math.Abs(spcc) >= cut
                         Select sId).ToArray
        Next

        source = (From x In source Where Not StringHelpers.IsNullOrEmpty(x.Members) Select x).ToList

        Return source.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Write.Network", Usage:="/Write.Network /in <regulons.csv> [/out <netDIR>]")>
    Public Function SaveNetwork(args As CommandLine.CommandLine) As Integer
        Dim inRegulons As String = args("/in")
        Dim out As String = args.GetValue("/out", inRegulons.TrimFileExt & ".net/")
        Dim regulons = inRegulons.LoadCsv(Of RegPreciseRegulon)
        Dim net = RegPreciseRegulon.ToNetwork(regulons)
        Return net.Save(out, Encodings.ASCII).CLICode
    End Function

    <ExportAPI("/Motif.From.MAL", Usage:="/Motif.From.MAL /in <clustal.fasta> /out <outDIR>")>
    Public Function MotifFromMAL(args As CommandLine.CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimFileExt & ".Motif/")
        Dim motif As MotifPWM = FromMla(FASTA.FastaFile.LoadNucleotideData([in]))
        Call motif.SaveAsXml(out & "/Motif.Xml")

        Return 0
    End Function
End Module
