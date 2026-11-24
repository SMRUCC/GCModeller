Option Strict On

Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.gwANI
Imports SMRUCC.genomics.SequenceModel.FASTA


    ''' <summary>
    ''' The gwANI self test and benchmark entry point.
    '''
    ''' ```
    ''' dotnet GwaniSelfTest.dll selftest
    ''' dotnet GwaniSelfTest.dll bench [--n 200000] [--len 25000] [--rows 512] [--seed 42]
    '''                                [--mmap --budget 1073741824] [--sketch] [--dedup] [--baseline]
    ''' ```
    ''' </summary>
    Module Program

        Dim _passed As Integer = 0
        Dim _failed As Integer = 0

        ''' <summary> The workspace root of the test data. </summary>
        Private ReadOnly DataDirectory As String = "g:/GCModeller/src/GCModeller/analysis/SequenceToolkit/data/gwANI"

        ''' <summary> Serializes the progress callback of the engine. </summary>
        Private ReadOnly ReportLock As New Object()

        Private Sub ReportProgress(message As String)
            SyncLock ReportLock
                Console.WriteLine($"  {message}")
            End SyncLock
        End Sub

        ''' <summary>
        ''' Splits the synthetic dataset into several divergent clusters, which is what
        ''' the sketch pre-filter needs in order to have pairs to skip.
        ''' </summary>
        Private Iterator Function ClusteredSequences(count As Integer, alignmentLength As Integer, seed As Integer, clusters As Integer) As IEnumerable(Of FastaSeq)
            Dim per As Integer = count \ clusters

            For c As Integer = 0 To clusters - 1
                Dim take As Integer = If(c = clusters - 1, count - per * (clusters - 1), per)

                For Each seq As FastaSeq In SyntheticMsa.Generate(
                        take, alignmentLength, seed + c * 7919,
                        substitutionRate:=0.05, gapRate:=0.01, unknownRate:=0.005,
                        namePrefix:=$"c{c}")
                    Yield seq
                Next
            Next
        End Function

        Function Main(args As String()) As Integer
            Dim command As String = If(args Is Nothing OrElse args.Length = 0, "selftest", args(0).ToLowerInvariant())

            Select Case command
                Case "selftest"
                    Return RunSelfTest()
                Case "bench"
                    Return RunBench(args)
                Case "kernel"
                    Return RunKernelBench(args)
                Case Else
                    Console.WriteLine($"unknown command '{command}', expected 'selftest' or 'bench'.")
                    Return 2
            End Select
        End Function

#Region "assertion helpers"

        Private Sub Assert(condition As Boolean, message As String)
            If condition Then
                _passed += 1
            Else
                _failed += 1
                Console.WriteLine($"  [FAIL] {message}")
            End If
        End Sub

        Private Sub AssertClose(expected As Double, actual As Double, message As String)
            Assert(Math.Abs(expected - actual) < 0.000001, $"{message}: expected {expected:F6}, got {actual:F6}")
        End Sub

        Private Sub AssertSameMatrix(reference As Double(,), rows As List(Of GwaniRow), tag As String)
            Dim names As Integer = reference.GetLength(0)

            Assert(rows.Count = names, $"{tag}: row count {rows.Count} != {names}")

            For i As Integer = 0 To Math.Min(rows.Count, names) - 1
                Dim values As Double() = rows(i).Identities

                Assert(values.Length = names, $"{tag}: row {i} has {values.Length} columns, expected {names}")

                For j As Integer = 0 To names - 1
                    AssertClose(reference(i, j), values(j), $"{tag}: cell ({i},{j})")
                Next
            Next
        End Sub

#End Region

#Region "self test"

        Private Function RunSelfTest() As Integer
            Console.WriteLine("gwANI self test")
            Console.WriteLine("===============")

            RunCase("the panito documentation example", AddressOf TestDocumentationExample)
            RunCase("the bundled test.txt alignment", AddressOf TestBundledAlignment)
            RunCase("the differential test against the scalar reference", AddressOf TestDifferential)
            RunCase("the edge cases", AddressOf TestEdgeCases)
            RunCase("the deduplication", AddressOf TestDeduplication)
            RunCase("the memory mapped backend", AddressOf TestMemoryMappedBackend)
            RunCase("the parallel determinism", AddressOf TestParallelDeterminism)
            RunCase("the sketch pre-filter", AddressOf TestSketchPreFilter)
            RunCase("the csv writer", AddressOf TestCsvWriter)

            Console.WriteLine()
            Console.WriteLine($"passed: {_passed}, failed: {_failed}")

            Return If(_failed = 0, 0, 1)
        End Function

        Private Sub RunCase(title As String, body As Action)
            Console.WriteLine()
            Console.WriteLine($"* {title}")

            Dim before As Integer = _failed

            Try
                body()
            Catch ex As Exception
                _failed += 1
                Console.WriteLine($"  [FAIL] unhandled exception: {ex}")
            End Try

            If _failed = before Then
                Console.WriteLine("  ok")
            End If
        End Sub

        ''' <summary>
        ''' The example of the panito documentation: 100 / 90 / 80.
        ''' </summary>
        Private Sub TestDocumentationExample()
            Dim doc As New FastaFile({
                New FastaSeq("AAAAAAAAAA", "sample1"),
                New FastaSeq("AAAAAAAAAC", "sample2"),
                New FastaSeq("AAAAAAAACC", "sample3")
            })

            Dim matrix As DataSet() = GwaniCalculator.Evaluate(doc)
            Dim names As String() = {"sample1", "sample2", "sample3"}
            Dim expected As Double(,) = {
                {100.0, 90.0, 80.0},
                {-1.0, 100.0, 90.0},
                {-1.0, -1.0, 100.0}
            }

            Assert(matrix.Length = 3, $"the documentation example should produce 3 rows, got {matrix.Length}")

            For i As Integer = 0 To 2
                Assert(matrix(i).ID = names(i), $"row {i} id is '{matrix(i).ID}', expected '{names(i)}'")

                For j As Integer = 0 To 2
                    AssertClose(expected(i, j), matrix(i)(names(j)), $"documentation cell ({i},{j})")
                Next
            Next
        End Sub

        ''' <summary>
        ''' The bundled three sample alignment must reproduce the documented values.
        ''' </summary>
        Private Sub TestBundledAlignment()
            Dim testPath As String = Path.Combine(DataDirectory, "test.txt")

            If Not System.IO.File.Exists(testPath) Then
                Console.WriteLine($"  [skip] {testPath} not found")
                Return
            End If

            ' NOTE: the FastaFile(path) constructor of Bio.Assembly is broken(it casts
            ' the DocParser iterator straight into a List), so the working Read helper
            ' is used here
            Dim msa As FastaFile = FastaFile.Read(testPath)
            Dim matrix As DataSet() = GwaniCalculator.Evaluate(msa)

            Assert(matrix.Length = 3, $"test.txt should produce 3 rows, got {matrix.Length}")
            AssertClose(90.0, matrix(0)("sample2"), "test.txt cell (0,1)")
            AssertClose(80.0, matrix(0)("sample3"), "test.txt cell (0,2)")
            AssertClose(90.0, matrix(1)("sample3"), "test.txt cell (1,2)")
        End Sub

        ''' <summary>
        ''' A seeded random alignment, including the lower case, the gaps, the unknowns
        ''' and the IUPAC ambiguity codes, is compared cell by cell against the scalar
        ''' reference implementation.
        ''' </summary>
        Private Sub TestDifferential()
            Dim msa As FastaFile = BuildRandomAlignment(count:=60, alignmentLength:=1000, seed:=20261020)

            ' the ambiguity codes are ordinary symbols for panito, so 'R' mismatches 'A'
            Dim rows As List(Of FastaSeq) = New List(Of FastaSeq)(msa)

            For i As Integer = 5 To 9
                Dim data As String = rows(i).SequenceData

                If data.Length > 40 Then
                    data = data.Remove(10, 1).Insert(10, "R")
                    data = data.Remove(20, 1).Insert(20, "r")
                    data = data.Remove(30, 1).Insert(30, "Y")
                    rows(i) = New FastaSeq(data, rows(i).Title)
                End If
            Next

            Dim patched As New FastaFile(rows)
            Dim reference As GwaniReference = GwaniReference.Load(patched)
            Dim engine As List(Of GwaniRow) = ComputeAll(patched, New GwaniOptions())

            AssertSameMatrix(reference.ComputeMatrix(), engine, "differential")
        End Sub

        ''' <summary>
        ''' The padding boundaries, the empty alignments and the all unknown alignments.
        ''' </summary>
        Private Sub TestEdgeCases()
            For Each siteCount As Integer In {1, 31, 32, 33, 63, 64, 65}
                Dim msa As FastaFile = SyntheticMsa.GenerateFile(count:=12, alignmentLength:=siteCount, seed:=1000 + siteCount)
                Dim reference As GwaniReference = GwaniReference.Load(msa)
                Dim engine As List(Of GwaniRow) = ComputeAll(msa, New GwaniOptions())

                AssertSameMatrix(reference.ComputeMatrix(), engine, $"length={siteCount}")
            Next

            ' a single sequence
            Dim solo As FastaFile = SyntheticMsa.GenerateFile(count:=1, alignmentLength:=77, seed:=5)
            Dim soloReference As GwaniReference = GwaniReference.Load(solo)
            Dim soloEngine As List(Of GwaniRow) = ComputeAll(solo, New GwaniOptions())

            Assert(soloEngine.Count = 1, $"a single sequence should produce a single row, got {soloEngine.Count}")
            AssertClose(100.0, soloEngine(0).Identities(0), "the single sequence diagonal")

            ' everything unknown, so every pair has an empty denominator
            Dim unknown As New FastaFile({
                New FastaSeq(New String("N"c, 50), "a"),
                New FastaSeq(New String("-"c, 50), "b"),
                New FastaSeq(New String("n"c, 50), "c")
            })
            Dim unknownReference As GwaniReference = GwaniReference.Load(unknown)
            Dim unknownEngine As List(Of GwaniRow) = ComputeAll(unknown, New GwaniOptions())

            AssertSameMatrix(unknownReference.ComputeMatrix(), unknownEngine, "all unknown")

            For i As Integer = 0 To 2
                For j As Integer = i + 1 To 2
                    AssertClose(0.0, unknownEngine(i).Identities(j), $"the all unknown pair ({i},{j})")
                Next
            Next
        End Sub

        ''' <summary>
        ''' The deduplication must collapse the identical sequences and must not change
        ''' the expanded result.
        ''' </summary>
        Private Sub TestDeduplication()
            Dim msa As FastaFile = SyntheticMsa.GenerateFile(count:=40, alignmentLength:=400, seed:=77, duplicateFraction:=0.5)
            Dim dedup As New GwaniOptions With {.EnableDeduplication = True}
            Dim classic As New GwaniOptions With {.EnableDeduplication = False}
            Dim reference As GwaniReference = GwaniReference.Load(msa)

            Using alignment As GwaniAlignment = GwaniAlignment.Build(msa, dedup)
                Assert(alignment.HasDuplicates, "the synthetic alignment should contain duplicates")
                Assert(alignment.UniqueCount < 40, $"the deduplication should collapse the duplicates, unique = {alignment.UniqueCount}")

                Dim rows As List(Of GwaniRow) = New List(Of GwaniRow)(GwaniCalculator.ComputeRows(alignment, dedup))

                Assert(rows.Count = alignment.UniqueCount, $"the deduplicated matrix should have {alignment.UniqueCount} rows, got {rows.Count}")

                ' the representative rows must agree with the scalar reference
                For r As Integer = 0 To rows.Count - 1
                    Dim input As Integer = alignment.FirstOccurrence(r)

                    Assert(rows(r).ID = reference.SequenceNames(input), $"representative {r} id mismatch")

                    For j As Integer = r + 1 To rows.Count - 1
                        Dim expected As Double = reference.IdentityOf(input, alignment.FirstOccurrence(j))

                        AssertClose(expected, rows(r).Identities(j), $"deduplicated cell ({r},{j})")
                    Next
                Next
            End Using

            ' the expansion of the deduplicated result must equal the classic matrix
            Dim classicEngine As List(Of GwaniRow) = ComputeAll(msa, classic)

            AssertSameMatrix(reference.ComputeMatrix(), classicEngine, "deduplication expansion")
        End Sub

        ''' <summary>
        ''' A tiny memory budget forces the memory mapped backend, the result must not
        ''' change.
        ''' </summary>
        Private Sub TestMemoryMappedBackend()
            Dim msa As FastaFile = SyntheticMsa.GenerateFile(count:=50, alignmentLength:=600, seed:=31337, duplicateFraction:=0.2)
            Dim mapped As New GwaniOptions With {
                .MemoryBudgetBytes = 4096,
                .ScratchDirectory = Path.GetTempPath()
            }

            Using alignment As GwaniAlignment = GwaniAlignment.Build(msa, mapped)
                Assert(alignment.Store.Backend = GwaniStoreBackend.MemoryMapped,
                    $"a 4KB budget should spill into the memory mapped backend, got {alignment.Store.Backend}")
            End Using

            Dim reference As GwaniReference = GwaniReference.Load(msa)
            Dim engine As List(Of GwaniRow) = ComputeAll(msa, mapped)

            AssertSameMatrix(reference.ComputeMatrix(), engine, "memory mapped")
        End Sub

        ''' <summary>
        ''' The parallel sweep must be deterministic.
        ''' </summary>
        Private Sub TestParallelDeterminism()
            Dim msa As FastaFile = SyntheticMsa.GenerateFile(count:=70, alignmentLength:=900, seed:=4242)
            Dim singleWorker As New GwaniOptions With {.MaxDegreeOfParallelism = 1}
            Dim multiWorker As New GwaniOptions With {.MaxDegreeOfParallelism = Environment.ProcessorCount}

            Dim a As List(Of GwaniRow) = ComputeAll(msa, singleWorker)
            Dim b As List(Of GwaniRow) = ComputeAll(msa, multiWorker)

            Assert(a.Count = b.Count, $"the row count differs: {a.Count} vs {b.Count}")

            For i As Integer = 0 To a.Count - 1
                For j As Integer = 0 To a(i).Identities.Length - 1
                    Assert(a(i).Identities(j) = b(i).Identities(j), $"the parallel result differs at cell ({i},{j})")
                Next
            Next
        End Sub

        ''' <summary>
        ''' The sketch pre-filter may skip a pair, but it must never change a pair that
        ''' is comfortably above the threshold.
        ''' </summary>
        Private Sub TestSketchPreFilter()
            ' two divergent clusters, the within cluster identity is high and the
            ' cross cluster identity is low
            Dim msa As FastaFile = BuildTwoClusterAlignment(countPerCluster:=25, alignmentLength:=1200, seedA:=11, seedB:=999)
            Dim reference As GwaniReference = GwaniReference.Load(msa)
            Dim sketch As New GwaniOptions With {
                .EnableSketchPreFilter = True,
                .MinIdentityThreshold = 0.8
            }

            Using alignment As GwaniAlignment = GwaniAlignment.Build(msa, sketch)
                Dim rows As List(Of GwaniRow) = New List(Of GwaniRow)(GwaniCalculator.ComputeRows(alignment, sketch))
                Dim exact As Double(,) = reference.ComputeMatrix()
                Dim skipped As Integer = 0
                Dim missed As Integer = 0
                Dim sharing As Integer = 0

                ' a direct "shares at least one sampled k-mer" count, used to cross
                ' check the LSH candidate generation
                Dim sketches As ULong()() = GwaniSketch.Build(alignment.Store, alignment.AlignmentLength, sketch)

                For i As Integer = 0 To alignment.UniqueCount - 1
                    For j As Integer = i + 1 To alignment.UniqueCount - 1
                        If GwaniKernels.SketchMatches(sketches(i), sketches(j)) > 0 Then
                            sharing += 1
                        End If
                    Next
                Next

                Console.WriteLine($"  sharing at least one sampled k-mer: {sharing} pairs")

                For i As Integer = 0 To rows.Count - 1
                    For j As Integer = i + 1 To rows.Count - 1
                        Dim value As Double = rows(i).Identities(j)

                        If value = GwaniValues.Skipped Then
                            skipped += 1
                        End If

                        ' a pair that is clearly above the threshold must never be
                        ' skipped and must be exact
                        If exact(i, j) >= 85.0 Then
                            If value = GwaniValues.Skipped Then
                                missed += 1
                            Else
                                AssertClose(exact(i, j), value, $"sketch cell ({i},{j})")
                            End If
                        End If
                    Next
                Next

                Console.WriteLine($"  sketch filter: {skipped} pairs skipped, {missed} missed above the threshold")
                Assert(missed = 0, $"the sketch filter skipped {missed} pairs that are above the threshold")
            End Using
        End Sub

        ''' <summary>
        ''' The tab separated matrix writer must produce a readable matrix.
        ''' </summary>
        Private Sub TestCsvWriter()
            Dim msa As FastaFile = SyntheticMsa.GenerateFile(count:=8, alignmentLength:=300, seed:=808)
            Dim inPath As String = Path.Combine(Path.GetTempPath(), $"gwANI-selftest-{Guid.NewGuid():N}.fasta")
            Dim outPath As String = Path.Combine(Path.GetTempPath(), $"gwANI-selftest-{Guid.NewGuid():N}.csv")

            Try
                Using writer As New System.IO.StreamWriter(inPath, append:=False, encoding:=Encoding.UTF8)
                    For Each seq As FastaSeq In msa
                        writer.WriteLine($">{seq.Title}")
                        writer.WriteLine(seq.SequenceData)
                    Next
                End Using

                GwaniCalculator.EvaluateToCsv(inPath, outPath, New GwaniOptions())

                Dim lines As String() = System.IO.File.ReadAllLines(outPath)

                Assert(lines.Length = 9, $"the csv should have a header plus 8 rows, got {lines.Length} lines")
                Assert(lines(0).StartsWith("sample_"), $"the csv header should start with a sample id, got '{lines(0)}'")

                Dim first As String() = lines(1).Split(vbTab)
                Dim second As String() = lines(2).Split(vbTab)

                Assert(first.Length = 9, $"the first csv row should have 9 cells, got {first.Length}")
                Assert(first(0) = "sample_000001", $"the first csv row id is '{first(0)}'")
                ' the first row has no lower triangle, its diagonal is the first cell
                Assert(first(1) = "100.000000", $"the csv diagonal should be '100.000000', got '{first(1)}'")
                Assert(second(1) = "-", $"the csv lower triangle should be '-', got '{second(1)}'")

                Dim reference As GwaniReference = GwaniReference.Load(msa)
                Dim expected As Double = reference.IdentityOf(0, 1)

                AssertClose(expected, Double.Parse(first(2), NumberStyles.Float, CultureInfo.InvariantCulture), "the csv cell (0,1)")
            Finally
                If System.IO.File.Exists(outPath) Then
                    System.IO.File.Delete(outPath)
                End If
                If System.IO.File.Exists(inPath) Then
                    System.IO.File.Delete(inPath)
                End If
            End Try
        End Sub

#End Region

#Region "helpers"

        ''' <summary>
        ''' Runs the engine and materializes every row. The deduplication is forced off
        ''' here, so that the emitted matrix is the classic input ordered one which can
        ''' be compared with the scalar reference cell by cell.
        ''' </summary>
        Private Function ComputeAll(msa As FastaFile, options As GwaniOptions) As List(Of GwaniRow)
            Dim classic As GwaniOptions = options.CloneWithoutDeduplication()

            Using alignment As GwaniAlignment = GwaniAlignment.Build(msa, classic)
                Return New List(Of GwaniRow)(GwaniCalculator.ComputeRows(alignment, classic))
            End Using
        End Function

        ''' <summary>
        ''' A seeded random alignment with the mixed case, the gaps and the unknowns.
        ''' </summary>
        Private Function BuildRandomAlignment(count As Integer, alignmentLength As Integer, seed As Integer) As FastaFile
            Return SyntheticMsa.GenerateFile(
                count:=count,
                alignmentLength:=alignmentLength,
                seed:=seed,
                substitutionRate:=0.06,
                gapRate:=0.02,
                unknownRate:=0.01,
                duplicateFraction:=0.15)
        End Function

        ''' <summary>
        ''' Two divergent clusters of sequences.
        ''' </summary>
        Private Function BuildTwoClusterAlignment(countPerCluster As Integer, alignmentLength As Integer, seedA As Integer, seedB As Integer) As FastaFile
            Dim first As List(Of FastaSeq) = New List(Of FastaSeq)(
                SyntheticMsa.Generate(countPerCluster, alignmentLength, seedA, substitutionRate:=0.03))
            Dim second As List(Of FastaSeq) = New List(Of FastaSeq)(
                SyntheticMsa.Generate(countPerCluster, alignmentLength, seedB, substitutionRate:=0.03))
            Dim all As New List(Of FastaSeq)(first)

            all.AddRange(second)

            Return New FastaFile(all)
        End Function

