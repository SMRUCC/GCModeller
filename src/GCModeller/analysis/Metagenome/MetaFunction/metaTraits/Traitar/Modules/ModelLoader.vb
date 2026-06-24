' ============================================================================
'  ModelLoader.vb - Loads pre-trained Traitar phenotype models from disk
'
'  Reads the three files per phenotype:
'    {pid}_bias.txt             - C<TAB>bias pairs (13 lines, accuracy-ordered)
'    {pid}_feats.txt            - Pfam<TAB>w_C1<TAB>w_C2 ... (full weight matrix)
'    {pid}_non-zero+weights.txt - Pfam<TAB>class<TAB>w_C1 ... <TAB>desc<TAB>cor
'
'  Also reads the global catalogs:
'    pt2acc.txt      - phenotype ID -> name + category
'    pf2acc_desc.txt - Pfam accession -> description
' ============================================================================
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.IO.Compression
Imports TraitarVBNet.Models

Namespace Modules

    Public Module ModelLoader

        ''' <summary>
        ''' Load the phenotype catalog (pt2acc.txt).
        ''' Returns a dictionary: phenotype ID -> (name, category).
        ''' </summary>
        Public Function LoadPhenotypeCatalog(path As String) _
            As Dictionary(Of String, Tuple(Of String, String))
            Dim result As New Dictionary(Of String, Tuple(Of String, String))(StringComparer.OrdinalIgnoreCase)
            If Not File.Exists(path) Then Return result
            For Each line As String In File.ReadLines(path)
                If String.IsNullOrEmpty(line) Then Continue For
                Dim parts As String() = line.Split(New Char() {ControlChars.Tab, " "c},
                                                   StringSplitOptions.RemoveEmptyEntries)
                If parts.Length < 3 Then Continue For
                Dim id As String = parts(0)
                ' Name may contain spaces; category is the last token
                Dim category As String = parts(parts.Length - 1)
                Dim name As New System.Text.StringBuilder()
                For i As Integer = 1 To parts.Length - 2
                    If name.Length > 0 Then name.Append(" "c)
                    name.Append(parts(i))
                Next
                result(id) = Tuple.Create(name.ToString(), category)
            Next
            Return result
        End Function

        ''' <summary>
        ''' Load the Pfam description catalog (pf2acc_desc.txt).
        ''' Returns a dictionary: Pfam accession -> description.
        ''' </summary>
        Public Function LoadPfamCatalog(path As String) _
            As Dictionary(Of String, String)
            Dim result As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
            If Not File.Exists(path) Then Return result
            For Each line As String In File.ReadLines(path)
                If String.IsNullOrEmpty(line) Then Continue For
                Dim idx As Integer = line.IndexOf(ControlChars.Tab)
                If idx < 0 Then idx = line.IndexOf(" "c)
                If idx <= 0 Then Continue For
                Dim acc As String = line.Substring(0, idx).Trim()
                Dim desc As String = line.Substring(idx + 1).Trim()
                result(acc) = desc
            Next
            Return result
        End Function

        ''' <summary>
        ''' Load one phenotype's bias file.
        ''' Returns a list of (C, bias) pairs in file order (which is the
        ''' accuracy-ranked order: best model first).
        ''' </summary>
        Public Function LoadBiasFile(path As String) As List(Of Tuple(Of Double, Double))
            Dim result As New List(Of Tuple(Of Double, Double))()
            If Not File.Exists(path) Then Return result
            For Each line As String In File.ReadLines(path)
                If String.IsNullOrEmpty(line) Then Continue For
                Dim parts As String() = line.Split(New Char() {ControlChars.Tab, " "c},
                                                   StringSplitOptions.RemoveEmptyEntries)
                If parts.Length < 2 Then Continue For
                Dim c As Double = 0.0R, b As Double = 0.0R
                If Not Double.TryParse(parts(0), c) Then Continue For
                If Not Double.TryParse(parts(1), b) Then Continue For
                result.Add(Tuple.Create(c, b))
            Next
            Return result
        End Function

        ''' <summary>
        ''' Load one phenotype's full feature weight matrix ({pid}_feats.txt).
        ''' Returns:
        '''   cHeader  - list of C values (column headers, e.g. "0.5_0" -> 0.5)
        '''   weights  - dictionary: Pfam accession -> (C -> weight)
        ''' </summary>
        Public Function LoadFeatsFile(path As String) _
            As Tuple(Of List(Of Double),
                     Dictionary(Of String, Dictionary(Of Double, Double)))
            Dim cHeader As New List(Of Double)()
            Dim weights As New Dictionary(Of String, Dictionary(Of Double, Double))(StringComparer.OrdinalIgnoreCase)
            If Not File.Exists(path) Then Return Tuple.Create(cHeader, weights)

            Dim firstLine As Boolean = True
            For Each line As String In File.ReadLines(path)
                If String.IsNullOrEmpty(line) Then Continue For
                Dim parts As String() = line.Split(New Char() {ControlChars.Tab}, StringSplitOptions.None)
                If firstLine Then
                    firstLine = False
                    ' Header row: empty cell, then "C_0" tokens
                    For j As Integer = 1 To parts.Length - 1
                        Dim tok As String = parts(j).Trim()
                        If tok.Length = 0 Then Continue For
                        ' Strip the "_0" suffix
                        Dim cStr As String = tok
                        Dim usIdx As Integer = tok.IndexOf("_"c)
                        If usIdx > 0 Then cStr = tok.Substring(0, usIdx)
                        Dim c As Double = 0.0R
                        If Double.TryParse(cStr, c) Then cHeader.Add(c)
                    Next
                    Continue For
                End If

                ' Data row: Pfam<TAB>w_C1<TAB>w_C2 ...
                If parts.Length < 2 Then Continue For
                Dim pfam As String = parts(0).Trim()
                If pfam.Length = 0 Then Continue For
                Dim row As New Dictionary(Of Double, Double)()
                For j As Integer = 1 To parts.Length - 1
                    If j - 1 >= cHeader.Count Then Exit For
                    Dim v As Double = 0.0R
                    Double.TryParse(parts(j).Trim(), v)
                    row(cHeader(j - 1)) = v
                Next
                weights(pfam) = row
            Next
            Return Tuple.Create(cHeader, weights)
        End Function

        ''' <summary>
        ''' Load one phenotype's non-zero+weights file.
        ''' Returns:
        '''   cHeader  - list of C values parsed from the file's own header
        '''   features - list of (Pfam, class +/-, weights-by-C, description, Pearson r)
        ''' </summary>
        Public Function LoadNonZeroWeightsFile(path As String) _
            As Tuple(Of List(Of Double),
                     List(Of Tuple(Of String, String, List(Of Double), String, Double)))
            Dim cHeader As New List(Of Double)()
            Dim result As New List(Of Tuple(Of String, String, List(Of Double), String, Double))()
            If Not File.Exists(path) Then Return Tuple.Create(cHeader, result)

            Dim firstLine As Boolean = True
            For Each line As String In File.ReadLines(path)
                If String.IsNullOrEmpty(line) Then Continue For
                Dim parts As String() = line.Split(New Char() {ControlChars.Tab}, StringSplitOptions.None)
                If firstLine Then
                    firstLine = False
                    ' Parse C columns from the header (tokens ending in "_0")
                    For j As Integer = 2 To parts.Length - 1
                        Dim tok As String = parts(j).Trim()
                        If tok.Length = 0 Then Continue For
                        ' Stop at "pfam_desc" or "cor" columns
                        If tok.Equals("pfam_desc", StringComparison.OrdinalIgnoreCase) OrElse
                           tok.Equals("cor", StringComparison.OrdinalIgnoreCase) Then Exit For
                        Dim cStr As String = tok
                        Dim usIdx As Integer = tok.IndexOf("_"c)
                        If usIdx > 0 Then cStr = tok.Substring(0, usIdx)
                        Dim c As Double = 0.0R
                        If Double.TryParse(cStr, c) Then cHeader.Add(c)
                    Next
                    Continue For
                End If
                If parts.Length < 2 + cHeader.Count Then Continue For

                Dim pfam As String = parts(0).Trim()
                Dim cls As String = parts(1).Trim()
                Dim w As New List(Of Double)()
                For j As Integer = 0 To cHeader.Count - 1
                    Dim v As Double = 0.0R
                    Double.TryParse(parts(2 + j).Trim(), v)
                    w.Add(v)
                Next

                ' Description: from index (2 + nC) up to (last - 1) joined with spaces
                Dim descStart As Integer = 2 + cHeader.Count
                Dim corIdx As Integer = parts.Length - 1
                Dim desc As New System.Text.StringBuilder()
                For k As Integer = descStart To corIdx - 1
                    If k >= parts.Length Then Exit For
                    If desc.Length > 0 Then desc.Append(" "c)
                    desc.Append(parts(k).Trim())
                Next
                Dim cor As Double = 0.0R
                If corIdx >= 0 AndAlso corIdx < parts.Length Then
                    Double.TryParse(parts(corIdx).Trim(), cor)
                End If
                result.Add(Tuple.Create(pfam, cls, w, desc.ToString(), cor))
            Next
            Return Tuple.Create(cHeader, result)
        End Function

        ''' <summary>
        ''' Load a complete PhenotypeModel from the three per-phenotype files
        ''' in a directory.
        ''' </summary>
        Public Function LoadPhenotypeModel(modelDir As String,
                                            phenotypeId As String,
                                            Optional pfamDescCatalog As Dictionary(Of String, String) = Nothing) _
                                            As PhenotypeModel
            Dim model As New PhenotypeModel()
            model.PhenotypeId = phenotypeId

            Dim biasPath As String = Path.Combine(modelDir, phenotypeId & "_bias.txt")
            Dim featsPath As String = Path.Combine(modelDir, phenotypeId & "_feats.txt")
            Dim nzPath As String = Path.Combine(modelDir, phenotypeId & "_non-zero+weights.txt")

            ' 1. Bias file (accuracy-ordered list of (C, bias) pairs)
            Dim biasPairs As List(Of Tuple(Of Double, Double)) = LoadBiasFile(biasPath)

            ' 2. Non-zero+weights file (authoritative source for non-zero weights;
            '    its C column order matches the bias file's accuracy order)
            Dim nzTuple As Tuple(Of List(Of Double),
                                 List(Of Tuple(Of String, String, List(Of Double), String, Double))) =
                LoadNonZeroWeightsFile(nzPath)
            Dim nzCHeader As List(Of Double) = nzTuple.Item1
            Dim nzFeatures As List(Of Tuple(Of String, String, List(Of Double), String, Double)) = nzTuple.Item2

            ' 3. Feats file (fallback: some Pfams may only appear here)
            Dim featsTuple As Tuple(Of List(Of Double),
                                    Dictionary(Of String, Dictionary(Of Double, Double))) =
                LoadFeatsFile(featsPath)
            Dim featsCHeader As List(Of Double) = featsTuple.Item1
            Dim featsWeights As Dictionary(Of String, Dictionary(Of Double, Double)) = featsTuple.Item2

            ' 4. Build SubModel objects (one per C value in the bias file)
            For Each bp As Tuple(Of Double, Double) In biasPairs
                Dim sm As New SubModel()
                sm.C = bp.Item1
                sm.Bias = bp.Item2

                ' 4a. Fill sparse weights from the non-zero+weights file (preferred)
                For Each nz As Tuple(Of String, String, List(Of Double), String, Double) In nzFeatures
                    For j As Integer = 0 To Math.Min(nzCHeader.Count, nz.Item3.Count) - 1
                        If Math.Abs(nzCHeader(j) - sm.C) < 0.0000001R AndAlso nz.Item3(j) <> 0.0R Then
                            sm.Weights(nz.Item1) = nz.Item3(j)
                            Exit For
                        End If
                    Next
                Next

                ' 4b. Supplement with any weights from the feats file not already set
                For Each kv As KeyValuePair(Of String, Dictionary(Of Double, Double)) In featsWeights
                    If sm.Weights.ContainsKey(kv.Key) Then Continue For
                    If kv.Value.ContainsKey(sm.C) Then
                        Dim w As Double = kv.Value(sm.C)
                        If w <> 0.0R Then sm.Weights(kv.Key) = w
                    End If
                Next

                model.SubModels.Add(sm)
            Next

            ' 5. Store feature metadata (description, correlation) from non-zero+weights
            For Each nz As Tuple(Of String, String, List(Of Double), String, Double) In nzFeatures
                Dim f As New PhenotypeFeature()
                f.PfamAcc = nz.Item1
                f.WeightClass = nz.Item2
                f.Description = nz.Item4
                f.PearsonCorrelation = nz.Item5
                ' Fill per-C weights
                For i As Integer = 0 To Math.Min(nzCHeader.Count, nz.Item3.Count) - 1
                    f.WeightsByC(nzCHeader(i)) = nz.Item3(i)
                Next
                ' If description is empty, fall back to the global catalog
                If f.Description.Length = 0 AndAlso pfamDescCatalog IsNot Nothing AndAlso
                   pfamDescCatalog.ContainsKey(f.PfamAcc) Then
                    f.Description = pfamDescCatalog(f.PfamAcc)
                End If
                model.Features.Add(f)
            Next

            Return model
        End Function

        ''' <summary>
        ''' Load all phenotype models found in a directory.
        ''' Returns a list of PhenotypeModel objects.
        ''' </summary>
        Public Function LoadAllModels(modelDir As String) As List(Of PhenotypeModel)
            Dim models As New List(Of PhenotypeModel)()
            If Not Directory.Exists(modelDir) Then Return models

            ' Load the global catalogs first
            Dim ptCatalog As Dictionary(Of String, Tuple(Of String, String)) =
                LoadPhenotypeCatalog(Path.Combine(modelDir, "pt2acc.txt"))
            Dim pfamCatalog As Dictionary(Of String, String) =
                LoadPfamCatalog(Path.Combine(modelDir, "pf2acc_desc.txt"))

            ' Find all phenotype IDs (files named {id}_bias.txt)
            Dim ids As New HashSet(Of String)()
            For Each f As String In Directory.GetFiles(modelDir, "*_bias.txt")
                Dim fn As String = Path.GetFileNameWithoutExtension(f)
                Dim id As String = fn.Replace("_bias", "")
                ids.Add(id)
            Next

            For Each id As String In ids
                Dim m As PhenotypeModel = LoadPhenotypeModel(modelDir, id, pfamCatalog)
                If ptCatalog.ContainsKey(id) Then
                    m.PhenotypeName = ptCatalog(id).Item1
                    m.Category = ptCatalog(id).Item2
                End If
                models.Add(m)
            Next

            ' Sort by phenotype ID for deterministic output
            models.Sort(Function(a, b) String.Compare(a.PhenotypeId, b.PhenotypeId, StringComparison.Ordinal))
            Return models
        End Function

        ''' <summary>
        ''' Load all models directly from a .zip archive (no extraction needed).
        ''' Reads each {pid}_*.txt entry from the zip.
        ''' </summary>
        Public Function LoadAllModelsFromZip(zipPath As String) As List(Of PhenotypeModel)
            ' Extract to a temp directory and reuse the directory loader
            Dim tempDir As String = Path.Combine(Path.GetTempPath(), "traitar_models_" &
                                                 Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(tempDir)
            Try
                Using archive As ZipArchive = ZipFile.OpenRead(zipPath)
                    For Each entry As ZipArchiveEntry In archive.Entries
                        Dim dest As String = Path.Combine(tempDir, entry.Name)
                        entry.ExtractToFile(dest, True)
                    Next
                End Using
                Return LoadAllModels(tempDir)
            Finally
                ' Best-effort cleanup
                Try
                    Directory.Delete(tempDir, True)
                Catch
                End Try
            End Try
        End Function

    End Module

End Namespace
