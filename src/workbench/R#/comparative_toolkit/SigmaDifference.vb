#Region "Microsoft.VisualBasic::41a8a564654bc22d01e871432671269b, R#\comparative_toolkit\SigmaDifference.vb"

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

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.Utility
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.Extensions
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Data.Framework.StorageProvider
Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.ComponentModels
Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Matrix
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998.CAI
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998.CAI.XML
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.Extensions
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Tasks.Models
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.Slicer
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports ObjectQuery = SMRUCC.genomics.ObjectQuery
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal
Imports vector = SMRUCC.Rsharp.Runtime.Internal.Object.vector

''' <summary>
''' Comparative genomics API module of the Karlin, Campbell &amp; Mrazek (1998)
''' ``Comparative DNA Analysis Across Diverse Genomes`` algorithm suite.
'''
''' This R# package exports:
'''
''' 1. the pairwise / batch sliding window ``delta*`` difference calculations;
''' 2. the partition-based genome homogeneity measurements (``dnaA``-``gyrB`` ruler);
''' 3. the codon usage / CAI compilation;
''' 4. the full set of the 1998 paper's analysis modules:
'''    the ``rho*`` genome signature, the ``tau*`` tetranucleotide relative
'''    abundance, the r-scan word distribution statistics, the site-specific
'''    codon signature, the ``B(F|C)`` codon bias difference, the 2D threshold
'''    alien gene detection, the sliding window ``delta*`` island profiling and
'''    the ``(C-G)/(C+G)`` strand composition asymmetry.
''' </summary>
<Package("sigma_difference",
         Description:="Calculates the nucleotide sequence delta-similarity to measure how closed between the two sequence.",
         Cites:="Karlin, S., et al. (1998). ""Comparative DNA analysis across diverse genomes."" Annu Rev Genet 32: 185-225.",
         Publisher:="xie.guigang@gcmodeller.org")>
