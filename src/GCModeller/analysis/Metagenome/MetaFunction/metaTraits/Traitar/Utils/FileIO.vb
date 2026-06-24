' ============================================================================
'  FileIO.vb - File parsing and serialization helpers
'  Traitar Microbial Trait Analyzer - VB.NET Implementation
'
'  Parses:
'    - GFF3 gene annotation files
'    - FASTA protein files
'    - HMMER hmmsearch --domtblout output (Pfam annotations)
'    - Traitar model files: {pid}_bias.txt, {pid}_feats.txt,
'      {pid}_non-zero+weights.txt, pt2acc.txt, pf2acc_desc.txt
' ============================================================================
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text.RegularExpressions
Imports TraitarVBNet.Models

Namespace Utils

    Public Module FileIO

        ' ---------------------------------------------------------------------
        '  GFF3 parsing
        ' ---------------------------------------------------------------------

        ''' <summary>
        ''' Parse a GFF3 file and return one ProteinRecord per "CDS" / "gene"
        ''' feature that has a Parent or locus_tag attribute. The protein
        ''' sequence itself is filled later from the FASTA file.
        ''' </summary>
        Public Function ParseGff(path As String) As List(Of ProteinRecord)
            Dim result As New List(Of ProteinRecord)()
            If Not File.Exists(path) Then Return result

            For Each line As String In File.ReadLines(path)
                If String.IsNullOrEmpty(line) Then Continue For
                If line.StartsWith("#") Then Continue For
                Dim parts As String() = line.Split(ControlChars.Tab)
                If parts.Length < 9 Then Continue For
                Dim contig As String = parts(0)
                Dim featureType As String = parts(2)
                If Not (featureType = "CDS" OrElse featureType = "gene" OrElse featureType = "exon") Then Continue For

                Dim startIdx As Integer, endIdx As Integer
                If Not Integer.TryParse(parts(3), startIdx) Then Continue For
                If Not Integer.TryParse(parts(4), endIdx) Then Continue For
                Dim strand As String = parts(6)
                Dim attrs As String = parts(8)

                ' Extract locus tag (try ID, then Parent, then locus_tag)
                Dim locusTag As String = ExtractAttribute(attrs, "ID")
                If String.IsNullOrEmpty(locusTag) Then locusTag = ExtractAttribute(attrs, "Parent")
                If String.IsNullOrEmpty(locusTag) Then locusTag = ExtractAttribute(attrs, "locus_tag")
                If String.IsNullOrEmpty(locusTag) Then Continue For

                Dim product As String = ExtractAttribute(attrs, "product")

                Dim rec As New ProteinRecord() With {
                    .LocusTag = locusTag,
                    .Contig = contig,
                    .Start = startIdx,
                    .End = endIdx,
                    .Strand = strand,
                    .Product = product
                }
                result.Add(rec)
            Next
            Return result
        End Function

        ''' <summary>Extract the value of an attribute key=val;... from a GFF3 column 9.</summary>
        Private Function ExtractAttribute(attrField As String, key As String) As String
            If String.IsNullOrEmpty(attrField) Then Return ""
            Dim tokens As String() = attrField.Split(";"c)
            For Each t As String In tokens
                Dim eq As Integer = t.IndexOf("="c)
                If eq > 0 Then
                    Dim k As String = t.Substring(0, eq).Trim()
                    Dim v As String = t.Substring(eq + 1).Trim()
                    If String.Equals(k, key, StringComparison.OrdinalIgnoreCase) Then
                        Return v
                    End If
                End If
            Next
            Return ""
        End Function

        ' ---------------------------------------------------------------------
        '  FASTA parsing
        ' ---------------------------------------------------------------------

        ''' <summary>
        ''' Parse a protein FASTA file into a dictionary keyed by locus tag.
        ''' The locus tag is taken from the first whitespace-delimited token of
        ''' the header line (after the leading '&gt;').
        ''' </summary>
        Public Function ParseFasta(path As String) As Dictionary(Of String, String)
            Dim result As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
            If Not File.Exists(path) Then Return result

            Dim currentId As String = ""
            Dim currentSeq As New System.Text.StringBuilder()
            For Each line As String In File.ReadLines(path)
                If String.IsNullOrEmpty(line) Then Continue For
                If line.StartsWith(">") Then
                    If currentId <> "" Then
                        result(currentId) = currentSeq.ToString()
                    End If
                    Dim header As String = line.Substring(1).Trim()
                    Dim sp As Integer = header.IndexOfAny(New Char() {" "c, ControlChars.Tab})
                    If sp < 0 Then
                        currentId = header
                    Else
                        currentId = header.Substring(0, sp)
                    End If
                    currentSeq.Clear()
                Else
                    currentSeq.Append(line.Trim())
                End If
            Next
            If currentId <> "" Then
                result(currentId) = currentSeq.ToString()
            End If
            Return result
        End Function

        ''' <summary>
        ''' Attach protein sequences from a FASTA file to the ProteinRecords
        ''' parsed from a GFF. Matching is by locus tag (case-insensitive).
        ''' </summary>
        Public Sub AttachSequences(proteins As List(Of ProteinRecord), fastaPath As String)
            Dim seqs As Dictionary(Of String, String) = ParseFasta(fastaPath)
            For Each p As ProteinRecord In proteins
                If seqs.ContainsKey(p.LocusTag) Then
                    p.Sequence = seqs(p.LocusTag)
                End If
            Next
        End Sub

        ' ---------------------------------------------------------------------
        '  HMMER hmmsearch --domtblout parsing
        ' ---------------------------------------------------------------------

        ''' <summary>
        ''' Parse a HMMER3 domtblout file and return all Pfam hits.
        ''' Each non-comment line yields one PfamHit; the caller is responsible
        ''' for applying the bit-score / E-value thresholds (PfamHit.Passes).
        '''
        ''' domtblout columns (1-indexed, after the leading 3 '#' comment chars):
        '''  1: target name      (protein locus tag)
        '''  2: target accession
        '''  4: query name       (e.g. 7tm_1)
        '''  5: query accession  (e.g. PF00001)
        '''  7: full E-value
        '''  8: full score (bit)
        '''  9: full bias
        ''' 10: dom # (1-based)
        ''' 12: dom E-value
        ''' 13: dom score (bit)
        ''' </summary>
        Public Function ParseHmmsearchDomtblout(path As String) As List(Of PfamHit)
            Dim hits As New List(Of PfamHit)()
            If Not File.Exists(path) Then Return hits

            For Each line As String In File.ReadLines(path)
                If String.IsNullOrEmpty(line) Then Continue For
                If line.StartsWith("#") Then Continue For
                Dim parts As String() = Regex.Split(line.Trim(), "\s+")
                If parts.Length < 13 Then Continue For

                Dim target As String = parts(0)
                Dim pfamAcc As String = parts(4)
                Dim pfamName As String = parts(3)
                ' Some HMMER builds leave the accession column empty; fall back
                If String.IsNullOrEmpty(pfamAcc) OrElse pfamAcc = "-" Then pfamAcc = "PF" & pfamName

                Dim fullE As Double, fullScore As Double
                If Not Double.TryParse(parts(6), fullE) Then Continue For
                If Not Double.TryParse(parts(7), fullScore) Then Continue For

                Dim hit As New PfamHit() With {
                    .TargetName = target,
                    .PfamAcc = pfamAcc,
                    .PfamName = pfamName,
                    .EValue = fullE,
                    .BitScore = fullScore
                }
                hits.Add(hit)
            Next
            Return hits
        End Function

        ''' <summary>
        ''' Parse a generic Pfam annotation TSV with columns:
        '''   target_name &lt;TAB&gt; pfam_acc &lt;TAB&gt; pfam_name &lt;TAB&gt; evalue &lt;TAB&gt; bitscore
        ''' This is the fallback format used by the demo when HMMER is not
        ''' installed (the user can supply pre-computed Pfam annotations).
        ''' </summary>
        Public Function ParsePfamTsv(path As String) As List(Of PfamHit)
            Dim hits As New List(Of PfamHit)()
            If Not File.Exists(path) Then Return hits

            For Each line As String In File.ReadLines(path)
                If String.IsNullOrEmpty(line) Then Continue For
                If line.StartsWith("#") Then Continue For
                Dim parts As String() = line.Split(ControlChars.Tab)
                If parts.Length < 5 Then Continue For

                Dim eVal As Double, bitScore As Double
                If Not Double.TryParse(parts(3), eVal) Then Continue For
                If Not Double.TryParse(parts(4), bitScore) Then Continue For

                hits.Add(New PfamHit() With {
                    .TargetName = parts(0),
                    .PfamAcc = parts(1),
                    .PfamName = parts(2),
                    .EValue = eVal,
                    .BitScore = bitScore
                })
            Next
            Return hits
        End Function

        ' ---------------------------------------------------------------------
        '  Traitar model file parsing
        ' ---------------------------------------------------------------------

        ''' <summary>
        ''' Parse pt2acc.txt: phenotype id &lt;TAB&gt; name &lt;TAB&gt; category.
        ''' Returns a dictionary keyed by phenotype id.
        ''' </summary>
        Public Function ParsePhenotypeTable(path As String) As Dictionary(Of Integer, Tuple(Of String, String))
            Dim result As New Dictionary(Of Integer, Tuple(Of String, String))()
            If Not File.Exists(path) Then Return result

            For Each line As String In File.ReadLines(path)
                If String.IsNullOrEmpty(line) Then Continue For
                If line.StartsWith("accession") Then Continue For
                Dim parts As String() = line.Split(ControlChars.Tab)
                If parts.Length < 3 Then Continue For
                Dim pid As Integer
                If Not Integer.TryParse(parts(0).Trim(), pid) Then Continue For
                Dim name As String = parts(1).Trim()
                Dim category As String = parts(2).Trim()
                result(pid) = Tuple.Create(name, category)
            Next
            Return result
        End Function

        ''' <summary>
        ''' Parse pf2acc_desc.txt: Pfam accession &lt;TAB&gt; description.
        ''' </summary>
        Public Function ParsePfamDescriptionTable(path As String) As Dictionary(Of String, String)
            Dim result As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
            If Not File.Exists(path) Then Return result
            For Each line As String In File.ReadLines(path)
                If String.IsNullOrEmpty(line) Then Continue For
                If line.StartsWith("description") Then Continue For
                Dim parts As String() = line.Split(ControlChars.Tab)
                If parts.Length < 2 Then Continue For
                result(parts(0).Trim()) = parts(1).Trim()
            Next
            Return result
        End Function

        ''' <summary>
        ''' Parse a {pid}_bias.txt file. Each line: C &lt;TAB&gt; bias.
        ''' Returns a list of (C, bias) pairs in file order (which is the
        ''' cross-validation-accuracy order, best first).
        ''' </summary>
        Public Function ParseBiasFile(path As String) As List(Of Tuple(Of Double, Double))
            Dim result As New List(Of Tuple(Of Double, Double))()
            If Not File.Exists(path) Then Return result
            For Each line As String In File.ReadLines(path)
                If String.IsNullOrEmpty(line) Then Continue For
                Dim parts As String() = Regex.Split(line.Trim(), "\s+")
                If parts.Length < 2 Then Continue For
                Dim c As Double, b As Double
                If Not Double.TryParse(parts(0), c) Then Continue For
                If Not Double.TryParse(parts(1), b) Then Continue For
                result.Add(Tuple.Create(c, b))
            Next
            Return result
        End Function

        ''' <summary>
        ''' Parse a {pid}_feats.txt file.
        ''' Row 1: header listing the C values (e.g. "0.5_0", "0.7_0", ...).
        ''' Subsequent rows: Pfam &lt;TAB&gt; w_C1 &lt;TAB&gt; w_C2 ...
        ''' Returns:
        '''   * the list of C values (in column order)
        '''   * a dictionary Pfam -> weight vector (one weight per C column)
        ''' </summary>
        Public Function ParseFeatsFile(path As String,
                                       ByRef cValues As List(Of Double),
                                       ByRef weights As Dictionary(Of String, List(Of Double))) _
                                   As Boolean
            cValues = New List(Of Double)()
            weights = New Dictionary(Of String, List(Of Double))(StringComparer.OrdinalIgnoreCase)
            If Not File.Exists(path) Then Return False

            Dim lines As String() = File.ReadAllLines(path)
            If lines.Length = 0 Then Return False

            ' Header row: parse C values from tokens like "0.5_0", "1_0"
            Dim headerParts As String() = Regex.Split(lines(0).Trim(), "\s+")
            For Each tok As String In headerParts
                If String.IsNullOrEmpty(tok) Then Continue For
                Dim underIdx As Integer = tok.IndexOf("_"c)
                Dim numStr As String = If(underIdx > 0, tok.Substring(0, underIdx), tok)
                Dim c As Double
                If Double.TryParse(numStr, c) Then cValues.Add(c)
            Next

            ' Body rows: Pfam + one weight per C column
            For i As Integer = 1 To lines.Length - 1
                Dim line As String = lines(i)
                If String.IsNullOrEmpty(line) Then Continue For
                Dim parts As String() = Regex.Split(line.Trim(), "\s+")
                If parts.Length < 1 Then Continue For
                Dim pfam As String = parts(0)
                If String.IsNullOrEmpty(pfam) Then Continue For
                Dim w As New List(Of Double)()
                For j As Integer = 1 To Math.Min(cValues.Count, parts.Length - 1)
                    Dim v As Double
                    If Double.TryParse(parts(j), v) Then
                        w.Add(v)
                    Else
                        w.Add(0.0R)
                    End If
                Next
                ' Pad if the row was shorter than the header
                While w.Count < cValues.Count
                    w.Add(0.0R)
                End While
                weights(pfam) = w
            Next
            Return True
        End Function

        ''' <summary>
        ''' Parse a {pid}_non-zero+weights.txt file.
        ''' Header: class &lt;TAB&gt; C1 &lt;TAB&gt; C2 ... &lt;TAB&gt; Pfam_desc &lt;TAB&gt; cor
        ''' Body:   Pfam &lt;TAB&gt; +/- &lt;TAB&gt; w_C1 ... &lt;TAB&gt; description &lt;TAB&gt; correlation
        '''
        ''' Returns the list of (Pfam, sign, weightsByC, description, correlation).
        ''' </summary>
        Public Function ParseNonZeroWeightsFile(path As String,
                                                ByRef cValues As List(Of Double),
                                                ByRef features As List(Of Tuple(Of String, String, List(Of Double), String, Double))) _
                                            As Boolean
            cValues = New List(Of Double)()
            features = New List(Of Tuple(Of String, String, List(Of Double), String, Double))()
            If Not File.Exists(path) Then Return False

            Dim lines As String() = File.ReadAllLines(path)
            If lines.Length = 0 Then Return False

            ' Header: first token is empty/class, then C1..Cn, then Pfam_desc, then cor
            Dim headerParts As String() = Regex.Split(lines(0).Trim(), "\s+")
            Dim nTokens As Integer = headerParts.Length
            ' Identify the position of "Pfam_desc" and "cor" in the header
            Dim pfamDescIdx As Integer = -1, corIdx As Integer = -1
            For k As Integer = 0 To nTokens - 1
                If headerParts(k) = "Pfam_desc" Then pfamDescIdx = k
                If headerParts(k) = "cor" Then corIdx = k
            Next
            ' C columns are between index 1 (after "class") and pfamDescIdx
            Dim cStart As Integer = 1
            Dim cEnd As Integer = If(pfamDescIdx > 0, pfamDescIdx - 1, nTokens - 2)
            For k As Integer = cStart To cEnd
                Dim tok As String = headerParts(k)
                Dim underIdx As Integer = tok.IndexOf("_"c)
                Dim numStr As String = If(underIdx > 0, tok.Substring(0, underIdx), tok)
                Dim c As Double
                If Double.TryParse(numStr, c) Then cValues.Add(c)
            Next
            Dim nC As Integer = cValues.Count

            ' Body rows
            For i As Integer = 1 To lines.Length - 1
                Dim line As String = lines(i)
                If String.IsNullOrEmpty(line) Then Continue For
                Dim parts As String() = Regex.Split(line.Trim(), "\s+")
                If parts.Length < 2 Then Continue For

                Dim pfam As String = parts(0)
                Dim sign As String = parts(1)
                ' Weights: parts(2) .. parts(2 + nC - 1)
                Dim w As New List(Of Double)()
                For j As Integer = 0 To nC - 1
                    Dim idx As Integer = 2 + j
                    Dim v As Double = 0.0R
                    If idx < parts.Length Then Double.TryParse(parts(idx), v)
                    w.Add(v)
                Next

                ' Description: from index (2 + nC) up to (corIdx - 1) joined with spaces
                Dim descStart As Integer = 2 + nC
                Dim descEnd As Integer = If(corIdx > 0, corIdx - 1, parts.Length - 2)
                Dim desc As New System.Text.StringBuilder()
                For k As Integer = descStart To descEnd
                    If k >= parts.Length Then Exit For
                    If desc.Length > 0 Then desc.Append(" "c)
                    desc.Append(parts(k))
                Next

                ' Correlation: last column (or at corIdx)
                Dim cor As Double = 0.0R
                If corIdx > 0 AndAlso corIdx < parts.Length Then
                    Double.TryParse(parts(corIdx), cor)
                ElseIf parts.Length > 0 Then
                    Double.TryParse(parts(parts.Length - 1), cor)
                End If

                features.Add(Tuple.Create(pfam, sign, w, desc.ToString(), cor))
            Next
            Return True
        End Function

    End Module

End Namespace
