#Region "Microsoft.VisualBasic::9bca207dac21d3f49385715fb836e01a, CLI_tools\MEME\Cli\MEME.vb"

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
    '     Function: __getLocusTag, __getModels, __getSites, __isFamilyName, __isFile
    '               __isLDMName, __loadMEME, __siteMatchesCommon, GetFastaSource, LogoBatch
    '               MastRegulations, MEMEBatch, MotifScan, RfamSites, SaveModel
    '               SequenceLogoTask, SiteMatch, SiteMatches, SiteMatchesText
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports MEME.Analysis
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Data.Regprecise.Regulator
Imports SMRUCC.genomics.Data.Regprecise.WebServices
Imports SMRUCC.genomics.Data.Regtransbase.WebServices
Imports SMRUCC.genomics.Interops.NBCR
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.ComponentModel
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.LDM
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    <ExportAPI("/mast.Regulations",
               Usage:="/mast.Regulations /in <mastSites.Csv> /correlation <sp_name/DIR> /DOOR <DOOR.opr> [/out <footprint.csv> /cut <0.65>]")>
    Public Function MastRegulations(args As CommandLine) As Integer
        Dim inSites As String = args("/in")
        Dim correlation As String = args("/correlation")
        Dim DOOR As String = args("/DOOR")
        Dim out As String = args.GetValue("/out", inSites.TrimSuffix & ".VirtualFootprints.Csv")
        Dim mastSites = inSites.LoadCsv(Of MastSites)
        Dim result = RegpreciseSummary.SiteToRegulation(mastSites, correlation, DOOR)
        Dim cut As Double = args.GetValue("/cut", 0.65)
        result = (From x In result Where Math.Abs(x.Pcc) >= cut OrElse Math.Abs(x.sPcc) >= cut Select x).ToArray
        Return result.SaveTo(out).CLICode
    End Function

    <ExportAPI("/MEME.Batch")>
    <Description("Batch meme task by using tmod toolbox.")>
    <Usage("/MEME.Batch /in <inDIR> [/out <outDIR> /evalue <1> /nmotifs <30> /mod <zoops> /maxw <100>]")>
    <ArgumentAttribute("/in", False, Description:="A directory path which contains the fasta sequence for the meme motifs analysis.")>
    <ArgumentAttribute("/out", True, Description:="A directory path which outputs the meme.txt data to that directory.")>
    Public Function MEMEBatch(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim out As String = args.GetValue("/out", inDIR & ".MEME_OUT/")
        Dim evalue As String = args.GetValue("/evalue", "1")
        Dim n As String = args.GetValue("/nmotifs", "30")
        Dim mode As String = args.GetValue("/mod", "zoops")
        Dim maxW As Integer = args.GetValue("/maxw", 100)

        Call "Try initialize TMoD toolbox sessions....".__DEBUG_ECHO
        Call TMoD.InitializeSession()
        Call "Try start MEME jobs...".__DEBUG_ECHO
        Call TMoD.BatchMEMEScanning(inDIR, out, evalue, n, mode, maxw:=maxW)

        Return 0
    End Function

    <ExportAPI("--logo.Batch", Usage:="--logo.Batch -in <inDIR> [/out <outDIR>]")>
    Public Function LogoBatch(args As CommandLine) As Integer
        Dim inDIR As String = args("-in")
        Dim out As String = args.GetValue("/out", inDIR)
        Return SequenceLogoAPI.BatchDrawingFromDirectory(inDIR).CLICode
    End Function

    <ExportAPI("/seq.logo", Usage:="/seq.logo /in <meme.txt> [/out <outDIR>]")>
    Public Function SequenceLogoTask(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & ".Logo/")
        Return SequenceLogoAPI.BatchDrawing(inFile, out)
    End Function

    ''' <summary>
    ''' 使用片段相似性来扫描Motif位点
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("MotifScan",
               Info:="Scan for the motif site by using fragment similarity.",
               Usage:="MotifScan -nt <nt.fasta> /motif <motifLDM.xml/LDM_Name/FamilyName> [/delta <default:80> /delta2 <default:70> /offSet <default:5> /out <saved.csv>]")>
    Public Function MotifScan(args As CommandLine) As Integer
        Dim Nt As String = args("-nt")
        Dim Motif As String = args("/motif")
        Dim Delta As Double = args.GetValue(Of Double)("/delta", 80)
        Dim Delta2 As Double = args.GetValue(Of Double)("/delta2", 70)
        Dim OffSet As Integer = args.GetValue(Of Integer)("/offset", 5)
        Dim Files As String() = __getModels(Motif)

        If Files.IsNullOrEmpty Then
            Call $"Motif source '{Motif}' is not a valid source input!".__DEBUG_ECHO
            Return -100
        End If

        Dim NtSequence = FastaSeq.Load(Nt)
        Dim LQuery = (From path As String
                      In Files
                      Let MotifSites = New MEME_Suite.Analysis.MotifScans.MotifScans(
                          path.LoadXml(Of AnnotationModel),
                          Nothing,
                          Delta,
                          Delta2,
                          OffSet)
                      Select MotifSites.Mast(NtSequence)).Unlist
        Dim Out As String = args("/out")
        If String.IsNullOrEmpty(Out) Then
            Out = $"{Nt}.{BaseName(Motif)}.csv"
        End If
        Return LQuery.SaveTo(Out, False).CLICode
    End Function

    Private Function __isFile(path As String) As Boolean
        Return path.FileExists
    End Function

    Private Function __isLDMName(name As String) As Boolean
        Return name.IndexOf("."c) > 0 AndAlso $"{GCModeller.FileSystem.MotifLDM}/{name}.xml".FileExists
    End Function

    Private Function __isFamilyName(name As String) As Boolean
        Return String.Equals(name, name.NormalizePathString(True), StringComparison.OrdinalIgnoreCase)
    End Function

    Private Function __getModels(motif As String) As String()
        If __isFile(motif) Then
            Return {motif}
        ElseIf __isLDMName(motif) Then
            Return {$"{GCModeller.FileSystem.MotifLDM}/{motif}.xml"}
        ElseIf __isFamilyName(motif) Then
            Return FileIO.FileSystem.GetFiles(GCModeller.FileSystem.MotifLDM, FileIO.SearchOption.SearchTopLevelOnly, $"{motif}*.xml").ToArray
        Else
            Return Nothing
        End If
    End Function

    <ExportAPI("--site.Match",
               Usage:="--site.Match /meme <meme.text> /mast <mast.xml> /out <out.csv> [/ptt <genome.ptt> /len <150,200,250,300,350,400,450,500>]")>
    <ArgumentAttribute("/len", True,
                   Description:="If not specific this parameter, then the function will trying to parsing the length value from the meme text automatically.")>
    Public Function SiteMatch(args As CommandLine) As Integer
        Dim Motifs As Motif() =
            args.GetObject(
            "/meme",
            AddressOf DocumentFormat.MEME.Text.Load)
        Dim Mast As XmlOutput.MAST.MAST =
            args.GetObject(Of XmlOutput.MAST.MAST)(
            "/mast", AddressOf LoadXml)
        Dim ResultSet = MEME_Suite.Analysis.HtmlMatching.Match(Motifs, Mast)

        If Not String.IsNullOrEmpty(args("/ptt")) Then
            Dim PTT = args.GetObject("/ptt", AddressOf TabularFormat.PTT.Load)
            Dim Length As Integer = args.GetValue("/len", DocumentFormat.MEME.Text.GetLength(args("/meme")))
            ResultSet = HtmlMatching.Match(ResultSet, PTT, Length)
        End If

        Return ResultSet.SaveTo(args("/out")).CLICode
    End Function

    <ExportAPI("/model.save")>
    <Description("/model.save /model <meme.txt> [/out <model.json>]")>
    Public Function SaveModel(args As CommandLine) As Integer
        Dim in$ = args <= "/model"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.json"
        Dim motifs As Motif() = DocumentFormat.MEME.Text.Load([in])
        Dim models As SequenceMotif() = motifs.CreateMotifModels.ToArray

        Return models.GetJson.SaveTo(out).CLICode
    End Function

    '''' <summary>
    '''' 寻找出在meme之中存在的但是在mast之中找不到结果的就是可能是本物种特异的调控位点
    '''' </summary>
    '''' <param name="args"></param>
    '''' <returns></returns>
    '<ExportAPI("--site.Novel", Usage:="--site.Novel /meme <DIR.meme.text> /mast <DIR.mast.xml> /out <out.csv> [/ptt <genome.ptt>]")>
    'Public Function SitesNovel(args As CommandLine) As Integer
    '    Dim MEMESrc As String = FileIO.FileSystem.GetDirectoryInfo(args("/meme")).FullName
    '    Dim Motifs As String() = FileIO.FileSystem.GetFiles(MEMESrc, FileIO.SearchOption.SearchAllSubDirectories, "meme.txt").ToArray
    '    Dim MastSrc As String = FileIO.FileSystem.GetDirectoryInfo(args("/mast")).FullName
    '    Dim Masts As String() = FileIO.FileSystem.GetFiles(MastSrc, FileIO.SearchOption.SearchAllSubDirectories, "mast.xml").ToArray  ' 自身所计算出来的Motif和regprecise数据库之中的数据的比对结果

    '    Dim MEMESets = (From file As String
    '                    In Motifs
    '                    Let id As String = file.Replace(MEMESrc, "").Replace("meme.txt", "")
    '                    Select id, file).ToDictionary(Function(obj) obj.id, elementSelector:=Function(obj) obj.file)
    '    Dim MastSets = (From file As String
    '                    In Masts
    '                    Let id As String = file.Replace(MastSrc, "").Replace("mast.xml", "")
    '                    Select id, file).ToArray   ' 请注意，在mast结果之中存在的都是已经被研究过的位点，而没有被研究过的位点则是在meme之中存在的但是在mast结果之中找不到的

    '    Call $"Start to scanning {MastSets.Length} mast records...".__DEBUG_ECHO

    '    Dim LQuery = (From mastFile In MastSets.AsParallel
    '                  Let mm = mastFile.file.LoadXml(Of SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.XmlOutput.MAST.MAST)(ThrowEx:=False)
    '                  Where mm Is Nothing' 加载出错说明没有生成mast文档，则可能是一个novel位点
    '                  Let memeFile = MEMESets(mastFile.id)
    '                  Let testEcho As String = memeFile.ToFileURL.__DEBUG_ECHO
    '                  Select mastFile, memeFile).ToArray

    '    Dim LoadNovels = (From file In LQuery.AsParallel
    '                      Let memes = SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.Text.SafelyLoad(file.memeFile)
    '                      Where Not memes.IsNullOrEmpty
    '                      Select file.mastFile.id, memes).ToArray

    'End Function

    <ExportAPI("--site.Matches.text",
               Usage:="--site.Matches.text /meme <DIR.meme.text> /mast <DIR.mast.xml> /out <out.csv> [/ptt <genome.ptt> /fasta <original.fasta.DIR>]",
               Info:="Using this function for processing the meme text output from the tmod toolbox.")>
    Public Function SiteMatchesText(args As CommandLine) As Integer
        Dim MEMESrc As String = FileIO.FileSystem.GetDirectoryInfo(args("/meme")).FullName
        Dim Motifs As String() = FileIO.FileSystem.GetFiles(MEMESrc, FileIO.SearchOption.SearchAllSubDirectories, "*.txt").ToArray
        Dim MastSrc As String = FileIO.FileSystem.GetDirectoryInfo(args("/mast")).FullName
        Dim Masts As String() = FileIO.FileSystem.GetFiles(MastSrc, FileIO.SearchOption.SearchAllSubDirectories, "mast.xml").ToArray  ' 自身所计算出来的Motif和regprecise数据库之中的数据的比对结果

        Call $"Get {Motifs.Length - 1} meme motif data entries...".__DEBUG_ECHO
        Dim MEMESets = (From file As String
                        In Motifs.AsParallel
                        Let id As String = file.Replace(MEMESrc, "")
                        Select id, file) _
                            .ToDictionary(Function(obj) obj.id,
                                          Function(obj) obj.file)
        Call $"Get {Masts.Length - 1} mast motif data entries...".__DEBUG_ECHO
        Dim MastSets = (From file As String
                        In Masts.AsParallel
                        Let _id As String = file.Replace(MastSrc, "").Replace("mast.xml", "")
                        Select id = Mid(_id, 1, Len(_id) - 1), file) _
                            .ToDictionary(Function(obj) obj.id,
                                          Function(obj) obj.file)   ' 请注意，在mast结果之中存在的都是已经被研究过的位点，而没有被研究过的位点则是在meme之中存在的但是在mast结果之中找不到的
        Return __siteMatchesCommon(MEMESets, MastSets, args("/out"), args("/ptt"), args("/fasta"))
    End Function

    Public Function GetFastaSource(FastaDIR As String) As Dictionary(Of String, String)
        FastaDIR = FileIO.FileSystem.GetDirectoryInfo(FastaDIR).FullName
        Dim FastaSource = (From file As String
                           In FileIO.FileSystem.GetFiles(
                               FastaDIR,
                               FileIO.SearchOption.SearchAllSubDirectories,
                               "*.fasta", "*.fa", "*.fsa")
                           Let id As String = FileIO.FileSystem.GetParentPath(file).Replace(FastaDIR, "") & "\" & BaseName(file) & ".txt"
                           Select id, file) _
                                .ToDictionary(Function(file) file.id,
                                              Function(file) file.file)
        Return FastaSource
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="ID"></param>
    ''' <param name="memeFile"></param>
    ''' <param name="source">Fasta source</param>
    ''' <param name="LocusFromFasta"></param>
    ''' <returns></returns>
    Private Function __loadMEME(ID As String, memeFile As String, source As Dictionary(Of String, String), LocusFromFasta As Boolean) As Motif()
        Dim motifs As Motif() = DocumentFormat.MEME.Text.SafelyLoad(memeFile)

        If LocusFromFasta Then
            Dim fastaFile As String = source(ID)
            Dim Fasta = SMRUCC.genomics.SequenceModel.FASTA.FastaFile.Read(fastaFile)
            Dim GeneLocus As Dictionary(Of String, String) = Fasta.Select(
                Function(fa) New With {
                    .key = fa.Title.Split().First,
                    .locusTag = __getLocusTag(fa.Title)}) _
                    .ToDictionary(Function(fa) fa.key,
                                  Function(fa) fa.locusTag)
            For Each motif In motifs
                For Each site In motif.Sites
                    site.Name = GeneLocus(site.Name)
                Next
            Next
        End If

        Return motifs
    End Function

    Private Function __getLocusTag(title As String) As String
        Try
            Return title.Split()(2).Split("="c)(1).Split(";"c).First
        Catch ex As Exception
            Return title
        End Try
    End Function

    ''' <summary>
    ''' 利用meme得到的motif数据，不是从数据库之中匹配出来的数据
    ''' </summary>
    ''' <param name="MEMESets"></param>
    ''' <param name="MastSets"></param>
    ''' <param name="out"></param>
    ''' <param name="PTTFile"></param>
    ''' <param name="FastaDir"></param>
    ''' <returns></returns>
    Private Function __siteMatchesCommon(MEMESets As Dictionary(Of String, String),
                                         MastSets As Dictionary(Of String, String),
                                         out As String,
                                         PTTFile As String, FastaDir As String) As Boolean
        Dim initSet = (From [set] As KeyValuePair(Of String, String)
                       In MastSets.AsParallel
                       Where FileIO.FileSystem.GetFileInfo([set].Value).Length > 0 AndAlso
                           MEMESets.ContainsKey([set].Key)
                       Select [set]).ToArray

        Call $"Start to processing {initSet.Length} motif data set....".__DEBUG_ECHO

        Dim LocusFromFasta As Boolean
        Dim FastaSource As Dictionary(Of String, String)

        If Not String.IsNullOrEmpty(FastaDir) Then
            LocusFromFasta = True
            FastaSource = GetFastaSource(FastaDir)
        Else
            LocusFromFasta = False
            FastaSource = New Dictionary(Of String, String)
        End If

        Dim ResultSet = (From [set] As KeyValuePair(Of String, String)
                         In initSet.AsParallel
                         Let memeFile As String = MEMESets([set].Key)
                         Let memeMotifs = __loadMEME([set].Key, memeFile, FastaSource, LocusFromFasta)
                         Let sites = __getSites(memeMotifs, [set].Value)
                         Let novels = HtmlMatching.NovelSites(memeMotifs, sites)
                         Select sites, [set], memeFile, novels).ToArray

        Dim chunkBuffer As IEnumerable(Of MotifSite)
        Dim setTag = New SetValue(Of MotifSite) <= NameOf(MotifSite.Tag)

        If Not String.IsNullOrEmpty(PTTFile) Then  ' 假若ptt数据不为空的话，则还会联系基因组上面的元素
            Dim PTT As PTT = TabularFormat.PTT.Load(PTTFile)

            Call $"Match genome context from {PTTFile.ToFileURL}".__DEBUG_ECHO

            Dim LQuery = (From [set] In ResultSet.AsParallel
                          Let len As Integer = DocumentFormat.MEME.Text.GetLength([set].memeFile)
                          Select HtmlMatching.Match([set].sites, PTT, len) _
                              .Select(Function(site) setTag(site, [set].set.Key))).ToArray
            chunkBuffer = LQuery.Unlist
        Else
            Dim LQuery = (From [set]
                          In ResultSet
                          Select [set].sites.Select(Function(site) setTag(site, [set].set.Key))).ToArray
            chunkBuffer = LQuery.Unlist
        End If

        If Not GCModeller.FileSystem.IsNullOrEmpty AndAlso
            GCModeller.FileSystem.Regulations.FileExists Then

            Call "Associates with regprecise family information...".__DEBUG_ECHO
            Dim Regulations As Regulations = GCModeller.FileSystem.Regulations.LoadXml(Of Regulations)
            Dim setValue = New SetValue(Of MotifSite) <= NameOf(MotifSite.Family)
            chunkBuffer = chunkBuffer.Select(
                Function(site) setValue(site, Regulations.GetMotifFamily(site.uid.Split("|"c).ElementAtOrDefault(Scan0)))).ToArray
        End If

        Dim novelSites = ResultSet.Select(Function([set]) [set].novels).Unlist.TrimNull
        Call novelSites.SaveTo(out.TrimSuffix & ".novels.csv")
        Return chunkBuffer.SaveTo(out).CLICode
    End Function

    ''' <summary>
    ''' 这个函数是和regprecise的匹配结果
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("--site.Matches",
               Usage:="--site.Matches /meme <DIR.meme.text> /mast <DIR.mast.xml> /out <out.csv> [/ptt <genome.ptt>]")>
    Public Function SiteMatches(args As CommandLine) As Integer
        Dim MEMESrc As String = FileIO.FileSystem.GetDirectoryInfo(args("/meme")).FullName
        Dim Motifs As String() = FileIO.FileSystem.GetFiles(MEMESrc, FileIO.SearchOption.SearchAllSubDirectories, "meme.txt").ToArray
        Dim MastSrc As String = FileIO.FileSystem.GetDirectoryInfo(args("/mast")).FullName
        Dim Masts As String() = FileIO.FileSystem.GetFiles(MastSrc, FileIO.SearchOption.SearchAllSubDirectories, "mast.xml").ToArray  ' 自身所计算出来的Motif和regprecise数据库之中的数据的比对结果

        Dim MEMESets = (From file As String
                        In Motifs
                        Let id As String = file.Replace(MEMESrc, "").Replace("meme.txt", "")
                        Select id, file) _
                            .ToDictionary(Function(obj) obj.id,
                                          Function(obj) obj.file)
        Dim MastSets = (From file As String
                        In Masts
                        Let id As String = file.Replace(MastSrc, "").Replace("mast.xml", "")
                        Select id, file) _
                            .ToDictionary(Function(obj) obj.id,
                                          Function(obj) obj.file)   ' 请注意，在mast结果之中存在的都是已经被研究过的位点，而没有被研究过的位点则是在meme之中存在的但是在mast结果之中找不到的

        Return __siteMatchesCommon(MEMESets, MastSets, args("/out"), args("/ptt"), "")
    End Function

    Private Function __getSites(memeMotifs As Motif(), mast As String) As MotifSite()
        Dim mastXml = mast.LoadXml(Of DocumentFormat.XmlOutput.MAST.MAST)(throwEx:=False)
        Dim sites = HtmlMatching.Match(memeMotifs, mastXml)
        Call Console.Write(".")
        Return sites
    End Function

    <ExportAPI("/RfamSites", Usage:="/RfamSites /source <sourceDIR> [/out <out.fastaDIR>]")>
    Public Function RfamSites(args As CommandLine) As Integer
        Dim inDIR As String = args("/source")
        Dim out As String = args.GetValue("/out", inDIR & ".Rfam.Sites")
        Dim loadFile = From file As String
                       In FileIO.FileSystem.GetFiles(inDIR, FileIO.SearchOption.SearchTopLevelOnly, "*.xml").AsParallel
                       Select file.LoadXml(Of BacteriaRegulome)
        Dim RfamSitesLQuery = (From x In loadFile.AsParallel
                               Let rfam = (From regulator In x.regulome.regulators Where regulator.type = Types.RNA Select regulator)
                               Select (From rna In rfam Select rna.family, rna.regulatorySites).ToArray).Unlist
        Dim RfamCategory = (From x In RfamSitesLQuery
                            Select x
                            Group x By x.family Into Group) _
                                 .ToDictionary(Function(x) x.family,
                                               Function(x) x.Group.Select(Function(xx) xx.regulatorySites).Unlist)
        For Each cat As KeyValuePair(Of String, List(Of MotifFasta)) In RfamCategory
            Dim path As String = $"{out}/{cat.Key}.fasta"
            Dim fa As New FastaFile(cat.Value)
            Call fa.Save(path)
        Next

        Return 0
    End Function
End Module
