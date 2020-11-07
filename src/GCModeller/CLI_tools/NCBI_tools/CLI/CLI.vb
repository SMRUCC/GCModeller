#Region "Microsoft.VisualBasic::dbc51f5d232b5a9b25c19a2590711841, CLI_tools\NCBI_tools\CLI\CLI.vb"

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
    '     Function: accidMatch, AssignFastaTaxonomy, AssignTaxonomy, AssignTaxonomy2, AssignTaxonomyFromRef
    '               Associates, AssociateTaxonomy, Build_gi2taxi, ExportGI, giMatch
    '               giMatchs, NtTaxonomy
    '     Class Taxono
    ' 
    '         Properties: Tag, Values
    ' 
    '         Function: Load, Parser_gi, Save, ToString
    ' 
    '     Class TaxiSummary
    ' 
    '         Properties: gi, Name, sequence, title
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ToString
    ' 
    '     Class ITaxon
    ' 
    '         Properties: [class], family, genus, order, phylum
    '                     species, superkingdom, taxid, Taxonomy
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService.SharedORM
Imports Microsoft.VisualBasic.CommandLine.ManView
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming.Levenshtein
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Parallel.Linq
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy
Imports SMRUCC.genomics.SequenceModel.FASTA

<Package("NCBI_tools.CLI",
                  Category:=APICategories.CLI_MAN,
                  Description:="Tools collection for handling NCBI data, includes: nt/nr database, NCBI taxonomy analysis, OTU taxonomy analysis, genbank database, and sequence query tools.",
                  Publisher:="xie.guigang@gcmodeller.org")>
