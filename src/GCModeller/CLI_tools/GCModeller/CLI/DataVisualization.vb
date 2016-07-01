Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports SMRUCC.genomics.AnalysisTools.DataVisualization.Interaction
Imports SMRUCC.genomics.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports SMRUCC.genomics.AnalysisTools.DataVisualization.Interaction.Phylip
Imports SMRUCC.genomics.AnalysisTools.SequenceTools.SequencePatterns
Imports System.Drawing
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.AnalysisTools.DataVisualization
Imports SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Application.RpsBLAST
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.AnalysisTools.ProteinTools.Sanger.Pfam.ProteinDomainArchitecture.MPAlignment
Imports Microsoft.VisualBasic.ComponentModel
Imports SMRUCC.genomics.NCBI.Extensions.Analysis
Imports SMRUCC.genomics.AnalysisTools.DataVisualization.ComparativeAlignment
Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.genomics.NCBI.Extensions.NCBIBlastResult
Imports SMRUCC.genomics.NCBI.Extensions
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.AnalysisTools.DataVisualization.NCBIBlastResult
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.AnalysisTools.DataVisualization.ComparativeGenomics.ModelAPI

Partial Module CLI

    <ExportAPI("/Draw.Comparative", Usage:="/Draw.Comparative /in <meta.Xml> /PTT <PTT_DIR> [/out <outDIR>]")>
    Public Function DrawMultipleAlignments(args As CommandLine.CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim PTT As String = args("/PTT")
        Dim out As String = args.GetValue("/out", [in].TrimFileExt & "-Comparative/")
        Dim meta As BestHit = [in].LoadXml(Of BestHit)
        Dim model As DrawingModel = MetaAPI.FromMetaData(meta, PTT)
    End Function

    <ExportAPI("--Drawing.ClustalW",
               Usage:="--Drawing.ClustalW /in <align.fasta> [/out <out.png> /dot.Size 10]")>
    Public Function DrawClustalW(args As CommandLine.CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim out As String = args.GetValue("/out", inFile.TrimFileExt & ".png")
        Dim aln As New FASTA.FastaFile(inFile)
        Call ClustalVisual.SetDotSize(args.GetValue("/dot.size", 10))
        Dim res As Image = ClustalVisual.InvokeDrawing(aln)
        Return res.SaveAs(out, ImageFormats.Png)
    End Function

    <ExportAPI("--Drawing.ChromosomeMap",
               Info:="Drawing the chromosomes map from the PTT object as the basically genome information source.",
               Usage:="--Drawing.ChromosomeMap /ptt <genome.ptt> [/conf <config.inf> /out <dir.export> /COG <cog.csv>]")>
    Public Function DrawingChrMap(args As CommandLine.CommandLine) As Integer
        Dim PTT = args.GetObject("/ptt", AddressOf TabularFormat.PTT.Load)
        Dim confInf As String = args("/conf")
        Dim out As String = args.GetValue("/out", App.CurrentDirectory)
        Dim config As ChromosomeMap.Configurations

        If String.IsNullOrEmpty(confInf) Then
            confInf = out & "/config.inf"
        End If

        If Not confInf.FileExists Then
Create:     config = ChromosomeMap.GetDefaultConfiguration(confInf)
        Else
            Try
                config = ChromosomeMap.LoadConfig(confInf)
            Catch ex As Exception
                GoTo Create
            End Try
        End If

        Dim COG As String = args("/COG")
        Dim COGProfiles As MyvaCOG() = COG.LoadCsv(Of MyvaCOG).ToArray
        Dim Device = ChromosomeMap.CreateDevice(config)
        Dim Model = ChromosomeMap.FromPTT(PTT, config)
        Model = ChromosomeMap.ApplyCogColorProfile(Model, COGProfiles)
        Dim res = Device.InvokeDrawing(Model)

        Return ChromosomeMap.SaveImage(res, out, "tiff")
    End Function

    <ExportAPI("--Gendist.From.Self.Overviews", Usage:="--Gendist.From.Self.Overviews /blast_out <blast_out.txt>")>
    Public Function SelfOverviewAsMAT(args As CommandLine.CommandLine) As Integer
        Dim blastOut As String = args("/blast_out")
        Dim outLog = BlastPlus.Parser.TryParse(blastOut)
        Dim MAT = SelfOverviewsGendist(outLog)
        Dim path As String = args("/blast_out").TrimFileExt & "Gendist.txt"
        Call MAT.GenerateDocument.SaveTo(path)
        Return MAT.lstID.SaveTo(path.TrimFileExt & ".lstID.csv")
    End Function

    <ExportAPI("--Gendist.From.SelfMPAlignment", Usage:="--Gendist.From.SelfMPAlignment /aln <mpalignment.csv>")>
    Public Function SelfMPAlignmentAsMAT(args As CommandLine.CommandLine) As Integer
        Dim out As String = args("/aln")
        Dim aln = out.LoadCsv(Of MPCsvArchive)
        Dim MAT = Phylip.MPAlignmentAsTree(aln)
        Dim path As String = out.TrimFileExt & ".MPAln_Gendist.txt"
        Call MAT.GenerateDocument.SaveTo(path)
        Return MAT.lstID.SaveTo(path.TrimFileExt & ".lstID.csv")
    End Function

    <ExportAPI("--Get.Subset.lstID", Usage:="--Get.Subset.lstID /subset <lstID.txt> /lstID <lstID.csv>")>
    Public Function GetSubsetID(args As CommandLine.CommandLine) As Integer
        Dim subset As String() = IO.File.ReadAllLines(args("/subset"))
        Dim lstID = args("/lstID").LoadCsv(Of KeyValuePair)
        Dim LQuery As String() =
            LinqAPI.Exec(Of String) <= From sId As String
                                       In subset
                                       Let getID As String = (From x As KeyValuePair
                                                              In lstID
                                                              Where String.Equals(sId, x.Key, StringComparison.OrdinalIgnoreCase)
                                                              Select x.Value).FirstOrDefault
                                       Where Not String.IsNullOrEmpty(getID)
                                       Select getID
        Dim path As String = args("/subset").TrimFileExt & ".lstID.txt"
        Return LQuery.FlushAllLines(path).CLICode
    End Function

    <ExportAPI("/Visual.BBH",
               Usage:="/Visual.BBH /in <bbh.Xml> /PTT <genome.PTT> /density <genomes.density.DIR> [/limits <sp-list.txt> /out <image.png>]")>
    <ParameterInfo("/PTT", False,
                   Description:="A directory which contains all of the information data files for the reference genome, 
                   this directory would includes *.gb, *.ptt, *.gff, *.fna, *.faa, etc.")>
    Public Function BBHVisual(args As CommandLine.CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim PTT As String = args("/PTT")
        Dim out As String = args.GetValue("/out", [in].TrimFileExt & ".visualize.png")
        Dim meta As Analysis.BestHit = [in].LoadXml(Of Analysis.BestHit)
        Dim limits As String() = args("/limits").ReadAllLines
        Dim density As String = args("/density")

        If Not limits.IsNullOrEmpty Then
            meta = meta.Take(limits)
        End If

        Dim scores As Func(Of Analysis.Hit, Double) =
            BBHMetaAPI.DensityScore(density, scale:=20)
        Dim PTTdb As PTT = TabularFormat.PTT.Load(PTT)
        Dim table As AlignmentTable =
            BBHMetaAPI.DataParser(meta,
                                  PTTdb,
                                  visualGroup:=True,
                                  scoreMaps:=scores)

        Call $"Min:={table.Hits.Min(Function(x) x.Identity)}, Max:={table.Hits.Max(Function(x) x.Identity)}".__DEBUG_ECHO

        Dim densityQuery As ICOGsBrush = ColorSchema.IdentitiesBrush(scores)
        Dim res As Image = BlastVisualize.InvokeDrawing(table,
                                                        PTTdb,
                                                        AlignmentColorSchema:="identities",
                                                        IdentityNoColor:=False,
                                                        queryBrush:=densityQuery)
        Return res.SaveAs(out, ImageFormats.Png).CLICode
    End Function
End Module
