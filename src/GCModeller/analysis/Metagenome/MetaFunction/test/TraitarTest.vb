' ============================================================================
'  Program.vb - Main entry point for the Traitar VB.NET phenotype predictor
'
'  Workflow:
'    1. Parse command-line arguments (GFF, FASTA, optional Pfam TSV, model dir)
'    2. Module 1: Build a GenomeSample from GFF + FASTA, attach Pfam hits
'       (either by running HMMER or by reading a pre-computed TSV).
'    3. Load all phenotype models from the model directory (or .zip).
'    4. For each phenotype model:
'         Module 6: Predict via the 5-member voting committee.
'         Module 7: Extract the top key Pfam features for explanation.
'    5. Write predictions + key features to TSV files in the output dir.
'    6. (Optional) If ground-truth labels are provided, run Module 8
'       (multi-label evaluation) and print macro/micro accuracy.
' ============================================================================
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.Models
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.Modules
Imports TraitarVBNet.Models
Imports TraitarVBNet.Modules
Imports TraitarVBNet.Utils

Namespace TraitarVBNet

    Public Class Program

        Public Shared Function Main(args As String()) As Integer
            Console.WriteLine()
            Console.WriteLine("==============================================================")
            Console.WriteLine("  Traitar VB.NET - Microbial Phenotype Predictor")
            Console.WriteLine("  Implementation of Weimann et al., mSystems 2016")
            Console.WriteLine("==============================================================")
            Console.WriteLine()

            ' ---- Parse arguments ----
            Dim opts As New CommandLineOptions()
            If Not ParseArgs(args, opts) Then
                PrintUsage()
                Return 1
            End If

            ' ---- Step 1: Build the genome sample ----
            Console.WriteLine("[1/5] Building genome sample from GFF + FASTA ...")
            Console.WriteLine("      GFF:   " & opts.GffPath)
            Console.WriteLine("      FASTA: " & opts.FastaPath)
            Dim sample As GenomeSample =
                GenomeAnnotation.BuildSampleFromGffAndFasta(opts.SampleId,
                                                            opts.GffPath,
                                                            opts.FastaPath)
            Console.WriteLine("      Parsed " & sample.Proteins.Count & " protein records.")

            ' ---- Step 2: Attach Pfam annotations ----
            Console.WriteLine()
            Console.WriteLine("[2/5] Attaching Pfam annotations ...")
            If Not String.IsNullOrEmpty(opts.PfamTsvPath) AndAlso File.Exists(opts.PfamTsvPath) Then
                Console.WriteLine("      Reading pre-computed Pfam TSV: " & opts.PfamTsvPath)
                GenomeAnnotation.AttachPfamHitsFromTsv(sample, opts.PfamTsvPath,
                                                        opts.BitScoreCutoff,
                                                        opts.EValueCutoff)
                Console.WriteLine("      Pfam hits attached and filtered.")
            ElseIf Not String.IsNullOrEmpty(opts.HmmsearchPath) AndAlso
                   Not String.IsNullOrEmpty(opts.PfamDbPath) Then
                Console.WriteLine("      Running HMMER hmmsearch ...")
                Console.WriteLine("      hmmsearch: " & opts.HmmsearchPath)
                Console.WriteLine("      Pfam DB:   " & opts.PfamDbPath)
                Dim workDir As String = Path.Combine(opts.OutputDir, "hmmer_work")
                Directory.CreateDirectory(workDir)
                GenomeAnnotation.AnnotateWithHmmer(sample, opts.PfamDbPath, workDir,
                                                    opts.EValueCutoff, opts.BitScoreCutoff)
                Console.WriteLine("      HMMER annotation complete.")
            Else
                Console.WriteLine("      WARNING: no Pfam annotation source provided.")
                Console.WriteLine("              Sample will have an empty Pfam profile.")
                sample.BuildPfamProfile()
            End If
            Console.WriteLine("      Pfam families present (after filtering): " &
                              sample.PfamProfile.Count)

            ' ---- Step 3: Load phenotype models ----
            Console.WriteLine()
            Console.WriteLine("[3/5] Loading phenotype models ...")
            Dim models As List(Of PhenotypeModel)
            If Not String.IsNullOrEmpty(opts.ModelZipPath) AndAlso File.Exists(opts.ModelZipPath) Then
                Console.WriteLine("      From ZIP: " & opts.ModelZipPath)
                models = ModelLoader.LoadAllModelsFromZip(opts.ModelZipPath)
            Else
                Console.WriteLine("      From directory: " & opts.ModelDir)
                models = ModelLoader.LoadAllModels(opts.ModelDir)
            End If
            Console.WriteLine("      Loaded " & models.Count & " phenotype models.")
            For Each m As PhenotypeModel In models
                Console.WriteLine("        - " & m.PhenotypeId & " (" & m.PhenotypeName &
                                  ")  active sub-models: " & m.GetVotingCommittee().Count)
            Next

            ' ---- Step 4: Predict each phenotype ----
            Console.WriteLine()
            Console.WriteLine("[4/5] Predicting phenotypes ...")
            Dim predictions As New List(Of PhenotypePrediction)()
            For Each m As PhenotypeModel In models
                Dim pred As Integer = m.Predict(sample)
                Dim score As Double = m.DecisionScore(sample)
                Dim keyFeats As List(Of PhenotypeFeature) = m.GetKeyFeatures()
                predictions.Add(New PhenotypePrediction() With {
                    .PhenotypeId = m.PhenotypeId,
                    .PhenotypeName = m.PhenotypeName,
                    .Category = m.Category,
                    .Prediction = pred,
                    .Score = score,
                    .KeyFeatures = keyFeats
                })
                Console.WriteLine("      " & m.PhenotypeId.PadLeft(5) & "  " &
                                  If(pred = 1, "+", "-") & "  " &
                                  score.ToString("0.000").PadLeft(8) & "   " &
                                  m.PhenotypeName)
            Next

            ' ---- Step 5: Write outputs ----
            Console.WriteLine()
            Console.WriteLine("[5/5] Writing outputs to " & opts.OutputDir & " ...")
            Directory.CreateDirectory(opts.OutputDir)
            WritePredictionsTsv(predictions, Path.Combine(opts.OutputDir, "phenotype_predictions.tsv"))
            WriteFeatureContributionsTsv(predictions, Path.Combine(opts.OutputDir, "feature_contributions.tsv"))
            WriteSummaryTxt(sample, predictions, Path.Combine(opts.OutputDir, "demo_log.txt"))

            Console.WriteLine()
            Console.WriteLine("Done. Output files:")
            Console.WriteLine("  " & Path.Combine(opts.OutputDir, "phenotype_predictions.tsv"))
            Console.WriteLine("  " & Path.Combine(opts.OutputDir, "feature_contributions.tsv"))
            Console.WriteLine("  " & Path.Combine(opts.OutputDir, "demo_log.txt"))
            Return 0
        End Function

        ' -------------------------------------------------------------------
        '  Output writers
        ' -------------------------------------------------------------------

        Private Shared Sub WritePredictionsTsv(predictions As List(Of PhenotypePrediction),
                                                path As String)
            Using sw As New StreamWriter(path)
                sw.WriteLine("PhenotypeID" & vbTab & "PhenotypeName" & vbTab & "Category" &
                             vbTab & "Prediction" & vbTab & "Score" & vbTab & "Confidence")
                For Each p As PhenotypePrediction In predictions
                    Dim conf As String = If(p.Prediction = 1, "POSITIVE", "negative")
                    sw.WriteLine(p.PhenotypeId & vbTab & p.PhenotypeName & vbTab & p.Category &
                                 vbTab & p.Prediction & vbTab &
                                 p.Score.ToString("0.######") & vbTab & conf)
                Next
            End Using
        End Sub

        Private Shared Sub WriteFeatureContributionsTsv(predictions As List(Of PhenotypePrediction),
                                                          path As String)
            Using sw As New StreamWriter(path)
                sw.WriteLine("PhenotypeID" & vbTab & "PhenotypeName" & vbTab & "Prediction" &
                             vbTab & "PfamAcc" & vbTab & "Class" & vbTab &
                             "PearsonCorr" & vbTab & "Description")
                For Each p As PhenotypePrediction In predictions
                    For Each f As PhenotypeFeature In p.KeyFeatures
                        sw.WriteLine(p.PhenotypeId & vbTab & p.PhenotypeName & vbTab &
                                     p.Prediction & vbTab & f.PfamAcc & vbTab &
                                     f.WeightClass & vbTab &
                                     f.PearsonCorrelation.ToString("0.####") & vbTab &
                                     f.Description)
                    Next
                Next
            End Using
        End Sub

        Private Shared Sub WriteSummaryTxt(sample As GenomeSample,
                                            predictions As List(Of PhenotypePrediction),
                                            path As String)
            Using sw As New StreamWriter(path)
                sw.WriteLine("Traitar VB.NET - Phenotype Prediction Demo Log")
                sw.WriteLine("================================================")
                sw.WriteLine()
                sw.WriteLine("Sample ID: " & sample.SampleId)
                sw.WriteLine("Proteins parsed: " & sample.Proteins.Count)
                sw.WriteLine("Pfam families present (after bit-score/E-value filter): " &
                             sample.PfamProfile.Count)
                sw.WriteLine()
                sw.WriteLine("Pfam families detected:")
                Dim keys As New List(Of String)(sample.PfamProfile.Keys)
                keys.Sort(StringComparer.OrdinalIgnoreCase)
                For Each k As String In keys
                    sw.WriteLine("  " & k & " = " & sample.PfamProfile(k))
                Next
                sw.WriteLine()
                sw.WriteLine("Phenotype predictions:")
                sw.WriteLine("------------------------------------------------")
                Dim nPos As Integer = 0
                For Each p As PhenotypePrediction In predictions
                    If p.Prediction = 1 Then nPos += 1
                    sw.WriteLine("  " & p.PhenotypeId.PadLeft(5) & "  " &
                                 If(p.Prediction = 1, "[+]", "[ ]") & "  " &
                                 p.Score.ToString("0.000").PadLeft(8) & "  " &
                                 p.PhenotypeName)
                Next
                sw.WriteLine()
                sw.WriteLine("Total phenotypes predicted POSITIVE: " & nPos &
                             " / " & predictions.Count)
                sw.WriteLine()
                sw.WriteLine("Top key Pfam features per positive phenotype:")
                sw.WriteLine("------------------------------------------------")
                For Each p As PhenotypePrediction In predictions
                    If p.Prediction <> 1 Then Continue For
                    sw.WriteLine()
                    sw.WriteLine("Phenotype " & p.PhenotypeId & " (" & p.PhenotypeName & "):")
                    For Each f As PhenotypeFeature In p.KeyFeatures
                        sw.WriteLine("  " & f.PfamAcc & "  [" & f.WeightClass & "]  " &
                                     "r=" & f.PearsonCorrelation.ToString("0.###").PadLeft(7) &
                                     "  " & f.Description)
                    Next
                Next
            End Using
        End Sub

        ' -------------------------------------------------------------------
        '  Command-line parsing
        ' -------------------------------------------------------------------

        Private Class CommandLineOptions
            Public SampleId As String = "sample_1"
            Public GffPath As String = ""
            Public FastaPath As String = ""
            Public PfamTsvPath As String = ""
            Public HmmsearchPath As String = ""
            Public PfamDbPath As String = ""
            Public ModelDir As String = ""
            Public ModelZipPath As String = ""
            Public OutputDir As String = "./output"
            Public BitScoreCutoff As Double = 25.0R
            Public EValueCutoff As Double = 0.01R
        End Class

        Private Shared Function ParseArgs(args As String(), opts As CommandLineOptions) As Boolean
            If args Is Nothing OrElse args.Length = 0 Then Return False
            For i As Integer = 0 To args.Length - 1
                Dim a As String = args(i)
                Select Case a.ToLowerInvariant()
                    Case "--gff" : If i + 1 < args.Length Then opts.GffPath = args(i + 1) : i += 1
                    Case "--fasta" : If i + 1 < args.Length Then opts.FastaPath = args(i + 1) : i += 1
                    Case "--pfam-tsv" : If i + 1 < args.Length Then opts.PfamTsvPath = args(i + 1) : i += 1
                    Case "--hmmsearch" : If i + 1 < args.Length Then opts.HmmsearchPath = args(i + 1) : i += 1
                    Case "--pfam-db" : If i + 1 < args.Length Then opts.PfamDbPath = args(i + 1) : i += 1
                    Case "--model-dir" : If i + 1 < args.Length Then opts.ModelDir = args(i + 1) : i += 1
                    Case "--model-zip" : If i + 1 < args.Length Then opts.ModelZipPath = args(i + 1) : i += 1
                    Case "--out" : If i + 1 < args.Length Then opts.OutputDir = args(i + 1) : i += 1
                    Case "--sample-id" : If i + 1 < args.Length Then opts.SampleId = args(i + 1) : i += 1
                    Case "--bitscore" : If i + 1 < args.Length Then opts.BitScoreCutoff = CDbl(args(i + 1)) : i += 1
                    Case "--evalue" : If i + 1 < args.Length Then opts.EValueCutoff = CDbl(args(i + 1)) : i += 1
                    Case "--help", "-h" : Return False
                End Select
            Next
            ' Require at least a FASTA and one model source
            If opts.FastaPath.Length = 0 Then Return False
            If opts.ModelDir.Length = 0 AndAlso opts.ModelZipPath.Length = 0 Then Return False
            Return True
        End Function

        Private Shared Sub PrintUsage()
            Console.WriteLine("Usage: TraitarVBNet --fasta <proteins.fasta> --model-dir <dir> [options]")
            Console.WriteLine()
            Console.WriteLine("Required:")
            Console.WriteLine("  --fasta <path>         Protein FASTA file (amino acid sequences)")
            Console.WriteLine("  --model-dir <dir>      Directory containing Traitar model files")
            Console.WriteLine("    OR --model-zip <zip>   ZIP archive of model files")
            Console.WriteLine()
            Console.WriteLine("Optional:")
            Console.WriteLine("  --gff <path>           GFF3 annotation file (for locus tags)")
            Console.WriteLine("  --pfam-tsv <path>      Pre-computed Pfam annotation TSV")
            Console.WriteLine("                         (target_name, pfam_acc, evalue, bitscore)")
            Console.WriteLine("  --hmmsearch <path>     Path to hmmsearch executable")
            Console.WriteLine("  --pfam-db <path>       Path to Pfam HMM database")
            Console.WriteLine("  --out <dir>            Output directory (default: ./output)")
            Console.WriteLine("  --sample-id <name>     Sample identifier (default: sample_1)")
            Console.WriteLine("  --bitscore <val>       Bit-score cutoff (default: 25.0)")
            Console.WriteLine("  --evalue <val>         E-value cutoff (default: 0.01)")
        End Sub

    End Class

    ''' <summary>One phenotype prediction result with its key features.</summary>
    Public Class PhenotypePrediction
        Public Property PhenotypeId As String = ""
        Public Property PhenotypeName As String = ""
        Public Property Category As String = ""
        Public Property Prediction As Integer
        Public Property Score As Double
        Public Property KeyFeatures As New List(Of PhenotypeFeature)()
    End Class

End Namespace
