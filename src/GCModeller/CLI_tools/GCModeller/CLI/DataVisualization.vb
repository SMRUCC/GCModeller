#Region "Microsoft.VisualBasic::71e36ad6c96de538809084e50c8d0857, ..\GCModeller\CLI_tools\GCModeller\CLI\DataVisualization.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
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
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.Visualize
Imports SMRUCC.genomics.Visualize.ComparativeAlignment
Imports SMRUCC.genomics.Visualize.ComparativeGenomics.ModelAPI
Imports SMRUCC.genomics.Visualize.NCBIBlastResult

Partial Module CLI

    <ExportAPI("/Draw.Comparative", Usage:="/Draw.Comparative /in <meta.Xml> /PTT <PTT_DIR> [/out <outDIR>]")>
    <Group(CLIGrouping.DataVisualizeTools)>
    Public Function DrawMultipleAlignments(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim PTT As String = args("/PTT")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-Comparative/")
        Dim meta As BestHit = [in].LoadXml(Of BestHit)
        Dim model As DrawingModel = MetaAPI.FromMetaData(meta, PTT)
    End Function

    <ExportAPI("--Drawing.ClustalW",
               Usage:="--Drawing.ClustalW /in <align.fasta> [/out <out.png> /dot.Size 10]")>
    <Group(CLIGrouping.DataVisualizeTools)>
    Public Function DrawClustalW(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & ".png")
        Dim aln As New FASTA.FastaFile(inFile)
        Call ClustalVisual.SetDotSize(args.GetValue("/dot.size", 10))
        Dim res As Image = ClustalVisual.InvokeDrawing(aln)
        Return res.SaveAs(out, ImageFormats.Png)
    End Function

    <ExportAPI("--Gendist.From.Self.Overviews", Usage:="--Gendist.From.Self.Overviews /blast_out <blast_out.txt>")>
    Public Function SelfOverviewAsMAT(args As CommandLine) As Integer
        Dim blastOut As String = args("/blast_out")
        Dim outLog = BlastPlus.Parser.TryParse(blastOut)
        Dim MAT = SelfOverviewsGendist(outLog)
        Dim path As String = blastOut.TrimSuffix & "Gendist.txt"
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
        Dim lstID = (args <= "/lstID").LoadCsv(Of KeyValuePair)
        Dim LQuery As String() = LinqAPI.Exec(Of String) <=
 _
            From sId As String
            In subset
            Let getID As String = (From x As KeyValuePair
                                   In lstID
                                   Where String.Equals(sId, x.Key, StringComparison.OrdinalIgnoreCase)
                                   Select x.Value).FirstOrDefault
            Where Not String.IsNullOrEmpty(getID)
            Select getID

        Dim path As String = (args <= "/subset").TrimSuffix & ".lstID.txt"
        Return LQuery.FlushAllLines(path, Encodings.ASCII).CLICode
    End Function

    <ExportAPI("/Plot.GC", Usage:="/Plot.GC /in <mal.fasta> [/plot <gcskew/gccontent> /colors <Jet> /out <out.png>]")>
    <Group(CLIGrouping.DataVisualizeTools)>
    Public Function PlotGC(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim plot As String = args.GetValue("/plot", "gcskew")
        Dim colors As String = args.GetValue("/colors", "Jet")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-" & plot & "-" & colors & ".png")
        Dim img As GraphicsData = GCPlot.PlotGC(New FastaFile([in]), plot, 50, 50,,,,, colors:=colors)
        Return img.Save(out)
    End Function
End Module
