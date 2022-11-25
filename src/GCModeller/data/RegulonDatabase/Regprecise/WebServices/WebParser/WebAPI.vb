#Region "Microsoft.VisualBasic::369bf993402b42f04cd93681c9b35223, GCModeller\data\RegulonDatabase\Regprecise\WebServices\WebParser\WebAPI.vb"

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

    '   Total Lines: 186
    '    Code Lines: 151
    ' Comment Lines: 8
    '   Blank Lines: 27
    '     File Size: 8.06 KB


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
        Public Const browse_genomes$ = "http://regprecise.lbl.gov/RegPrecise/browse_genomes.jsp"

        <ExportAPI("Regprecise.Downloads")>
        Public Function Download(Optional EXPORT$ = "./") As TranscriptionFactors
            Dim html$
            Dim list$()
            Dim genomes As New List(Of BacteriaRegulome)
            Dim index$ = $"{EXPORT}/index.html"

            Call "Start to fetch regprecise genome information....".__DEBUG_ECHO

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

            Call $"{list.Length} bacteria genome are ready to download!".__DEBUG_ECHO

            Using progress As New ProgressBar("Download regprecise database...")
                Dim tick As New ProgressProvider(progress, total:=list.Length)
                Dim ETA$
                Dim message$
                Dim skip As Boolean = False

                For i As Integer = 0 To list.Length - 1
                    genomes += doDownload(list(i), EXPORT, skip:=skip)
                    ETA = tick.ETA().FormatTime
                    message = $"{genomes(i).genome.name}  ETA: {ETA}"
                    progress.SetProgress(tick.StepProgress, message)

                    If Not skip Then
                        Call Thread.Sleep(60 * 1000)
                    End If

                    ' Call Console.Clear()
                Next
            End Using

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
