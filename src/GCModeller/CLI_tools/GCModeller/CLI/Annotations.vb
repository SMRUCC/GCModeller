#Region "Microsoft.VisualBasic::9570134d937f3b95286a2f2eb22662a3, CLI_tools\GCModeller\CLI\Annotations.vb"

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
'     Function: BuildFamilies, ExportBaSys
' 
' /********************************************************************************/

#End Region

Imports System.Threading
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv
Imports SMRUCC.genomics.Assembly.EBI
Imports SMRUCC.genomics.Data.BASys

Partial Module CLI

    <ExportAPI("--Interpro.Build", Usage:="--Interpro.Build /xml <interpro.xml>")>
    Public Function BuildFamilies(args As CommandLine) As Integer
        Dim DbPath As String = args("/xml")
        Dim DbXml = SMRUCC.genomics.Analysis.Annotations.Interpro.Xml.LoadDb(DbPath)
        Dim Families = SMRUCC.genomics.Analysis.Annotations.Interpro.Xml.BuildFamilies(DbXml)
        Dim out As String = DbXml.FilePath.TrimSuffix & ".Families.csv"
        Return Families.SaveTo(out)
    End Function

    <ExportAPI("/Export.Basys",
               Usage:="/Export.Basys /in <in.DIR> [/out <out.DIR>]")>
    Public Function ExportBaSys(args As CommandLine) As Integer
        Dim [in] As String = args.GetFullDIRPath("/in")
        Dim out As String = args.GetValue("/out", [in].TrimDIR & ".Basys.EXPORT/")
        Dim proj As Project = Project.Parser([in])
        Return proj.Write(EXPORT:=out)
    End Function

    <ExportAPI("/download.chebi")>
    <Usage("/download.chebi [/max 80000 /save <directory>]")>
    Public Function DownloadChebi(args As CommandLine) As Integer
        Dim save$ = args("/save") Or $"{App.CurrentDirectory}/chebi/"
        Dim max% = args.GetValue("/max", 80000)

        ' 这些失败的编号都是在数据库之中不存在的，没有数据的，会被忽略掉
        Dim failures As New Index(Of String)((save & "/failures.csv").ReadAllLines)
        ' 生成的[1,max]范围内的id序列之后进行随机获得乱序的序列模仿随机查询，然后使用每1000个块元素分块查询     
        Dim idlist = Enumerable.Range(1, max) _
            .Where(Function(id) failures.IndexOf(id.ToString) = -1) _
            .Shuffles _
            .Split(1000)

        For Each block As Integer() In idlist
            Dim failList$() = Nothing

            Call ChEBI.WebServices.BatchQuery(block.Select(Function(n) CStr(n)).ToArray, save, failList)
            Call failList.DoEach(Sub(id) Call failures.Add(id))
            ' 每个块休眠1分钟，降低目标服务器的负载
            Call Thread.Sleep(60 * 1000)
        Next

        Call failures.Objects.SaveTo(save & "/failures.csv")

        Return 0
    End Function
End Module
