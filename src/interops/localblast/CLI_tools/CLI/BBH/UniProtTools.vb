﻿#Region "Microsoft.VisualBasic::853b7c58a27700a7e2433e5ec7e41e80, CLI_tools\CLI\BBH\UniProtTools.vb"

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
'     Function: ExportKOFromUniprot, getSuffix, proteinEXPORT, UniProtBBHMapTable
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.IO
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Pipeline
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports UniProtEntry = SMRUCC.genomics.Assembly.Uniprot.XML.entry

Partial Module CLI

    <ExportAPI("/UniProt.bbh.mappings")>
    <Usage("/UniProt.bbh.mappings /in <bbh.csv> [/reverse /out <mappings.txt>]")>
    <Group(CLIGrouping.UniProtTools)>
    Public Function UniProtBBHMapTable(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.mappings.txt"
        Dim mappings = [in].LoadCsv(Of BiDirectionalBesthit)
        Dim reversed As Boolean = args("/reverse")
        Dim table As Dictionary(Of String, String) = mappings _
            .Where(Function(query)
                       Return Not (query.HitName.StringEmpty OrElse query.HitName.TextEquals(HITS_NOT_FOUND))
                   End Function) _
            .ToDictionary(Function(query)
                              Return query.QueryName
                          End Function,
                          Function(query)
                              Return query.HitName.Split("|"c)(1)
                          End Function)

        Return table.Tsv(out,, reversed:=reversed).CLICode
    End Function

    ''' <summary>
    ''' 从uniprot数据库之中导出具有KO编号的所有蛋白序列
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/UniProt.KO.faa")>
    <Usage("/UniProt.KO.faa /in <uniprot.xml> [/lineBreak <default=120> /out <proteins.faa>]")>
    <Description("Export all of the protein sequence from the Uniprot database which have KO number been assigned.")>
    <Argument("/in", False, CLITypes.File, PipelineTypes.std_in,
              Extensions:="*.Xml",
              Description:="The Uniprot database which is downloaded from the Uniprot website or ftp site.")>
    <Argument("/out", True, CLITypes.File, PipelineTypes.std_out,
              AcceptTypes:={GetType(FastaFile)},
              Extensions:="*.faa, *.fasta, *.fa",
              Description:="The file path of the export protein sequence, title of each sequence consist with these fields: ``KO|uniprot_id fullName|scientificName``")>
    <Group(CLIGrouping.UniProtTools)>
    Public Function ExportKOFromUniprot(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.KO.faa"
        Dim i As VBInteger = 0
        Dim lineBreak As Integer = args("/lineBreak") Or 120

        Using writer As StreamWriter = out.OpenWriter(Encodings.ASCII)
            Dim source As IEnumerable(Of UniProtEntry) = UniProtXML.EnumerateEntries(path:=[in])

            For Each prot As UniProtEntry In source.Where(Function(g) Not g.sequence Is Nothing)
                Dim KO As dbReference = prot.KO

                If KO Is Nothing Then
                    Continue For
                End If

                Dim seq As String = prot.ProteinSequence
                Dim fa As New FastaSeq With {
                    .SequenceData = seq,
                    .Headers = {KO.id, prot.accessions.First & " " & prot.proteinFullName, prot.organism.scientificName}
                }

                Call writer.WriteLine(fa.GenerateDocument(lineBreak))

                If ++i Mod 100 = 0 Then
                    Console.Write(i)
                    Console.Write(vbTab)
                End If
            Next
        End Using

        Return 0
    End Function

    Private Function getSuffix(sp As String, exclude As Boolean) As String
        Dim suffix$

        If sp.StringEmpty Then
            Return ""
        Else
            suffix = sp.NormalizePathString.Replace(" ", "_")
        End If

        If exclude Then
            suffix = $"-exclude-{suffix}"
        End If

        Return suffix
    End Function

    ''' <summary>
    ''' 从UniProt数据库之中导出给定物种的蛋白序列
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/protein.EXPORT")>
    <Usage("/protein.EXPORT /in <uniprot.xml> [/sp <name> /exclude /out <out.fasta>]")>
    <Description("Export the protein sequence and save as fasta format from the uniprot database dump XML.")>
    <Argument("/sp", True, CLITypes.String,
          AcceptTypes:={GetType(String)},
          Description:="The organism scientific name.")>
    <Argument("/uniprot", False, CLITypes.File, PipelineTypes.std_in,
          AcceptTypes:={GetType(UniProtXML)},
          Extensions:="*.xml",
          Description:="The Uniprot protein database in XML file format.")>
    <Argument("/exclude", True, CLITypes.Boolean,
          Description:="Exclude the specific organism by ``/sp`` scientific name instead of only include it?")>
    <Argument("/out", True, CLITypes.File,
          Extensions:="*.fa, *.fasta, *.txt",
          Description:="The saved file path for output protein sequence fasta file. The title format of this command output is ``uniprot_id fullName``")>
    <Group(CLIGrouping.UniProtTools)>
    Public Function proteinEXPORT(args As CommandLine) As Integer
        Dim [in] As String = args <= "/in"
        Dim sp As String = args <= "/sp"
        Dim exclude As Boolean = args("/exclude")
        Dim suffix$ = getSuffix(sp, exclude)
        Dim out As String = args("/out") Or ([in].TrimSuffix & $"{suffix}.fasta")

        ' 20190707 这个函数的功能与ExportKOFromUniprot命令行函数基本一致
        ' 只不过ExportKOFromUniprot函数所导出来的序列的标题会存在KO编号以及
        ' 物种信息，并且该命令只导出具有KO编号的序列
        ' 所以ExportKOFromUniprot导出来的序列会比较少
        '
        ' 而这个函数则不一样，其会导出所有的蛋白序列

        ' 1GB buffer size?
        Call App.SetBufferSize(128 * 1024 * 1024)

        Using writer As StreamWriter = out.OpenWriter(Encodings.ASCII)
            Dim source As IEnumerable(Of UniProtEntry) = UniProtXML.EnumerateEntries(path:=[in])

            If Not String.IsNullOrEmpty(sp) Then
                If exclude Then
                    source = source _
                        .Where(Function(gene)
                                   Return Not gene.organism.scientificName = sp
                               End Function)
                Else
                    source = source _
                        .Where(Function(gene)
                                   Return gene.organism.scientificName = sp
                               End Function)
                End If
            End If

            For Each prot As UniProtEntry In source.Where(Function(g) Not g.sequence Is Nothing)
                Dim seq$ = prot _
                    .sequence _
                    .sequence _
                    .LineTokens _
                    .JoinBy("") _
                    .Replace(" ", "")
                Dim fa As New FastaSeq With {
                    .SequenceData = seq,
                    .Headers = {prot.accessions.First & " " & prot.proteinFullName}
                }

                Call writer.WriteLine(fa.GenerateDocument(120))
            Next
        End Using

        Return 0
    End Function

    ''' <summary>
    ''' 阈值筛选应该是发生在sbh导出的时候，在这里将不会做任何阈值筛选操作
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/UniProt.KO.assign")>
    <Usage("/UniProt.KO.assign /in <query_vs_uniprot.KO.besthit> [/bbh <uniprot_vs_query.KO.besthit> /out <out.KO.csv>]")>
    <Description("Assign KO number to query from Uniprot reference sequence database alignment result.")>
    <Argument("/in", False, CLITypes.File, PipelineTypes.std_in,
              AcceptTypes:={GetType(BestHit)},
              Extensions:="*.csv",
              Description:="The sbh result of the alignment: query vs uniprot.KO.")>
    <Argument("/bbh", True, CLITypes.File,
              AcceptTypes:={GetType(BestHit)},
              Extensions:="*.csv",
              Description:="If this argument is presents in the cli input, then it means we use the bbh method for assign the KO number to query. 
              Both ``/in`` and ``/bbh`` is not top best selection output. The input file for this argument should be the result of ``/SBH.Export.Large``
              command, and ``/keeps_raw.queryName`` option should be enabled for keeps the taxonomy information.")>
    <Argument("/out", True, CLITypes.File, PipelineTypes.std_out,
              AcceptTypes:={},
              Extensions:="*.csv",
              Description:="Use the eggHTS command ``/proteins.KEGG.plot`` for export the final KO number assignment result table.")>
    Public Function UniProtKOAssign(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim bbh$ = args <= "/bbh"
        Dim out$

        If bbh.FileExists Then
            out = args("/out") Or $"{[in].TrimSuffix}.KO_bbh.csv"
        Else
            out = args("/out") Or $"{[in].TrimSuffix}.KO.csv"
        End If

        Dim skipHitNotFound As New NamedValue(Of Func(Of String, Boolean)) With {
            .Name = "hit_name",
            .Value = Function(colVal)
                         ' 将所有的HITS_NOT_FOUND的行都跳过
                         ' 这样子可以节省比较多的内存
                         Return colVal = "HITS_NOT_FOUND"
                     End Function
        }
        Dim queryVsUniprot As BestHit() = [in].LoadCsv(Of BestHit)(skipWhile:=skipHitNotFound).ToArray
        Dim uniprotVsquery As BestHit() = bbh _
            .LoadCsv(Of BestHit)(skipWhile:=skipHitNotFound) _
            .ToArray

        ' 在这里主要是完成对标题的解析操作
        ' 然后导出其他的命令能够识别得了的数据格式

        If uniprotVsquery.IsNullOrEmpty Then
            Return KOAssignment.KOAssignmentSBH(queryVsUniprot) _
                .SaveTo(out) _
                .CLICode
        Else
            Return KOAssignment.KOassignmentBBH(queryVsUniprot, uniprotVsquery) _
                .ToArray _
                .SaveTo(out) _
                .CLICode
        End If
    End Function
End Module
