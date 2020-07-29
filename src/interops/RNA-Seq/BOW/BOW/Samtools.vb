#Region "Microsoft.VisualBasic::9e61c5ac4610542973da962933423192, RNA-Seq\BOW\BOW\Samtools.vb"

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



    ' /********************************************************************************/

    ' Summaries:

    ' Module Samtools
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: Assembly, Bam2Sam, GetBitFLAGSDescription, Import, Index
    '               Indexing, ReadSam, Sam2Bam, SaveCsv, Sort
    '               Viewing
    ' 
    '     Sub: __release, Assembly
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.SAM

''' <summary>
''' SAMtools is a set of utilities for interacting with and post-processing short DNA sequence read alignments 
''' in the SAM, BAM and CRAM formats, written by Heng Li. These files are generated as output by short read 
''' aligners like BWA. Both simple and advanced tools are provided, supporting complex tasks like variant calling 
''' and alignment viewing as well as sorting, indexing, data extraction and format conversion.[2] SAM files can 
''' be very large (10s of Gigabytes is common), so compression is used to save space. SAM files are human-readable 
''' text files, and BAM files are simply their binary equivalent, whilst CRAM files are a restructured column-oriented 
''' binary container format. BAM files are typically compressed and more efficient for software to work with than SAM. 
''' 
''' SAMtools makes it possible to work directly with a compressed BAM file, without having to uncompress the whole file. 
''' Additionally, since the format for a SAM/BAM file is somewhat complex - containing reads, references, alignments, 
''' quality information, and user-specified annotations - SAMtools reduces the effort needed to use SAM/BAM files by 
''' hiding low-level details.
''' </summary>
<Package("Samtools", Description:="Tools for alignments in the SAM format, 
SAMtools is a set of utilities for interacting with and post-processing short DNA sequence read alignments in the SAM, BAM and CRAM formats, 
written by Heng Li. These files are generated as output by short read aligners like BWA. Both simple and advanced tools are provided, 
supporting complex tasks like variant calling and alignment viewing as well as sorting, indexing, data extraction and format conversion.
SAM files can be very large (10s of Gigabytes is common), so compression is used to save space. SAM files are human-readable text files, 
and BAM files are simply their binary equivalent, whilst CRAM files are a restructured column-oriented binary container format. 

BAM files are typically compressed and more efficient for software to work with than SAM. SAMtools makes it possible to work directly with 
a compressed BAM file, without having to uncompress the whole file. Additionally, since the format for a SAM/BAM file is somewhat complex - 
containing reads, references, alignments, quality information, and user-specified annotations - SAMtools reduces the effort needed to use 
SAM/BAM files by hiding low-level details.")>
Public Module Samtools

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="SAM"></param>
    ''' <param name="TrimError"></param>
    ''' <param name="EXPORT">The data export directory path</param>
    ''' <returns></returns>
    <ExportAPI("Assembly")>
    Public Function Assembly(SAM As SAM,
                             <Parameter("Trim.Error")>
                             Optional TrimError As Boolean = True,
                             Optional EXPORT As String = "") As FASTA.FastaFile

        Dim Forwards As Contig() = Nothing, Reversed As Contig() = Nothing
        Call SAM.Assembling(Forwards, Reversed, TrimError)

        Dim Fasta = Forwards.Join(Reversed)
        Dim FastaFile = CType((From Contig In Fasta.AsParallel Select Contig.ToFastaToken).ToArray, FASTA.FastaFile)

        If Not String.IsNullOrEmpty(EXPORT) Then

            Dim Csv = (From Contig As Contig
                       In Fasta.AsParallel
                       Select Loci = Contig.Location.ToString,
                           FLAGS = String.Join(" / ", Contig.FLAGS)).ToArray
            Call Csv.SaveTo(EXPORT)
        End If

        Return FastaFile
    End Function

    <ExportAPI("Assembly")>
    Public Sub Assembly(<Parameter("Dir.SAM")> SAMSource As String,
                             <Parameter("Dir.Export")> EXPORT As String,
                             <Parameter("Trim.Error")> Optional TrimError As Boolean = True, Optional Parallel As Boolean = False, Optional CsvExport As String = "")

        Dim source = (From pathEntry In SAMSource.LoadSourceEntryList({"*.sam"})
                      Select name = pathEntry.Key,
                             SAM = pathEntry.Value,
                             SavedFasta = $"{EXPORT}/{pathEntry.Key}.fasta").ToArray
        If Parallel Then
            Dim LQuery = (From File In source.AsParallel
                          Let SAM = SAM.Load(File.SAM)
                          Let Fasta = Assembly(SAM, TrimError, CsvExport)
                          Select Fasta.Save(File.SavedFasta)).ToArray
        Else
            For Each File In source
                Dim SAM As SAM = SAM.Load(File.SAM)
                Dim Fasta = Assembly(SAM, TrimError, CsvExport)
                Call Fasta.Save(File.SavedFasta)
            Next
        End If
    End Sub

    <ExportAPI("Read.SAM")>
    Public Function ReadSam(Path As String, <Parameter("UnMapped.Trim")> Optional TrimUnMapped As Boolean = False) As SAM
        Dim doc = SAM.Load(Path)
        If TrimUnMapped Then
            Call Console.WriteLine("Trim unmapped alignment reads.....")
            doc = doc.TrimUnmappedReads
            Call Console.WriteLine($"[DEBUG {Now.ToString}] Alignment reads data trimming job done!")
        End If
        Call Console.WriteLine("Returns sam data.")
        Return doc
    End Function

    <ExportAPI("BitFLAGS")>
    Public Function GetBitFLAGSDescription(BitFLAG As Integer) As String
        Dim FLAGS As BitFlags() = BitFLAGS_API.ComputeBitFLAGS(BitFLAG)
        Dim s As String = BitFLAGS_API.GetBitFLAGDescriptions(FLAGS)
        Return s
    End Function

    <ExportAPI("SavedAsCsv", Info:="Only for debugging....")>
    Public Function SaveCsv(SAM As SAM, SaveTo As String) As Boolean
        Return SAM.AlignmentsReads.SaveTo(SaveTo, False)
    End Function

    'view
    'Convert a bam file into a sam file.

    '  samtools view sample.bam > sample.sam

    'Convert a sam file into a bam file. The -b Option compresses Or leaves compressed input data.

    '  samtools view -bS sample.sam > sample.bam

    'Extract all the reads aligned To the range specified, which are those that are aligned To the reference element 
    'named chr1 And cover its 10th, 11th, 12th Or 13th base. The results Is saved To a BAM file including the header. 
    'An index Of the input file Is required For extracting reads according To their mapping position In the reference 
    'genome, As created by samtools index.

    '  samtools view sample_sorted.bam "chr1:10-13"

    'Extract the same reads As above, but instead Of displaying them, writes them To a New bam file, tiny.bam. 
    'The -b Option makes the output compressed And the -h Option causes the SAM headers To be output also. 
    'These headers include a description Of the reference that the reads In sample.bam were aligned To And will be 
    'needed If the tiny.bam file Is To be used With some Of the more advanced SAMtools commands. The order Of extracted 
    'reads Is preserved.

    'samtools view -h -b sample_sorted.bam "chr1:10-13" > tiny_sorted.bam

    'tview

    'samtools tview sample_sorted.bam

    'Start an interactive viewer To visualize a small region Of the reference, the reads aligned, And mismatches. 
    'Within the view, can jump To a New location by typing g: And a location, Like g:chr1:10,000,000. If the reference 
    'element name And following colon Is replaced With {{{1}}}, the current reference element Is used, i.e. 
    'If {{{1}}} Is typed after the previous "goto" command, the viewer jumps To the region 200 base pairs down On chr1. 
    'Typing ? brings up help information.

    'sort

    'samtools sort unsorted_in.bam sorted_out

    'Read the specified unsorted_in.bam As input, sort it by aligned read position, And write it out To sorted_out.bam, 
    'the bam file whose name (without extension) was specified.

    'samtools sort -m 5000000 unsorted_in.bam sorted_out

    'Read the specified unsorted_in.bam As input, sort it In blocks up To 5 million k (5 Gb) [TODO: verify units here, this could be wrong] 
    'And write output To a series Of bam files named sorted_out.0000.bam, sorted_out.0001.bam, etc., where all bam 0 reads come before 
    'any bam 1 read, etc. [TODO: verify this Is correct].

    'index

    'samtools index sorted.bam

    'Creates an index file, sorted.bam.bai For the sorted.bam file.

    Private Sub __release()
        Call CliResCommon.TryRelease(NameOf(My.Resources.bcftools))
        Call CliResCommon.TryRelease(NameOf(My.Resources.bgzip))
        Call CliResCommon.TryRelease(NameOf(My.Resources.razip))
    End Sub

    Sub New()
        Call __release()
    End Sub

    ''' <summary>
    ''' Like bwa, Samtools also requires us to go through several steps before we have our data in usable form. 
    ''' First, we need to have Samtools generate its own index of the reference genome
    ''' </summary>
    ''' <param name="Fasta"></param>
    ''' <returns></returns>
    <ExportAPI("faidx", Info:="Like bwa, Samtools also requires us to go through several steps before we have our data in usable form. First, we need to have Samtools generate its own index of the reference genome")>
    Public Function Indexing(<Parameter("In.Fasta")> Fasta As String) As String
        Dim cli As String = $"faidx {Fasta.CLIPath}"
        Dim Path As String = Fasta & ".fai"

        Try

            If Not 0 = New CommandLine.IORedirectFile(TryRelease(NameOf(My.Resources.samtools)), cli).Run Then
                Return ""
            End If

        Catch ex As Exception
            Call Console.WriteLine(ex.ToString)
            Return ""
        End Try

        Return Path
    End Function

    ''' <summary>
    ''' Next, we need to convert the SAM file into a BAM file. (A BAM file is just a binary version of a SAM file.)
    ''' </summary>
    ''' <returns></returns>
    <ExportAPI("import", Info:="Next, we need to convert the SAM file into a BAM file. (A BAM file is just a binary version of a SAM file.)")>
    Public Function Import(<Parameter("In.Sam")> Sam As String,
                            <Parameter("In.Ref_List")> Fai As String,
                            <Parameter("Out.Bam")> Bam As String) As Boolean
        Dim cli As String = $"import {Fai.CLIPath} {Sam.CLIPath} {Bam.CLIPath}"

        Try

            Return 0 = New CommandLine.IORedirectFile(TryRelease(NameOf(My.Resources.samtools)), cli).Run

        Catch ex As Exception
            Call Console.WriteLine(ex.ToString)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Now, we need to sort the BAM file. The sort command sorts a BAM file based on its position in the reference, 
    ''' as determined by its alignment. The element + coordinate in the reference that the first matched base in the 
    ''' read aligns to is used as the key to order it by. [TODO: verify]. The sorted output is dumped to a new file 
    ''' by default, although it can be directed to stdout (using the -o option). As sorting is memory intensive and 
    ''' BAM files can be large, this command supports a sectioning mode (with the -m options) to use at most a given 
    ''' amount of memory and generate multiple output file. These files can then be merged to produce a complete 
    ''' sorted BAM file [TODO - investigate the details of this more carefully].
    ''' (请注意，本函数只能够对bam文件进行排序，假若需要对sam文件进行排序，请先转换为bam文件)
    ''' </summary>
    ''' <param name="Bam"></param>
    ''' <param name="Out"></param>
    ''' <returns></returns>
    <ExportAPI("Sort", Info:="Now, we need to sort the BAM file, The sort command sorts a BAM file based on its position in the reference, as determined by its alignment. 
The element + coordinate in the reference that the first matched base in the read aligns to is used as the key to order it by. [TODO: verify]. 
The sorted output is dumped to a new file by default, although it can be directed to stdout (using the -o option). As sorting is memory intensive 
and BAM files can be large, this command supports a sectioning mode (with the -m options) to use at most a given amount of memory and generate 
multiple output file. These files can then be merged to produce a complete sorted BAM file [TODO - investigate the details of this more carefully].")>
    Public Function Sort(<Parameter("in.bam")> Bam As String,
                         <Parameter("out.prefix")> Out As String,
                         <Parameter("Sort.Name")> Optional SortByName As Boolean = True) As Boolean

        Dim cli As String = If(SortByName, $"sort -n {Bam.CLIPath} {Out.CLIPath}", $"sort {Bam.CLIPath} {Out.CLIPath}")

        Try

            Return 0 = New CommandLine.IORedirectFile(TryRelease(NameOf(My.Resources.samtools)), cli).Run

        Catch ex As Exception
            Call Console.WriteLine(ex.ToString)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' And last, we need Samtools to index the BAM file. The index command creates a new index file that allows fast look-up of data in a 
    ''' (sorted) SAM or BAM. Like an index on a database, the generated *.sam.sai or *.bam.bai file allows programs that can read it to 
    ''' more efficiently work with the data in the associated files.
    ''' </summary>
    ''' <param name="Bam"></param>
    ''' <returns></returns>
    <ExportAPI("index", Info:="And last, we need Samtools to index the BAM file. The index command creates a new index file that allows fast 
look-up of data in a (sorted) SAM or BAM. Like an index on a database, the generated *.sam.sai or *.bam.bai file allows programs that can 
read it to more efficiently work with the data in the associated files.")>
    Public Function Index(<Parameter("in.bam")> Bam As String,
                          <Parameter("out.index")> Optional Out As String = "") As Boolean
        Dim cli As String = If(String.IsNullOrEmpty(Out), $"index {Bam.CLIPath}", $"index {Bam.CLIPath} {Out.CLIPath}")

        Try

            Return 0 = New CommandLine.IORedirectFile(TryRelease(NameOf(My.Resources.samtools)), cli).Run

        Catch ex As Exception
            Call Console.WriteLine(ex.ToString)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Sam">The input source file path of the sam file.</param>
    ''' <param name="Bam">The save file path of the converted bam file.</param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("Sam2Bam", Info:="Convert the text file format mapping file into the binary format of the mapping file.")>
    Public Function Sam2Bam(Sam As String, Bam As String) As Boolean
        Dim Cli As String = $"view -bS -o {Bam.CLIPath} {Sam.CLIPath}"

        Try
            Return 0 = New Microsoft.VisualBasic.CommandLine.IORedirectFile(CliResCommon.TryRelease(NameOf(My.Resources.samtools)), Cli).Run()
        Catch ex As Exception
            Call Console.WriteLine(ex.ToString)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Viewing the output with TView. Now that we've generated the files, we can view the output with TView.
    ''' The tview command starts an interactive ascii-based viewer that can be used to visualize how reads are aligned to specified 
    ''' small regions of the reference genome. Compared to a graphics based viewer like IGV,[3] it has few features. Within the view, 
    ''' it is possible to jumping to different positions along reference elements (using 'g') and display help information ('?').
    ''' </summary>
    ''' <param name="Bam"></param>
    ''' <param name="Fasta"></param>
    ''' <returns></returns>
    <ExportAPI("tview", Info:="Viewing the output with TView. Now that we've generated the files, we can view the output with TView. 
The tview command starts an interactive ascii-based viewer that can be used to visualize how reads are aligned to specified small 
regions of the reference genome. Compared to a graphics based viewer like IGV,[3] it has few features. Within the view, it is 
possible to jumping to different positions along reference elements (using 'g') and display help information ('?').")>
    Public Function Viewing(<Parameter("aln.bam")> Bam As String,
                            <Parameter("ref.fasta")> Optional Fasta As String = "") As Boolean
        Dim cli As String = If(String.IsNullOrEmpty(Fasta), $"tview {Bam.CLIPath}", $"tview {Bam.CLIPath} {Fasta.CLIPath}")

        Try
            Return 0 = New Microsoft.VisualBasic.CommandLine.IORedirectFile(CliResCommon.TryRelease(NameOf(My.Resources.samtools)), cli).Run()
        Catch ex As Exception
            Call Console.WriteLine(ex.ToString)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Convert the binary format Sam mapping file into the text format Sam mapping file.
    ''' The view command filters SAM or BAM formatted data. Using options and arguments it understands what data to select 
    ''' (possibly all of it) and passes only that data through. Input is usually a sam or bam file specified as an argument, 
    ''' but could be sam or bam data piped from any other command. Possible uses include extracting a subset of data into a 
    ''' new file, converting between BAM and SAM formats, and just looking at the raw file contents. 
    ''' The order of extracted reads is preserved.
    ''' </summary>
    ''' <param name="Bam"></param>
    ''' <param name="Sam"></param>
    ''' <returns></returns>
    <ExportAPI("Bam2Sam", Info:="The view command filters SAM or BAM formatted data. Using options and arguments it understands what data 
to select (possibly all of it) and passes only that data through. Input is usually a sam or bam file specified as an argument, 
but could be sam or bam data piped from any other command. Possible uses include extracting a subset of data into a new file, 
converting between BAM and SAM formats, and just looking at the raw file contents. The order of extracted reads is preserved.")>
    Public Function Bam2Sam(Bam As String, Sam As String) As Boolean
        Dim Cli As String = $"view -h -o {Sam.CLIPath} {Bam.CLIPath}"

        Try
            Return 0 = New Microsoft.VisualBasic.CommandLine.IORedirectFile(CliResCommon.TryRelease(NameOf(My.Resources.samtools)), Cli).Run()
        Catch ex As Exception
            Call Console.WriteLine(ex.ToString)
            Return False
        End Try
    End Function
End Module
