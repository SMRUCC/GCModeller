Imports System.IO
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.Linq
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
        Dim out As String = args.GetValue("/out", [in].TrimFileExt & ".bin")
        Return Taxonomy.Archive([in], out)
    End Function

    <ExportAPI("/Export.GI", Usage:="/Export.GI /in <ncbi:nt.fasta> [/out <out.csv>]")>
    Public Function ExportGI(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim out As String = args.GetValue("/out", [in].TrimFileExt & ".gi.Csv")
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
        Dim taxiHash As Dictionary(Of Integer, Integer) = Taxonomy.LoadArchive(gi2taxi)
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
                          Let gi As String = Regex.Match(x.Name, "gi\|\d+", RegexICSng).Value.Split("|"c).Last
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
        Dim out As String = args.GetValue("/out", [in].TrimFileExt & ".Taxonomy.fasta")
        Dim taxiHash As Dictionary(Of Integer, Integer) = Taxonomy.LoadArchive(gi2taxi)
        Dim taxTree As New NcbiTaxonomyTree(tax)

        Using writer As StreamWriter = out.OpenWriter(Encodings.ASCII)
            For Each fa As FastaToken In New StreamIterator([in]).ReadStream
                Dim gi As String = Regex.Match(fa.Title, "gi\|\d+", RegexICSng).Value

                If Not String.IsNullOrEmpty(gi) AndAlso taxiHash.ContainsKey(gi) Then
                    Dim taxi As Integer = taxiHash(gi)
                    Dim tree = taxTree.GetAscendantsWithRanksAndNames({taxi}, True)
                    Dim taxonomy As String = TaxonNode.Taxonomy(tree.First.Value)
                    Dim title As String = $"gi_{gi}.{taxi} {taxonomy}"
                    fa = New FastaToken({title}, fa.SequenceData)
                    Call writer.WriteLine(fa.GenerateDocument(120))
                End If
            Next

            Return 0
        End Using
    End Function
End Module