<Cite(Title:="Comparative DNA analysis across diverse genomes",
      Journal:="Annu Rev Genet", PubMed:=9928479,
      Pages:="185-225",
      AuthorAddress:="Department of Mathematics, Stanford University, California 94305-2125, USA.",
      Abstract:="We review concepts and methods for comparative analysis of complete genomes including assessments of genomic compositional contrasts based on dinucleotide and tetranucleotide relative abundance values, identifications of rare and frequent oligonucleotides, evaluations and interpretations of codon biases in several large prokaryotic genomes, and characterizations of compositional asymmetry between the two DNA strands in certain bacterial genomes. 
The discussion also covers means for identifying alien (e.g. laterally transferred) genes and detecting potential specialization islands in bacterial genomes.",
      Authors:="Karlin, S.
Campbell, A. M.
Mrazek, J.",
      Keywords:="Animals
Bacteria/genetics
Base Composition
Base Sequence
Codon/genetics
DNA/chemistry/*genetics
DNA, Bacterial/genetics
Eukaryotic Cells
*Genome
Genome, Bacterial
Prokaryotic Cells
Species Specificity",
      DOI:="10.1146/annurev.genet.32.1.185", ISSN:="0066-4197 (Print)
0066-4197 (Linking)", Issue:="", Volume:=32, Year:=1998)>
<RTypeExport("partitioning_data", GetType(PartitioningData))>
<RTypeExport("site_delta", GetType(WindowDelta))>
<RTypeExport("word_scan", GetType(WordScanResult))>
<RTypeExport("window_delta", GetType(WindowDelta))>
<RTypeExport("strand_skew", GetType(StrandSkew))>
<RTypeExport("codon_signature_profile", GetType(CodonSignatureProfile))>
<RTypeExport("codon_usage_profile", GetType(CodonUsageProfile))>
<RTypeExport("codon_weight_table", GetType(CodonWeightTable))>
<RTypeExport("alien_gene_prediction", GetType(AlienGenePrediction))>
Public Module SigmaDifference

#Region "Sliding window delta* calculations"

    ''' <summary>
    ''' Evaluate the sliding window ``delta*`` profile of a collection of pre-built
    ''' window caches against one comparison genome. (non-parallel inner helper of
    ''' <see cref="BatchCalculation"/>; the parallelization is expected at the caller level)
    ''' </summary>
    ''' <param name="cache">the pre-built sliding window caches of the query genome</param>
    ''' <param name="compare">the comparison genome sequence</param>
    ''' <returns>profile rows ordered by the site position</returns>
    Private Function slidingWindowDeltaProfile(cache As Cache(), compare As FastaSeq) As WindowDelta()
        Call "Creating compare cache..... ".debug
        Dim compareCache As New DeltaSimilarity1998.NucleicAcid(compare)
        Call "Compare cache creating job done!".debug

        Return cache _
            .Select(Function(window)
                        Dim deltaStar As Double = DeltaStarDistance.DeltaStar(window.Cache, compareCache)

                        Return New WindowDelta With {
                            .site = window.SlideWindow.Index,
                            .deltaStar = deltaStar,
                            .level = DeltaStarDistance.DeltaStarLevel(deltaStar)
                        }
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' Calculate the sliding window ``delta*`` profile for every sequence pair and
    ''' export each profile as a separate csv document.
    ''' </summary>
    ''' <param name="pairedList">the sequence pairs to be compared</param>
    ''' <param name="windowsSize">the sliding window size in bp</param>
    ''' <param name="EXPORT">the csv output directory</param>
    ''' <returns>the ``query -> subject`` file name pairs of the exported profiles</returns>
    Private Function calculateBatchPairs(pairedList As Tuple(Of FastaSeq, FastaSeq)(),
                                         windowsSize As Integer,
                                         EXPORT As String) As KeyValuePair(Of String, String)()

        Dim tuples As New List(Of KeyValuePair(Of String, String))

        For Each paired As Tuple(Of FastaSeq, FastaSeq) In pairedList
            Dim profile As WindowDelta() = GenomeDeltaStarProfile(paired.Item1, paired.Item2, windowsSize)
            Dim queryId As String = paired.Item1.locus_tag
            Dim subjectId As String = paired.Item2.locus_tag
            Dim fileTag As New KeyValuePair(Of String, String)(queryId, subjectId)

            Call Console.WriteLine("[DEBUG] Calculation job done, trying to export data to filesystem " & EXPORT)
            Call profile.SaveTo($"{EXPORT}/{fileTag.Key}-{fileTag.Value}.csv", False)
            Call tuples.Add(fileTag)
        Next

        Return tuples.ToArray
    End Function

    ''' <summary>
    ''' Pairwise genome comparison for a large sequence collection: calculate the
    ''' sliding window ``delta*`` profile of every complete pair and export each
    ''' profile as a csv file. (suitable for the large scale data calculation)
    ''' </summary>
    ''' <param name="source">the directory of the ``*.fasta`` / ``*.fsa`` genome sequence files</param>
    ''' <param name="EXPORT">the csv output directory</param>
    ''' <param name="windowsSize">the sliding window size in bp, default 1kb</param>
    ''' <returns>true when all of the profiles have been exported</returns>
    Public Function BatchCalculation2(source As String, EXPORT As String, Optional windowsSize As Integer = 1000) As Boolean
        Call Console.WriteLine("[DEBUG] start to load fasta data from " & source)

        Using pb As New CBusyIndicator(start:=True)
            Dim fastaObjects As FastaSeq() = (From path As String
                                              In FileIO.FileSystem.GetFiles(source, FileIO.SearchOption.SearchTopLevelOnly, "*.fasta", "*.fsa").AsParallel
                                              Select FastaSeq.Load(path)).ToArray

            Call Console.WriteLine($"[DEBUG] fasta data load done!, start to calculates the delta* differences in window_size {windowsSize / 1000}KB....")

            Dim pairs As IEnumerable(Of Tuple(Of FastaSeq, FastaSeq)()) = Comb(Of FastaSeq).CreateCompleteObjectPairs(fastaObjects)
            Dim chunkBuffer As KeyValuePair(Of String, String)() =
                pairs _
                    .Select(Function(pairedList) calculateBatchPairs(pairedList, windowsSize, EXPORT)) _
                    .Unlist

            Call Console.WriteLine("All data calculation job done!, grouping data!")

            Dim grouped = (From item In chunkBuffer Select item Group By item.Key Into Group).ToArray
            Call Console.WriteLine("Compiling data....")
            Call grouped.AsParallel.Select(Function(item) compileSigmaExport(item.Group.ToArray, EXPORT)).ToArray
            Call Console.WriteLine("[JOB DONE]")
        End Using

        Return True
    End Function

    ''' <summary>
    ''' Pairwise genome comparison for a small sequence collection: calculate the
    ''' sliding window ``delta*`` profile of every complete pair and export each
    ''' profile as a csv file. (suitable for the small scale data calculation)
    ''' </summary>
    ''' <param name="source">the directory of the ``*.fasta`` / ``*.fsa`` genome sequence files</param>
    ''' <param name="EXPORT">the csv output directory</param>
    ''' <param name="windowsSize">the sliding window size in bp, default 1kb</param>
    ''' <returns>true when all of the profiles have been exported</returns>
    Public Function BatchCalculation(source As String, EXPORT As String, Optional windowsSize As Integer = 1000) As Boolean
        Call Console.WriteLine("[DEBUG] start to load fasta data from " & source)

        Dim pb As New CBusyIndicator(start:=True)
        Dim fastaObjects As FastaSeq() = (From path As String
                                          In FileIO.FileSystem.GetFiles(source, FileIO.SearchOption.SearchTopLevelOnly, "*.fasta", "*.fsa").AsParallel
                                          Select FastaSeq.Load(path)).ToArray

        Call $"Fasta data load done!, start to calculates the delta* differences in window_size {windowsSize / 1000}KB....".debug

        Dim pairs As IEnumerable(Of Tuple(Of FastaSeq, FastaSeq)()) = Comb(Of FastaSeq).CreateCompleteObjectPairs(fastaObjects)
        Dim chunkBuffer As KeyValuePair(Of String, String)() =
            pairs.AsParallel _
                .Select(Function(pairedList) calculateBatchPairs(pairedList, windowsSize, EXPORT)) _
                .Unlist

        Call Console.WriteLine("All data calculation job done!, grouping data!")

        Dim grouped = (From item In chunkBuffer Select item Group By item.Key Into Group).ToArray
        Call Console.WriteLine("Compiling data....")
        Call grouped.AsParallel.Select(Function(item) compileSigmaExport(item.Group.ToArray, EXPORT)).ToArray
        Call Console.WriteLine("[JOB DONE]")
        Call pb.Dispose()

        Return True
    End Function

    ''' <summary>
    ''' Compile the pairwise sliding window ``delta*`` profile csv files into one
    ''' merged matrix document: one row per window site, one ``delta* / level``
    ''' column pair per comparison.
    ''' </summary>
    ''' <param name="dat">the ``query -> subject`` file name pairs sharing the same query genome</param>
    ''' <param name="export">the profile csv source directory</param>
    ''' <returns>true when the compiled csv document has been saved</returns>
    Private Function compileSigmaExport(dat As KeyValuePair(Of String, String)(), export As String) As Boolean
        Dim fileName As String = $"{export}/Compiled/{dat.First.Key}.csv"
        Dim file As New IO.File
        ' keep the row order one-to-one: no parallelization here
        Dim data As WindowDelta()() = dat _
            .Select(Function(path) $"{export}/{path.Key}-{path.Value}.csv".LoadCsv(Of WindowDelta)(False).ToArray) _
            .ToArray
        Dim head As New IO.RowObject

        Call head.Add("Site")

        For Each entry As KeyValuePair(Of String, String) In dat
            Call head.Add("")
            Call head.Add("DeltaStar")
            Call head.Add($"{entry.Value}->Level")
        Next

        Call file.Add(head)

        For i As Integer = 0 To data.First.Count - 1
            Dim row As New IO.RowObject

            Call row.Add(i)

            For Each profile As WindowDelta() In data
                Call row.Add("")
                Call row.Add(profile(i).deltaStar)
                Call row.Add(profile(i).level)
            Next

            Call file.Add(row)
        Next

        Return file.Save(fileName, False)
    End Function

    ''' <summary>
    ''' Generate the comparative genomics report grouped by the functional
    ''' partition tags.
    ''' </summary>
    ''' <param name="source">the source directory of the delta query export directory</param>
    ''' <param name="partitions">the partition definitions of the query proteins</param>
    ''' <param name="CDSInfo">the gene information on the <see cref="WindowDelta.site"/> positions</param>
    ''' <returns>the report table (currently not implemented)</returns>
    ''' <remarks>
    ''' Expected output layout:
    '''
    ''' Description QueryProtein PartitionTag genome1.delta genome1.level genome2.delta genome2.level
    ''' dsc1 a 1 ...
    ''' dsc2 b 2 ...
    ''' </remarks>
    Public Function GenerateDeltaDiffReport(source As String,
                                            partitions As IEnumerable(Of ChromosomePartitioningEntry),
                                            CDSInfo As IEnumerable(Of GeneTable)) As IO.File
        Dim partitionGroups = (From item In partitions Select item Group By item.PartitioningTag Into Group).ToArray
        Dim deltaQuery = (From path As NamedValue(Of String)
                          In gbExportService _
                              .LoadGbkSource(source) _
                              .Values _
                              .AsParallel
                          Select ID = path.Name,
                              dat = path.Value.LoadCsv(Of WindowDelta)(False).ToArray).ToArray

        Throw New NotImplementedException
    End Function

    ''' <summary>
    ''' Load one sliding window ``delta*`` profile from its csv document.
    ''' </summary>
    ''' <param name="path">the csv file path of one <see cref="WindowDelta"/> profile</param>
    ''' <returns>the profile rows ordered by the site position</returns>
    <ExportAPI("read.site_delta")>
    <RApiReturn(GetType(WindowDelta))>
    Public Function SiteDataLoad(path As String) As Object
        Return path.LoadCsv(Of WindowDelta)(mute:=True).ToArray
    End Function

    ''' <summary>
    ''' Compile a set of pairwise ``delta*`` profile csv files (all created from the
    ''' same query genome against different subjects) into one merged matrix csv
    ''' document. The output file names cannot be changed by this requirement.
    ''' </summary>
    ''' <param name="source">the source directory that contains the profile csv files</param>
    ''' <param name="saveCsv">the merged matrix csv output file path</param>
    ''' <returns>true when the merged document has been saved</returns>
    <ExportAPI("compile.delta_query")>
    Public Function Compile(source As String, saveCsv As String) As Boolean
        Dim entry = gbExportService.LoadGbkSource(source)
        Dim profiles = (From item
                        In entry.Values.AsParallel
                        Select New KeyValuePair(Of String, WindowDelta())(item.Name, item.Value.LoadCsv(Of WindowDelta)(False).ToArray)).ToArray
        Dim file As IO.File = compileSiteDeltaMatrix(profiles)

        Return file.Save(saveCsv, False)
    End Function

    ''' <summary>
    ''' Compare one query genome against a directory of subject genomes: calculate
    ''' the sliding window ``delta*`` profile of the query against every subject and
    ''' export each profile as a csv file, then compile all of the profiles into one
    ''' merged matrix csv document.
    ''' </summary>
    ''' <param name="query">the query genome fasta file path</param>
    ''' <param name="sbjDIR">the directory of the subject genome fasta files</param>
    ''' <param name="EXPORT">the csv output directory</param>
    ''' <param name="windowsSize">the sliding window size in bp, default 1kb</param>
    ''' <returns>true when the compiled matrix has been saved</returns>
    <ExportAPI("sigma_diff.query")>
    Public Function SigmaCompareWith(query As String, sbjDIR As String, EXPORT As String, Optional windowsSize As Integer = 1000) As Boolean
        Call ("Start to load subject fasta data from " & sbjDIR).debug

        Using pb = New CBusyIndicator(start:=True)
            Return compareQueryWithSubjects(query, sbjDIR, EXPORT, windowsSize)
        End Using
    End Function

    ''' <summary>
    ''' The internal worker of <see cref="SigmaCompareWith"/>.
    ''' </summary>
    ''' <param name="query">the query genome fasta file path</param>
    ''' <param name="subject">the directory of the subject genome fasta files</param>
    ''' <param name="EXPORT">the csv output directory</param>
    ''' <param name="windowsSize">the sliding window size in bp</param>
    ''' <returns>true when the compiled matrix has been saved</returns>
    Private Function compareQueryWithSubjects(query As String, subject As String, EXPORT As String, windowsSize As Integer) As Boolean
        Dim subjects As FastaSeq() = (From path As String
                                      In FileIO.FileSystem.GetFiles(subject, FileIO.SearchOption.SearchTopLevelOnly, "*.fasta", "*.fsa").AsParallel
                                      Select FastaSeq.LoadNucleotideData(path)).ToArray

        Call $"Fasta data load done!, start to calculates the delta* differences in window_size {windowsSize / 1000}KB....".debug

        Dim queryFasta As FastaSeq = FastaSeq.LoadNucleotideData(query)
        Dim windows As SlideWindow(Of DNA)() = New NucleotideModels.NucleicAcid(queryFasta).ToArray.CreateSlideWindows(windowsSize)
        Dim windowCaches As Cache() = windows _
            .AsParallel _
            .Select(Function(window)
                        Return New Cache With {
                            .Cache = New DeltaSimilarity1998.NucleicAcid(window.Items),
                            .SlideWindow = window
                        }
                    End Function) _
            .ToArray

        Call Console.WriteLine($"[INFO] query for the delta* difference calculation in length of {queryFasta.Length / 1000}KB...")

        Dim profiles = (From subjectFasta As FastaSeq
                        In subjects.AsParallel
                        Select processSubject(subjectFasta, queryFasta, EXPORT, windowCaches)).ToArray
        Dim fileName As String = $"{EXPORT}/Compiled/{BaseName(query)}.csv"
        Dim file As IO.File = compileSiteDeltaMatrix(profiles)

        Call Console.WriteLine("[JOB DONE]")

        Return file.Save(fileName, False)
    End Function

    ''' <summary>
    ''' Calculate the ``delta*`` profile of one subject genome against the pre-built
    ''' query window caches and export the profile as a csv file.
    ''' </summary>
    ''' <param name="subjectFasta">the subject genome sequence</param>
    ''' <param name="queryFasta">the query genome sequence (for the output file naming)</param>
    ''' <param name="export">the csv output directory</param>
    ''' <param name="windowCaches">the pre-built sliding window caches of the query genome</param>
    ''' <returns>the subject id and its ``delta*`` profile</returns>
    Private Function processSubject(subjectFasta As FastaSeq,
                                    queryFasta As FastaSeq,
                                    export As String,
                                    windowCaches As Cache()) As KeyValuePair(Of String, WindowDelta())
        Call Console.WriteLine($"[DEBUG] Start the calculation threads ""{subjectFasta.Title}""... ")

        Dim profile As WindowDelta() = slidingWindowDeltaProfile(windowCaches, subjectFasta)
        Dim queryId As String = queryFasta.Title.Split(CChar("|")).First.NormalizePathString
        Dim subjectId As String = subjectFasta.Title.Split(CChar("|")).First.NormalizePathString
        Dim path As String = $"{export}/{queryId}-{subjectId}.csv"

        Call Console.WriteLine("[DEBUG] Calculation job done, trying to export data to filesystem " & path)
        Call profile.SaveTo(path, False)

        Return New KeyValuePair(Of String, WindowDelta())(subjectId, profile)
    End Function

    ''' <summary>
    ''' Merge a collection of ``delta*`` profiles (all calculated from the same query
    ''' genome) into one matrix table: one row per window site, one ``delta* / level``
    ''' column pair per subject.
    ''' </summary>
    ''' <param name="profiles">the ``subject id -> profile`` pairs sharing the same query genome</param>
    ''' <returns>the merged matrix table</returns>
    Private Function compileSiteDeltaMatrix(profiles As KeyValuePair(Of String, WindowDelta())()) As IO.File
        Dim file As New IO.File
        Dim head As New IO.RowObject     ' keep the one-to-one row order: no parallelization here

        Call Console.WriteLine("Compiling data....")
        Call head.Add("Site")

        For Each entry As KeyValuePair(Of String, WindowDelta()) In profiles
            Call head.Add("")
            Call head.Add("DeltaStar")
            Call head.Add($"{entry.Key}->Level")
        Next

        Call file.Add(head)

        For i As Integer = 0 To profiles.First.Value.Count - 1
            Dim row As New IO.RowObject

            Call row.Add(i)

            For Each entry As KeyValuePair(Of String, WindowDelta()) In profiles
                Dim line As WindowDelta = entry.Value(i)

                Call row.Add("")
                Call row.Add(line.deltaStar)
                Call row.Add(line.level)
            Next

            Call file.Add(row)
        Next

        Return file
    End Function

    ''' <summary>
    ''' Sliding window ``delta*`` profile: the delta-difference between each local
    ''' window of the genome and the comparison sequence.
    '''
    ''' PERFORMANCE: implemented with the incremental window count matrix
    ''' (O(1) update per slide + O(16) distance per sample), and the comparison
    ''' sequence signature is built only once.
    ''' </summary>
    ''' <param name="genome">the query genome sequence</param>
    ''' <param name="compare">the comparison genome sequence</param>
    ''' <param name="windowsSize">the sliding window size in bp, default 1kb</param>
    ''' <returns>profile rows ordered by the site position, an array of <see cref="WindowDelta"/></returns>
    <ExportAPI("genome.delta_star_profile")>
    <RApiReturn(GetType(WindowDelta))>
    Public Function GenomeDeltaStarProfile(genome As FastaSeq, compare As FastaSeq, Optional windowsSize As Integer = 1000) As Object
        Dim reference As New DeltaSimilarity1998.NucleicAcid(compare)
        Dim targetGenome As New DeltaSimilarity1998.NucleicAcid(genome)

        Call "Start the sliding window delta* profile calculation...".info

        Dim profile As WindowDelta() = targetGenome.DeltaStarProfile(reference, windowSize:=windowsSize, stepSize:=1).ToArray
        Dim rows As WindowDelta() = profile _
            .Select(Function(window)
                        Return New WindowDelta With {
                            .site = window.site,
                            .deltaStar = window.deltaStar,
                            .level = window.level
                        }
                    End Function) _
            .ToArray

        Call "[JOB DONE!] delta* profile created.".info

        Return rows
    End Function

#End Region

#Region "Partitioning data comparison"

    ''' <summary>
    ''' Compare one query genome against a set of subject genomes restricted to
    ''' the given chromosomal partition regions (the partition data are grouped by
    ''' the partition tag, and every partition is compared independently).
    ''' </summary>
    ''' <param name="source">the partition data of all of the genomes under study</param>
    ''' <param name="query">the genome id of the query genome (sensitive to the character case)</param>
    ''' <param name="EXPORT">the csv output directory</param>
    ''' <param name="winSize">the sliding window size in bp, default 1kb</param>
    ''' <returns>true when at least one partition profile has been exported</returns>
    ''' <remarks>Please notice that the query parameter is sensitive to the character case.</remarks>
    Public Function PartitioningSigmaCompareWith(source As IEnumerable(Of PartitioningData), query As String, EXPORT As String, Optional winSize As Integer = 1000) As Boolean
        Using pb = New CBusyIndicator(start:=True)
            Dim partitionIndex = (From nn In (From item In source Select item Group By item.PartitioningTag Into Group).ToArray
                                  Select nn.PartitioningTag,
                                      dict = nn.Group.ToDictionary(Function(item) item.GenomeID)) _
                                 .ToDictionary(Function(item) item.PartitioningTag,
                                               Function(item) item.dict)
            Dim queryPartitions = (From item In partitionIndex Select item.Value(query)).ToArray

            Call $"Fasta data load done!, start to calculates the delta* differences in window_size {winSize / 1000}KB....".debug

            Dim results = (From queryItem In queryPartitions.AsParallel
                           Where Not String.IsNullOrEmpty(queryItem.SequenceData) OrElse
                               queryItem.SequenceData.Length = 1
                           Select queryPartition(queryItem, subject:=partitionIndex(queryItem.PartitioningTag), windowsSize:=winSize, EXPORT:=EXPORT, ptag:=queryItem.PartitioningTag)).ToArray

            Return Not results.IsNullOrEmpty
        End Using
    End Function

    ''' <summary>
    ''' Compare all of the subject partitions of one partition tag against the query
    ''' partition and export the compiled profile matrix.
    ''' </summary>
    ''' <param name="querySource">the query partition data</param>
    ''' <param name="subject">the subject partition data of the same partition tag, keyed by the genome id</param>
    ''' <param name="windowsSize">the sliding window size in bp</param>
    ''' <param name="EXPORT">the csv output directory</param>
    ''' <param name="ptag">the partition tag (for the output file naming)</param>
    ''' <returns>true when the compiled document has been saved</returns>
    Private Function queryPartition(querySource As PartitioningData,
                                    subject As Dictionary(Of String, PartitioningData),
                                    windowsSize As Integer,
                                    EXPORT As String,
                                    ptag As String) As Boolean
        Dim windows As SlideWindow(Of DNA)() = New NucleotideModels.NucleicAcid(querySource.SequenceData).ToArray.CreateSlideWindows(windowsSize)
        Dim windowCaches As Cache() = windows _
            .Select(Function(window)
                        Return New Cache With {
                            .Cache = New DeltaSimilarity1998.NucleicAcid(window.Items),
                            .SlideWindow = window
                        }
                    End Function) _
            .ToArray

        Call $"[INFO] query for the delta* difference calculation in length of {querySource.SequenceData.Length / 1000}KB...".debug

        Dim emptyProfile As WindowDelta() = New WindowDelta() {}
        Dim results = (From subjectData As KeyValuePair(Of String, PartitioningData) In subject
                       Let subjectFasta As FastaSeq = subjectData.Value.ToFasta
                       Let procResult As KeyValuePair(Of String, WindowDelta()) =
                           processPartitionSubject(subjectData, subjectFasta, emptyProfile, windowCaches, querySource, EXPORT)
                       Where Not procResult.Value.IsNullOrEmpty
                       Select procResult).ToArray
        Dim fileName As String = $"{EXPORT}/Compiled/{querySource.Title.NormalizePathString}_{ptag}.csv"
        Dim file As IO.File = compileSiteDeltaMatrix(results)

        Call Console.WriteLine("[JOB DONE]")

        Return file.Save(fileName, False)
    End Function

    ''' <summary>
    ''' Calculate the ``delta*`` profile of one subject partition against the query
    ''' partition and export the profile as a csv file.
    ''' </summary>
    ''' <param name="subjectData">the subject partition data entry (keyed by the genome id)</param>
    ''' <param name="subjectFasta">the subject partition sequence</param>
    ''' <param name="emptyProfile">the empty profile placeholder returned for the empty subject sequences</param>
    ''' <param name="windowCaches">the pre-built sliding window caches of the query partition</param>
    ''' <param name="querySource">the query partition data (for the output file naming)</param>
    ''' <param name="EXPORT">the csv output directory</param>
    ''' <returns>the subject genome id and its ``delta*`` profile</returns>
    Private Function processPartitionSubject(subjectData As KeyValuePair(Of String, PartitioningData),
                                             subjectFasta As FastaSeq,
                                             emptyProfile As WindowDelta(),
                                             windowCaches As Cache(),
                                             querySource As PartitioningData,
                                             EXPORT As String) As KeyValuePair(Of String, WindowDelta())
        Call $"Start the calculation threads ""{subjectData.Key}""... ".debug

        Dim profile As WindowDelta() = If(String.IsNullOrEmpty(subjectFasta.SequenceData) OrElse subjectFasta.SequenceData.Length = 1,
            emptyProfile,
            slidingWindowDeltaProfile(windowCaches, subjectFasta))
        Dim queryId As String = querySource.Title.Split(CChar("|")).First.NormalizePathString
        Dim subjectId As String = subjectFasta.Title.Split(CChar("|")).First.NormalizePathString
        Dim path As String = $"{EXPORT}/{queryId}-{subjectId}.csv"

        Call $"Calculation job done, trying to export data to filesystem {path}".debug
        Call profile.SaveTo(path, False)

        Return New KeyValuePair(Of String, WindowDelta())(subjectId, profile)
    End Function

    ''' <summary>
    ''' Merge the partition-level ``delta*`` profiles with the two-way BLAST best hit
    ''' rendering data: every window site is annotated with the query genes and the
    ''' matched subject genes.
    ''' </summary>
    ''' <param name="LoadData">the ``subject id -> profile`` pairs sharing the same query genome</param>
    ''' <param name="Query">the gene objects of the query genome</param>
    ''' <param name="render_source">the xml file of the <see cref="SpeciesBesthit"/> rendering data</param>
    ''' <param name="saveto">the merged csv output file path</param>
    ''' <param name="Samples">the row sampling step (1 = keep every row)</param>
    ''' <returns>true when the merged csv document has been saved</returns>
    Private Function mergeDeltaRendering(LoadData As KeyValuePair(Of String, WindowDelta())(),
                                         Query As IEnumerable(Of IGeneBrief),
                                         render_source As String,
                                         saveto As String,
                                         Samples As Integer) As Boolean
        If Samples <= 0 Then
            Samples = 1
        End If

        LoadData = (From item In LoadData Select New KeyValuePair(Of String, WindowDelta())(item.Key, value:=item.Value.sampleEveryNth(Samples))).ToArray

        Dim sitesData = (From site In LoadData.First.Value.AsParallel
                         Let lstName As String() = (From item As IGeneBrief
                                                    In Query.GetObjects(site.site, direction:=Strands.Unknown)
                                                    Select item.Key).ToArray
                         Select New KeyValuePair(Of Integer, String())(site.site, lstName)).ToArray
        ' load the two-way BLAST best hit rendering data of the genome pairs
        Dim renderData As SpeciesBesthit = render_source.LoadXml(Of SpeciesBesthit)()

        ' the genes are marked in the forward direction: a matched gene is colored
        ' with the subject gene id, an unmatched gene keeps an empty id
        Dim csvData As New IO.File
        Dim head As New IO.RowObject From {"Site", "QUERY_ID"}

        For Each item As KeyValuePair(Of String, WindowDelta()) In LoadData
            Call head.Add("")
            Call head.Add(item.Key)
            Call head.Add("Level")
            Call head.Add("GeneID")
        Next

        Call csvData.Add(head)

        For i As Integer = 0 To LoadData.First.Value.Count - 1 ' all of the profiles share the same query genome => the same length
            Dim row As New IO.RowObject From {sitesData(i).Key}

            Call row.Add(String.Join("; ", sitesData(i).Value))

            For Each item As KeyValuePair(Of String, WindowDelta()) In LoadData
                Dim site As WindowDelta = item.Value(i)
                Dim cols = (From id As String In sitesData(i).Value Select renderData(QueryName:=id, HitSpecies:=item.Key)).ToArray

                Call row.Add("")
                Call row.Add(site.deltaStar)
                Call row.Add(site.level.ToString)
                Call row.Add(String.Join("; ", cols))
            Next

            Call csvData.Add(row)
        Next

        Return csvData.Save(saveto, False)
    End Function

    ''' <summary>
    ''' Merge the exported partition-level ``delta*`` profile csv files with the
    ''' two-way BLAST best hit rendering data.
    ''' </summary>
    ''' <param name="source">the source directory that contains the profile csv files</param>
    ''' <param name="query">the gene objects of the query genome</param>
    ''' <param name="render_source">the xml file of the <see cref="SpeciesBesthit"/> rendering data</param>
    ''' <param name="saveto">the merged csv output file path</param>
    ''' <param name="samples">the row sampling step (1 = keep every row)</param>
    ''' <returns>true when the merged csv document has been saved</returns>
    <ExportAPI("rendering_merge.delta_source")>
    Public Function MergeDelta(source As String, query As IEnumerable(Of IGeneBrief), render_source As String, saveto As String, Optional samples As Integer = 1) As Boolean
        Dim loadData = (From path As String
                        In FileIO.FileSystem.GetFiles(source, FileIO.SearchOption.SearchTopLevelOnly, "*.csv").AsParallel
                        Select New KeyValuePair(Of String, WindowDelta())(BaseName(path).Split(CChar("_")).Last,
                             value:=path.LoadCsv(Of WindowDelta)(False).ToArray)).ToArray

        Return mergeDeltaRendering(loadData, query, render_source, saveto, samples)
    End Function

    ''' <summary>
    ''' Build the delta site color rendering data: every window site is annotated
    ''' with its ``delta*`` value, the similarity level and the query / subject gene ids.
    ''' </summary>
    ''' <param name="UID">the subject genome id</param>
    ''' <param name="delta">the ``delta*`` profile of the query genome</param>
    ''' <param name="PTT">the gene table of the query genome</param>
    ''' <param name="render">the two-way BLAST best hit rendering data</param>
    ''' <param name="querySites">the query gene ids on every window site</param>
    ''' <returns>the rendering data rows</returns>
    Private Function colorRender(UID As String,
                                 delta As WindowDelta(),
                                 PTT As PTT,
                                 render As SpeciesBesthit,
                                 querySites As KeyValuePair(Of Integer, String())()) As SegmentRenderData()
        Dim renderData = (From site In delta
                          Let querySite = querySites(site.site)
                          Select New SegmentRenderData With {
                              .site = site.site,
                              .level = site.level,
                              .deltaStar = site.deltaStar,
                              .QueryId = querySite.Value,
                              .SubjectId = (From id As String In querySite.Value Select render(QueryName:=id, HitSpecies:=UID))}).ToArray

        Return renderData
    End Function

#End Region

#Region "CAI codon adaptation compilation"

    ''' <summary>
    ''' Compile the codon usage table of every species gene collection.
    ''' The reference set of each species is its own gene collection
    ''' (for a real high-expression reference set use
    ''' <see cref="ToolsAPI.BuildCAIReference"/> with the ribosomal protein genes).
    ''' </summary>
    ''' <param name="genes">the directory of the ``*.fasta`` / ``*.fsa`` species gene collections</param>
    ''' <returns>the compiled codon usage csv document</returns>
    ''' <remarks>
    ''' Output layout:
    '''
    ''' SpeciesID, CAI, CUBIAS_LIST
    ''' src1 ...
    ''' src2 ...
    ''' </remarks>
    <ExportAPI("cai_bias_dataset")>
    <RApiReturn(GetType(CAIBiasTable))>
    Public Function CompileCABIAS(genes As String) As Object
        Dim files As String() = (ls - l - r - {"*.fasta", "*.fsa", "*.fna"} <= genes).ToArray
        Dim fastaFiles = (From path As String
                          In files.AsParallel
                          Let fasta = FastaFile.LoadNucleotideData(path, True)
                          Where Not fasta.IsNullOrEmpty
                          Select ID = BaseName(path),
                              fasta).ToArray
        Dim caiTables = (From item In fastaFiles.AsParallel
                         Let wTable As CodonWeightTable = New CodonWeightTable(item.fasta, name:=item.ID)
                         Let meanCAI As Double = item.fasta _
                             .Select(Function(gene) gene.CAI(wTable)) _
                             .Where(Function(x) x > 0) _
                             .DefaultIfEmpty(-1) _
                             .Average
                         Select ID = item.ID,
                             MeanCAI = meanCAI,
                             Table = New CodonAdaptationIndex(wTable)).ToArray
        Dim bias_table As CAIBiasTable() = CAIBiasTable.compileCaiTable(caiTables).ToArray

        Return bias_table
    End Function

    ''' <summary>
    ''' Compile the CAI w weight table of one reference gene collection and export
    ''' the codon usage csv table. (legacy batch compilation entry)
    ''' </summary>
    ''' <param name="genes">the reference gene collection of one species</param>
    <ExportAPI("cai_bias_table")>
    <RApiReturn(GetType(CAIBiasTable))>
    Public Function CompileCAIBIASCalculationThread(genes As FastaFile) As Object
        Dim wTable As New CodonWeightTable(genes, name:=genes.FilePath.BaseName)
        Dim meanCAI As Double = genes _
            .Select(Function(gene) gene.CAI(wTable)) _
            .Where(Function(x) x > 0) _
            .DefaultIfEmpty(-1) _
            .Average
        Dim table As New CodonAdaptationIndex(wTable)
        Dim compiledData As (ID As String, MeanCAI As Double, Table As CodonAdaptationIndex)() =
            {(BaseName(genes.FilePath), meanCAI, table)}

        Return CAIBiasTable.compileCaiTable(compiledData).ToArray
    End Function

#End Region

#Region "Partitioning data manipulation"

    ''' <summary>
    ''' Create the chromosome partitioning data from the two-way BLAST best hit
    ''' data: for every partition tag, locate the corresponding ORFs on every
    ''' subject genome and cut the partition nucleotide segments.
    ''' </summary>
    ''' <param name="besthit">the two-way BLAST best hit data of the genome pairs</param>
    ''' <param name="partitions">the partition definitions of the query genome</param>
    ''' <param name="allCDSInfo">the CDS gene table of all of the genomes</param>
    ''' <param name="faDIR">the directory of the subject genome fasta files</param>
    ''' <returns>the partition nucleotide segments of every subject genome</returns>
    <ExportAPI("partition_data.create")>
    Public Function CreateChromesomePartitioningData(besthit As SpeciesBesthit,
                                                     partitions As IEnumerable(Of ChromosomePartitioningEntry),
                                                     allCDSInfo As IEnumerable(Of GeneTable),
                                                     faDIR As String) As PartitioningData()
        Dim resource = faDIR.LoadSourceEntryList({})
        Dim fastaReader = (From entry As KeyValuePair(Of String, String)
                           In resource.AsParallel
                           Let fasta = FastaSeq.Load(entry.Value)
                           Select ID = entry.Key,
                               Reader = fasta) _
                              .ToDictionary(Function(item) item.ID,
                                            Function(item) item.Reader)
        Dim partitionGroups = (From part In partitions
                               Select part
                               Group part By part.PartitioningTag Into Group)
        Dim groupHits = (From nn In partitionGroups.AsParallel
                         Let queryEntries As String() = (From item In nn.Group Select item.ORF).ToArray
                         Select nn.PartitioningTag,
                             besthits = (From item As HitCollection In besthit.hits
                                         Where Array.IndexOf(queryEntries, item.QueryName) > -1
                                         Select item).ToArray).ToArray
        Dim cdsIndex = (From g As GeneTable
                        In allCDSInfo
                        Where Not String.IsNullOrEmpty(g.locus_id)
                        Select g).ToDictionary(Function(obj) obj.locus_id)
        Dim partitionedHits = (From item In groupHits.AsParallel
                               Let hits = groupBesthitsBySpecies(item.besthits)
                               Select item.PartitioningTag,
                                   Data = (From hit In hits Select GenomeID = hit.Key, ORF = (From h In hit.Value Select cdsIndex(h.hitName)).ToArray).AsList) _
                                   .ToDictionary(Function(item) item.PartitioningTag,
                                                 Function(item) item.Data)
        Dim result = (From item In partitionedHits
                      Select (From genome In item.Value
                              Let left As Integer = (From nn In genome.ORF Select nn.left).Min
                              Let right As Integer = (From nn In genome.ORF Select nn.right).Max
                              Let seq As String = readSequence(fastaReader, genome.GenomeID, left, right)
                              Select New PartitioningData With {
                                  .GenomeID = genome.GenomeID,
                                  .LociLeft = left,
                                  .LociRight = right,
                                  .SequenceData = seq,
                                  .ORFList = (From nnnn In genome.ORF Select nnnn.locus_id).ToArray,
                                  .PartitioningTag = item.Key}).ToArray).Unlist
        Dim removed = CType((From item In result.AsParallel Where String.Equals(CType(item.GenomeID, String), CType(besthit.sp, String), StringComparison.OrdinalIgnoreCase) Select item).ToArray, PartitioningData())

        For Each item In removed
            Call result.Remove(item)
        Next

        ' append the query genome partition segments
        For Each p In (From item In partitions Select item Group item By item.PartitioningTag Into Group).ToArray
            Dim left = (From nn In p.Group Let l = cdsIndex(nn.ORF).left Select l).ToArray.Min
            Dim right = (From nn In p.Group Let r = cdsIndex(nn.ORF).right Select r).ToArray.Max
            Dim genomeID As String = besthit.sp
            Dim part As New PartitioningData With {
                .GenomeID = genomeID,
                .LociLeft = left,
                .LociRight = right,
                .SequenceData = readSequence(fastaReader, genomeID, left, right),
                .ORFList = (From nn In p.Group Select nn.ORF).ToArray,
                .PartitioningTag = p.PartitioningTag
            }

            Call result.Add(part)
        Next

        Return result.ToArray
    End Function

    ''' <summary>
    ''' Group the two-way BLAST best hit records by the subject species id.
    ''' </summary>
    ''' <param name="besthits">the best hit records of one partition tag</param>
    ''' <returns>the hit lists keyed by the subject species id</returns>
    Private Function groupBesthitsBySpecies(besthits As HitCollection()) As KeyValuePair(Of String, Hit())()
        Dim grouped = (From o As Hit
                       In (From nn As HitCollection
                           In besthits
                           Select nn.hits).Unlist
                       Group o By o.tag Into Group).ToArray
        Dim result = (From item In grouped
                      Select item.tag,
                          hits = (From fd As Hit In item.Group.ToArray Where Not String.IsNullOrEmpty(fd.hitName) Select fd).ToArray).ToArray

        Return result _
            .Select(Function(item) New KeyValuePair(Of String, Hit())(item.tag, item.hits)) _
            .ToArray
    End Function

    ''' <summary>
    ''' Cut the nucleotide segment ``[left, right]`` from the genome fasta sequence
    ''' of the given genome id (the loci order is normalized internally).
    ''' </summary>
    ''' <param name="fastaReader">the genome fasta readers keyed by the genome id</param>
    ''' <param name="genomeId">the target genome id</param>
    ''' <param name="left">the left loci of the segment</param>
    ''' <param name="right">the right loci of the segment</param>
    ''' <returns>the segment sequence, or an empty string when the genome id is missing</returns>
    Private Function readSequence(fastaReader As Dictionary(Of String, FastaSeq),
                                  genomeId As String,
                                  left As Integer,
                                  right As Integer) As String
        If Not fastaReader.ContainsKey(genomeId) Then
            Call $"The genome id ""{genomeId}"" is not exists in the fasta source...".debug
            Return ""
        End If

        Dim l As Integer = left
        Dim r As Integer = right

        If l > r Then  ' the reverse strand loci: the left is greater than the right
            Dim t = l
            l = r
            r = t
        End If

        Dim reader As FastaSeq = fastaReader(genomeId)
        Dim seq As String = If(reader Is Nothing, "", reader.CutSequenceLinear(l, r))

        Return seq
    End Function

    ''' <summary>
    ''' Save the partitioning data collection as a csv document.
    ''' </summary>
    ''' <param name="dat">the partitioning data collection</param>
    ''' <param name="saveto">the csv output file path</param>
    ''' <returns>true when the csv document has been saved</returns>
    <ExportAPI("write.csv.genome_partition_data")>
    Public Function WritePartionalData(dat As IEnumerable(Of PartitioningData), saveto As String) As Boolean
        Return dat.SaveTo(saveto, silent:=True)
    End Function

    ''' <summary>
    ''' Load the partitioning data collection from its csv document.
    ''' </summary>
    ''' <param name="path">the csv file path</param>
    ''' <returns>the partitioning data collection</returns>
    <ExportAPI("read.csv.genome_partition_data")>
    Public Function ReadPartitioningData(path As String) As PartitioningData()
        Return path.LoadCsv(Of PartitioningData)(mute:=True).ToArray
    End Function

    ''' <summary>
    ''' Load the chromosome partitioning entries from their csv document.
    ''' </summary>
    ''' <param name="path">the csv file path</param>
    ''' <returns>the chromosome partitioning entries</returns>
    <ExportAPI("Read.Csv.Chromsome_Partitioning")>
    Public Function ReadPartitionalData(path As String) As ChromosomePartitioningEntry()
        Return path.LoadCsv(Of ChromosomePartitioningEntry)(mute:=False).ToArray
    End Function

    ''' <summary>
    ''' Calculate the pairwise ``delta*`` homogeneity matrix between all of the
    ''' genome partition segments (the values are reported as ``delta* x 1000``).
    ''' </summary>
    ''' <param name="data">the partition segments of all of the genomes</param>
    ''' <returns>the data frame with one ``Delta(i, j)`` column per pair</returns>
    <ExportAPI("Partition.Similarity.Calculates")>
    Public Function PartitionSimilarity(data As IEnumerable(Of PartitioningData)) As IO.File
        Dim dataArray As PartitioningData() = data.ToArray
        Dim df As DataFrameResolver = DataFrameResolver.CreateObject(dataArray.ToCsvDoc(False))
        Dim dataSource = df.CreateDataSource
        Dim deltas = (From i As Integer
                      In dataArray.Sequence
                      Let info1 As PartitioningData = dataArray(i)
                      Let nt1 = New NucleotideModels.NucleicAcid(info1)
                      Select (From j As Integer
                              In dataArray.Sequence
                              Let info2 As PartitioningData = dataArray(j)
                              Let nt2 = New NucleotideModels.NucleicAcid(info2)
                              Let delta As String = (1000 * DeltaStarDistance.DeltaStar(nt1, nt2)).ToString
                              Select Idx = i, j, delta)).Unlist ' keep the row order: no parallelization here

        For Each row In deltas
            Call row.GetJson.debug
            Call dataSource(row.Idx).SetAttributeValue(String.Format("Delta({0}, {1})", row.Idx, row.j), row.delta)
        Next

        Return df.csv
    End Function

    ''' <summary>
    ''' Create the partition data from a raw data frame: the start / stop columns
    ''' define the segment loci on the given genome sequence.
    ''' </summary>
    ''' <param name="PartitionRaw">the raw partition data frame</param>
    ''' <param name="TagCol">the column name of the partition tag</param>
    ''' <param name="StartTag">the column name of the segment start loci</param>
    ''' <param name="StopTag">the column name of the segment stop loci</param>
    ''' <param name="Nt">the genome sequence</param>
    ''' <returns>the created partition data collection</returns>
    <ExportAPI("Partitions.Creates")>
    Public Function PartionDataCreates(PartitionRaw As DataFrameResolver,
                                       TagCol As String,
                                       StartTag As String,
                                       StopTag As String,
                                       Nt As FastaSeq) As PartitioningData()
        Dim result As PartitioningData() =
            LinqAPI.Exec(Of PartitioningData) <= From row As DynamicObjectLoader
                                                 In PartitionRaw.CreateDataSource
                                                 Let tag As String = row(TagCol)
                                                 Select parsePartitionSequence(row, tag, Nt, StartTag, StopTag, Nt)

        Return result
    End Function

    ''' <summary>
    ''' Cut the partition nucleotide segment from the genome sequence by the
    ''' start / stop loci columns of one data frame row.
    ''' </summary>
    ''' <param name="row">the data frame row</param>
    ''' <param name="tag">the partition tag</param>
    ''' <param name="reader">the genome sequence reader</param>
    ''' <param name="startTag">the start loci column name</param>
    ''' <param name="stopTag">the stop loci column name</param>
    ''' <param name="nt">the genome sequence (for the output genome id)</param>
    ''' <returns>the created partition data of this row</returns>
    Private Function parsePartitionSequence(row As DynamicObjectLoader,
                                            tag$,
                                            reader As IPolymerSequenceModel,
                                            startTag$,
                                            <Parameter("Column.Stop")> stopTag$,
                                            nt As FastaSeq) As PartitioningData
        Dim left As String = row(startTag).Replace(",", "")
        Dim right As String = row(stopTag).Replace(",", "")
        Dim joined As Boolean = False
        Dim seq$

        If InStr(left, "to", CompareMethod.Text) > 0 AndAlso InStr(right, "to", CompareMethod.Text) > 0 Then
            joined = True
            left = left.Split.First
            right = right.Split.Last
        End If

        If joined Then
            seq = reader.CutSequenceCircular(CInt(Val(left)), CInt(Val(right))).SequenceData
        Else
            seq = reader.CutSequenceLinear(CInt(Val(left)), CInt(Val(right) - Val(left))).SequenceData
        End If

        Return New PartitioningData With {
            .PartitioningTag = tag,
            .GenomeID = nt.Title,
            .SequenceData = seq,
            .LociLeft = Val(left),
            .LociRight = Val(right)
        }
    End Function

    ''' <summary>
    ''' Sample one row from every ``sample`` rows of the data collection.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="data"></param>
    ''' <param name="sample">the sampling step</param>
    ''' <returns>the sampled data rows</returns>
    <Extension>
    Private Function sampleEveryNth(Of T)(data As T(), sample As Integer) As T()
        Dim list As New List(Of T)

        For i As Integer = 0 To data.Count - 1 Step sample
            Call list.Add(data(i))
        Next

        Return list.ToArray
    End Function

    ''' <summary>
    ''' Measure the homogeneity property of the genome partition segments against
    ''' the ``dnaA``-``gyrB`` ruler segment of the reference genomes in batch.
    ''' </summary>
    ''' <param name="PartitionData">the partition segments of all of the genomes</param>
    ''' <param name="RuleSource">
    ''' The original GenBank download data directory which should contains the ``*.ptt``
    ''' file for parsing the ruler segment between the ``dnaA`` and ``gyrB`` genes and
    ''' the ``*.fna`` file for parsing the genome nucleotide fasta sequence.
    ''' </param>
    ''' <returns>the data frame with one ruler distance column per reference genome</returns>
    <ExportAPI("measure_homogeneity")>
    Public Function MeasureHomogeneity(PartitionData As IEnumerable(Of PartitioningData), RuleSource As String) As DataFrameResolver
        Dim dataArray As PartitioningData() = PartitionData.ToArray
        Dim df = DataFrameResolver.CreateObject(dataArray.ToCsvDoc(False))
        Call df.AppendLine({"GC%"})

        Dim dfSource = df.CreateDataSource

        For Each folder As String In FileIO.FileSystem.GetDirectories(RuleSource)
            Dim genes = From path
                        In folder.LoadSourceEntryList({"*.ptt"}).AsParallel
                        Let ptt As PTT = PTT.Load(path.Value),
                            ID = path.Key,
                            Name = path.Value.BaseName
                        Where Not ptt Is Nothing
                        Let dnaA = ObjectQuery.MatchGene(ptt, "dnaA", {"chromosomal replication initiator protein DnaA", "chromosomal replication initiator"}),
                            gyrB = ObjectQuery.MatchGene(ptt, "gyrB", {"DNA gyrase B subunit", "DNA gyrase, B subunit"})
                        Where Not (dnaA Is Nothing OrElse gyrB Is Nothing)
                        Select dnaA,
                            gyrB,
                            ID,
                            Name

            For Each locus In genes
                Call locus.debug

                Dim rule As FastaSeq = FastaSeq.LoadNucleotideData(folder & "/" & locus.ID & ".fna")

                If rule Is Nothing Then
                    Continue For
                End If

                Dim reader As IPolymerSequenceModel = rule
                Dim startLoci As Integer = locus.dnaA.Location.left
                Dim stopLoci As Integer = locus.gyrB.Location.right

                If locus.dnaA.Location.Strand = Strands.Reverse Then
                    startLoci = locus.gyrB.Location.left
                    stopLoci = locus.dnaA.Location.right
                End If

                Dim ruleSegment As NucleotideModels.NucleicAcid

                Try
                    ruleSegment = New NucleotideModels.NucleicAcid(reader.CutSequenceLinear(startLoci, stopLoci - startLoci))

                    If ruleSegment.Length > 10 * 1000 Then
                        Continue For
                    End If

                    Call Console.WriteLine($"{locus.dnaA.Gene}  ---> {locus.dnaA.Product}")
                    Call Console.WriteLine($"{locus.gyrB.Gene}  ---> {locus.gyrB.Product}")
                Catch ex As Exception
                    Call App.LogException(ex)
                    Continue For
                End Try

                ' one column of the ruler segment delta* distances per locus,
                ' plus the GC% of the ruler segment on the last row
                For i As Integer = 0 To dfSource.Length - 2
                    Dim partition As DynamicObjectLoader = dfSource(i)
                    Dim sequence = New NucleotideModels.NucleicAcid(dataArray(i).ToFasta)
                    Dim delta As Double = DeltaStarDistance.DeltaStar(sequence, ruleSegment) * 1000

                    Call partition.SetAttributeValue(locus.Name, delta)
                Next

                Call dfSource.Last.SetAttributeValue(locus.Name, ruleSegment.GC)
            Next
        Next

        Return df
    End Function

    ''' <summary>
    ''' Create a delta* distance matrix for a given sequence collection.
    ''' </summary>
    ''' <param name="seqs">the sequence collection</param>
    ''' <returns>the distance matrix</returns>
    <ExportAPI("seq.dist")>
    Public Function dist(<RRawVectorArgument> seqs As Object) As DistanceMatrix
        Throw New NotImplementedException
    End Function

#End Region

#Region "Sequence level delta* utilities"

    ''' <summary>
    ''' The ``delta*`` measure of difference between two sequences f and g (from
    ''' different organisms or from different regions of the same genome): the
    ''' average absolute dinucleotide relative abundance difference
    '''
    ''' ```
    ''' delta*(f, g) = (1/16) SUM |rho*XY(f) - rho*XY(g)|
    ''' ```
    '''
    ''' where the sum extends over all 16 dinucleotides.
    ''' </summary>
    ''' <param name="f">the query sequence</param>
    ''' <param name="g">the subject sequence</param>
    ''' <returns>the delta* value with the similarity level description attached as its unit</returns>
    <ExportAPI("delta_star")>
    Public Function DeltaStar(f As FastaSeq, g As FastaSeq) As vector
        Dim dist As Double = DeltaStarDistance.DeltaStar(f, g)
        Dim desc As String = DeltaStarDistance.DeltaStarLevel(dist).ToString

        Return vector.asVector({dist}, New unit(desc))
    End Function

    ''' <summary>
    ''' Using the DNA segment between the ``dnaA`` and ``gyrB`` genes as the reference
    ''' ruler for the genome homogeneity measurement.
    ''' </summary>
    ''' <param name="nt">a fasta sequence object or an NCBI genbank database object</param>
    ''' <param name="context">the optional PTT gene table of the fasta sequence input</param>
    ''' <param name="env"></param>
    ''' <returns>the ``dnaA``-``gyrB`` ruler segment sequence</returns>
    <ExportAPI("dnaA_gyrB")>
    <RApiReturn(GetType(FastaSeq))>
    Public Function dnaA_gyrB(nt As Object, Optional context As PTT = Nothing, Optional env As Environment = Nothing) As Object
        If nt Is Nothing Then
            Return Nothing
        End If

        If TypeOf nt Is GBFF.File Then
            Return DirectCast(nt, GBFF.File).dnaA_gyrB
        ElseIf TypeOf nt Is FastaSeq Then
            Return DirectCast(nt, FastaSeq).dnaA_gyrB(proteins:=context)
        Else
            Return RInternal.debug.stop({
                "invalid object type for data sequence input...",
                "require: fasta",
                "given: " & nt.GetType.FullName
            }, env)
        End If
    End Function

#End Region

#Region "Karlin 1998: genome signature rho*"

    ''' <summary>
    ''' The ``rho*`` genome signature profile: the double-strand symmetrized
    ''' dinucleotide relative abundance ``rho*XY = fXY / (fX fY)`` of all 16
    ''' dinucleotides, each reported with its six-level significance symbol
    ''' (``---`` / ``--`` / ``-`` / ``+`` / ``++`` / ``+++``, thresholds 0.50 / 0.70 /
    ''' 0.78 / 1.23 / 1.30 / 1.50).
    ''' </summary>
    ''' <param name="nt">the genome nucleotide sequence</param>
    ''' <returns>the ``{XY, rho* [symbol]}`` profile table</returns>
    <ExportAPI("genome.signature_profile")>
    Public Function GenomeSignatureProfile(nt As FastaSeq) As NamedValue(Of String)()
        Return New DeltaSimilarity1998.NucleicAcid(nt).SignatureProfile
    End Function

#End Region

#Region "Karlin 1998: tau* tetranucleotide relative abundance"

    ''' <summary>
    ''' The symmetrized ``tau*`` generalized odds ratio of one tetranucleotide:
    '''
    ''' ```
    ''' tau*(XYZW) = f(XYZW) f(YZ) / ( f(XYZ) f(YZW) )
    ''' ```
    '''
    ''' A value far below 1 indicates a restriction site avoidance signal
    ''' (e.g. the CTAG avoidance in ``M. jannaschii``, tau* = 0.06).
    ''' </summary>
    ''' <param name="nt">the genome nucleotide sequence</param>
    ''' <param name="word">the four-base word under study, e.g. ``"CTAG"``</param>
    ''' <returns>the tau* value; 0 when the word is absent</returns>
    <ExportAPI("tau_star")>
    Public Function TauStar(nt As FastaSeq, word As String) As Double
        Return New DeltaSimilarity1998.NucleicAcid(nt).TauStar(word)
    End Function

    ''' <summary>
    ''' The ``tau*`` tetranucleotide relative abundance profile of all 256
    ''' tetranucleotides.
    ''' </summary>
    ''' <param name="nt">the genome nucleotide sequence</param>
    ''' <returns>the ``{word, tau*}`` dictionary (only the present words are included)</returns>
    <ExportAPI("tau_star.profile")>
    Public Function TetranucleotideProfile(nt As FastaSeq) As Dictionary(Of String, Double)
        Return New DeltaSimilarity1998.NucleicAcid(nt).TauStarProfile
    End Function

    ''' <summary>
    ''' List the rare and the frequent tetranucleotides of the ``tau*`` profile
    ''' against the given thresholds (the restriction avoidance candidates).
    ''' </summary>
    ''' <param name="nt">the genome nucleotide sequence</param>
    ''' <param name="rareBelow">words with ``tau*`` below this value are rare</param>
    ''' <param name="frequentAbove">words with ``tau*`` above this value are frequent</param>
    ''' <returns>a dictionary with the ``rare`` and ``frequent`` word lists</returns>
    <ExportAPI("tau_star.rare_frequent")>
    Public Function RareFrequentTetranucleotides(nt As FastaSeq,
                                                 Optional rareBelow As Double = 0.78,
                                                 Optional frequentAbove As Double = 1.23) As Dictionary(Of String, String())
        Dim profile As Dictionary(Of String, Double) = New DeltaSimilarity1998.NucleicAcid(nt).TauStarProfile
        Dim lists = profile.RareFrequentWords(rareBelow, frequentAbove)

        Return New Dictionary(Of String, String()) From {
            {"rare", lists.Rare},
            {"frequent", lists.Frequent}
        }
    End Function

    ''' <summary>
    ''' Count the absolute occurrences of one oligonucleotide word in the genome
    ''' sequence (e.g. the CTAG rarity report).
    ''' </summary>
    ''' <param name="nt">the genome nucleotide sequence</param>
    ''' <param name="word">the oligonucleotide word to be counted</param>
    ''' <returns>the number of occurrences</returns>
    <ExportAPI("word.count")>
    Public Function OligoCount(nt As FastaSeq, word As String) As Integer
        Return New DeltaSimilarity1998.NucleicAcid(nt).WordCount(word)
    End Function

#End Region

#Region "Karlin 1998: r-scan word spatial distribution statistics"

    ''' <summary>
    ''' Locate all occurrences of one oligonucleotide word in the genome sequence
    ''' (exact match, 0-based start positions).
    ''' </summary>
    ''' <param name="genome">the genome nucleotide sequence</param>
    ''' <param name="word">the oligonucleotide word to be located</param>
    ''' <returns>the 0-based start positions of the word occurrences</returns>
    <ExportAPI("word.locate")>
    Public Function LocateWord(genome As FastaSeq, word As String) As Integer()
        Return New DeltaSimilarity1998.NucleicAcid(genome).LocateWord(word)
    End Function

    ''' <summary>
    ''' Run the r-scan significance test on the spatial distribution of one
    ''' oligonucleotide word (Dembo-Karlin 1992): detect the clustering, the
    ''' overdispersion or the even spacing patterns against the random (Poisson)
    ''' spacing expectation.
    ''' </summary>
    ''' <param name="genome">the genome nucleotide sequence</param>
    ''' <param name="word">the oligonucleotide word under study</param>
    ''' <param name="RMax">the maximum r of the left-tail r-scan table; increase this
    ''' value when the word copy number is large (e.g. the USS / HIP1 elements)</param>
    ''' <returns>the r-scan result with the pattern interpretation</returns>
    <ExportAPI("word.rscan")>
    Public Function WordRScan(genome As FastaSeq,
                              word As String,
                              Optional RMax As Integer = 5) As WordScanResult
        Return New DeltaSimilarity1998.NucleicAcid(genome).RScan(
            word,
            New Microsoft.VisualBasic.Math.Statistics.RScan.RScanOptions With {.RMax = RMax})
    End Function

#End Region

#Region "Karlin 1998: site-specific codon signature"

    ''' <summary>
    ''' Build the site-specific codon signature profile of a gene collection:
    ''' the odds ratios ``rhoXY(1,2) / rhoYZ(2,3) / rhoXZ(1,3)`` within the codons
    ''' and ``rhoZW(3,4)`` at the codon junctions, distinguished from the global
    ''' genome signature.
    ''' </summary>
    ''' <param name="genes">the CDS gene sequences of the gene collection</param>
    ''' <param name="name">the profile name</param>
    ''' <returns>the codon signature profile</returns>
    <ExportAPI("codon.signature_profile")>
    Public Function BuildCodonSignatureProfile(genes As FastaFile,
                                               Optional name As String = Nothing) As CodonSignatureProfile
        Return CodonSignature.CodonSignatureProfile(genes, name)
    End Function

    ''' <summary>
    ''' Build the codon usage frequency profile c(x, y, z) of a gene collection
    ''' (the reference class C of the ``B(F|C)`` codon bias measurement).
    ''' </summary>
    ''' <param name="genes">the CDS gene sequences of the gene collection</param>
    ''' <param name="name">the profile name</param>
    ''' <param name="excludeStopCodons">exclude the stop codons from the frequency total</param>
    ''' <returns>the codon usage profile</returns>
    <ExportAPI("codon.usage_profile")>
    Public Function BuildCodonUsageProfile(genes As FastaFile,
                                           Optional name As String = Nothing,
                                           Optional excludeStopCodons As Boolean = True) As CodonUsageProfile
        Return CodonUsageProfile.FromGenes(genes, name:=name, excludeStopCodons:=excludeStopCodons)
    End Function

#End Region

#Region "Karlin 1998: B(F|C) codon usage bias difference"

    ''' <summary>
    ''' The ``B(F|C)`` codon usage bias difference between two gene classes (the
    ''' paper's formula [1]):
    '''
    ''' ```
    ''' B(F|C) = SUM_a p_a(F) SUM_{(x,y,z)->a} | f(x,y,z) - c(x,y,z) |
    ''' ```
    '''
    ''' the per-codon absolute difference weighted by the amino acid frequency of
    ''' the query class F. Note that this measurement is asymmetric: F is the
    ''' query object and C is the reference ruler.
    ''' </summary>
    ''' <param name="f">the query codon usage profile (gene family F)</param>
    ''' <param name="c">the reference codon usage profile (class C)</param>
    ''' <returns>the B(F|C) value</returns>
    <ExportAPI("codon.bias_B.FC")>
    Public Function CodonBiasDifference(f As CodonUsageProfile, c As CodonUsageProfile) As Double
        Return CodonBiasMeasure.BiasDifference(f, c)
    End Function

    ''' <summary>
    ''' The ``B(g|C)`` codon usage bias difference of one single gene g against the
    ''' reference class C.
    ''' </summary>
    ''' <param name="gene">the query CDS gene sequence</param>
    ''' <param name="reference">the reference codon usage profile (class C)</param>
    ''' <returns>the B(g|C) value</returns>
    <ExportAPI("codon.bias_B.gC")>
    Public Function GeneCodonBias(gene As FastaSeq, reference As CodonUsageProfile) As Double
        Return CodonBiasMeasure.BiasDifference(gene, reference)
    End Function

#End Region

#Region "Karlin 1998: 2D threshold alien gene detection"

    ''' <summary>
    ''' Build the codon usage profile of the ribosomal protein gene set (the
    ''' highly expressed reference set RP of the 2D threshold method).
    ''' </summary>
    ''' <param name="genes">all CDS gene sequences of the genome under study</param>
    ''' <param name="pattern">the ribosomal protein product name matching pattern in the fasta header</param>
    ''' <returns>the ribosomal protein codon usage profile, or Nothing when no
    ''' ribosomal protein gene was matched</returns>
    <ExportAPI("alien.genes.rp_reference")>
    Public Function RibosomalProteinReference(genes As FastaFile,
                                              Optional pattern As String = "ribosomal protein") As CodonUsageProfile
        Dim rpGenes As FastaSeq() = AlienGeneDetection.RibosomalProteinGenes(genes, pattern)

        If rpGenes.IsNullOrEmpty Then
            Return Nothing
        Else
            Return CodonUsageProfile.FromGenes(rpGenes, name:="ribosomal-proteins")
        End If
    End Function

    ''' <summary>
    ''' Identify the alien (laterally transferred) genes with the 2D threshold
    ''' method (the paper's ``B. subtilis`` thresholds by default):
    '''
    ''' B(g|all) &gt; 0.42 and B(g|RP) &gt; 0.45: alien gene - unlike both the average
    ''' host gene and the host highly expressed genes;
    '''
    ''' B(g|all) &gt; 0.42 but B(g|RP) &lt; 0.45: highly expressed host gene.
    ''' </summary>
    ''' <param name="genes">all CDS gene sequences of the genome under study</param>
    ''' <param name="rpPattern">the ribosomal protein product name matching pattern;
    ''' pass an empty string to skip the B(g|RP) axis</param>
    ''' <param name="biasAll">the B(g|all) threshold, default = paper value 0.42</param>
    ''' <param name="biasRP">the B(g|RP) threshold, default = paper value 0.45</param>
    ''' <returns>the classification prediction of every long gene</returns>
    <ExportAPI("alien.genes.scan")>
    Public Function AlienGeneScan(genes As FastaFile,
                                  Optional rpPattern As String = "ribosomal protein",
                                  Optional biasAll As Double = 0.42,
                                  Optional biasRP As Double = 0.45) As AlienGenePrediction()
        Dim referenceAll As CodonUsageProfile = CodonUsageProfile.FromGenes(genes, name:="all-genes")
        Dim referenceRP As CodonUsageProfile = Nothing

        If Not String.IsNullOrEmpty(rpPattern) Then
            referenceRP = RibosomalProteinReference(genes, rpPattern)
        End If

        Dim thresholds As New AlienGeneThresholds With {
            .BiasAll = biasAll,
            .BiasRP = biasRP
        }

        Return AlienGeneDetection.DetectAlienGenes(genes, referenceAll, referenceRP, thresholds)
    End Function

#End Region

#Region "Karlin 1998: sliding window delta* island profiling"

    ''' <summary>
    ''' The sliding window ``delta*`` profile of the genome against its own global
    ''' average genome signature: the alien DNA that has not yet been ameliorated
    ''' shows a signature deviating from the host average and therefore appears as
    ''' peaks on the curve (e.g. the pathogenicity islands).
    ''' </summary>
    ''' <param name="genome">the genome nucleotide sequence</param>
    ''' <param name="windowSize">the sliding window size in bp, default = the paper's 50kb contig size</param>
    ''' <param name="stepSize">the sampling step in bp</param>
    ''' <returns>the profile rows ordered by the site position</returns>
    <ExportAPI("genome.delta_star_islands")>
    Public Function DeltaStarIslands(genome As FastaSeq,
                                     Optional windowSize As Integer = 50000,
                                     Optional stepSize As Integer = 5000) As WindowDelta()
        Return New DeltaSimilarity1998.NucleicAcid(genome) _
            .DeltaStarProfile(windowSize:=windowSize, stepSize:=stepSize)
    End Function

#End Region

#Region "Karlin 1998: strand composition asymmetry"

    ''' <summary>
    ''' The ``(C - G) / (C + G)`` sliding window profile of the genome strand
    ''' composition asymmetry: the leading strand favors purines (G &gt; C), so the
    ''' curve flips its sign at the replication origin oriC.
    ''' </summary>
    ''' <param name="genome">the genome nucleotide sequence</param>
    ''' <param name="windowSize">the sliding window size in bp, default 10kb</param>
    ''' <param name="stepSize">the sampling step in bp, default 1kb</param>
    ''' <returns>the skew profile rows ordered by the site position</returns>
    <ExportAPI("genome.gc_skew")>
    Public Function GenomeGcSkew(genome As FastaSeq,
                                 Optional windowSize As Integer = 10000,
                                 Optional stepSize As Integer = 1000) As StrandSkew()
        Return New DeltaSimilarity1998.NucleicAcid(genome) _
            .GcSkewProfile(windowSize:=windowSize, stepSize:=stepSize)
    End Function

    ''' <summary>
    ''' Locate the candidate replication origin (oriC) and terminus positions from
    ''' the GC skew profile: the sign-flip sites of the cumulative skew curve.
    ''' A single bidirectional replication origin produces one sign flip at oriC;
    ''' genomes with many origins (e.g. some archaea) show no significant
    ''' asymmetry at all.
    ''' </summary>
    ''' <param name="skew">the GC skew profile of <see cref="GenomeGcSkew"/></param>
    ''' <returns>the window site positions where the skew changes sign</returns>
    <ExportAPI("replication_origin.predict")>
    Public Function PredictReplicationOrigin(skew As StrandSkew()) As Integer()
        Return ReplicationAsymmetry.PredictReplicationOrigin(skew)
    End Function

#End Region

End Module
