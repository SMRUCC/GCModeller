﻿#Region "Microsoft.VisualBasic::4e2dd0d9262ad393b7c41dfbffb8139f, data\RegulonDatabase\Regprecise\WebServices\WebParser\WebAPI.vb"

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

    '     Module WebAPI
    ' 
    '         Function: __download, __downloads, CreateRegulator, Download, DownloadRegulatorSequence
    '                   DownloadRegulon, GetRegulates, GetsId, ToType
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Threading
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Terminal.ProgressBar
Imports Microsoft.VisualBasic.Text.HtmlParser
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Data.Regprecise.Regulator
Imports SMRUCC.genomics.Data.Regprecise.WebServices
Imports SMRUCC.genomics.SequenceModel
Imports r = System.Text.RegularExpressions.Regex

Namespace Regprecise

    <Package("Regprecise.WebAPI",
                      Category:=APICategories.UtilityTools,
                      Description:="Tools API for download and search RegPrecise database.",
                      Publisher:="")>
    Public Module WebAPI

        ''' <summary>
        ''' 下载某一个基因组内所有预测的调控元的数据
        ''' </summary>
        ''' <param name="url"></param>
        ''' <returns></returns>
        <ExportAPI("Regulon.Downloads")>
        Public Function DownloadRegulon(url As String) As Regulon
            Dim html$ = r _
                .Match(url.GET, "<table class=""stattbl"".+?</table>", RegexOptions.Singleline) _
                .Match("<tbody>.+</tbody>", RegexOptions.Singleline)
            Dim list$() = r _
                .Matches(html, "<tr .+?</tr>", RegexOptions.Singleline + RegexOptions.IgnoreCase) _
                .ToArray
            Dim regulators As New List(Of Regulator)
            Dim str$

            For i As Integer = 0 To list.Length - 1
                str = list(i)
                regulators += Regulator.CreateObject(str)
                Thread.Sleep(2000)
            Next

            Return New Regulon With {
                .regulators = regulators
            }
        End Function

        <ExportAPI("Get.RegulatedGenes")>
        Public Function GetRegulates(url As String) As RegulatedGene()
            Dim page As String = url.GET
            Return RegulatedGene.DocParser(doc:=page)
        End Function

        <ExportAPI("Regulator.Creates")>
        Public Function CreateRegulator(Family As String,
                                        Bacteria As String,
                                        RegulatorSites As FASTA.FastaFile,
                                        RegulatorId As String) As Regulator
            Dim regSites = (From FastaObject As FastaReaders.Site
                            In FastaReaders.Site.CreateObject(RegulatorSites)
                            Select FastaObject.ToFastaObject).ToArray
            Dim Regulator As Regulator = New Regulator With {
                .family = Family,
                .locus_tag = New NamedValue With {.name = RegulatorId},
                .regulog = New NamedValue With {
                    .name = String.Format("{0} - {1}", Family, Bacteria)
                },
                .type = Types.TF,
                .regulator = New NamedValue With {.name = RegulatorId},
                .regulatorySites = regSites
            }
            Return Regulator
        End Function

        <ExportAPI("Get.sId")>
        Public Function GetsId(strData As String) As String
            Dim Id As String = Mid(strData, InStr(strData, """>") + 2)
            If String.IsNullOrEmpty(Trim(Id)) Then
                Return ""
            Else
                Id = Mid(Id, 1, Len(Id) - 4)
                Return Id
            End If
        End Function

        <ExportAPI("s.ToType")>
        Public Function ToType(type As String) As Types
            If String.Equals(type, "tf", StringComparison.OrdinalIgnoreCase) Then
                Return Types.TF
            ElseIf String.Equals(type, "rna", StringComparison.OrdinalIgnoreCase) Then
                Return Types.RNA
            Else
                Return Types.NotSpecific
            End If
        End Function

        Public Const TABLE_REGEX As String = "<table class=""stattbl"">.+</table>"
        Public Const browse_genomes$ = "http://regprecise.lbl.gov/RegPrecise/browse_genomes.jsp"

        <ExportAPI("Regprecise.Downloads")>
        Public Function Download(Optional EXPORT$ = "./") As TranscriptionFactors
            Dim html$
            Dim list$()
            Dim genomes As New List(Of BacteriaRegulome)

            Call "Start to fetch regprecise genome information....".__DEBUG_ECHO

            html$ = r _
                .Match(browse_genomes.GET, TABLE_REGEX, RegexOptions.Singleline) _
                .Value
            list$ = r _
                .Matches(html, "<tr .+?</tr>", RegexOptions.Singleline + RegexOptions.IgnoreCase) _
                .ToArray

            Call $"{list.Length} bacteria genome are ready to download!".__DEBUG_ECHO

            Using progress As New ProgressBar("Download regprecise database...")
                Dim tick As New ProgressProvider(total:=list.Length)
                Dim ETA$
                Dim message$
                Dim skip As Boolean = False

                For i As Integer = 0 To list.Length - 1
                    genomes += __download(list(i), EXPORT, skip:=skip)
                    ETA = tick.ETA(progress.ElapsedMilliseconds).FormatTime
                    message = $"{genomes(i).genome.name}  ETA: {ETA}"
                    progress.SetProgress(tick.StepProgress, message)

                    If Not skip Then
                        Call Thread.Sleep(60 * 1000)
                    End If
                Next
            End Using

            Return New TranscriptionFactors With {
                .genomes = genomes,
                .update = Now.ToShortDateString
            }
        End Function

        Private Function __download(entryHref$, EXPORT$, ByRef skip As Boolean) As BacteriaRegulome
            Dim str$ = r.Match(entryHref, "href="".+?"">.+?</a>").Value
            Dim entry As KeyValuePair = KeyValuePair.CreateObject(GetsId(str), "http://regprecise.lbl.gov/RegPrecise/" & str.href)
            Dim name$ = entry.Key.NormalizePathString
            Dim save$ = EXPORT & $"/{name}.xml"

            skip = False

            If save.FileLength > 1024 Then
                skip = True
                Return save.LoadXml(Of BacteriaRegulome)()
            Else
                With New BacteriaRegulome With {
                    .genome = New JSON.genome With {
                        .name = entry.Key
                    },
                    .regulons = WebAPI.DownloadRegulon(entry.Value)
                }
                    Call .GetXml.SaveTo(save)

                    Return .ByRef
                End With
            End If
        End Function

        ''' <summary>
        ''' 从KEGG数据库中下载调控因子的蛋白质序列
        ''' </summary>
        ''' <param name="Regprecise"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Regulators.Downloads",
                   Info:="Downloads the protein sequence of the regulators which is archives in Regprecise database.")>
        Public Function DownloadRegulatorSequence(Regprecise As TranscriptionFactors, DownloadDIR As String) As FASTA.FastaFile
            Dim FileData As FASTA.FastaFile = New FASTA.FastaFile
            Using ErrLog As New LogFile($"{DownloadDIR}/DownloadError_{Now.ToString.NormalizePathString}.log")
                For Each Bacteria As BacteriaRegulome In Regprecise.genomes
                    Dim downloads = (From regulator As Regulator
                                     In Bacteria.regulons.regulators
                                     Let fa As FASTA.FastaSeq = __downloads(regulator, Bacteria, ErrLog, DownloadDIR)
                                     Where Not fa Is Nothing
                                     Select fa).ToArray
                    Call FileData.AddRange(downloads)
                Next
            End Using

            Return FileData
        End Function

        Private Function __downloads(regulator As Regulator,
                                     genome As BacteriaRegulome,
                                     ErrLog As LogFile,
                                     DownloadDIR As String) As FASTA.FastaSeq

            If regulator.type = Types.RNA Then
                Return Nothing
            End If

            If String.IsNullOrEmpty(regulator.locus_tag.name) Then
                Dim exMsg As String = $"[null_LOCUS_ID] [Regulog={regulator.Regulog.name}] [Bacteria={genome.genome.name}]" & vbCrLf
                Call ErrLog.WriteLine(exMsg, "", MSG_TYPES.INF)
                Return Nothing
            End If

            Dim FastaSaved As String = String.Format("{0}/{1}.fasta", DownloadDIR, regulator.locus_tag.name)
            Dim FastaObject = RegulatorDownloads(regulator, genome, ErrLog, DownloadDIR, FastaSaved)
            Return FastaObject
        End Function
    End Module
End Namespace
