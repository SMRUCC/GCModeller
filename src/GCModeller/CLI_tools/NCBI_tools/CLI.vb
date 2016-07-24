Imports System.IO
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.Linq
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.Assembly.NCBI.Entrez
Imports SMRUCC.genomics.SequenceModel.FASTA

<PackageNamespace("NCBI_tools.CLI", Category:=APICategories.CLI_MAN, Publisher:="xie.guigang@gcmodeller.org")>
Module CLI

    <ExportAPI("/Build_gi2taxi",
               Usage:="/Build_gi2taxi /in <gi2taxi.dmp> [/out <out.dat>]")>
    Public Function Build_gi2taxi(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".bin")
        Return Taxonomy.Archive([in], out)
    End Function

    <ExportAPI("/Export.GI", Usage:="/Export.GI /in <ncbi:nt.fasta> [/out <out.csv>]")>
    Public Function ExportGI(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".gi.Csv")
        Dim nt As New StreamIterator([in])

        Using writer As New WriteStream(Of TaxiValue)(out)

            writer.BaseStream.AutoFlush = True
            writer.BaseStream.NewLine = vbLf

            For Each fa As FastaToken In nt.ReadStream
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

    <ExportAPI("/Associate.Taxonomy",
           Usage:="/Associate.Taxonomy /in <in.DIR> /tax <ncbi_taxonomy:names,nodes> /gi2taxi <gi2taxi.bin> [/gi <nt.gi.csv> /out <out.DIR>]")>
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
            TaxiValue.BuildHash(ref.LoadCsv(Of TaxiValue)),
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
                    x.x.TaxonomyTree = TaxonNode.Taxonomy(taxTree.GetAscendantsWithRanksAndNames({CInt(x.x.taxid)}, True).Values.First)
                Else
                    Call x.gi.ToString.Warning
                End If
            Next

            data = LQuery.ToArray(Function(x) x.x)

            Call data.SaveTo(out)
        Next

        Return 0
    End Function

    <ExportAPI("/Nt.Taxonomy",
               Usage:="/Nt.Taxonomy /in <nt.fasta> /gi2taxi <gi2taxi.bin> /tax <ncbi_taxonomy:names,nodes> [/out <out.fasta>]")>
    Public Function NtTaxonomy(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim gi2taxi As String = args("/gi2taxi")
        Dim tax As String = args("/tax")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".Taxonomy.fasta")
        Dim taxiHash As BucketDictionary(Of Integer, Integer) = Taxonomy.Hash_gi2Taxi(gi2taxi)
        Dim taxTree As New NcbiTaxonomyTree(tax)
        Dim notFoundTax As String = out.TrimSuffix & ".notFound.txt"

        Using notFound As StreamWriter = (out.TrimSuffix & ".NotFound.fasta").OpenWriter,
            writer As StreamWriter = out.OpenWriter(Encodings.ASCII),
            table As New WriteStream(Of TaxiSummary)(out.TrimSuffix & ".Csv"),
            taxNotFoun = notFoundTax.OpenWriter

            For Each fa As FastaToken In New StreamIterator([in]).ReadStream
                Dim gi As Integer = CInt(Val(Regex.Match(fa.Title, "gi\|\d+", RegexICSng).Value.Split("|"c).Last))

                If gi > 0 AndAlso taxiHash.ContainsKey(gi) Then
                    Dim taxi As Integer = taxiHash(gi)
                    Dim tree = taxTree.GetAscendantsWithRanksAndNames({taxi}, True)
                    Dim taxonomy As String = TaxonNode.Taxonomy(tree.First.Value)
                    Dim hash = TaxonNode.ToHash(tree.First.Value)

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
                    fa = New FastaToken({title}, fa.SequenceData)
                    Call writer.WriteLine(fa.GenerateDocument(120))
                Else
                    Call $"gi {gi} not found taxid...".__DEBUG_ECHO
                    Call notFound.WriteLine(fa.GenerateDocument(-1))
                End If
            Next

            Return 0
        End Using
    End Function

    <ExportAPI("/Assign.Taxonomy",
               Usage:="/Assign.Taxonomy /in <in.DIR> /gi <regexp> /index <fieldName> /tax <NCBI nodes/names.dmp> /gi2taxi <gi2taxi.txt/bin> [/out <out.DIR>]")>
    Public Function AssignTaxonomy(args As CommandLine) As Integer
        Dim [in] As String = args.GetFullDIRPath("/in")
        Dim regexp As String = args("/gi")
        Dim index As String = args("/index")
        Dim tax As String = args("/tax")
        Dim gi2taxi As String = args("/gi2taxi")
        Dim EXPORT As String = args.GetValue("/out", [in].TrimDIR & ".Taxonomy/")
        Dim tree As New NcbiTaxonomyTree(tax)
        Dim giTaxidhash As BucketDictionary(Of Integer, Integer) =
            Taxonomy.AcquireAuto(gi2taxi)
        Dim getGI = Taxono.Parser_gi(regexp)

        For Each file As String In ls - l - r - wildcards("*.Csv") <= [in]
            Dim data = Taxono.Load(file, index)
            Dim out As String = EXPORT & "/" & file.BaseName & ".Csv"

            For Each x In data
                Dim gi As Integer = getGI(x)

                If giTaxidhash.ContainsKey(gi) Then
                    Dim taxid As Integer = giTaxidhash(gi)
                    x.taxid = taxid
                    Dim nodes = tree.GetAscendantsWithRanksAndNames({taxid}, True)
                    Dim hash = TaxonNode.ToHash(nodes.First.Value)

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

    <ExportAPI("/Assign.Taxonomy.SSU",
               Usage:="/Assign.Taxonomy.SSU /in <in.DIR> /index <fieldName> /ref <SSU-ref.fasta> [/out <out.DIR>]")>
    Public Function AssignTaxonomy2(args As CommandLine) As Integer
        Dim [in] As String = args.GetFullDIRPath("/in")
        Dim index As String = args("/index")
        Dim tax As String = args("/ref")
        Dim EXPORT As String = args.GetValue("/out", [in].TrimDIR & ".Taxonomy/")
        Dim taxHash = (From x As FastaToken
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
                If taxHash.ContainsKey(x.Tag) Then
                    Dim tokens As String() = taxHash(x.Tag).Split(";"c)
                    Dim hash As New Dictionary(Of String, String) From {{"species", tokens(6)},
            {"genus", tokens(5)},
            {"family", tokens(4)},
            {"order", tokens(3)},
            {"class", tokens(2)},
            {"phylum", tokens(1)},
            {"superkingdom", tokens(0)}}

                    With x
                        .class = hash.TryGetValue(NcbiTaxonomyTree.class)
                        .family = hash.TryGetValue(NcbiTaxonomyTree.family)
                        .genus = hash.TryGetValue(NcbiTaxonomyTree.genus)
                        .order = hash.TryGetValue(NcbiTaxonomyTree.order)
                        .phylum = hash.TryGetValue(NcbiTaxonomyTree.phylum)
                        .species = hash.TryGetValue(NcbiTaxonomyTree.species)
                        .superkingdom = hash.TryGetValue(NcbiTaxonomyTree.superkingdom)
                    End With

                    x.Taxonomy = taxHash(x.Tag)
                End If
            Next

            Call Taxono.Save(out, data, index)
        Next

        Return 0
    End Function

    Public Class Taxono : Inherits ITaxon

        Public Property Tag As String
        <Meta> Public Property Values As Dictionary(Of String, String)

        Public Shared Function Load(path As String, index As String) As IEnumerable(Of Taxono)
            Dim maps As New Dictionary(Of String, String) From {{index, NameOf(Taxono.Tag)}}
            Return path.LoadCsv(Of Taxono)(maps:=maps)
        End Function

        Public Shared Function Save(path As String, result As IEnumerable(Of Taxono), index As String) As Boolean
            Dim maps As New Dictionary(Of String, String) From {{NameOf(Taxono.Tag), index}}
            Return result.SaveTo(path, maps:=maps)
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Shared Function Parser_gi(regexp As String) As Func(Of Taxono, Integer)
            Dim rg As New Regex(regexp, RegexICSng)
            Return Function(x) CInt(Val(Regex.Match(rg.Match(x.Tag).Value, "\d+").Value))
        End Function
    End Class

    Public Class TaxiSummary : Inherits ITaxon

        Public Property Name As String
        Public Property gi As String

        Public Property title As String
        Public Property sequence As String
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
    End Class
End Module
