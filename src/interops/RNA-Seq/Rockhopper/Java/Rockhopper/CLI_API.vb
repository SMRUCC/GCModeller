#Region "Microsoft.VisualBasic::9ad2a41afaa9bc3b444abae31bcc7f8c, RNA-Seq\Rockhopper\Java\Rockhopper\CLI_API.vb"

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

    '     Module CLI_API
    ' 
    '         Function: separator
    ' 
    '         Sub: commandLineArguments, Main, output, outputDifferentiallyExpressedGenesForBrowser, outputRNAsForBrowser
    '              outputUTRsForBrowser
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic

Namespace Java

    Module CLI_API

        ''' <summary>
        '''***************************************
        ''' **********   CLASS VARIABLES   **********
        ''' </summary>

        Public Const version As String = "2.03"
        'Public Shared output As JTextArea
        ' Output to GUI or, if null, to System.out
        ' Public Shared worker As SwingWorker
        ' Update progress bar if using GUI.
        Public stagesProcessed As Integer
        ' Number of files (stages) processed. Needed for GUI progress bar.
        Public genomeSizes As List(Of Integer)
        ' Sizes of each 1-indexed genome in nucleotides
        Public genomeSize As Integer
        ' Size of all genomes combined
        Public numConditions As Integer
        ' Number of experimental conditions
        Public genome_DIRs As List(Of String)
        ' PARAM: directories of geneome files
        Public output_DIR As String = "Rockhopper_Results/"
        ' PARAM: write output files to this directory
        Public browser_DIR As String = "genomeBrowserFiles/"
        ' Write browser files to this sub-directory
        Public conditionFiles As List(Of String)
        ' PARAM: list of seq-read files for each condition
        Public computeExpression As Boolean = True
        ' PARAM: compute differential expression
        Public computeOperons As Boolean = True
        ' PARAM: compute operons
        Public computeTranscripts As Boolean = True
        ' PARAM: compute transcript boundaries
        Public summaryWriter As PrintWriter = Nothing
        ' For outputting summary file
        Public summaryFile As String = "summary.txt"
        Public expressionFile As String = "transcripts.txt"
        Private operonGenePairFile As String = "operonGenePairs.txt"
        Public operonMergedFile As String = "operons.txt"
        Public unstranded As Boolean = False
        ' Is RNA-seq data strand specific or ambiguous?
        Public transcriptSensitivity As Double = 0.5
        Public verbose As Boolean = False
        Public labels As String()
        Private time As Boolean = False
        Public isDeNovo As Boolean = False
        ' Reference based (false) or de novo (true) assembly

        ''' <summary>
        '''*********************************************
        ''' **********   PRIVATE CLASS METHODS   **********
        ''' </summary>

        ''' <summary>
        ''' Create WIG file of predicted UTRs that
        ''' can be loaded into a genome browser.
        ''' </summary>
        Public Sub outputUTRsForBrowser(outputFile As String, genomeName As String, genes As List(Of Gene), size As Integer)
            ' Determine coordinates of UTRs
            Dim geneCoordinates As Integer() = New Integer(size - 1) {}
            For i As Integer = 0 To genes.Count - 1
                Dim g As Gene = genes(i)

                ' UTRs
                If g.oRF AndAlso (g.startT > 0) AndAlso (g.strand = "+"c) Then
                    ' 5'UTR on plus strand
                    For j As Integer = g.startT To g.start - 1
                        geneCoordinates(j) = 1
                    Next
                End If
                If g.oRF AndAlso (g.startT > 0) AndAlso (g.strand = "-"c) Then
                    ' 5'UTR on minus strand
                    For j As Integer = g.start + 1 To g.startT
                        geneCoordinates(j) = -1
                    Next
                End If
                If g.oRF AndAlso (g.stopT > 0) AndAlso (g.strand = "+"c) Then
                    ' 3'UTR on plus strand
                    For j As Integer = g.[stop] + 1 To g.stopT
                        geneCoordinates(j) = 1
                    Next
                End If
                If g.oRF AndAlso (g.stopT > 0) AndAlso (g.strand = "-"c) Then
                    ' 3'UTR on minus strand
                    For j As Integer = g.stopT To g.[stop] - 1
                        geneCoordinates(j) = -1
                    Next
                End If
            Next

            ' Output differentially expressed genes to genome browser file
            Try
                Dim writer As New PrintWriter(New Oracle.Java.IO.File(outputFile))
                writer.println("track name=" & """" & "UTRs" & """" & " color=255,0,255 altColor=255,0,255 graphType=bar viewLimits=-1:1")
                writer.println("fixedStep chrom=" & genomeName & " start=1 step=1")
                For j As Integer = 1 To geneCoordinates.Length - 1
                    writer.println(geneCoordinates(j))
                Next
                writer.close()
            Catch e As FileNotFoundException
                output(vbLf & "Error - could not open file " & outputFile & vbLf & vbLf)
            End Try
        End Sub

        ''' <summary>
        ''' Create WIG file of predicted RNAs that
        ''' can be loaded into a genome browser.
        ''' </summary>
        Public Sub outputRNAsForBrowser(outputFile As String, genomeName As String, genes As List(Of Gene), size As Integer)
            ' Determine coordinates of ncRNAs
            Dim geneCoordinates As Integer() = New Integer(size - 1) {}
            For i As Integer = 0 To genes.Count - 1
                Dim g As Gene = genes(i)

                ' ncRNAs
                If Not g.oRF AndAlso (g.name.Equals("predicted RNA")) Then
                    ' novel RNA
                    Dim value As Integer = 1
                    If g.strand = "-"c Then
                        value = -1
                    End If
                    For j As Integer = g.first To g.last
                        geneCoordinates(j) = value
                    Next
                End If
            Next

            ' Output differentially expressed genes to genome browser file
            Try
                Dim writer As New PrintWriter(New Oracle.Java.IO.File(outputFile))
                writer.println("track name=" & """" & "Novel RNAs" & """" & " color=0,255,0 altColor=0,255,0 graphType=bar viewLimits=-1:1")
                writer.println("fixedStep chrom=" & genomeName & " start=1 step=1")
                For j As Integer = 1 To geneCoordinates.Length - 1
                    writer.println(geneCoordinates(j))
                Next
                writer.close()
            Catch e As FileNotFoundException
                output(vbLf & "Error - could not open file " & outputFile & vbLf & vbLf)
            End Try
        End Sub

        ''' <summary>
        ''' Create WIG file of differentially expressed genes that
        ''' can be loaded into a genome browser.
        ''' </summary>
        Public Sub outputDifferentiallyExpressedGenesForBrowser(outputFile As String, genomeName As String, genes As List(Of Gene), size As Integer)
            ' Determine coordinates of differentially expressed genes
            Dim geneCoordinates As Integer() = New Integer(size - 1) {}
            For i As Integer = 0 To genes.Count - 1
                Dim g As Gene = genes(i)
                Dim qValue As Double = g.minQvalue
                Dim value As Integer = 0
                If qValue = 0.0 Then
                    ' Special case. We cannot take log of zero.
                    value = 300
                Else
                    value = CInt(Math.Truncate(-Math.Log10(qValue)))
                End If
                If g.strand = "-"c Then
                    value = -value
                End If
                For j As Integer = g.first To g.last
                    geneCoordinates(j) = value
                Next
            Next

            ' Output differentially expressed genes to genome browser file
            Try
                Dim writer As New PrintWriter(New Oracle.Java.IO.File(outputFile))
                writer.println("track name=" & """" & "Differentially expressed genes" & """" & " color=0,255,0 altColor=0,255,0 graphType=bar viewLimits=-10:10")
                writer.println("fixedStep chrom=" & genomeName & " start=1 step=1")
                For j As Integer = 1 To geneCoordinates.Length - 1
                    writer.println(geneCoordinates(j))
                Next
                writer.close()
            Catch e As FileNotFoundException
                output(vbLf & "Error - could not open file " & outputFile & vbLf & vbLf)
            End Try
        End Sub

        ''' <summary>
        ''' Returns the system-dependent file separator that
        ''' can be used as a RegEx.
        ''' </summary>
        Public Function separator() As String
            If System.IO.Path.DirectorySeparatorChar = "\"c Then
                Return "\\"
            Else
                Return Oracle.Java.IO.File.separator
            End If
        End Function



        ''' <summary>
        '''********************************************
        ''' **********   PUBLIC CLASS METHODS   **********
        ''' </summary>

        '' Used for updating the GUI progress bar.
        'Public Shared Sub updateProgress()
        '    If worker IsNot Nothing Then
        '        Dim totalStages As Double = 0.0
        '        For Each files As String In conditionFiles
        '            totalStages += StringSplit(files, ",", True).Length
        '        Next
        '        If isDeNovo Then
        '            For Each files As String In conditionFiles
        '                totalStages += StringSplit(files, ",", True).Length
        '            Next
        '            For Each files As String In conditionFiles
        '                totalStages += StringSplit(files, ",", True).Length
        '            Next
        '        End If
        '        totalStages += 1.0
        '        ' For analyzing transcripts and computing expression
        '        stagesProcessed += 1
        '        '   worker.firePropertyChange("progress", worker.progress, CInt(Math.Truncate(100.0 * stagesProcessed / totalStages)))
        '    End If
        'End Sub

        ''' <summary>从这里进行参数的设置，之后使用<see cref="Rockhopper"/>的构造函数既可以运行整个程序了</summary>
        Public Sub commandLineArguments(args As String())
            If args.IsNullOrEmpty OrElse args.Length < 1 Then
                output(vbLf & "*************************************************" & vbLf)
                output("**********   Rockhopper version " & version & "   **********" & vbLf)
                output("*************************************************" & vbLf)
                output(vbLf & "The Rockhopper application has the following required command line arguments." & vbLf)

                output(vbLf & "REQUIRED ARGUMENTS" & vbLf & vbLf)
                output(vbTab & "exp1A.fastq,exp1B.fastq,exp1C.fastq  exp2A.fastq,exp2B.fastq" & vbTab & "a comma separated list of sequencing files (in FASTQ, QSEQ, FASTA, SAM, or BAM format) for replicate experiments, one list per experimental condition (mate-pair files should be delimited by '%')" & vbLf)

                output(vbLf & "REFERENCE BASED ASSEMBLY VS. DE NONO ASSEMBLY:" & vbLf)
                output("IF THE -g OPTION IS USED THEN ROCKHOPPER ALIGNS READS TO ONE OR MORE REFERENCE GENOMES," & vbLf)
                output("OTHERWISE, ROCKHOPPER PERFORMS DE NOVO TRANSCRIPT ASSEMBLY." & vbLf & vbLf)
                output(vbTab & "-g <DIR1,DIR2>" & vbTab & "a comma separated list of directories, each containing a genome file (*.fna), gene file (*.ptt), and rna file (*.rnt)" & vbLf)

                output(vbLf & "OPTIONAL ARGUMENTS FOR EITHER REFERENCE BASED ASSEMBLY OR DE NOVO ASSEMBLY" & vbLf & vbLf)
                output(vbTab & "-c <boolean>" & vbTab & "reverse complement single-end reads (default is false)" & vbLf)
                output(vbTab & "-ff/fr/rf/rr" & vbTab & "orientation of two mate reads for paired-end read, f=forward and r=reverse_complement (default is fr)" & vbLf)
                output(vbTab & "-d <integer>" & vbTab & "maximum number of bases between mate pairs for paired-end reads (default is 500)" & vbLf)
                output(vbTab & "-a <boolean>" & vbTab & "identify 1 alignment (true) or identify all optimal alignments (false), (default is true)" & vbLf)
                output(vbTab & "-p <integer>" & vbTab & "number of processors (default is self-identification of processors)" & vbLf)
                output(vbTab & "-e <boolean>" & vbTab & "compute differential expression for transcripts in pairs of experimental conditions (default is true)" & vbLf)
                output(vbTab & "-s <boolean>" & vbTab & "RNA-seq experiments are strand specific (true) or strand ambiguous (false), (default is true)" & vbLf)
                output(vbTab & "-L <comma separated list>" & vbTab & "labels for each condition" & vbLf)
                output(vbTab & "-o <DIR>" & vbTab & "directory where output files are written (default is Rockhopper_Results/)" & vbLf)
                output(vbTab & "-v <boolean>" & vbTab & "verbose output including raw/normalized counts aligning to each gene (default is false)" & vbLf)
                output(vbTab & "-SAM       " & vbTab & "output a SAM format file" & vbLf)
                output(vbTab & "-TIME      " & vbTab & "output time taken to execute program" & vbLf)

                output(vbLf & "OPTIONAL ARGUMENTS FOR REFERENCE BASED ASSEMBLY ONLY" & vbLf & vbLf)
                output(vbTab & "-m <number>" & vbTab & "allowed mismatches as percent of read length (default is 0.15)" & vbLf)
                output(vbTab & "-l <number>" & vbTab & "minimum seed as percent of read length (default is 0.33)" & vbLf)
                output(vbTab & "-y <boolean>" & vbTab & "compute operons (default is true)" & vbLf)
                output(vbTab & "-t <boolean>" & vbTab & "identify transcript boundaries including UTRs and ncRNAs (default is true)" & vbLf)
                output(vbTab & "-z <number>" & vbTab & "minimum expression of UTRs and ncRNAs, a number in range [0.0, 1.0] (default is 0.5)" & vbLf)

                output(vbLf & "OPTIONAL ARGUMENTS FOR DE NOVO ASSEMBLY ONLY" & vbLf & vbLf)
                output(vbTab & "-k <integer>" & vbTab & "size of k-mer, range of values is 15 to 31 (default is 25)" & vbLf)
                output(vbTab & "-j <integer>" & vbTab & "minimum length required to use a sequencing read after trimming/processing (default is 35)" & vbLf)
                output(vbTab & "-n <integer>" & vbTab & "size of k-mer hashtable is ~ 2^n (default is 25). HINT: should normally be 25 or, if more memory is available, 26. WARNING: if increased above 25 then more than 1.2M of memory must be allocated" & vbLf)
                output(vbTab & "-b <integer>" & vbTab & "minimum number of full length reads required to map to a de novo assembled trancript (default is 20)" & vbLf)
                output(vbTab & "-u <integer>" & vbTab & "minimum length of de novo assembled transcripts (default is 2*k)" & vbLf)
                output(vbTab & "-w <integer>" & vbTab & "minimum count of k-mer to use it to seed a new de novo assembled transcript (default is 50)" & vbLf)
                output(vbTab & "-x <integer>" & vbTab & "minimum count of k-mer to use it to extend an existing de novo assembled transcript (default is 5)" & vbLf)

                output(vbLf & "EXAMPLE EXECUTION: REFERENCE BASED ASSEMBLY WITH SINGLE-END READS" & vbLf)
                output(vbLf & "java Rockhopper <options> -g genome_DIR1,genome_DIR2 aerobic_replicate1.fastq,aerobic_replicate2.fastq anaerobic_replicate1.fastq,anaerobic_replicate2.fastq" & vbLf)
                output(vbLf & "EXAMPLE EXECUTION: REFERENCE BASED ASSEMBLY WITH PAIRED-END READS" & vbLf)
                output(vbLf & "java Rockhopper <options> -g genome_DIR1,genome_DIR2 aerobic_replicate1_pairedend1.fastq%aerobic_replicate1_pairedend2.fastq,aerobic_replicate2_pairedend1.fastq%aerobic_replicate2_pairedend2.fastq anaerobic_replicate1_pairedend1.fastq%anaerobic_replicate1_pairedend2.fastq,anaerobic_replicate2_pairedend1.fastq%anaerobic_replicate2_pairedend2.fastq" & vbLf)
                output(vbLf & "EXAMPLE EXECUTION: DE NOVO ASSEMBLY WITH SINGLE-END READS" & vbLf)
                output(vbLf & "java Rockhopper <options> aerobic_replicate1.fastq,aerobic_replicate2.fastq anaerobic_replicate1.fastq,anaerobic_replicate2.fastq" & vbLf)
                output(vbLf & "EXAMPLE EXECUTION: DE NOVO ASSEMBLY WITH PAIRED-END READS" & vbLf)
                output(vbLf & "java Rockhopper <options> aerobic_replicate1_pairedend1.fastq%aerobic_replicate1_pairedend2.fastq,aerobic_replicate2_pairedend1.fastq%aerobic_replicate2_pairedend2.fastq anaerobic_replicate1_pairedend1.fastq%anaerobic_replicate1_pairedend2.fastq,anaerobic_replicate2_pairedend1.fastq%anaerobic_replicate2_pairedend2.fastq" & vbLf)

                output(vbLf)
                Environment.[Exit](0)
            End If

            ' Initially set the number of threads
            Peregrine.numThreads = Oracle.Java.System.Runtime.Runtime.availableProcessors()
            If Peregrine.numThreads > 4 Then
                Peregrine.numThreads *= 0.75
            End If
            Assembler.numThreads = Oracle.Java.System.Runtime.Runtime.availableProcessors()
            If Assembler.numThreads > 4 Then
                Assembler.numThreads = CInt(Math.Truncate(Math.Min(Assembler.numThreads * 0.75, 8.0)))
            End If

            Dim i As Integer = 0
            conditionFiles = New List(Of String)()
            genome_DIRs = Nothing
            While i < args.Length
                If args(i).Equals("-g") Then
                    If (i = args.Length - 1) OrElse (args(i + 1).StartsWith("-")) Then
                        output("Error - command line argument -g must be followed by the name of one or more directories." & vbLf)
                        Environment.[Exit](0)
                    End If
                    Dim parse_dirs As String() = StringSplit(args(i + 1), ",", True)
                    genome_DIRs = New List(Of String)()
                    For Each s As String In parse_dirs
                        If s.EndsWith(Oracle.Java.IO.File.separator) Then
                            genome_DIRs.Add(s)
                        Else
                            genome_DIRs.Add(s & System.IO.Path.DirectorySeparatorChar)
                        End If
                    Next
                    isDeNovo = False
                    i += 2
                ElseIf args(i).StartsWith("-a") Then
                    If (i = args.Length - 1) OrElse (args(i + 1).StartsWith("-")) Then
                        output("Error - command line argument -a must be followed by a boolean." & vbLf)
                        Environment.[Exit](0)
                    End If
                    Peregrine.stopAfterOneHit = Convert.ToBoolean(args(i + 1))
                    Assembler.stopAfterOneHit = Convert.ToBoolean(args(i + 1))
                    i += 2
                ElseIf args(i).StartsWith("-p") Then
                    If (i = args.Length - 1) OrElse (args(i + 1).StartsWith("-")) Then
                        output("Error - command line argument -p must be followed by an integer." & vbLf)
                        Environment.[Exit](0)
                    End If
                    Peregrine.numThreads = Convert.ToInt32(args(i + 1))
                    Assembler.numThreads = Convert.ToInt32(args(i + 1))
                    i += 2
                ElseIf args(i).StartsWith("-e") Then
                    If (i = args.Length - 1) OrElse (args(i + 1).StartsWith("-")) Then
                        output("Error - command line argument -e must be followed by a boolean." & vbLf)
                        Environment.[Exit](0)
                    End If
                    computeExpression = Convert.ToBoolean(args(i + 1))
                    Assembler.computeExpression = Convert.ToBoolean(args(i + 1))
                    i += 2
                ElseIf args(i).StartsWith("-s") Then
                    If (i = args.Length - 1) OrElse (args(i + 1).StartsWith("-")) Then
                        output("Error - command line argument -s must be followed by a boolean." & vbLf)
                        Environment.[Exit](0)
                    End If
                    unstranded = Not Convert.ToBoolean(args(i + 1))
                    Assembler.unstranded = Not Convert.ToBoolean(args(i + 1))
                    i += 2
                ElseIf args(i).StartsWith("-d") Then
                    If (i = args.Length - 1) OrElse (args(i + 1).StartsWith("-")) Then
                        output("Error - command line argument -d must be followed by an integer." & vbLf)
                        Environment.[Exit](0)
                    End If
                    Peregrine.maxPairedEndLength = Convert.ToInt32(args(i + 1))
                    Assembler.maxPairedEndLength = Convert.ToInt32(args(i + 1))
                    SamOps.maxPairedEndLength = Convert.ToInt32(args(i + 1))
                    i += 2
                ElseIf args(i).Equals("-L") Then
                    If i = args.Length - 1 Then
                        output("Error - command line argument -L must be followed by a comma separated list of names for the conditions." & vbLf)
                        Environment.[Exit](0)
                    End If
                    labels = StringSplit(args(i + 1), ",", True)
                    Assembler.labels = StringSplit(args(i + 1), ",", True)
                    i += 2
                ElseIf args(i).Equals("-o") Then
                    If (i = args.Length - 1) OrElse (args(i + 1).StartsWith("-")) Then
                        output("Error - command line argument -o must be followed the name of a directory." & vbLf)
                        Environment.[Exit](0)
                    End If
                    If args(i + 1).EndsWith("/") Then
                        output_DIR = args(i + 1)
                    Else
                        output_DIR = args(i + 1) & "/"
                    End If
                    Peregrine.outputDIR = output_DIR
                    Assembler.output_DIR = output_DIR
                    i += 2
                ElseIf args(i).StartsWith("-v") Then
                    If (i = args.Length - 1) OrElse (args(i + 1).StartsWith("-")) Then
                        output("Error - command line argument -v must be followed by a boolean." & vbLf)
                        Environment.[Exit](0)
                    End If
                    verbose = Convert.ToBoolean(args(i + 1))
                    Assembler.verbose = Convert.ToBoolean(args(i + 1))
                    i += 2
                ElseIf args(i).StartsWith("-m") Then
                    If (i = args.Length - 1) OrElse (args(i + 1).StartsWith("-")) Then
                        output("Error - command line argument -m must be followed by a decimal number." & vbLf)
                        Environment.[Exit](0)
                    End If
                    Peregrine.percentMismatches = Convert.ToDouble(args(i + 1))
                    i += 2
                ElseIf args(i).StartsWith("-l") Then
                    If (i = args.Length - 1) OrElse (args(i + 1).StartsWith("-")) Then
                        output("Error - command line argument -l must be followed by a decimal number." & vbLf)
                        Environment.[Exit](0)
                    End If
                    Peregrine.percentSeedLength = Convert.ToDouble(args(i + 1))
                    i += 2
                ElseIf args(i).StartsWith("-y") Then
                    If (i = args.Length - 1) OrElse (args(i + 1).StartsWith("-")) Then
                        output("Error - command line argument -y must be followed by a boolean." & vbLf)
                        Environment.[Exit](0)
                    End If
                    computeOperons = Convert.ToBoolean(args(i + 1))
                    i += 2
                ElseIf args(i).StartsWith("-t") Then
                    If (i = args.Length - 1) OrElse (args(i + 1).StartsWith("-")) Then
                        output("Error - command line argument -t must be followed by a boolean." & vbLf)
                        Environment.[Exit](0)
                    End If
                    computeTranscripts = Convert.ToBoolean(args(i + 1))
                    i += 2
                ElseIf args(i).Equals("-z") Then
                    If (i = args.Length - 1) OrElse (args(i + 1).StartsWith("-")) Then
                        output("Error - command line argument -z must be followed by a number in the range [0.0,1.0]." & vbLf)
                        Environment.[Exit](0)
                    End If
                    transcriptSensitivity = Convert.ToDouble(args(i + 1))
                    i += 2
                ElseIf args(i).StartsWith("-k") Then
                    If (i = args.Length - 2) OrElse (args(i + 1).StartsWith("-")) Then
                        output("Error - command line argument -k must be followed by an integer." & vbLf)
                        Environment.[Exit](0)
                    End If
                    Assembler.k = Convert.ToInt32(args(i + 1))
                    If Assembler.minTranscriptLength = 0 Then
                        Assembler.minTranscriptLength = 2 * Assembler.k
                    End If
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
                ElseIf args(i).StartsWith("-c") Then
                    If (i = args.Length - 1) OrElse (args(i + 1).StartsWith("-")) Then
                        output("Error - command line argument -c must be followed by a boolean." & vbLf)
                        Environment.[Exit](0)
                    End If
                    Peregrine.singleEndOrientationReverseComplement = Convert.ToBoolean(args(i + 1))
                    Assembler.singleEndOrientationReverseComplement = Convert.ToBoolean(args(i + 1))
                    i += 2
                ElseIf args(i).Equals("-ff") Then
                    Peregrine.pairedEndOrientation = "ff"
                    Assembler.pairedEndOrientation = "ff"
                    i += 1
                ElseIf args(i).Equals("-fr") Then
                    Peregrine.pairedEndOrientation = "fr"
                    Assembler.pairedEndOrientation = "fr"
                    i += 1
                ElseIf args(i).Equals("-rf") Then
                    Peregrine.pairedEndOrientation = "rf"
                    Assembler.pairedEndOrientation = "rf"
                    i += 1
                ElseIf args(i).Equals("-rr") Then
                    Peregrine.pairedEndOrientation = "rr"
                    Assembler.pairedEndOrientation = "rr"
                    i += 1
                ElseIf args(i).Equals("-TIME") Then
                    time = True
                    Assembler.time = True
                    i += 1
                ElseIf args(i).Equals("-SAM") Then
                    Peregrine.outputSAM = True
                    Assembler.outputSAM = True
                    i += 1
                Else
                    conditionFiles.Add(args(i))
                    i += 1
                End If
            End While

            ' Handle erroneous command line arguments
            If genome_DIRs Is Nothing Then
                isDeNovo = True
            Else
                For j As Integer = 0 To genome_DIRs.Count - 1
                    If Not (System.IO.Directory.Exists(genome_DIRs(j)) OrElse System.IO.File.Exists(genome_DIRs(j))) Then
                        output("Error - directory " & genome_DIRs(j) & " does not exist." & vbLf)
                        Environment.[Exit](0)
                    End If
                Next
            End If
            If conditionFiles.Count = 0 Then
                output("Error - sequencing reads files (in FASTQ, QSEQ, FASTA, SAM, or BAM format) are required as command line arguments." & vbLf)
                Environment.[Exit](0)
            End If
            Assembler.conditionFiles = conditionFiles
            Assembler.expressionFile = expressionFile
            Assembler.output_DIR = output_DIR
            Peregrine.outputDIR = output_DIR
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
                    output("Error - could not create directory " & output_DIR & "." & vbLf)
                    Environment.[Exit](0)
                End If
            End If
            Peregrine.browserDIR = browser_DIR
            If Not (System.IO.Directory.Exists(output_DIR & browser_DIR) OrElse System.IO.File.Exists(output_DIR & browser_DIR)) Then
                If Not (System.IO.Directory.CreateDirectory(output_DIR & browser_DIR).Exists) Then
                    output("Error - could not create directory " & (output_DIR & browser_DIR) & "." & vbLf)
                    Environment.[Exit](0)
                End If
            End If
            Try
                summaryWriter = New PrintWriter(New Oracle.Java.IO.File(output_DIR & summaryFile))
                Peregrine.summaryWriter = summaryWriter
                Assembler.summaryWriter = summaryWriter
            Catch e As FileNotFoundException
                output("Error - could not create file " & (output_DIR & summaryFile) & vbLf)
                Environment.[Exit](0)
            End Try

        End Sub

        Public Sub output(s As String)
            If summaryWriter IsNot Nothing Then
                summaryWriter.print(s)
            End If

            Console.Write(s)
        End Sub



        ''' <summary>
        '''***********************************
        ''' **********   MAIN METHOD   **********
        ''' </summary>

        ''' <summary>
        ''' The Main method, when invoked with the appropriate command line
        ''' arguments, executes the Rockhopper application for the purpose
        ''' of analyzing RNA-seq data.
        ''' </summary>
        Public Sub Main(args As String())
            commandLineArguments(args)
            Dim runningTime As Long = Oracle.Java.System.CurrentTimeMillis()
            Dim rh As New Rockhopper()
            runningTime = Oracle.Java.System.CurrentTimeMillis() - runningTime
            If time Then
                ' Output time taken to execute program
                Console.WriteLine("Execution time:" & vbTab & (runningTime \ 60000) & " minutes " & ((runningTime Mod 60000) \ 1000) & " seconds" & vbLf)
            End If
        End Sub
    End Module
End Namespace
