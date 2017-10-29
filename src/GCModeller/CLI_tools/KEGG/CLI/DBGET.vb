#Region "Microsoft.VisualBasic::b15f28f6b47abcdd67c4bd6f00fbd110, ..\CLI_tools\KEGG\CLI\DBGET.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.EBI.ChEBI.Database.IO.StreamProviders.Tsv.Tables
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.ReferenceMap
Imports kegMap = SMRUCC.genomics.Assembly.KEGG.WebServices.Downloader

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

    <ExportAPI("/Download.Module.Maps",
               Info:="Download the KEGG reference modules map data.",
               Usage:="/Download.Module.Maps [/out <EXPORT_DIR, default=""./"">]")>
    Public Function DownloadReferenceModule(args As CommandLine) As Integer
        Dim out$ = args.GetValue("/out", "./")

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

    <ExportAPI("/dump.kegg.maps")>
    <Description("Dumping the KEGG maps database for human species.")>
    <Usage("/dump.kegg.maps /htext <htext.txt> [/out <save_dir>]")>
    <Argument("/htext", False, CLITypes.File,
              Extensions:="*.txt",
              Description:="The KEGG category term provider")>
    <Argument("/out", True, CLITypes.File,
              Description:="A directory path that contains the download KEGG reference pathway map model data, this output can be using as the KEGG pathway map rendering repository source.")>
    <Group(CLIGroups.DBGET_tools)>
    Public Function DumpKEGGMaps(args As CommandLine) As Integer
        Dim htext$ = args <= "/htext"

        With (args <= "/out") Or $"{htext.TrimSuffix}/KEGG.pathwayMaps/".AsDefault
            Return kegMap.Downloads(EXPORT:= .ref, briefFile:=htext) _
                .GetJson _
                .SaveTo(.ref & "/failures.json") _
                .CLICode
        End With
    End Function
End Module
