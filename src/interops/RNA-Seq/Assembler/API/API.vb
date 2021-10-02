#Region "Microsoft.VisualBasic::8fb995c6d3b2c673caeb568b7c7e3dbe, RNA-Seq\Assembler\API\API.vb"

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

    ' Module API
    ' 
    '     Function: AssemblerAssemble, AssemblerAssembleAPI, AssemblerBatch, LoadReads, LoadTranscripts
    '               SaveTranscripts
    ' 
    ' /********************************************************************************/

#End Region

Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat
Imports LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application
Imports LANS.SystemsBiology.Toolkits.RNA_Seq.Assembler.DocumentFormat
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Scripting.MetaData

<[PackageNamespace]("Assembler", Description:="", Publisher:="amethyst.asuka@gcmodeller.org")>
Public Module API

    <ExportAPI("Load.Reads.BlastnMapping", Info:="Read the blastn mapping alignment dataset from a specific csv format data file.")>
    Public Function LoadReads(<Parameter("Csv.Path", "The csv data file which is export from the blastn alignment of the fq short reads.")> path As String,
                              <Parameter("Trim",
 "If this option is true then all of the duplicated alignment and unperfect alignment reads will be removed from the input dataset.")>
                              Optional Trim As Boolean = True) As BlastnMapping()

        Dim readsBuffer = path.LoadCsv(Of BlastnMapping)(False).ToArray
        Call $"{NameOf(readsBuffer)}=>{readsBuffer.LongLength} reads....".__DEBUG_ECHO

        If Trim Then
            Call $"Data set will be trimmed for removed of the unique and unperfect reads alignment....".__DEBUG_ECHO
            Return MapsAPI.TrimAssembly(readsBuffer)
        Else
            Return readsBuffer
        End If
    End Function

    <ExportAPI("Assembler.pInvoke")>
    Public Function AssemblerAssemble(Reads As IEnumerable(Of BlastnMapping),
                                      Optional PTT As PTT = Nothing,
                                      Optional DataOverview As String = "",
                                      Optional Delta As Integer = 35,
                                      Optional sharedThreshold As Integer = 30,
                                      <Parameter("ATG.Dist")> Optional ATGDist As Integer = 500) As Transcript()
        Dim DataOverViewBuffer As DocumentStream.File = Nothing
        Dim Result As Transcript() = API.AssemblerAssembleAPI(Reads, PTT, Delta, DataOverViewBuffer, sharedThreshold, ATGDist)

        Try
            If String.IsNullOrEmpty(DataOverview) Then
                DataOverview = $"{FileIO.FileSystem.CurrentDirectory}/{NameOf(DataOverview)}_{Now.ToString.NormalizePathString}.csv"
            End If

            Call DataOverViewBuffer.Save(DataOverview, False)
        Catch ex As Exception
            Call ex.ToString.__DEBUG_ECHO
        End Try

        Return Result
    End Function

    <ExportAPI("Write.Csv.Transcripts")>
    Public Function SaveTranscripts(data As IEnumerable(Of Transcript), SaveTo As String) As Boolean
        Return data.SaveTo(SaveTo, False)
    End Function

    <ExportAPI("Read.Csv.Transcripts")>
    Public Function LoadTranscripts(path As String) As DocumentFormat.Transcript()
        Return path.LoadCsv(Of Transcript)(False).ToArray
    End Function

    Const AssemblerScript As String = "Imports Assembler" & vbCrLf &
                                      "Imports GCModeller.Assembly.File.IO" & vbCrLf &
                                      "path <- %path" & vbCrLf &
                                      " PTT < (PTT) %ptt" & vbCrLf &
                                      "data <- Load.Reads.BlastnMapping path $path Trim %Trim" & vbCrLf &
                                      "data <- assembler Assembler.pInvoke Reads $data PTT $PTT DataOverview $path.assembler_DataOverview.csv" & vbCrLf &
                                      "Call $data -> Write.Csv.Transcripts SaveTo $path.assembler.csv"

    <ExportAPI("BlastnMapping.Assembler.Batch")>
    Public Function AssemblerBatch(Dir As String, PTT As String, Optional Trim As Boolean = True) As Boolean
        Dim Reads = Dir.LoadSourceEntryList({"*.csv"})
        Dim LQuery = (From readsFile In Reads.AsParallel
                      Let Script As String = AssemblerScript.Replace("%path", readsFile.Value.CliPath).Replace("%ptt", PTT.CliPath).Replace("%Trim", CStr(Trim))
                      Select Settings.Session.FolkShoalThread(Script, readsFile.Value)).ToArray
        Return True
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Reads"></param>
    ''' <param name="PTT"></param>
    ''' <param name="ContigOverlapsDelta">建议值为40bp左右</param>
    ''' <param name="DataOverview"></param>
    ''' <param name="sharedThreshold">默认值为文献之中的30条</param>
    ''' <returns></returns>
    Public Function AssemblerAssembleAPI(Reads As Generic.IEnumerable(Of LANS.SystemsBiology.SequenceModel.NucleotideModels.Contig),
                                         PTT As LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.PTT,
                                         ContigOverlapsDelta As Integer,
                                         ByRef DataOverview As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File,
                                         sharedThreshold As Integer, ATGDist As Integer) As Transcript()

        Dim AssemblerInvoked As New Assembler(ContigOverlapsDelta)
        Dim sw As Stopwatch = Stopwatch.StartNew
        Dim Transcripts As Transcript() = (From contig As LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation
                                           In AssemblerInvoked.Assembling(Reads).AsParallel
                                           Select Transcript = Transcript.CreateObject(Of Transcript)(contig)).ToArray

        Call $"[Assembler Job Done!] {Reads.Count} assembled {Transcripts.Count} contigs in {sw.ElapsedMilliseconds}ms...".__DEBUG_ECHO

        '   Call TSSsIdentification.IdentifyTSSs(AssemblerInvoked, Transcripts, sharedThreshold)

        'If Not PTT Is Nothing Then
        '    Call "Start to associates of genome context...".__DEBUG_ECHO

        '    Dim LQuery = (From Transcript In Transcripts.AsParallel Select GenomeContext(Transcript, PTT, ATGDist)).ToArray
        '    Transcripts = LQuery.MatrixToVector

        '    Call $"Association job done!".__DEBUG_ECHO
        'End If

        DataOverview = AssemblerInvoked.DataOverviews
        Call DataOverview.Add({NameOf(sharedThreshold), sharedThreshold, "bp"})
        Call DataOverview.Add({NameOf(ContigOverlapsDelta), ContigOverlapsDelta, "bp"})

        Return Transcripts
    End Function
End Module
