#Region "Microsoft.VisualBasic::2fc80c2b4153ee8443ce2d44a3f14b3e, CLI_tools\Map\CLI\BlastAlignment.vb"

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
    '     Function: BBHVisual, BlastnVisualizeWebResult, VisualOrthologyProfiles
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.BitmapImage
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ContextModel
Imports SMRUCC.genomics.Interops.NCBI.Extensions
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract
Imports SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult
Imports SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult.WebBlast
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Tasks.Models
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.Visualize
Imports SMRUCC.genomics.Visualize.NCBIBlastResult
Imports SMRUCC.genomics.Visualize.SyntenyVisualize.ComparativeGenomics.ModelAPI

Partial Module CLI

    <ExportAPI("/visual.orthology.profiles")>
    <Usage("/visual.orthology.profiles /in <bbh.csv> [/out <plot.png>]")>
    Public Function VisualOrthologyProfiles(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.orthology_profiles.png"
        Dim orthology As BBHIndex() = [in].LoadCsv(Of BBHIndex)

        Return orthology _
            .OrthologyProfiles(OrthologyProfiles.DefaultColors) _
            .Plot _
            .AsGDIImage _
            .SaveAs(out) _
            .CLICode
    End Function

    ''' <summary>
    ''' 这个函数是从编译好的blast bbh xml结果之中绘图
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Visual.BBH",
               Info:="Visualize the blastp result.",
               Usage:="/Visual.BBH /in <bbh.Xml> /PTT <genome.PTT> /density <genomes.density.DIR> [/limits <sp-list.txt> /out <image.png>]")>
    <ArgumentAttribute("/PTT", False,
              Description:="A directory which contains all of the information data files for the reference genome, this directory would includes *.gb, *.ptt, *.gff, *.fna, *.faa, etc.")>
    Public Function BBHVisual(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim PTTfile As String = args("/PTT")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".visualize.png")
        Dim meta As SpeciesBesthit = [in].LoadXml(Of SpeciesBesthit)
        Dim limits As String() = (args <= "/limits").ReadAllLines
        Dim density As String = args("/density")

        If Not limits.IsNullOrEmpty Then
            meta = meta.Take(limits)
        End If

        Dim scores As Func(Of Hit, Double) = BBHMetaAPI.DensityScore(density, scale:=20)
        Dim PTT As PTT = TabularFormat.PTT.Load(PTTfile)
        Dim table As AlignmentTable = BBHMetaAPI.DataParser(
            meta, PTT,
            visualGroup:=True,
            scoreMaps:=scores)

        Call $"Min:={table.Hits.Min(Function(x) x.Identity)}, Max:={table.Hits.Max(Function(x) x.Identity)}".__DEBUG_ECHO

        Dim densityQuery As ICOGsBrush = ColorSchema.IdentitiesBrush(scores)
        Dim res As Image = BlastVisualize.PlotMap(
            table, PTT,
            AlignmentColorSchema:="identities",
            IdentityNoColor:=False,
            queryBrush:=densityQuery)

        Return res.SaveAs(out, ImageFormats.Png).CLICode
    End Function

    <ExportAPI("/Visualize.blastn.alignment",
               Info:="Blastn result alignment visualization from the NCBI web blast. This tools is only works for a plasmid blastn search result or a small gene cluster region in a large genome.",
               Usage:="/Visualize.blastn.alignment /in <alignmentTable.txt> /genbank <genome.gb> [/ORF.catagory <catagory.tsv> /region <left,right> /local /out <image.png>]")>
    <ArgumentAttribute("/genbank", Description:="Provides the target genome coordinates for the blastn map plots.")>
    <ArgumentAttribute("/local", Description:="The file for ``/in`` parameter is a local blastn output result file?")>
    <ArgumentAttribute("/ORF.catagory", Description:="Using for the ORF shape color render, in a text file and each line its text format like: ``geneID``<TAB>``COG/KOG/GO/KO``")>
    Public Function BlastnVisualizeWebResult(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim gb$ = args <= "/genbank"
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & ".blastn.visualize.png")
        Dim cata$ = args <= "/ORF.catagory"
        Dim local As Boolean = args.GetBoolean("/local")
        Dim genbank As GBFF.File = GBFF.File.Load(gb)
        Dim regionValue$ = args <= "/region"
        Dim alignments As AlignmentTable

        If local Then
            alignments = AlignmentTableParser.CreateFromBlastnFile([in], 120)
        Else
            alignments = AlignmentTableParser.LoadTable([in], headerSplit:=True)
        End If

        GCSkew.Steps = 250

        Dim nt As FastaSeq = genbank.Origin.ToFasta
        Dim PTT As PTT = genbank.GbffToPTT(ORF:=True)
        Dim region As Location = alignments.GetAlignmentRegion

        If region.Length <= PTT.Size / 5 Then
            ' 这个比对结果是一个基因簇，则需要剪裁操作
            Call $"{[in].BaseName} probably is a cluster in genome {PTT.Title}.".__INFO_ECHO

            If regionValue.StringEmpty OrElse regionValue.IndexOf(","c) = -1 Then
                Throw New InvalidExpressionException("Gene cluster region range value is invalid! Please set /region value in format: (left,right).")
            End If

            ' 由于使用一个cluster片段直接比对的话，因为没有直接的位置信息，所以querystart是从1开始的
            ' 直接从比对结果之中解析出region，是从1开始的，不正确的
            ' 所以对于cluster类型而言需要使用手工的region范围输入
            region = New Location(CType(regionValue, IntRange))
            ' alignments = alignments.Offset(region)
            PTT = PTT.RangeSelection(region, offset:=True)
            nt = New FastaSeq({PTT.Title}, nt.CutSequenceLinear(region))
        End If

        If cata.FileLength() > 0 Then
            Dim category As Dictionary(Of NamedValue(Of String)) =
                cata _
                .ReadAllLines _
                .Select(Function(s) s.Split(ASCII.TAB)) _
                .Select(Function(g) New NamedValue(Of String)(g(0), g.ElementAtOrDefault(1))) _
                .ToDictionary()

            For Each gene As GeneBrief In PTT
                gene.COG = category(gene.Synonym).Value
                If gene.COG Is Nothing Then
                    gene.COG = ""
                End If
            Next
        End If

        Dim plot As Image = BlastVisualize.PlotMap(
            alignments, PTT,
            AlignmentColorSchema:="identities",
            IdentityNoColor:=False,
            QueryNT:=nt)

        Return plot _
            .CorpBlank(margin:=120, blankColor:=Color.White) _
            .SaveAs(out, ImageFormats.Png) _
            .CLICode
    End Function
End Module
