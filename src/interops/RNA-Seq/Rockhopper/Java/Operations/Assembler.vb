#Region "Microsoft.VisualBasic::58692078a13dd94a887213e13ecf30b6, RNA-Seq\Rockhopper\Java\Operations\Assembler.vb"

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

    '     Class Assembler
    ' 
    '         Properties: compressedFileName
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: encode, getFileNameBase, getPhredOffset, getReadsInputFileType, mapPhred
    '                   parametersToString, removePath, reverseComplement, separator
    ' 
    '         Sub: commandLineArguments, Main, outputResults, outputSamHeader, outputToFile
    '              processReads, readInSequencingReads
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
' * Copyright 2014 Brian Tjaden
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

    Public Class Assembler

        ''' <summary>
        '''******************************************
        ''' **********   Instance Variables   **********
        ''' </summary>

        Private kDict As Dictionary
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
        Private nameCount As AtomicInteger
        ' Used when outputting SAM file and reads have no name
        Private firstLineDone As Boolean
        ' Used when reading in the first line of FASTA files
        Private totalTime As Long
        ' Time taken to read in sequencing reads and assemble transcripts
        Private mapFullReadsToTranscriptome As Boolean
        ' First pass (false), second pass (true).
        Private transcripts As DeNovoTranscripts

        ' Results
        Public totalReads As AtomicInteger
        Private invalidQualityReads As AtomicInteger
        ' Reads containing an invalid quality score
        Private ambiguousReads As AtomicInteger
        ' Reads with ambiguous nucleotides
        Private lowQualityReads As AtomicInteger
        ' Reads with quality scores too low for mapping


        ''' <summary>
        '''***************************************
        ''' **********   Class Variables   **********
        ''' </summary>

        ' Parameters
        '   Public Shared output As JTextArea
        ' Output to GUI or, if null, to System.out
        Public Shared k As Integer = 25
        ' Size of k-mer
        Private Shared readsInputFormat As Integer
        ' 0=FASTQ, 1=QSEQ, 2=FASTA, 3=SAM(single), 4=SAM(paired), 5=BAM(single), 6=BAM(paired)
        Public Shared conditionFiles As New List(Of String)()
        Public Shared singleEndOrientationReverseComplement As Boolean = False
        ' RevComp reads
        Public Shared numThreads As Integer = 1
        ' Number of threads
        Public Shared minReadLength As Integer = 35
        ' Minimum allowed sequencing read length
        Public Shared stopAfterOneHit As Boolean = True
        Public Shared computeExpression As Boolean = True
        ' PARAM: compute differential expression
        Public Shared CAPACITY_POWER As Integer = 25
        ' PARAM: capacity of hashtable is ~2^n
        Public Shared MIN_READS_MAPPING As Integer = 20
        ' PARAM: Min # of full length reads mapping to do novo transcripts
        Public Shared minTranscriptLength As Integer
        ' PARAM: Min transcript length: 2*k
        Public Shared minSeedExpression As Integer = 50
        ' PARAM: Min expression to initiate a transcript
        Public Shared minExpression As Integer = 5
        ' PARAM: Min number of occurrences to be part of transcript
        Public Shared unstranded As Boolean = False
        ' PARAM: Is RNA-seq data strand specific or ambiguous?
        Public Shared pairedEndOrientation As String = "fr"
        ' PARAM: ff, fr, rf, rr
        Public Shared isPairedEnd As Boolean = False
        ' Do we have paired-end reads?
        Private Shared pairedEndFile As String
        ' Name of mate-pair file if using paired-end reads
        Public Shared maxPairedEndLength As Integer = 500
        Public Shared labels As String()
        Public Shared readsFileIndex As Integer
        ' Keep track of current sequencing reads file
        Public Shared summaryWriter As PrintWriter = Nothing
        ' For outputting summary file
        Private Shared summaryFile As String = "summary.txt"
        Public Shared expressionFile As String = "transcripts.txt"
        Public Shared outputSAM As Boolean = False
        ' Output reads to SAM file
        Public Shared output_DIR As String = "Assembler_Results/"
        ' PARAM: write output files to this directory
        Public Shared time As Boolean = False
        ' PARAM: output execution time of program
        Public Shared verbose As Boolean = False
        ' PARAM: verbose output


        ''' <summary>
        '''************************************
        ''' **********   Constructors   **********
        ''' </summary>

        Public Sub New()

            totalTime = Oracle.Java.System.CurrentTimeMillis()
            Dim foo As New FileOps(compressedFileName, conditionFiles)
            If foo.valid Then
                output(vbLf)
                For i As Integer = 0 To conditionFiles.Count - 1
                    Dim files As String() = StringSplit(conditionFiles(i), ",", True)
                    For j As Integer = 0 To files.Length - 1
                        Dim parse_readFiles As String() = StringSplit(files(j), "%", True)
                        If parse_readFiles.Length = 1 Then
                            ' Using single-end reads (except maybe SAM/BAM)
                            Dim file1 As String = parse_readFiles(0)
                            output("Assembling transcripts from reads in file:             " & vbTab & removePath(file1) & vbLf)
                            '  Rockhopper.updateProgress()
                            '  Rockhopper.updateProgress()
                            Assembler.isPairedEnd = False
                        Else
                            ' Using paired-end reads
                            Dim file1 As String = parse_readFiles(0)
                            Dim file2 As String = parse_readFiles(1)
                            output("Assembling transcripts from reads in files:" & vbLf)
                            output(vbTab & removePath(file1) & vbLf)
                            output(vbTab & removePath(file2) & vbLf)
                            '   Rockhopper.updateProgress()
                            '   Rockhopper.updateProgress()
                            Assembler.isPairedEnd = True
                        End If
                    Next
                Next
                transcripts = New DeNovoTranscripts(foo.transcripts)
                readsFileIndex = 0
                output(vbLf)
                For i As Integer = 0 To conditionFiles.Count - 1
                    Dim files As String() = StringSplit(conditionFiles(i), ",", True)
                    For j As Integer = 0 To files.Length - 1
                        Dim parse_readFiles As String() = StringSplit(files(j), "%", True)
                        If parse_readFiles.Length = 1 Then
                            ' Using single-end reads (except maybe SAM/BAM)
                            Dim file1 As String = parse_readFiles(0)
                            output("Aligning reads to assembled transcripts using file:" & vbTab & removePath(file1) & vbLf)
                            Assembler.isPairedEnd = False
                            output(vbLf & vbTab & "Total reads in file:        " & vbTab & DeNovoTranscripts.numReads(readsFileIndex) & vbLf)
                            output(vbTab & "Perfectly aligned reads:    " & vbTab & DeNovoTranscripts.numMappingReads(readsFileIndex) & vbTab & Math.Round((100.0 * DeNovoTranscripts.numMappingReads(readsFileIndex)) / DeNovoTranscripts.numReads(readsFileIndex)) & "%" & vbLf & vbLf)
                            '    Rockhopper.updateProgress()
                        Else
                            ' Using paired-end reads
                            Dim file1 As String = parse_readFiles(0)
                            Dim file2 As String = parse_readFiles(1)
                            output("Aligning reads to assembled transcripts using files:" & vbLf)
                            output(vbTab & removePath(file1) & vbLf)
                            output(vbTab & removePath(file2) & vbLf)
                            Assembler.isPairedEnd = True
                            output(vbLf & vbTab & "Total reads in files:        " & vbTab & DeNovoTranscripts.numReads(readsFileIndex) & vbLf)
                            output(vbTab & "Perfectly aligned reads:     " & vbTab & DeNovoTranscripts.numMappingReads(readsFileIndex) & vbTab & Math.Round((100.0 * DeNovoTranscripts.numMappingReads(readsFileIndex)) / DeNovoTranscripts.numReads(readsFileIndex)) & "%" & vbLf & vbLf)
                            '   Rockhopper.updateProgress()
                        End If
                        readsFileIndex += 1
                    Next
                Next
                If outputSAM Then
                    output("Rockhopper did not output a SAM file since sequencing read alignments were previously cached. If you want a SAM file to be output, you must first clear the cache." & vbLf & vbLf)
                End If
            Else

                ' Create k-mer dictionary and assemble transcripts de novo
                kDict = New Dictionary(k)
                totalReads = New AtomicInteger(0)
                invalidQualityReads = New AtomicInteger(0)
                ambiguousReads = New AtomicInteger(0)
                lowQualityReads = New AtomicInteger(0)
                mapFullReadsToTranscriptome = False
                readsFileIndex = 0
                output(vbLf)
                For i As Integer = 0 To conditionFiles.Count - 1
                    Dim files As String() = StringSplit(conditionFiles(i), ",", True)
                    For j As Integer = 0 To files.Length - 1
                        Dim parse_readFiles As String() = StringSplit(files(j), "%", True)
                        If parse_readFiles.Length = 1 Then
                            ' Using single-end reads (except maybe SAM/BAM)
                            Dim file1 As String = parse_readFiles(0)
                            output("Assembling transcripts from reads in file:             " & vbTab & removePath(file1) & vbLf)
                            Assembler.isPairedEnd = False
                            Assembler.pairedEndFile = Nothing
                            readsInputFormat = getReadsInputFileType(file1)
                            Me.PHRED_offset = getPhredOffset(file1)
                            If outputSAM Then
                                outputSamHeader(file1)
                            End If
                            Dim numThreads_OLD As Integer = numThreads
                            If (readsInputFormat = 5) OrElse (readsInputFormat = 6) Then
                                numThreads = 1
                            End If
                            firstLineDone = False
                            readInSequencingReads(file1)
                            ' Read in the sequencing reads
                            numThreads = numThreads_OLD
                            If samWriter IsNot Nothing Then
                                samWriter.close()
                                output("Sequencing reads written to SAM file:                  " & vbTab & output_DIR & getFileNameBase(file1) & ".sam" & vbLf)
                            End If
                            '   Rockhopper.updateProgress()
                            '  Rockhopper.updateProgress()
                        Else
                            ' Using paired-end reads
                            Dim file1 As String = parse_readFiles(0)
                            Dim file2 As String = parse_readFiles(1)
                            output("Assembling transcripts from reads in files:" & vbLf)
                            output(vbTab & removePath(file1) & vbLf)
                            output(vbTab & removePath(file2) & vbLf)
                            Assembler.isPairedEnd = True
                            Assembler.pairedEndFile = file2
                            readsInputFormat = getReadsInputFileType(file1)
                            Me.PHRED_offset = getPhredOffset(file1)
                            If outputSAM Then
                                outputSamHeader(file1)
                            End If
                            Dim numThreads_OLD As Integer = numThreads
                            If (readsInputFormat = 5) OrElse (readsInputFormat = 6) Then
                                numThreads = 1
                            End If
                            firstLineDone = False
                            readInSequencingReads(file1)
                            ' Read in the sequencing reads
                            numThreads = numThreads_OLD
                            If samWriter IsNot Nothing Then
                                samWriter.close()
                                output("Sequencing reads written to SAM file:                  " & vbTab & output_DIR & getFileNameBase(file1) & ".sam" & vbLf)
                            End If
                            '   Rockhopper.updateProgress()
                            '   Rockhopper.updateProgress()
                        End If
                        readsFileIndex += 1
                    Next
                Next
                kDict.assembleTranscripts()
                ' Assemble transcripts for final time
                ' Map full length reads to transcripts
                totalReads = New AtomicInteger(0)
                invalidQualityReads = New AtomicInteger(0)
                ambiguousReads = New AtomicInteger(0)
                lowQualityReads = New AtomicInteger(0)
                mapFullReadsToTranscriptome = True
                kDict.initializeReadMapping(stopAfterOneHit)
                readsFileIndex = 0
                output(vbLf)
                For i As Integer = 0 To conditionFiles.Count - 1
                    Dim files As String() = StringSplit(conditionFiles(i), ",", True)
                    For j As Integer = 0 To files.Length - 1
                        Dim parse_readFiles As String() = StringSplit(files(j), "%", True)
                        If parse_readFiles.Length = 1 Then
                            ' Using single-end reads (except maybe SAM/BAM)
                            Dim file1 As String = parse_readFiles(0)
                            output("Aligning reads to assembled transcripts using file:" & vbTab & removePath(file1) & vbLf)
                            Assembler.isPairedEnd = False
                            Assembler.pairedEndFile = Nothing
                            readsInputFormat = getReadsInputFileType(file1)
                            Me.PHRED_offset = getPhredOffset(file1)
                            Dim numThreads_OLD As Integer = numThreads
                            If (readsInputFormat = 5) OrElse (readsInputFormat = 6) Then
                                numThreads = 1
                            End If
                            firstLineDone = False
                            readInSequencingReads(file1)
                            ' Read in the sequencing reads
                            numThreads = numThreads_OLD
                            If unstranded Then
                                ' Fix read double counting
                                kDict.bwtIndex.halveReads(readsFileIndex)
                            End If
                            output(vbLf & vbTab & "Total reads in file:        " & vbTab & kDict.bwtIndex.getNumReads(readsFileIndex) & vbLf)
                            output(vbTab & "Perfectly aligned reads:    " & vbTab & kDict.bwtIndex.getNumMappingReads(readsFileIndex) & vbTab & Math.Round((100.0 * kDict.bwtIndex.getNumMappingReads(readsFileIndex)) / kDict.bwtIndex.getNumReads(readsFileIndex)) & "%" & vbLf & vbLf)
                            '   Rockhopper.updateProgress()
                        Else
                            ' Using paired-end reads
                            Dim file1 As String = parse_readFiles(0)
                            Dim file2 As String = parse_readFiles(1)
                            output("Aligning reads to assembled transcripts using files:" & vbLf)
                            output(vbTab & removePath(file1) & vbLf)
                            output(vbTab & removePath(file2) & vbLf)
                            Assembler.isPairedEnd = True
                            Assembler.pairedEndFile = file2
                            readsInputFormat = getReadsInputFileType(file1)
                            Me.PHRED_offset = getPhredOffset(file1)
                            Dim numThreads_OLD As Integer = numThreads
                            If (readsInputFormat = 5) OrElse (readsInputFormat = 6) Then
                                numThreads = 1
                            End If
                            firstLineDone = False
                            readInSequencingReads(file1)
                            ' Read in the sequencing reads
                            numThreads = numThreads_OLD
                            output(vbLf & vbTab & "Total reads in files:        " & vbTab & kDict.bwtIndex.getNumReads(readsFileIndex) & vbLf)
                            output(vbTab & "Perfectly aligned reads:     " & vbTab & kDict.bwtIndex.getNumMappingReads(readsFileIndex) & vbTab & Math.Round((100.0 * kDict.bwtIndex.getNumMappingReads(readsFileIndex)) / kDict.bwtIndex.getNumReads(readsFileIndex)) & "%" & vbLf & vbLf)
                            '  Rockhopper.updateProgress()
                        End If
                        readsFileIndex += 1
                    Next
                Next
                transcripts = New DeNovoTranscripts(kDict.bwtIndex)
                If computeExpression Then
                    transcripts.minDiffExpressionLevels = kDict.bwtIndex
                End If
                kDict.bwtIndex = Nothing
                ' Clear Index and Dictionary
                kDict = Nothing
                ' Clear Index and Dictionary
                Oracle.Java.System.GC()
                ' Clear Index and Dictionary
                If computeExpression Then
                    transcripts.computeDifferentialExpression()
                End If
                FileOps.writeCompressedFile(compressedFileName, conditionFiles, DeNovoTranscripts.numReads, DeNovoTranscripts.numMappingReads, transcripts)
            End If

            ' Incorporate results from SAM file
            If sam IsNot Nothing Then
                totalReads.addAndGet(sam.badReads.[Get]())
                invalidQualityReads.addAndGet(sam.badReads.[Get]())
            End If

            outputToFile(output_DIR & expressionFile, transcripts.ToString(labels))
            totalTime = Oracle.Java.System.CurrentTimeMillis() - totalTime
            outputResults()
            summaryWriter.close()
            '   Rockhopper.updateProgress()
        End Sub



        ''' <summary>
        '''***********************************************
        ''' **********   Public Instance Methods   **********
        ''' </summary>

        ''' <summary>
        ''' Read in sequencing reads from file.
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
                                Dim flag As Integer = 4
                                SyncLock samWriter
                                    samWriter.println(name & vbTab & flag & vbTab & "*" & vbTab & "0" & vbTab & "255" & vbTab & "*" & vbTab & "*" & vbTab & "0" & vbTab & "0" & vbTab & parse_line(9) & vbTab & mapPhred(parse_line(10)))
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
                                Dim flag As Integer = 77
                                ' 01001101
                                Dim flag2 As Integer = 141
                                ' 10001101
                                SyncLock samWriter
                                    samWriter.println(name & vbTab & flag & vbTab & "*" & vbTab & "0" & vbTab & "255" & vbTab & "*" & vbTab & "*" & vbTab & "0" & vbTab & "0" & vbTab & parse_line(9) & vbTab & mapPhred(parse_line(10)))
                                    samWriter.println(name2 & vbTab & flag2 & vbTab & "*" & vbTab & "0" & vbTab & "255" & vbTab & "*" & vbTab & "*" & vbTab & "0" & vbTab & "0" & vbTab & parse_line2(9) & vbTab & mapPhred(parse_line2(10)))
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
                            Dim flag As Integer = 4
                            SyncLock samWriter
                                samWriter.println(name & vbTab & flag & vbTab & "*" & vbTab & "0" & vbTab & "255" & vbTab & "*" & vbTab & "*" & vbTab & "0" & vbTab & "0" & vbTab & parse_line(9) & vbTab & parse_line(10))
                            End SyncLock
                            Continue While
                        Else
                            Continue While
                        End If
                        ' SAM file (paired-end)
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
                            Dim flag As Integer = 77
                            ' 01001101
                            Dim flag2 As Integer = 141
                            ' 10001101
                            SyncLock samWriter
                                samWriter.println(name & vbTab & flag & vbTab & "*" & vbTab & "0" & vbTab & "255" & vbTab & "*" & vbTab & "*" & vbTab & "0" & vbTab & "0" & vbTab & parse_line(9) & vbTab & parse_line(10))
                                samWriter.println(name2 & vbTab & flag2 & vbTab & "*" & vbTab & "0" & vbTab & "255" & vbTab & "*" & vbTab & "*" & vbTab & "0" & vbTab & "0" & vbTab & parse_line2(9) & vbTab & parse_line2(10))
                            End SyncLock
                            Continue While
                        Else
                            Continue While
                        End If
                    End If

                    ' Output to SAM file
                    If (samWriter IsNot Nothing) AndAlso (Not mapFullReadsToTranscriptome) Then
                        While name.StartsWith("@")
                            name = name.Substring(1)
                        End While
                        If isPairedEnd OrElse ((sam IsNot Nothing) AndAlso (sam.pairedEnd)) Then
                            ' Paired-end
                            While name2.StartsWith("@")
                                name2 = name2.Substring(1)
                            End While
                            If name.Length = 0 Then
                                name = "r" & Convert.ToString(nameCount.getAndIncrement)
                                name2 = name
                            End If
                            Dim flag As Integer = 77
                            ' 01001101
                            Dim flag2 As Integer = 141
                            ' 10001101
                            SyncLock samWriter
                                samWriter.println(name & vbTab & flag & vbTab & "*" & vbTab & "0" & vbTab & "255" & vbTab & "*" & vbTab & "*" & vbTab & "0" & vbTab & "0" & vbTab & read & vbTab & quality)
                                samWriter.println(name2 & vbTab & flag2 & vbTab & "*" & vbTab & "0" & vbTab & "255" & vbTab & "*" & vbTab & "*" & vbTab & "0" & vbTab & "0" & vbTab & read2 & vbTab & quality2)
                            End SyncLock
                        Else
                            ' Single-end
                            If name.Length = 0 Then
                                name = "r" & Convert.ToString(nameCount.getAndIncrement)
                            End If
                            Dim flag As Integer = 4
                            SyncLock samWriter
                                samWriter.println(name & vbTab & flag & vbTab & "*" & vbTab & "0" & vbTab & "255" & vbTab & "*" & vbTab & "*" & vbTab & "0" & vbTab & "0" & vbTab & read & vbTab & quality)
                            End SyncLock
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

                    ' Process sequencing reads.
                    totalReads.getAndIncrement()

                    If ambiguous Then
                        ambiguousReads.getAndIncrement()
                    ElseIf (quality.Length < minReadLength) OrElse (isPairedEnd AndAlso (quality2.Length < minReadLength)) OrElse ((sam IsNot Nothing) AndAlso (sam.pairedEnd) AndAlso (quality2.Length < minReadLength)) Then
                        ' Minimum read length
                        lowQualityReads.getAndIncrement()
                    ElseIf invalid Then
                        ' Invalid quality score
                        invalidQualityReads.getAndIncrement()
                    Else
                        If Not mapFullReadsToTranscriptome Then
                            ' First pass. Assemble transcriptome.
                            kDict.add(read)
                            ' Add read to dictionary!
                            If isPairedEnd OrElse ((sam IsNot Nothing) AndAlso (sam.pairedEnd)) Then
                                ' Using paired-end reads
                                kDict.add(read2)
                            End If
                        Else
                            ' Second pass. Map full length reads to assembled transcripts.
                            If isPairedEnd OrElse ((sam IsNot Nothing) AndAlso (sam.pairedEnd)) Then
                                ' Using paired-end reads
                                kDict.mapFullLengthRead(read, read2)
                            Else
                                ' Using single-end reads
                                kDict.mapFullLengthRead(read)
                                If unstranded Then
                                    kDict.mapFullLengthRead(reverseComplement(read))
                                End If
                            End If
                        End If
                    End If
                End While
            Catch e As IOException
                output("Error - could not read in from sequencing reads file." & vbLf & vbLf)
                Environment.[Exit](0)
            End Try
        End Sub

        ''' <summary>
        ''' Return the base file name for the compressed transcripts file.
        ''' For example, if A,B,C are replicates in conditions 1 and 
        ''' X,Y,Z are replicates in condition 2, then the base file name
        ''' would be ABC_XYZ__parameters
        ''' </summary>
        Public Overridable ReadOnly Property compressedFileName() As String
            Get
                Dim sb As New StringBuilder()
                For i As Integer = 0 To conditionFiles.Count - 1
                    Dim files As String() = StringSplit(conditionFiles(i), ",", True)
                    For j As Integer = 0 To files.Length - 1
                        sb.Append(encode(files(j)))
                    Next
                    sb.Append("_")
                Next
                sb.Append(parametersToString())
                Return sb.ToString()
            End Get
        End Property



        '''' <summary>
        ''''********************************************
        '''' **********   Public Class Methods   **********
        '''' </summary>

        'Public Shared Sub output(s As String)
        '	If summaryWriter IsNot Nothing Then
        '		summaryWriter.print(s)
        '	End If
        '	If output Is Nothing Then
        '		Console.Write(s)
        '	Else
        '		output.append(s)
        '		output.caretPosition = output.document.length
        '	End If
        'End Sub

        ''' <summary>
        ''' Returns a reverse complemented version of String s.
        ''' </summary>
        Public Shared Function reverseComplement(s As String) As String
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
        '''************************************************
        ''' **********   Private Instance Methods   **********
        ''' </summary>

        ''' <summary>
        ''' Read in sequencing reads from file.
        ''' </summary>
        Private Sub readInSequencingReads(readsFile As String)

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
                '    threads(i) = New ProcessReadsDeNovo_Thread(Me)
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
        ''' Outputs to STDOUT stats on sequencing reads.
        ''' </summary>
        Private Sub outputResults()
            output("Total number of assembled transcripts:" & vbTab & vbTab & transcripts.numTranscripts & vbLf)
            output(vbTab & "Average transcript length:             " & vbTab & transcripts.averageTranscriptLength & vbLf)
            output(vbTab & "Median transcript length:              " & vbTab & transcripts.medianTranscriptLength & vbLf)
            output(vbTab & "Total number of assembled bases:       " & vbTab & transcripts.totalAssembledBases & vbLf)
            output(vbLf)
            output("Summary of results written to file:               " & vbTab & output_DIR & summaryFile & vbLf)
            output("Details of assembled transcripts written to file: " & vbTab & output_DIR & expressionFile & vbLf)
            output(vbLf)
            output("FINSIHED." & vbLf & vbLf)
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
                sam = New SamOps(readsFile, MAX_LINES, True)
                If sam.validSam OrElse sam.validBam Then
                    Dim replicons As String() = sam.replicons
                    If sam.validSam AndAlso Not sam.pairedEnd Then
                        ' Single-end SAM file
                        Return 3
                    ElseIf sam.validSam Then
                        ' Paired-end SAM file
                        Return 4
                    ElseIf sam.validBam AndAlso Not sam.pairedEnd Then
                        ' Single-end BAM file
                        Return 5
                    ElseIf sam.validBam Then
                        ' Paired-end BAM file
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
                output(vbLf & "Error - could not open sequencing reads file: " & readsFile & vbLf & vbLf)
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
        ''' Open SAM file for writing.
        ''' Output header information to SAM file.
        ''' </summary>
        Private Sub outputSamHeader(readsFile As String)
            Dim samFileName As String = output_DIR & getFileNameBase(readsFile) & ".sam"
            If readsFile.Equals(samFileName) Then
                ' Do not read and write same file at same time
                Return
            End If
            nameCount = New AtomicInteger(1)
            Try
                samWriter = New PrintWriter(New Oracle.Java.IO.File(samFileName))
                samWriter.println("@HD" & vbTab & "VN:1.0" & vbTab & "SO:unsorted")
                samWriter.println("@PG" & vbTab & "ID:Rockhopper" & vbTab & "PN:Rockhopper" & vbTab & "VN:" & version)
            Catch f As IOException
                samWriter = Nothing
            End Try
        End Sub

        ''' <summary>
        ''' Return a String representation of the parameter settings.
        ''' </summary>
        Private Function parametersToString() As String
            Return "_k" & k & "_e" & ("" & computeExpression).ToUpper()(0) & "_a" & ("" & stopAfterOneHit).ToUpper()(0) & "_d" & maxPairedEndLength & "_j" & minReadLength & "_n" & CAPACITY_POWER & "_b" & MIN_READS_MAPPING & "_u" & minTranscriptLength & "_w" & minSeedExpression & "_x" & minExpression & "_" & pairedEndOrientation & "_s" & ("" & unstranded).ToUpper()(0) & "_c" & ("" & singleEndOrientationReverseComplement).ToUpper()(0)
        End Function



        ''' <summary>
        '''*********************************************
        ''' **********   Private Class Methods   **********
        ''' </summary>

        ''' <summary>
        ''' Returns the base of the specified file name, i.e.,
        ''' the file extension and path to the file are excluded.
        ''' </summary>
        Private Shared Function getFileNameBase(fileName As String) As String
            Dim slashIndex As Integer = fileName.LastIndexOf(separator())
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
        ''' Returns the system-dependent file separator that
        ''' can be used as a RegEx.
        ''' </summary>
        Private Shared Function separator() As String
            If System.IO.Path.DirectorySeparatorChar = "\"c Then
                Return "\\"
            Else
                Return Oracle.Java.IO.File.separator
            End If
        End Function

        ''' <summary>
        ''' Returns the file name with the leading path removed.
        ''' </summary>
        Private Shared Function removePath(fileName As String) As String
            Dim slashIndex As Integer = fileName.LastIndexOf(separator())
            If slashIndex = -1 Then
                slashIndex = fileName.LastIndexOf("/")
            End If
            Return fileName.Substring(slashIndex + 1)
        End Function

        ''' <summary>
        ''' Returns a 2 character encoding of the String s.
        ''' First, s is converted to an integer via hashing.
        ''' Then, the integer is converted into a 2 character
        ''' encoding, where each character takes on one of
        ''' 52 different values (a-z, A-Z).
        ''' </summary>
        Private Shared Function encode(s As String) As String
            Dim encoding As Integer = s.GetHashCode() Mod 2704
            ' 52*52=2704
            If encoding < 0 Then
                encoding += 2704
            End If
            Dim digit1 As Integer = encoding \ 52
            Dim digit2 As Integer = encoding Mod 52
            If digit2 < 0 Then
                digit2 += 52
            End If
            Dim char1 As Char = "?"c
            Dim char2 As Char = "?"c
            If digit1 < 26 Then
                char1 = ChrW("A"c.Asc + digit1)
            Else
                char1 = ChrW("a"c.Asc - 26 + digit1)
            End If
            If digit2 < 26 Then
                char2 = ChrW("A"c.Asc + digit2)
            Else
                char2 = ChrW("a"c.Asc - 26 + digit2)
            End If
            Return char1 & "" & char2
        End Function

        ''' <summary>
        ''' Output the specified string s to the specified file.
        ''' </summary>
        Private Shared Sub outputToFile(fileName As String, s As String)
            Try
                Dim writer As New PrintWriter(New Oracle.Java.IO.File(fileName))
                writer.print(s)
                writer.close()
            Catch e As FileNotFoundException
                output(vbLf & "Error - could not open file " & fileName & vbLf & vbLf)
            End Try
        End Sub

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
            If args.Length < 1 Then
                output(vbLf & "The Assembler application takes one or more files of RNA-seq reads and it assembles the reads (de novo) into a transcriptome map." & vbLf)
                output(vbLf & "The Assembler application has one required command line argument." & vbLf)
                output(vbLf & "REQUIRED ARGUMENT" & vbLf & vbLf)
                output(vbTab & "a FASTQ file of RNA-seq reads" & vbLf)

                output(vbLf & "OPTIONAL ARGUMENTS" & vbLf & vbLf)
                output(vbTab & "-k <integer>" & vbTab & "size of k-mer, range of values is 15 to 31 (default is 25)" & vbLf)
                output(vbTab & "-c <boolean>" & vbTab & "reverse complement single-end reads (default is false)" & vbLf)
                output(vbTab & "-ff/fr/rf/rr" & vbTab & "orientation of two mate reads for paired-end read, f=forward and r=reverse_complement (default is fr)" & vbLf)
                output(vbTab & "-s <boolean>" & vbTab & "RNA-seq experiments are strand specific (true) or strand ambiguous (false) (default is true)" & vbLf)
                output(vbTab & "-p <integer>" & vbTab & "number of processors (default is self-identify)" & vbLf)
                output(vbTab & "-e <boolean>" & vbTab & "compute differential expression for transcripts in pairs of experimental conditions (default is true)" & vbLf)
                output(vbTab & "-a <boolean>" & vbTab & "report 1 alignment (true) or report all optimal alignments (false), (default is true)" & vbLf)
                output(vbTab & "-d <integer>" & vbTab & "maximum number of bases between mate pairs for paired-end reads (default is 500)" & vbLf)
                output(vbTab & "-j <integer>" & vbTab & "minimum length required to use a sequencing read after trimming/processing (default is 35)" & vbLf)
                output(vbTab & "-n <integer>" & vbTab & "size of k-mer hashtable is ~ 2^n (default is 25). HINT: should normally be 25 or, if more memory is available, 26. WARNING: if increased above 25 then more than 1.2M of memory must be allocated" & vbLf)
                output(vbTab & "-b <integer>" & vbTab & "minimum number of full length reads required to map to a de novo assembled trancript (default is 20)" & vbLf)
                output(vbTab & "-u <integer>" & vbTab & "minimum length of de novo assembled transcripts (default is 2*k)" & vbLf)
                output(vbTab & "-w <integer>" & vbTab & "minimum count of k-mer to use it to seed a new de novo assembled transcript (default is 50)" & vbLf)
                output(vbTab & "-x <integer>" & vbTab & "minimum count of k-mer to use it to extend an existing de novo assembled transcript (default is 5)" & vbLf)
                output(vbTab & "-L <comma separated list>" & vbTab & "labels for each condition" & vbLf)
                output(vbTab & "-v <boolean>" & vbTab & "verbose output including raw/normalized counts aligning to each transcript (default is false)" & vbLf)
                output(vbTab & "-TIME       " & vbTab & "output time taken to execute program" & vbLf)

                output(vbLf & "EXAMPLE EXECUTION: SINGLE-END READS" & vbLf)
                output(vbLf & "java Assembler <options> aerobic_replicate1.fastq,aerobic_replicate2.fastq anaerobic_replicate1.fastq,anaerobic_replicate2.fastq" & vbLf)
                output(vbLf & "EXAMPLE EXECUTION: PAIRED-END READS" & vbLf)
                output(vbLf & "java Assembler <options> aerobic_replicate1_pairedend1.fastq%aerobic_replicate1_pairedend2.fastq,aerobic_replicate2_pairedend1.fastq%aerobic_replicate2_pairedend2.fastq anaerobic_replicate1_pairedend1.fastq%anaerobic_replicate1_pairedend2.fastq,anaerobic_replicate2_pairedend1.fastq%anaerobic_replicate2_pairedend2.fastq" & vbLf)
                output(vbLf)
                Environment.[Exit](0)
            End If

            ' Initially set the number of threads
            Assembler.numThreads = Oracle.Java.System.Runtime.Runtime.availableProcessors()
            If Assembler.numThreads > 4 Then
                Assembler.numThreads = CInt(Math.Truncate(Math.Min(Assembler.numThreads * 0.75, 8.0)))
            End If

            Dim i As Integer = 0
            While i < args.Length
                If args(i).StartsWith("-k") Then
                    If (i = args.Length - 2) OrElse (args(i + 1).StartsWith("-")) Then
                        output("Error - command line argument -k must be followed by an integer." & vbLf)
                        Environment.[Exit](0)
                    End If
                    Assembler.k = Convert.ToInt32(args(i + 1))
                    If Assembler.minTranscriptLength = 0 Then
                        Assembler.minTranscriptLength = 2 * Assembler.k
                    End If
                    i += 2
                ElseIf args(i).StartsWith("-c") Then
                    If (i = args.Length - 2) OrElse (args(i + 1).StartsWith("-")) Then
                        output("Error - command line argument -c must be followed by a boolean." & vbLf)
                        Environment.[Exit](0)
                    End If
                    Assembler.singleEndOrientationReverseComplement = Convert.ToBoolean(args(i + 1))
                    i += 2
                ElseIf args(i).StartsWith("-s") Then
                    If (i = args.Length - 2) OrElse (args(i + 1).StartsWith("-")) Then
                        output("Error - command line argument -s must be followed by a boolean." & vbLf)
                        Environment.[Exit](0)
                    End If
                    Assembler.unstranded = Not Convert.ToBoolean(args(i + 1))
                    i += 2
                ElseIf args(i).StartsWith("-p") Then
                    If (i = args.Length - 2) OrElse (args(i + 1).StartsWith("-")) Then
                        output("Error - command line argument -p must be followed by an integer." & vbLf)
                        Environment.[Exit](0)
                    End If
                    Assembler.numThreads = Convert.ToInt32(args(i + 1))
                    i += 2
                ElseIf args(i).StartsWith("-e") Then
                    If (i = args.Length - 2) OrElse (args(i + 1).StartsWith("-")) Then
                        output("Error - command line argument -e must be followed by a boolean." & vbLf)
                        Environment.[Exit](0)
                    End If
                    Assembler.computeExpression = Convert.ToBoolean(args(i + 1))
                    i += 2
                ElseIf args(i).StartsWith("-a") Then
                    If (i = args.Length - 2) OrElse (args(i + 1).StartsWith("-")) Then
                        output("Error - command line argument -a must be followed by a boolean." & vbLf)
                        Environment.[Exit](0)
                    End If
                    Assembler.stopAfterOneHit = Convert.ToBoolean(args(i + 1))
                    i += 2
                ElseIf args(i).StartsWith("-d") Then
                    If (i = args.Length - 2) OrElse (args(i + 1).StartsWith("-")) Then
                        output("Error - command line argument -d must be followed by an integer." & vbLf)
                        Environment.[Exit](0)
                    End If
                    Assembler.maxPairedEndLength = Convert.ToInt32(args(i + 1))
                    SamOps.maxPairedEndLength = Convert.ToInt32(args(i + 1))
                    i += 2
                ElseIf args(i).StartsWith("-j") Then
                    If (i = args.Length - 2) OrElse (args(i + 1).StartsWith("-")) Then
                        output("Error - command line argument -j must be followed by an integer." & vbLf)
                        Environment.[Exit](0)
                    End If
                    Assembler.minReadLength = Convert.ToInt32(args(i + 1))
                    i += 2
                ElseIf args(i).StartsWith("-n") Then
                    If (i = args.Length - 2) OrElse (args(i + 1).StartsWith("-")) Then
                        output("Error - command line argument -n must be followed by an integer." & vbLf)
                        Environment.[Exit](0)
                    End If
                    Assembler.CAPACITY_POWER = Convert.ToInt32(args(i + 1))
                    i += 2
                ElseIf args(i).StartsWith("-b") Then
                    If (i = args.Length - 2) OrElse (args(i + 1).StartsWith("-")) Then
                        output("Error - command line argument -b must be followed by an integer." & vbLf)
                        Environment.[Exit](0)
                    End If
                    Assembler.MIN_READS_MAPPING = Convert.ToInt32(args(i + 1))
                    i += 2
                ElseIf args(i).StartsWith("-u") Then
                    If (i = args.Length - 2) OrElse (args(i + 1).StartsWith("-")) Then
                        output("Error - command line argument -u must be followed by an integer." & vbLf)
                        Environment.[Exit](0)
                    End If
                    Assembler.minTranscriptLength = Convert.ToInt32(args(i + 1))
                    i += 2
                ElseIf args(i).StartsWith("-w") Then
                    If (i = args.Length - 2) OrElse (args(i + 1).StartsWith("-")) Then
                        output("Error - command line argument -w must be followed by an integer." & vbLf)
                        Environment.[Exit](0)
                    End If
                    Assembler.minSeedExpression = Convert.ToInt32(args(i + 1))
                    i += 2
                ElseIf args(i).StartsWith("-x") Then
                    If (i = args.Length - 2) OrElse (args(i + 1).StartsWith("-")) Then
                        output("Error - command line argument -x must be followed by an integer." & vbLf)
                        Environment.[Exit](0)
                    End If
                    Assembler.minExpression = Convert.ToInt32(args(i + 1))
                    i += 2
                ElseIf args(i).StartsWith("-L") Then
                    If (i = args.Length - 2) OrElse (args(i + 1).StartsWith("-")) Then
                        output("Error - command line argument -L must be followed by a comma separated list of names for the conditions." & vbLf)
                        Environment.[Exit](0)
                    End If
                    Assembler.labels = StringSplit(args(i + 1), ",", True)
                    i += 2
                ElseIf args(i).StartsWith("-v") Then
                    If (i = args.Length - 2) OrElse (args(i + 1).StartsWith("-")) Then
                        output("Error - command line argument -v must be followed by a boolean." & vbLf)
                        Environment.[Exit](0)
                    End If
                    Assembler.verbose = Convert.ToBoolean(args(i + 1))
                    i += 2
                ElseIf args(i).Equals("-ff") Then
                    Assembler.pairedEndOrientation = "ff"
                    i += 1
                ElseIf args(i).Equals("-fr") Then
                    Assembler.pairedEndOrientation = "fr"
                    i += 1
                ElseIf args(i).Equals("-rf") Then
                    Assembler.pairedEndOrientation = "rf"
                    i += 1
                ElseIf args(i).Equals("-rr") Then
                    Assembler.pairedEndOrientation = "rr"
                    i += 1
                ElseIf args(i).Equals("-TIME") Then
                    time = True
                    i += 1
                Else
                    ' Sequencing reads files
                    Assembler.conditionFiles.Add(args(i))
                    i += 1
                End If
            End While

            ' Handle erroneous command line arguments
            If conditionFiles.Count = 0 Then
                output("Error - sequencing reads files (in FASTQ, QSEQ, or FASTA format) are required as command line arguments." & vbLf)
                Environment.[Exit](0)
            End If
            For j As Integer = 0 To conditionFiles.Count - 1
                Dim files As String() = StringSplit(conditionFiles(j), ",", True)
                For k As Integer = 0 To files.Length - 1
                    Dim pairedEnd_files As String() = StringSplit(files(k), "%", True)
                    If Not (System.IO.Directory.Exists(pairedEnd_files(0)) OrElse System.IO.File.Exists(pairedEnd_files(0))) Then
                        output("Error - file " & pairedEnd_files(0) & " does not exist." & vbLf)
                        Environment.[Exit](0)
                    End If
                    If (pairedEnd_files.Length > 1) AndAlso (Not (System.IO.Directory.Exists(pairedEnd_files(1)) OrElse System.IO.File.Exists(pairedEnd_files(1)))) Then
                        output("Error - file " & pairedEnd_files(1) & " does not exist." & vbLf)
                        Environment.[Exit](0)
                    End If
                    If pairedEnd_files.Length > 1 Then
                        ' Assume paired-end reads are strand specific
                        Assembler.unstranded = False
                    End If
                Next
            Next

            ' If not set by command line arguments, set de novo assembled transcript length
            If Assembler.minTranscriptLength = 0 Then
                Assembler.minTranscriptLength = 2 * Assembler.k
            End If

            ' Output directory does not exist. Create it.
            If Not (System.IO.Directory.Exists(output_DIR) OrElse System.IO.File.Exists(output_DIR)) Then
                If Not (System.IO.Directory.CreateDirectory(output_DIR).Exists) Then
                    Output("Error - could not create directory " & output_DIR & "." & vbLf)
                    Environment.[Exit](0)
                End If
            End If

            Try
                summaryWriter = New PrintWriter(New Oracle.Java.IO.File(output_DIR & summaryFile))
            Catch e As FileNotFoundException
                output("Error - could not create file " & (output_DIR & summaryFile) & vbLf)
                Environment.[Exit](0)
            End Try
        End Sub



        ''' <summary>
        '''***********************************
        ''' **********   Main Method   **********
        ''' </summary>

        Private Shared Sub Main(args As String())

            commandLineArguments(args)
            Dim a As New Assembler()

        End Sub

    End Class



    '''' <summary>
    ''''*************************************************
    '''' **********   ProcessReadsDeNovo_Thread   **********
    '''' </summary>

    'Friend Class ProcessReadsDeNovo_Thread
    '    Inherits System.Threading.Thread

    '    Private a As Assembler

    '    Public Sub New(a As Assembler)
    '        Me.a = a
    '    End Sub

    '    Public Overridable Sub run()
    '        a.processReads()
    '    End Sub
    'End Class
End Namespace
