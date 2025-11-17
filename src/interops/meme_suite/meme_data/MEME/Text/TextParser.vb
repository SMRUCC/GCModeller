#Region "Microsoft.VisualBasic::1ae96ab14561e526208dda38d4545e49, meme_suite\MEME.DocParser\MEME\Text\TextParser.vb"

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

    '     Module MEME_TEXT
    ' 
    '         Function: __createBlockDiagrams, __getBitsValue, __getFactor, __tryParseMotif, CopyObject
    '                   CopyObjects, DistanceNormalization, ExportMotif, ExportMotifs, GetLength
    '                   GetSubsampleID, Load, Normalization, SafelyLoad, SaveMotifs
    '                   Statics
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.Extensions
Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.ComponentModel
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.LDM
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace DocumentFormat.MEME.Text

    ''' <summary>
    ''' MEME - Motif discovery tool
    ''' </summary>
    ''' <remarks>
    ''' ********************************************************************************
    ''' MEME - Motif discovery tool
    ''' ********************************************************************************
    ''' MEME version 3.5.4 (Release date: 3.5.4)
    ''' 
    ''' For further information on how to interpret these results or to get
    ''' a copy of the MEME software please access http://meme.nbcr.net.
    ''' 
    ''' This file may be used as input to the MAST algorithm for searching
    ''' sequence databases for matches to groups of motifs.  MAST is available
    ''' for interactive use and downloading at http://meme.nbcr.net.
    ''' ********************************************************************************
    ''' 
    ''' 
    ''' ********************************************************************************
    ''' REFERENCE
    ''' ********************************************************************************
    ''' If you use this program in your research, please cite:
    ''' 
    ''' Timothy L. Bailey and Charles Elkan,
    ''' "Fitting a mixture model by expectation maximization to discover
    ''' motifs in biopolymers", Proceedings of the Second International
    ''' Conference on Intelligent Systems for Molecular Biology, pp. 28-36,
    ''' AAAI Press, Menlo Park, California, 1994.
    ''' ********************************************************************************
    ''' </remarks>
    <Package("MEME.Text",
                        Description:="Text file format meme motif data.
                    <br /><br /><br />
<pre>********************************************************************************
MEME - Motif discovery tool
********************************************************************************
MEME version 3.5.4 (Release date: 3.5.4)
 
For further information on how to interpret these results or to get
a copy of the MEME software please access http://meme.nbcr.net.

This file may be used as input to the MAST algorithm for searching
sequence databases for matches to groups of motifs.  MAST is available
for interactive use and downloading at http://meme.nbcr.net.
*******************************************************************************
 
 
********************************************************************************
REFERENCE
********************************************************************************
If you use this program in your research, please cite:
 
Timothy L. Bailey and Charles Elkan,
""Fitting a mixture model by expectation maximization to discover motifs in biopolymers"", Proceedings of the Second International
Conference on Intelligent Systems for Molecular Biology, pp. 28-36,
AAAI Press, Menlo Park, California, 1994.
********************************************************************************</pre>",
                        Url:="http://meme.nbcr.net",
                        Publisher:="Timothy L. Bailey and Charles Elkan",
                        Revision:=33,
                        Cites:="<li>Bailey, T. L., et al. (2006). ""MEME: discovering And analyzing DNA And protein sequence motifs."" Nucleic Acids Res 34(Web Server issue): W369-373.
<p>MEME (Multiple EM for Motif Elicitation) is one of the most widely used tools for searching for novel 'signals' in sets of biological sequences. Applications include the discovery of new transcription factor binding sites and protein domains. MEME works by searching for repeated, ungapped sequence patterns that occur in the DNA or protein sequences provided by the user. Users can perform MEME searches via the web server hosted by the National Biomedical Computation Resource (http://meme.nbcr.net) and several mirror sites. Through the same web server, users can also access the Motif Alignment and Search Tool to search sequence databases for matches to motifs encoded in several popular formats. By clicking on buttons in the MEME output, users can compare the motifs discovered in their input sequences with databases of known motifs, search sequence databases for matches to the motifs and display the motifs in various formats. This article describes the freely accessible web server and its architecture, and discusses ways to use MEME effectively to find new sequence patterns in biological sequences and analyze their significance.</li>///
<li>Bailey, T. L., et al. (2009). ""MEME SUITE: tools for motif discovery And searching."" Nucleic Acids Res 37(Web Server issue): W202-208.
<p>The MEME Suite web server provides a unified portal for online discovery and analysis of sequence motifs representing features such as DNA binding sites and protein interaction domains. The popular MEME motif discovery algorithm is now complemented by the GLAM2 algorithm which allows discovery of motifs containing gaps. Three sequence scanning algorithms--MAST, FIMO and GLAM2SCAN--allow scanning numerous DNA and protein sequence databases for motifs discovered by MEME and GLAM2. Transcription factor motifs (including those discovered using MEME) can be compared with motifs in many popular motif databases using the motif database scanning algorithm TOMTOM. Transcription factor motifs can be further analyzed for putative function by association with Gene Ontology (GO) terms using the motif-GO term association tool GOMO. MEME output now contains sequence LOGOS for each discovered motif, as well as buttons to allow motifs to be conveniently submitted to the sequence and motif database scanning algorithms (MAST, FIMO and TOMTOM), or to GOMO, for further analysis. GLAM2 output similarly contains buttons for further analysis using GLAM2SCAN and for rerunning GLAM2 with different parameters. All of the motif-based tools are now implemented as web services via Opal. Source code, binaries and a web server are freely available for noncommercial use at http://meme.nbcr.net.</li>


")>
    <Cite(Title:="MEME: discovering And analyzing DNA And protein sequence motifs",
          AuthorAddress:="Institute of Molecular Bioscience, The University of Queensland, St Lucia, QLD 4072, Australia. t.bailey@imb.uq.edu.au",
          Authors:="Bailey, T. L. Williams, N. Misleh, C. Li, W. W.",
          PubMed:=16845028,
          Abstract:="MEME (Multiple EM for Motif Elicitation) is one of the most widely used tools for searching for novel 'signals' in sets of biological sequences. 
Applications include the discovery of new transcription factor binding sites and protein domains. MEME works by searching for repeated, ungapped sequence patterns that occur in the DNA or protein sequences provided by the user. 
Users can perform MEME searches via the web server hosted by the National Biomedical Computation Resource (http://meme.nbcr.net) and several mirror sites. 
Through the same web server, users can also access the Motif Alignment and Search Tool to search sequence databases for matches to motifs encoded in several popular formats. By clicking on buttons in the MEME output, users can compare the motifs discovered in their input sequences with databases of known motifs, search sequence databases for matches to the motifs and display the motifs in various formats. 
This article describes the freely accessible web server and its architecture, and discusses ways to use MEME effectively to find new sequence patterns in biological sequences and analyze their significance.",
          DOI:="10.1093/nar/gkl198", Journal:="Nucleic acids research", Year:=2006, Volume:=34, Pages:="W369-73",
          Keywords:="Amino Acid Motifs
Binding Sites
Internet
Protein Structure, Tertiary
Sequence Alignment
Sequence Analysis, DNA/*methods
Sequence Analysis, Protein/*methods
*Software
Transcription Factors/metabolism
User-Computer Interface", Issue:="Web Server issue", ISSN:="1362-4962 (Electronic);
0305-1048 (Linking)")>
    Public Module MEME_TEXT

        Const SPLIT_MOTIFS As String = "********************************************************************************" & vbLf & vbLf & vbLf & "********************************************************************************"

        ''' <summary>
        ''' Load the motif data from the meme text format calculation result
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        <ExportAPI("Load.MEME_Text", Info:="Load the motif data from the meme text format calculation result output file.")>
        Public Function Load(path As String, Optional simplyGuid As Boolean = False) As Motif()
            Dim str As String = FileIO.FileSystem.ReadAllText(path)
            Dim Motifs = Strings.Split(str.Replace(vbCr, ""), SPLIT_MOTIFS)
            Dim size As Dictionary(Of String, Integer) = SiteLength.GetSize(Motifs.ElementAtOrDefault(2))
            Motifs = Motifs.Skip(3).ToArray
            Dim NtMol As Boolean = InStr(str, "ALPHABET= ACGT", CompareMethod.Text) > 0
            Dim list As New List(Of String)

            For Each part As String In Motifs
                If Regex.Match(part, "MOTIF\s+\d+", RegexICSng).Success Then
                    list += part
                Else
                    Exit For
                End If
            Next

            Motifs = list.ToArray

            If Motifs.IsNullOrEmpty Then Return New Motif() {}

            path = FileIO.FileSystem.GetParentPath(path) & "/" & path.BaseName

            Dim Guid As String

            If simplyGuid Then
                Guid = path.BaseName
                If String.Equals(Guid, "meme", StringComparison.OrdinalIgnoreCase) Then
                    Guid = path.ParentDirName
                End If
            Else
                Guid = path.Split(CChar(":")).Last.Replace("\", "/").Replace("/", "_")
            End If

            Dim setValue = New SetValue(Of Motif)().GetSet(NameOf(Motif.NtMolType))
            Dim LQuery As Motif() =
                LinqAPI.Exec(Of Motif) <= From strData As String
                                          In Motifs
                                          Let Motif As Motif = __tryParseMotif(strData, Guid)
                                          Select setValue(Motif, NtMol).SetSize(size)
            Return LQuery
        End Function

        ''' <summary>
        ''' 函数自动从meme.text文档里面解析出序列数据源的长度参数，假若你不太方便手工输入序列长度的话
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Function GetLength(path As String) As Integer
            Try
                Dim str As String = FileIO.FileSystem.ReadAllText(path)
                Dim Motifs = Strings.Split(str.Replace(vbCr, ""), SPLIT_MOTIFS)
                Dim Length As String = Motifs(2)
                Length = Strings.Split(Length, "********************************************************************************")(1)
                Dim Tokens As String() = Length.LineTokens

                If Tokens.Length >= 7 Then
                    Length = Tokens(6)
                Else
                    Length = Tokens(5)
                End If

                Tokens = （From s As String
                       In Length.Split
                          Let ss = s.Trim
                          Where Not String.IsNullOrEmpty(ss)
                          Select ss).ToArray
                Length = Tokens(2)
                Return CInt(Val(Length))
            Catch ex As Exception
                Dim Tokens As String() = FileIO.FileSystem.GetParentPath(path).Replace("\", "/").Split("/"c)

                For i As Integer = Tokens.Length - 1 To 0 Step -1
                    Dim s As String = Tokens(i)
                    If String.Equals(s, Regex.Match(s, "\d+").Value) Then
                        Return CInt(Val(s))
                    End If
                Next

                Call $"{path.ToFileURL} segment length can not be determined...".debug
                Return 150
            End Try
        End Function

        ''' <summary>
        ''' 发生错误会返回空值
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        <ExportAPI("MEME_Text.TryParse")>
        Public Function SafelyLoad(path As String, Optional simplyGuid As Boolean = False) As Motif()
            Try
                Dim doc As Motif() = Load(path, simplyGuid)
                Return doc
            Catch ex As Exception
                ex = New Exception(path.ToFileURL, ex)
                Call ex.PrintException
                Return App.LogException(ex, GetType(MEME_TEXT).FullName & "::" & NameOf(SafelyLoad))
            End Try
        End Function

        Const SPLASH As String = "^[-]+?$"
        Const SECTION_REGEX As String = SPLASH & ".+?\s+Motif \d+.+?" & SPLASH & ".+?" & SPLASH

        ''' <summary>
        ''' 可能会有完全一样的出现
        ''' </summary>
        ''' <param name="str"></param>
        ''' <returns></returns>
        Private Function __createBlockDiagrams(str As String()) As Dictionary(Of String, Integer)
            Dim LQuery = (From s As String In str
                          Let ID As String = s.Split.First
                          Let Right As Double = Val(Regex.Match(s, Site.BLOCK).Value.Split(CChar("_")).Last)
                          Select ID,
                              Right).ToArray
#If DEBUG Then
            Try
#End If
                Dim hash As Dictionary(Of String, Integer) =
                LQuery.RemoveDuplicates(Function(obj) obj.ID) _
                      .ToDictionary(Of String, Integer)(Function(obj) obj.ID,
                                                        Function(obj) CInt(obj.Right))
                Return hash
#If DEBUG Then
            Catch ex As Exception
                Dim DuplicatedPreviews = LQuery.Select(Function(obj) obj.ID).CheckDuplicated(Function(s) s.ToLower)
                Dim Previews As String = String.Join(",  ", DuplicatedPreviews.Select(Function(obj) obj.Group(Scan0)))
                Throw New Exception(Previews, ex)
            End Try
#End If
        End Function

        ''' <summary>
        ''' MOTIF  1	width =   21   sites =  12   llr = 178   E-value = 4.6e-004
        ''' </summary>
        ''' <param name="strData"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function __tryParseMotif(strData As String, Guid As String) As Motif
            Dim SectionData = (From s As String In Regex.Split(strData, SPLASH, RegexOptions.Multiline) Where Not String.Equals(vbLf & vbLf, s) Select s).ToArray
            Dim s_Head As String = SectionData.First
            SectionData = SectionData.Skip(1).ToArray
            SectionData = SectionData.Take(SectionData.Length - 1).ToArray

            Dim SectionBuffer = SectionData.Split(2)
            Dim Sections = (From Tokens In SectionBuffer
                            Select Head = Regex.Replace(Tokens(0), "Motif \d+", ""),
                                Data = Tokens(1)) _
                               .ToDictionary(Function(obj) obj.Head.Replace(vbLf, "").Replace(vbCr, "").Trim,
                                             Function(obj) obj.Data)
            Dim Motif As Motif = New Motif

            Call Sections.TryGetValue("regular expression", Motif.Signature)
            Motif.Signature = Motif.Signature.Replace(vbLf, "")
            Dim Temp As String = ""
            Call Sections.TryGetValue("sites sorted by position p-value", Temp)
            Dim ChunkBuffer As String() = (From s As String In Strings.Split(Temp, vbLf).Skip(3).ToArray Where Not String.IsNullOrEmpty(s) Select s).ToArray
            Motif.Sites = (From s As String In ChunkBuffer Select Site.InternalParser(s)).ToArray
            Call Sections.TryGetValue("block diagrams", Temp)
            ChunkBuffer = (From s As String In Strings.Split(Temp, vbLf).Skip(3) Where Not String.IsNullOrEmpty(s) Select s).ToArray
            Dim BlockDiagrams = __createBlockDiagrams(ChunkBuffer)

            For Each site In Motif.Sites
                site.Right = BlockDiagrams(site.Name)
            Next

            Call Sections.TryGetValue("position-specific probability matrix", Temp)
            ChunkBuffer = (From s As String In Strings.Split(Temp, vbLf) Where Not String.IsNullOrEmpty(s) Select s).ToArray.Skip(1).ToArray
            Motif.PspMatrix = (From s As String In ChunkBuffer Select New MotifPM(s)).ToArray

            Call Sections.TryGetValue("Description", Temp)
            ChunkBuffer = Strings.Split(Strings.Split(Temp, "Multilevel           ").Last, vbLf)
            Dim McsList As New List(Of String)
            Call McsList.Add(ChunkBuffer.First)
            Call McsList.AddRange((From s As String In ChunkBuffer.Skip(1) Where Not String.IsNullOrEmpty(s) Select Mid(s, 22)).ToArray)
            Motif.Mcs = McsList.ToArray
            ChunkBuffer = Temp.Split(CChar(vbLf))
            ChunkBuffer = (From s As String In ChunkBuffer
                           Let ssss As String = Regex.Match(s, BITS).Value
                           Where Not String.IsNullOrEmpty(ssss)
                           Select ssss.TrimStart).ToArray

            Dim BitsHash = (From s As String
                            In ChunkBuffer
                            Let bitsValue As String = Regex.Match(s, "\d+\.\d+").Value
                            Let seq As String = Mid(s, Len(bitsValue) + 2).TrimEnd
                            Select bitsValue, seq).ToDictionary(Function(obj) Val(obj.bitsValue),
                                                                Function(obj) obj.seq)

            For i As Integer = 0 To Motif.PspMatrix.Count - 1
                Motif.PspMatrix(i).Bits = __getBitsValue(i, BitsHash)
            Next

            '  Dim ss As String = Strings.Split(strData, "********************************************************************************").First

            'MOTIF  1	width =   16   sites =  12   llr = 116   E-value = 5.4e-025

            Motif.Id = Regex.Match(s_Head, "MOTIF\s+\d*").Value.Split.Last
            Sections = (From m As Match
                        In Regex.Matches(s_Head, "\S+\s+=\s+\S+")
                        Let Tokens As String() = (From s As String In m.Value.Split Where Not String.IsNullOrEmpty(s) Select s).ToArray
                        Select KEY = Tokens.First,
                            value = Tokens.Last) _
                            .ToDictionary(Function(obj) obj.KEY,
                                          Function(obj) obj.value)

            Motif.Width = CInt(Val(Sections("width")))
            Motif.llr = CInt(Val(Sections("llr")))
            Motif.Evalue = Val(Sections("E-value"))
            Motif.uid = $"{Guid}-{Motif.Id}"

            Return Motif
        End Function

        Private Function __getBitsValue(i As Integer, hash As Dictionary(Of Double, String)) As Double
            Dim LQuery = (From line In hash Select __getFactor(i, line.Value, line.Key)).ToArray.Max
            Return LQuery
        End Function

        Private Function __getFactor(i As Integer, line As String, bits As Double) As Double
            Dim ch As Char

            If i < Len(line) Then
                ch = line(i)
            Else
                ch = "-"c
            End If

            If ch = " "c OrElse ch = "-"c Then
                Return 0
            Else
                Return bits
            End If

        End Function

        Const BITS As String = "    \d+\.\d+(\s|\*)+"

        Private Function GetSubsampleID(s As String) As String
            s = Regex.Replace(s, "_\d+_\d+$", "", RegexOptions.Multiline)
            Return s
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Motifs"></param>
        ''' <param name="getsId">获取基因组编号的函数指针</param>
        ''' <returns></returns>
        ''' <remarks>
        '''          motif1 motif2 motif3
        ''' genome1
        ''' genome2
        ''' genome3
        ''' </remarks>
        <ExportAPI("Motif.Statics")>
        Public Function Statics(Motifs As IEnumerable(Of Motif), Optional getsId As Func(Of String, String) = Nothing) As IO.File
            If getsId Is Nothing Then
                getsId = AddressOf GetSubsampleID
            End If

            Call $"There are {Motifs.Count} motifs from the source!".debug

            Dim LQuery = (From nn In (From motifX As Motif
                                      In Motifs
                                      Select (From site As Site
                                              In motifX.Sites
                                              Let site_id As String = site.Name
                                              Let gid As String = getsId(site_id)
                                              Select gid,
                                                  Motif = motifX.Signature).ToArray).ToArray.Unlist
                          Select nn
                          Group nn By nn.gid Into Group).ToArray
            Dim File As IO.File = New IO.File
            Dim Head As New IO.RowObject From {"GenomeID"}

            For Each Motif In Motifs
                Call Head.Add(Motif.Signature)
            Next

            Call $" There are {LQuery.Length} genomes are ready to go to write into the output data!".debug
            Call File.AppendLine(Head)

            For Each Genome In LQuery
                Dim Row As New IO.RowObject From {Genome.gid}

                Call Console.WriteLine(" {0}   => {1} items", Genome.gid, Genome.Group.Count)

                For Each Motif As Motif In Motifs
                    Dim MLQuery = (From x In Genome.Group.AsParallel Where String.Equals(x.Motif, Motif.Signature) Select x).ToArray.Length
                    Call Row.Add(MLQuery)
                Next

                Call File.Add(Row)
            Next

            Return File
        End Function

        ''' <summary>
        ''' 计算单位段长度之内的motif出现的频率的高低
        ''' </summary>
        ''' <param name="csv"></param>
        ''' <param name="faDIR"></param>
        ''' <param name="scale">-1表示使用自动配置的scale参数，其他的非零的正数则表示指定扩大的级别</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 可以使用本方法所生成的矩阵进行Gene Frequencies and Continuous Character Data Programs的方法进行进化树的绘制
        ''' 
        ''' phylip软件之中的帮助说明
        ''' 
        ''' The programs in this group use gene frequencies and quantitative character values. One (Contml) constructs maximum likelihood estimates of the phylogeny, another (Gendist) computes genetic distances for use in the distance matrix programs, and the third (Contrast) examines correlation of traits as they evolve along a given phylogeny.
        ''' 
        ''' When the gene frequencies data are used in Contml or Gendist, this involves the following assumptions:
        ''' 
        ''' Different lineages evolve independently.
        ''' After two lineages split, their characters change independently.
        ''' Each gene frequency changes by genetic drift, with or without mutation (this varies from method to method).
        ''' Different loci or characters drift independently.
        ''' How these assumptions affect the methods will be seen in my papers on inference of phylogenies from gene frequency and continuous character data (Felsenstein, 1973b, 1981c, 1985c).
        ''' </remarks>
        <ExportAPI("distr.normalization",
             Info:="if the maximum density value is too small(This is mainly caused by the long genome sequence length but fewer number of the motifs, so the density maybe two small.), then you can using the scale(>0) parameter to adjust.")>
        Public Function Normalization(csv As IO.File, faDIR As String, Optional scale As Integer = -1) As IO.File
            Dim File As New IO.File
            Dim fasta = (From path As KeyValuePair(Of String, String)
                         In faDIR.LoadSourceEntryList({})
                         Select path.Key,
                             FastaSeq.Load(path.Value).Length) _
                                .ToDictionary(Function(item) item.Key.ToUpper,
                                              Function(item) item.Length)

            Call File.Add(csv.First)

            For Each row As IO.RowObject In csv.Skip(1)
                Dim newrow = New IO.RowObject From {row.First}
                Dim l As Integer
                If fasta.ContainsKey(row.First) Then
                    l = fasta(row.First)
                Else
                    Call $"Could not fould the genomeId {row.First} in the fasta source!".debug
                    l = 0
                End If

                Call newrow.AddRange((From col In row.Skip(1) Select CStr(Val(col) / l)).ToArray)   '相比于基因组序列的长度，计算出来的频率的值通常会非常的小，所以在这里都乘以100，方便阅读

                Call File.AppendLine(newrow)
            Next

            If scale <= 0 Then
                Dim mnc = (From row In File.Skip(1) Select (From s As String In row.Skip(1) Select Val(s)).ToArray).ToArray.Unlist.Max
                scale = 1 / mnc
            End If

            For Each row In File.Skip(1)
                For i As Integer = 1 To row.Count - 1
                    row(i) = Val(row(i)) * scale
                Next
            Next

            Return File
        End Function

        ''' <summary>
        ''' 计算基因组之间的距离，使用这个文件之中的数据利用飞利浦软件里面的Distance matrix methods方法进行进化树的构建
        ''' </summary>
        ''' <param name="csv"></param>
        ''' <param name="faDIR"></param>
        ''' <param name="queryref">参照的基因组的编号</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Distant.Normalization")>
        Public Function DistanceNormalization(csv As IO.File, faDIR As String, queryref As String) As IO.File
            Dim File As New IO.File
            Dim fasta = (From path As KeyValuePair(Of String, String)
                         In faDIR.LoadSourceEntryList({})
                         Select path.Key,
                             FastaSeq.Load(path.Value).Length) _
                             .ToDictionary(Function(item) item.Key.ToUpper,
                                           Function(item) item.Length)

            Dim Reference = csv.FindAtColumn(KeyWord:=queryref, Column:=0).FirstOrDefault

            If Reference Is Nothing OrElse Reference.IsNullOrEmpty Then
                Call "Could not found the query reference, the calculation is unable to carried out!".debug
                Return csv
            End If

            Call File.Add(csv.First)

            Dim QueryLength As Integer = fasta(Reference.First)  'query的基因组序列的长度
            Dim QueryValue = (From n In Reference.Skip(1) Select Val(n) / QueryLength).ToArray  'motif出现的次数的向量

            For Each row In csv.Skip(1)
                Dim newrow As New IO.RowObject From {row.First}
                Dim l As Integer
                If fasta.ContainsKey(row.First) Then
                    l = fasta(row.First)
                Else
                    Call $"Could not fould the genomeid {row.First} in the fasta source!".debug
                    l = Integer.MaxValue
                End If

                Call newrow.AddRange((From i As Integer In row.Skip(1).Sequence
                                      Let col As Double = Val(row(i))
                                      Select CStr(Math.Abs(col / l - QueryValue(i)))).ToArray)  '使用两个密度值相减的值的绝对值作为两个基因组之间的距离

                Call File.AppendLine(newrow)
            Next

            Return File
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="MEME_Text">MEME text motif 文档的文件路径</param>
        ''' <returns></returns>
        <ExportAPI("Export.Motif")>
        Public Function ExportMotif(<Parameter("Path.MEME.Text")> MEME_Text As String) As MotifSite()
            Dim Motifs = DocumentFormat.MEME.Text.Load(MEME_Text)
            Dim LQuery = (From Motif In Motifs.AsParallel Select CopyObjects(Motif)).ToArray.Unlist
            Return LQuery.ToArray
        End Function

        <ExportAPI("Export.Motifs")>
        Public Function ExportMotifs(Dir As String, <Parameter("Merged")> Optional Merged As Boolean = False) As Boolean
            Dim LQuery = (From path In Dir.LoadSourceEntryList({"*.txt"}).AsParallel Select Motifs = ExportMotif(MEME_Text:=path.Value), ID = path.Key).ToArray
            If Merged Then
                Return (From Motifs In LQuery Select Motifs.Motifs).ToArray.Unlist.SaveTo(Dir & "/MotifsChunk.csv", False)
            Else

                For Each Motif In LQuery
                    Call Motif.Motifs.SaveTo($"{Dir}/{Motif.ID}.csv", False)
                Next

                Return True
            End If
        End Function

        <ExportAPI("Write.Csv.Motifs")>
        Public Function SaveMotifs(Motifs As Generic.IEnumerable(Of MotifSite), SaveTo As String) As Boolean
            Return Motifs.SaveTo(SaveTo, False)
        End Function

        <ExportAPI("MotifSites.Create")>
        Public Function CopyObjects(Motif As LDM.Motif) As MotifSite()
            Dim LQuery = (From site As Site In Motif.Sites Select CopyObject(Motif, site)).ToArray
            Return LQuery
        End Function

        Public Function CopyObject(Motif As LDM.Motif, site As Site) As MotifSite
            Return New MotifSite With {
                .Sequence = site.Site,
                .Locus_tag = site.Name,
                .Start = site.Start,
                .Signature = Motif.Signature,
                .EValue = Motif.Evalue,
                .PValue = site.Pvalue,
                .RightEndDownStream = -(Motif.Width + site.Right),
                .uid = Motif.uid
            }
        End Function
    End Module
End Namespace
