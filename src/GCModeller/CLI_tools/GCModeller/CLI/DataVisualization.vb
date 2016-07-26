#Region "Microsoft.VisualBasic::a488f42e42f211ed69762b8d715af513, ..\GCModeller\CLI_tools\GCModeller\CLI\DataVisualization.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Data.Xfam.Pfam.ProteinDomainArchitecture.MPAlignment
Imports SMRUCC.genomics.Interops.NCBI.Extensions
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Analysis
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.RpsBLAST
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult
Imports SMRUCC.genomics.Interops.Visualize.Phylip
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.Visualize
Imports SMRUCC.genomics.Visualize.ComparativeAlignment
Imports SMRUCC.genomics.Visualize.ComparativeGenomics.ModelAPI
Imports SMRUCC.genomics.Visualize.NCBIBlastResult

Partial Module CLI

    <ExportAPI("/Draw.Comparative", Usage:="/Draw.Comparative /in <meta.Xml> /PTT <PTT_DIR> [/out <outDIR>]")>
    Public Function DrawMultipleAlignments(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim PTT As String = args("/PTT")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-Comparative/")
        Dim meta As BestHit = [in].LoadXml(Of BestHit)
        Dim model As DrawingModel = MetaAPI.FromMetaData(meta, PTT)
    End Function

    <ExportAPI("--Drawing.ClustalW",
               Usage:="--Drawing.ClustalW /in <align.fasta> [/out <out.png> /dot.Size 10]")>
    Public Function DrawClustalW(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & ".png")
        Dim aln As New FASTA.FastaFile(inFile)
        Call ClustalVisual.SetDotSize(args.GetValue("/dot.size", 10))
        Dim res As Image = ClustalVisual.InvokeDrawing(aln)
        Return res.SaveAs(out, ImageFormats.Png)
    End Function

    <ExportAPI("--Drawing.ChromosomeMap",
               Info:="Drawing the chromosomes map from the PTT object as the basically genome information source.",
               Usage:="--Drawing.ChromosomeMap /ptt <genome.ptt> [/conf <config.inf> /out <dir.export> /COG <cog.csv>]")>
    Public Function DrawingChrMap(args As CommandLine) As Integer
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
    Public Function SelfOverviewAsMAT(args As CommandLine) As Integer
        Dim blastOut As String = args("/blast_out")
        Dim outLog = BlastPlus.Parser.TryParse(blastOut)
        Dim MAT = SelfOverviewsGendist(outLog)
        Dim path As String = args("/blast_out").TrimSuffix & "Gendist.txt"
        Call MAT.GenerateDocument.SaveTo(path)
        Return MAT.lstID.SaveTo(path.TrimSuffix & ".lstID.csv")
    End Function

    <ExportAPI("--Gendist.From.SelfMPAlignment", Usage:="--Gendist.From.SelfMPAlignment /aln <mpalignment.csv>")>
    Public Function SelfMPAlignmentAsMAT(args As CommandLine) As Integer
        Dim out As String = args("/aln")
        Dim aln = out.LoadCsv(Of MPCsvArchive)
        Dim MAT = Phylip.MPAlignmentAsTree(aln)
        Dim path As String = out.TrimSuffix & ".MPAln_Gendist.txt"
        Call MAT.GenerateDocument.SaveTo(path)
        Return MAT.lstID.SaveTo(path.TrimSuffix & ".lstID.csv")
    End Function

    <ExportAPI("--Get.Subset.lstID", Usage:="--Get.Subset.lstID /subset <lstID.txt> /lstID <lstID.csv>")>
    Public Function GetSubsetID(args As CommandLine) As Integer
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
        Dim path As String = args("/subset").TrimSuffix & ".lstID.txt"
        Return LQuery.FlushAllLines(path).CLICode
    End Function

    <ExportAPI("/Visual.BBH",
               Usage:="/Visual.BBH /in <bbh.Xml> /PTT <genome.PTT> /density <genomes.density.DIR> [/limits <sp-list.txt> /out <image.png>]")>
    <ParameterInfo("/PTT", False,
                   Description:="A directory which contains all of the information data files for the reference genome, 
                   this directory would includes *.gb, *.ptt, *.gff, *.fna, *.faa, etc.")>
    Public Function BBHVisual(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim PTT As String = args("/PTT")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".visualize.png")
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

