#Region "Microsoft.VisualBasic::04228cc10003c30f9f6e687404f53e54, CLI_tools\MEME\Cli\MotifSimilarity\MEME_TOM.vb"

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
    '     Function: MEMEPlantSimilarity, MEMEPlantSimilarityBatch, MEMETOM_MotifSimilarity
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Programs

Partial Module CLI

    ''' <summary>
    ''' 导出tomtom程序的分析结果
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Motif.Similarity",
               Info:="Export of the calculation result from the tomtom program.",
               Usage:="/Motif.Similarity /in <tomtom.DIR> /motifs <MEME_OUT.DIR> [/out <out.csv> /bp.var]")>
    Public Function MEMETOM_MotifSimilarity(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim motifs As String = args("/motifs")
        Dim out As String = args.GetValue("/out", inDIR & ".MotifMatches.Csv")

        If Not args.GetBoolean("/bp.var") Then
            Dim hashTag As String = FileIO.FileSystem.GetDirectoryInfo(inDIR).Name
            Dim matches = BuildSimilarity(inDIR, motifs, hashTag)
            Return matches.SaveTo(out).CLICode
        Else
            Dim inDIRs = FileIO.FileSystem.GetDirectories(inDIR, FileIO.SearchOption.SearchTopLevelOnly)
            Dim motifsDIR = (From DIR As String
                             In FileIO.FileSystem.GetDirectories(motifs, FileIO.SearchOption.SearchTopLevelOnly)
                             Let name As String = FileIO.FileSystem.GetDirectoryInfo(DIR).Name
                             Select name,
                                 DIR) _
                                 .ToDictionary(Function(x) x.name,
                                               Function(x) x.DIR)
            Dim list As New List(Of MotifMatch)

            For Each source As String In inDIRs
                Dim name As String = FileIO.FileSystem.GetDirectoryInfo(source).Name
                If motifsDIR.ContainsKey(name) Then
                    list += BuildSimilarity(source, motifsDIR(name), hashTag:=name)
                End If
            Next

            Return list.SaveTo(out)
        End If
    End Function

    ''' <summary>
    ''' 计算出Motif的相似度，然后方便分组归纳新的Motif数据，这里只是计算出一个模块的
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/TomTOM.Similarity",
               Usage:="/TomTOM.Similarity /in <TOM_OUT.DIR> [/out <out.Csv>]")>
    Public Function MEMEPlantSimilarity(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & $".{NameOf(MEMEPlantSimilarity)}.Csv")
        Dim init As MotifMatch() = LoadsSimilarity([in], False, blocks:=True)
        Return init.SaveTo(out)
    End Function

    <ExportAPI("/TOMTOM.Similarity.Batch",
               Usage:="/TOMTOM.Similarity.Batch /in <inDIR> [/out <out.csv>]")>
    Public Function MEMEPlantSimilarityBatch(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim out As String = args.GetValue("/out", inDIR & ".TOMTOM.Similarity.Csv")
        Dim DIRs As IEnumerable(Of String) = ls - l - lsDIR <= inDIR
        Dim result As New List(Of MotifMatch)

        For Each DIR As String In DIRs
            result += LoadsSimilarity(DIR, False, blocks:=True)
        Next

        Return result.SaveTo(out).CLICode
    End Function
End Module
