#Region "Microsoft.VisualBasic::8fdd703e0c3f767571dbf48e832b04b2, GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\GenBank\TabularFormat\FeatureBriefs\PTT\PTTDbLoader.vb"

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

    '   Total Lines: 283
    '    Code Lines: 187
    ' Comment Lines: 58
    '   Blank Lines: 38
    '     File Size: 12.58 KB


    '     Class PTTDbLoader
    ' 
    '         Properties: Count, GeneFastas, Keys, Proteins, RptGenomeBrief
    '                     Values
    ' 
    '         Constructor: (+3 Overloads) Sub New
    '         Function: ContainsKey, CreateObject, ExportCOGProfiles, GenomeFasta, GetEnumerator
    '                   GetEnumerator1, GetGeneUniqueId, GetLocusId, (+2 Overloads) GetRelatedGenes, ORF_PTT
    '                   RNARnt, TryGetValue
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ContextModel
Imports SMRUCC.genomics.SequenceModel

Namespace Assembly.NCBI.GenBank.TabularFormat

    Public Class PTTDbLoader : Inherits TabularLazyLoader
        Implements IReadOnlyDictionary(Of String, GeneBrief)

        Dim _ORF_PTT As PTT
        Dim _RNARnt As PTT
        Dim _genomeOrigin As FASTA.FastaSeq
        Dim _lstFile As PTTEntry

        ''' <summary>
        ''' 整个基因组中的所有基因的集合，包括有蛋白质编码基因和RNA基因
        ''' </summary>
        ''' <remarks></remarks>
        Dim _genomeContext As New Dictionary(Of String, GeneBrief)

        Public ReadOnly Property GeneFastas As Dictionary(Of String, FastaObjects.Fasta)
        Public ReadOnly Property Proteins As Dictionary(Of String, FastaObjects.Fasta)

        ''' <summary>
        ''' 从PTT基因组注释数据之中获取COG分类数据
        ''' </summary>
        ''' <typeparam name="T_Entry"></typeparam>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExportCOGProfiles(Of T_Entry As IGeneBrief)() As T_Entry()
            Dim LQuery As T_Entry() =
                LinqAPI.Exec(Of T_Entry) <= From gene As GeneBrief
                                            In Me._genomeContext.Values.AsParallel
                                            Select gene.getCOGEntry(Of T_Entry)()
            Return LQuery
        End Function

        ''' <summary>
        ''' 从其他的数据类型进行数据转换，转换数据格式为PTT格式，以方便用于后续的分析操作用途
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateObject(briefs As IEnumerable(Of IGeneBrief), SourceFasta As FASTA.FastaSeq) As PTTDbLoader
            Dim BriefData As GeneBrief() =
                LinqAPI.Exec(Of GeneBrief) <= From prot As IGeneBrief
                                              In briefs
                                              Select GeneBrief.CreateObject(prot)
            Return New PTTDbLoader With {
                ._lstFile = New PTTEntry,
                ._ORF_PTT = New PTT With {
                    .GeneObjects = BriefData,
                    .Size = SourceFasta.Length,
                    .Title = SourceFasta.Title
                },
                ._DIR = "NULL",
                ._genomeOrigin = SourceFasta,
                ._genomeContext = BriefData.ToDictionary(Function(gene) gene.Synonym),
                ._RptGenomeBrief = New Rpt With {
                    .Size = SourceFasta.Length,
                    .NumberOfGenes = briefs.Count
                }
            }
        End Function

        Dim __contextProvider As GenomeContextProvider(Of GeneBrief)

        ''' <summary>
        ''' 会依照所传递进来的<paramref name="Strand">链的方向的参数</paramref>来查找与之相关的基因
        ''' </summary>
        ''' <param name="LociStart"></param>
        ''' <param name="LociEnds"></param>
        ''' <param name="Strand"></param>
        ''' <param name="ATGDistance"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetRelatedGenes(LociStart As Integer, LociEnds As Integer, Strand As Strands, Optional ATGDistance As Integer = 500) As Relationship(Of GeneBrief)()
            Dim loci As New NucleotideLocation(LociStart, LociEnds, Strand)
            Return __contextProvider.GetAroundRelated(loci, True, ATGDistance)
        End Function

        ''' <summary>
        ''' This function will ignore the nucleoside direction adn founds the gene on both strand of the DNA.(将会忽略DNA链，两条链的位置上都会寻找)
        ''' </summary>
        ''' <param name="LociStart"></param>
        ''' <param name="LociEnds"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetRelatedGenes(LociStart As Integer, LociEnds As Integer, Optional ATGDistance As Integer = 500) As Relationship(Of GeneBrief)()
            Dim loci As New NucleotideLocation(LociStart, LociEnds, Strands.Forward)
            Return __contextProvider.GetAroundRelated(loci, False, ATGDistance)
        End Function

        Protected Sub New()
            Call MyBase.New("", DbAPI.PTTs)
        End Sub

        Sub New(entry As PTTEntry, Optional simplify As Boolean = False)
            Call MyBase.New(entry.DIR, DbAPI.PTTs)

            Dim strict As Boolean = Not simplify

            _lstFile = entry
            _RptGenomeBrief = Rpt.Load(Of Rpt)(_lstFile.rpt)

            For Each genEntry As GeneBrief In ORF_PTT().GeneObjects
                Call _genomeContext.Add(genEntry.Synonym, genEntry)
            Next
            If Not RNARnt() Is Nothing Then
                For Each genEntry As GeneBrief In RNARnt()?.GeneObjects.SafeQuery
                    Call _genomeContext.Add(genEntry.Synonym, genEntry)
                Next
            End If

            Dim FastaFile As FASTA.FastaFile
            FastaFile = FASTA.FastaFile.Read(_lstFile.faa, strict)
            Proteins = (From prot As FASTA.FastaSeq
                        In FastaFile.SafeQuery
                        Let uniqueId As String = GetLocusId(prot.Headers.ElementAtOrDefault(1), prot.Headers.First)
                        Select FastaObjects.Fasta.CreateObject(uniqueId, prot)) _
                            .ToDictionary(Function(x) x.UniqueId)

            FastaFile = FASTA.FastaFile.Read(_lstFile.ffn, strict)
            GeneFastas = (From genFa As FASTA.FastaSeq
                          In FastaFile.SafeQuery
                          Let UniqueId As String = GetGeneUniqueId(genFa.Headers.ElementAtOrDefault(4), genFa.Headers.First)
                          Select FastaObjects.Fasta.CreateObject(UniqueId, genFa)) _
                             .ToDictionary(Function(x As FastaObjects.Fasta) x.UniqueId)
            FastaFile = FASTA.FastaFile.Read(_lstFile.frn, strict)
            For Each genFa As FastaObjects.Fasta In (From fa As FASTA.FastaSeq
                                                     In FastaFile.SafeQuery
                                                     Let UniqueId As String = Regex.Match(fa.Title, "locus_tag=[^]]+").Value.Split(CChar("=")).Last
                                                     Select FastaObjects.Fasta.CreateObject(UniqueId, fa))
                Call GeneFastas.Add(genFa.UniqueId, genFa)
            Next
        End Sub

        ''' <summary>
        ''' 请注意，通过这个构造函数只能够读取一个数据库的数据，假若一个文件夹里面同时包含有了基因组数据和质粒的数据，则不推荐使用这个函数进行构造加载
        ''' </summary>
        ''' <param name="DIR"></param>
        ''' <param name="simplify">数据库是精简版的，有些文件是缺失的</param>
        Sub New(DIR As String, Optional simplify As Boolean = False)
            Call Me.New(DbAPI.GetEntryList(DIR).First, simplify)
        End Sub

        ''' <summary>
        ''' 基因序列中的pid似乎是无效的，都一样的，只能通过location来获取序列的标识号了
        ''' </summary>
        ''' <param name="Location"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetGeneUniqueId(Location As String, [default] As String) As String
            If Location Is Nothing Then
                Return [default]
            Else
                Location = Location.Split.First
            End If

            Dim Points As Integer() = LinqAPI.Exec(Of Integer) <=
                From m As Match
                In Regex.Matches(Location, "\d+")
                Let n As Integer = CInt(Val(m.Value))
                Select n
                Order By n Ascending
            Dim LQuery As String = LinqAPI.DefaultFirst(Of String)([default]) <=
                From g In _genomeContext
                Where g.Value.Location.Left = Points.First AndAlso
                    g.Value.Location.Right = Points.Last
                Select g.Key

            Return LQuery
        End Function

        Private Function GetLocusId(Pid As String, [default] As String) As String
            Dim LQuery As String = LinqAPI.DefaultFirst(Of String)([default]) <=
                From g As KeyValuePair(Of String, GeneBrief)
                In _genomeContext
                Where String.Equals(Pid, g.Value.PID)
                Select g.Key

            Return LQuery
        End Function

        ''' <summary>
        ''' *.ptt
        ''' </summary>
        ''' <returns></returns>
        Public Function ORF_PTT() As PTT
            If _ORF_PTT Is Nothing Then
                _ORF_PTT = PTT.Load(_lstFile.ptt)
            End If
            Return _ORF_PTT
        End Function

        ''' <summary>
        ''' *.rpt
        ''' </summary>
        ''' <returns></returns>
        Public Function RNARnt() As PTT
            If _RNARnt Is Nothing Then
                _RNARnt = PTT.Load(_lstFile.rnt)
            End If
            Return _RNARnt
        End Function

        ''' <summary>
        ''' (*.fna)(基因组的全长序列)
        ''' </summary>
        ''' <returns></returns>
        Public Function GenomeFasta() As FASTA.FastaSeq
            If _genomeOrigin Is Nothing Then
                _genomeOrigin = FASTA.FastaSeq.LoadNucleotideData(_lstFile.fna)
            End If
            Return _genomeOrigin
        End Function

        ''' <summary>
        ''' *.rpt
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property RptGenomeBrief As Rpt

#Region " Implements IReadOnlyDictionary(Of String, PTT.GeneBrief)"

        Public Iterator Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of String, TabularFormat.ComponentModels.GeneBrief)) Implements IEnumerable(Of KeyValuePair(Of String, TabularFormat.ComponentModels.GeneBrief)).GetEnumerator
            For Each Item As KeyValuePair(Of String, TabularFormat.ComponentModels.GeneBrief) In _genomeContext
                Yield Item
            Next
        End Function

        Public ReadOnly Property Count As Integer Implements IReadOnlyCollection(Of KeyValuePair(Of String, TabularFormat.ComponentModels.GeneBrief)).Count
            Get
                Return _genomeContext.Count
            End Get
        End Property

        Public Function ContainsKey(key As String) As Boolean Implements IReadOnlyDictionary(Of String, TabularFormat.ComponentModels.GeneBrief).ContainsKey
            Return _genomeContext.ContainsKey(key)
        End Function

        Default Public ReadOnly Property Item(key As String) As TabularFormat.ComponentModels.GeneBrief Implements IReadOnlyDictionary(Of String, TabularFormat.ComponentModels.GeneBrief).Item
            Get
                If _genomeContext.ContainsKey(key) Then
                    Return _genomeContext(key)
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Public ReadOnly Property Keys As IEnumerable(Of String) Implements IReadOnlyDictionary(Of String, TabularFormat.ComponentModels.GeneBrief).Keys
            Get
                Return _genomeContext.Keys
            End Get
        End Property

        Public Function TryGetValue(key As String, ByRef value As TabularFormat.ComponentModels.GeneBrief) As Boolean Implements IReadOnlyDictionary(Of String, TabularFormat.ComponentModels.GeneBrief).TryGetValue
            Return _genomeContext.TryGetValue(key, value)
        End Function

        Public ReadOnly Property Values As IEnumerable(Of TabularFormat.ComponentModels.GeneBrief) Implements IReadOnlyDictionary(Of String, TabularFormat.ComponentModels.GeneBrief).Values
            Get
                Return _genomeContext.Values
            End Get
        End Property

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function

#End Region

        Public Shared Narrowing Operator CType(PTTDb As PTTDbLoader) As GeneBrief()
            Return PTTDb.Values.ToArray
        End Operator
    End Class
End Namespace