<GroupingDefine(CLIGrouping.GITools, Description:=CLIGrouping.GIWasObsoleted)>
<ExceptionHelp(Documentation:="http://docs.gcmodeller.org", Debugging:="https://github.com/SMRUCC/GCModeller/wiki", EMailLink:="xie.guigang@gcmodeller.org")>
<CLI> Public Module CLI

    <ExportAPI("/Build_gi2taxi")>
    <Usage("/Build_gi2taxi /in <gi2taxi.dmp> [/out <out.dat>]")>
    <Group(CLIGrouping.GITools)>
    Public Function Build_gi2taxi(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".bin")
        Return Taxonomy.Archive([in], out)
    End Function

    <ExportAPI("/Export.GI")>
    <Usage("/Export.GI /in <ncbi:nt.fasta> [/out <out.csv>]")>
    <Group(CLIGrouping.GITools)>
    Public Function ExportGI(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".gi.Csv")
        Dim nt As New StreamIterator([in])

        Using writer As New WriteStream(Of TaxiValue)(out)

            writer.BaseStream.AutoFlush = True
            writer.BaseStream.NewLine = vbLf

            For Each fa As FastaSeq In nt.ReadStream
                Dim title As String = fa.Title
                Dim gi As String = title.Match("gi\|\d+", RegexICSng).Split("|"c).Last  ' 由于bowetie程序建库的时候只取最开始的值，所以在这里只需要第一个match就行了
                Dim result As New TaxiValue With {
                    .Name = gi,
                    .Title = title
                }
                Call writer.Flush(result)
            Next
        End Using

        Return 0
    End Function

    <ExportAPI("/Associate.Taxonomy")>
    <Usage("/Associate.Taxonomy /in <in.DIR> /tax <ncbi_taxonomy:names,nodes> /gi2taxi <gi2taxi.bin> [/gi <nt.gi.csv> /out <out.DIR>]")>
    <Group(CLIGrouping.GITools)>
    Public Function AssociateTaxonomy(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim tax As String = args("/tax")
        Dim EXPORT As String = args.GetValue("/out", [in].TrimDIR & ".NCBI.Taxonomy/")
        Dim ref As String = args("/gi")
        Dim gi2taxi As String = args("/gi2taxi")
        Dim taxiHash As BucketDictionary(Of Integer, Integer) = Taxonomy.Hash_gi2Taxi(gi2taxi)
        Dim taxTree As New NcbiTaxonomyTree(tax)
        Dim hash As Dictionary(Of String, String) =
            If(ref.FileExists,
            TaxiValue.BuildDictionary(ref.LoadCsv(Of TaxiValue)),
            New Dictionary(Of String, String))

        For Each file As String In ls - l - r - wildcards("*.Csv") <= [in]
            Dim data As IEnumerable(Of TaxiValue) = file.LoadCsv(Of TaxiValue)
            Dim out As String = EXPORT & "/" & file.BaseName & ".Csv"
            Dim LQuery = (From x As TaxiValue
                          In data
                          Let gi As Integer = CInt(Val(Regex.Split(Regex.Match(x.Name, "gi(_|\|)\d+", RegexICSng).Value, "_|\|").Last))
                          Select gi,
                              x).ToArray

            For Each x In LQuery
                If hash.ContainsKey(x.gi) Then
                    x.x.Title = hash(x.gi)
                End If
            Next

            For Each x In LQuery
                If taxiHash.ContainsKey(x.gi) Then
                    x.x.taxid = taxiHash(x.gi)
                    x.x.TaxonomyTree = TaxonomyNode.Taxonomy(taxTree.GetAscendantsWithRanksAndNames({CInt(x.x.taxid)}, True).Values.First)
                Else
                    Call x.gi.ToString.Warning
                End If
            Next

            data = LQuery.Select(Function(x) x.x).ToArray

            Call data.SaveTo(out)
        Next

        Return 0
    End Function

    <ExportAPI("/Nt.Taxonomy")>
    <Usage("/Nt.Taxonomy /in <nt.fasta> /gi2taxi <gi2taxi.bin> /tax <ncbi_taxonomy:names,nodes> [/out <out.fasta>]")>
    <Group(CLIGrouping.GITools)>
    Public Function NtTaxonomy(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim gi2taxi As String = args("/gi2taxi")
        Dim tax As String = args("/tax")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".Taxonomy.fasta")
        Dim taxiHash As BucketDictionary(Of Integer, Integer) = Taxonomy.Hash_gi2Taxi(gi2taxi)
        Dim taxTree As New NcbiTaxonomyTree(tax)
        Dim notFoundTax As String = out.TrimSuffix & ".notFound.txt"

        Using notFound As StreamWriter = (out.TrimSuffix & ".NotFound.fasta").OpenWriter(Encodings.ASCII),
            writer As StreamWriter = out.OpenWriter(Encodings.ASCII),
            table As New WriteStream(Of TaxiSummary)(out.TrimSuffix & ".Csv"),
            taxNotFoun = notFoundTax.OpenWriter(Encodings.ASCII)

            For Each fa As FastaSeq In New StreamIterator([in]).ReadStream
                Dim gi As Integer = CInt(Val(Regex.Match(fa.Title, "gi\|\d+", RegexICSng).Value.Split("|"c).Last))

                If gi > 0 AndAlso taxiHash.ContainsKey(gi) Then
                    Dim taxi As Integer = taxiHash(gi)
                    Dim tree = taxTree.GetAscendantsWithRanksAndNames({taxi}, True)
                    Dim taxonomy As String = TaxonomyNode.Taxonomy(tree.First.Value)
                    Dim hash = TaxonomyNode.RankTable(tree.First.Value)

#If DEBUG Then
                    VBDebugger.Mute = True
#End If

                    Dim x As New TaxiSummary With {
                        .Name = fa.Title.Split.First,
                        .gi = gi,
                        .taxid = taxi,
                        .title = Mid(fa.Title, 1, 128),
                        .sequence = fa.SequenceData,
                        .class = hash.TryGetValue(NcbiTaxonomyTree.class),
                        .family = hash.TryGetValue(NcbiTaxonomyTree.family),
                        .genus = hash.TryGetValue(NcbiTaxonomyTree.genus),
                        .order = hash.TryGetValue(NcbiTaxonomyTree.order),
                        .phylum = hash.TryGetValue(NcbiTaxonomyTree.phylum),
                        .species = hash.TryGetValue(NcbiTaxonomyTree.species),
                        .superkingdom = hash.TryGetValue(NcbiTaxonomyTree.superkingdom)
                    }

                    x.Taxonomy = $"k__{x.superkingdom};p__{x.phylum};c__{x.class};o__{x.order};f__{x.family};g__{x.genus};s__{x.species}"

#If DEBUG Then
                    VBDebugger.Mute = False
#End If
                    If hash.Count = 0 Then
                        Call taxNotFoun.WriteLine({CStr(gi), CStr(taxi), fa.Title}.JoinBy(vbTab))
                    End If

                    Call table.Flush(x)

                    Dim title As String = $"gi_{gi} {taxi} {x.Taxonomy}"
                    fa = New FastaSeq({title}, fa.SequenceData)
                    Call writer.WriteLine(fa.GenerateDocument(120))
                Else
                    Call $"gi {gi} not found taxid...".__DEBUG_ECHO
                    Call notFound.WriteLine(fa.GenerateDocument(-1))
                End If
            Next

            Return 0
        End Using
    End Function

    <ExportAPI("/Assign.Taxonomy")>
    <Usage("/Assign.Taxonomy /in <in.DIR> /gi <regexp> /index <fieldName> /tax <NCBI nodes/names.dmp> /gi2taxi <gi2taxi.txt/bin> [/out <out.DIR>]")>
    <Group(CLIGrouping.GITools)>
    Public Function AssignTaxonomy(args As CommandLine) As Integer
        Dim [in] As String = args.GetFullDIRPath("/in")
        Dim regexp As String = args("/gi")
        Dim index As String = args("/index")
        Dim tax As String = args("/tax")
        Dim gi2taxi As String = args("/gi2taxi")
        Dim EXPORT As String = args.GetValue("/out", [in].TrimDIR & ".Taxonomy/")
        Dim tree As New NcbiTaxonomyTree(tax)
        Dim giTaxid As BucketDictionary(Of Integer, Integer) = Taxonomy.AcquireAuto(gi2taxi)
        Dim getGI = Taxono.Parser_gi(regexp)

        For Each file As String In ls - l - r - wildcards("*.Csv") <= [in]
            Dim data = Taxono.Load(file, index)
            Dim out As String = EXPORT & "/" & file.BaseName & ".Csv"

            For Each x In data
                Dim gi As Integer = getGI(x)

                If giTaxid.ContainsKey(gi) Then
                    Dim taxid As Integer = giTaxid(gi)
                    x.taxid = taxid
                    Dim nodes = tree.GetAscendantsWithRanksAndNames({taxid}, True)
                    Dim hash = TaxonomyNode.RankTable(nodes.First.Value)

                    With x
                        .class = hash.TryGetValue(NcbiTaxonomyTree.class)
                        .family = hash.TryGetValue(NcbiTaxonomyTree.family)
                        .genus = hash.TryGetValue(NcbiTaxonomyTree.genus)
                        .order = hash.TryGetValue(NcbiTaxonomyTree.order)
                        .phylum = hash.TryGetValue(NcbiTaxonomyTree.phylum)
                        .species = hash.TryGetValue(NcbiTaxonomyTree.species)
                        .superkingdom = hash.TryGetValue(NcbiTaxonomyTree.superkingdom)
                    End With

                    x.Taxonomy = $"k__{x.superkingdom};p__{x.phylum};c__{x.class};o__{x.order};f__{x.family};g__{x.genus};s__{x.species}"
                End If
            Next

            Call Taxono.Save(out, data, index)
        Next

        Return 0
    End Function

    <ExportAPI("/Assign.Taxonomy.SSU")>
    <Usage("/Assign.Taxonomy.SSU /in <in.DIR> /index <fieldName> /ref <SSU-ref.fasta> [/out <out.DIR>]")>
    Public Function AssignTaxonomy2(args As CommandLine) As Integer
        Dim [in] As String = args.GetFullDIRPath("/in")
        Dim index As String = args("/index")
        Dim tax As String = args("/ref")
        Dim EXPORT As String = args.GetValue("/out", [in].TrimDIR & ".Taxonomy/")
        Dim taxHash = (From x As FastaSeq
                       In New FastaFile(tax)
                       Select sid = x.Title.Split.First,
                           x.Title
                       Group By sid Into Group) _
                               .ToDictionary(Function(x) x.sid,
                                             Function(x) x.Group.First.Title.Replace(x.sid, "").Trim)

        For Each file As String In ls - l - r - wildcards("*.Csv") <= [in]
            Dim data = Taxono.Load(file, index)
            Dim out As String = EXPORT & "/" & file.BaseName & ".Csv"

            For Each x As Taxono In data
                Dim key As String = x.Tag.Split.First

                If taxHash.ContainsKey(key) Then
                    Dim tokens As String() = taxHash(key).Split(";"c)
                    Dim hash As New Dictionary(Of String, String) From {
                        {"species", tokens(6)},
                        {"genus", tokens(5)},
                        {"family", tokens(4)},
                        {"order", tokens(3)},
                        {"class", tokens(2)},
                        {"phylum", tokens(1)},
                        {"superkingdom", tokens(0)}
                    }

                    With x
                        .class = hash.TryGetValue(NcbiTaxonomyTree.class)
                        .family = hash.TryGetValue(NcbiTaxonomyTree.family)
                        .genus = hash.TryGetValue(NcbiTaxonomyTree.genus)
                        .order = hash.TryGetValue(NcbiTaxonomyTree.order)
                        .phylum = hash.TryGetValue(NcbiTaxonomyTree.phylum)
                        .species = hash.TryGetValue(NcbiTaxonomyTree.species)
                        .superkingdom = hash.TryGetValue(NcbiTaxonomyTree.superkingdom)
                    End With

                    x.Taxonomy = taxHash(key)
                End If
            Next

            Call Taxono.Save(out, data, index)
        Next

        Return 0
    End Function

    ''' <summary>
    ''' 假若参考序列文件标题之中已经存在了物种分类信息，则可以使用这个函数直接解析赋值，而不再需要加载NCBI的库文件来获取物种信息
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Assign.Taxonomy.From.Ref")>
    <Usage("/Assign.Taxonomy.From.Ref /in <in.DIR> /ref <nt.taxonomy.fasta> [/index <Name> /non-BIOM /out <out.DIR>]")>
    Public Function AssignTaxonomyFromRef(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim ref As String = args("/ref")
        Dim index As String = args.GetValue("/index", NameOf(NamedValue(Of Object).Name))
        Dim outDIR As String = args.GetValue("/out", [in].TrimDIR & ".Taxonomy/")
        Dim tax As New Dictionary(Of String, String)

        For Each fa In New StreamIterator(ref).ReadStream
            Dim title As String = fa.Title
            Dim uid As String = fa.Headers.First.Split.First
            tax(uid) = title
        Next

        For Each file As String In ls - l - r - wildcards("*.csv") <= [in]
            Dim data = Taxono.Load(file, index)
            Dim out As String = outDIR & "/" & file.BaseName & ".Csv"

            For Each x In data
                If tax.ContainsKey(x.Tag) Then
                    x.Taxonomy = tax(x.Tag)
                End If
            Next

            Call Taxono.Save(out, data, index)
        Next

        Return 0
    End Function

    <ExportAPI("/Associates.Brief")>
    <Usage("/Associates.Brief /in <in.DIR> /ls <ls.txt> [/index <Name> /out <out.tsv>]")>
    Public Function Associates(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim lsWords$() = (args <= "/ls") _
            .ReadAllLines _
            .Where(Function(s) Not String.IsNullOrEmpty(Trim(s))) _
            .Select(Function(s) s.Trim.ToLower) _
            .ToArray
        Dim outDIR As String = args.GetValue("/out", [in].TrimDIR & ".associates.tsv")
        Dim index As String =
            args.GetValue("/index", NameOf(NamedValue(Of Object).Name))

        Using output As StreamWriter = outDIR.OpenWriter(Encodings.ASCII)
            For Each file As String In ls - l - r - wildcards("*.csv") <= [in]
                Dim data = Taxono.Load(file, index)
                Dim out As String = outDIR & "/" & file.BaseName & ".Csv"

                For Each x In data
                    If x.Taxonomy Is Nothing Then
                        Continue For
                    End If
                    For Each line As String In lsWords
                        Dim words = line.Split ' 小写的
                        Dim tax = x.Taxonomy.ToLower.Split

                        For Each xx In tax
                            For Each y In words
                                Dim compare = LevenshteinDistance.ComputeDistance(xx, y)
                                If Not compare Is Nothing AndAlso compare.Score >= 0.6 Then
                                    Call output.WriteLine(String.Join(vbTab, xx, x.Taxonomy, line, x.Values.GetJson))
                                End If
                            Next
                        Next
                    Next
                Next
            Next

            output.Flush()
            output.Close()
        End Using

        Return 0
    End Function

    Public Class Taxono : Inherits ITaxon

        Public Property Tag As String
        <Meta>
        Public Property Values As Dictionary(Of String, String)

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function Load(path As String, index As String) As IEnumerable(Of Taxono)
            Return path.LoadCsv(Of Taxono)(maps:={{index, NameOf(Taxono.Tag)}})
        End Function

        Public Shared Function Save(path As String, result As IEnumerable(Of Taxono), index As String) As Boolean
            Dim maps As New Dictionary(Of String, String) From {{NameOf(Taxono.Tag), index}}
            Return result.SaveTo(path, maps:=maps)
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Shared Function Parser_gi(regexp As String) As Func(Of Taxono, Integer)
            Static rg As New Regex(regexp, RegexICSng)
            Return Function(x)
                       Return CInt(Val(Regex.Match(rg.Match(x.Tag).Value, "\d+").Value))
                   End Function
        End Function
    End Class

    Public Class TaxiSummary : Inherits ITaxon

        Public Property Name As String
        Public Property gi As String

        Public Property title As String
        Public Property sequence As String

        Sub New()
        End Sub

        Sub New(tree As IEnumerable(Of TaxonomyNode))
            MyBase.New(tree)
        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class ITaxon

        Public Property taxid As String

        Public Property species As String
        Public Property genus As String
        Public Property family As String
        Public Property order As String
        Public Property [class] As String
        Public Property phylum As String
        Public Property superkingdom As String
        Public Property Taxonomy As String

        Sub New()
        End Sub

        Sub New(tree As IEnumerable(Of TaxonomyNode))
            Dim data = TaxonomyNode.RankTable(tree)

            With Me
                .class = data.TryGetValue(NcbiTaxonomyTree.class)
                .family = data.TryGetValue(NcbiTaxonomyTree.family)
                .genus = data.TryGetValue(NcbiTaxonomyTree.genus)
                .order = data.TryGetValue(NcbiTaxonomyTree.order)
                .phylum = data.TryGetValue(NcbiTaxonomyTree.phylum)
                .species = data.TryGetValue(NcbiTaxonomyTree.species)
                .superkingdom = data.TryGetValue(NcbiTaxonomyTree.superkingdom)
            End With
        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

#Region "subsets"

    ' 因为gi2taxid或者accession2taxid这两个库都非常大，不可以一次性的加载到内存之中
    ' 所以一般首先会需要这些函数进行部分的subset以减小数据集合的大小

    <ExportAPI("/gi.Match")>
    <Usage("/gi.Match /in <nt.parts.fasta/list.txt> /gi2taxid <gi2taxid.dmp> [/out <gi_match.txt>]")>
    <Group(CLIGrouping.GITools)>
    Public Function giMatch(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim gi2taxid As String = args("/gi2taxid")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".gi_match.txt")
        Dim gis As List(Of String)
        Dim gi As String

        If FastaFile.IsValidFastaFile([in]) Then
            gis = New List(Of String)

            For Each seq As FastaSeq In New StreamIterator([in]).ReadStream
                gis += Regex.Match(seq.Title, "gi\|\d+", RegexICSng).Value.Split("|"c).Last
            Next
        Else
            gis = New List(Of String)([in].ReadAllLines)
        End If

        Dim hash = (From id As String
                    In gis
                    Select id
                    Group id By id Into Group).ToDictionary(Function(x) x.id, Function(x) "")

        Using match As StreamWriter = out.OpenWriter(Encodings.ASCII)
            For Each line As String In gi2taxid.IterateAllLines
                gi = line.Split(ASCII.TAB).First

                If hash.ContainsKey(gi) Then
                    Call match.WriteLine(line)
                    Call hash.Remove(gi)

                    If hash.Count = 0 Then  ' 假若所有的gi号都已经匹配完毕了，则跳出循环，节省计算时间
                        Exit For
                    End If
                End If
            Next
        End Using

        Return 0
    End Function

    ''' <summary>
    ''' 进行accessionID物种信息数据库的subset操作
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/accid2taxid.Match")>
    <Usage("/accid2taxid.Match /in <nt.parts.fasta/list.txt> /acc2taxid <acc2taxid.dmp/DIR> [/gb_priority /append.src /accid_grep <default=-> /out <acc2taxid_match.txt>]")>
    <Description("Creates the subset of the ultra-large accession to ncbi taxonomy id database.")>
    <Group(CLIGrouping.TaxonomyTools)>
    <ArgumentAttribute("/accid_grep", True, CLITypes.String, PipelineTypes.undefined, AcceptTypes:={GetType(String)},
              Description:="When the fasta title or the text line in the list is not an NCBI accession id, 
              then you needs this script for accession id grep operation.")>
    Public Function accidMatch(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim acc2taxid As String = args("/acc2taxid")
        Dim out As String = args("/out") Or $"{[in].TrimSuffix}.accession2taxid.txt"
        Dim acclist As New List(Of NamedValue(Of String))
        Dim grep As TextGrepScriptEngine = TextGrepScriptEngine.Compile(args <= "/accid_grep")
        Dim accid_grep As TextGrepMethod = grep.PipelinePointer
        Dim appendSrc As Boolean = args("/append.src")

        Call grep.Explains.JoinBy(vbCrLf & "--> ").__INFO_ECHO

        If appendSrc Then
            Call "The input info will append to each rows".__DEBUG_ECHO
        End If

        If FastaFile.IsValidFastaFile([in]) Then
            Dim title$

            Call "Load accession id from fasta files".__INFO_ECHO

            For Each seq As FastaSeq In New StreamIterator([in]).ReadStream
                title = seq.Title
                acclist += New NamedValue(Of String) With {
                    .Name = accid_grep(title),
                    .Value = title
                }
            Next
        Else
            acclist = [in] _
                .ReadAllLines _
                .Select(Function(line)
                            Return New NamedValue(Of String)(accid_grep(line), line)
                        End Function) _
                .AsList
        End If

        Dim gb_priority As Boolean = args("/gb_priority")
        Dim result = Accession2Taxid.Matchs(
            acclist.Keys.Distinct,
            acc2taxid,
            debug:=True,
            gb_priority:=gb_priority
        )

        If appendSrc Then
            Dim listTable As Dictionary(Of String, String) = acclist _
                .GroupBy(Function(acc) acc.Name) _
                .ToDictionary(Function(acc)
                                  Return acc.Key.Split("."c).First
                              End Function,
                              Function(input) input.First.Value)
            ' 会将原始的输入信息追加到对应的行的最末尾
            result = result _
                .Skip(1) _
                .Select(Function(row)
                            Return row & vbTab & listTable(row.Split(ASCII.TAB).First)
                        End Function)
            result = {Accession2Taxid.Acc2Taxid_Header}.Join(result)
        End If

        Return result.SaveTo(out, Encoding.ASCII).CLICode
    End Function

    ''' <summary>
    ''' 批量的利用fasta标题之中的编号进行gi2taxid的subset操作
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/gi.Matchs")>
    <Usage("/gi.Matchs /in <nt.parts.fasta.DIR> /gi2taxid <gi2taxid.dmp> [/out <gi_match.txt.DIR> /num_threads <-1>]")>
    <Group(CLIGrouping.GITools)>
    Public Function giMatchs(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim gi2taxid As String = args("/gi2taxid")
        Dim out As String = args("/out")
        Dim CLI As Func(Of String, String)
        Dim n As Integer = args.GetValue("/num_threads", -1)

        If String.IsNullOrEmpty(out) Then
            CLI = Function(file) $"{GetType(CLI).API(NameOf(giMatch))} /in {file.CLIPath} /gi2taxid {gi2taxid.CLIPath}"
        Else
            CLI = Function(file) $"{GetType(CLI).API(NameOf(giMatch))} /in {file.CLIPath} /gi2taxid {gi2taxid.CLIPath} /out {(out & "/" & file.BaseName & ".gi_match.txt").CLIPath}"
        End If

        Dim tasks$() =
            (ls - l - r - wildcards("*.fasta") <= [in]).Select(CLI).ToArray

        Return App.SelfFolks(tasks, LQuerySchedule.AutoConfig(n))
    End Function
#End Region

    ''' <summary>
    ''' 这个函数会执行以下功能：
    ''' 
    ''' 1. 为fasta序列的标题添加taxonomy信息
    ''' 2. 产生一个物种统计信息表格
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("/assign.fasta.taxonomy")>
    <Usage("/assign.fasta.taxonomy /in <database.fasta> /accession2taxid <accession2taxid.txt> /taxonomy <names.dmp/nodes.dmp> [/accid_grep <default=-> /append <data.csv> /summary.tsv /out <out.directory>]")>
    <ArgumentAttribute("/accession2taxid", False, CLITypes.File, PipelineTypes.undefined, AcceptTypes:={GetType(String())},
              Description:="This mapping data file is usually a subset of the accession2taxid file, and comes from the ``/accid2taxid.Match`` command.")>
    <ArgumentAttribute("/append", True, CLITypes.File, PipelineTypes.undefined, AcceptTypes:={GetType(EntityObject)},
              Description:="If this parameter was presented, then additional data will append to the fasta title and the csv summary file. 
              This file should have a column named ``ID`` correspond to the ``accession_id``, 
              or a column named ``Species`` correspond to the ``species`` from NCBI taxonomy.")>
    <ArgumentAttribute("/summary.tsv", True, CLITypes.Boolean, Description:="The output summary table file saved in tsv format.")>
    Public Function AssignFastaTaxonomy(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim acc2taxid = Accession2Taxid.ReadFile(args <= "/accession2taxid").ToDictionary.FlatTable
        Dim taxonomyTree = New NcbiTaxonomyTree(args <= "/taxonomy")
        Dim out$ = [in].TrimSuffix
        Dim headers As List(Of String) = {"title", "taxid"} _
            .JoinIterates(NcbiTaxonomyTree.stdranks.Objects.Reverse) _
            .AsList
        Dim grep As TextGrepScriptEngine = TextGrepScriptEngine.Compile(args <= "/accid_grep")
        Dim accid_grep As TextGrepMethod = grep.PipelinePointer
        Dim append$ = args <= "/append"
        Dim appendByID As Boolean
        Dim indexAppendData As Dictionary(Of String, EntityObject) = Nothing

        Call grep.Explains.JoinBy(vbCrLf & "--> ").__INFO_ECHO

        If append.FileExists Then
            Dim map$ = Nothing

            If FileFormat.ContainsIDField(append) Then
                appendByID = True
                map = NameOf(EntityObject.ID)

                Call "Additional data will index by [accession ID]".__DEBUG_ECHO
            Else
                appendByID = False
                map = "Species"

                Call "Additional data will index by [Species] field".__DEBUG_ECHO
            End If

            indexAppendData = EntityObject _
                .LoadDataSet(append, uidMap:=map) _
                .GroupBy(Function(d) d.ID) _
                .ToDictionary(Function(g) g.Key,
                              Function(g)
                                  Return g.First
                              End Function)

            headers = headers + indexAppendData _
                .First _
                .Value _
                .EnumerateKeys
        Else
            indexAppendData = New Dictionary(Of String, EntityObject)
        End If

        Using fastaWriter As StreamWriter = $"{out}/taxonomy.fasta".OpenWriter(Encodings.ASCII),
              summary As New WriteStream(Of EntityObject)(
                  path:=$"{out}/summary.csv" Or $"{out}/summary.txt".When(args("/summary.tsv")),
                  metaKeys:=headers,
                  tsv:=args("/summary.tsv")
              )

            For Each seq As FastaSeq In New StreamIterator([in]).ReadStream
                Dim title = seq.Title
                Dim accession$ = TrimAccessionVersion(accid_grep(title))
                Dim taxid% = acc2taxid.TryGetValue(accession, [default]:=-1)

                If taxid < 0 Then
                    Call $"[{title}] taxonomy not found!".Warning

                    Call fastaWriter.WriteLine(seq.GenerateDocument(-1))
                    Call summary.Flush(New EntityObject With {
                             .ID = accession,
                             .Properties = New Dictionary(Of String, String) From {
                                 {"title", title},
                                 {"taxid", taxid},
                                 {NcbiTaxonomyTree.superkingdom, ""},
                                 {NcbiTaxonomyTree.phylum, ""},
                                 {NcbiTaxonomyTree.class, ""},
                                 {NcbiTaxonomyTree.order, ""},
                                 {NcbiTaxonomyTree.family, ""},
                                 {NcbiTaxonomyTree.genus, ""},
                                 {NcbiTaxonomyTree.species, ""}
                             }
                         })

                    Continue For
                End If

                Dim nodes = taxonomyTree.GetAscendantsWithRanksAndNames({taxid}, True)
                Dim table = TaxonomyNode.RankTable(nodes.First.Value)
                Dim taxonomy$
                Dim lineage$()
                Dim additionals As EntityObject = Nothing

                With New SMRUCC.genomics.Metagenomics.Taxonomy
                    .class = table.TryGetValue(NcbiTaxonomyTree.class)
                    .family = table.TryGetValue(NcbiTaxonomyTree.family)
                    .genus = table.TryGetValue(NcbiTaxonomyTree.genus)
                    .order = table.TryGetValue(NcbiTaxonomyTree.order)
                    .phylum = table.TryGetValue(NcbiTaxonomyTree.phylum)
                    .species = table.TryGetValue(NcbiTaxonomyTree.species)
                    .kingdom = table.TryGetValue(NcbiTaxonomyTree.superkingdom)

                    taxonomy = $"k__{ .kingdom};p__{ .phylum};c__{ .class};o__{ .order};f__{ .family};g__{ .genus};s__{ .species}"
                    lineage = .Select

                    If appendByID Then
                        additionals = indexAppendData.TryGetValue(accession)
                    Else
                        additionals = indexAppendData.TryGetValue(.species)
                    End If
                End With

                If Not additionals Is Nothing Then
                    seq.Headers = New String() {title, taxid, taxonomy}.Join(additionals.Properties.Select(Function(tuple) $"{tuple.Key}={tuple.Value}"))
                Else
                    seq.Headers = New String() {title, taxid, taxonomy}
                End If

                Call fastaWriter.WriteLine(seq.GenerateDocument(-1))
                Call summary.Flush(New EntityObject With {
                         .ID = accession,
                         .Properties = New Dictionary(Of String, String) From {
                             {"title", title},
                             {"taxid", taxid},
                             {NcbiTaxonomyTree.superkingdom, lineage(0)},
                             {NcbiTaxonomyTree.phylum, lineage(1)},
                             {NcbiTaxonomyTree.class, lineage(2)},
                             {NcbiTaxonomyTree.order, lineage(3)},
                             {NcbiTaxonomyTree.family, lineage(4)},
                             {NcbiTaxonomyTree.genus, lineage(5)},
                             {NcbiTaxonomyTree.species, lineage(6)}
                         }.AddRange(
                            data:=additionals?.Properties,
                            replaceDuplicated:=True
                         )
                     })
            Next
        End Using

        Return 0
    End Function
End Module
