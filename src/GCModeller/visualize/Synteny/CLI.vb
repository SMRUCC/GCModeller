Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService.SharedORM
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Scripting
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application
Imports SMRUCC.genomics.Visualize.SyntenyVisualize.ComparativeGenomics

<CLI> Module CLI

    ''' <summary>
    ''' Plot of the blastn mapping result
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 对于常见的fasta标题，可以使用脚本``tokens | first``
    ''' </remarks>
    <ExportAPI("/mapping.plot")>
    <Usage("/mapping.plot /mapping <blastn_mapping.csv> /query <query.gff3> /ref <subject.gff3> [/Ribbon <default=Spectral:c6> /size <default=6000,4000> /auto.reverse <default=0.9> /grep <default=""-""> /out <Synteny.png>]")>
    Public Function PlotMapping(args As CommandLine) As Integer
        Dim in$ = args <= "/mapping"
        Dim query$ = args <= "/query"
        Dim ref$ = args <= "/ref"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.synteny.png"
        Dim mappings = [in].LoadCsv(Of BlastnMapping).ToArray
        Dim queryGff = GFF.Load(query)
        Dim refGff = GFF.Load(ref)
        Dim grep As TextGrepScriptEngine = TextGrepScriptEngine.Compile(args("/grep"))
        Dim plotModel As DrawingModel = (queryGff, refGff) _
            .SyntenyTuple _
            .LinkFromBlastnMaps(
                maps:=mappings,
                grepOp:=grep,
                ribbonColors:=args("/ribbon") Or "Spectral:c6"
            ) _
            .AutoReverse(args("/auto.reverse") Or 0.9)

        Return New DrawingDevice() _
            .Plot(plotModel,
                  canvasSize:=args("/size") Or "6000,3000",
                  margin:="padding: 300px 100px 1200px 100px"
            ) _
            .SaveAs(out) _
            .CLICode
    End Function

    <ExportAPI("/test")>
    Public Function Test() As Integer
        Call Synteny.test.batch()
    End Function
End Module
