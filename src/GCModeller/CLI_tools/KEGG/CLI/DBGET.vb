Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.ReferenceMap

Partial Module CLI

    <ExportAPI("/Download.Reaction", Usage:="/Download.Reaction [/save <DIR>]")>
    <Group(CLIGroups.DBGET_tools)>
    Public Function DownloadKEGGReaction(args As CommandLine) As Integer
        Dim save$ = args.GetValue("/save", "./br08201/")
        Return EnzymaticReaction _
            .DownloadReactions(save) _
            .SaveTo(save & "/failures.txt") _
            .CLICode
    End Function

    <ExportAPI("/Download.Compounds", Usage:="/Download.Compounds [/flat /updates /save <DIR>]")>
    <Group(CLIGroups.DBGET_tools)>
    Public Function DownloadCompounds(args As CommandLine) As Integer
        Dim save$ = args.GetValue("/save", "./KEGG_cpd/")
        Dim flat As Boolean = args.GetBoolean("/flat")
        Dim updates As Boolean = args.GetBoolean("/updates")
        Dim failures As IEnumerable(Of String) = BriteHEntry.Compound.DownloadFromResource(save, Not flat, updates)
        Return failures.SaveTo(save & "/failures.txt").CLICode
    End Function

    <ExportAPI("-ref.map.download", Usage:="-ref.map.download -o <out_dir>")>
    <Group(CLIGroups.DBGET_tools)>
    Public Function DownloadReferenceMapDatabase(argvs As CommandLine) As Integer
        Dim OutDir As String = argvs("-o")
        Dim IDList = SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry.Pathway.LoadFromResource
        Dim DownloadLQuery = (From ID As SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry.Pathway
                              In IDList
                              Let MapID As String = "map" & ID.EntryId
                              Let Map = ReferenceMapData.Download(MapID)
                              Select Map.GetXml.SaveTo(OutDir & "/" & MapID & ".xml")).ToArray
        Return 0
    End Function

    <ExportAPI("/Download.Pathway.Maps",
               Usage:="/Download.Pathway.Maps /sp <kegg.sp_code> [/out <EXPORT_DIR>]")>
    <Group(CLIGroups.DBGET_tools)>
    Public Function DownloadPathwayMaps(args As CommandLine) As Integer
        Dim sp As String = args("/sp")
        Dim EXPORT As String = args.GetValue("/out", App.CurrentDirectory & "/" & sp)

        Call LinkDB.Pathways.Downloads(sp, EXPORT).ToArray

        Return 0
    End Function

    ''' <summary>
    ''' 这里下载的是标准的参考图数据
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Pathways.Downloads.All",
               Info:="Download all of the KEGG reference pathway map data.",
               Usage:="/Pathways.Downloads.All [/out <outDIR>]")>
    <Group(CLIGroups.DBGET_tools)>
    Public Function DownloadsAllPathways(args As CommandLine) As Integer
        Dim outDIR As String = args.GetValue("/out", App.HOME & "/br08901/")

        WebServiceUtils.Proxy = "http://127.0.0.1:8087/"

        If PathwayMap.DownloadAll(outDIR) <> 0 Then
            Call "Some maps file download failured, please check error logs for detail information...".Warning
            Return -10
        Else
            Return 0
        End If
    End Function
End Module