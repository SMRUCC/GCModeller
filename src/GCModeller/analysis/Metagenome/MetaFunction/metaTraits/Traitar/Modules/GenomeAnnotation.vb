' ============================================================================
'  Module 1 - GenomeAnnotation.vb
'  Genome annotation & feature extraction (from DNA / proteins to Pfam profile)
'
'  Pipeline:
'    1a. Parse GFF3 -> list of ProteinRecords (locus tags, coordinates).
'    1b. Parse protein FASTA -> attach sequences to ProteinRecords.
'    1c. Run HMMER3 hmmsearch against Pfam (command-line) OR read a
'        pre-computed Pfam annotation TSV. (Per the paper: "在这里只需要
'        直接通过命令行调用，生成基因的PFAM注释结果即可".)
'    1d. Apply Traitar's bit-score >= 25 AND E-value <= 1e-2 filter.
'    1e. Binarize: Pfam present (1) if >=1 protein carries a passing hit,
'        else absent (0). This yields the phyletic profile vector x.
' ============================================================================
Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.IO
Imports TraitarVBNet.Models
Imports TraitarVBNet.Utils

Namespace Modules

    Public Module GenomeAnnotation

        ''' <summary>
        ''' Build a GenomeSample from a GFF file and a protein FASTA file.
        ''' Sequences are attached to the GFF-parsed proteins by locus tag.
        ''' </summary>
        Public Function BuildSampleFromGffAndFasta(sampleId As String,
                                                    gffPath As String,
                                                    fastaPath As String) As GenomeSample
            Dim sample As New GenomeSample() With {
                .SampleId = sampleId,
                .SourceGff = gffPath,
                .SourceFasta = fastaPath
            }
            sample.Proteins = FileIO.ParseGff(gffPath)
            FileIO.AttachSequences(sample.Proteins, fastaPath)
            Return sample
        End Function

        ''' <summary>
        ''' Build a GenomeSample directly from a protein FASTA file (no GFF).
        ''' Each FASTA entry becomes one ProteinRecord with its locus tag.
        ''' </summary>
        Public Function BuildSampleFromFasta(sampleId As String,
                                             fastaPath As String) As GenomeSample
            Dim sample As New GenomeSample() With {
                .SampleId = sampleId,
                .SourceFasta = fastaPath
            }
            Dim seqs As Dictionary(Of String, String) = FileIO.ParseFasta(fastaPath)
            For Each kv As KeyValuePair(Of String, String) In seqs
                sample.Proteins.Add(New ProteinRecord() With {
                    .LocusTag = kv.Key,
                    .Sequence = kv.Value
                })
            Next
            Return sample
        End Function

        ''' <summary>
        ''' Run HMMER3 hmmsearch against a Pfam database and attach the hits
        ''' to the sample's proteins. Returns the path to the domtblout file.
        '''
        ''' Command line (matches the Traitar paper):
        '''   hmmsearch --domtblout &lt;out&gt; --cut_tc -E 1e-2 &lt;pfam.hmm&gt; &lt;proteins.faa&gt;
        '''
        ''' If hmmsearch is not on PATH, this method throws FileNotFoundException
        ''' so the caller can fall back to AnnotateFromPrecomputed().
        ''' </summary>
        Public Function AnnotateWithHmmer(sample As GenomeSample,
                                          pfamHmmDb As String,
                                          workingDir As String,
                                          Optional evalueCutoff As Double = 0.01,
                                          Optional bitscoreCutoff As Double = 25.0) As String
            ' 1. Write the sample's proteins to a temporary FASTA
            Dim fastaPath As String = Path.Combine(workingDir, sample.SampleId & "_proteins.faa")
            Using writer As New StreamWriter(fastaPath)
                For Each p As ProteinRecord In sample.Proteins
                    If String.IsNullOrEmpty(p.Sequence) Then Continue For
                    writer.WriteLine(">" & p.LocusTag)
                    writer.WriteLine(p.Sequence)
                Next
            End Using

            ' 2. Run hmmsearch
            Dim domtbloutPath As String = Path.Combine(workingDir, sample.SampleId & "_pfam.domtblout")
            Dim args As String = String.Format("--domtblout ""{0}"" -E {1} ""{2}"" ""{3}""",
                                              domtbloutPath, evalueCutoff, pfamHmmDb, fastaPath)
            Dim psi As New ProcessStartInfo() With {
                .FileName = "hmmsearch",
                .Arguments = args,
                .UseShellExecute = False,
                .RedirectStandardOutput = True,
                .RedirectStandardError = True,
                .CreateNoWindow = True
            }
            Using proc As New Process()
                proc.StartInfo = psi
                proc.Start()
                proc.WaitForExit()
                If proc.ExitCode <> 0 Then
                    Dim err As String = proc.StandardError.ReadToEnd()
                    Throw New Exception("hmmsearch failed: " & err)
                End If
            End Using

            ' 3. Parse the domtblout and attach hits
            AttachPfamHitsFromDomtblout(sample, domtbloutPath, bitscoreCutoff, evalueCutoff)
            Return domtbloutPath
        End Function

        ''' <summary>
        ''' Attach Pfam hits parsed from a HMMER domtblout file to the sample's
        ''' proteins, applying Traitar's bit-score / E-value thresholds.
        ''' </summary>
        Public Sub AttachPfamHitsFromDomtblout(sample As GenomeSample,
                                               domtbloutPath As String,
                                               Optional bitscoreCutoff As Double = 25.0,
                                               Optional evalueCutoff As Double = 0.01)
            Dim hits As List(Of PfamHit) = FileIO.ParseHmmsearchDomtblout(domtbloutPath)
            AttachPfamHitsInternal(sample, hits, bitscoreCutoff, evalueCutoff)
        End Sub

        ''' <summary>
        ''' Attach Pfam hits parsed from a pre-computed TSV file (columns:
        ''' target_name, pfam_acc, pfam_name, evalue, bitscore). Used by the
        ''' demo when HMMER is not installed.
        ''' </summary>
        Public Sub AttachPfamHitsFromTsv(sample As GenomeSample,
                                         tsvPath As String,
                                         Optional bitscoreCutoff As Double = 25.0,
                                         Optional evalueCutoff As Double = 0.01)
            Dim hits As List(Of PfamHit) = FileIO.ParsePfamTsv(tsvPath)
            AttachPfamHitsInternal(sample, hits, bitscoreCutoff, evalueCutoff)
        End Sub

        ''' <summary>
        ''' Internal helper: distribute PfamHit objects to the matching
        ''' ProteinRecords by target name, after applying the reliability filter.
        ''' </summary>
        Private Sub AttachPfamHitsInternal(sample As GenomeSample,
                                           hits As List(Of PfamHit),
                                           bitscoreCutoff As Double,
                                           evalueCutoff As Double)
            ' Index proteins by locus tag for O(1) lookup
            Dim byTag As New Dictionary(Of String, ProteinRecord)(StringComparer.OrdinalIgnoreCase)
            For Each p As ProteinRecord In sample.Proteins
                If Not byTag.ContainsKey(p.LocusTag) Then byTag(p.LocusTag) = p
            Next

            For Each h As PfamHit In hits
                h.ScoreThreshold = bitscoreCutoff
                h.EValueThreshold = evalueCutoff
                If Not h.Passes Then Continue For
                If byTag.ContainsKey(h.TargetName) Then
                    byTag(h.TargetName).PfamHits.Add(h)
                End If
            Next

            ' Finally build the binary Pfam profile
            sample.BuildPfamProfile()
        End Sub

        ''' <summary>
        ''' Return the binary phyletic profile as a sorted list of
        ''' (Pfam accession, 1) pairs - useful for inspection / debugging.
        ''' </summary>
        Public Function GetPfamProfileList(sample As GenomeSample) As List(Of Tuple(Of String, Integer))
            Dim result As New List(Of Tuple(Of String, Integer))()
            For Each kv As KeyValuePair(Of String, Integer) In sample.PfamProfile
                result.Add(Tuple.Create(kv.Key, kv.Value))
            Next
            result.Sort(Function(a, b) String.Compare(a.Item1, b.Item1, StringComparison.Ordinal))
            Return result
        End Function

    End Module

End Namespace
