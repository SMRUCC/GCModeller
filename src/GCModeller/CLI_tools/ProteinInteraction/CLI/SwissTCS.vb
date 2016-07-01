Imports SMRUCC.genomics.AnalysisTools.ProteinTools.Interactions.SwissTCS
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic

Partial Module CLI

    <ExportAPI("--SwissTCS.Downloads", Usage:="--SwissTCS.Downloads /out <out.DIR>")>
    Public Function DownloadEntireDb(args As CommandLine.CommandLine) As Integer
        Dim out As String = args("/out")
        Call SwissRegulon.Download(out)
        Return 0
    End Function

    <ExportAPI("--ProtFasta.Downloads", Usage:="--ProtFasta.Downloads /in <sp.DIR>")>
    Public Function ProtFastaDownloads(args As CommandLine.CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Call __downloads(inDIR)
        Return 0
    End Function

    Private Sub __downloads(inDIR As String)
        Dim Hisk As String() = FileIO.FileSystem.GetFiles(inDIR & "/HisK/",
                                                        FileIO.SearchOption.SearchTopLevelOnly,
                                                        "*.csv").ToArray(Function(x) IO.Path.GetFileNameWithoutExtension(x))
        Dim RR As String() = FileIO.FileSystem.GetFiles(inDIR & "/RR/",
                                                        FileIO.SearchOption.SearchTopLevelOnly,
                                                        "*.csv").ToArray(Function(x) IO.Path.GetFileNameWithoutExtension(x))
        Dim fa = SMRUCC.genomics.Assembly.KEGG.WebServices.DownloadsBatch(inDIR, Hisk)
        If Not fa Is Nothing Then Call fa.Save(inDIR & "/HisK.fasta")
        fa = SMRUCC.genomics.Assembly.KEGG.WebServices.DownloadsBatch(inDIR, RR)
        If Not fa Is Nothing Then Call fa.Save(inDIR & "/RR.fasta")
    End Sub

    <ExportAPI("--ProtFasta.Downloads.Batch", Usage:="--ProtFasta.Downloads.Batch /in <sp.DIR.Source>")>
    Public Function ProtFastaDownloadsBatch(args As CommandLine.CommandLine) As Integer
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
    Public Function MergePfam(args As CommandLine.CommandLine) As Integer
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
                        Select Hisk.Join(RR)).ToArray.MatrixToList
        Dim Merge As String = Temp & "/swissTCS.fasta"
        Dim MergeFasta As New SMRUCC.genomics.SequenceModel.FASTA.FastaFile(LoadProt)
        Call MergeFasta.Save(Merge)

        Dim blast As New SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Programs.BLASTPlus(GCModeller.FileSystem.GetLocalBlast)
        Dim Pfam As String = GCModeller.FileSystem.CDD & "/Pfam.fasta"
        Dim out As String = Temp & "/Pfam.txt"

        Call blast.FormatDb(Pfam, blast.MolTypeProtein).Start(WaitForExit:=True)
        Call blast.Blastp(Merge, Pfam, out).Start(WaitForExit:=True)

        Dim Logs = SMRUCC.genomics.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus.Parser.TryParse(out)
        Dim PfamString = SMRUCC.genomics.AnalysisTools.ProteinTools.Sanger.Pfam.CreatePfamString(Logs, disableUltralarge:=True)
        out = Temp & "swissTCS.Pfam.csv"
        Return PfamString.SaveTo(out)
    End Function

    Sub New()
        Call Settings.Session.Initialize()
    End Sub

    <ExportAPI("--Contacts", Usage:="--Contacts /in <in.DIR>")>
    Public Function Contacts(args As CommandLine.CommandLine) As Integer
        Dim input As String = args("/in")
        Dim Pfam As String = input & "/Pfam/"
        Dim source As String() = FileIO.FileSystem.GetDirectories(input, FileIO.SearchOption.SearchTopLevelOnly).ToArray
        source = (From DIR As String
                  In source
                  Where Not String.Equals("Pfam",
                      FileIO.FileSystem.GetDirectoryInfo(DIR).Name,
                      StringComparison.OrdinalIgnoreCase)
                  Select DIR).ToArray
        Dim swissTCS = (From s As Sanger.Pfam.PfamString.PfamString
                        In (Pfam & "/swissTCS.Pfam.csv").LoadCsv(Of Sanger.Pfam.PfamString.PfamString)
                        Select s
                        Group s By s.ProteinId Into Group) _
                            .ToDictionary(Function(x) x.ProteinId.Split(":"c).Last,
                                          elementSelector:=Function(x) x.Group.First)
        Dim LQuery = (From dir As String In source
                      Select FileIO.FileSystem.GetFiles(dir, FileIO.SearchOption.SearchAllSubDirectories, "*.csv") _
                          .ToArray(Function(x) x.LoadCsv(Of CrossTalks))).ToArray.MatrixToList.MatrixToList
        Dim CL = (From Ctk As CrossTalks In LQuery.AsParallel
                  Where swissTCS.ContainsKey(Ctk.Kinase) AndAlso
                      swissTCS.ContainsKey(Ctk.Regulator)
                  Select __contact(swissTCS(Ctk.Kinase), swissTCS(Ctk.Regulator), Ctk)).ToArray
        Return (From x In CL Where Not x Is Nothing Select x).ToArray.SaveTo(Pfam & "/swissTCS_contacts.csv")
    End Function

    Private Function __contact(HisK As Sanger.Pfam.PfamString.PfamString,
                               RR As Sanger.Pfam.PfamString.PfamString,
                               CrossTalk As CrossTalks) As Sanger.Pfam.PfamString.PfamString
        Try
            Return __contactTrace(HisK, RR, CrossTalk)
        Catch ex As Exception
            ex = New Exception(HisK.ToString & vbCrLf &
                               RR.ToString & vbCrLf &
                               CrossTalk.ToString, ex)
            Throw ex
        End Try
    End Function

    Private Function __contactTrace(HisK As Sanger.Pfam.PfamString.PfamString,
                                    RR As Sanger.Pfam.PfamString.PfamString,
                                    CrossTalk As CrossTalks) As Sanger.Pfam.PfamString.PfamString
        If StringHelpers.IsNullOrEmpty(HisK.PfamString) OrElse
            StringHelpers.IsNullOrEmpty(RR.PfamString) Then
            Return Nothing
        End If

        Dim dsa As New Microsoft.VisualBasic.ComponentModel.DataStructures.Set(HisK.Domains)
        Dim dsb As New Microsoft.VisualBasic.ComponentModel.DataStructures.Set(RR.Domains)
        Dim PfamString As New Sanger.Pfam.PfamString.PfamString With {
            .ProteinId = $"{HisK.ProteinId}-{RR.ProteinId.Split(":"c).Last}",
            .Length = HisK.Length + RR.Length,
            .Description = CrossTalk.Probability,
            .Domains = (dsa + dsb).ToArray.ToArray(Function(x) DirectCast(x, String))
        }
        Dim hDomains = HisK.GetDomainData(False)
        Dim rDomains = RR.GetDomainData(False)
        Dim lst As New List(Of ProteinModel.DomainObject)(hDomains)

        For Each d In rDomains
            d.Position = d.Position.OffSet(HisK.Length)
        Next

        Call lst.AddRange(rDomains)
        PfamString.PfamString = lst.ToArray(Function(x) $"{x.Identifier}({x.Position.Left}|{x.Position.Right})")

        Return PfamString
    End Function

    <ExportAPI("--Profiles.Create", Usage:="--Profiles.Create /MiST2 <MiST2.xml> /pfam <pfam-string.csv> [/out <out.csv>]")>
    Public Function CreateProfiles(args As CommandLine.CommandLine) As Integer
        Dim MiST2 = args("/mist2").LoadXml(Of SMRUCC.genomics.Assembly.MiST2.MiST2)
        Dim Pfam = args("/pfam").LoadCsv(Of Sanger.Pfam.PfamString.PfamString).ToDictionary(Function(x) x.ProteinId)
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
                              Select __contactTrace(hkString, Pfam(reg), pretent)).ToArray).ToArray.MatrixToList
        Dim out As String = args.GetValue("/out", args("/pfam").TrimFileExt & ".swissTCS.csv")
        Return (From x In Combos Where Not x Is Nothing Select x).ToArray.SaveTo(out).CLICode
    End Function

    ''' <summary>
    ''' 做MP比对，得到的结果得分和贝叶斯的计算得分相乘，之后求总和再取平均即为互作的可能性
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("--CrossTalks.Probability",
               Usage:="--CrossTalks.Probability /query <pfam-string.csv> /swiss <swissTCS_pfam-string.csv> [/out <out.CrossTalks.csv> /test <queryName>]")>
    Public Function CrossTalksCal(args As CommandLine.CommandLine) As Integer
        Dim Query = args("/query").LoadCsv(Of Sanger.Pfam.PfamString.PfamString).ToArray
        Dim SwissTCS = args("/swiss").LoadCsv(Of Sanger.Pfam.PfamString.PfamString).ToArray
        Dim out As String = args.GetValue("/out", args("/query").TrimFileExt & ".CrossTalks.csv")
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
                                 query As Sanger.Pfam.PfamString.PfamString,
                                 swissTCS As Sanger.Pfam.PfamString.PfamString())
        Dim LQuery = (From record As Sanger.Pfam.PfamString.PfamString
                      In swissTCS.AsParallel
                      Select Sanger.Pfam.ProteinDomainArchitecture.MPAlignment.PfamStringEquals(query, record, 0.95)).ToArray
        LQuery = (From result In LQuery.AsParallel Where result.NumMatches >= 2 AndAlso result.MatchSimilarity >= 0.95 Select result).ToArray

        If LQuery.IsNullOrEmpty Then
            Return
        End If

        Dim score As Double = LQuery.ToArray(Function(x) x.MatchSimilarity).Average
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