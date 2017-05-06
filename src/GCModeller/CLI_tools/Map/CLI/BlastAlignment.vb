Imports System.Drawing
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Interops.NCBI.Extensions
Imports SMRUCC.genomics.Interops.NCBI.Extensions.NCBIBlastResult
Imports SMRUCC.genomics.Visualize.ComparativeGenomics.ModelAPI
Imports SMRUCC.genomics.Visualize.NCBIBlastResult

Partial Module CLI

    ''' <summary>
    ''' 这个函数是从编译好的blast bbh xml结果之中绘图
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Visual.BBH",
               Usage:="/Visual.BBH /in <bbh.Xml> /PTT <genome.PTT> /density <genomes.density.DIR> [/limits <sp-list.txt> /out <image.png>]")>
    <Argument("/PTT", False,
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

        Dim scores As Func(Of Analysis.Hit, Double) = BBHMetaAPI.DensityScore(density, scale:=20)
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

    <ExportAPI("/Visualize.blastn.alignment",
               Usage:="/Visualize.blastn.alignment /in <alignmentTable.txt> /PTT <genome.PTT> [/local /out <image.png>]",
               Info:="Blastn result alignment visualization from the NCBI web blast.")>
    <Argument("/local", Description:="The file for ``/in`` parameter is a local blastn output result file?")>
    Public Function BlastnVisualizeWebResult(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim PTT$ = args <= "/PTT"
        Dim out$ = args.GetValue("/out", [in].TrimSuffix & ".blastn.visualize.png")
        Dim local As Boolean = args.GetBoolean("/local")

        If local Then
        Else

        End If
    End Function
End Module