#Region "Microsoft.VisualBasic::eaa4b343e9755b2b6af7792945ab9765, RNA-Seq\Rockhopper\Java\Peregrine.vb"

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

    '     Class Peregrine
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: getCompressedFileName, getFileNameBase, getGenomeName, getInformalName, getListOfCompressedFileNames
    '                   getPhredOffset, getReadsInputFileType, mapPhred, parametersToString, reverse
    '                   reverseComplement, ToArray
    ' 
    '         Sub: commandLineArguments, Main, mapBamNames, mapHitsToCoordinates, mapReads
    '              output, outputAnnotationStats, outputBrowserFile, outputReadsToFile, outputResults
    '              outputSamHeader, processReads, releaseMemory
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic
Imports System.Text
Imports System.Threading
Imports Oracle.Java.util.concurrent.atomic
Imports Oracle.Java.util.zip

'
' * Copyright 2013 Brian Tjaden
' *
' * This file is part of Rockhopper.
' *
' * Rockhopper is free software: you can redistribute it and/or modify
' * it under the terms of the GNU General Public License as published by
' * the Free Software Foundation, either version 3 of the License, or
' * any later version.
' *
' * Rockhopper is distributed in the hope that it will be useful,
' * but WITHOUT ANY WARRANTY; without even the implied warranty of
' * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' * GNU General Public License for more details.
' *
' * You should have received a copy of the GNU General Public License
' * (in the file gpl.txt) along with Rockhopper.  
' * If not, see <http://www.gnu.org/licenses/>.
' 

