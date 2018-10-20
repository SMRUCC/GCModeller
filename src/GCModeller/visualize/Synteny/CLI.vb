Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Scripting
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application
Imports SMRUCC.genomics.Visualize.SyntenyVisualize.ComparativeGenomics

Module CLI

    ''' <summary>
    ''' Plot of the blastn mapping result
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/mapping.plot")>
    <Usage("/mapping.plot /mapping <blastn_mapping.csv> /query <query.gff3> /ref <subject.gff3> [/grep <default=""-""> /out <Synteny.png>]")>
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
            .LinkFromBlastnMaps(mappings, grep)

        Return New DrawingDevice() _
            .InvokeDrawing(plotModel) _
            .SaveAs(out) _
            .CLICode
    End Function
End Module
