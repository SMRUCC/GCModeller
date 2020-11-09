#Region "Microsoft.VisualBasic::fabbe1b259892b97256c826ca9faa37c, CLI_tools\KEGG\CLI\DBGET.vb"

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
    '     Function: DownloadCompounds, DownloadKEGGReaction, DownloadPathwayMaps, DownloadPathwayMapsBatchTask, DownloadReactionClass
    '               DownloadReferenceMapDatabase, DownloadsAllPathways, DownloadsBacteriasRefMaps, HumanKEGGMaps
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
Imports Microsoft.VisualBasic.Linq.JoinExtensions
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.ELIXIR.EBI.ChEBI.Database.IO.StreamProviders.Tsv.Tables
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Organism
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.ReferenceMap
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.WebQuery
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Metagenomics
Imports kegMap = SMRUCC.genomics.Assembly.KEGG.WebServices.MapDownloader
Imports Organism = SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Organism.Organism
Imports OrganismHText = SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry.Organism

Partial Module CLI

    <ExportAPI("/Download.reaction_class")>
    <Usage("/Download.reaction_class [/save <dir, default=./>]")>
    Public Function DownloadReactionClass(args As CommandLine) As Integer
        Dim save$ = args("/save") Or "./"
        Dim list = ReactionClassWebQuery.DownloadReactionClass(save).ToArray

        Return list.GetJson _
            .SaveTo($"{save}/failures.json") _
            .CLICode
    End Function

    ''' <summary>
    ''' 使用这个工具下载KEGG之中的代谢反应的模型信息
    ''' 
    ''' 20181019 测试OK
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Download.Reaction")>
    <Usage("/Download.Reaction [/try_all /compounds <compounds.directory> /save <DIR> /@set sleep=2000]")>
    <Description("Downloads the KEGG enzyme reaction reference model data. Usually use these reference reaction data applied for metabolism network analysis.")>
    <Group(CLIGroups.DBGET_tools)>
    <ArgumentAttribute("/compounds", True, CLITypes.File,
              Description:="If this argument Is present in the commandline, then it means only this collection of compounds related reactions will be download.")>
    Public Function DownloadKEGGReaction(args As CommandLine) As Integer
        Dim save$ = args("/save") Or "./br08201/"
        Dim compounds$ = args <= "/compounds"

        If compounds.DirectoryExists Then
            Call CompoundRepository.ScanModels(directory:=compounds) _
                .Compounds _
                .Select(Function(c) c.Entity) _
                .DownloadRelatedReactions(EXPORT:=save, cache:=$"{save}/.reactions/") _
                .SaveTo($"{save}/failures.txt") _
                .CLICode
            If args.IsTrue("/try_all") Then
                Call DownloadAllReactions(EXPORT:=save, cache:=$"{save}/.reactions/")
            End If
        ElseIf args.IsTrue("/try_all") Then
            ' 假若不添加compound参数,则系统会自动使用br08201文件做下载依据
            ' 因为br08201的文件夹目录与all或者compound做下载依据的结构不一样,导致重复下载
            ' 所以在这里添加一个重复的判断来避开调用br08201做下载依据
            Call DownloadAllReactions(EXPORT:=save, cache:=$"{save}/.reactions/")
        Else
            Return EnzymaticReaction _
                .DownloadReactions(save, cache:=$"{save}/.br08201/") _
                .SaveTo(save & "/failures.txt") _
                .CLICode
        End If

        Return 0
    End Function

    ''' <summary>
    ''' gif图片是以base64编码放在XML文件里面的
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Download.Compounds")>
    <Description("Downloads the KEGG compounds data from KEGG web server using dbget API. Apply this downloaded KEGG compounds data used for metabolism annotation in LC-MS data analysis.")>
    <Usage("/Download.Compounds [/list <idlist.txt> /chebi <accessions.tsv> /reactions <kegg.reactions.repository> /flat /skip.compoundbrite /updates /save <DIR>]")>
    <ArgumentAttribute("/chebi", True, CLITypes.File,
              AcceptTypes:={GetType(Accession)},
              Description:="Some compound metabolite in the KEGG database have no brite catalog info, then using the brite database for the compounds downloads will missing some compounds, 
              then you can using this option for downloads the complete compounds data in the KEGG database.")>
    <Group(CLIGroups.DBGET_tools)>
    Public Function DownloadCompounds(args As CommandLine) As Integer
        Dim save$ = args("/save") Or "./KEGG_cpd/"
        Dim flat As Boolean = args("/flat")
        Dim updates As Boolean = args("/updates")

        If Not args("/skip.compoundbrite") Then
            Call CompoundBrite.DownloadFromResource(
                EXPORT:=save,
                directoryOrganized:=Not flat,
                structInfo:=True
            )
        End If

        ' 下载补充数据
        Dim accs As String = args <= "/chebi"

        If accs.FileExists(True) Then
            Call MetaboliteWebApi.CompleteUsingChEBI(save, accs, updates)
        End If

        Dim repo$ = args <= "/reactions"
        Dim list As String = args <= "/list"

        If repo.DirectoryExists Then
            Dim reactions As Reaction() = ReactionRepository.ScanModel(repo).metabolicNetwork
            Dim compoundsId As String() = reactions _
                .Select(Function(r)
                            Return r.GetSubstrateCompounds()
                        End Function) _
                .IteratesALL _
                .Distinct _
                .ToArray

            Call CompoundBrite.DownloadOthers(
                EXPORT:=save,
                compoundIds:=compoundsId,
                structInfo:=True
            )
        End If

        If list.FileExists Then
            Dim compoundsId As String() = list.ReadAllLines

            Call CompoundBrite.DownloadOthers(
                EXPORT:=save,
                compoundIds:=compoundsId,
                structInfo:=True
            )
        End If

        Return 0
    End Function

    <ExportAPI("-ref.map.download")>
    <Usage("-ref.map.download -o <out_dir>")>
    <Group(CLIGroups.DBGET_tools)>
    Public Function DownloadReferenceMapDatabase(argvs As CommandLine) As Integer
        Dim EXPORT As String = argvs("-o")
        Dim IDList = BriteHEntry.Pathway.LoadFromResource
        Dim Downloads = LinqAPI.Exec(Of Boolean) <=
 _
            From id As BriteHEntry.Pathway
            In IDList
            Let mapId As String = "map" & id.EntryId
            Let map = ReferenceMapData.Download(mapId)
            Let save As String = EXPORT & "/" & mapId & ".xml"
            Select map.GetXml.SaveTo(save)

        Return 0
    End Function

    ''' <summary>
    ''' 下载指定物种编号的物种基因组之中所有的pathway信息，包括代谢物和基因
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Download.Pathway.Maps")>
    <Usage("/Download.Pathway.Maps /sp <kegg.sp_code> [/KGML /out <EXPORT_DIR> /debug /@set <progress_bar=disabled>]")>
    <Description("Fetch all of the pathway map information for a specific kegg organism by using a specifc kegg sp code.")>
    <ArgumentAttribute("/sp", False, CLITypes.String,
              PipelineTypes.std_in,
              AcceptTypes:={GetType(String)},
              Description:="The 3 characters kegg organism code, example as: ""xcb"" Is stands for organism ""Xanthomonas campestris pv. campestris 8004 (Beijing)""")>
    <Group(CLIGroups.DBGET_tools)>
    <LastUpdated("2019-06-22 21:00:00.00")>
    Public Function DownloadPathwayMaps(args As CommandLine) As Integer
        Dim sp As String = args("/sp")
        Dim EXPORT As String = args("/out") Or (App.CurrentDirectory & "/" & sp)
        Dim isKGML As Boolean = args.IsTrue("/KGML")
        Dim infoJSON$ = $"{EXPORT}/kegg.json"

        If Not args("/debug") Then
            If infoJSON.LoadJSON(Of OrganismInfo)(throwEx:=False) Is Nothing Then
                Call Apps.KEGG_tools.ShowOrganism(code:=sp, out:=infoJSON)
            End If
        End If

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
    <ArgumentAttribute("/sp", False, CLITypes.File, PipelineTypes.std_in,
              AcceptTypes:={GetType(String), GetType(Organism)},
              Extensions:="*.txt, *.csv",
              Description:="A list of kegg species code. If this parameter Is a text file, 
              then each line should be start with the kegg organism code in three Or four letters, 
              else if this parameter is a csv table file, then it must in format of kegg organism data model.")>
    Public Function DownloadPathwayMapsBatchTask(args As CommandLine) As Integer
        Dim sp$ = args <= "/sp"
        Dim out$ = args("/out") Or $"{sp.TrimSuffix}/"
        Dim isKGML As Boolean = args.IsTrue("/KGML")
        Dim codes$()
        Dim directory$

        If sp.ExtensionSuffix.TextEquals("csv") Then
            codes = sp.LoadCsv(Of Organism) _
                .Select(Function(o) o.KEGGId) _
                .Distinct _
                .ToArray
        Else
            codes = sp.IterateAllLines _
                .Select(Function(l)
                            Return l.StringSplit("\s+").First
                        End Function) _
                .ToArray
        End If

        For Each id As String In codes
            directory = $"{out}/{id}/"

            Call Apps.KEGG_tools.DownloadPathwayMaps(
                sp:=id,
                out:=directory,
                kgml:=isKGML,
                _set:="progress_bar=disabled"
            )
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
            htext = OrganismHText.GetResource
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

    ''' <summary>
    ''' 这里下载的是标准的参考图数据
    ''' 
    ''' 包含有pathway的定义
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Pathways.Downloads.All")>
    <Description("Download all of the blank KEGG reference pathway map data. Apply for render KEGG pathway enrichment result or other biological system modelling work.")>
    <Usage("/Pathways.Downloads.All [/out <outDIR>]")>
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
    <ExportAPI("/download.kegg.maps")>
    <Description("Dumping the blank reference KEGG maps database.")>
    <Usage("/download.kegg.maps [/htext <htext.txt> /out <save_dir>]")>
    <ArgumentAttribute("/htext", False, CLITypes.File,
              Extensions:="*.txt",
              Description:="The KEGG category term provider")>
    <ArgumentAttribute("/out", True, CLITypes.File,
              Description:="A directory path that contains the download KEGG reference pathway map model data, this output can be using as the KEGG pathway map rendering repository source.")>
    <Group(CLIGroups.DBGET_tools)>
    Public Function HumanKEGGMaps(args As CommandLine) As Integer
        Dim htext$ = args <= "/htext"

        With (args <= "/out") Or $"./br08901_pathwayMaps/".AsDefault
            Return kegMap.Downloads(EXPORT:= .ByRef, briefFile:=htext) _
                .GetJson _
                .SaveTo(.ByRef & "/failures.json") _
                .CLICode
        End With
    End Function
End Module
