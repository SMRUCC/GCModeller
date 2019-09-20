#Region "Microsoft.VisualBasic::eb66f2afc3212c639522f0e7aed5ba09, CLI_tools\ProteinInteraction\CLI\SwissTCS.vb"

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
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: __contact, __contactTrace, Contacts, CreateProfiles, CrossTalksCal
    '               DownloadEntireDb, MergePfam, ProtFastaDownloads, ProtFastaDownloadsBatch
    ' 
    '     Sub: __downloads, __getProbability
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.SwissTCS
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.Xfam
Imports SMRUCC.genomics.Interops.NCBI.Extensions

Partial Module CLI

    <ExportAPI("--SwissTCS.Downloads", Usage:="--SwissTCS.Downloads /out <out.DIR>")>
    Public Function DownloadEntireDb(args As CommandLine) As Integer
        Dim out As String = args("/out")
        Call SwissRegulon.Download(out)
        Return 0
    End Function

    <ExportAPI("--ProtFasta.Downloads", Usage:="--ProtFasta.Downloads /in <sp.DIR>")>
    Public Function ProtFastaDownloads(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Call __downloads(inDIR)
        Return 0
    End Function

    Private Sub __downloads(inDIR As String)
        Dim Hisk As String() = FileIO.FileSystem.GetFiles(inDIR & "/HisK/",
                                                        FileIO.SearchOption.SearchTopLevelOnly,
                                                        "*.csv").Select(Function(x) BaseName(x))
        Dim RR As String() = FileIO.FileSystem.GetFiles(inDIR & "/RR/",
                                                        FileIO.SearchOption.SearchTopLevelOnly,
                                                        "*.csv").Select(Function(x) BaseName(x))
        Dim fa = SMRUCC.genomics.Assembly.KEGG.WebServices.DownloadsBatch(inDIR, Hisk)
        If Not fa Is Nothing Then Call fa.Save(inDIR & "/HisK.fasta")
        fa = SMRUCC.genomics.Assembly.KEGG.WebServices.DownloadsBatch(inDIR, RR)
        If Not fa Is Nothing Then Call fa.Save(inDIR & "/RR.fasta")
    End Sub

    <ExportAPI("--ProtFasta.Downloads.Batch", Usage:="--ProtFasta.Downloads.Batch /in <sp.DIR.Source>")>
    Public Function ProtFastaDownloadsBatch(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim lstDIR As String() = FileIO.FileSystem.GetDirectories(inDIR, FileIO.SearchOption.SearchTopLevelOnly).ToArray

        For Each DIR As String In lstDIR
            Call __downloads(DIR)
        Next

        Return 0
    End Function

    ''' <summary>
    ''' 将可用的序列数据合并在一起然后做Pfam分析导出Pfam-String数据
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("--Merge.Pfam", Usage:="--Merge.Pfam /in <in.DIR>")>
    Public Function MergePfam(args As CommandLine) As Integer
        Dim Input As String = args("/in")
        Dim Temp As String = Input & "/Pfam/"
        Dim source As String() = FileIO.FileSystem.GetDirectories(Input, FileIO.SearchOption.SearchTopLevelOnly).ToArray
        source = (From DIR As String
                  In source
                  Where Not String.Equals("Pfam",
                      FileIO.FileSystem.GetDirectoryInfo(DIR).Name,
                      StringComparison.OrdinalIgnoreCase)
                  Select DIR).ToArray
        Dim LoadProt = (From DIR As String
                        In source.AsParallel
                        Let Hisk = SMRUCC.genomics.SequenceModel.FASTA.FastaFile.Read(DIR & "/HisK.fasta", False)
                        Let RR = SMRUCC.genomics.SequenceModel.FASTA.FastaFile.Read(DIR & "/RR.fasta", False)
                        Where Not (Hisk.IsNullOrEmpty OrElse RR.IsNullOrEmpty)  ' 必须要两个都同时存在的才会可以继续下去，只要有任何一个没有数据则抛弃掉
                        Select Hisk.Join(RR)).ToArray.Unlist
        Dim Merge As String = Temp & "/swissTCS.fasta"
        Dim MergeFasta As New SMRUCC.genomics.SequenceModel.FASTA.FastaFile(LoadProt)
        Call MergeFasta.Save(Merge)

        Dim blast As New LocalBLAST.Programs.BLASTPlus(GCModeller.FileSystem.GetLocalblast)
        Dim Pfam As String = GCModeller.FileSystem.CDD & "/Pfam.fasta"
        Dim out As String = Temp & "/Pfam.txt"

        Call blast.FormatDb(Pfam, blast.MolTypeProtein).Start(waitForExit:=True)
        Call blast.Blastp(Merge, Pfam, out).Start(waitForExit:=True)

        Dim Logs = LocalBLAST.BLASTOutput.BlastPlus.Parser.TryParse(out)
        Dim PfamString = Xfam.Pfam.CreatePfamString(Logs, disableUltralarge:=True)
        out = Temp & "swissTCS.Pfam.csv"
        Return PfamString.SaveTo(out)
    End Function

    Sub New()
        Call Settings.Session.Initialize()
    End Sub

    <ExportAPI("--Contacts", Usage:="--Contacts /in <in.DIR>")>
    Public Function Contacts(args As CommandLine) As Integer
        Dim input As String = args("/in")
        Dim Pfam As String = input & "/Pfam/"
        Dim source As String() = FileIO.FileSystem.GetDirectories(input, FileIO.SearchOption.SearchTopLevelOnly).ToArray
        source = (From DIR As String
                  In source
                  Where Not String.Equals("Pfam",
                      FileIO.FileSystem.GetDirectoryInfo(DIR).Name,
                      StringComparison.OrdinalIgnoreCase)
                  Select DIR).ToArray
        Dim swissTCS = (From s As Pfam.PfamString.PfamString
                        In (Pfam & "/swissTCS.Pfam.csv").LoadCsv(Of Pfam.PfamString.PfamString)
                        Select s
                        Group s By s.ProteinId Into Group) _
                            .ToDictionary(Function(x) x.ProteinId.Split(":"c).Last,
                                          elementSelector:=Function(x) x.Group.First)
        Dim LQuery = (From dir As String In source
                      Select FileIO.FileSystem.GetFiles(dir, FileIO.SearchOption.SearchAllSubDirectories, "*.csv") _
                          .Select(Function(x) x.LoadCsv(Of CrossTalks))).ToArray.Unlist.Unlist
        Dim CL = (From Ctk As CrossTalks In LQuery.AsParallel
                  Where swissTCS.ContainsKey(Ctk.Kinase) AndAlso
                      swissTCS.ContainsKey(Ctk.Regulator)
                  Select __contact(swissTCS(Ctk.Kinase), swissTCS(Ctk.Regulator), Ctk)).ToArray
        Return (From x In CL Where Not x Is Nothing Select x).ToArray.SaveTo(Pfam & "/swissTCS_contacts.csv")
    End Function

    Private Function __contact(HisK As Pfam.PfamString.PfamString,
                               RR As Pfam.PfamString.PfamString,
                               CrossTalk As CrossTalks) As Pfam.PfamString.PfamString
        Try
            Return __contactTrace(HisK, RR, CrossTalk)
        Catch ex As Exception
            ex = New Exception(HisK.ToString & vbCrLf &
                               RR.ToString & vbCrLf &
                               CrossTalk.ToString, ex)
            Throw ex
        End Try
    End Function

    Private Function __contactTrace(HisK As Pfam.PfamString.PfamString, RR As Pfam.PfamString.PfamString, CrossTalk As CrossTalks) As Pfam.PfamString.PfamString
        If HisK.PfamString.IsNullOrEmpty OrElse RR.PfamString.IsNullOrEmpty Then
            Return Nothing
        End If

        Dim dsa As New [Set](HisK.Domains)
        Dim dsb As New [Set](RR.Domains)
        Dim PfamString As New Pfam.PfamString.PfamString With {
            .ProteinId = $"{HisK.ProteinId}-{RR.ProteinId.Split(":"c).Last}",
            .Length = HisK.Length + RR.Length,
            .Description = CrossTalk.Probability,
            .Domains = (dsa + dsb).ToArray.Select(Function(x) DirectCast(x, String)).ToArray
        }
        Dim hDomains = HisK.GetDomainData(False)
        Dim rDomains = RR.GetDomainData(False)
        Dim lst As New List(Of ProteinModel.DomainObject)(hDomains)

        For Each d In rDomains
            d.Position = d.Position.OffSet(HisK.Length)
        Next

        Call lst.AddRange(rDomains)

        PfamString.PfamString = lst _
            .Select(Function(x)
                        Return $"{x.Name}({x.Position.Left}|{x.Position.Right})"
                    End Function) _
            .ToArray

        Return PfamString
    End Function

    <ExportAPI("--Profiles.Create", Usage:="--Profiles.Create /MiST2 <MiST2.xml> /pfam <pfam-string.csv> [/out <out.csv>]")>
    Public Function CreateProfiles(args As CommandLine) As Integer
        Dim MiST2 = args("/mist2").LoadXml(Of SMRUCC.genomics.Assembly.MiST2.MiST2)
        Dim Pfam = args("/pfam").LoadCsv(Of Pfam.PfamString.PfamString).ToDictionary(Function(x) x.ProteinId)
        Dim Hisk = MiST2.MajorModules.First.TwoComponent.get_HisKinase
        Dim RR = MiST2.MajorModules.First.TwoComponent.GetRR
        Dim Combos = (From hk As String In Hisk
                      Where Pfam.ContainsKey(hk)
                      Let hkString = Pfam(hk)
                      Select (From reg As String
                              In RR
                              Where Pfam.ContainsKey(reg)
                              Let pretent = New CrossTalks With {
                                  .Kinase = hk,
                                  .Regulator = reg,
                                  .Probability = -1
                              }
                              Select __contactTrace(hkString, Pfam(reg), pretent)).ToArray).Unlist
        Dim out As String = args.GetValue("/out", (args <= "/pfam").TrimSuffix & ".swissTCS.csv")

        Return (From x In Combos Where Not x Is Nothing Select x) _
            .ToArray _
            .SaveTo(out) _
            .CLICode
    End Function

    ''' <summary>
    ''' 做MP比对，得到的结果得分和贝叶斯的计算得分相乘，之后求总和再取平均即为互作的可能性
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("--CrossTalks.Probability",
               Usage:="--CrossTalks.Probability /query <pfam-string.csv> /swiss <swissTCS_pfam-string.csv> [/out <out.CrossTalks.csv> /test <queryName>]")>
    Public Function CrossTalksCal(args As CommandLine) As Integer
        Dim queryFile$ = args <= "/query"
        Dim Query = queryFile.LoadCsv(Of Pfam.PfamString.PfamString).ToArray
        Dim SwissTCS = args("/swiss").LoadCsv(Of Pfam.PfamString.PfamString).ToArray
        Dim out As String = args.GetValue("/out", queryFile.TrimSuffix & ".CrossTalks.csv")
        Dim CrossTalks As New List(Of CrossTalks)
        Dim test As String = args("/test")

        If Not String.IsNullOrEmpty(test) Then
            Dim testData = (From x In Query.AsParallel Where String.Equals(test, x.ProteinId, StringComparison.OrdinalIgnoreCase) Select x).FirstOrDefault
            If testData Is Nothing Then
                Call $"Could not found test data {test}....".__DEBUG_ECHO
                Return -1
            End If
            Call __getProbability(CrossTalks, testData, SwissTCS)
            Return CrossTalks.SaveTo(out).CLICode
        End If

        For i As Integer = 0 To Query.Length - 1
            'Dim sp As New ConsoleDevice.Spinner(txt:="{0} ^b" & pretend.ToString)
            'Call sp.RunTask()
            Dim pretend = Query(i)

            Call $" ({i}/{Query.Length}) {pretend.ToString}".__DEBUG_ECHO
            Call __getProbability(CrossTalks, pretend, SwissTCS)
            'Call sp.Break()
        Next

        Return CrossTalks.SaveTo(out).CLICode
    End Function

    Private Sub __getProbability(ByRef CrossTalks As List(Of CrossTalks),
                                 query As Pfam.PfamString.PfamString,
                                 swissTCS As Pfam.PfamString.PfamString())
        Dim LQuery = (From record As Pfam.PfamString.PfamString
                      In swissTCS.AsParallel
                      Select Pfam.ProteinDomainArchitecture.MPAlignment.PfamStringEquals(query, record, 0.95)).ToArray
        LQuery = (From result In LQuery.AsParallel Where result.NumMatches >= 2 AndAlso result.MatchSimilarity >= 0.95 Select result).ToArray

        If LQuery.IsNullOrEmpty Then
            Return
        End If

        Dim score As Double = LQuery.Select(Function(x) x.MatchSimilarity).Average
        If score <= 0 Then
            Return
        End If

        Dim Tokens As String() = query.ProteinId.Split("-"c)
        Dim CrossTalk As New CrossTalks With {
            .Probability = score,
            .Kinase = Tokens(Scan0),
            .Regulator = Tokens(1)
        }
        Call CrossTalks.Add(CrossTalk)
        Call CrossTalk.__DEBUG_ECHO
    End Sub
End Module
