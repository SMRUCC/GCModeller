#Region "Microsoft.VisualBasic::c92aeee67e075f1ebda82d66deeb2557, CLI_tools\ProteinInteraction\CLI\Interactions.vb"

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
    '     Function: __align, __getCategory, __getScore, __loadFa, Align
    '               (+2 Overloads) AlignLDM, DbMergeFromExists, DomainInteractions, GenerateModel, Predicts
    '               SignatureGenerates
    '     Class Category
    ' 
    '         Properties: Alignments, Signature
    ' 
    '         Function: CreateObject, Folk, FromAlign, GetSignatureFasta
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming.Levenshtein
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Parallel.Tasks
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics
Imports SMRUCC.genomics.Analysis.ProteinTools
Imports SMRUCC.genomics.Data.Xfam
Imports SMRUCC.genomics.Data.Xfam.Pfam.ProteinDomainArchitecture.MPAlignment
Imports SMRUCC.genomics.Interops
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.Patterns.Clustal

Partial Module CLI

    ''' <summary>
    ''' 会将该分类之下的序列倒出来做一次多序列比对
    ''' 然后将分别进行特征比较，得分最高的特征为其最终的得分
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("--predicts.TCS", Usage:="--predicts.TCS /pfam <pfam-string.csv> /prot <prot.fasta> /Db <interaction.xml>")>
    Public Function Predicts(args As CommandLine) As Integer
        Dim Interactions = args("/db").LoadXml(Of Category())
        Dim predictSource = (args <= "/pfam").LoadCsv(Of Pfam.PfamString.PfamString)
        Dim prot = FastaFile.Read(args("/prot")).ToDictionary(Function(x) x.Headers.First.Split.First.Split(":"c).Last)
        Dim clustal As ClustalOrg.Clustal = ClustalOrg.Clustal.CreateSession
        Dim LQuery = (From predictsX As Pfam.PfamString.PfamString
                      In predictSource'.AsParallel
                      Select predictsX,
                           score = __getScore(predictsX, prot, Interactions, clustal)).ToArray
        Dim setValue = New SetValue(Of Pfam.PfamString.PfamString) <=
            NameOf(Pfam.PfamString.PfamString.Description)
        Dim out = LQuery.Select(Function(x) setValue(x.predictsX, CStr(x.score)))
        Return out.SaveTo((args <= "/pfam").TrimSuffix & ".Interactions.csv")
    End Function

    Private Function __getScore(predict As Pfam.PfamString.PfamString,
                                prot As Dictionary(Of String, FastaSeq),
                                subject As Category(),
                                clustal As ClustalOrg.Clustal) As Double
        Dim tokens = predict.ProteinId.Split("-"c)
        Dim contacts As New FASTA.FastaSeq With {
            .Headers = {predict.ProteinId},
            .SequenceData = prot(tokens.First).SequenceData & prot(tokens.Last).SequenceData
        }
        Dim struct = predict.GetDomainData(False).Select(Function(x) x.Name).ToArray
        Dim LQuery = (From x In subject.AsParallel
                      Let fm_tokens As String() = x.Family.Split("+"c)
                      Let score = LevenshteinDistance.Similarity(struct, fm_tokens, 0.95)
                      Where score > 0
                      Select score, x
                      Order By score Descending).Take(10).ToArray

        For i As Integer = 0 To LQuery.Length - 1
            Dim score As Double = 0
            Call __align(contacts, LQuery(i).x, clustal, score)

            If score > 0 Then
                Return score
            End If
        Next

        Return 0
    End Function

    Public Function Align(query As FastaSeq, category As Category,
                          ByRef score As Double,
                          Optional mp As Double = 0.65) As LevAlign
        Dim clustal As ClustalOrg.Clustal = ClustalOrg.Clustal.CreateSession
        Return __align(query, category, clustal, score, mp)
    End Function

    Private Function __align(seq As FastaSeq,
                             category As Category,
                             clustal As ClustalOrg.Clustal,
                             ByRef score As Double,
                             Optional mp As Double = 0.65,
                             Optional block As Double = 0.6) As LevAlign
        If category.Signature.IsNullOrEmpty Then
            Return Nothing
        End If

        ' category = category.Folk(1, 5)

        Dim n As Integer = CInt(category.Signature.Length / 2) - 1
        Dim source = (From x In category.Signature Select New FASTA.FastaSeq(x)).AsList
        Call source.Add(seq.Repeats(n))
        Dim tmp = TempFileSystem.GetAppSysTempFile(".fasta")
        Call New FASTA.FastaFile(source).Save(tmp)
        Dim aln = clustal.MultipleAlignment(tmp)
        Dim SRChains As SRChain() = SR.FromAlign(aln, block)
        Dim f = category.Signature.First.PfamString.PfamString.Length
        Dim SRChain = (From x In SRChains Where x.Hits >= f Select x).FirstOrDefault

        If SRChain Is Nothing Then
            score = 0
            Return Nothing
        End If

        Dim sig = SRChain.lstSR.GetPfamString("query")
        Dim s As Double = 1
        Dim d As Double = 1 / category.Signature.Length

        For Each lv In category.Signature
#If DEBUG Then
            Call sig.__DEBUG_ECHO
            Call lv.PfamString.__DEBUG_ECHO
#End If
            Dim out As LevAlign = PfamStringEquals(sig, lv.PfamString, mp)
            score = out.MatchSimilarity

            If score >= s - d Then
                score = s * score
                Return out
            Else
                s -= d
            End If
        Next

        score = 0

        Return Nothing
    End Function

    ''' <summary>
    ''' 从已经存在的多重比对的数据之中建立数据库
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("--Db.From.Exists", Usage:="--Db.From.Exists /aln <clustal-aln.DIR> /pfam <pfam-string.csv>")>
    Public Function DbMergeFromExists(args As CommandLine) As Integer
        Dim inPfam = (args <= "/pfam").LoadCsv(Of Pfam.PfamString.PfamString)
        Dim LoadFasta = (From file As String
                         In FileIO.FileSystem.GetFiles(args("/aln"), FileIO.SearchOption.SearchTopLevelOnly, "*.fasta").AsParallel
                         Select FastaFile.Read(file)).Select(Function(x) x.ToDictionary(Function(xx) xx.Title.Split.First.Split(":"c).Last)).ToArray
        Dim LQuery = (From x As Pfam.PfamString.PfamString
                      In inPfam.AsParallel
                      Let order As String = (From id As ProteinModel.DomainObject
                                             In x.GetDomainData(False)
                                             Select id.Name).JoinBy("+")
                      Select order, x
                      Group By order Into Group) _
                 .ToDictionary(Function(x) x.order,
                               elementSelector:=Function(x) x.Group.Select(Function(xx) xx.x).ToArray)
        Dim Categories = LQuery.Select(Function(x) Family.FileSystem.Family.CreateObject(x.Key, x.Value))
        Dim list = (From fm As Family.FileSystem.Family
                    In Categories.AsParallel
                    Select __getCategory(fm, LoadFasta)).ToArray
        Return list.GetXml.SaveTo((args <= "/pfam").TrimSuffix & ".PfamMPAln.xml").CLICode
    End Function

    Private Function __getCategory(fm As Family.FileSystem.Family, loadFasta As Dictionary(Of String, SequenceModel.FASTA.FastaSeq)()) As Category
        Dim ct = (From rec As Family.FileSystem.PfamString
                  In fm.PfamString
                  Let id As String = rec.LocusTag.Split(":"c).Last
                  Let Tokens As String() = id.Split("-"c)
                  Select hk = Tokens(0), Rg = Tokens(1)).ToArray
        Dim getHk = (From file In loadFasta Let hits = (From x In ct Let hhh = file.TryGetValue(x.hk) Where Not hhh Is Nothing Select hhh).ToArray Select hits Order By hits.Length Descending).First
        Dim getRR = (From file In loadFasta Let hits = (From x In ct Let hhh = file.TryGetValue(x.Rg) Where Not hhh Is Nothing Select hhh).ToArray Select hits Order By hits.Length Descending).First
        Dim interactions As String() = fm.PfamString.Select(Function(x) x.LocusTag)
        Dim alignments As KeyValuePair() = Nothing
        Dim rp = AlignLDM((From x In getHk Select x Group By x.Title.Split.First Into Group).Select(Function(x) x.Group.First),
                          (From x In getRR Select x Group By x.Title.Split.First Into Group).Select(Function(x) x.Group.First),
                          interactions,
                          fm.Family,
                          alignments)
        Return Category.CreateObject(fm, rp, alignments)
    End Function

    <ExportAPI("--signature", Usage:="--signature /in <aln.fasta> [/p-cut <0.95>]")>
    Public Function SignatureGenerates(args As CommandLine) As Integer
        Dim aln As New FastaFile(args("/in"))
        Dim pCut As Double = args.GetValue("/p-cut", 0.95)
        Dim SRChain As SRChain() = SR.FromAlign(aln, pCut)
        Dim sig = SRChain.Select(Function(x) Signature.CreateObject(x.lstSR, ""))
        Dim fa As New SequenceModel.FASTA.FastaFile(sig)
        Return fa.Save((args <= "/in").TrimSuffix & "Signatures.fasta")
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="HisK"></param>
    ''' <param name="RR"></param>
    ''' <returns></returns>
    Public Function AlignLDM(HisK As SequenceModel.FASTA.FastaSeq(),
                             RR As SequenceModel.FASTA.FastaSeq(),
                             interactions As String(),
                             name As String,
                             ByRef alignments As KeyValuePair()) As Signature()

        Dim RRFa = RR.ToDictionary(Function(x) x.Title.Split.First.Split(":"c).Last)
        Dim HiskFa = HisK.ToDictionary(Function(x) x.Title.Split.First.Split(":"c).Last)
        Dim intsList As KeyValuePair(Of String, String)() = (From x As String In interactions
                                                             Let id As String = x.Split(":"c).Last
                                                             Let tokens = id.Split("-"c)
                                                             Select New KeyValuePair(Of String, String)(tokens(0), tokens(1))).ToArray
        Dim contracts = intsList.Where(Function(x) HiskFa.ContainsKey(x.Key) AndAlso RRFa.ContainsKey(x.Value)).Select(
            Function(x) New SequenceModel.FASTA.FastaSeq With {
                .SequenceData = HiskFa(x.Key).SequenceData & RRFa(x.Value).SequenceData,
                .Headers = {$"{x.Key}-{x.Value}"}}).ToArray
        alignments = contracts.Select(
            Function(x) New KeyValuePair With {
                .Key = x.Title,
                .Value = x.SequenceData}).ToArray
        Dim SRChain As SRChain() = SR.FromAlign(contracts, 0.95)
        Dim sig = SRChain.Select(Function(x) Signature.CreateObject(x.lstSR, name))
        Return sig
    End Function

    <ExportAPI("--domain.Interactions", Usage:="--domain.Interactions /pfam <pfam-string.csv> /swissTCS <swissTCS.DIR>")>
    Public Function DomainInteractions(args As CommandLine) As Integer
        Dim inPfam = (args <= "/pfam").LoadCsv(Of Pfam.PfamString.PfamString)
        Dim LQuery = (From x As Pfam.PfamString.PfamString
                      In inPfam.AsParallel
                      Let order As String = (From id As ProteinModel.DomainObject
                                             In x.GetDomainData(False)
                                             Select id.Name).ToArray.JoinBy("+")
                      Select order, x
                      Group By order Into Group) _
                         .ToDictionary(Function(x) x.order,
                                       elementSelector:=Function(x) x.Group.Select(Function(xx) xx.x).ToArray)
        Dim Categories = LQuery.Select(Function(x) Family.FileSystem.Family.CreateObject(x.Key, x.Value))
        Dim Database As New Family.FileSystem.FamilyPfam With {
            .Family = Categories,
            .Title = "SwissTCS"
        }
        Dim DIRS = FileIO.FileSystem.GetDirectories(args("/swissTCS"), FileIO.SearchOption.SearchTopLevelOnly).ToArray
        Dim HisK = __loadFa(DIRS, Function(dir) dir & "/HisK.fasta")
        Dim RRPro = __loadFa(DIRS, Function(dir) dir & "/RR.fasta")
        Dim out As String = args("/swissTCS") & "/Pfam/"
        Dim clustal = ClustalOrg.Clustal.CreateSession
        Dim list As New List(Of Category)

        For Each fm In Categories
            Dim ct = (From rec As Family.FileSystem.PfamString
                      In fm.PfamString
                      Let id As String = rec.LocusTag.Split(":"c).Last
                      Let Tokens As String() = id.Split("-"c)
                      Select hk = HisK(Tokens(0)),
                          Rg = RRPro(Tokens(1))).ToArray
            Dim hkk = New SequenceModel.FASTA.FastaFile((From x In ct Select x.hk, x.hk.Headers.First.Split.First Group By First Into Group).Select(Function(x) x.Group.First.hk))
            Dim rrg = New SequenceModel.FASTA.FastaFile((From x In ct Select x.Rg, x.Rg.Headers.First.Split.First Group By First Into Group).Select(Function(x) x.Group.First.Rg))
            Dim interactions As String() = fm.PfamString.Select(Function(x) x.LocusTag)
            Dim HisKFa As String = out & $"/fa/{fm.Family.NormalizePathString}-Hisk.fasta"
            Dim RRFa As String = out & $"/fa/{fm.Family.NormalizePathString}-RR.fasta"

            Call hkk.Save(HisKFa)
            Call rrg.Save(RRFa)

            Dim alignments As KeyValuePair() = Nothing
            Dim rp = AlignLDM(HisKFa,
                              RRFa,
                              interactions,
                              clustal,
                              fm.Family,
                              alignments)
            Call list.Add(Category.CreateObject(fm, rp, alignments))
        Next

        Return list.ToArray.GetXml.SaveTo((args <= "/pfam").TrimSuffix & ".PfamMPAln.xml").CLICode
    End Function

    Public Class Category : Inherits Family.FileSystem.Family
        Public Property Signature As Signature()
        Public Property Alignments As KeyValuePair()

        Public Overloads Shared Function CreateObject(Family As Family.FileSystem.Family,
                                                      sig As Signature(),
                                                      alignments As KeyValuePair()) As Category
            Return New Category With {
                .Alignments = alignments,
                .Domains = Family.Domains,
                .Family = Family.Family,
                .PfamString = Family.PfamString,
                .Signature = sig
            }
        End Function

        Public Function GetSignatureFasta() As SequenceModel.FASTA.FastaFile
            Dim lstFa = Signature.Select(Function(x) New FastaSeq(x))
            Dim fa As New FastaFile(lstFa)
            Return fa
        End Function

        Public Function Folk(cutoff As Double, level As Integer) As Category
            Dim aln As FastaSeq() = Alignments.Select(
                Function(x) New FastaSeq With {
                    .SequenceData = x.Value,
                    .Headers = {x.Key}}).ToArray
            Return FromAlign(aln, cutoff, name:=Family & "-" & CStr(cutoff), level:=level)
        End Function

        Public Shared Function FromAlign(aln As Generic.IEnumerable(Of FastaSeq),
                                         Optional cutoff As Double = 1,
                                         Optional level As Integer = 10,
                                         Optional name As String = "") As Category
            Dim chains = SR.FromAlign(aln, cutoff, level)
            Dim setValue = New SetValue(Of Interactions.Signature) <=
                NameOf(Interactions.Signature.Level)
            Dim signatures = chains.Select(Function(x, index) setValue(Interactions.Signature.CreateObject(x), (level - index) / level)).ToArray
            Dim ppiCategory As New Interactions.Category With {
                .Signature = signatures,
                .Alignments = aln.Select(Function(x) New KeyValuePair With {
                .Key = x.Title,
                .Value = x.SequenceData}),
                .PfamString = signatures.Select(Function(x) Analysis.ProteinTools.Family.FileSystem.PfamString.CreateObject(x.PfamString)),
                .Family = name
            }
            Return ppiCategory
        End Function
    End Class

    Private Function __loadFa(DIRS As String(), getFa As Func(Of String, String)) As Dictionary(Of String, FastaSeq)
        Dim prot = (From x In (From DIR As String
                               In DIRS
                               Let fa = FastaFile.Read(getFa(DIR), False)
                               Where Not fa.IsNullOrEmpty
                               Select fa.Select(Function(x) New With {
                                   .Id = x.Headers.First.Split.First.Split(":"c).Last,
                                   .fa = x})).ToArray.Unlist
                    Select x
                    Group x By x.Id Into Group) _
                      .ToDictionary(Function(x) x.Id, elementSelector:=Function(x) x.Group.First.fa)
        Return prot
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="HisK"></param>
    ''' <param name="RR"></param>
    ''' <returns></returns>
    Public Function AlignLDM(HisK As String,
                             RR As String,
                             interactions As String(),
                             Clustal As ClustalOrg.Clustal,
                             name As String,
                             ByRef alignments As KeyValuePair()) As Signature()
        Dim hisKAlignTask As AsyncHandle(Of FastaFile) = Clustal.AlignmentTask(HisK)
        Dim RRAlign = Clustal.MultipleAlignment(RR)
        Dim HiskAlign As FastaFile = hisKAlignTask.GetValue

        If RRAlign.IsNullOrEmpty Then
            RRAlign = New SequenceModel.FASTA.FastaFile
        End If
        If HiskAlign.IsNullOrEmpty Then
            HiskAlign = New SequenceModel.FASTA.FastaFile
        End If

        Dim RRFa = RRAlign.ToDictionary(Function(x) x.Title.Split.First.Split(":"c).Last)
        Dim HiskFa = HiskAlign.ToDictionary(Function(x) x.Title.Split.First.Split(":"c).Last)
        Dim intsList As KeyValuePair(Of String, String)() = (From x As String In interactions
                                                             Let id As String = x.Split(":"c).Last
                                                             Let tokens = id.Split("-"c)
                                                             Select New KeyValuePair(Of String, String)(tokens(0), tokens(1))).ToArray
        Dim contracts = intsList.Where(Function(x) HiskFa.ContainsKey(x.Key) AndAlso RRFa.ContainsKey(x.Value)).Select(
            Function(x) New SequenceModel.FASTA.FastaSeq With {
                .SequenceData = HiskFa(x.Key).SequenceData & RRFa(x.Value).SequenceData,
                .Headers = {$"{x.Key}-{x.Value}"}}).ToArray

        alignments = contracts.Select(
            Function(x) New KeyValuePair With {
                .Key = x.Title,
                .Value = x.SequenceData}).ToArray
        Dim SRChain As SRChain() = SR.FromAlign(contracts, 0.9)
        Dim sig = SRChain.Select(Function(x) Signature.CreateObject(x.lstSR, name))
        Return sig
    End Function

    <ExportAPI("--align.LDM", Usage:="--align.LDM /in <source.fasta>")>
    Public Function GenerateModel(args As CommandLine) As Integer
        Dim input$ = args("/in")
        Dim clustal = ClustalOrg.Clustal.CreateSession
        Dim align = clustal.MultipleAlignment(input)
        Dim SRChain As SRChain() = SR.FromAlign(align, 0.85)
        Dim Name As String = BaseName(args("/in"))
        Dim file As String = input$.TrimSuffix & ".Pfam-String.csv"
        ' Call SRChain.SaveTo(args("/in").TrimFileExt & ".Blocks.csv")
        Call SRChain.Select(Function(x) x.ToPfamString()).SaveTo(file)
        Return 0
    End Function
End Module
