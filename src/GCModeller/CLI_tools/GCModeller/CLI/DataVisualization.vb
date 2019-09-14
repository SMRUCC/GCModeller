#Region "Microsoft.VisualBasic::6998c21c5b3eadb65610690157870d80, CLI_tools\GCModeller\CLI\DataVisualization.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:

    ' Module CLI
    ' 
    '     Function: DrawClustalW, DrawMultipleAlignments, GetSubsetID, SelfMPAlignmentAsMAT, SelfOverviewAsMAT
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns
Imports SMRUCC.genomics.Data.Xfam.Pfam.ProteinDomainArchitecture.MPAlignment
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Tasks.Models
Imports SMRUCC.genomics.Interops.Visualize.Phylip
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.Visualize.SyntenyVisualize.ComparativeAlignment

Partial Module CLI

    <ExportAPI("/Draw.Comparative", Usage:="/Draw.Comparative /in <meta.Xml> /PTT <PTT_DIR> [/out <outDIR>]")>
    <Group(CLIGrouping.DataVisualizeTools)>
    Public Function DrawMultipleAlignments(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim PTT As String = args("/PTT")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-Comparative/")
        Dim meta As SpeciesBesthit = [in].LoadXml(Of SpeciesBesthit)
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
End Module
