#Region "Microsoft.VisualBasic::d572c06cc90291b5405a78857cb3cc2a, localblast\LocalBLAST\LocalBLAST\LocalBLAST\Application\NtMapping\BlastnMapping.vb"

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

    '     Class BlastnMapping
    ' 
    '         Properties: AlignmentFullLength, Evalue, Extensions, GapsFraction, gapsValue
    '                     IdentitiesFraction, identitiesValue, PerfectAlignment, QueryLeft, QueryLength
    '                     QueryRight, QueryStrand, RawScore, ReadQuery, Reference
    '                     ReferenceLeft, ReferenceRight, ReferenceStrand, Score, Strand
    '                     Unique
    ' 
    '         Function: __getMappingLoci, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Map(Of String, String)
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace LocalBLAST.Application.NtMapping

    ''' <summary>
    ''' Blastn Mapping for fastaq
    ''' </summary>
    ''' <remarks>
    ''' ##### BLAST word-size
    '''
    ''' Length of an exact sequence match, as start region for the final alignment
    '''
    ''' ```
    ''' blastn -query genes.ffn -subject genome.fna -word_size 11
    ''' ```
    '''
    ''' A BLAST search starts With finding a perfect sequence match Of length given by -word_size. 
    ''' This initial region Of an exact sequence match Is Then extended In both direction allowing 
    ''' gaps And substitutions based On the scoring thresholds. 
    '''
    ''' Changing the initial word-size can help To find more, but less accurate hits; Or To limit 
    ''' the results To almost perfect hits. 
    '''
    ''' + Decreasing the word-size will increase the number Of detected homologous sequences, but hits 
    ''' can include alignments Of higher fragmentation due To gaps And substitutions (example: search 
    ''' for homologous genes between distant species, see also: -task blastn)
    ''' + Increasing the word-size will give less hits As it requires a longer continuous regions Of 
    ''' exact match. If the word-size Is chosen To be almost the size Of the query, BLAST will search 
    ''' For almost exact matches (example: search for location of gene sequences in the original genome 
    ''' of the gene)
    '''
    ''' For Short sequences, word-size must be less than half the query length, otherwise reliable hits can be missed.
    '''
    ''' Default word-sizes
    '''
    ''' + nucleotide sequence search blastn With Default megablast (bastn): ``-word_size 28``   
    ''' + nucleotide sequence search blastn only (bastn -task blastn): ``-word_size 11``  
    ''' + amino acid search (blastp): ``-word_size 3`` 
    '''
    ''' Setting the word-size To a very low value (-word_size 5) makes a blastn search very slow. 
    ''' 
    ''' 如果做motif位点搜索，因为motif序列通常比较短，并且有些区域差异很大，所以word size可以设置的比较小
    ''' </remarks>
    Public Class BlastnMapping : Inherits Contig
        Implements IMap
        Implements IMapping

        ''' <summary>
        ''' The name of the reads query
        ''' </summary>
        ''' <returns></returns>
        <Column("Reads.Query")> Public Property ReadQuery As String Implements IMap.Key, IMapping.Qname
        ''' <summary>
        ''' The name of the reference genome sequence.
        ''' </summary>
        ''' <returns></returns>
        Public Property Reference As String Implements IMap.Maps, IMapping.Sname
        ''' <summary>
        ''' Length of <see cref="ReadQuery"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property QueryLength As Integer
        <Column("Score(bits)")> Public Property Score As Integer
        <Column("Score(Raw)")> Public Property RawScore As Integer
        <Column("E-value")> Public Property Evalue As Double
        ''' <summary>
        ''' Identities(%)
        ''' </summary>
        ''' <returns></returns>
        <Column("Identities(%)")> Public Property identitiesValue As Double
        <Column("Identities")> Public Property IdentitiesFraction As String
            Get
                Return _identitiesFraction
            End Get
            Set(value As String)
                _identitiesFraction = value

                If Not String.IsNullOrEmpty(value) Then
                    Dim tokens As String() = value.Replace("\", "/").Split("/"c)

                    If tokens.Count > 1 Then
                        __identitiesFraction = CInt(Math.Abs(Val(tokens(Scan0)) - Val(tokens(1))))
                    Else
                        __identitiesFraction = Integer.MaxValue
                    End If
                Else
                    __identitiesFraction = Integer.MaxValue
                End If
            End Set
        End Property

        Dim _identitiesFraction As String
        Dim __identitiesFraction As Integer

        ''' <summary>
        ''' Gaps(%)
        ''' </summary>
        ''' <returns></returns>
        <Column("Gaps(%)")> Public Property gapsValue As String
        <Column("Gaps")> Public Property GapsFraction As String

#Region "Public Property Strand As String"

        <Ignored> Public ReadOnly Property QueryStrand As Strands
        ''' <summary>
        ''' 在进行装配的时候是以基因组上面的链方向以及位置为基准的
        ''' </summary>
        ''' <returns></returns>
        <Ignored> Public ReadOnly Property ReferenceStrand As Strands

        Dim _strand As String

        Public Property Strand As String
            Get
                Return _strand
            End Get
            Set(value As String)
                _strand = value

                If String.IsNullOrEmpty(value) Then
                    Me._QueryStrand = Strands.Unknown
                    Me._ReferenceStrand = Strands.Unknown
                    Return
                End If

                Dim Tokens As String() = value.Split("/"c)
                Me._QueryStrand = GetStrand(Tokens(Scan0))
                Me._ReferenceStrand = GetStrand(Tokens(1))
            End Set
        End Property
#End Region

        <Column("Left(Query)")> Public Property QueryLeft As Integer Implements IMapping.Qstart
        <Column("Right(Query)")> Public Property QueryRight As Integer Implements IMapping.Qstop
        <Column("Left(Reference)")> Public Property ReferenceLeft As Integer Implements IMapping.Sstart
        <Column("Right(Reference)")> Public Property ReferenceRight As Integer Implements IMapping.Sstop

        'Public Property Lambda As Double
        'Public Property K As Double
        'Public Property H As Double

        '<Column("Lambda(Gapped)")> Public Property Lambda_Gapped As Double
        '<Column("K(Gapped)")> Public Property K_Gapped As Double
        '<Column("H(Gapped)")> Public Property H_Gapped As Double

        '<Column("Effective Search Space")> Public Property EffectiveSearchSpaceUsed As String

        ''' <summary>
        ''' Unique?(这个属性值应该从blastn日志之中导出mapping数据的时候就执行了的)
        ''' </summary>
        ''' <returns></returns>
        <Column("Unique?")> Public Property Unique As Boolean
        <Column("FullLength?")> Public ReadOnly Property AlignmentFullLength As Boolean
            Get
                Return QueryLeft = 1 AndAlso QueryLength = QueryRight
            End Get
        End Property

        ''' <summary>
        ''' Perfect?
        ''' </summary>
        ''' <returns></returns>
        <Column("Perfect?")> Public ReadOnly Property PerfectAlignment As Boolean
            Get
                ' Explicit conditions
                Return (identitiesValue = 100.0R AndAlso __identitiesFraction <= 3) AndAlso Val(gapsValue) = 0R
            End Get
        End Property

        <Meta(GetType(String))>
        Public Property Extensions As Dictionary(Of String, String)

        ''' <summary>
        ''' 不存在的键名会返回空值
        ''' </summary>
        ''' <param name="key$"></param>
        ''' <returns></returns>
        Default Public ReadOnly Property Data(key$) As String
            Get
                If Extensions.ContainsKey(key) Then
                    Return Extensions(key)
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{Me.ReadQuery} //{MappingLocation.ToString}"
        End Function

        Protected Overrides Function __getMappingLoci() As NucleotideLocation
            Return New NucleotideLocation(ReferenceLeft, ReferenceRight, ReferenceStrand)
        End Function
    End Class
End Namespace
