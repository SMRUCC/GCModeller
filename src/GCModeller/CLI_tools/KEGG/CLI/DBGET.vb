#Region "Microsoft.VisualBasic::b2d1ab7bbb9603a657a165e02e61c50b, CLI_tools\KEGG\CLI\DBGET.vb"

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
    '     Function: DownloadCompounds, DownloadKEGGReaction, DownloadPathwayMaps, DownloadPathwayMapsBatchTask, DownloadReferenceMapDatabase
    '               DownloadReferenceModule, DownloadsAllPathways, DownloadsBacteriasRefMaps, DumpKEGGMaps
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.ELIXIR.EBI.ChEBI.Database.IO.StreamProviders.Tsv.Tables
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Organism
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.ReferenceMap
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Metagenomics
Imports kegMap = SMRUCC.genomics.Assembly.KEGG.WebServices.MapDownloader
Imports org = SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry.Organism

Partial Module CLI

    ''' <summary>
    ''' 使用这个工具下载KEGG之中的代谢反应的模型信息
    ''' 
    ''' 20181019 测试OK
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Download.Reaction", Usage:="/Download.Reaction [/compounds <compounds.directory> /save <DIR> /@set sleep=2000]")>
    <Description("Downloads the KEGG enzyme reaction reference model data.")>
    <Group(CLIGroups.DBGET_tools)>
    <Argument("/compounds", True, CLITypes.File,
              Description:="If this argument is present in the commandline, then it means only this collection of compounds related reactions will be download.")>
    Public Function DownloadKEGGReaction(args As CommandLine) As Integer
        Dim save$ = args("/save") Or "./br08201/"
        Dim compounds$ = args <= "/compounds"

        If compounds.DirectoryExists Then

        Else
            Return EnzymaticReaction _
                .DownloadReactions(save, cache:=$"{save}/.br08201/") _
                .SaveTo(save & "/failures.txt") _
                .CLICode
        End If
    End Function

    ''' <summary>
    ''' gif图片是以base64编码放在XML文件里面的
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Download.Compounds")>
    <Description("Downloads the KEGG compounds data from KEGG web server using dbget API")>
    <Usage("/Download.Compounds [/chebi <accessions.tsv> /flat /updates /save <DIR>]")>
    <Argument("/chebi", True, CLITypes.File,
              AcceptTypes:={GetType(Accession)},
              Description:="Some compound metabolite in the KEGG database have no brite catalog info, then using the brite database for the compounds downloads will missing some compounds, 
              then you can using this option for downloads the complete compounds data in the KEGG database.")>
    <Group(CLIGroups.DBGET_tools)>
    Public Function DownloadCompounds(args As CommandLine) As Integer
        Dim save$ = args("/save") Or "./KEGG_cpd/"
        Dim flat As Boolean = args("/flat")
        Dim updates As Boolean = args("/updates")

        Call CompoundBrite.DownloadFromResource(
            EXPORT:=save,
            directoryOrganized:=Not flat,
            structInfo:=True
        )

        ' 下载补充数据
        Dim accs As String = args <= "/chebi"

        If accs.FileExists(True) Then
            Call MetaboliteWebApi.CompleteUsingChEBI(save, accs, updates)
        End If

        Return 0
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

    ''' <summary>
    ''' 下载指定物种编号的物种基因组之中所有的pathway信息，包括代谢物和基因
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Download.Pathway.Maps")>
    <Usage("/Download.Pathway.Maps /sp <kegg.sp_code> [/KGML /out <EXPORT_DIR> /@set <progress_bar=disabled>]")>
    <Description("Fetch all of the pathway map information for a specific kegg organism by using a specifc kegg sp code.")>
    <Argument("/sp", False, CLITypes.String,
              PipelineTypes.std_in,
              AcceptTypes:={GetType(String)},
              Description:="The 3 characters kegg organism code, example as: ""xcb"" is stands for organism ""Xanthomonas campestris pv. campestris 8004 (Beijing)""")>
    <Group(CLIGroups.DBGET_tools)>
    <LastUpdated("2019-06-22 21:00:00.00")>
    Public Function DownloadPathwayMaps(args As CommandLine) As Integer
        Dim sp As String = args("/sp")
        Dim EXPORT As String = args("/out") Or (App.CurrentDirectory & "/" & sp)
        Dim isKGML As Boolean = args.IsTrue("/KGML")
        Dim infoJSON$ = $"{EXPORT}/kegg.json"

        Call Apps.KEGG_tools.ShowOrganism(code:=sp, out:=infoJSON)

        With infoJSON.LoadJSON(Of OrganismInfo)
            Dim assembly$ = .DataSource _
                            .Where(Function(d)
                                       Return InStr(d.text, "https://www.ncbi.nlm.nih.gov/assembly/", CompareMethod.Text) > 0
                                   End Function) _
                            .First _
                            .name

            ' 在这里写入两个空文件是为了方便进行标记
            Call "".SaveTo($"{EXPORT}/{ .FullName}.txt")
            Call "".SaveTo($"{EXPORT}/{assembly}.txt")
            ' 这个文件方便程序进行信息的读取操作
            Call { .FullName, assembly}.FlushAllLines($"{EXPORT}/index.txt")
        End With

        If isKGML AndAlso args("/out").IsEmpty Then
            EXPORT &= ".KGML/"

            Return MapDownloader _
                .DownloadsKGML(sp, EXPORT) _
                .SaveTo(EXPORT & "/failures.txt") _
                .CLICode
        Else
            Return LinkDB.Pathways _
                .Downloads(sp, EXPORT, cache:=EXPORT & "/.kegg/") _
                .SaveTo(EXPORT & "/failures.txt") _
                .CLICode
        End If
    End Function

    <ExportAPI("/Download.Pathway.Maps.Batch")>
    <Usage("/Download.Pathway.Maps.Batch /sp <kegg.sp_code.list> [/KGML /out <EXPORT_DIR>]")>
    <Group(CLIGroups.DBGET_tools)>
    Public Function DownloadPathwayMapsBatchTask(args As CommandLine) As Integer
        Dim sp$ = args("/sp")
        Dim out$ = args("/out") Or $"{sp.TrimSuffix}/"
        Dim isKGML As Boolean = args.IsTrue("/KGML")

        For Each id As String In sp.IterateAllLines.Select(Function(l) l.StringSplit("\s+").First)
            Dim directory$ = $"{out}/{id}/"
            Call Apps.KEGG_tools.DownloadPathwayMaps(sp:=id, out:=directory, kgml:=isKGML, _set:="progress_bar=disabled")
        Next

        Return 0
    End Function

    <ExportAPI("/Download.Pathway.Maps.Bacteria.All")>
    <Usage("/Download.Pathway.Maps.Bacteria.All [/in <brite.keg> /KGML /out <out.directory>]")>
    <Group(CLIGroups.DBGET_tools)>
    Public Function DownloadsBacteriasRefMaps(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$
        Dim htext As htext

        If [in].FileExists Then
            out = args("/out") Or ([in].TrimSuffix & ".bacteria.All/")
            htext = htext.StreamParser([in])
        Else
            out = args("/out") Or (App.CurrentDirectory & $"/bacteria.All/")
            htext = org.GetResource
        End If

        Dim codes = htext.GetBacteriaList
        Dim EXPORT$

        For Each code As Taxonomy In codes
            EXPORT = code.scientificName.NormalizePathString
            EXPORT = $"{out}/{EXPORT}/"

            Apps.KEGG_tools.DownloadPathwayMaps(
                sp:=code.species,
                out:=EXPORT
            )
        Next

        Return 0
    End Function

    <ExportAPI("/Download.Module.Maps",
               Info:="Download the KEGG reference modules map data.",
               Usage:="/Download.Module.Maps [/out <EXPORT_DIR, default=""./"">]")>
    Public Function DownloadReferenceModule(args As CommandLine) As Integer
        Dim out$ = args.GetValue("/out", "./")

    End Function

    ''' <summary>
    ''' 这里下载的是标准的参考图数据
    ''' 
    ''' 包含有pathway的定义
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Pathways.Downloads.All",
               Info:="Download all of the KEGG reference pathway map data.",
               Usage:="/Pathways.Downloads.All [/out <outDIR>]")>
    <Group(CLIGroups.DBGET_tools)>
    Public Function DownloadsAllPathways(args As CommandLine) As Integer
        Dim EXPORT = args("/out") Or (App.HOME & "/br08901/")

        If PathwayMap.DownloadAll(EXPORT) <> 0 Then
            Call "Some maps file download failured, please check error logs for detail information...".Warning
            Return -10
        Else
            Return 0
        End If
    End Function

    ''' <summary>
    ''' 这个是只下载图，上面包含空白的pathway图以及shapes和每一个shapes的位置
    ''' 下载的数据可以用于KEGG富集分析的结果可视化
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/dump.kegg.maps")>
    <Description("Dumping the KEGG maps database for human species.")>
    <Usage("/dump.kegg.maps [/htext <htext.txt> /out <save_dir>]")>
    <Argument("/htext", False, CLITypes.File,
              Extensions:="*.txt",
              Description:="The KEGG category term provider")>
    <Argument("/out", True, CLITypes.File,
              Description:="A directory path that contains the download KEGG reference pathway map model data, this output can be using as the KEGG pathway map rendering repository source.")>
    <Group(CLIGroups.DBGET_tools)>
    Public Function DumpKEGGMaps(args As CommandLine) As Integer
        Dim htext$ = args <= "/htext"

        With (args <= "/out") Or $"./br08901_pathwayMaps/".AsDefault
            Return kegMap.Downloads(EXPORT:= .ByRef, briefFile:=htext) _
                .GetJson _
                .SaveTo(.ByRef & "/failures.json") _
                .CLICode
        End With
    End Function
End Module
