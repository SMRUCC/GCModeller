#Region "Microsoft.VisualBasic::71530ae6f9ac74361b6ee808e7122830, core\Bio.Assembly\Assembly\NCBI\Database\GenBank\GBK\File.vb"

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


    ' Code Statistics:

    '   Total Lines: 351
    '    Code Lines: 173 (49.29%)
    ' Comment Lines: 143 (40.74%)
    '    - Xml Docs: 95.10%
    ' 
    '   Blank Lines: 35 (9.97%)
    '     File Size: 15.85 KB


    '     Class File
    ' 
    '         Properties: Accession, Comment, DbLinks, Definition, Features
    '                     HasSequenceData, isPlasmid, IsWGS, Keywords, Locus
    '                     Origin, Reference, Source, SourceFeature, Taxon
    '                     Version
    ' 
    '         Function: IsValidGenbankFormat, Load, (+2 Overloads) LoadDatabase, Read, readGenbankBuffer
    '                   (+3 Overloads) Save
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports std = System.Math

Namespace Assembly.NCBI.GenBank.GBFF

    ''' <summary>
    ''' NCBI GenBank database file.
    ''' </summary>
    ''' <remarks>
    ''' The National Center for Biotechnology Information (NCBI) GenBank® database is a comprehensive, 
    ''' annotated collection of all publicly available nucleotide sequences and their protein translations. 
    ''' It is a key resource for bioinformatics researchers and scientists around the world. Here is 
    ''' an introduction to the GenBank database file:
    ''' 
    ''' What is GenBank?
    ''' 
    ''' Establishment: GenBank was established in 1982 and is maintained by the NCBI, which is part of 
    ''' the National Library of Medicine (NLM) at the National Institutes of Health (NIH) in the United
    ''' States.
    ''' Content: It contains sequences from various sources, including genomic, transcriptomic, and 
    ''' metagenomic studies, as well as sequences from viruses, prokaryotes, eukaryotes, and organelles.
    ''' Purpose: The primary purpose of GenBank is to provide scientists with a reliable and freely 
    ''' accessible database to deposit and retrieve genetic sequence data.
    ''' Key Components of a GenBank File:
    ''' A GenBank file is typically composed of several sections, each containing specific information about
    ''' the sequence entry. Here are the main components:
    ''' Header Section: This includes the LOCUS line, which provides a summary of the entry, including the
    ''' length of the sequence, the type of molecule (DNA, RNA, or protein), the date of the last update,
    ''' and the unique accession number.
    ''' Definition Line: This is the DEFINITION line, which gives a brief description of the sequence.
    ''' Accession and Version: The ACCESSION line contains the unique identifier for the sequence entry, and
    ''' the VERSION line indicates the version number of the entry and may include a secondary accession number.
    ''' Keywords: The KEYWORDS line provides terms that are associated with the sequence and can be used for searching.
    ''' Source: The SOURCE section describes the organism from which the sequence was derived, including 
    ''' taxonomy and often the specific tissue or cell type.
    ''' References: This section lists the literature citations associated with the sequence data, including
    ''' the authors, title, journal, and publication year.
    ''' Comment: The COMMENT section can contain various types of information, such as the method used for 
    ''' sequence determination, the experimental procedures, or additional notes relevant to the sequence.
    ''' Features Table: This is one of the most important sections, as it provides a detailed annotation of 
    ''' the sequence. It includes feature key-value pairs that describe the locations and characteristics of
    ''' genes, exons, introns, regulatory regions, and other sequence features.
    ''' Sequence Data: The actual nucleotide sequence is presented in the sequence data section, usually in a 
    ''' 60-character-per-line format. This section can be quite long depending on the size of the sequence.
    ''' File Format:
    ''' GenBank files are typically plain text files and can be opened with any text editor. However, they are 
    ''' often handled using bioinformatics tools and software that can parse and interpret the data more effectively. 
    ''' The file extension for GenBank files is usually .gb or .gbk.
    ''' How to Access GenBank:
    ''' You can access GenBank directly through the NCBI website (https://www.ncbi.nlm.nih.gov/genbank/).
    ''' Sequences can be searched by various criteria, including accession number, keyword, author, or organism name.
    ''' Users can also download sequences in various formats, including GenBank flatfile format, FASTA, and others.
    ''' GenBank providing a wealth of data that scientists use for comparative genomics, evolutionary studies,
    ''' and many other types of biological research.
    ''' </remarks>
    '''
    <Package("NCBI.Genbank.GBFF")>
    Public Class File : Implements ISaveHandle

        Public Property Comment As Keywords.COMMENT
        ''' <summary>
        ''' This GenBank keyword section stores the sequence data for this database.
        ''' </summary>
        ''' <returns></returns>
        Public Property Origin As Keywords.ORIGIN
        Public Property Features As Keywords.FEATURES.FEATURES
        ''' <summary>
        ''' LocusID, GI or AccessionID
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Accession As Keywords.ACCESSION
        Public Property Reference As Keywords.REFERENCE
        ''' <summary>
        ''' The definition value for this organism's GenBank data.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Definition As Keywords.DEFINITION
        Public Property Version As Keywords.VERSION

        ''' <summary>
        ''' 物种信息
        ''' </summary>
        ''' <returns></returns>
        Public Property Source As Keywords.SOURCE
        ''' <summary>
        ''' The brief entry information of this genbank data.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Locus As Keywords.LOCUS
        Public Property Keywords As Keywords.KEYWORDS
        Public Property DbLinks As DBLINK

        ''' <summary>
        ''' Is plasmid source?
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>(这个Genbank对象是否为一个质粒的基因组数据)</remarks>
        Public ReadOnly Property isPlasmid As Boolean
            Get
                Return Not String.IsNullOrEmpty(Features.source.Query("plasmid"))
            End Get
        End Property

        ''' <summary>
        ''' 物种数据
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Taxon As String
            Get
                Dim db_xref As String() = Features.source.QueryDuplicated("db_xref")
                Dim LQuery = LinqAPI.DefaultFirst(Of String) <=
                    From s As String
                    In db_xref
                    Let tokens As String() = s.Split(CChar(":"))
                    Where String.Equals(tokens.First, "taxon", StringComparison.OrdinalIgnoreCase)
                    Select tokens.Last

                Return LQuery
            End Get
        End Property

        ''' <summary>
        ''' 这个Genbank对象是否具有序列数据
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property HasSequenceData As Boolean
            Get
                If Origin Is Nothing Then
                    Return False
                Else
                    Return Not String.IsNullOrEmpty(Origin.SequenceData)
                End If
            End Get
        End Property

        ''' <summary>
        ''' This GenBank data is the WGS(Whole genome shotgun) type data.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsWGS As Boolean
            Get
                For Each KeyWord As String In GenBank.GBFF.Keywords.DEFINITION.WGSKeywords
                    If InStr(Definition.Value, KeyWord, CompareMethod.Text) > 0 Then
                        Return True
                    End If

                    For Each s As String In Keywords
                        If InStr(s, KeyWord) = 1 Then
                            Return True
                        End If
                    Next
                Next

                Return False
            End Get
        End Property

        ''' <summary>
        ''' Gets the original source brief entry information of this genome.(获取这个基因组的摘要信息)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property SourceFeature As Feature
            Get
                Return Features.source
            End Get
        End Property

        ''' <summary>
        ''' Read the gene nucleic acid sequence of a gene feature and then returns a fasta sequence object.
        ''' (读取一个基因特性的核酸序列，该Feature对象可以为任意形式的Qualifier的值，但是必需要具有Location属性)
        ''' </summary>
        ''' <param name="feature">The target feature site on the genome DNA sequence.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Read(feature As Feature) As FASTA.FastaSeq
            Dim left As Long = feature.Location.Locations.First.Left
            Dim right As Long = feature.Location.Locations.Last.Right
            Dim sequence As String = Mid(Origin, left, std.Abs(left - right))

            If feature.Location.Complement Then
                sequence = (NucleicAcid.Complement(sequence))
            End If

            Dim fa As New FASTA.FastaSeq With {
                .SequenceData = sequence,
                .Headers = New String() {
                    "Feature",
                    feature.Location.ToString,
                    feature.KeyName
                }
            }

            Return fa
        End Function

        ''' <summary>
        ''' Read a specific GenBank database text file.
        ''' (读取一个特定的GenBank数据库文件)
        ''' </summary>
        ''' <param name="Path">The target database text file to read.(所要读取的目标数据库文件)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Shadows Widening Operator CType(Path As String) As NCBI.GenBank.GBFF.File
            Return GbkParser.Read(Path)
        End Operator

        ''' <summary>
        ''' 当发生错误的时候，会返回空值
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        '''
        <ExportAPI("Load")>
        Public Shared Function Load(path As String) As NCBI.GenBank.GBFF.File
            Return GbkParser.Read(path)
        End Function

        ''' <summary>
        ''' 检查目标文件是否为Genbank文件格式
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Shared Function IsValidGenbankFormat(path As String) As Boolean
            Static samplingHeaders As Index(Of String) = {
                "LOCUS", "DEFINITION", "ACCESSION", "VERSION", "DBLINK"
            }

            Dim hits%

            For Each line As String In path.IterateAllLines.Take(samplingHeaders.Count)
                If line.Split.First Like samplingHeaders Then
                    hits += 1
                End If
            Next

            Return hits / samplingHeaders.Count >= 0.5
        End Function

        Public Const GenbankMultipleRecordDelimiterRegexp$ = "^//$"
        Public Const GenbankMultipleRecordDelimiter$ = "//"

        ''' <summary>
        ''' Using this function to load the ncbi genbank database file if the database file 
        ''' contains more than one genome.
        ''' 
        ''' (假若一个gbk文件之中包含有多个记录的话，可以使用这个函数进行数据的加载，多个genebank记录在一个文件之中
        ''' 一般出现在细菌具有染色体基因组和质粒基因组这种多个复制子的情况)
        ''' </summary>
        ''' <param name="filePath">The file path of the genbank database file, this gb file may contains sevral gb sections</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        '''
        Public Shared Function LoadDatabase(filePath As String, Optional suppressError As Boolean = False) As IEnumerable(Of File)
            Return LoadDatabase(filePath.Open(FileMode.OpenOrCreate, doClear:=False, [readOnly]:=True), filePath.BaseName, suppressError)
        End Function

        ''' <summary>
        ''' Load multiple genbank assembly data that parsed from a specific data file
        ''' </summary>
        ''' <param name="file"></param>
        ''' <param name="defaultAccession"></param>
        ''' <param name="suppressError"></param>
        ''' <returns></returns>
        Public Shared Iterator Function LoadDatabase(file As Stream, Optional defaultAccession$ = Nothing, Optional suppressError As Boolean = False) As IEnumerable(Of File)
            If defaultAccession.StringEmpty Then
                If TypeOf file Is FileStream Then
                    defaultAccession = DirectCast(file, FileStream).Name.BaseName
                Else
                    defaultAccession = "unknown"
                End If
            End If

            For Each buf As String() In readGenbankBuffer(file)
                Try
                    Dim sDat As String() = bufferTrims(buf)

                    If sDat.IsNullOrEmpty Then
                        Continue For
                    Else
                        Yield doLoadData(sDat, defaultAccession)
                    End If
                Catch ex As Exception
                    ex = New Exception(defaultAccession, ex)

                    If Not suppressError Then
                        Throw ex
                    End If
                End Try
            Next
        End Function

        Private Shared Iterator Function readGenbankBuffer(file As Stream) As IEnumerable(Of String())
            Using read As New StreamReader(file)
                Dim buffer As New List(Of String)
                Dim line As Value(Of String) = ""

                Do While Not read.EndOfStream
                    If (line = read.ReadLine) = GenbankMultipleRecordDelimiter Then
                        Yield buffer.PopAll
                    Else
                        buffer += line
                    End If
                Loop

                If buffer > 0 Then
                    Yield buffer.ToArray
                End If
            End Using
        End Function

        Public Function Save(FilePath As String, Encoding As Encoding) As Boolean Implements ISaveHandle.Save
            Using file As Stream = FilePath.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
                Return Save(file, Encoding)
            End Using
        End Function

        Public Function Save(s As Stream, encoding As Encoding) As Boolean Implements ISaveHandle.Save
            Using wr As New StreamWriter(s, encoding)
                Call wr.WriteLine(Me.CreateDoc)
                Call wr.Flush()
            End Using

            Return True
        End Function

        Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return Save(path, encoding.CodePage)
        End Function
    End Class
End Namespace