#End Region

#Region "kernel micro benchmark"

        ''' <summary>
        ''' Measures the raw throughput of the vectorized pair kernel, without the
        ''' engine around it.
        ''' </summary>
        Private Function RunKernelBench(args As String()) As Integer
            Dim length As Integer = ReadInt(args, "--len", 25000)
            Dim rounds As Integer = ReadInt(args, "--rounds", 20000)
            Dim threads As Integer = ReadInt(args, "--threads", -1)

            Dim padded As Integer = NucleotideCodec.PaddedLength(length)
            Dim random As New Random(20261020)
            Dim a As Byte() = New Byte(padded - 1) {}
            Dim b As Byte() = New Byte(padded - 1) {}

            For i As Integer = 0 To padded - 1
                a(i) = CByte(AscW("ACGT"(random.Next(4))))
                b(i) = CByte(AscW("ACGT"(random.Next(4))))
            Next

            Console.WriteLine("gwANI kernel micro benchmark")
            Console.WriteLine("============================")
            Console.WriteLine($"row length     : {padded} bytes")
            Console.WriteLine($"rounds         : {rounds}")
            Console.WriteLine($"AVX2 supported : {System.Runtime.Intrinsics.X86.Avx2.IsSupported}")
            Console.WriteLine()

            ' serial
            Dim watch As Stopwatch = Stopwatch.StartNew()
            Dim total As Long = 0

            For r As Integer = 0 To rounds - 1
                Dim m As Integer = 0
                Dim v As Integer = 0

                GwaniKernels.CountPair(a, b, padded, m, v)
                total += m + v
            Next

            watch.Stop()

            Dim serialSeconds As Double = watch.Elapsed.TotalSeconds

            Console.WriteLine($"serial kernel  : {serialSeconds:F3}s, {rounds * padded * 2.0 / 1073741824.0 / serialSeconds:N2} GB/s")

            ' parallel
            watch.Restart()

            System.Threading.Tasks.Parallel.For(
                0, rounds,
                New System.Threading.Tasks.ParallelOptions With {.MaxDegreeOfParallelism = If(threads > 0, threads, Environment.ProcessorCount)},
                Sub(r)
                    Dim m As Integer = 0
                    Dim v As Integer = 0

                    GwaniKernels.CountPair(a, b, padded, m, v)
                End Sub)

            watch.Stop()

            Dim parallelSeconds As Double = watch.Elapsed.TotalSeconds

            Console.WriteLine($"parallel       : {parallelSeconds:F3}s, {rounds * padded * 2.0 / 1073741824.0 / parallelSeconds:N2} GB/s")
            Console.WriteLine($"checksum       : {total}")

            Return 0
        End Function

