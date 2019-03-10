#Region "Microsoft.VisualBasic::2cc7a8b869635aadad6590a10953d530, RNA-Seq\Rockhopper\Java\SamOps.vb"

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

    '     Class SamOps
    ' 
    '         Properties: maxQuality, pairedEnd, replicons, validBam, validSam
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: byteArrayToInt, checkAlignmentLine_BAM, checkAlignmentLine_SAM, getRepliconLength, isHeaderLine
    '                   isTextFile, parseAlignmentLine_BAM, parseAlignmentLine_SAM, parseHeaderLine_SAM, prepBAM
    '                   prepSAM, readCigarString, readInSAM, readInt, readSequence
    '                   readString, reverseComplement, ToString
    ' 
    '         Sub: disregardHeader_BAM, Main, mapReadToCoords, merge, mergesort
    '              output, parseCigar, readInBAM
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic
Imports System.IO
Imports System.Text
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


    ''' <summary>
    ''' An instance of the SamOps class supports operations
    ''' on SAM/BAM files. It is assumed that a SAM/BAM file contains
    ''' information about a single RNA-seq experiment.
    ''' Java 7 is required for properly handling BAM files.
    ''' </summary>
    Public Class SamOps

        ''' <summary>
        '''******************************************
        ''' **********   INSTANCE VARIABLES   **********
        ''' </summary>

        Private fileName As String
        Private isValidSam As Boolean
        Private isValidBam As Boolean
        Private isPairedEnd As Boolean
        Private isDenovo As Boolean
        Private _replicons As Dictionary(Of String, Integer)
        Private repliconLengths As List(Of Integer)
        Public alignedReads As AtomicIntegerArray
        Public unalignedReads As AtomicInteger
        Public badReads As AtomicInteger
        Public avgLengthReads As AtomicLong
        Public coordinates_plus As AtomicIntegerArray()
        Public coordinates_minus As AtomicIntegerArray()
        Private _maxQuality As Integer
        Private pairedEndCount As Long
        ' Temp counter used when reading in first lines
        Private qualities As String
        ' Temp String used when reading in unaligned reads
        Private cigarMap As Integer()
        ' Indexed by ASCII char values {M,I,D,N,S,H,P,=,X}
        Private cigarChars As Char() = {"M"c, "I"c, "D"c, "N"c, "S"c, "H"c,
        "P"c, "="c, "X"c}
        Private Shared intBuffer As SByte() = New SByte(3) {}
        ' Temp array used for reading bam files
        Private Shared buffer1 As SByte() = New SByte(0) {}
        ' Temp array used for reading bam files
        Private Shared NT_code As Char() = {"="c, "A"c, "C"c, "M"c, "G"c, "R"c,
        "S"c, "V"c, "T"c, "W"c, "Y"c, "H"c,
        "K"c, "D"c, "B"c, "N"c}
        Public Shared ReadOnly emptyStringArray As String() = New String(-1) {}
        Private [in] As GZIPInputStream
        ' Used for reading BAM files
        Public Shared maxPairedEndLength As Integer = 500



        ''' <summary>
        '''************************************
        ''' **********   CONSTRUCTORS   **********
        ''' </summary>

        ''' <summary>
        ''' Constructor for reading in a subset of lines in
        ''' SAM/BAM file to check if file is valid.
        ''' </summary>
        Public Sub New(fileName As String, numLines As Long, isDenovo As Boolean)
            Me.fileName = fileName
            Me._replicons = New Dictionary(Of String, Integer)()
            Me.repliconLengths = New List(Of Integer)()
            Me.alignedReads = New AtomicIntegerArray(1)
            Me.unalignedReads = New AtomicInteger(0)
            Me.badReads = New AtomicInteger(0)
            Me.avgLengthReads = New AtomicLong(0)
            Me.isPairedEnd = False
            Me.isDenovo = isDenovo
            Me.pairedEndCount = 0
            Me.qualities = ""
            If isTextFile(fileName) Then
                Me.isValidSam = prepSAM(numLines)
            Else
                Me.isValidBam = prepBAM(numLines)
            End If
            If isValidSam OrElse isValidBam Then
                If (_replicons.Count > 0) AndAlso (Not isDenovo) Then
                    alignedReads = New AtomicIntegerArray(_replicons.Count)
                    coordinates_plus = New AtomicIntegerArray(_replicons.Count - 1) {}
                    coordinates_minus = New AtomicIntegerArray(_replicons.Count - 1) {}
                    For i As Integer = 0 To repliconLengths.Count - 1
                        coordinates_plus(i) = New AtomicIntegerArray(repliconLengths(i) + 1)
                        coordinates_minus(i) = New AtomicIntegerArray(repliconLengths(i) + 1)
                    Next
                Else
                    coordinates_plus = New AtomicIntegerArray(0) {}
                    coordinates_minus = New AtomicIntegerArray(0) {}
                    coordinates_plus(0) = New AtomicIntegerArray(0)
                    coordinates_minus(0) = New AtomicIntegerArray(0)
                End If
            End If
        End Sub



        ''' <summary>
        '''***********************************************
        ''' **********   PUBLIC INSTANCE METHODS   **********
        ''' </summary>

        ''' <summary>
        ''' Returns true if the first lines of the input SAM
        ''' file are valid (correspond to what we are expecting
        ''' from a SAM file) and false otherwise.
        ''' </summary>
        Public Overridable ReadOnly Property validSam() As Boolean
            Get
                Return Me.isValidSam
            End Get
        End Property

        ''' <summary>
        ''' Returns true if the first lines of the input BAM
        ''' file are valid (correspond to what we are expecting
        ''' from a BAM file) and false otherwise.
        ''' </summary>
        Public Overridable ReadOnly Property validBam() As Boolean
            Get
                Return Me.isValidBam
            End Get
        End Property

        ''' <summary>
        ''' Returns true if file contains paired end reads.
        ''' False otherwise.
        ''' Only defined if file is valid.
        ''' </summary>
        Public Overridable ReadOnly Property pairedEnd() As Boolean
            Get
                Return isPairedEnd
            End Get
        End Property

        ''' <summary>
        ''' Returns the maximum quality score found in the first lines of the file.
        ''' </summary>
        Public Overridable ReadOnly Property maxQuality() As Integer
            Get
                Return _maxQuality
            End Get
        End Property

        ''' <summary>
        ''' Returns the length of the replicon at the specified index.
        ''' </summary>
        Public Overridable Function getRepliconLength(i As Integer) As Integer
            Return repliconLengths(i)
        End Function

        ''' <summary>
        ''' Returns an array of replicon names. The array is ordered
        ''' so that the indices in the array are the same as those
        ''' for the repliconLengths list.
        ''' </summary>
        Public Overridable ReadOnly Property replicons() As String()
            Get
                Dim names As String() = New String(_replicons.Count - 1) {}
                Dim indices As Integer() = New Integer(_replicons.Count - 1) {}
                Dim i As Integer = 0
                For Each key As String In _replicons.Keys
                    names(i) = key
                    indices(i) = _replicons(key)
                    i += 1
                Next
                mergesort(indices, names, 0, _replicons.Count - 1)
                Return names
            End Get
        End Property

        ''' <summary>
        ''' Return a String containing stats/information about this SamOps object.
        ''' </summary>
        Public Overridable Overloads Function ToString() As String
            If Not validSam AndAlso Not validBam Then
                Return "Not a valid SAM/BAM file." & vbLf
            End If
            Dim sb As New StringBuilder()
            sb.AppendLine()
            sb.Append("Sam/Bam file contains reads mapping to " & _replicons.Count & " replicons." & vbLf)
            If _replicons.Count = 0 Then
                ' No replicons in SAM/BAM header
                Dim final_NT As Integer = 0
                Dim repliconIndex As Integer = 0
                Dim count_plus As Integer = 0
                For i As Integer = 0 To coordinates_plus(repliconIndex).length - 1
                    If coordinates_plus(repliconIndex).[Get](i) > 0 Then
                        count_plus += 1
                        If i > final_NT Then
                            final_NT = i
                        End If
                    End If
                Next
                Dim count_minus As Integer = 0
                For i As Integer = 0 To coordinates_minus(repliconIndex).length - 1
                    If coordinates_minus(repliconIndex).[Get](i) > 0 Then
                        count_minus += 1
                        If i > final_NT Then
                            final_NT = i
                        End If
                    End If
                Next
                sb.Append(vbTab & vbTab & "Percent of plus strand covered: " & vbTab & CInt(Math.Truncate(100.0 * count_plus / final_NT)) & "%" & vbLf)
                sb.Append(vbTab & vbTab & "Percent of minus strand covered:" & vbTab & CInt(Math.Truncate(100.0 * count_minus / final_NT)) & "%" & vbLf)
            Else
                ' We have replicons in SAM/BAM header
                For Each name As String In _replicons.Keys
                    Dim repliconIndex As Integer = _replicons(name)
                    sb.Append(vbTab & "Replicon " & name & " with length " & Convert.ToString(repliconLengths(repliconIndex)) & vbLf)
                    Dim count As Integer = 0
                    For i As Integer = 0 To coordinates_plus(repliconIndex).length - 1
                        If coordinates_plus(repliconIndex).[Get](i) > 0 Then
                            count += 1
                        End If
                    Next
                    sb.Append(vbTab & vbTab & "Percent of plus strand covered: " & vbTab & CInt(100.0 * count / coordinates_plus(repliconIndex).length) & "%" & vbLf)
                    count = 0
                    For i As Integer = 0 To coordinates_minus(repliconIndex).length - 1
                        If coordinates_minus(repliconIndex).[Get](i) > 0 Then
                            count += 1
                        End If
                    Next
                    sb.Append(vbTab & vbTab & "Percent of minus strand covered:" & vbTab & CInt(100.0 * count / coordinates_minus(repliconIndex).length) & "%" & vbLf)
                Next
            End If
            sb.AppendLine()
            sb.Append("Bad reads:      " & vbTab & vbTab & badReads.[Get]() & vbLf)
            sb.Append("Unaligned reads:" & vbTab & vbTab & unalignedReads.[Get]() & vbLf)
            Dim orderedReplicons As String() = replicons
            For i As Integer = 0 To orderedReplicons.Length - 1
                sb.Append("Aligned reads in " & orderedReplicons(i) & ":  " & vbTab & alignedReads.[Get](i) & vbLf)
            Next

            Return sb.ToString()
        End Function

        ''' <summary>
        ''' Parse one line of alignment information from the SAM file.
        ''' Returns empty String array if line should be skipped.
        ''' Returns parsed String array with "U" prepended at index 0 if
        ''' read is unaligned and alignment should be attempted.
        ''' Returns parsed String array with "A" prepended at index 0 if
        ''' read has already been aligned.
        ''' </summary>
        Public Overridable Function parseAlignmentLine_SAM(line As String) As String()
            Try
                If isHeaderLine(line) Then
                    ' We have a header line
                    Return emptyStringArray
                End If
                Dim parse_line As String() = StringSplit(line, vbTab, True)
                'String qname = parse_line[0];  // QNAME
                Dim flag As Integer = Convert.ToInt32(parse_line(1))
                ' FLAG
                Dim rname As String = parse_line(2)
                ' RNAME
                Dim pos As Integer = Convert.ToInt32(parse_line(3))
                ' POS
                'int mapq = Integer.parseInt(parse_line[4]);  // MAPQ
                'String rnext = parse_line[6];  // RNEXT
                'int pnext = Integer.parseInt(parse_line[7]);  // PNEXT
                Dim tlen As Integer = Convert.ToInt32(parse_line(8))
                ' TLEN
                Dim seq As String = parse_line(9)
                ' SEQ
                Dim qual As String = parse_line(10)
                ' QUAL
                If flag >= 256 Then
                    ' Bad or chimeric read
                    badReads.incrementAndGet()
                    Return emptyStringArray
                    ' Unaligned read
                ElseIf ((CInt(CUInt(flag) >> 2) And 1) = 1) OrElse (rname.Equals("*")) OrElse (pos = 0) OrElse (((flag And 1) = 1) AndAlso (tlen = 0)) OrElse isDenovo Then
                    unalignedReads.incrementAndGet()
                    If seq.Equals("*") Then
                        Return emptyStringArray
                    End If
                    If isDenovo AndAlso ((CInt(CUInt(flag) >> 2) And 1) = 0) AndAlso ((CInt(CUInt(flag) >> 4) And 1) = 0) Then
                        parse_line(9) = reverseComplement(seq)
                    End If
                    parse_line(0) = "U" & parse_line(0)
                    If qual.Length = seq.Length Then
                        Return parse_line
                    ElseIf qualities.Length <> seq.Length Then
                        qualities = (New String(New Char(seq.Length - 1) {})).Replace(ControlChars.NullChar, "("c)
                    End If
                    parse_line(10) = qualities
                    Return parse_line
                    ' Single-end aligned read
                ElseIf (flag And 1) = 0 Then
                    Dim length As Integer = seq.Length
                    Dim isRevComp As Boolean = ((CInt(CUInt(flag) >> 4) And 1) = 1)
                    Dim repliconIndex As Integer = 0
                    If _replicons.ContainsKey(rname) Then
                        repliconIndex = _replicons(rname)
                    End If
                    mapReadToCoords(repliconIndex, pos, length, isRevComp)
                    alignedReads.incrementAndGet(repliconIndex)
                    avgLengthReads.addAndGet(length)
                    parse_line(0) = "A" & parse_line(0)
                    Return parse_line
                    ' First of paired-end aligned read
                ElseIf ((flag And 1) = 1) AndAlso (tlen <> 0) AndAlso ((CInt(CUInt(flag) >> 6) And 1) = 1) AndAlso ((CInt(CUInt(flag) >> 7) And 1) = 0) AndAlso (Math.Abs(tlen) <= maxPairedEndLength) Then
                    Dim isRevComp As Boolean = ((CInt(CUInt(flag) >> 4) And 1) = 1)
                    Dim repliconIndex As Integer = 0
                    If _replicons.ContainsKey(rname) Then
                        repliconIndex = _replicons(rname)
                    End If
                    mapReadToCoords(repliconIndex, pos, tlen, isRevComp)
                    alignedReads.incrementAndGet(repliconIndex)
                    avgLengthReads.addAndGet(Math.Abs(tlen))
                    parse_line(0) = "A" & parse_line(0)
                    Return parse_line
                    ' Second of paired-end aligned read
                ElseIf ((flag And 1) = 1) AndAlso (tlen <> 0) AndAlso ((CInt(CUInt(flag) >> 6) And 1) = 0) AndAlso ((CInt(CUInt(flag) >> 7) And 1) = 1) AndAlso (Math.Abs(tlen) <= maxPairedEndLength) Then
                    parse_line(0) = "A" & parse_line(0)
                    Return parse_line
                End If
                Return emptyStringArray
            Catch e As Exception
                Return emptyStringArray
            End Try
        End Function

        ''' <summary>
        ''' Parse one line of alignment information from the BAM file.
        ''' Return null if there is an error or if we reach the end
        ''' of the file. 
        ''' Returns empty String array if line should be skipped.
        ''' Returns parsed String array with "U" prepended at index 0 if
        ''' read is unaligned and alignment should be attempted.
        ''' Returns parsed String array with "A" prepended at index 0 if
        ''' read has already been aligned.
        ''' </summary>
        Public Overridable Function parseAlignmentLine_BAM() As String()
            Try
                If Me.[in] Is Nothing Then
                    Me.[in] = New GZIPInputStream(New FileInputStream(fileName))
                    disregardHeader_BAM(Me.[in])
                End If
                Dim lengthOfAlignmentSection As Integer = readInt([in])
                Dim repliconIndex As Integer = readInt([in])
                Dim pos As Integer = readInt([in]) + 1
                Dim bin_mq_nl As Integer = readInt([in])
                'int bin = bin_mq_nl >>> 16;
                Dim mapq As Integer = CInt(CUInt(bin_mq_nl) >> 8) And &HFF
                Dim l_read_name As Integer = bin_mq_nl And &HFF
                Dim flag_nc As Integer = readInt([in])
                Dim flag As Integer = CInt(CUInt(flag_nc) >> 16)
                Dim numCigarOps As Integer = flag_nc And &HFFFF
                Dim seqLength As Integer = readInt([in])
                Dim nextRefID As Integer = readInt([in])
                Dim pnext As Integer = readInt([in]) + 1
                Dim tlen As Integer = readInt([in])
                Dim readName As String = readString([in], l_read_name)
                Dim qname As String = readName.Substring(0, readName.Length - 1)
                Dim cigar As String = readCigarString([in], numCigarOps)
                Dim seq As String = readSequence([in], seqLength)
                Dim qual As String = readString([in], seqLength)
                Dim partialLengthOfAlignmentSection As Integer = 8 * 4 + l_read_name + numCigarOps * 4 + ((seqLength + 1) \ 2) + seqLength
                Dim remainder As String = readString([in], lengthOfAlignmentSection - partialLengthOfAlignmentSection)

                If flag >= 256 Then
                    ' Bad or chimeric read
                    badReads.incrementAndGet()
                    Return emptyStringArray
                    ' Unaligned read
                ElseIf ((CInt(CUInt(flag) >> 2) And 1) = 1) OrElse (repliconIndex < 0) OrElse (pos = 0) OrElse (((flag And 1) = 1) AndAlso (tlen = 0)) OrElse isDenovo Then
                    unalignedReads.incrementAndGet()
                    If seq.Length <= 1 Then
                        Return emptyStringArray
                    End If
                    If isDenovo AndAlso ((CInt(CUInt(flag) >> 2) And 1) = 0) AndAlso ((CInt(CUInt(flag) >> 4) And 1) = 0) Then
                        seq = reverseComplement(seq)
                    End If
                    Dim parse_line As String() = New String(10) {}
                    parse_line(0) = "U" & qname
                    ' QNAME; Sentinel: "unaligned"
                    parse_line(1) = flag & ""
                    ' FLAG
                    parse_line(2) = repliconIndex & ""
                    ' RNAME
                    parse_line(3) = pos & ""
                    ' POS
                    parse_line(4) = mapq & ""
                    ' MAPQ
                    parse_line(5) = cigar
                    ' CIGAR
                    parse_line(6) = nextRefID & ""
                    ' RNEXT
                    parse_line(7) = pnext & ""
                    ' PNEXT
                    parse_line(8) = tlen & ""
                    ' TLEN
                    parse_line(9) = seq
                    ' SEQ
                    parse_line(10) = qual
                    ' QUAL
                    If (qual.Length = seq.Length) AndAlso (qual.IndexOf(ControlChars.Tab) = -1) Then
                        Return parse_line
                    ElseIf qualities.Length <> seq.Length Then
                        qualities = (New String(New Char(seq.Length - 1) {})).Replace(ControlChars.NullChar, "("c)
                    End If
                    parse_line(10) = qualities
                    Return parse_line
                    ' Single-end aligned read
                ElseIf (flag And 1) = 0 Then
                    Dim length As Integer = seq.Length
                    Dim isRevComp As Boolean = ((CInt(CUInt(flag) >> 4) And 1) = 1)
                    mapReadToCoords(repliconIndex, pos, length, isRevComp)
                    alignedReads.incrementAndGet(repliconIndex)
                    avgLengthReads.addAndGet(length)
                    Dim parse_line As String() = New String(10) {}
                    parse_line(0) = "A" & qname
                    ' QNAME; Sentinel: "aligned"
                    parse_line(1) = flag & ""
                    ' FLAG
                    parse_line(2) = repliconIndex & ""
                    ' RNAME
                    parse_line(3) = pos & ""
                    ' POS
                    parse_line(4) = mapq & ""
                    ' MAPQ
                    parse_line(5) = cigar
                    ' CIGAR
                    parse_line(6) = nextRefID & ""
                    ' RNEXT
                    parse_line(7) = pnext & ""
                    ' PNEXT
                    parse_line(8) = tlen & ""
                    ' TLEN
                    parse_line(9) = seq
                    ' SEQ
                    parse_line(10) = qual
                    ' QUAL
                    Return parse_line
                    ' First of paired-end aligned read
                ElseIf ((flag And 1) = 1) AndAlso (tlen <> 0) AndAlso ((CInt(CUInt(flag) >> 6) And 1) = 1) AndAlso ((CInt(CUInt(flag) >> 7) And 1) = 0) AndAlso (Math.Abs(tlen) <= maxPairedEndLength) Then
                    Dim isRevComp As Boolean = ((CInt(CUInt(flag) >> 4) And 1) = 1)
                    mapReadToCoords(repliconIndex, pos, tlen, isRevComp)
                    alignedReads.incrementAndGet(repliconIndex)
                    avgLengthReads.addAndGet(Math.Abs(tlen))
                    Dim parse_line As String() = New String(10) {}
                    parse_line(0) = "A" & qname
                    ' QNAME; Sentinel: "aligned"
                    parse_line(1) = flag & ""
                    ' FLAG
                    parse_line(2) = repliconIndex & ""
                    ' RNAME
                    parse_line(3) = pos & ""
                    ' POS
                    parse_line(4) = mapq & ""
                    ' MAPQ
                    parse_line(5) = cigar
                    ' CIGAR
                    parse_line(6) = nextRefID & ""
                    ' RNEXT
                    parse_line(7) = pnext & ""
                    ' PNEXT
                    parse_line(8) = tlen & ""
                    ' TLEN
                    parse_line(9) = seq
                    ' SEQ
                    parse_line(10) = qual
                    ' QUAL
                    Return parse_line
                    ' Second of paired-end aligned read
                ElseIf ((flag And 1) = 1) AndAlso (tlen <> 0) AndAlso ((CInt(CUInt(flag) >> 6) And 1) = 0) AndAlso ((CInt(CUInt(flag) >> 7) And 1) = 1) AndAlso (Math.Abs(tlen) <= maxPairedEndLength) Then
                    Dim parse_line As String() = New String(10) {}
                    parse_line(0) = "A" & qname
                    ' QNAME; Sentinel: "aligned"
                    parse_line(1) = flag & ""
                    ' FLAG
                    parse_line(2) = repliconIndex & ""
                    ' RNAME
                    parse_line(3) = pos & ""
                    ' POS
                    parse_line(4) = mapq & ""
                    ' MAPQ
                    parse_line(5) = cigar
                    ' CIGAR
                    parse_line(6) = nextRefID & ""
                    ' RNEXT
                    parse_line(7) = pnext & ""
                    ' PNEXT
                    parse_line(8) = tlen & ""
                    ' TLEN
                    parse_line(9) = seq
                    ' SEQ
                    parse_line(10) = qual
                    ' QUAL
                    Return parse_line
                End If
                Return emptyStringArray
            Catch e As Exception
                Try
                    If Me.[in] IsNot Nothing Then
                        [in].close()
                        [in] = Nothing
                    End If
                Catch e2 As IOException
                    Return Nothing
                End Try
                Return Nothing
            End Try
        End Function



        ''' <summary>
        '''************************************************
        ''' **********   PRIVATE INSTANCE METHODS   **********
        ''' </summary>

        ''' <summary>
        ''' Read in the first lines of the SAM file, extract useful header info,
        ''' and check validity.
        ''' </summary>
        Private Function prepSAM(numLines As Long) As Boolean
            cigarMap = New Integer(90) {}
            ' Converts cigar characters to counts
            Dim f As New Oracle.Java.IO.File(fileName)
            If Not f.exists() Then
                ' File does not exist
                'output("Error - file " + fileName + " does not exist.\n");
                Return False
            End If
            Try
                Dim reader As New Scanner(f)
                Dim lineCount As Long = CLng(0)
                Dim totalAlignmentLines As Integer = 0
                ' Total number of non-header lines
                While reader.hasNextLine() AndAlso (lineCount < numLines)
                    Dim line As String = reader.nextLine()
                    Dim isValidLine As Boolean
                    If isHeaderLine(line) Then
                        isValidLine = parseHeaderLine_SAM(line)
                    Else
                        isValidLine = checkAlignmentLine_SAM(line)
                        totalAlignmentLines += 1
                    End If
                    If Not isValidLine Then
                        If pairedEndCount = totalAlignmentLines Then
                            isPairedEnd = True
                        End If
                        Return False
                    End If
                    lineCount += 1
                End While
                reader.close()
                If pairedEndCount = totalAlignmentLines Then
                    isPairedEnd = True
                End If
                Return True
            Catch e As FileNotFoundException
                'output("Error - could not read in from file " + fileName + "\n");
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Read in the first lines of the BAM file, extract useful header info,
        ''' and check validity.
        ''' </summary>
        Private Function prepBAM(numLines As Long) As Boolean
            Dim f As New Oracle.Java.IO.File(fileName)
            If Not f.exists() Then
                ' File does not exist
                'output("Error - file " + fileName + " does not exist.\n");
                Return False
            End If
            Try
                Me.[in] = New GZIPInputStream(New FileInputStream(fileName))

                ' Read in header info
                Dim magicString As String = readString([in], 4)
                If Not magicString.Substring(0, 3).ToLower().Equals("bam") Then
                    ' Not a bam file
                    [in].close()
                    [in] = Nothing
                    Return False
                End If
                Dim headerTextLength As Integer = readInt([in])
                Dim samHeaderText As String = readString([in], headerTextLength)
                Dim numRefSeqs As Integer = readInt([in])
                ' Read in each replicon (reference sequence) name and length
                For i As Integer = 0 To numRefSeqs - 1
                    Dim repliconNameLength As Integer = readInt([in])
                    Dim repliconName As String = readString([in], repliconNameLength)
                    Dim repliconLength As Integer = readInt([in])
                    _replicons.Add(repliconName, _replicons.Count)
                    repliconLengths.Add(repliconLength)
                Next

                Dim lineCount As Long = CLng(0)
                Dim isValidLine As Boolean = True
                While isValidLine AndAlso (lineCount < numLines)
                    lineCount += 1
                    isValidLine = checkAlignmentLine_BAM()
                    If Not isValidLine Then
                        If pairedEndCount = lineCount Then
                            isPairedEnd = True
                        End If
                        [in].close()
                        [in] = Nothing
                        Return False
                    End If
                End While
                [in].close()
                [in] = Nothing
                If pairedEndCount = lineCount Then
                    isPairedEnd = True
                End If
                Return True
            Catch e As IOException
                'output("Error - could not read in from file " + fileName + "\n");
                Try
                    [in].close()
                Catch e2 As Exception
                End Try
                [in] = Nothing
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Read in the entire SAM file.
        ''' </summary>
        Private Function readInSAM() As Boolean
            Try
                Dim f As New Oracle.Java.IO.File(fileName)
                Dim reader As New Scanner(f)
                While reader.hasNextLine()
                    parseAlignmentLine_SAM(reader.nextLine())
                End While
                reader.close()
                Return True
            Catch e As FileNotFoundException
                output("Error - could not read in from file " & fileName & vbLf)
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Read in the entire BAM file.
        ''' </summary>
        Private Sub readInBAM()
            While parseAlignmentLine_BAM() IsNot Nothing
            End While
        End Sub

        ''' <summary>
        ''' Parse one line of header information from the SAM file.
        ''' </summary>
        Private Function parseHeaderLine_SAM(line As String) As Boolean
            Try
                Dim parse_line As String() = StringSplit(line, vbTab, True)
                ' Information in header line is currently unused
                If parse_line(0).Equals("@HD") Then
                ElseIf parse_line(0).Equals("@SQ") Then
                    Dim repliconName As String = Nothing
                    Dim repliconLength As Integer = Nothing
                    For i As Integer = 1 To parse_line.Length - 1
                        Dim pair As String() = StringSplit(parse_line(i), ":", True)
                        Dim tag As String = pair(0)
                        Dim value As String = pair(1)
                        If tag.Equals("SN") Then
                            repliconName = value
                        End If
                        If tag.Equals("LN") Then
                            repliconLength = Convert.ToInt32(value)
                        End If
                    Next
                    If (repliconName IsNot Nothing) AndAlso (repliconLength > 0) Then
                        _replicons.Add(repliconName, _replicons.Count)
                        repliconLengths.Add(repliconLength)
                    Else
                        'output("Error - reference sequence header line in SAM file is not formatted as expected:\n");
                        'output("\t" + line + "\n\n");
                        Return False
                    End If
                    ' Information in read group line is currently unused
                ElseIf parse_line(0).Equals("@RG") Then
                    ' Information in program line is currently unused
                ElseIf parse_line(0).Equals("@PG") Then
                    ' Information in comment line is currently unused
                ElseIf parse_line(0).Equals("@CO") Then
                Else
                    ' Unrecognized header record type
                    'output("Error - unrecognized SAM record type " + parse_line[0] + " in header line:\n");
                    'output("\t" + line + "\n\n");
                    Return False
                End If
                Return True
            Catch e As Exception
                'output("Error - line not in expected SAM format:\n");
                'output("\t" + line + "\n\n");
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Read in and disregard header of BAM file.
        ''' </summary>
        'JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        'ORIGINAL LINE: private void disregardHeader_BAM(java.util.zip.GZIPInputStream in) throws java.io.IOException
        Private Sub disregardHeader_BAM([in] As GZIPInputStream)
            Dim magicString As String = readString([in], 4)
            Dim headerTextLength As Integer = readInt([in])
            Dim samHeaderText As String = readString([in], headerTextLength)
            Dim numRefSeqs As Integer = readInt([in])

            ' Read in each replicon (reference sequence) name and length
            For i As Integer = 0 To numRefSeqs - 1
                Dim repliconNameLength As Integer = readInt([in])
                Dim repliconName As String = readString([in], repliconNameLength)
                Dim repliconLength As Integer = readInt([in])
            Next
        End Sub

        ''' <summary>
        ''' Parse one line of alignment information from the SAM file and check its validity.
        ''' </summary>
        Private Function checkAlignmentLine_SAM(line As String) As Boolean
            Try
                Dim parse_line As String() = StringSplit(line, vbTab, True)
                'String qname = parse_line[0];  // QNAME
                Dim flag As Integer = Convert.ToInt32(parse_line(1))
                ' FLAG
                Dim rname As String = parse_line(2)
                ' RNAME
                Dim pos As Integer = Convert.ToInt32(parse_line(3))
                ' POS
                'int mapq = Integer.parseInt(parse_line[4]);  // MAPQ
                'String rnext = parse_line[6];  // RNEXT
                'int pnext = Integer.parseInt(parse_line[7]);  // PNEXT
                Dim tlen As Integer = Convert.ToInt32(parse_line(8))
                ' TLEN
                Dim seq As String = parse_line(9)
                ' SEQ
                Dim qual As String = parse_line(10)
                ' QUAL
                If (flag And 1) = 1 Then
                    pairedEndCount += 1
                End If

                If flag >= 256 Then
                    ' Do nothing
                    ' Bad or chimeric read
                    ' Unaligned read
                ElseIf ((CInt(CUInt(flag) >> 2) And 1) = 1) OrElse (rname.Equals("*")) OrElse (pos = 0) OrElse (((flag And 1) = 1) AndAlso (tlen = 0)) Then
                    If (Not seq.Equals("*")) AndAlso (Not qual.Equals("*")) AndAlso (seq.Length <> qual.Length) Then
                        'output("Error - unrecognized format in SAM file - expecting seq length to equal qual length:\n");
                        'output("\t" + line + "\n\n");
                        Return False
                    End If
                    ' Single-end aligned read
                ElseIf (flag And 1) = 0 Then
                    If Not rname.Equals("*") AndAlso (_replicons.Count > 0) AndAlso (Not _replicons.ContainsKey(rname)) Then
                        'output("Error - unrecognized format in SAM file - expecting RNAME to match a SQ-SN tag:\n");
                        'output("\t" + line + "\n\n");
                        Return False
                    End If
                    parseCigar(parse_line(5))
                    ' CIGAR
                    Dim cigarLength As Integer = (cigarMap("M"c.Asc) + cigarMap("I"c.Asc) + cigarMap("S"c.Asc) + cigarMap("="c.Asc) + cigarMap("X"c.Asc))
                    If Not seq.Equals("*") AndAlso (seq.Length <> cigarLength) Then
                        'output("Error - unrecognized format in SAM file - expecting sequence length to match CIGAR length:\n");
                        'output("\t" + line + "\n\n");
                        Return False
                    End If
                    ' Paired-end aligned read (leftmost mate-pair)
                ElseIf ((flag And 1) = 1) AndAlso (tlen > 0) AndAlso (((CInt(CUInt(flag) >> 6) And 1) = 0) OrElse ((CInt(CUInt(flag) >> 7) And 1) = 0)) Then
                    If Not rname.Equals("*") AndAlso (_replicons.Count > 0) AndAlso (Not _replicons.ContainsKey(rname)) Then
                        'output("Error - unrecognized format in SAM file - expecting RNAME to match a SQ-SN tag:\n");
                        'output("\t" + line + "\n\n");
                        Return False
                    End If
                End If
                Return True
            Catch e As Exception
                'output("Error - line not in expected SAM format:\n");
                'output("\t" + line + "\n\n");
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Parse one line of alignment information from the BAM file and check its validity.
        ''' </summary>
        Private Function checkAlignmentLine_BAM() As Boolean
            Try
                Dim lengthOfAlignmentSection As Integer = readInt([in])
                Dim repliconIndex As Integer = readInt([in])
                Dim pos As Integer = readInt([in]) + 1
                Dim bin_mq_nl As Integer = readInt([in])
                'int bin = bin_mq_nl >>> 16;
                'int mapq = (bin_mq_nl >>> 8) & 0xFF;
                Dim l_read_name As Integer = bin_mq_nl And &HFF
                Dim flag_nc As Integer = readInt([in])
                Dim flag As Integer = CInt(CUInt(flag_nc) >> 16)
                Dim numCigarOps As Integer = flag_nc And &HFFFF
                Dim seqLength As Integer = readInt([in])
                Dim nextRefID As Integer = readInt([in])
                Dim pnext As Integer = readInt([in]) + 1
                Dim tlen As Integer = readInt([in])
                Dim readName As String = readString([in], l_read_name)
                'String qname = readName.substring(0, readName.length()-1);
                For i As Integer = 0 To numCigarOps - 1
                    ' Ignore CIGAR ops
                    readInt([in])
                Next
                Dim seq As String = readSequence([in], seqLength)
                Dim qual As String = readString([in], seqLength)
                Dim partialLengthOfAlignmentSection As Integer = 8 * 4 + l_read_name + numCigarOps * 4 + ((seqLength + 1) \ 2) + seqLength
                Dim remainder As String = readString([in], lengthOfAlignmentSection - partialLengthOfAlignmentSection)

                If (flag And 1) = 1 Then
                    pairedEndCount += 1
                End If
                'this.maxReadLength = Math.max(this.maxReadLength, seq.length());
                For i As Integer = 0 To qual.Length - 1
                    Me._maxQuality = Math.Max(Me._maxQuality, AscW(qual(i)))
                Next

                If flag >= 256 Then
                    ' Do nothing
                    ' Bad or chimeric read
                    ' Unaligned read
                ElseIf ((CInt(CUInt(flag) >> 2) And 1) = 1) OrElse (repliconIndex < 0) OrElse (pos = 0) OrElse (((flag And 1) = 1) AndAlso (tlen = 0)) Then
                    If (Not seq.Equals("*")) AndAlso (Not qual.Equals("*")) AndAlso (seq.Length <> qual.Length) Then
                        'output("Error - unrecognized format in BAM file - expecting seq length to equal qual length:\n");
                        'output("\t" + line + "\n\n");
                        Return False
                    End If
                    ' Single-end aligned read
                ElseIf (flag And 1) = 0 Then
                    If repliconIndex >= _replicons.Count Then
                        'output("Error - unrecognized format in BAM file - expecting valid replicon index:\n");
                        'output("\t" + line + "\n\n");
                        Return False
                    End If
                    ' Paired-end aligned read (leftmost mate-pair)
                ElseIf ((flag And 1) = 1) AndAlso (tlen > 0) AndAlso (((CInt(CUInt(flag) >> 6) And 1) = 0) OrElse ((CInt(CUInt(flag) >> 7) And 1) = 0)) Then
                    If repliconIndex >= _replicons.Count Then
                        'output("Error - unrecognized format in BAM file - expecting RNAME to match a SQ-SN tag:\n");
                        'output("\t" + line + "\n\n");
                        Return False
                    End If
                End If
                Return True
            Catch e As Exception
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Parse a CIGAR string from a SAM file and populate the cigarMap.
        ''' </summary>
        Private Sub parseCigar(s As String)
            For Each ch As Char In cigarChars
                ' Initialize cigar map
                cigarMap(ch.Asc) = 0
            Next
            If s.Equals("*") Then
                Return
            End If
            Dim index As Integer = 0
            While index < s.Length
                Dim sb As New StringBuilder()
                While Char.IsDigit(s(index))
                    sb.Append(s(index))
                    index += 1
                End While
                cigarMap(s(index).Asc) += Convert.ToInt32(sb.ToString())
                index += 1
            End While
        End Sub

        ''' <summary>
        ''' Read in CIGAR information from a BAM file and return the CIGAR string.
        ''' </summary>
        'JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        'ORIGINAL LINE: private String readCigarString(java.util.zip.GZIPInputStream in, int numCigarOps) throws java.io.IOException
        Private Function readCigarString([in] As GZIPInputStream, numCigarOps As Integer) As String
            Dim cigar As String = ""
            For i As Integer = 0 To numCigarOps - 1
                Dim bin_cigar_op As Integer = readInt([in])
                Dim op As Integer = bin_cigar_op And &HF
                If op > 9 Then
                    Return ""
                End If
                Dim op_len As Integer = CInt(CUInt(bin_cigar_op) >> 4)
                cigar += op_len & "" & cigarChars(op)
            Next
            Return cigar
        End Function

        ''' <summary>
        ''' Map a read to the appropriate loci in the appropriate coords array.
        ''' </summary>
        Private Sub mapReadToCoords(repliconIndex As Integer, pos As Integer, length As Integer, isRevComp As Boolean)
            If length < 0 Then
                pos = pos - length + 1
                If pos < 0 Then
                    pos = 0
                End If
                length = -length
            End If
            If coordinates_plus(repliconIndex).length < (pos + length) Then
                ' Increase size of arrays
                SyncLock coordinates_plus
                    If coordinates_plus(repliconIndex).length < (pos + length) Then
                        ' Double check if we want to increase size of arrays
                        Dim new_size As Integer = Math.Max(pos + length, 2 * coordinates_plus(repliconIndex).length)
                        Dim temp_plus As New AtomicIntegerArray(new_size)
                        Dim temp_minus As New AtomicIntegerArray(new_size)
                        For i As Integer = 0 To coordinates_plus(repliconIndex).length - 1
                            temp_plus.[Set](i, coordinates_plus(repliconIndex).[Get](i))
                            temp_minus.[Set](i, coordinates_minus(repliconIndex).[Get](i))
                        Next
                        coordinates_plus(repliconIndex) = temp_plus
                        coordinates_minus(repliconIndex) = temp_minus
                    End If
                End SyncLock
            End If
            If Not isRevComp Then
                ' Plus strand
                For i As Integer = pos To pos + (length - 1)
                    coordinates_plus(repliconIndex).incrementAndGet(i)
                Next
            Else
                ' Minus strand
                For i As Integer = pos To pos + (length - 1)
                    coordinates_minus(repliconIndex).incrementAndGet(i)
                Next
            End If
        End Sub



        ''' <summary>
        '''********************************************
        ''' **********   PUBLIC CLASS METHODS   **********
        ''' </summary>

        ''' <summary>
        ''' Returns true if line is a SAM header line, false otherwise.
        ''' </summary>
        Public Shared Function isHeaderLine(line As String) As Boolean
            Return line.StartsWith("@")
        End Function

        ''' <summary>
        ''' Returns true if the file is a text file.
        ''' Returns false if the file is a binary file (e.g., 
        ''' gzip or bam).
        ''' </summary>
        Public Shared Function isTextFile(fileName As String) As Boolean
            Try
                Dim [in] As New FileInputStream(New Oracle.Java.IO.File(fileName))
                Dim size As Integer = [in].available()
                If size > 1024 Then
                    size = 1024
                End If
                Dim data As SByte() = New SByte(size - 1) {}
                [in].read(data)
                [in].close()

                Dim ascii As Integer = 0
                Dim other As Integer = 0
                For i As Integer = 0 To data.Length - 1
                    Dim b As SByte = data(i)
                    If b < &H9 Then
                        Return False
                    End If
                    If (b = &H9) OrElse (b = &HA) OrElse (b = &HC) OrElse (b = &HD) Then
                        ascii += 1
                    ElseIf (b >= &H20) AndAlso (b <= &H7E) Then
                        ascii += 1
                    Else
                        other += 1
                    End If
                Next
                Return ((100 * ascii \ size) > 95)
            Catch e As IOException
                Return False
            End Try
        End Function



        ''' <summary>
        '''*********************************************
        ''' **********   PRIVATE CLASS METHODS   **********
        ''' </summary>

        Private Shared Sub output(s As String)
            Oracle.Java.System.Err.print(s)
        End Sub

        ''' <summary>
        ''' Converts the first four bytes in the specified array into an integer.
        ''' Assumes little-endianness.
        ''' </summary>
        Private Shared Function byteArrayToInt(b As SByte()) As Integer
            If b.Length < 4 Then
                Return 0
            End If
            'return (b[0] << 24) + ((b[1] & 0xFF) << 16) + ((b[2] & 0xFF) << 8) + (b[3] & 0xFF);
            Return ((b(3) And &HFF) << 24) + ((b(2) And &HFF) << 16) + ((b(1) And &HFF) << 8) + (b(0) And &HFF)
        End Function

        ''' <summary>
        ''' Reads an integer (little-endian) from a gzip/bam file.
        ''' </summary>
        'JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        'ORIGINAL LINE: private static int readInt(java.util.zip.GZIPInputStream in) throws java.io.IOException
        Private Shared Function readInt([in] As GZIPInputStream) As Integer
            [in].read(buffer1)
            intBuffer(0) = buffer1(0)
            [in].read(buffer1)
            intBuffer(1) = buffer1(0)
            [in].read(buffer1)
            intBuffer(2) = buffer1(0)
            [in].read(buffer1)
            intBuffer(3) = buffer1(0)
            Return byteArrayToInt(intBuffer)
        End Function

        ''' <summary>
        ''' Reads a String of the specified length from a gzip/bam file.
        ''' </summary>
        'JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        'ORIGINAL LINE: private static String readString(java.util.zip.GZIPInputStream in, int length) throws java.io.IOException
        Private Shared Function readString([in] As GZIPInputStream, length As Integer) As String
            Dim s As SByte() = New SByte(length - 1) {}
            For i As Integer = 0 To length - 1
                [in].read(buffer1)
                s(i) = buffer1(0)
            Next
            Return (New String(s.CharValue))
        End Function

        ''' <summary>
        ''' Reads a nucleotide sequence of the given length from a gzip/bam file.
        ''' </summary>
        'JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        'ORIGINAL LINE: private static String readSequence(java.util.zip.GZIPInputStream in, int length) throws java.io.IOException
        Private Shared Function readSequence([in] As GZIPInputStream, length As Integer) As String
            Dim sequence As Char() = New Char(length - 1) {}
            Dim NT_index As Integer = 15
            For i As Integer = 0 To (length + 1) \ 2 - 1
                [in].read(buffer1)
                NT_index = CInt(CUInt(buffer1(0)) >> 4) And &HF
                sequence(2 * i) = NT_code(NT_index)
                NT_index = buffer1(0) And &HF
                If 2 * i + 1 < length Then
                    sequence(2 * i + 1) = NT_code(NT_index)
                End If
            Next
            Return (New String(sequence))
        End Function

        ''' <summary>
        ''' Mergesort parallel arrays "a" and "b" based on values in "a"
        ''' </summary>
        Private Shared Sub mergesort(a As Integer(), b As String(), lo As Integer, hi As Integer)
            If lo < hi Then
                Dim q As Integer = (lo + hi) \ 2
                mergesort(a, b, lo, q)
                mergesort(a, b, q + 1, hi)
                merge(a, b, lo, q, hi)
            End If
        End Sub

        ''' <summary>
        ''' Mergesort helper method
        ''' </summary>
        Private Shared Sub merge(a As Integer(), b As String(), lo As Integer, q As Integer, hi As Integer)
            Dim a1 As Integer() = New Integer(q - lo) {}
            Dim a2 As Integer() = New Integer(hi - q - 1) {}
            Dim b1 As String() = New String(q - lo) {}
            Dim b2 As String() = New String(hi - q - 1) {}
            Dim i As Integer = 0
            Dim j As Integer = 0

            For i = 0 To a1.Length - 1
                a1(i) = a(lo + i)
                b1(i) = b(lo + i)
            Next
            For j = 0 To a2.Length - 1
                a2(j) = a(q + 1 + j)
                b2(j) = b(q + 1 + j)
            Next
            i = 0
            ' Index for first half arrays
            j = 0
            ' Index for second half arrays
            For k As Integer = lo To hi
                If i >= a1.Length Then
                    a(k) = a2(j)
                    b(k) = b2(j)
                    j += 1
                ElseIf j >= a2.Length Then
                    a(k) = a1(i)
                    b(k) = b1(i)
                    i += 1
                ElseIf a1(i) <= a2(j) Then
                    a(k) = a1(i)
                    b(k) = b1(i)
                    i += 1
                Else
                    a(k) = a2(j)
                    b(k) = b2(j)
                    j += 1
                End If
            Next
        End Sub

        ''' <summary>
        ''' Returns a reverse complemented version of String s.
        ''' </summary>
        Private Shared Function reverseComplement(s As String) As String
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
        '''***********************************
        ''' **********   MAIN METHOD   **********
        ''' </summary>

        Private Shared Sub Main(args As String())
            If args.Length < 1 Then
                Oracle.Java.System.Err.println(vbLf & "SamOps requires a SAM/BAM file as a command line argument and it outputs information about the SAM/BAM file." & vbLf)
                Oracle.Java.System.Err.println(vbTab & "java SampOps <foo.sam or foo.bam>" & vbLf)
                Environment.[Exit](0)
            End If
            Dim numLines As Long = 10000
            Dim S As New SamOps(args(0), numLines, False)
            ' Read in first few lines to check validity
            If S.validSam Then
                Console.WriteLine(vbLf & "File appears to be in valid SAM format.")
                Console.WriteLine("Does file contain paired-end reads?" & vbTab & S.pairedEnd & vbLf)
                S.readInSAM()
                Console.WriteLine(S.ToString())
            End If
            If S.validBam Then
                Console.WriteLine(vbLf & "File appears to be in valid BAM format.")
                Console.WriteLine("Does file contain paired-end reads?" & vbTab & S.pairedEnd & vbLf)
                S.readInBAM()
                Console.WriteLine(S.ToString())
            End If
        End Sub

    End Class
End Namespace
