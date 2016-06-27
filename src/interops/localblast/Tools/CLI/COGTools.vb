Imports LANS.SystemsBiology.Assembly.NCBI
Imports LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.RpsBLAST
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports LANS.SystemsBiology.Assembly.DOOR
Imports LANS.SystemsBiology.Assembly.NCBI.COG
Imports Microsoft.VisualBasic.Language

Partial Module CLI

    <ExportAPI("/COG.Statics",
               Usage:="/COG.Statics /in <myva_cogs.csv> [/locus <locus.txt/csv> /locuMap <Gene> /out <out.csv>]")>
    Public Function COGStatics(args As CommandLine.CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim locus As String = args("/locus")
        Dim out As String
        If Not locus.FileExists Then
            out = args.GetValue("/out", inFile.TrimFileExt & ".COG.Stat.Csv")
        Else
            out = args.GetValue("/out", inFile.TrimFileExt & "." & IO.Path.GetFileNameWithoutExtension(locus) & ".COG.Stat.Csv")
        End If
        Dim myvaCogs = inFile.LoadCsv(Of MyvaCOG)

        If locus.FileExists Then
            Dim ext As String = IO.Path.GetExtension(locus)
            Dim locusTag As String()

            If String.Equals(ext, ".csv", StringComparison.OrdinalIgnoreCase) Then
                Dim temp = DocumentStream.EntityObject.LoadDataSet(locus, args.GetValue("/locusMap", "Gene"))
                locusTag = temp.ToArray(Function(x) x.Identifier)
            Else
                locusTag = locus.ReadAllLines
            End If

            myvaCogs = (From x As MyvaCOG In myvaCogs
                        Where Array.IndexOf(locusTag, x.QueryName) > -1
                        Select x).ToList
        End If

        Dim func As COG.Function = COG.Function.Default
        Dim stst = COGFunc.GetClass(myvaCogs, func)
        Return stst.SaveTo(out).CLICode
    End Function

    <ExportAPI("/EXPORT.COGs.from.DOOR",
               Usage:="/EXPORT.COGs.from.DOOR /in <DOOR.opr> [/out <out.csv>]")>
    Public Function ExportDOORCogs(args As CommandLine.CommandLine) As Integer
        Dim opr As String = args("/in")
        Dim out As String = args.GetValue("/out", opr.TrimFileExt & ".COGs.csv")
        Dim DOOR As DOOR = DOOR_API.Load(opr)

        Return (LinqAPI.MakeList(Of MyvaCOG) _
             <= From x As GeneBrief
                In DOOR.Genes
                Select New MyvaCOG With {
                    .COG = x.COG_number,
                    .MyvaCOG = x.COG_number,
                    .QueryName = x.Synonym,
                    .Description = x.Product,
                    .Category = x.COG_number
               }) >> OpenHandle(out)
    End Function

End Module