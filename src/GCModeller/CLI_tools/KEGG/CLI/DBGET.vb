Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.Assembly.EBI.ChEBI.Database.IO.StreamProviders.Tsv.Tables
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

    <ExportAPI("/Download.Compounds",
               Info:="Downloads the KEGG compounds data from KEGG web server using dbget API",
               Usage:="/Download.Compounds [/chebi <accessions.tsv> /flat /updates /save <DIR>]")>
    <Argument("/chebi", True, CLITypes.File,
              AcceptTypes:={GetType(Accession)},
              Description:="Some compound metabolite in the KEGG database have no brite catalog info, then using the brite database for the compounds downloads will missing some compounds, then you can using this option for downloads the complete compounds data in the KEGG database.")>
    <Group(CLIGroups.DBGET_tools)>
    Public Function DownloadCompounds(args As CommandLine) As Integer
        Dim save$ = args.GetValue("/save", "./KEGG_cpd/")
        Dim flat As Boolean = args.GetBoolean("/flat")
        Dim updates As Boolean = args.GetBoolean("/updates")
        Dim failures As New List(Of String)(BriteHEntry.Compound.DownloadFromResource(save, Not flat, updates))
        ' 下载补充数据
        Dim accs As String = args <= "/chebi"
        If accs.FileExists(True) Then
            failures += MetabolitesDBGet.CompleteUsingChEBI(save, accs, updates)
        End If
        Return failures.SaveTo(save & "/failures.txt").CLICode
    End Function

    <ExportAPI("-ref.map.download", Usage:="-ref.map.download -o <out_dir>")>
    <Group(CLIGroups.DBGET_tools)>
    Public Function DownloadReferenceMapDatabase(argvs As CommandLine) As Integer
        Dim EXPORT As String = argvs("-o")
        Dim IDList = BriteHEntry.Pathway.LoadFromResource
        Dim Downloads = LinqAPI.Exec(Of Boolean) <=
 _
            From ID As BriteHEntry.Pathway
            In IDList
            Let MapID As String = "map" & ID.EntryId
            Let Map = ReferenceMapData.Download(MapID)
            Let save As String = EXPORT & "/" & MapID & ".xml"
            Select Map.GetXml.SaveTo(save)

        Return 0
    End Function

    <ExportAPI("/Download.Pathway.Maps",
               Usage:="/Download.Pathway.Maps /sp <kegg.sp_code> [/out <EXPORT_DIR>]")>
    <Group(CLIGroups.DBGET_tools)>
    Public Function DownloadPathwayMaps(args As CommandLine) As Integer
        Dim sp As String = args("/sp")
        Dim EXPORT As String = args.GetValue("/out", App.CurrentDirectory & "/" & sp)
        Return LinkDB.Pathways _
            .Downloads(sp, EXPORT) _
            .SaveTo(EXPORT & "/failures.txt") _
            .CLICode
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