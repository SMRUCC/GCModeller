Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Similarity
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    <ExportAPI("/Filter.Exports", Usage:="/Filter.Exports /in <nt.fasta> /tax <taxonomy_DIR> /gi2taxid <gi2taxid.txt> /words <list.txt> /out <out.DIR>")>
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
End Module
