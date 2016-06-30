Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports LANS.SystemsBiology.AnalysisTools.DataVisualization.Interaction.Phylip.ShellScriptAPI
Imports LANS.SystemsBiology.NCBI.Extensions.Analysis
Imports LANS.SystemsBiology.AnalysisTools.DataVisualization.Interaction.Phylip.MatrixFile

Partial Module CLI

    ''' <summary>
    ''' 为phylip构建进化树创建遗传矩阵
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/venn.Matrix",
               Usage:="/venn.Matrix /besthits <besthits.xml.DIR> [/query <sp.name> /limits -1 /out <out.txt>]")>
    Public Function VennMatrix(args As CommandLine) As Integer
        Dim inDIR As String = args - "/besthits"
        Dim out As String = args.GetValue("/out", inDIR & $".{NameOf(VennMatrix)}.txt")
        Dim source As BestHit() = LoadHitsVennData(inDIR)
        Dim limits As Integer = args.GetInt32("/limits")
        Dim query As String = args - "/query"
        Dim gendist As Gendist = source.ExportGendistMatrixFromBesthitMeta(query, Limits:=limits)
        Call gendist.MATRaw.Save(out.TrimFileExt & ".csv", Encodings.ASCII)
        Call gendist.GenerateDocument.SaveTo(out, Encodings.ASCII.GetEncodings)
        Return 0
    End Function
End Module
