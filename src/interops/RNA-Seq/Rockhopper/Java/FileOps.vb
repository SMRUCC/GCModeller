#Region "Microsoft.VisualBasic::f7643f705a5031ea3b4f9c9515a6fcfb, RNA-Seq\Rockhopper\Java\FileOps.vb"

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

    '     Class FileOps
    ' 
    '         Properties: ambiguousReads, avgLengthReads, coordinates_minus, coordinates_plus, exactMappedReads
    '                     inexactMappedReads, invalidQualityReads, lowQualityReads, mappedMoreThanOnceReads, mappedOnceReads
    '                     readFileName, totalReads, transcripts, valid
    ' 
    '         Constructor: (+3 Overloads) Sub New
    ' 
    '         Function: byteToDoubleList, byteToIntArray, byteToLong2Darray, byteToLongList, doubleArrayToByteArray
    '                   getInt, (+2 Overloads) getLine, getStopIndex, InlineAssignHelper, intArrayToByteArray
    '                   isGZIP, longArrayToByteArray, readCompressedFile, readCompressedFile_DeNovo
    ' 
    '         Sub: Main, (+2 Overloads) writeCompressedFile
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic
Imports System.Text
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

    ''' <summary>
    ''' Supports reading and writing of compressed files.
    ''' </summary>
    Public Class FileOps

        ''' <summary>
        '''******************************************
        ''' **********   Instance Variables   **********
        ''' </summary>

        Private _readFileName As String
        Private readFileLength As Long
        Private readFileDate As Long
        Private repliconFileName As String
        Private repliconFileLength As Long
        Private repliconFileDate As Long
        Private repliconHeader As String
        Private repliconLength As Integer
        Private _avgLengthReads As Long
        Private _coordinates_plus As Integer()
        Private _coordinates_minus As Integer()
        Private _transcripts As List(Of DeNovoTranscript)
        ' For de novo assembly
        Private isValid As Boolean

        ' Results
        Private _totalReads As Integer
        Private _invalidQualityReads As Integer
        ' Reads containing an invalid quality score
        Private _ambiguousReads As Integer
        ' Reads with ambiguous nucleotides
        Private _lowQualityReads As Integer
        ' Reads with quality scores too low for mapping
        Private _mappedOnceReads As Integer
        ' Reads mapping to one location
        Private _mappedMoreThanOnceReads As Integer
        ' Reads mapping to multiple locations
        Private _exactMappedReads As Integer
        ' Reads mapping without mismatches/errors
        Private _inexactMappedReads As Integer
        ' Reads mapping with mismatches/errors


        ''' <summary>
        '''***************************************
        ''' **********   Class Variables   **********
        ''' </summary>

        Private Shared DIR As String = output_DIR + "intermediary/"
        Private Shared buffer1 As SByte() = New SByte(0) {}
        ' Temp array used when reading in GZIP file


        ''' <summary>
        '''************************************
        ''' **********   Constructors   **********
        ''' </summary>

        Public Sub New(inFileName As String)
            isValid = readCompressedFile(inFileName)
        End Sub

        Public Sub New(inFileName As String, readFileName As String, repliconFileName As String)
            isValid = readCompressedFile(inFileName)
            Dim readFile As New Oracle.Java.IO.File(readFileName)
            Dim repliconFile As New Oracle.Java.IO.File(repliconFileName)
            isValid = isValid AndAlso (readFile.name.Equals(Me._readFileName)) AndAlso (readFile.length = readFileLength) AndAlso (readFile.lastModified() = readFileDate) AndAlso (repliconFile.name.Equals(Me.repliconFileName)) AndAlso (repliconFile.length = repliconFileLength) AndAlso (repliconFile.lastModified() = repliconFileDate)
        End Sub

        ''' <summary>
        ''' Constructor used for de novo transcript assembly.
        ''' </summary>
        Public Sub New(inFileName As String, conditionFiles As List(Of String))
            isValid = readCompressedFile_DeNovo(inFileName, conditionFiles)
        End Sub



        ''' <summary>
        '''***********************************************
        ''' **********   Public Instance Methods   **********
        ''' </summary>

        ''' <summary>
        ''' Returns true if the input file is valid, i.e.,
        ''' the stats of the read-file and replicon-file file 
        ''' correspond to the stats from the input file.
        ''' Returns false otherwise.
        ''' </summary>
        Public Overridable ReadOnly Property valid() As Boolean
            Get
                Return Me.isValid
            End Get
        End Property

        ''' <summary>
        ''' Returns the name of the sequencing read file.
        ''' </summary>
        Public Overridable ReadOnly Property readFileName() As String
            Get
                Return Me._readFileName
            End Get
        End Property

        ''' <summary>
        ''' Returns the average length of sequencing reads
        ''' in the sequencing reads file.
        ''' </summary>
        Public Overridable ReadOnly Property avgLengthReads() As Long
            Get
                Return _avgLengthReads
            End Get
        End Property

        ''' <summary>
        ''' Returns plus strand coordinates.
        ''' </summary>
        Public Overridable ReadOnly Property coordinates_plus() As Integer()
            Get
                Return Me._coordinates_plus
            End Get
        End Property

        ''' <summary>
        ''' Returns minus strand coordinates.
        ''' </summary>
        Public Overridable ReadOnly Property coordinates_minus() As Integer()
            Get
                Return Me._coordinates_minus
            End Get
        End Property

        ''' <summary>
        ''' Returns the total reads
        ''' </summary>
        Public Overridable ReadOnly Property totalReads() As Integer
            Get
                Return Me._totalReads
            End Get
        End Property

        ''' <summary>
        ''' Returns the invalide quality reads
        ''' </summary>
        Public Overridable ReadOnly Property invalidQualityReads() As Integer
            Get
                Return Me._invalidQualityReads
            End Get
        End Property

        ''' <summary>
        ''' Returns the ambiguous reads
        ''' </summary>
        Public Overridable ReadOnly Property ambiguousReads() As Integer
            Get
                Return Me._ambiguousReads
            End Get
        End Property

        ''' <summary>
        ''' Returns the low quality reads
        ''' </summary>
        Public Overridable ReadOnly Property lowQualityReads() As Integer
            Get
                Return Me._lowQualityReads
            End Get
        End Property

        ''' <summary>
        ''' Returns the mapped once reads
        ''' </summary>
        Public Overridable ReadOnly Property mappedOnceReads() As Integer
            Get
                Return Me._mappedOnceReads
            End Get
        End Property

        ''' <summary>
        ''' Returns the mapped more than once reads
        ''' </summary>
        Public Overridable ReadOnly Property mappedMoreThanOnceReads() As Integer
            Get
                Return Me._mappedMoreThanOnceReads
            End Get
        End Property

        ''' <summary>
        ''' Returns the exact mapped reads
        ''' </summary>
        Public Overridable ReadOnly Property exactMappedReads() As Integer
            Get
                Return Me._exactMappedReads
            End Get
        End Property

        ''' <summary>
        ''' Returns the inexact mapped reads
        ''' </summary>
        Public Overridable ReadOnly Property inexactMappedReads() As Integer
            Get
                Return Me._inexactMappedReads
            End Get
        End Property

        ''' <summary>
        ''' Returns the de novo assembled transcripts
        ''' </summary>
        Public Overridable ReadOnly Property transcripts() As List(Of DeNovoTranscript)
            Get
                Return Me._transcripts
            End Get
        End Property



        ''' <summary>
        '''********************************************
        ''' **********   Public Class Methods   **********
        ''' </summary>

        ''' <summary>
        ''' Converts the specified pair of int arrays to byte arrays
        ''' and writes these byte arrays to a compressed file. The following
        ''' header information is also written to the compressed output file:
        '''      version
        '''      read file name
        '''      read file length
        '''      read file last modified date
        '''      replicon file name
        '''      replicon file length
        '''      replicon file last modified date
        '''      replicon FASTA header line
        '''      replicon length
        '''      average length of reads
        '''      total reads
        '''      invalid quality reads
        '''      ambiguous reads
        '''      low quality reads
        '''      mapped once reads
        '''      mapped more than once reads
        '''      exact mapped reads
        '''      inexact mapped reads
        ''' </summary>
        Public Shared Sub writeCompressedFile(outFileName As String, readFileName As String, repliconFileName As String, repliconHeader As String, repliconLength As Integer, totalReads As Integer,
        invalidQualityReads As Integer, ambiguousReads As Integer, lowQualityReads As Integer, mappedOnceReads As Integer, mappedMoreThanOnceReads As Integer, exactMappedReads As Integer,
        inexactMappedReads As Integer, avgLengthReads As Long, a1 As Integer(), a2 As Integer())
            Dim readFile As New Oracle.Java.IO.File(readFileName)
            Dim repliconFile As New Oracle.Java.IO.File(repliconFileName)
            Dim header As SByte() = (((((version & vbLf) + readFile.name & vbLf) + readFile.length & vbLf & readFile.lastModified() & vbLf) + repliconFile.name & vbLf) + repliconFile.length & vbLf & repliconFile.lastModified() & vbLf & repliconHeader & vbLf & repliconLength & vbLf & avgLengthReads & vbLf & totalReads & vbLf & invalidQualityReads & vbLf & ambiguousReads & vbLf & lowQualityReads & vbLf & mappedOnceReads & vbLf & mappedMoreThanOnceReads & vbLf & exactMappedReads & vbLf & inexactMappedReads & vbLf).Bytes
            Dim b1 As SByte() = intArrayToByteArray(a1)
            Dim b2 As SByte() = intArrayToByteArray(a2)
            Try
                ' Set up directory
                Dim d As New Oracle.Java.IO.File(DIR)
                If Not d.Directory Then
                    d.mkdir()
                End If
                Dim gzos As New GZIPOutputStream(New FileOutputStream(DIR & outFileName & ".gz"))
                gzos.write(header, 0, header.Length)
                gzos.write(b1, 0, b1.Length)
                gzos.write(b2, 0, b2.Length)
                gzos.close()
            Catch e As IOException
                output("Could not write to file " & outFileName & ".gz" & vbLf)
            End Try
        End Sub

        ''' <summary>
        ''' Writes a list of de novo assembled transcripts to a compressed file.
        ''' The following header information is also written to the compressed file:
        '''      version
        '''      number of conditions
        '''      number of replicates in each condition
        '''      for each read file: its name, its length, and its last modified time
        '''      number of files
        '''      for each file: total reads, total mapping reads
        '''      number of transcripts in list
        '''      number of p-values per transcript
        '''      for each transcript:
        '''            sequence
        '''            for each condition: rawCounts_nts, rawCounts_reads, normalizedCounts
        '''            RPKMs, means, variances, lowess, pValues, qValues
        ''' </summary>
        Public Shared Sub writeCompressedFile(outFileName As String, conditionFiles As List(Of String), numReads As Integer(), numMappingReads As Integer(), transcripts As DeNovoTranscripts)

            ' Version, number of conditions, and number of replicates in each condition
            Dim header As New StringBuilder()
            header.Append(version & vbLf)
            header.Append(conditionFiles.Count & vbLf)
            For i As Integer = 0 To conditionFiles.Count - 1
                header.Append(Convert.ToString(StringSplit(conditionFiles(i), ",", True).Length) & vbLf)
            Next

            ' Name, length, and last modified time of each file (including mate-pair files)
            For i As Integer = 0 To conditionFiles.Count - 1
                Dim files As String() = StringSplit(conditionFiles(i), ",", True)
                For j As Integer = 0 To files.Length - 1
                    Dim parse_readFiles As String() = StringSplit(files(j), "%", True)
                    If parse_readFiles.Length = 1 Then
                        ' Using single-end reads
                        Dim file1 As String = parse_readFiles(0)
                        Dim readFile As New Oracle.Java.IO.File(file1)
                        header.Append(readFile.name + vbLf + readFile.length & vbLf & readFile.lastModified() & vbLf)
                    Else
                        ' Using paired-end reads
                        Dim file1 As String = parse_readFiles(0)
                        Dim readFile1 As New Oracle.Java.IO.File(file1)
                        header.Append(readFile1.name + vbLf + readFile1.length & vbLf & readFile1.lastModified() & vbLf)
                        Dim file2 As String = parse_readFiles(1)
                        Dim readFile2 As New Oracle.Java.IO.File(file2)
                        header.Append(readFile2.name + vbLf + readFile2.length & vbLf & readFile2.lastModified() & vbLf)
                    End If
                Next
            Next

            ' Number of files. Total reads per file and total number of mapping reads per file
            header.Append(numReads.Length & vbLf)
            For i As Integer = 0 To numReads.Length - 1
                header.Append(numReads(i) & vbLf & numMappingReads(i) & vbLf)
            Next

            ' Number of transcripts and number of p-values per transcript
            header.Append(transcripts.numTranscripts & vbLf)
            If transcripts.numTranscripts = 0 Then
                header.Append("0" & vbLf)
            Else
                header.Append(Convert.ToString(transcripts.getTranscript(0).numPvalues) & vbLf)
            End If

            ' Write header information and information for each transcript
            Try
                ' Set up directory
                Dim d As New Oracle.Java.IO.File(DIR)
                If Not d.Directory Then
                    d.mkdir()
                End If
                Dim gzos As New GZIPOutputStream(New FileOutputStream(DIR & outFileName & ".gz"))
                Dim header_bytes As SByte() = header.ToString().Bytes
                gzos.write(header_bytes, 0, header_bytes.Length)
                For i As Integer = 0 To transcripts.numTranscripts - 1
                    Dim transcript As DeNovoTranscript = transcripts.getTranscript(i)
                    Dim b As SByte() = (transcript.SequenceData() & vbLf).Bytes
                    ' Add new line to sequence
                    gzos.write(b, 0, b.Length)
                    For j As Integer = 0 To conditionFiles.Count - 1
                        b = longArrayToByteArray(transcript.getRawCounts_nts_array(j))
                        gzos.write(b, 0, b.Length)
                        b = longArrayToByteArray(transcript.getRawCounts_reads_array(j))
                        gzos.write(b, 0, b.Length)
                        b = longArrayToByteArray(transcript.getNormalizedCounts_array(j))
                        gzos.write(b, 0, b.Length)
                    Next
                    b = longArrayToByteArray(transcript.rPKMs)
                    gzos.write(b, 0, b.Length)
                    b = longArrayToByteArray(transcript.means)
                    gzos.write(b, 0, b.Length)
                    b = longArrayToByteArray(transcript.variances)
                    gzos.write(b, 0, b.Length)
                    b = longArrayToByteArray(transcript.lowess)
                    gzos.write(b, 0, b.Length)
                    b = doubleArrayToByteArray(transcript.pvalues)
                    gzos.write(b, 0, b.Length)
                    b = doubleArrayToByteArray(transcript.qvalues)
                    gzos.write(b, 0, b.Length)
                Next
                gzos.close()
            Catch e As IOException
                output("Could not write to file " & outFileName & ".gz" & vbLf)
            End Try
        End Sub

        ''' <summary>
        ''' Returns true is file is successfully read. False otherwise.
        ''' </summary>
        Public Overridable Function readCompressedFile(inFileName As String) As Boolean
            Try
                Dim [in] As New GZIPInputStream(New FileInputStream(DIR & inFileName & ".gz"))

                ' Read in header information
                Dim version As String = getLine([in])
                If Not version.Equals(version) Then
                    Return False
                End If
                Me._readFileName = getLine([in])
                Me.readFileLength = Convert.ToInt64(getLine([in]))
                Me.readFileDate = Convert.ToInt64(getLine([in]))
                Me.repliconFileName = getLine([in])
                Me.repliconFileLength = Convert.ToInt64(getLine([in]))
                Me.repliconFileDate = Convert.ToInt64(getLine([in]))
                Me.repliconHeader = getLine([in])
                Me.repliconLength = Convert.ToInt32(getLine([in]))
                Me._avgLengthReads = Convert.ToInt64(getLine([in]))
                Me._totalReads = Convert.ToInt32(getLine([in]))
                Me._invalidQualityReads = Convert.ToInt32(getLine([in]))
                Me._ambiguousReads = Convert.ToInt32(getLine([in]))
                Me._lowQualityReads = Convert.ToInt32(getLine([in]))
                Me._mappedOnceReads = Convert.ToInt32(getLine([in]))
                Me._mappedMoreThanOnceReads = Convert.ToInt32(getLine([in]))
                Me._exactMappedReads = Convert.ToInt32(getLine([in]))
                Me._inexactMappedReads = Convert.ToInt32(getLine([in]))

                ' Retrieve data
                Me._coordinates_plus = New Integer(repliconLength - 1) {}
                For i As Integer = 0 To repliconLength - 1
                    _coordinates_plus(i) = getInt([in])
                Next
                Me._coordinates_minus = New Integer(repliconLength - 1) {}
                For i As Integer = 0 To repliconLength - 1
                    _coordinates_minus(i) = getInt([in])
                Next
                [in].close()
                Return True
            Catch e As IOException
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Returns true is file is successfully read and is valid. False otherwise.
        ''' </summary>
        Public Overridable Function readCompressedFile_DeNovo(inFileName As String, conditionFiles As List(Of String)) As Boolean
            Try

                ' Read in file
                Dim [in] As New GZIPInputStream(New FileInputStream(DIR & inFileName & ".gz"))
                Dim bytes As New List(Of SByte)(1000000)
                Dim buffer As SByte() = New SByte(1023) {}
                Dim len As Integer
                While (InlineAssignHelper(len, [in].read(buffer))) >= 0
                    For i As Integer = 0 To len - 1
                        bytes.Add(buffer(i))
                    Next
                End While
                [in].close()

                ' First header line: version
                Dim startIndex As Integer = 0
                Dim stopIndex As Integer = getStopIndex(bytes, startIndex)
                Dim version As String = getLine(bytes, startIndex, stopIndex)
                If Not version.Equals(version) Then
                    Return False
                End If

                ' Second header line: number of conditions
                startIndex = stopIndex + 1
                stopIndex = getStopIndex(bytes, startIndex)
                Dim numConditions As Integer = Convert.ToInt32(getLine(bytes, startIndex, stopIndex))
                startIndex = stopIndex + 1

                ' One line for each condition indicating number of replicates in condition
                Dim numReplicates As Integer() = New Integer(numConditions - 1) {}
                For i As Integer = 0 To numConditions - 1
                    stopIndex = getStopIndex(bytes, startIndex)
                    numReplicates(i) = Convert.ToInt32(getLine(bytes, startIndex, stopIndex))
                    startIndex = stopIndex + 1
                Next

                ' For each read file, we have its name, length, and last modified time
                Dim names As String()()() = CubeArray(Of String)(2, numConditions)
                '= new string[2][numConditions][]; // First coordinate is mate-pair
                Dim lengths As Long()()() = CubeArray(Of Long)(2, numConditions)
                '= new long[2][numConditions][]; // First coordinate is mate-pair
                Dim times As Long()()() = CubeArray(Of Long)(2, numConditions)
                '= new long[2][numConditions][]; // First coordinate is mate-pair
                For i As Integer = 0 To numConditions - 1
                    names(0)(i) = New String(numReplicates(i) - 1) {}
                    lengths(0)(i) = New Long(numReplicates(i) - 1) {}
                    times(0)(i) = New Long(numReplicates(i) - 1) {}
                    names(1)(i) = New String(numReplicates(i) - 1) {}
                    lengths(1)(i) = New Long(numReplicates(i) - 1) {}
                    times(1)(i) = New Long(numReplicates(i) - 1) {}
                    Dim files As String() = StringSplit(conditionFiles(i), ",", True)
                    For j As Integer = 0 To numReplicates(i) - 1
                        stopIndex = getStopIndex(bytes, startIndex)
                        names(0)(i)(j) = getLine(bytes, startIndex, stopIndex)
                        startIndex = stopIndex + 1
                        stopIndex = getStopIndex(bytes, startIndex)
                        lengths(0)(i)(j) = Convert.ToInt64(getLine(bytes, startIndex, stopIndex))
                        startIndex = stopIndex + 1
                        stopIndex = getStopIndex(bytes, startIndex)
                        times(0)(i)(j) = Convert.ToInt64(getLine(bytes, startIndex, stopIndex))
                        startIndex = stopIndex + 1

                        ' Handle mate-pair files, if any
                        Dim parse_readFiles As String() = StringSplit(files(j), "%", True)
                        If parse_readFiles.Length > 1 Then
                            ' Using paired-end reads
                            stopIndex = getStopIndex(bytes, startIndex)
                            names(1)(i)(j) = getLine(bytes, startIndex, stopIndex)
                            startIndex = stopIndex + 1
                            stopIndex = getStopIndex(bytes, startIndex)
                            lengths(1)(i)(j) = Convert.ToInt64(getLine(bytes, startIndex, stopIndex))
                            startIndex = stopIndex + 1
                            stopIndex = getStopIndex(bytes, startIndex)
                            times(1)(i)(j) = Convert.ToInt64(getLine(bytes, startIndex, stopIndex))
                            startIndex = stopIndex + 1
                        End If
                    Next
                Next

                ' Number of files. For each file, number of reads and number of mapping reads
                stopIndex = getStopIndex(bytes, startIndex)
                Dim numFiles As Integer = Convert.ToInt32(getLine(bytes, startIndex, stopIndex))
                startIndex = stopIndex + 1
                Dim numReads As Integer() = New Integer(numFiles - 1) {}
                Dim numMappingReads As Integer() = New Integer(numFiles - 1) {}
                For i As Integer = 0 To numFiles - 1
                    stopIndex = getStopIndex(bytes, startIndex)
                    numReads(i) = Convert.ToInt32(getLine(bytes, startIndex, stopIndex))
                    startIndex = stopIndex + 1
                    stopIndex = getStopIndex(bytes, startIndex)
                    numMappingReads(i) = Convert.ToInt32(getLine(bytes, startIndex, stopIndex))
                    startIndex = stopIndex + 1
                Next

                ' Final two header lines
                stopIndex = getStopIndex(bytes, startIndex)
                Dim numTranscripts As Integer = Convert.ToInt32(getLine(bytes, startIndex, stopIndex))
                startIndex = stopIndex + 1
                stopIndex = getStopIndex(bytes, startIndex)
                Dim numPvalues As Integer = Convert.ToInt32(getLine(bytes, startIndex, stopIndex))
                startIndex = stopIndex + 1

                ' Transcripts
                Dim transcripts As New List(Of DeNovoTranscript)(numTranscripts)
                For i As Integer = 0 To numTranscripts - 1
                    ' For each transcript
                    stopIndex = getStopIndex(bytes, startIndex)
                    Dim tempData As SByte()
                    Dim sequence As String = getLine(bytes, startIndex, stopIndex)
                    startIndex = stopIndex + 1
                    Dim rawCounts_nts As New List(Of List(Of Long))(numConditions)
                    Dim rawCounts_reads As New List(Of List(Of Long))(numConditions)
                    Dim normalizedCounts As New List(Of List(Of Long))(numConditions)
                    For j As Integer = 0 To numConditions - 1
                        tempData = New SByte(numReplicates(j) * 8 - 1) {}
                        For k As Integer = startIndex To startIndex + (numReplicates(j) * 8 - 1)
                            tempData(k - startIndex) = CSByte(bytes(k))
                        Next
                        rawCounts_nts.Add(byteToLongList(tempData))
                        startIndex += numReplicates(j) * 8
                        For k As Integer = startIndex To startIndex + (numReplicates(j) * 8 - 1)
                            tempData(k - startIndex) = CSByte(bytes(k))
                        Next
                        rawCounts_reads.Add(byteToLongList(tempData))
                        startIndex += numReplicates(j) * 8
                        For k As Integer = startIndex To startIndex + (numReplicates(j) * 8 - 1)
                            tempData(k - startIndex) = CSByte(bytes(k))
                        Next
                        normalizedCounts.Add(byteToLongList(tempData))
                        startIndex += numReplicates(j) * 8
                    Next
                    tempData = New SByte(numConditions * 8 - 1) {}
                    For k As Integer = startIndex To startIndex + (numConditions * 8 - 1)
                        tempData(k - startIndex) = CSByte(bytes(k))
                    Next
                    Dim RPKMs As List(Of Long) = byteToLongList(tempData)
                    startIndex += numConditions * 8
                    For k As Integer = startIndex To startIndex + (numConditions * 8 - 1)
                        tempData(k - startIndex) = CSByte(bytes(k))
                    Next
                    Dim means As List(Of Long) = byteToLongList(tempData)
                    startIndex += numConditions * 8
                    tempData = New SByte(numConditions * numConditions * 8 - 1) {}
                    For k As Integer = startIndex To startIndex + (numConditions * numConditions * 8 - 1)
                        tempData(k - startIndex) = CSByte(bytes(k))
                    Next
                    Dim variances As Long()() = byteToLong2Darray(tempData)
                    startIndex += numConditions * numConditions * 8
                    For k As Integer = startIndex To startIndex + (numConditions * numConditions * 8 - 1)
                        tempData(k - startIndex) = CSByte(bytes(k))
                    Next
                    Dim lowess As Long()() = byteToLong2Darray(tempData)
                    startIndex += numConditions * numConditions * 8
                    tempData = New SByte(numPvalues * 8 - 1) {}
                    For k As Integer = startIndex To startIndex + (numPvalues * 8 - 1)
                        tempData(k - startIndex) = CSByte(bytes(k))
                    Next
                    Dim pValues As List(Of Double) = byteToDoubleList(tempData)
                    startIndex += numPvalues * 8
                    For k As Integer = startIndex To startIndex + (numPvalues * 8 - 1)
                        tempData(k - startIndex) = CSByte(bytes(k))
                    Next
                    Dim qValues As List(Of Double) = byteToDoubleList(tempData)
                    startIndex += numPvalues * 8
                    transcripts.Add(New DeNovoTranscript(sequence, rawCounts_nts, rawCounts_reads, normalizedCounts, RPKMs, means,
                    variances, lowess, pValues, qValues))
                Next

                ' Check validity
                If numConditions <> conditionFiles.Count Then
                    Return False
                End If
                For i As Integer = 0 To numConditions - 1
                    Dim files As String() = StringSplit(conditionFiles(i), ",", True)
                    If numReplicates(i) <> files.Length Then
                        Return False
                    End If
                    For j As Integer = 0 To numReplicates(i) - 1
                        Dim parse_readFiles As String() = StringSplit(files(j), "%", True)
                        If parse_readFiles.Length = 1 Then
                            ' Using single-end reads
                            Dim file1 As String = parse_readFiles(0)
                            Dim f As New Oracle.Java.IO.File(file1)
                            If Not names(0)(i)(j).Equals(f.name) Then
                                Return False
                            End If
                            If lengths(0)(i)(j) <> f.length Then
                                Return False
                            End If
                            If times(0)(i)(j) <> f.lastModified() Then
                                Return False
                            End If
                        Else
                            ' Using paired-end reads
                            Dim file1 As String = parse_readFiles(0)
                            Dim file2 As String = parse_readFiles(1)
                            Dim f1 As New Oracle.Java.IO.File(file1)
                            If Not names(0)(i)(j).Equals(f1.name) Then
                                Return False
                            End If
                            If lengths(0)(i)(j) <> f1.length Then
                                Return False
                            End If
                            If times(0)(i)(j) <> f1.lastModified() Then
                                Return False
                            End If
                            Dim f2 As New Oracle.Java.IO.File(file2)
                            If Not names(1)(i)(j).Equals(f2.name) Then
                                Return False
                            End If
                            If lengths(1)(i)(j) <> f2.length Then
                                Return False
                            End If
                            If times(1)(i)(j) <> f2.lastModified() Then
                                Return False
                            End If
                        End If
                    Next
                Next
                Me._transcripts = transcripts
                DeNovoTranscripts.numReads = numReads
                DeNovoTranscripts.numMappingReads = numMappingReads
                Return True
            Catch e As Exception
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Returns true if the specified file is in gzip format.
        ''' Returns false otherwise.
        ''' </summary>
        Public Shared Function isGZIP(fileName As String) As Boolean
            Try
                Dim [in] As New GZIPInputStream(New FileInputStream(fileName))
                [in].close()
                Return True
            Catch e As Exception
                Return False
            End Try
        End Function



        ''' <summary>
        '''************************************************
        ''' **********   Private Instance Methods   **********
        ''' </summary>

        ''' <summary>
        ''' Returns the first index after the specified startIndex in the Byte list 
        ''' corresponding to a new line (end of line).
        ''' </summary>
        Private Function getStopIndex(bytes As List(Of SByte), startIndex As Integer) As Integer
            Dim stopIndex As Integer = startIndex
            While (stopIndex < bytes.Count) AndAlso (CSByte(bytes(stopIndex)) <> 10)
                stopIndex += 1
            End While
            Return stopIndex
        End Function

        ''' <summary>
        ''' Returns a String corresponding to one line, i.e., the bytes from
        ''' startIndex to stopIndex.
        ''' </summary>
        Private Function getLine(bytes As List(Of SByte), startIndex As Integer, stopIndex As Integer) As String
            ' Convert sequence of bytes to a String representing a line
            Dim bytesInLine As SByte() = New SByte(stopIndex - startIndex - 1) {}
            For i As Integer = startIndex To stopIndex - 1
                bytesInLine(i - startIndex) = CSByte(bytes(i))
            Next
            Return (New String(bytesInLine.CharValue))
        End Function



        ''' <summary>
        '''*********************************************
        ''' **********   Private Class Methods   **********
        ''' </summary>

        ''' <summary>
        ''' Returns one line from the specified input stream.
        ''' </summary>
        Private Shared Function getLine([in] As GZIPInputStream) As String
            Try
                Dim len As Integer = -1
                Dim buffer As New List(Of SByte)()
                While ((InlineAssignHelper(len, [in].read(buffer1))) >= 0) AndAlso (buffer1(0) <> 10)
                    buffer.Add(buffer1(0))
                End While
                Dim buffer_bytes As SByte() = New SByte(buffer.Count - 1) {}
                For i As Integer = 0 To buffer.Count - 1
                    buffer_bytes(i) = CSByte(buffer(i))
                Next
                Return (New String(buffer_bytes.CharValue))
            Catch e As IOException
                Return ""
            End Try
        End Function

        ''' <summary>
        ''' Returns one integer from the specified input stream.
        ''' </summary>
        Private Shared Function getInt([in] As GZIPInputStream) As Integer
            Try
                Dim result As Integer = 0
                [in].read(buffer1)
                result += (buffer1(0) And &HFF) << 24
                [in].read(buffer1)
                result += (buffer1(0) And &HFF) << 16
                [in].read(buffer1)
                result += (buffer1(0) And &HFF) << 8
                [in].read(buffer1)
                result += (buffer1(0) And &HFF)
                Return result
            Catch e As IOException
                Return -1
            End Try
        End Function

        ''' <summary>
        ''' Returns a byte array representation of the specified int array.
        ''' </summary>
        Private Shared Function intArrayToByteArray(a As Integer()) As SByte()
            Dim b As SByte() = New SByte(a.Length * 4 - 1) {}
            For i As Integer = 0 To a.Length - 1
                b(4 * i + 0) = CSByte(CInt(CUInt(a(i)) >> 24))
                b(4 * i + 1) = CSByte(CInt(CUInt(a(i)) >> 16))
                b(4 * i + 2) = CSByte(CInt(CUInt(a(i)) >> 8))
                b(4 * i + 3) = CSByte(CInt(CUInt(a(i)) >> 0))
            Next
            Return b
        End Function

        ''' <summary>
        ''' Returns a byte array representation of the specified long array.
        ''' </summary>
        Private Shared Function longArrayToByteArray(a As Long()) As SByte()
            Dim b As SByte() = New SByte(a.Length * 8 - 1) {}
            For i As Integer = 0 To a.Length - 1
                b(8 * i + 0) = CSByte(CLng(CULng(a(i)) >> 56))
                b(8 * i + 1) = CSByte(CLng(CULng(a(i)) >> 48))
                b(8 * i + 2) = CSByte(CLng(CULng(a(i)) >> 40))
                b(8 * i + 3) = CSByte(CLng(CULng(a(i)) >> 32))
                b(8 * i + 4) = CSByte(CLng(CULng(a(i)) >> 24))
                b(8 * i + 5) = CSByte(CLng(CULng(a(i)) >> 16))
                b(8 * i + 6) = CSByte(CLng(CULng(a(i)) >> 8))
                b(8 * i + 7) = CSByte(CLng(CULng(a(i)) >> 0))
            Next
            Return b
        End Function

        ''' <summary>
        ''' Returns a byte array representation of the specified double array.
        ''' </summary>
        Private Shared Function doubleArrayToByteArray(a As Double()) As SByte()
            Dim b As SByte() = New SByte(a.Length * 8 - 1) {}
            For i As Integer = 0 To a.Length - 1
                b(8 * i + 0) = CSByte(CInt(CUInt(Oracle.Java.Lang.Double.doubleToRawLongBits(a(i))) >> 56))
                b(8 * i + 1) = CSByte(CInt(CUInt(Oracle.Java.Lang.Double.doubleToRawLongBits(a(i))) >> 48))
                b(8 * i + 2) = CSByte(CInt(CUInt(Oracle.Java.Lang.Double.doubleToRawLongBits(a(i))) >> 40))
                b(8 * i + 3) = CSByte(CInt(CUInt(Oracle.Java.Lang.Double.doubleToRawLongBits(a(i))) >> 32))
                b(8 * i + 4) = CSByte(CInt(CUInt(Oracle.Java.Lang.Double.doubleToRawLongBits(a(i))) >> 24))
                b(8 * i + 5) = CSByte(CInt(CUInt(Oracle.Java.Lang.Double.doubleToRawLongBits(a(i))) >> 16))
                b(8 * i + 6) = CSByte(CInt(CUInt(Oracle.Java.Lang.Double.doubleToRawLongBits(a(i))) >> 8))
                b(8 * i + 7) = CSByte(CInt(CUInt(Oracle.Java.Lang.Double.doubleToRawLongBits(a(i))) >> 0))
            Next
            Return b
        End Function

        ''' <summary>
        ''' Returns an int array representation of the specified byte array.
        ''' </summary>
        Private Shared Function byteToIntArray(b As SByte()) As Integer()
            Dim a As Integer() = New Integer(b.Length \ 4 - 1) {}
            For i As Integer = 0 To b.Length - 1 Step 4
                a(i \ 4) = (b(i + 0) << 24) + ((b(i + 1) And &HFF) << 16) + ((b(i + 2) And &HFF) << 8) + (b(i + 3) And &HFF)
            Next
            Return a
        End Function

        ''' <summary>
        ''' Returns a long list representation of the specified byte array.
        ''' </summary>
        Private Shared Function byteToLongList(b As SByte()) As List(Of Long)
            Dim a As New List(Of Long)(b.Length \ 8)
            For i As Integer = 0 To b.Length - 1 Step 8
                a.Add((CLng(b(i + 0) And &HFF) << 56) Or (CLng(b(i + 1) And &HFF) << 48) Or (CLng(b(i + 2) And &HFF) << 40) Or (CLng(b(i + 3) And &HFF) << 32) Or (CLng(b(i + 4) And &HFF) << 24) Or (CLng(b(i + 5) And &HFF) << 16) Or (CLng(b(i + 6) And &HFF) << 8) Or (CLng(b(i + 7) And &HFF) << 0))
            Next
            Return a
        End Function

        ''' <summary>
        ''' Returns a double list representation of the specified byte array.
        ''' </summary>
        Private Shared Function byteToDoubleList(b As SByte()) As List(Of Double)
            Dim a As New List(Of Double)(b.Length \ 8)
            For i As Integer = 0 To b.Length - 1 Step 8
                a.Add(Oracle.Java.Lang.Double.longBitsToDouble((CLng(b(i + 0) And &HFF) << 56) Or (CLng(b(i + 1) And &HFF) << 48) Or (CLng(b(i + 2) And &HFF) << 40) Or (CLng(b(i + 3) And &HFF) << 32) Or (CLng(b(i + 4) And &HFF) << 24) Or (CLng(b(i + 5) And &HFF) << 16) Or (CLng(b(i + 6) And &HFF) << 8) Or (CLng(b(i + 7) And &HFF) << 0)))
            Next
            Return a
        End Function

        ''' <summary>
        ''' Returns a 2D long array representation of the specified byte array.
        ''' </summary>
        Private Shared Function byteToLong2Darray(b As SByte()) As Long()()
            Dim a As List(Of Long) = byteToLongList(b)

            ' Convert 1D long list into 2D long array
            Dim size As Integer = CInt(Math.Truncate(Math.Sqrt(a.Count)))
            'ORIGINAL LINE: long[][] c = new long[size][size];
            'JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
            Dim c As Long()() = ReturnRectangularLongArray(size, size)
            Dim index As Integer = 0
            For x As Integer = 0 To size - 1
                For y As Integer = 0 To size - 1
                    c(x)(y) = a(index)
                    index += 1
                Next
            Next
            Return c
        End Function



        ''' <summary>
        '''***********************************
        ''' **********   Main Method   **********
        ''' </summary>

        Private Shared Sub Main(args As String())

        End Sub
        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
            target = value
            Return value
        End Function

    End Class

End Namespace