Namespace Java

    Public Class Peregrine

        ''' <summary>
        '''******************************************
        ''' **********   Instance Variables   **********
        ''' </summary>

        Private index As Index
        Private coordinates_plus As AtomicIntegerArray()
        ' Number of reads mapping to each coordinate in each sequence
        Private coordinates_minus As AtomicIntegerArray()
        ' Number of reads mapping to each coordinate in each sequence
        Private PHRED_offset As Integer
        Private reader As BufferedReader
        Private reader2 As BufferedReader
        ' Mate-pair file if using paired-end reads
        Private samWriter As PrintWriter
        ' Output reads to SAM file
        Private isCompressedFile As Boolean
        ' True if gzip file. False otherwise.
        Private sam As SamOps
        ' Used if reads file is in SAM/BAM format
        Private firstLineDone As Boolean
        ' Used when reading in the first line of FASTA files
        Private parametersString As String
        Private genomeNames As String()
        ' Used when outputting SAM files
        Private runningTime As Long
        ' Time taken to execute program
        ' Results
        Public totalReads As AtomicInteger
        Private invalidQualityReads As AtomicInteger
        ' Reads containing an invalid quality score
        Private ambiguousReads As AtomicInteger
        ' Reads with ambiguous nucleotides
        Private lowQualityReads As AtomicInteger
        ' Reads with quality scores too low for mapping
        Private mappedOnceReads As AtomicInteger
        ' Reads mapping to one location
        Private mappedMoreThanOnceReads As AtomicInteger
        ' Reads mapping to multiple locations
        Public exactMappedReads As AtomicIntegerArray
        ' Reads mapping without mismatches/errors to each sequence
        Public inexactMappedReads As AtomicIntegerArray
        ' Reads mapping with mismatches/errors to each sequence
        Public avgLengthReads As AtomicLong
        ' Avg length of mapping reads
        Private numMappingReads As AtomicInteger
        ' Number of reads mapping
        Private nameCount As AtomicInteger
        ' Used when outputting SAM file and reads have no name


        ''' <summary>
        '''***************************************
        ''' **********   Class Variables   **********
        ''' </summary>

        ' Parameters
        '  Public Shared output As JTextArea
        ' Output to GUI or, if null, to System.out
        Public Shared summaryWriter As PrintWriter = Nothing
        ' For outputting summary file
        Public Shared numSequences As Integer
        Private Shared readsInputFormat As Integer
        ' 0=FASTQ, 1=QSEQ, 2=FASTA, 3=SAM(single), 4=SAM(paired), 5=BAM(single), 6=BAM(paired)
        Public Shared sequenceFiles As String()
        Public Shared annotationsPlus As String()() = Nothing
        ' Gene/RNA coords for each replicon
        Public Shared annotationsMinus As String()() = Nothing
        ' Gene/RNA coords for each replicon
        Public Shared readsFile As String
        Public Shared percentMismatches As Double = 0.15
        Public Shared stopAfterOneHit As Boolean = True
        Public Shared percentSeedLength As Double = 0.33
        ' Minimum allowed seed (read) length
        Public Shared minReadLength As Integer = 20
        Public Shared isPairedEnd As Boolean = False
        ' Do we have paired-end reads?
        Public Shared pairedEndFile As String
        ' Name of mate-pair file if using paired-end reads
        Public Shared maxPairedEndLength As Integer = 500
        Public Shared singleEndOrientationReverseComplement As Boolean = False
        ' RevComp reads
        Public Shared pairedEndOrientation As String = "fr"
        ' ff, fr, rf, rr
        Public Shared numThreads As Integer = 1
        ' Number of threads
        Public Shared outputSAM As Boolean = False
        ' Output reads to SAM file
        Public Shared outputDIR As String = "Peregrine_Results/"
        ' Output directory
        Private Shared randomSeed As Long = -1
        ' Seed for the random number generator
        Private Shared time As Boolean = False
        ' Output time taken to execute program
        Public Shared browserDIR As String = "genomeBrowserFiles/"
        ' Files for genome browser
        Public Shared wigFiles As List(Of String)
        ' List of comma-delimited set of wig files for IGV
        Public Shared _outputBrowserFile As Boolean = True
        ' Should we output a browser file?


        ''' <summary>
        '''************************************
        ''' **********   Constructors   **********
        ''' </summary>

        Public Sub New()

            runningTime = Oracle.Java.System.CurrentTimeMillis()
            Me.coordinates_plus = New AtomicIntegerArray(Peregrine.numSequences - 1) {}
            Me.coordinates_minus = New AtomicIntegerArray(Peregrine.numSequences - 1) {}
            Me.exactMappedReads = New AtomicIntegerArray(Peregrine.numSequences)
            Me.inexactMappedReads = New AtomicIntegerArray(Peregrine.numSequences)
            If wigFiles Is Nothing Then
                wigFiles = New List(Of String)(numSequences)
                For i As Integer = 0 To numSequences - 1
                    wigFiles.Add("")
                Next
            End If
            parametersString = parametersToString()
            ' For paired-end reads, we must consider all optimal alignments
            If Peregrine.isPairedEnd Then
                Peregrine.stopAfterOneHit = False
            End If

            ' Check if reads hits have been pre-computed
            Dim isValid As Boolean = True
            For i As Integer = 0 To numSequences - 1
                Dim inFileName As String = getCompressedFileName(readsFile, sequenceFiles(i))
                Dim foo As New FileOps(inFileName, readsFile, sequenceFiles(i))
                If foo.valid Then
                    coordinates_plus(i) = New AtomicIntegerArray(foo.coordinates_plus)
                    coordinates_minus(i) = New AtomicIntegerArray(foo.coordinates_minus)
                    totalReads = New AtomicInteger(foo.totalReads)
                    invalidQualityReads = New AtomicInteger(foo.invalidQualityReads)
                    ambiguousReads = New AtomicInteger(foo.ambiguousReads)
                    lowQualityReads = New AtomicInteger(foo.lowQualityReads)
                    mappedOnceReads = New AtomicInteger(foo.mappedOnceReads)
                    mappedMoreThanOnceReads = New AtomicInteger(foo.mappedMoreThanOnceReads)
                    avgLengthReads = New AtomicLong(foo.avgLengthReads)
                    numMappingReads = New AtomicInteger(0)
                    nameCount = New AtomicInteger(1)
                    exactMappedReads.[Set](i, foo.exactMappedReads)
                    inexactMappedReads.[Set](i, foo.inexactMappedReads)
                    runningTime = Oracle.Java.System.CurrentTimeMillis() - runningTime
                    If i = numSequences - 1 Then
                        outputResults()
                    End If
                Else
                    isValid = False
                    totalReads = New AtomicInteger(0)
                    invalidQualityReads = New AtomicInteger(0)
                    ambiguousReads = New AtomicInteger(0)
                    lowQualityReads = New AtomicInteger(0)
                    mappedOnceReads = New AtomicInteger(0)
                    mappedMoreThanOnceReads = New AtomicInteger(0)
                    avgLengthReads = New AtomicLong(0)
                    numMappingReads = New AtomicInteger(0)
                    nameCount = New AtomicInteger(1)
                    exactMappedReads.[Set](i, 0)
                    inexactMappedReads.[Set](i, 0)
                End If
                If wigFiles(i).Length > 0 Then
                    wigFiles(i) = wigFiles(i) & ","
                End If
                wigFiles(i) = wigFiles(i) & outputDIR & browserDIR & inFileName & ".plus" & ".wig"
                wigFiles(i) = wigFiles(i) & "," & outputDIR & browserDIR & inFileName & ".minus" & ".wig"
            Next

            If isValid AndAlso outputSAM Then
                output("Rockhopper did not output a SAM file since sequencing read alignments were previously cached. If you want a SAM file to be output, you must first clear the cache." & vbLf & vbLf)
            End If

            If Not isValid Then
                Me.index = New Index(sequenceFiles)
                readsInputFormat = getReadsInputFileType(readsFile)
                Dim numThreads_OLD As Integer = numThreads
                If (readsInputFormat = 5) OrElse (readsInputFormat = 6) Then
                    ' BAM file
                    numThreads = 1
                End If
                For i As Integer = 0 To numSequences - 1
                    coordinates_plus(i) = New AtomicIntegerArray(index.getReplicon(i).length)
                    coordinates_minus(i) = New AtomicIntegerArray(index.getReplicon(i).length)
                Next
                Me.PHRED_offset = getPhredOffset(readsFile)
                firstLineDone = False
                If outputSAM Then
                    outputSamHeader()
                End If

                ' Set parameters
                index.stopAfterOneHit = Peregrine.stopAfterOneHit
                If randomSeed >= 0 Then
                    Index.randomSeed = Peregrine.randomSeed
                End If

                mapReads()
                runningTime = Oracle.Java.System.CurrentTimeMillis() - runningTime
                numThreads = numThreads_OLD
                ' Reset the number of threads
                If numMappingReads.[Get]() > 0 Then
                    avgLengthReads.[Set](avgLengthReads.[Get]() / numMappingReads.[Get]())
                End If
                avgLengthReads.[Set](Math.Max(avgLengthReads.[Get](), 72))
                ' Minimum of 72 nts
                ' Incorporate results from SAM file
                If sam IsNot Nothing Then
                    Dim samAlignedReads As Integer = 0
                    For i As Integer = 0 To sam.alignedReads.length - 1
                        samAlignedReads += sam.alignedReads.[Get](i)
                    Next
                    If samAlignedReads > 0 Then
                        avgLengthReads.[Set](Math.Max(avgLengthReads.[Get](), sam.avgLengthReads.[Get]() / samAlignedReads))
                    End If
                    totalReads.addAndGet(sam.badReads.[Get]())
                    invalidQualityReads.addAndGet(sam.badReads.[Get]())
                    For i As Integer = 0 To Math.Min(exactMappedReads.length, sam.alignedReads.length) - 1
                        totalReads.addAndGet(sam.alignedReads.[Get](i))
                        exactMappedReads.addAndGet(i, sam.alignedReads.[Get](i))
                        For j As Integer = 0 To Math.Min(coordinates_plus(i).length, sam.coordinates_plus(i).length) - 1
                            coordinates_plus(i).addAndGet(j, sam.coordinates_plus(i).[Get](j))
                            coordinates_minus(i).addAndGet(j, sam.coordinates_minus(i).[Get](j))
                        Next
                    Next
                End If

                If samWriter IsNot Nothing Then
                    samWriter.close()
                End If
                outputResults()
                outputReadsToFile()
            End If
        End Sub



        ''' <summary>
        '''***********************************************
        ''' **********   Public Instance Methods   **********
        ''' </summary>

        ''' <summary>
        ''' Read from reads file and map any reads from file to 
        ''' reference index.
        ''' </summary>
        Public Overridable Sub processReads()
            Dim line As String = ""
            Dim name As String = ""
            Dim nextName As String = ""
            ' Special case (FASTA)
            Dim read As String = ""
            Dim quality As String = ""
            Dim quality_fasta As String = ""
            ' Special case (FASTA)
            Dim qualities As Integer() = New Integer(-1) {}
            Dim parse_line As String() = Nothing
            Dim hits As New List(Of Hit)()
            Dim seedHits As New List(Of Hit)()
            Dim a As New Alignment()
            ' Using paired-end reads
            Dim line2 As String = ""
            Dim name2 As String = ""
            Dim nextName2 As String = ""
            ' Special case (FASTA)
            Dim read2 As String = ""
            Dim quality2 As String = ""
            Dim quality_fasta2 As String = ""
            ' Special case (FASTA)
            Dim qualities2 As Integer() = New Integer(-1) {}
            Dim parse_line2 As String() = Nothing
            Dim hits2 As New List(Of Hit)()
            Dim seedHits2 As New List(Of Hit)()
            Dim combinedHits As New List(Of Hit)()
            Dim a2 As New Alignment()

            Try
                SyncLock reader
                    If (readsInputFormat = 2) AndAlso Not firstLineDone Then
                        ' Special case (FASTA)
                        line = reader.readLine()
                        ' Special case (FASTA)
                        If line.StartsWith(">") Then
                            nextName = line.Substring(1)
                        End If
                        If isPairedEnd Then
                            line2 = reader2.readLine()
                            If line2.StartsWith(">") Then
                                nextName2 = line2.Substring(1)
                            End If
                        End If
                    End If
                    firstLineDone = True
                End SyncLock

                While line IsNot Nothing

                    SyncLock reader
                        ' Thread safe reading from file
                        If readsInputFormat < 5 Then
                            ' Not a BAM file
                            line = reader.readLine()
                        End If
                        If line Is Nothing Then
                            Continue While
                        End If
                        If readsInputFormat = 0 Then
                            ' FASTQ file
                            name = line
                            read = reader.readLine()
                            line = reader.readLine()
                            quality = reader.readLine()
                            ' QSEQ file
                        ElseIf readsInputFormat = 1 Then
                            parse_line = StringSplit(line, vbTab, True)
                            name = parse_line(0) & ":" & parse_line(2) & ":" & parse_line(3) & ":" & parse_line(4) & ":" & parse_line(5) & ":" & parse_line(6) & ":" & parse_line(7)
                            read = parse_line(8)
                            quality = parse_line(9)
                            ' FASTA file
                        ElseIf readsInputFormat = 2 Then
                            name = nextName
                            read = ""
                            While (line IsNot Nothing) AndAlso ((line.Length = 0) OrElse (line(0) <> ">"c))
                                read += line
                                line = reader.readLine()
                            End While
                            If line Is Nothing Then
                                nextName = ""
                            ElseIf line.StartsWith(">") Then
                                nextName = line.Substring(1)
                            Else
                                ' Case should never be reached
                                nextName = ""
                            End If
                            If quality_fasta.Length <> read.Length Then
                                quality_fasta = (New String(New Char(read.Length - 1) {})).Replace(ControlChars.NullChar, "("c)
                            End If
                            quality = quality_fasta
                            ' SAM file (single-end)
                        ElseIf readsInputFormat = 3 Then
                            If SamOps.isHeaderLine(line) Then
                                Continue While
                            End If
                            ' SAM file (paired-end)
                        ElseIf readsInputFormat = 4 Then
                            If SamOps.isHeaderLine(line) Then
                                Continue While
                            End If
                            line2 = reader.readLine()
                            ' BAM file (single-end)
                        ElseIf readsInputFormat = 5 Then
                            parse_line = sam.parseAlignmentLine_BAM()
                            If parse_line Is Nothing Then
                                line = Nothing
                                Continue While
                            End If
                            If parse_line Is SamOps.emptyStringArray Then
                                Continue While
                            End If
                            If parse_line(0).StartsWith("U") Then
                                read = parse_line(9)
                                quality = mapPhred(parse_line(10))
                            ElseIf samWriter IsNot Nothing Then
                                name = parse_line(0).Substring(1)
                                While name.StartsWith("@")
                                    name = name.Substring(1)
                                End While
                                mapBamNames(parse_line)
                                SyncLock samWriter
                                    samWriter.println(name & vbTab & parse_line(1) & vbTab & parse_line(2) & vbTab & parse_line(3) & vbTab & parse_line(4) & vbTab & parse_line(5) & vbTab & parse_line(6) & vbTab & parse_line(7) & vbTab & parse_line(8) & vbTab & parse_line(9) & vbTab & mapPhred(parse_line(10)))
                                    Continue While
                                End SyncLock
                            Else
                                Continue While
                            End If
                            ' BAM file (paired-end)
                        ElseIf readsInputFormat = 6 Then
                            parse_line = sam.parseAlignmentLine_BAM()
                            If parse_line Is Nothing Then
                                line = Nothing
                                Continue While
                            End If
                            parse_line2 = sam.parseAlignmentLine_BAM()
                            If parse_line2 Is Nothing Then
                                line = Nothing
                                Continue While
                            End If
                            If (parse_line Is SamOps.emptyStringArray) OrElse (parse_line2 Is SamOps.emptyStringArray) Then
                                Continue While
                            End If
                            If (parse_line(0).StartsWith("U")) OrElse parse_line2(0).StartsWith("U") Then
                                read = parse_line(9)
                                quality = mapPhred(parse_line(10))
                                read2 = parse_line2(9)
                                quality2 = mapPhred(parse_line2(10))
                            ElseIf samWriter IsNot Nothing Then
                                name = parse_line(0).Substring(1)
                                While name.StartsWith("@")
                                    name = name.Substring(1)
                                End While
                                name2 = parse_line2(0).Substring(1)
                                While name2.StartsWith("@")
                                    name2 = name2.Substring(1)
                                End While
                                mapBamNames(parse_line)
                                mapBamNames(parse_line2)
                                SyncLock samWriter
                                    samWriter.println(name & vbTab & parse_line(1) & vbTab & parse_line(2) & vbTab & parse_line(3) & vbTab & parse_line(4) & vbTab & parse_line(5) & vbTab & parse_line(6) & vbTab & parse_line(7) & vbTab & parse_line(8) & vbTab & parse_line(9) & vbTab & mapPhred(parse_line(10)))
                                    samWriter.println(name2 & vbTab & parse_line2(1) & vbTab & parse_line2(2) & vbTab & parse_line2(3) & vbTab & parse_line2(4) & vbTab & parse_line2(5) & vbTab & parse_line2(6) & vbTab & parse_line2(7) & vbTab & parse_line2(8) & vbTab & parse_line2(9) & vbTab & mapPhred(parse_line2(10)))
                                    Continue While
                                End SyncLock
                            Else
                                Continue While
                            End If
                        End If

                        If isPairedEnd Then
                            ' Using paired-end reads
                            SyncLock reader2
                                ' Thread safe reading from file
                                line2 = reader2.readLine()
                                If line2 Is Nothing Then
                                    output("Error - the two files of mate-pair reads contain different numbers of sequencing reads.")
                                    Continue While
                                End If
                                If readsInputFormat = 0 Then
                                    ' FASTQ file
                                    name2 = line
                                    read2 = reader2.readLine()
                                    line2 = reader2.readLine()
                                    quality2 = reader2.readLine()
                                    ' QSEQ file
                                ElseIf readsInputFormat = 1 Then
                                    parse_line2 = StringSplit(line2, vbTab, True)
                                    name2 = parse_line2(0) & ":" & parse_line2(2) & ":" & parse_line2(3) & ":" & parse_line2(4) & ":" & parse_line2(5) & ":" & parse_line2(6) & ":" & parse_line2(7)
                                    read2 = parse_line2(8)
                                    quality2 = parse_line2(9)
                                    ' FASTA file
                                ElseIf readsInputFormat = 2 Then
                                    name2 = nextName2
                                    read2 = ""
                                    While (line2 IsNot Nothing) AndAlso ((line2.Length = 0) OrElse (line2(0) <> ">"c))
                                        read2 += line2
                                        line2 = reader2.readLine()
                                    End While
                                    If line2 Is Nothing Then
                                        nextName2 = ""
                                    ElseIf line2.StartsWith(">") Then
                                        nextName2 = line2.Substring(1)
                                    Else
                                        ' Case should never be reached
                                        nextName2 = ""
                                    End If
                                    If quality_fasta2.Length <> read2.Length Then
                                        quality_fasta2 = (New String(New Char(read2.Length - 1) {})).Replace(ControlChars.NullChar, "("c)
                                    End If
                                    quality2 = quality_fasta2
                                End If
                            End SyncLock
                        End If
                    End SyncLock

                    If readsInputFormat = 3 Then
                        ' SAM file (single-end)
                        parse_line = sam.parseAlignmentLine_SAM(line)
                        If parse_line Is SamOps.emptyStringArray Then
                            Continue While
                        End If
                        If parse_line(0).StartsWith("U") Then
                            read = parse_line(9)
                            quality = parse_line(10)
                            ' Output to SAM file
                        ElseIf samWriter IsNot Nothing Then
                            name = parse_line(0).Substring(1)
                            While name.StartsWith("@")
                                name = name.Substring(1)
                            End While
                            SyncLock samWriter
                                samWriter.println(name & vbTab & parse_line(1) & vbTab & parse_line(2) & vbTab & parse_line(3) & vbTab & parse_line(4) & vbTab & parse_line(5) & vbTab & parse_line(6) & vbTab & parse_line(7) & vbTab & parse_line(8) & vbTab & parse_line(9) & vbTab & parse_line(10))
                            End SyncLock
                            Continue While
                        Else
                            Continue While
                        End If
                        ' SAM File (paired-end)
                    ElseIf readsInputFormat = 4 Then
                        parse_line = sam.parseAlignmentLine_SAM(line)
                        parse_line2 = sam.parseAlignmentLine_SAM(line2)
                        If (parse_line Is SamOps.emptyStringArray) OrElse (parse_line2 Is SamOps.emptyStringArray) Then
                            Continue While
                        End If
                        If (parse_line(0).StartsWith("U")) OrElse parse_line2(0).StartsWith("U") Then
                            read = parse_line(9)
                            quality = parse_line(10)
                            read2 = parse_line2(9)
                            quality2 = parse_line2(10)
                        ElseIf samWriter IsNot Nothing Then
                            name = parse_line(0).Substring(1)
                            While name.StartsWith("@")
                                name = name.Substring(1)
                            End While
                            name2 = parse_line2(0).Substring(1)
                            While name2.StartsWith("@")
                                name2 = name2.Substring(1)
                            End While
                            SyncLock samWriter
                                samWriter.println(name & vbTab & parse_line(1) & vbTab & parse_line(2) & vbTab & parse_line(3) & vbTab & parse_line(4) & vbTab & parse_line(5) & vbTab & parse_line(6) & vbTab & parse_line(7) & vbTab & parse_line(8) & vbTab & parse_line(9) & vbTab & parse_line(10))
                                samWriter.println(name2 & vbTab & parse_line2(1) & vbTab & parse_line2(2) & vbTab & parse_line2(3) & vbTab & parse_line2(4) & vbTab & parse_line2(5) & vbTab & parse_line2(6) & vbTab & parse_line2(7) & vbTab & parse_line2(8) & vbTab & parse_line2(9) & vbTab & parse_line2(10))
                            End SyncLock
                            Continue While
                        Else
                            Continue While
                        End If
                    End If

                    ' Process read
                    read = read.ToUpper()
                    Dim ambiguous As Boolean = False
                    For i As Integer = 0 To read.Length - 1
                        If (read(i) <> "A"c) AndAlso (read(i) <> "C"c) AndAlso (read(i) <> "G"c) AndAlso (read(i) <> "T"c) Then
                            ambiguous = True
                        End If
                    Next
                    If isPairedEnd OrElse ((sam IsNot Nothing) AndAlso (sam.pairedEnd)) Then
                        ' Using paired-end reads
                        read2 = read2.ToUpper()
                        For i As Integer = 0 To read2.Length - 1
                            If (read2(i) <> "A"c) AndAlso (read2(i) <> "C"c) AndAlso (read2(i) <> "G"c) AndAlso (read2(i) <> "T"c) Then
                                ambiguous = True
                            End If
                        Next

                        ' Orient paired-end reads (0=ff, 1=fr, 2=rf, 3=rr)
                        If pairedEndOrientation(0) = "r"c Then
                            ' Reverse first read
                            read = reverseComplement(read)
                            quality = reverse(quality)
                        End If
                        If pairedEndOrientation(1) = "r"c Then
                            ' Reverse second read
                            read2 = reverseComplement(read2)
                            quality2 = reverse(quality2)
                        End If
                    Else
                        ' Using single-end reads
                        ' Orient single-end reads (true=reverse_complement, false=nothing)
                        If singleEndOrientationReverseComplement Then
                            ' Reverse complement read
                            read = reverseComplement(read)
                            quality = reverse(quality)
                        End If
                    End If

                    ' Process quality
                    If quality.Length <> read.Length Then
                        output("Quality sequence has different length than read sequence." & vbLf)
                        Continue While
                    End If
                    Dim endIndex As Integer = quality.Length - 1
                    While (endIndex >= 0) AndAlso (AscW(quality(endIndex)) - PHRED_offset = 2)
                        endIndex -= 1
                    End While
                    quality = quality.Substring(0, endIndex + 1)
                    read = read.Substring(0, endIndex + 1)
                    If qualities.Length <> read.Length Then
                        qualities = New Integer(read.Length - 1) {}
                    End If
                    Dim invalid As Boolean = False
                    For i As Integer = 0 To quality.Length - 1
                        qualities(i) = AscW(quality(i)) - PHRED_offset
                        If (qualities(i) < 0) OrElse (qualities(i) > 93) Then
                            invalid = True
                        End If
                    Next
                    If isPairedEnd OrElse ((sam IsNot Nothing) AndAlso (sam.pairedEnd)) Then
                        ' Using paired-end reads
                        If quality2.Length <> read2.Length Then
                            output("Quality sequence has different length than read sequence." & vbLf)
                            Continue While
                        End If
                        Dim endIndex2 As Integer = quality2.Length - 1
                        While (endIndex2 >= 0) AndAlso (AscW(quality2(endIndex2)) - PHRED_offset = 2)
                            endIndex2 -= 1
                        End While
                        quality2 = quality2.Substring(0, endIndex2 + 1)
                        read2 = read2.Substring(0, endIndex2 + 1)
                        If qualities2.Length <> read2.Length Then
                            qualities2 = New Integer(read2.Length - 1) {}
                        End If
                        For i As Integer = 0 To quality2.Length - 1
                            qualities2(i) = AscW(quality2(i)) - PHRED_offset
                            If (qualities2(i) < 0) OrElse (qualities2(i) > 93) Then
                                invalid = True
                            End If
                        Next
                    End If

                    ' Align read to genome
                    totalReads.getAndIncrement()

                    If ambiguous Then
                        ambiguousReads.getAndIncrement()
                    ElseIf (quality.Length < minReadLength) OrElse (isPairedEnd AndAlso (quality2.Length < minReadLength)) OrElse ((sam IsNot Nothing) AndAlso (sam.pairedEnd) AndAlso (quality2.Length < minReadLength)) Then
                        ' Minimum read length
                        lowQualityReads.getAndIncrement()
                    ElseIf invalid OrElse (read.Length < minReadLength) Then
                        ' Invalid quality score
                        invalidQualityReads.getAndIncrement()
                    Else
                        index.inexactMatch(read, qualities, a, hits, seedHits)
                        If (isPairedEnd AndAlso (hits.Count > 0)) OrElse ((sam IsNot Nothing) AndAlso (sam.pairedEnd) AndAlso (hits.Count > 0)) Then
                            ' Using paired-end reads
                            index.inexactMatch(read2, qualities2, a2, hits2, seedHits2)
                            Hit.combinePairedEndHits(combinedHits, hits, hits2, maxPairedEndLength)
                            ' Swap "hits" and "combinedHits"
                            Dim temp As List(Of Hit) = hits
                            hits = combinedHits
                            combinedHits = temp
                        End If
                        For i As Integer = 0 To hits.Count - 1
                            ' Update coordinates
                            mapHitsToCoordinates(hits(i))
                        Next
                        If hits.Count > 0 Then
                            ' Update stats
                            If hits(0).errors = 0 Then
                                exactMappedReads.getAndIncrement(hits(0).repliconIndex)
                            Else
                                inexactMappedReads.getAndIncrement(hits(0).repliconIndex)
                            End If
                            If hits.Count = 1 Then
                                mappedOnceReads.getAndIncrement()
                            Else
                                mappedMoreThanOnceReads.getAndIncrement()
                            End If
                            avgLengthReads.addAndGet(hits(0).length)
                            numMappingReads.incrementAndGet()
                        End If
                    End If

                    ' Output to SAM file
                    If samWriter IsNot Nothing Then
                        SyncLock samWriter
                            Dim flag As Integer = 0
                            Dim flag2 As Integer = 0
                            If (quality.Length < minReadLength) OrElse (isPairedEnd AndAlso (quality2.Length < minReadLength)) OrElse ((sam IsNot Nothing) AndAlso (sam.pairedEnd) AndAlso (quality2.Length < minReadLength)) Then
                                ' Do nothing.
                                ' READ TOO SHORT, DON'T USE
                            ElseIf ambiguous OrElse invalid OrElse (hits.Count = 0) Then
                                ' UNALIGNED
                                If isPairedEnd OrElse ((sam IsNot Nothing) AndAlso (sam.pairedEnd)) Then
                                    ' Paired-end
                                    If readsInputFormat = 6 Then
                                        ' BAM
                                        mapBamNames(parse_line)
                                        mapBamNames(parse_line2)
                                        parse_line(10) = mapPhred(parse_line(10))
                                        parse_line2(10) = mapPhred(parse_line2(10))
                                    End If
                                    If (readsInputFormat = 4) OrElse (readsInputFormat = 6) Then
                                        ' SAM/BAM
                                        name = parse_line(0).Substring(1)
                                        While name.StartsWith("@")
                                            name = name.Substring(1)
                                        End While
                                        name2 = parse_line2(0).Substring(1)
                                        While name2.StartsWith("@")
                                            name2 = name2.Substring(1)
                                        End While
                                        samWriter.println(name & vbTab & parse_line(1) & vbTab & parse_line(2) & vbTab & parse_line(3) & vbTab & parse_line(4) & vbTab & parse_line(5) & vbTab & parse_line(6) & vbTab & parse_line(7) & vbTab & parse_line(8) & vbTab & parse_line(9) & vbTab & parse_line(10))
                                        samWriter.println(name2 & vbTab & parse_line2(1) & vbTab & parse_line2(2) & vbTab & parse_line2(3) & vbTab & parse_line2(4) & vbTab & parse_line2(5) & vbTab & parse_line2(6) & vbTab & parse_line2(7) & vbTab & parse_line2(8) & vbTab & parse_line2(9) & vbTab & parse_line2(10))
                                    Else
                                        ' FASTA, QSEQ, FASTQ
                                        flag = 77
                                        ' 01001101
                                        If pairedEndOrientation(0) = "r"c Then
                                            ' Reverse first read
                                            read = reverseComplement(read)
                                            quality = reverse(quality)
                                        End If
                                        While name.StartsWith("@")
                                            name = name.Substring(1)
                                        End While
                                        If name.Length = 0 Then
                                            name = "r" & Convert.ToString(nameCount.getAndIncrement)
                                            name2 = name
                                        End If
                                        samWriter.println(name & vbTab & flag & vbTab & "*" & vbTab & "0" & vbTab & "0" & vbTab & "*" & vbTab & "*" & vbTab & "0" & vbTab & "0" & vbTab & read & vbTab & quality)
                                        flag2 = 141
                                        ' 10001101
                                        If pairedEndOrientation(1) = "r"c Then
                                            ' Reverse second read
                                            read2 = reverseComplement(read2)
                                            quality2 = reverse(quality2)
                                        End If
                                        While name2.StartsWith("@")
                                            name2 = name2.Substring(1)
                                        End While
                                        samWriter.println(name2 & vbTab & flag2 & vbTab & "*" & vbTab & "0" & vbTab & "0" & vbTab & "*" & vbTab & "*" & vbTab & "0" & vbTab & "0" & vbTab & read2 & vbTab & quality2)
                                    End If
                                Else
                                    ' Single-end
                                    If readsInputFormat = 5 Then
                                        ' BAM
                                        mapBamNames(parse_line)
                                        parse_line(10) = mapPhred(parse_line(10))
                                    End If
                                    If (readsInputFormat = 3) OrElse (readsInputFormat = 5) Then
                                        ' SAM/BAM
                                        name = parse_line(0).Substring(1)
                                        While name.StartsWith("@")
                                            name = name.Substring(1)
                                        End While
                                        samWriter.println(name & vbTab & parse_line(1) & vbTab & parse_line(2) & vbTab & parse_line(3) & vbTab & parse_line(4) & vbTab & parse_line(5) & vbTab & parse_line(6) & vbTab & parse_line(7) & vbTab & parse_line(8) & vbTab & parse_line(9) & vbTab & parse_line(10))
                                    Else
                                        ' FASTA, QSEQ, FASTQ
                                        flag = 4
                                        ' Unmapped
                                        If singleEndOrientationReverseComplement Then
                                            ' Reverse complement read
                                            read = reverseComplement(read)
                                            quality = reverse(quality)
                                        End If
                                        While name.StartsWith("@")
                                            name = name.Substring(1)
                                        End While
                                        If name.Length = 0 Then
                                            name = "r" & Convert.ToString(nameCount.getAndIncrement)
                                        End If
                                        samWriter.println(name & vbTab & flag & vbTab & "*" & vbTab & "0" & vbTab & "0" & vbTab & "*" & vbTab & "*" & vbTab & "0" & vbTab & "0" & vbTab & read & vbTab & quality)
                                    End If
                                End If
                            Else
                                ' ALIGNED
                                If isPairedEnd OrElse ((sam IsNot Nothing) AndAlso (sam.pairedEnd)) Then
                                    ' Paired-end (FASTA, QSEQ, FASTQ, SAM, BAM)
                                    flag = 67
                                    ' 01000011
                                    flag2 = 131
                                    ' 10000011
                                    If pairedEndOrientation(0) = "r"c Then
                                        ' Reverse first read
                                        read = reverseComplement(read)
                                        quality = reverse(quality)
                                        If hits(0).strand = "+"c Then
                                            flag += 16
                                            flag2 += 32
                                        End If
                                    Else
                                        If hits(0).strand = "-"c Then
                                            flag += 16
                                            flag2 += 32
                                        End If
                                    End If
                                    If pairedEndOrientation(1) = "r"c Then
                                        ' Reverse second read
                                        read2 = reverseComplement(read2)
                                        quality2 = reverse(quality2)
                                        If hits(0).strand = "+"c Then
                                            flag2 += 16
                                            flag += 32
                                        End If
                                    Else
                                        If hits(0).strand = "+"c Then
                                            flag2 += 16
                                            flag += 32
                                        End If
                                    End If
                                    If (readsInputFormat = 4) OrElse (readsInputFormat = 6) Then
                                        ' SAM/BAM
                                        name = parse_line(0).Substring(1)
                                        name2 = parse_line2(0).Substring(1)
                                    End If
                                    While name.StartsWith("@")
                                        name = name.Substring(1)
                                    End While
                                    While name2.StartsWith("@")
                                        name2 = name2.Substring(1)
                                    End While
                                    If name.Length = 0 Then
                                        name = "r" & Convert.ToString(nameCount.getAndIncrement)
                                        name2 = name
                                    End If
                                    samWriter.println(name & vbTab & flag & vbTab & genomeNames(hits(0).repliconIndex) & vbTab & hits(0).start & vbTab & "255" & vbTab & read.Length & "M" & vbTab & "=" & vbTab & (hits(0).[stop] - read2.Length + 1) & vbTab & hits(0).length & vbTab & read & vbTab & quality)
                                    samWriter.println(name2 & vbTab & flag2 & vbTab & genomeNames(hits(0).repliconIndex) & vbTab & (hits(0).[stop] - read2.Length + 1) & vbTab & "255" & vbTab & read2.Length & "M" & vbTab & "=" & vbTab & hits(0).start & vbTab & (0 - hits(0).length) & vbTab & read2 & vbTab & quality2)
                                Else
                                    ' Single-end (FASTA, QSEQ, FASTQ, SAM, BAM)
                                    flag = 0
                                    If singleEndOrientationReverseComplement Then
                                        ' Reverse complement read
                                        read = reverseComplement(read)
                                        quality = reverse(quality)
                                        If hits(0).strand = "+"c Then
                                            flag += 16
                                        End If
                                    Else
                                        If hits(0).strand = "-"c Then
                                            flag += 16
                                        End If
                                    End If
                                    If (readsInputFormat = 3) OrElse (readsInputFormat = 5) Then
                                        ' SAM/BAM
                                        name = parse_line(0).Substring(1)
                                    End If
                                    While name.StartsWith("@")
                                        name = name.Substring(1)
                                    End While
                                    If name.Length = 0 Then
                                        name = "r" & Convert.ToString(nameCount.getAndIncrement)
                                    End If
                                    samWriter.println(name & vbTab & flag & vbTab & genomeNames(hits(0).repliconIndex) & vbTab & hits(0).start & vbTab & "255" & vbTab & read.Length & "M" & vbTab & "*" & vbTab & "0" & vbTab & "0" & vbTab & read & vbTab & quality)
                                End If
                            End If
                        End SyncLock
                    End If
                End While
            Catch e As IOException
                output("Error - could not read in from file " & readsFile & vbLf & vbLf)
                Environment.[Exit](0)
            End Try
        End Sub

        ''' <summary>
        ''' Return the base file name for the compressed sequencing reads file.
        ''' </summary>
        Public Overridable Function getCompressedFileName(readsFile As String, genomeFile As String) As String
            If Not isPairedEnd Then
                Return getFileNameBase(readsFile) & "_" & getFileNameBase(genomeFile) & parametersString
            Else
                Return getFileNameBase(readsFile) & "_" & getFileNameBase(pairedEndFile) & "_" & getFileNameBase(genomeFile) & parametersString
            End If
        End Function

        ''' <summary>
        ''' Return a list of the base file names for the compressed sequencing reads files.
        ''' </summary>
        Public Overridable Function getListOfCompressedFileNames(readsFile As String) As List(Of String)
            Dim fileNames As New List(Of String)(numSequences)
            For i As Integer = 0 To numSequences - 1
                fileNames.Add(getCompressedFileName(readsFile, sequenceFiles(i)))
            Next
            Return fileNames
        End Function



        ''' <summary>
        '''********************************************
        ''' **********   Public Class Methods   **********
        ''' </summary>

        ''' <summary>
        ''' Output mapped reads to WIG file that can be loaded
        ''' by a genome browser.
        ''' </summary>
        Public Shared Sub outputBrowserFile(readsFile As String, genomeFile As String, dir1 As String, dir2 As String, outFileName As String, coordinates_plus As Integer(),
        coordinates_minus As Integer(), trackRange As Integer)

            Try
                ' Set up directory
                Dim d As New Oracle.Java.IO.File(dir1)
                If Not d.Directory Then
                    d.mkdir()
                End If
                d = New Oracle.Java.IO.File(dir1 & dir2)
                If Not d.Directory Then
                    d.mkdir()
                End If
                Dim out As New PrintWriter(New Oracle.Java.IO.File(dir1 & dir2 & outFileName & ".plus" & ".wig"))
                out.println("track name=" & getFileNameBase(readsFile) & "_PLUS color=0,0,255 graphType=bar viewLimits=0:" & trackRange)
                out.println("fixedStep chrom=" & getGenomeName(genomeFile) & " start=1 step=1")
                For j As Integer = 1 To coordinates_plus.Length - 1
                    out.println(coordinates_plus(j))
                Next
                out.close()
                out = New PrintWriter(New Oracle.Java.IO.File(dir1 & dir2 & outFileName & ".minus" & ".wig"))
                out.println("track name=" & getFileNameBase(readsFile) & "_MINUS color=255,0,0 graphType=bar viewLimits=0:" & trackRange)
                out.println("fixedStep chrom=" & getGenomeName(genomeFile) & " start=1 step=1")
                For j As Integer = 1 To coordinates_minus.Length - 1
                    out.println(coordinates_minus(j))
                Next
                out.close()
            Catch e As IOException
                output("Could not write wiggle file in directory " & (dir1 & dir2) & vbLf)
            End Try
        End Sub

        Public Shared Sub output(s As String)
            If summaryWriter IsNot Nothing Then
                summaryWriter.print(s)
            End If
            Console.Write(s)
        End Sub



        ''' <summary>
        '''************************************************
        ''' **********   Private Instance Methods   **********
        ''' </summary>

        ''' <summary>
        ''' Read in reads file and map all reads to reference
        ''' index using multiple threads.
        ''' </summary>
        Private Sub mapReads()

            Try
                If isCompressedFile Then
                    reader = New BufferedReader(New InputStreamReader(New GZIPInputStream(New FileInputStream(readsFile))))
                Else
                    reader = New BufferedReader(New FileReader(readsFile))
                End If
                If isPairedEnd Then
                    If FileOps.isGZIP(pairedEndFile) Then
                        reader2 = New BufferedReader(New InputStreamReader(New GZIPInputStream(New FileInputStream(pairedEndFile))))
                    Else
                        reader2 = New BufferedReader(New FileReader(pairedEndFile))
                    End If
                End If
                Dim threads As Thread() = New Thread(numThreads - 1) {}
                'For i As Integer = 0 To numThreads - 1
                '    threads(i) = New ProcessReads_Thread(Me)
                '    threads(i).Start()
                'Next
                For i As Integer = 0 To numThreads - 1
                    threads(i).Join()
                Next
                reader.close()
                If isPairedEnd Then
                    reader2.close()
                End If
            Catch e As IOException
                output("Error - could not read from " & readsFile & vbLf & vbLf)
                Environment.[Exit](0)
                'Catch e As InterruptedException
                '    output("Error - thread execution was interrupted." & vbLf & vbLf)
                '    Environment.[Exit](0)
            End Try
        End Sub

        ''' <summary>
        ''' Output the reads aligning to each replicon to a file.
        ''' </summary>
        Private Sub outputReadsToFile()
            For i As Integer = 0 To numSequences - 1
                Dim outFileName As String = getCompressedFileName(readsFile, sequenceFiles(i))
                FileOps.writeCompressedFile(outFileName, readsFile, sequenceFiles(i), index.getReplicon(i).name, index.getReplicon(i).length, totalReads.[Get](),
                invalidQualityReads.[Get](), ambiguousReads.[Get](), lowQualityReads.[Get](), mappedOnceReads.[Get](), mappedMoreThanOnceReads.[Get](), exactMappedReads.[Get](i),
                inexactMappedReads.[Get](i), avgLengthReads.[Get](), ToArray(coordinates_plus(i)), ToArray(coordinates_minus(i)))

                ' Write Wiggle files
                ' If Peregrine.outputBrowserFile Then
                Dim plusCoords As Integer() = New Integer(coordinates_plus(i).length - 1) {}
                Dim minusCoords As Integer() = New Integer(coordinates_minus(i).length - 1) {}
                For j As Integer = 0 To plusCoords.Length - 1
                    plusCoords(j) = coordinates_plus(i).[Get](j)
                Next
                For j As Integer = 0 To minusCoords.Length - 1
                    minusCoords(j) = coordinates_minus(i).[Get](j)
                Next
                Dim lengthOfAllReplicons As Integer = 0
                For k As Integer = 0 To index.numReplicons - 1
                    lengthOfAllReplicons += index.getReplicon(k).length
                Next
                Dim trackRange As Long = 2 * avgLengthReads.[Get]() * CLng(exactMappedReads.[Get](i) + inexactMappedReads.[Get](i)) \ lengthOfAllReplicons
                outputBrowserFile(readsFile, sequenceFiles(i), outputDIR, browserDIR, outFileName, plusCoords,
                    minusCoords, CInt(trackRange))
                ' End If
            Next
        End Sub

        ''' <summary>
        ''' Outputs to STDOUT stats on how many reads align to the genomic sequence.
        ''' </summary>
        Private Sub outputResults()
            output("Total reads:            " & vbTab & Convert.ToString(totalReads) & vbLf)
            'output("Invalid quality reads:  \t" + invalidQualityReads + "\t" + Math.round((100.0*invalidQualityReads.get())/totalReads.get()) + "%" + "\n");
            'output("Ambiguous reads:        \t" + ambiguousReads + "\t" + Math.round((100.0*ambiguousReads.get())/totalReads.get()) + "%" + "\n");
            'output("Low quality reads:      \t" + lowQualityReads + "\t" + Math.round((100.0*lowQualityReads.get())/totalReads.get()) + "%" + "\n");
            'output("Aligned 1 time reads:   \t" + mappedOnceReads + "\t" + Math.round((100.0*mappedOnceReads.get())/totalReads.get()) + "%" + "\n");
            'output("Aligned >1 time reads:   \t" + mappedMoreThanOnceReads + "\t" + Math.round((100.0*mappedMoreThanOnceReads.get())/totalReads.get()) + "%" + "\n");
            'output("Exact alignment reads:  \t" + exactMappedReads + "\t" + Math.round((100.0*exactMappedReads.get())/totalReads.get(i)) + "%" + "\n");
            'output("Inexact alignment reads:\t" + inexactMappedReads + "\t" + Math.round((100.0*inexactMappedReads.get(i))/totalReads.get()) + "%" + "\n");
            'output("\n");
            For i As Integer = 0 To Peregrine.numSequences - 1
                output("Successfully aligned reads:" & vbTab + (exactMappedReads.[Get](i) + inexactMappedReads.[Get](i)) & vbTab & Math.Round((100.0 * (exactMappedReads.[Get](i) + inexactMappedReads.[Get](i))) / totalReads.[Get]()) & "%" & vbTab & "(" & getInformalName(sequenceFiles(i)) & ")" & vbLf)
                ' Number of reads aligning genes and RNAs
                outputAnnotationStats(i)
            Next
            output(vbLf)

            If samWriter IsNot Nothing Then
                output("Sequencing reads written to SAM file:" & vbTab & outputDIR & getFileNameBase(readsFile) & ".sam" & vbLf & vbLf)
            End If

            If time Then
                ' Output time taken to execute program
                output("Time taken to align reads:" & vbTab & (runningTime \ 60000) & " minutes " & ((runningTime Mod 60000) \ 1000) & " seconds" & vbLf)
            End If
        End Sub

        ''' <summary>
        ''' Determines the file format of the input reads file.
        ''' Returns 0 if the file is in FASTQ format.
        ''' Returns 1 if the file is in QSEQ format.
        ''' Returns 2 if the file is in FASTA format.
        ''' Returns 3 if the file is in SAM single-end format.
        ''' Returns 4 if the file is in SAM paired-end format.
        ''' Returns 5 if the file is in BAM single-end format.
        ''' Returns 6 if the file is in BAM paired-end format.
        ''' </summary>
        Private Function getReadsInputFileType(readsFile As String) As Integer
            Dim MAX_LINES As Integer = 10000
            Dim lineCounter As Integer = 0
            Dim fastqCount As Integer = 0
            Dim qseqCount As Integer = 0
            Dim fastaCount As Integer = 0
            Try
                ' First check if we have a SAM file
                sam = New SamOps(readsFile, MAX_LINES, False)
                If sam.validSam OrElse sam.validBam Then
                    Dim replicons As String() = sam.replicons
                    output(vbLf & "User should make sure that reference replicons input by the user correspond to those in SAM/BAM file." & vbLf)
                    output(vbTab & "Reference replicons input by user:" & vbLf)
                    For i As Integer = 0 To numSequences - 1
                        output(vbTab & vbTab & getGenomeName(sequenceFiles(i)) & " with length " & (index.getReplicon(i).length - 1) & " nts" & vbLf)
                    Next
                    output(vbTab & "Reference replicons found in SAM file:" & vbLf)
                    For i As Integer = 0 To replicons.Length - 1
                        output(vbTab & vbTab & replicons(i) & " with length " & sam.getRepliconLength(i) & " nts" & vbLf)
                    Next
                    output(vbLf)
                    If sam.validSam AndAlso Not sam.pairedEnd Then
                        ' Single-end SAM file
                        Return 3
                    ElseIf sam.validSam Then
                        ' Paired-end SAM file
                        Peregrine.stopAfterOneHit = False
                        Return 4
                        ' Single-end BAM file
                    ElseIf sam.validBam AndAlso Not sam.pairedEnd Then
                        Return 5
                    ElseIf sam.validBam Then
                        ' Paired-end BAM file
                        Peregrine.stopAfterOneHit = False
                        Return 6
                    Else
                        ' This case should never be reached
                        Return -1
                    End If
                Else
                    sam = Nothing
                End If

                Dim reader As Scanner = Nothing
                isCompressedFile = FileOps.isGZIP(readsFile)
                If isCompressedFile Then
                    reader = New Scanner(New GZIPInputStream(New FileInputStream(readsFile)))
                Else
                    reader = New Scanner(New Oracle.Java.IO.File(readsFile))
                End If
                Dim line As String
                While reader.hasNextLine() AndAlso (lineCounter < MAX_LINES)
                    line = reader.nextLine()
                    If line.Length > 0 Then
                        If line(0) = "@"c Then
                            fastqCount += 1
                        ElseIf StringSplit(line, vbTab, True).Length >= 11 Then
                            qseqCount += 1
                        ElseIf line(0) = ">"c Then
                            fastaCount += 1
                        End If
                    End If
                    lineCounter += 1
                End While
                reader.close()
                If fastqCount > Math.Max(qseqCount, fastaCount) Then
                    Return 0
                ElseIf qseqCount > fastaCount Then
                    Return 1
                ElseIf fastaCount > 0 Then
                    Return 2
                Else
                    output("Error - could not recognize format of file " & readsFile & vbLf & vbLf)
                    Environment.[Exit](0)
                End If
            Catch e As IOException
                output(vbLf & "Error - could not open file " & readsFile & vbLf & vbLf)
                Environment.[Exit](0)
            End Try
            Return -1
        End Function

        ''' <summary>
        ''' Depending on version of Solexa machine used, the quality
        ''' score may range from 35 (or 37) up to 73, or it may range
        ''' from 66 up to 104. To obtain the Phred score, we subtract 
        ''' either 33 or 64, depending on the version. Here, we get the
        ''' maximum quality score over all nucleotides in the first
        ''' 100,000 reads (it should be either 73 or 104). The offset 
        ''' is the number we must subtract from this maximum to 
        ''' obtain 40, i.e., either 33 or 64.
        ''' </summary>
        Private Function getPhredOffset(readsFile As String) As Integer
            Dim lineCounter As Integer = 0
            Try
                Dim maxQuality As Integer = Integer.MinValue
                If (readsInputFormat = 5) OrElse (readsInputFormat = 6) Then
                    ' BAM file
                    maxQuality = sam.maxQuality
                Else
                    Dim reader As Scanner = Nothing
                    If isCompressedFile Then
                        reader = New Scanner(New GZIPInputStream(New FileInputStream(readsFile)))
                    Else
                        reader = New Scanner(New Oracle.Java.IO.File(readsFile))
                    End If
                    Dim line As String
                    Dim read As String = ""
                    Dim quality As String = ""

                    While reader.hasNextLine() AndAlso (lineCounter < 100000)

                        line = reader.nextLine()
                        If readsInputFormat = 0 Then
                            ' FASTQ file
                            read = reader.nextLine()
                            line = reader.nextLine()
                            quality = reader.nextLine()
                            ' QSEQ file
                        ElseIf readsInputFormat = 1 Then
                            Dim parse_line As String() = StringSplit(line, vbTab, True)
                            read = parse_line(8)
                            quality = parse_line(9)
                            ' FASTA file
                        ElseIf readsInputFormat = 2 Then
                            read = reader.nextLine()
                            ' ASCII value of 40
                            quality = "("
                            ' SAM file
                        ElseIf (readsInputFormat = 3) OrElse (readsInputFormat = 4) Then
                            If line.StartsWith("@") Then
                                read = ""
                                quality = ""
                            Else
                                Dim parse_line As String() = StringSplit(line, vbTab, True)
                                read = parse_line(9)
                                quality = parse_line(10)
                                If quality.Equals("*") Then
                                    quality = ""
                                End If
                            End If
                        End If

                        ' Process quality
                        For i As Integer = 0 To quality.Length - 1
                            maxQuality = Math.Max(maxQuality, AscW(quality(i)))
                        Next

                        lineCounter += 1
                    End While
                    reader.close()
                End If

                ' Are we Phred+33 or Phred+64?
                If Math.Abs(maxQuality - 40 - 33) < Math.Abs(maxQuality - 40 - 64) Then
                    Return 33
                Else
                    Return 64
                End If
            Catch e As IOException
                output(vbLf & "Error - could not open file " & readsFile & vbLf & vbLf)
                Environment.[Exit](0)
            End Try
            Return -1
        End Function

        ''' <summary>
        ''' Given the hit of a read mapping to the genome, we update
        ''' the coordinates corresponding to the read hit.
        ''' </summary>
        Private Sub mapHitsToCoordinates(a As Hit)
            Dim coordinates As AtomicIntegerArray = Me.coordinates_plus(a.repliconIndex)
            ' Plus strand
            If a.strand = "-"c Then
                ' Minus strand
                coordinates = Me.coordinates_minus(a.repliconIndex)
            End If
            For i As Integer = a.start To a.[stop]
                coordinates.getAndIncrement(i)
            Next
        End Sub

        ''' <summary>
        ''' Output number of reads aligning sense/antisense to
        ''' annotated genes and RNAs in replicon i.
        ''' </summary>
        Private Sub outputAnnotationStats(i As Integer)
            If (annotationsPlus Is Nothing) OrElse annotationsMinus Is Nothing Then
                Return
            End If
            If (i >= annotationsPlus.Length) OrElse (i >= annotationsMinus.Length) Then
                Return
            End If

            Dim gene_sense As Long = 0
            Dim gene_antisense As Long = 0
            Dim rRNA_sense As Long = 0
            Dim rRNA_antisense As Long = 0
            Dim tRNA_sense As Long = 0
            Dim tRNA_antisense As Long = 0
            Dim miscRNA_sense As Long = 0
            Dim miscRNA_antisense As Long = 0
            Dim IG As Long = 0

            Dim annotationPlus As String() = annotationsPlus(i)
            Dim annotationMinus As String() = annotationsMinus(i)
            Dim coordinate_plus As AtomicIntegerArray = coordinates_plus(i)
            Dim coordinate_minus As AtomicIntegerArray = coordinates_minus(i)
            For j As Integer = 1 To coordinate_plus.length - 1
                ' 1-indexed
                ' There are five possibilities on each strand (rRNA, tRNA, RNA, gene, empty).
                ' We handle all twenty-five cases.
                If annotationPlus(j).Equals("rRNA") Then
                    ' Plus rRNA
                    rRNA_sense += coordinate_plus.[Get](j)
                    If annotationMinus(j).Length = 0 Then
                        rRNA_antisense += coordinate_minus.[Get](j)
                    ElseIf annotationMinus(j).Equals("rRNA") Then
                        rRNA_sense += coordinate_minus.[Get](j)
                    ElseIf annotationMinus(j).Equals("tRNA") Then
                        tRNA_sense += coordinate_minus.[Get](j)
                    ElseIf annotationMinus(j).Equals("RNA") Then
                        miscRNA_sense += coordinate_minus.[Get](j)
                    Else
                        gene_sense += coordinate_minus.[Get](j)
                    End If
                    ' Plus tRNA
                ElseIf annotationPlus(j).Equals("tRNA") Then
                    tRNA_sense += coordinate_plus.[Get](j)
                    If annotationMinus(j).Length = 0 Then
                        tRNA_antisense += coordinate_minus.[Get](j)
                    ElseIf annotationMinus(j).Equals("rRNA") Then
                        rRNA_sense += coordinate_minus.[Get](j)
                    ElseIf annotationMinus(j).Equals("tRNA") Then
                        tRNA_sense += coordinate_minus.[Get](j)
                    ElseIf annotationMinus(j).Equals("RNA") Then
                        miscRNA_sense += coordinate_minus.[Get](j)
                    Else
                        gene_sense += coordinate_minus.[Get](j)
                    End If
                    ' Plus misc RNA
                ElseIf annotationPlus(j).Equals("RNA") Then
                    miscRNA_sense += coordinate_plus.[Get](j)
                    If annotationMinus(j).Length = 0 Then
                        miscRNA_antisense += coordinate_minus.[Get](j)
                    ElseIf annotationMinus(j).Equals("rRNA") Then
                        rRNA_sense += coordinate_minus.[Get](j)
                    ElseIf annotationMinus(j).Equals("tRNA") Then
                        tRNA_sense += coordinate_minus.[Get](j)
                    ElseIf annotationMinus(j).Equals("RNA") Then
                        miscRNA_sense += coordinate_minus.[Get](j)
                    Else
                        gene_sense += coordinate_minus.[Get](j)
                    End If
                    ' Plus protein-coding gene
                ElseIf annotationPlus(j).Length > 0 Then
                    gene_sense += coordinate_plus.[Get](j)
                    If annotationMinus(j).Length = 0 Then
                        gene_antisense += coordinate_minus.[Get](j)
                    ElseIf annotationMinus(j).Equals("rRNA") Then
                        rRNA_sense += coordinate_minus.[Get](j)
                    ElseIf annotationMinus(j).Equals("tRNA") Then
                        tRNA_sense += coordinate_minus.[Get](j)
                    ElseIf annotationMinus(j).Equals("RNA") Then
                        miscRNA_sense += coordinate_minus.[Get](j)
                    Else
                        gene_sense += coordinate_minus.[Get](j)
                    End If
                Else
                    ' Plus no annotation
                    If annotationMinus(j).Equals("rRNA") Then
                        rRNA_sense += coordinate_minus.[Get](j)
                        rRNA_antisense += coordinate_plus.[Get](j)
                    ElseIf annotationMinus(j).Equals("tRNA") Then
                        tRNA_sense += coordinate_minus.[Get](j)
                        tRNA_antisense += coordinate_plus.[Get](j)
                    ElseIf annotationMinus(j).Equals("RNA") Then
                        miscRNA_sense += coordinate_minus.[Get](j)
                        miscRNA_antisense += coordinate_plus.[Get](j)
                    ElseIf annotationMinus(j).Length > 0 Then
                        gene_sense += coordinate_minus.[Get](j)
                        gene_antisense += coordinate_plus.[Get](j)
                    Else
                        IG += coordinate_minus.[Get](j)
                        IG += coordinate_plus.[Get](j)
                    End If
                End If
            Next

            ' Output stats
            Dim total As Long = gene_sense + gene_antisense + rRNA_sense + rRNA_antisense + tRNA_sense + tRNA_antisense + miscRNA_sense + miscRNA_antisense + IG
            output(vbTab & "Aligning (sense) to protein-coding genes:" & vbTab & Math.Round(100.0 * gene_sense / total) & "%" & vbLf)
            output(vbTab & "Aligning (antisense) to protein-coding genes:" & vbTab & Math.Round(100.0 * gene_antisense / total) & "%" & vbLf)
            output(vbTab & "Aligning (sense) to ribosomal RNAs:" & vbTab & Math.Round(100.0 * rRNA_sense / total) & "%" & vbLf)
            output(vbTab & "Aligning (antisense) to ribosomal RNAs:" & vbTab & Math.Round(100.0 * rRNA_antisense / total) & "%" & vbLf)
            output(vbTab & "Aligning (sense) to transfer RNAs:" & vbTab & Math.Round(100.0 * tRNA_sense / total) & "%" & vbLf)
            output(vbTab & "Aligning (antisense) to transfer RNAs:" & vbTab & Math.Round(100.0 * tRNA_antisense / total) & "%" & vbLf)
            output(vbTab & "Aligning (sense) to miscellaneous RNAs:" & vbTab & Math.Round(100.0 * miscRNA_sense / total) & "%" & vbLf)
            output(vbTab & "Aligning (antisense) to miscellaneous RNAs:" & vbTab & Math.Round(100.0 * miscRNA_antisense / total) & "%" & vbLf)
            output(vbTab & "Aligning to unannotated regions:        " & vbTab & Math.Round(100.0 * IG / total) & "%" & vbLf)
        End Sub

        ''' <summary>
        ''' Open SAM file for writing.
        ''' Output header information to SAM file.
        ''' </summary>
        Private Sub outputSamHeader()
            Dim samFileName As String = outputDIR & getFileNameBase(readsFile) & ".sam"
            If readsFile.Equals(samFileName) Then
                ' Do not read and write same file at same time
                Return
            End If
            Try
                samWriter = New PrintWriter(New Oracle.Java.IO.File(samFileName))
                samWriter.println("@HD" & vbTab & "VN:1.0" & vbTab & "SO:unsorted")
                genomeNames = New String(sequenceFiles.Length - 1) {}
                For i As Integer = 0 To sequenceFiles.Length - 1
                    samWriter.println("@SQ" & vbTab & "SN:" & getGenomeName(sequenceFiles(i)) & vbTab & "LN:" & (index.getReplicon(i).length - 1) & vbTab & "SP:" & getInformalName(sequenceFiles(i)))
                    genomeNames(i) = getGenomeName(sequenceFiles(i))
                Next
                samWriter.println("@PG" & vbTab & "ID:Rockhopper" & vbTab & "PN:Rockhopper" & vbTab & "VN:" & version)
            Catch f As IOException
                samWriter = Nothing
            End Try
        End Sub

        ''' <summary>
        ''' Takes a String array representing one line from a BAM file and
        ''' we map the repliconIndex (at parse_line[2] and parse_line[6])
        ''' to the corresponding replicon name.
        ''' </summary>
        Private Sub mapBamNames(parse_line As String())
            If parse_line(2).Equals("*") Then
                parse_line(6) = "*"
            Else
                Dim repliconIndex As Integer = 0
                Try
                    repliconIndex = Convert.ToInt32(parse_line(2))
                Catch e As NumberFormatException
                    parse_line(2) = "*"
                    parse_line(6) = "*"
                    Return
                End Try
                If repliconIndex < 0 Then
                    parse_line(2) = "*"
                    parse_line(6) = "*"
                Else
                    parse_line(2) = genomeNames(repliconIndex)
                End If
                If Not parse_line(6).Equals("*") Then
                    If parse_line(2).Equals(parse_line(6)) Then
                        parse_line(6) = "="
                    Else
                        Try
                            repliconIndex = Convert.ToInt32(parse_line(6))
                        Catch e As NumberFormatException
                            parse_line(6) = "*"
                            Return
                        End Try
                        If repliconIndex < 0 Then
                            parse_line(6) = "*"
                        Else
                            parse_line(6) = genomeNames(repliconIndex)
                        End If
                    End If
                End If
            End If
        End Sub

        ''' <summary>
        ''' Return a String representation of the parameter settings.
        ''' </summary>
        Private Function parametersToString() As String
            Return "_v" & numSequences & "_m" & CInt(Math.Truncate(100 * percentMismatches)) & "_a" & ("" & stopAfterOneHit).ToUpper()(0) & "_d" & maxPairedEndLength & "_l" & CInt(Math.Truncate(100 * percentSeedLength)) & "_" & pairedEndOrientation & "_c" & ("" & singleEndOrientationReverseComplement).ToUpper()(0)
        End Function

        ''' <summary>
        ''' Returns an int[] version of an atomic integer array.
        ''' </summary>
        Private Function ToArray(a As AtomicIntegerArray) As Integer()
            Dim b As Integer() = New Integer(a.length - 1) {}
            For i As Integer = 0 To a.length - 1
                b(i) = a.[Get](i)
            Next
            Return b
        End Function

        ''' <summary>
        ''' Returns a reversed version of String s.
        ''' </summary>
        Private Function reverse(s As String) As String
            'JAVA TO C# CONVERTER TODO TASK: There is no .NET StringBuilder equivalent to the Java 'reverse' method:
            Return (New StringBuilder(s)).Reverse().ToString()
        End Function

        ''' <summary>
        ''' Returns a reverse complemented version of String s.
        ''' </summary>
        Private Function reverseComplement(s As String) As String
            Dim sb As New StringBuilder(s.Length)
            For i As Integer = s.Length - 1 To 0 Step -1
                If s(i) = "A"c Then
                    sb.Append("T"c)
                ElseIf s(i) = "C"c Then
                    sb.Append("G"c)
                ElseIf s(i) = "G"c Then
                    sb.Append("C"c)
                ElseIf s(i) = "T"c Then
                    sb.Append("A"c)
                Else
                    sb.Append(s(i))
                End If
            Next
            Return sb.ToString()
        End Function



        ''' <summary>
        '''*********************************************
        ''' **********   Private Class Methods   **********
        ''' </summary>

        ''' <summary>
        ''' Free up memory by deleting class variables that
        ''' we are finished with.
        ''' </summary>
        Public Shared Sub releaseMemory()
            annotationsPlus = Nothing
            annotationsMinus = Nothing
        End Sub



        ''' <summary>
        '''*********************************************
        ''' **********   Private Class Methods   **********
        ''' </summary>

        ''' <summary>
        ''' Returns the base of the specified file name, i.e., 
        ''' the file extension and path to the file are excluded.
        ''' </summary>
        Private Shared Function getFileNameBase(fileName As String) As String
            Dim slashIndex As Integer = fileName.LastIndexOf(Oracle.Java.IO.File.separator)
            If slashIndex = -1 Then
                slashIndex = fileName.LastIndexOf("/")
            End If
            Dim periodIndex As Integer = fileName.IndexOf("."c, slashIndex + 1)
            If periodIndex = -1 Then
                periodIndex = fileName.Length
            End If
            Return fileName.Substring(slashIndex + 1, periodIndex - (slashIndex + 1))
        End Function

        ''' <summary>
        ''' Returns the name of the replicon from the FASTA header line,
        ''' e.g., gi|59800473|ref|NC_002946.2|
        ''' </summary>
        Private Shared Function getGenomeName(fileName As String) As String
            Dim genomeName As String = ""
            Try
                Dim reader As New Scanner(New Oracle.Java.IO.File(fileName))
                Dim line As String = reader.nextLine()
                Dim parse_line As String() = StringSplit(line, "\s+", True)
                genomeName = parse_line(0).Substring(1)
                reader.close()
            Catch e As IOException
                output("Error - could not read in from file " & fileName & vbLf & vbLf)
                Environment.[Exit](0)
            End Try
            Return genomeName
        End Function

        ''' <summary>
        ''' Returns the informal name of the replicon from the FASTA header line,
        ''' e.g., Escherichia coli str. K-12 substr. MG1655 chromosome
        ''' </summary>
        Private Shared Function getInformalName(fileName As String) As String
            Dim genomeName As String = ""
            Try
                Dim reader As New Scanner(New Oracle.Java.IO.File(fileName))
                Dim line As String = reader.nextLine()
                Dim parse_line As String() = StringSplit(line, "\|", True)
                If parse_line.Length > 4 Then
                    ' Appropriately formatted FASTA header
                    Dim parse_name As String() = StringSplit(parse_line(4), ",", True)
                    genomeName = parse_name(0).Trim()
                Else
                    ' Unexpected format for FASTA header
                    genomeName = line.Trim()
                End If
                reader.close()
            Catch e As IOException
                output("Error - could not read in from file " & fileName & vbLf & vbLf)
                Environment.[Exit](0)
            End Try
            Return genomeName
        End Function

        ''' <summary>
        ''' Checks if quality scores specified by parameter String
        ''' need to be adjusted by a Phred offset of 33. If not,
        ''' the original String is returned. If so, a new
        ''' adjusted String is returned.
        ''' </summary>
        Private Shared Function mapPhred(qualities As String) As String
            Dim offset As Integer = 33
            Dim maxPhred As Integer = 104
            Dim needsAdjusting As Boolean = False
            Dim needsResetting As Boolean = False
            For i As Integer = 0 To qualities.Length - 1
                If AscW(qualities(i)) < offset Then
                    needsAdjusting = True
                End If
                If AscW(qualities(i)) > maxPhred Then
                    needsResetting = True
                End If
            Next
            If Not needsAdjusting AndAlso Not needsResetting Then
                Return qualities
            End If
            Dim sb As New StringBuilder(qualities.Length)
            For i As Integer = 0 To qualities.Length - 1
                If needsAdjusting Then
                    sb.Append(ChrW(offset + qualities(i).Asc))
                ElseIf needsResetting Then
                    sb.Append("("c)
                End If
            Next
            Return sb.ToString()
        End Function

        Private Shared Sub commandLineArguments(args As String())
            If args.Length < 2 Then
                Oracle.Java.System.Err.println(vbLf & "The Peregrine application takes one or more files of genomic sequences (such as FASTA genome files) and a file of RNA-seq reads and it aligns the reads to the genomic sequences.")
                Oracle.Java.System.Err.println(vbLf & "The Peregrine application has two required command line arguments.")
                Oracle.Java.System.Err.println(vbLf & "REQUIRED ARGUMENTS" & vbLf)
                Oracle.Java.System.Err.println(vbTab & "a comma-separated list of files containing replicon sequences in FASTA format")
                Oracle.Java.System.Err.println(vbTab & "a file <FASTQ,QSEQ,FASTA> of RNA-seq reads")

                Oracle.Java.System.Err.println(vbLf & "OPTIONAL ARGUMENTS" & vbLf)
                Oracle.Java.System.Err.println(vbTab & "-2 <file>" & vbTab & "a file <FASTQ,QSEQ,FASTA> of mate-paired RNA-seq reads, (default is single-end rather than paired-end reads)")
                Oracle.Java.System.Err.println(vbTab & "-c <boolean>" & vbTab & "reverse complement single-end reads (default is false)")
                Oracle.Java.System.Err.println(vbTab & "-ff/fr/rf/rr" & vbTab & "orientation of two mate reads for each paired-end read, f=forward and r=reverse_complement (default is fr)")
                Oracle.Java.System.Err.println(vbTab & "-m <number>" & vbTab & "allowed mismatches as percent of read length (default is 0.15)")
                Oracle.Java.System.Err.println(vbTab & "-a <boolean>" & vbTab & "report 1 alignment (true) or report all optimal alignments (false), (default is true)")
                Oracle.Java.System.Err.println(vbTab & "-d <integer>" & vbTab & "maximum number of bases between mate pairs for paired-end reads (default is 500)")
                Oracle.Java.System.Err.println(vbTab & "-l <number>" & vbTab & "minimum seed as percent of read length (default is 0.33)")
                Oracle.Java.System.Err.println(vbTab & "-p <integer>" & vbTab & "number of processors (default is self-identification of processors)")
                Oracle.Java.System.Err.println(vbTab & "-o <DIR>" & vbTab & "directory where output files are written (default is Peregrine_Results/)")
                Oracle.Java.System.Err.println(vbTab & "-time      " & vbTab & "output time taken to execute program")

                Oracle.Java.System.Err.println(vbLf & "java Peregrine <options> NC_002505.fna,NC_002506.fna reads.fastq" & vbLf)
                Environment.[Exit](0)
            End If

            ' Initially set the number of threads
            Peregrine.numThreads = Oracle.Java.System.Runtime.Runtime.availableProcessors()
            If Peregrine.numThreads > 4 Then
                Peregrine.numThreads *= 0.75
            End If

            Dim i As Integer = 0
            While i < args.Length - 2
                If args(i).StartsWith("-2") Then
                    If (i = args.Length - 3) OrElse (args(i + 1).StartsWith("-")) Then
                        Oracle.Java.System.Err.println("Error - command line argument -2 must be followed by a mate-paired sequencing read file.")
                        Environment.[Exit](0)
                    End If
                    Peregrine.isPairedEnd = True
                    Peregrine.pairedEndFile = args(i + 1)
                    i += 2
                ElseIf args(i).StartsWith("-c") Then
                    If (i = args.Length - 3) OrElse (args(i + 1).StartsWith("-")) Then
                        Oracle.Java.System.Err.println("Error - command line argument -c must be followed by a boolean.")
                        Environment.[Exit](0)
                    End If
                    Peregrine.singleEndOrientationReverseComplement = Convert.ToBoolean(args(i + 1))
                    i += 2
                ElseIf args(i).Equals("-ff") Then
                    Peregrine.pairedEndOrientation = "ff"
                    i += 1
                ElseIf args(i).Equals("-fr") Then
                    Peregrine.pairedEndOrientation = "fr"
                    i += 1
                ElseIf args(i).Equals("-rf") Then
                    Peregrine.pairedEndOrientation = "rf"
                    i += 1
                ElseIf args(i).Equals("-rr") Then
                    Peregrine.pairedEndOrientation = "rr"
                    i += 1
                ElseIf args(i).StartsWith("-m") Then
                    If (i = args.Length - 3) OrElse (args(i + 1).StartsWith("-")) Then
                        Oracle.Java.System.Err.println("Error - command line argument -m must be followed by a decimal number.")
                        Environment.[Exit](0)
                    End If
                    Peregrine.percentMismatches = Convert.ToDouble(args(i + 1))
                    i += 2
                ElseIf args(i).StartsWith("-a") Then
                    If (i = args.Length - 3) OrElse (args(i + 1).StartsWith("-")) Then
                        Oracle.Java.System.Err.println("Error - command line argument -a must be followed by a boolean.")
                        Environment.[Exit](0)
                    End If
                    Peregrine.stopAfterOneHit = Convert.ToBoolean(args(i + 1))
                    i += 2
                ElseIf args(i).StartsWith("-d") Then
                    If (i = args.Length - 3) OrElse (args(i + 1).StartsWith("-")) Then
                        Oracle.Java.System.Err.println("Error - command line argument -d must be followed by an integer.")
                        Environment.[Exit](0)
                    End If
                    Peregrine.maxPairedEndLength = Convert.ToInt32(args(i + 1))
                    SamOps.maxPairedEndLength = Convert.ToInt32(args(i + 1))
                    i += 2
                ElseIf args(i).StartsWith("-l") Then
                    If (i = args.Length - 3) OrElse (args(i + 1).StartsWith("-")) Then
                        Oracle.Java.System.Err.println("Error - command line argument -l must be followed by a decimal number.")
                        Environment.[Exit](0)
                    End If
                    Peregrine.percentSeedLength = Convert.ToDouble(args(i + 1))
                    i += 2
                ElseIf args(i).StartsWith("-p") Then
                    If (i = args.Length - 3) OrElse (args(i + 1).StartsWith("-")) Then
                        Oracle.Java.System.Err.println("Error - command line argument -p must be followed by an integer.")
                        Environment.[Exit](0)
                    End If
                    Peregrine.numThreads = Convert.ToInt32(args(i + 1))
                    i += 2
                ElseIf args(i).StartsWith("-o") Then
                    If (i = args.Length - 3) OrElse (args(i + 1).StartsWith("-")) Then
                        Oracle.Java.System.Err.println("Error - command line argument -o must be followed by a directory path.")
                        Environment.[Exit](0)
                    End If
                    Peregrine.outputDIR = args(i + 1)
                    If Peregrine.outputDIR(outputDIR.Length - 1) <> "/"c Then
                        outputDIR += "/"c
                    End If
                    i += 2
                ElseIf args(i).StartsWith("-r") Then
                    If (i = args.Length - 3) OrElse (args(i + 1).StartsWith("-")) Then
                        Oracle.Java.System.Err.println("Error - command line argument -r must be followed by a long integer.")
                        Environment.[Exit](0)
                    End If
                    Peregrine.randomSeed = Convert.ToInt64(args(i + 1))
                    i += 2
                ElseIf args(i).Equals("-time") Then
                    Peregrine.time = True
                    i += 1
                Else
                    Oracle.Java.System.Err.println("Error - could not recognize command line argument: " & args(i))
                    Environment.[Exit](0)
                End If
            End While

            ' Determine required input files
            Peregrine.sequenceFiles = StringSplit(args(i), ",", True)
            Peregrine.readsFile = args(i + 1)
            Peregrine.numSequences = Peregrine.sequenceFiles.Length

            ' Handle erroneous command line arguments
            For j As Integer = 0 To sequenceFiles.Length - 1
                If Not (System.IO.Directory.Exists(sequenceFiles(j)) OrElse System.IO.File.Exists(sequenceFiles(j))) Then
                    Oracle.Java.System.Err.println("Error - file " & sequenceFiles(j) & " does not exist.")
                    Environment.[Exit](0)
                End If
            Next
            If Not (System.IO.Directory.Exists(readsFile) OrElse System.IO.File.Exists(readsFile)) Then
                Oracle.Java.System.Err.println("Error - file " & readsFile & " does not exist.")
                Environment.[Exit](0)
            End If
            If isPairedEnd AndAlso Not (System.IO.Directory.Exists(pairedEndFile) OrElse System.IO.File.Exists(pairedEndFile)) Then
                Oracle.Java.System.Err.println("Error - file " & pairedEndFile & " does not exist.")
                Environment.[Exit](0)
            End If
        End Sub



        ''' <summary>
        '''***********************************
        ''' **********   Main Method   **********
        ''' </summary>

        Private Shared Sub Main(args As String())

            commandLineArguments(args)
            Dim p As New Peregrine()

        End Sub

    End Class



    '''' <summary>
    ''''*******************************************
    '''' **********   ProcessReads_Thread   **********
    '''' </summary>

    'Friend Class ProcessReads_Thread
    '    Inherits System.Threading.Thread

    '    Private p As Peregrine

    '    Public Sub New(p As Peregrine)
    '        Me.p = p
    '    End Sub

    '    Public Overridable Sub run()
    '        p.processReads()
    '    End Sub
    'End Class
End Namespace
