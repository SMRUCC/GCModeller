Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Text.Similarity
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    <ExportAPI("/Filter.Exports",
               Usage:="/Filter.Exports /in <nt.fasta> /tax <taxonomy_DIR> /gi2taxid <gi2taxid.txt> /words <list.txt> [/out <out.DIR>]")>
    Public Function FilterExports(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim tax As New NcbiTaxonomyTree(args("/tax"))
        Dim gi2taxid = Taxonomy.AcquireAuto(args("/gi2taxid"))
        Dim words As String = args("/words")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "." & words.BaseName & ".EXPORT/")
        Dim wordList As String() = words.IterateAllLines.Select(AddressOf Trim).ToArray
        Dim m As New Value(Of DistResult)

        Const deli As String = "AAAAAAAAAAAA"

        For Each fa In New StreamIterator([in]).ReadStream
            Dim gi As Integer = CInt(Val(Regex.Match(fa.Title, "gi\|\d+").Value.Split("|"c).Last))

            If gi2taxid.ContainsKey(gi) Then
                Dim taxid As Integer = gi2taxid(gi)
                Dim taxon = tax.GetAscendantsWithRanksAndNames(taxid, True)
                Dim tree = TaxonNode.Taxonomy(taxon, deli).NormalizePathString.Replace(deli, "/")
                Dim name As String = fa.Attributes.Get(4, "").Trim
                Dim write As Boolean = False

                Call fa.Attributes.Add(TaxonNode.Taxonomy(taxon,))

                For Each s As String In wordList
                    If InStr(name, s, CompareMethod.Text) > 0 OrElse
                        ((Not (m = MatchFuzzy(s, name,,)) Is Nothing) AndAlso
                        s.Split.Length - m.value.NumMatches < 3) Then

                        Dim path As String = out & "/" & tree & "/" & s & ".fasta"
                        Call fa.GenerateDocument(-1, False).SaveTo(path, Encoding.ASCII, append:=True)
                        write = True
                        Call fa.Title.__DEBUG_ECHO

                        Exit For
                    End If
                Next

                If Not write Then
                    Dim hash = TaxonNode.ToHash(taxon)
                    Dim sp As String = hash.TryGetValue("species", [default]:=Nothing)

                    If Not String.IsNullOrEmpty(sp) Then

                        For Each s As String In wordList
                            If InStr(s, sp, CompareMethod.Text) > 0 OrElse
                                InStr(sp, s, CompareMethod.Text) > 0 OrElse
                                ((Not (m = MatchFuzzy(s, sp,,)) Is Nothing) AndAlso
                                s.Split.Length - m.value.NumMatches < 3) Then

                                Dim path As String = out & "/" & tree & "/" & s & ".fasta"
                                Call fa.GenerateDocument(-1, False).SaveTo(path, Encoding.ASCII, append:=True)
                                write = True
                                Call fa.Title.__DEBUG_ECHO

                                Exit For
                            End If
                        Next
                    End If
                End If
            End If
        Next

        Return 0
    End Function

    <ExportAPI("/nt.matches", Usage:="/nt.matches /in <nt.fasta> /list <words.txt> [/out <out.fasta>]")>
    Public Function NtKeyMatches(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim list As String = args("/list")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "_" & list.BaseName & ".fasta")
        Dim terms As String() =
            list _
            .ReadAllLines _
            .Where(Function(s) Not s.IsBlank) _
            .ToArray(Function(s) s.Trim.ToLower)
        Dim words As Dictionary(Of String, String()) =
            (From term In (From line As String
                           In terms
                           Let ws As String() = line.Split
                           Select From w As String
                                  In ws
                                  Select termLine = line,
                                      word = w).MatrixAsIterator
             Select term
             Group term.termLine By term.word Into Group) _
                .ToDictionary(Function(x) x.word,
                              Function(x) x.Group.Distinct.ToArray)

        Call words.GetJson(True).__DEBUG_ECHO

        Using writer As StreamWriter = out.OpenWriter(encoding:=Encodings.ASCII)
            For Each fa As FastaToken In New StreamIterator([in]).ReadStream
                For Each word As String In words.Keys

                    Dim attrs As String() = fa.Attributes
                    Dim title As String = fa.Title.ToLower
                    Dim writeData = Sub()
                                        Dim hit As New List(Of String)(attrs)
                                        hit += String.Join("; ", words(word))
                                        Dim write As New FastaToken(hit, fa.SequenceData)

                                        Call writer.WriteLine(write.GenerateDocument(120))
                                        Call write.Title.__DEBUG_ECHO
                                    End Sub

                    If InStr(title, word) > 0 Then
                        Call writeData()
                    Else
                        For Each s As String In attrs
                            For Each x As String In s.Trim.ToLower.Split

                                Dim d = LevenshteinDistance.ComputeDistance(x, word)

                                If Not (d Is Nothing OrElse d.Score < 0.8) Then
                                    Call writeData()
                                End If
                            Next
                        Next
                    End If
                Next
            Next
        End Using

        Return 0
    End Function
End Module
