' /********************************************************************************/
'
'  Rockhopper —— Genome 数据模型
'
'  复刻自原始 Rockhopper（Brian Tjaden, 2013）Java 源码中的 Genome.java。
'  迁移要点：
'    * `Oracle.Java.IO.File.list()` → `System.IO.Directory.GetFiles`
'    * `Scanner/PrintWriter`      → `StreamReader/StreamWriter`
'    * 基因组序列改为强类型 FastaSeq 读取。
'    * 算法逻辑（1-indexed 序列、基因合并、GFF 输出、注释图）保持不变。
'
' /********************************************************************************/

Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Core

    ''' <summary>
    ''' An instance of the Genome class represents a genome and its
    ''' annotation, including the genome sequence, protein-coding
    ''' genes, and RNA genes in the genome.
    ''' </summary>
    Public Class Genome

        Private m_ID As String
        Private _name As String
        ''' <summary>
        ''' 基因组核酸序列。首位追加一个字符以保证 1-indexed 访问。
        ''' </summary>
        Private genome As String
        ''' <summary>
        ''' First token of FASTA header line, e.g., gi|49175990|ref|NC_000913.2|
        ''' </summary>
        Private _formalGenomeName As String = ""
        ''' <summary>
        ''' Path and name of genome file sans .fna extension.
        ''' </summary>
        Private _baseFileName As String = ""
        Private ReadOnly _codingGenes As New List(Of Gene)()
        Private ReadOnly rnas As New List(Of Gene)()
        ''' <summary>Merged coding genes and RNAs.</summary>
        Private _genes As New List(Of Gene)()

        ''' <summary>
        ''' Constructs a new Genome object based on files in the specified
        ''' directory (e.g., genome.fna, genes.ptt, rna.rnt).
        ''' </summary>
        Public Sub New(directory As String)
            Dim genomeFileName As String = Nothing
            Dim geneFileName As String = Nothing
            Dim rnaFileName As String = Nothing

            If Not directory.EndsWith("/") AndAlso Not directory.EndsWith("\") Then
                directory += IO.Path.DirectorySeparatorChar
            End If

            If Directory.Exists(directory) Then
                For Each file As String In Directory.GetFiles(directory)
                    If file.EndsWith(".fna") Then genomeFileName = file
                    If file.EndsWith(".ptt") Then geneFileName = file
                    If file.EndsWith(".rnt") Then rnaFileName = file
                Next
            End If

            If genomeFileName IsNot Nothing Then
                Me.genome = readInGenome(genomeFileName)
            Else
                Me.genome = "?"
            End If
            If geneFileName IsNot Nothing Then
                Me._codingGenes = readInGenes(geneFileName, "ORF")
            End If
            If rnaFileName IsNot Nothing Then
                Me.rnas = readInGenes(rnaFileName, "RNA")
            End If
            Me._genes = mergeGenes(Me._codingGenes, Me.rnas)
            If genomeFileName IsNot Nothing Then
                Call create_GFF_file(Me._genes, genomeFileName, Me.genome.Length - 1)
                Me._baseFileName = genomeFileName.Substring(0, genomeFileName.Length - 4)
            End If
        End Sub

        ''' <summary>
        ''' Constructs a new Genome object based on the specified files
        ''' (e.g., genome.fna, genes.ptt, rna.rnt). If any of the file
        ''' names are empty, then they are simply ignored.
        ''' </summary>
        Public Sub New(genomeFileName As String, geneFileName As String, rnaFileName As String)
            If Not String.IsNullOrEmpty(genomeFileName) Then
                Me.genome = readInGenome(genomeFileName)
            Else
                Me.genome = "?"
            End If
            If Not String.IsNullOrEmpty(geneFileName) Then
                Me._codingGenes = readInGenes(geneFileName, "ORF")
            End If
            If Not String.IsNullOrEmpty(rnaFileName) Then
                Me.rnas = readInGenes(rnaFileName, "RNA")
            End If
            Me._genes = mergeGenes(Me._codingGenes, Me.rnas)
            If Not String.IsNullOrEmpty(genomeFileName) Then
                Call create_GFF_file(Me._genes, genomeFileName, Me.genome.Length - 1)
                Me._baseFileName = genomeFileName.Substring(0, genomeFileName.Length - 4)
            End If
        End Sub

        ''' <summary>
        ''' Returns the size in nucleotides of the genome sequence.
        ''' Note: the returned size will be 1 bigger than the actual genome
        ''' size since we add a character to the beginning of the genome
        ''' sequence to ensure 1-indexing.
        ''' </summary>
        Public Function Size() As Integer
            Return genome.Length
        End Function

        ''' <summary>Returns the ID of the genome.</summary>
        Public ReadOnly Property ID As String
            Get
                Return m_ID
            End Get
        End Property

        ''' <summary>Returns the name of the genome.</summary>
        Public ReadOnly Property Name As String
            Get
                Return _name
            End Get
        End Property

        ''' <summary>
        ''' Returns the path and name of the genome file sans .fna extension.
        ''' </summary>
        Public ReadOnly Property BaseFileName As String
            Get
                Return _baseFileName
            End Get
        End Property

        ''' <summary>Returns the raw (1-indexed) genome sequence.</summary>
        Public ReadOnly Property Sequence As String
            Get
                Return genome
            End Get
        End Property

        ''' <summary>
        ''' Returns a subsequence of the (1-indexed) genome as specified by
        ''' the two coordinates, inclusive.
        ''' </summary>
        Public Function GetSeq(start As Integer, [stop] As Integer) As String
            Return genome.Substring(System.Math.Max(start, 1), System.Math.Min([stop] + 1, genome.Length) - System.Math.Max(start, 1))
        End Function

        ''' <summary>Return the number of genes in the genome.</summary>
        Public Function NumGenes() As Integer
            Return _genes.Count
        End Function

        ''' <summary>
        ''' Returns the formal genome name, e.g., gi|49175990|ref|NC_000913.2|
        ''' </summary>
        Public ReadOnly Property FormalGenomeName As String
            Get
                Return Me._formalGenomeName
            End Get
        End Property

        ''' <summary>Return the Gene at the specified index.</summary>
        Public Function GetGene(i As Integer) As Gene
            If i >= 0 AndAlso i < _genes.Count Then
                Return _genes(i)
            End If
            Return Nothing
        End Function

        ''' <summary>Return the collection of protein coding genes.</summary>
        Public ReadOnly Property CodingGenes As List(Of Gene)
            Get
                Return Me._codingGenes
            End Get
        End Property

        ''' <summary>Return the collection of RNA genes.</summary>
        Public ReadOnly Property RnaGenes As List(Of Gene)
            Get
                Return Me.rnas
            End Get
        End Property

        ''' <summary>
        ''' 将预测得到的新转录本（RNA）合并进基因集合。
        ''' </summary>
        Public Sub AddPredictedRNAs(predictedRNAs As List(Of Gene))
            Me._genes = mergeGenes(Me._genes, predictedRNAs)
        End Sub

        ''' <summary>Return the collection of all genes.</summary>
        Public ReadOnly Property Genes As List(Of Gene)
            Get
                Return Me._genes
            End Get
        End Property

        ''' <summary>
        ''' Returns a String[] representing the annotation
        ''' (gene, rRNA, tRNA, RNA, "") for each coordinate in the
        ''' genome on the specified strand.
        ''' </summary>
        Public Function GetAnnotations(strand As Char) As String()
            Dim annotation As String() = New String(genome.Length - 1) {}
            For i As Integer = 0 To annotation.Length - 1
                annotation(i) = ""
            Next
            For i As Integer = 0 To _codingGenes.Count - 1
                Dim g As Gene = _codingGenes(i)
                If g.Strand = strand Then
                    For j As Integer = g.First To g.Last
                        annotation(j) = g.Name
                    Next
                End If
            Next
            For i As Integer = 0 To rnas.Count - 1
                Dim g As Gene = rnas(i)
                If g.Strand = strand Then
                    For j As Integer = g.First To g.Last
                        If g.Product.Contains("tRNA") Then
                            annotation(j) = "tRNA"
                        ElseIf g.Product.Contains("rRNA") OrElse g.Product.Contains("ribosomal") Then
                            annotation(j) = "rRNA"
                        Else
                            annotation(j) = "RNA"
                        End If
                    Next
                End If
            Next
            Return annotation
        End Function

        ''' <summary>
        ''' Return a String representation of all genes in the genome
        ''' (即 *_transcripts.txt 的表头与数据行）。
        ''' </summary>
        Public Function GenesToString(conditions As List(Of Condition), labels As String()) As String
            Dim sb As New StringBuilder()
            sb.Append("Transcription Start" & vbTab & "Translation Start" & vbTab & "Translation Stop" & vbTab & "Transcription Stop" & vbTab & "Strand" & vbTab & "Name" & vbTab & "Synonym" & vbTab & "Product")
            For j As Integer = 0 To conditions.Count - 1
                Dim conditionName As String = "" & (j + 1)
                If labels IsNot Nothing AndAlso labels.Length = conditions.Count Then
                    conditionName = labels(j)
                End If
                If Logging.Verbose Then
                    If conditions(j).NumReplicates() = 1 Then
                        sb.Append(vbTab & "Raw Counts " & conditionName)
                        sb.Append(vbTab & "Normalized Counts " & conditionName)
                    Else
                        For k As Integer = 0 To conditions(j).NumReplicates() - 1
                            sb.Append(vbTab & "Raw Counts " & conditionName & " Replicate " & (k + 1))
                        Next
                        For k As Integer = 0 To conditions(j).NumReplicates() - 1
                            sb.Append(vbTab & "Normalized Counts " & conditionName & " Replicate " & (k + 1))
                        Next
                    End If
                    sb.Append(vbTab & "RPKM " & conditionName)
                End If
                sb.Append(vbTab & "Expression " & conditionName)
            Next

            Dim numQvalues As Integer = 0
            For x As Integer = 0 To conditions.Count - 2
                For y As Integer = x + 1 To conditions.Count - 1
                    If Logging.Verbose Then
                        If labels IsNot Nothing AndAlso labels.Length = conditions.Count Then
                            sb.Append(vbTab & "pValue " & labels(x) & " vs " & labels(y))
                        Else
                            sb.Append(vbTab & "pValue " & (x + 1) & " vs " & (y + 1))
                        End If
                    End If
                    If NumGenes() > 0 AndAlso _genes(0).HasQvalue(numQvalues) Then
                        If labels IsNot Nothing AndAlso labels.Length = conditions.Count Then
                            sb.Append(vbTab & "qValue " & labels(x) & " vs " & labels(y))
                        Else
                            sb.Append(vbTab & "qValue " & (x + 1) & " vs " & (y + 1))
                        End If
                    End If
                    numQvalues += 1
                Next
            Next
            sb.AppendLine()
            For i As Integer = 0 To NumGenes() - 1
                sb.Append(_genes(i).ToString() & _genes(i).ExpressionToString() & vbLf)
            Next
            Return sb.ToString()
        End Function

        ''' <summary>
        ''' 从 FASTA 文件读取基因组序列，并解析 ID 与名称。
        ''' </summary>
        Private Function readInGenome(fileName As String) As String
            Me.m_ID = ""
            Me._name = ""

            Dim fasta As FastaSeq = FastaSeq.Load(fileName)
            Dim sequence As String = "?" & fasta.SequenceData

            Dim parse_header As String() = fasta.Title.Split("|"c)
            If parse_header.Length >= 5 Then
                Me.m_ID = parse_header(3).Split("."c)(0)
                Me._name = parse_header(4).Split(","c)(0).Trim()
            End If

            Return sequence
        End Function

        ''' <summary>
        ''' Reads in a file of genes (either *.ptt or *.rnt) and returns
        ''' a list of gene objects.
        ''' </summary>
        Private Function readInGenes(fileName As String, type As String) As List(Of Gene)
            Dim listOfGenes As New List(Of Gene)()
            Try
                Dim lines As String() = File.ReadAllLines(fileName)
                ' Ignore 3 header lines
                For i As Integer = 3 To lines.Length - 1
                    If String.IsNullOrWhiteSpace(lines(i)) Then Continue For
                    listOfGenes.Add(New Gene(lines(i), type))
                Next
            Catch e As FileNotFoundException
                Call Logging.Output($"Error - the file {fileName} could not be found and opened.{vbLf}")
            End Try
            Return listOfGenes
        End Function

        ''' <summary>
        ''' Given two lists of Genes, creates a new list of Genes containing
        ''' all the Genes from the two specified lists combined.
        ''' </summary>
        Private Function mergeGenes(genes1 As List(Of Gene), genes2 As List(Of Gene)) As List(Of Gene)
            Dim listOfGenes As New List(Of Gene)()
            If genes1 IsNot Nothing AndAlso genes1.Count > 0 Then listOfGenes.AddRange(genes1)
            If genes2 IsNot Nothing AndAlso genes2.Count > 0 Then listOfGenes.AddRange(genes2)
            Return listOfGenes
        End Function

        ''' <summary>
        ''' Based on a list of genes and a FASTA genome file, create a GFF file
        ''' that can be read by a genome browser. The newly created file is
        ''' output to the same directory as the FASTA genome file and has the
        ''' same name except with the extension *.gff.
        ''' </summary>
        Private Sub create_GFF_file(genes As List(Of Gene), genomeFileName As String, genomeSize As Integer)
            Dim GFF_fileName As String = genomeFileName.Substring(0, genomeFileName.Length - 4) & ".gff"
            Try
                Me._formalGenomeName = FastaSeq.Load(genomeFileName).Title.Split(" "c)(0)

                Using writer As New StreamWriter(GFF_fileName)
                    writer.WriteLine("track name=Genes color=255,0,255")
                    writer.WriteLine("##gff-version 3")
                    writer.WriteLine($"##sequence-region {_formalGenomeName} 1 {genomeSize}")
                    For i As Integer = 0 To genes.Count - 1
                        Dim g As Gene = genes(i)
                        Dim type As String = "RNA"
                        Dim start As Integer = g.MinCoordinate
                        Dim [stop] As Integer = g.MaxCoordinate
                        If g.ORF Then
                            type = "Coding gene"
                            start = System.Math.Min(g.Start, g.[Stop])
                            [stop] = System.Math.Max(g.Start, g.[Stop])
                        End If
                        writer.WriteLine($"{_formalGenomeName}{vbTab}RefSeq{vbTab}{type}{vbTab}{start}{vbTab}{[stop]}{vbTab}.{vbTab}{g.Strand}{vbTab}.{vbTab}name={g.Name};product=""{g.Product}""")
                    Next
                End Using
            Catch e As IOException
                Call Logging.Output($"Error - could not create GFF file {GFF_fileName}{vbLf}")
            End Try
        End Sub

    End Class

End Namespace
