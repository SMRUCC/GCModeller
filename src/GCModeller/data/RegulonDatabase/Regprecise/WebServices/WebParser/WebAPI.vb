#Region "Microsoft.VisualBasic::a5b01d0ef75e880f77de158ec8101072, data\RegulonDatabase\Regprecise\WebServices\WebParser\WebAPI.vb"

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


' Code Statistics:

'   Total Lines: 189
'    Code Lines: 154 (81.48%)
' Comment Lines: 8 (4.23%)
'    - Xml Docs: 75.00%
' 
'   Blank Lines: 27 (14.29%)
'     File Size: 8.26 KB


'     Module WebAPI
' 
'         Function: __downloads, CreateRegulator, doDownload, Download, DownloadRegulatorSequence
'                   GetRegulates, ToType
' 
' 
' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Threading
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Data.Regprecise.WebServices
Imports SMRUCC.genomics.SequenceModel
Imports r = System.Text.RegularExpressions.Regex

Namespace Regprecise

    <Package("Regprecise.WebAPI",
                      Category:=APICategories.UtilityTools,
                      Description:="Tools API for download and search RegPrecise database.",
                      Publisher:="")>
    Public Module WebAPI

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
        Public Const browse_genomes$ = "https://regprecise.lbl.gov/browse_genomes.jsp"
        Public Const taxonomic_collection$ = "https://regprecise.lbl.gov/collections_tax.jsp"

        <ExportAPI("Regprecise.Downloads")>
        Public Function Download(Optional EXPORT$ = "./") As TranscriptionFactors
            Dim html$
            Dim list$()
            Dim genomes As New List(Of BacteriaRegulome)
            Dim index$ = $"{EXPORT}/.cache/index.html"

            Call "Start to fetch regprecise genome information....".debug

            If index.FileLength > 1024 Then
                html = index.ReadAllText
            Else
                html = browse_genomes.GET
                html.SaveTo(index)
            End If

            html$ = r _
                .Match(html, TABLE_REGEX, RegexOptions.Singleline) _
                .Value
            list$ = r _
                .Matches(html, "<tr .+?</tr>", RegexICSng) _
                .ToArray

            Dim message As String
            Dim bar As Tqdm.ProgressBar = Nothing

            Call $"{list.Length} bacteria genome are ready to download!".debug

            For Each entry As String In Tqdm.Wrap(list, bar:=bar)
                genomes += doDownload(entry, EXPORT, skip:=skip)
                message = genomes.Last.genome.name
                bar.SetLabel(message)
            Next

            Return New TranscriptionFactors With {
                .genomes = genomes,
                .update = Now.ToShortDateString
            }
        End Function

        Private Function doDownload(entryHref$, EXPORT$, ByRef skip As Boolean) As BacteriaRegulome
            Dim str$ = r.Match(entryHref, "href="".+?"">.+?</a>").Value
            Dim entry$ = RegulomeQuery.GetsId(str)
            Dim name$ = entry.NormalizePathString
            Dim save$ = EXPORT & $"/{name}.xml"

            Static webQuery As New Dictionary(Of String, RegulomeQuery)

            Dim WebApi As RegulomeQuery = webQuery.ComputeIfAbsent(
                key:=$"{EXPORT}/.cache/",
                lazyValue:=Function(cache)
                               Return New RegulomeQuery(cache,,)
                           End Function)

            With New BacteriaRegulome With {
                .genome = New JSON.genome With {
                    .name = entry
                },
                .regulome = WebApi.Query(Of Regulome)(entryHref, hitCache:=skip)
            }
                Call .GetXml.SaveTo(save)

                Return .ByRef
            End With
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
                                     In Bacteria.regulome.regulators
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
                Dim exMsg As String = $"[null_LOCUS_ID] [Regulog={regulator.regulog.name}] [Bacteria={genome.genome.name}]" & vbCrLf
                Call ErrLog.WriteLine(exMsg, "", MSG_TYPES.INF)
                Return Nothing
            End If

            Dim FastaSaved As String = String.Format("{0}/{1}.fasta", DownloadDIR, regulator.locus_tag.name)
            Dim FastaObject = RegulatorDownloads(regulator, genome, ErrLog, DownloadDIR, FastaSaved)
            Return FastaObject
        End Function
    End Module
End Namespace