#End Region

#Region "benchmark"

        Private Function RunBench(args As String()) As Integer
            Dim n As Integer = ReadInt(args, "--n", 200000)
            Dim alignmentLength As Integer = ReadInt(args, "--len", 25000)
            Dim quota As Integer = ReadInt(args, "--rows", 512)
            Dim seed As Integer = ReadInt(args, "--seed", 42)
            Dim threads As Integer = ReadInt(args, "--threads", -1)
            Dim budget As Long = ReadLong(args, "--budget", 32L * 1024 * 1024 * 1024)
            Dim forceMapped As Boolean = Array.IndexOf(args, "--mmap") >= 0
            Dim useSketch As Boolean = Array.IndexOf(args, "--sketch") >= 0
            Dim dedup As Boolean = Array.IndexOf(args, "--dedup") >= 0
            Dim baseline As Boolean = Array.IndexOf(args, "--baseline") >= 0

            Dim options As New GwaniOptions With {
                .MaxDegreeOfParallelism = threads,
                .MemoryBudgetBytes = If(forceMapped, Math.Min(budget, 1024L * 1024), budget),
                .EnableDeduplication = dedup,
                .EnableSketchPreFilter = useSketch,
                .ScratchDirectory = Path.GetTempPath()
            }

            Console.WriteLine("gwANI benchmark")
            Console.WriteLine("===============")
            Console.WriteLine($"sequences      : {n}")
            Console.WriteLine($"alignment      : {alignmentLength} sites (padded {NucleotideCodec.PaddedLength(alignmentLength)})")
            Console.WriteLine($"all pairs      : {CLng(n) * (n - 1) \ 2:N0}")
            Console.WriteLine($"workers        : {options.ResolveParallelism()}")
            Console.WriteLine($"backend        : {If(forceMapped, "memory mapped (forced)", "in memory / auto spill")}")
            Console.WriteLine($"deduplication  : {dedup}")
            Console.WriteLine($"sketch filter  : {useSketch}")
            Console.WriteLine()

            ' 1. build the encoded store from the lazily generated dataset
            Dim clusters As Integer = ReadInt(args, "--clusters", 1)
            Dim sequences As IEnumerable(Of FastaSeq)

            If clusters <= 1 Then
                sequences = SyntheticMsa.Generate(n, alignmentLength, seed, substitutionRate:=0.05, gapRate:=0.01, unknownRate:=0.005)
            Else
                sequences = ClusteredSequences(n, alignmentLength, seed, clusters)
            End If

            If options.Progress Is Nothing Then
                options.Progress = AddressOf ReportProgress
            End If

            Dim watch As Stopwatch = Stopwatch.StartNew()
            Dim alignment As GwaniAlignment = GwaniAlignment.Build(sequences, options)
            watch.Stop()

            Console.WriteLine($"encode         : {watch.Elapsed.TotalSeconds:F2}s")
            Console.WriteLine($"stored rows    : {alignment.UniqueCount} (deduplication collapsed {n - alignment.UniqueCount})")
            Console.WriteLine($"store backend  : {alignment.Store.Backend}")
            Console.WriteLine($"store bytes    : {alignment.Store.TotalBytes / 1024 / 1024:N0} MB")
            Console.WriteLine($"working set    : {PeakWorkingSetMb():N0} MB")
            Console.WriteLine()

            ' 2. compute a quota of the rows and report the throughput
            watch.Restart()

            Dim computed As Long = 0
            Dim emitted As Integer = 0

            For Each row As GwaniRow In GwaniCalculator.ComputeRows(alignment, options)
                computed += alignment.UniqueCount - row.RowIndex - 1
                emitted += 1

                If emitted >= quota Then
                    Exit For
                End If
            Next

            watch.Stop()

            Dim seconds As Double = watch.Elapsed.TotalSeconds
            Dim totalPairs As Long = CLng(alignment.UniqueCount) * (alignment.UniqueCount - 1) \ 2
            Dim kernelBytes As Double = computed * alignment.Store.PaddedLength * 2.0

            Console.WriteLine($"computed rows  : {emitted} of {alignment.UniqueCount}")
            Console.WriteLine($"pairs          : {computed:N0}")
            Console.WriteLine($"elapsed        : {seconds:F2}s")
            Console.WriteLine($"throughput     : {computed / Math.Max(seconds, 0.000001) / 1000000:N2} M pairs/s")
            Console.WriteLine($"kernel traffic : {kernelBytes / 1073741824.0 / Math.Max(seconds, 0.000001):N2} GB/s")
            Console.WriteLine($"peak working   : {PeakWorkingSetMb():N0} MB")

            If computed > 0 Then
                Dim full As Double = totalPairs / (computed / Math.Max(seconds, 0.000001))

                Console.WriteLine($"extrapolated   : the full {totalPairs:N0} pair matrix would need {TimeSpan.FromSeconds(full):g}")
            End If

            Console.WriteLine()

            ' 3. the scalar reference baseline on a much smaller dataset
            If baseline Then
                RunBaseline()
            End If

            alignment.Dispose()

            Return 0
        End Function

        Private Sub RunBaseline()
            Dim n As Integer = 1000
            Dim length As Integer = 10000
            Dim msa As FastaFile = SyntheticMsa.GenerateFile(n, length, 4242, substitutionRate:=0.05, gapRate:=0.01, unknownRate:=0.005)
            Dim reference As GwaniReference = GwaniReference.Load(msa)
            Dim watch As Stopwatch = Stopwatch.StartNew()

            Dim matrix As Double(,) = reference.ComputeMatrix()

            watch.Stop()

            Dim pairs As Long = CLng(n) * (n - 1) \ 2
            Dim seconds As Double = watch.Elapsed.TotalSeconds

            Console.WriteLine($"baseline       : {n} x {length}, {pairs:N0} pairs")
            Console.WriteLine($"scalar elapsed : {seconds:F2}s")
            Console.WriteLine($"scalar rate    : {pairs / Math.Max(seconds, 0.000001) / 1000:N2} K pairs/s")

            ' the same dataset through the vectorized engine
            Dim options As New GwaniOptions With {.MaxDegreeOfParallelism = 1}
            Dim engineWatch As Stopwatch = Stopwatch.StartNew()

            Using alignment As GwaniAlignment = GwaniAlignment.Build(msa, options)
                Dim rows As New List(Of GwaniRow)(GwaniCalculator.ComputeRows(alignment, options))

                engineWatch.Stop()

                Dim engineSeconds As Double = engineWatch.Elapsed.TotalSeconds

                Console.WriteLine($"vector elapsed : {engineSeconds:F2}s (single worker)")
                Console.WriteLine($"speedup        : {seconds / Math.Max(engineSeconds, 0.000001):N2}x")

                Dim mismatch As Integer = 0

                For i As Integer = 0 To n - 1
                    For j As Integer = i + 1 To n - 1
                        If Math.Abs(matrix(i, j) - rows(i).Identities(j)) > 0.000001 Then
                            mismatch += 1
                        End If
                    Next
                Next

                Console.WriteLine($"mismatches     : {mismatch}")
            End Using
        End Sub

        Private Function PeakWorkingSetMb() As Double
            Dim process As Process = Process.GetCurrentProcess()

            process.Refresh()

            Return process.PeakWorkingSet64 / 1024.0 / 1024.0
        End Function

        Private Function ReadInt(args As String(), name As String, fallback As Integer) As Integer
            Dim index As Integer = Array.IndexOf(args, name)

            If index < 0 OrElse index + 1 >= args.Length Then
                Return fallback
            End If

            Dim value As Integer = 0

            If Integer.TryParse(args(index + 1), value) Then
                Return value
            Else
                Return fallback
            End If
        End Function

        Private Function ReadLong(args As String(), name As String, fallback As Long) As Long
            Dim index As Integer = Array.IndexOf(args, name)

            If index < 0 OrElse index + 1 >= args.Length Then
                Return fallback
            End If

            Dim value As Long = 0

            If Long.TryParse(args(index + 1), value) Then
                Return value
            Else
                Return fallback
            End If
        End Function

#End Region

    End Module

