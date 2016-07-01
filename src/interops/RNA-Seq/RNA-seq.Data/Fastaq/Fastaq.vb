' http://en.wikipedia.org/wiki/FASTQ_format

Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Fastaq

    ''' <summary>
    ''' FASTQ format is a text-based format for storing both a biological sequence (usually nucleotide sequence) and 
    ''' its corresponding quality scores. Both the sequence letter and quality score are each encoded with a single 
    ''' ASCII character for brevity. It was originally developed at the Wellcome Trust Sanger Institute to bundle a 
    ''' FASTA sequence and its quality data, but has recently become the de facto standard for storing the output of 
    ''' high-throughput sequencing instruments such as the Illumina Genome Analyzer.
    ''' 
    ''' There is no standard file extension for a FASTQ file, but .fq and .fastq, are commonly used.
    ''' </summary>
    ''' <remarks>
    ''' A FASTQ file normally uses four lines per sequence.
    ''' 
    ''' Line 1 begins with a '@' character and is followed by a sequence identifier and an optional description (like a FASTA title line).
    ''' Line 2 is the raw sequence letters.
    ''' Line 3 begins with a '+' character and is optionally followed by the same sequence identifier (and any description) again.
    ''' Line 4 encodes the quality values for the sequence in Line 2, and must contain the same number of symbols as letters in the sequence.
    ''' 
    ''' 一条Fastaq序列文件通常使用4行代表一条序列数据：
    ''' 第一行： 起始于@字符，后面跟随着序列的标识符，以及一段可选的摘要描述信息
    ''' 第二行： 原始的序列
    ''' 第三行： 起始于+符号，与第一行的作用类似
    ''' 第四行： 编码了第二行的序列数据的质量高低，长度与第二行相同
    ''' </remarks>
    Public Class Fastaq : Inherits ISequenceModel
        Implements IAbstractFastaToken

        ''' <summary>
        ''' 第一行的摘要描述信息
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Title As String Implements IAbstractFastaToken.Title
            Get
                Return SEQ_ID.Identifier
            End Get
        End Property

        ''' <summary>
        ''' 第一行的序列标识符
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SEQ_ID As FastaqIdentifier
        Public Property SEQ_ID2 As FastaqIdentifier
        Public Property Quantities As String

        Public Property Attributes As String() Implements IAbstractFastaToken.Attributes

        Public Overrides Function ToString() As String
            Return Title
        End Function

        ''' <summary>
        ''' The character '!' represents the lowest quality while '~' is the highest. Here are the quality value characters in left-to-right increasing order of quality (ASCII):
        ''' </summary>
        ''' <remarks></remarks>
        Public Const QUANTITY_ORDERS As String = "!""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~"

        Public Shared Function GetQuantityOrder(q As Char) As Integer
            Return Fastaq.QUANTITY_ORDERS.IndexOf(q)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="str"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' The original Sanger FASTQ files also allowed the sequence and quality strings to be wrapped (split over multiple lines), 
        ''' but this is generally discouraged as it can make parsing complicated due to the unfortunate choice of "@" and "+" as 
        ''' markers (these characters can also occur in the quality string).[2] An example of a tools that break the 4 line convention 
        ''' is vcfutils.pl from samtools.[3]
        ''' </remarks>
        Public Shared Function FastaqParser(str As String()) As Fastaq
            Dim Fastaq As New Fastaq With {
                .SequenceData = str(1),
                .SEQ_ID = FastaqIdentifier.IDParser(str(0)),
                .SEQ_ID2 = FastaqIdentifier.IDParser(str(2)),
                .Quantities = str(3)
            }

            Return Fastaq
        End Function
    End Class

    ''' <summary>
    ''' Illumina sequence identifiers
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FastaqIdentifier

        'Sequences from the Illumina software use a systematic identifier:

        '@HWUSI-EAS100R:6:73:941:1973#0/1

        'HWUSI-EAS100R	the unique instrument name
        '          6    flowcell lane
        '         73	tile number within the flowcell lane
        '        941	'x'-coordinate of the cluster within the tile
        '       1973    'y'-coordinate of the cluster within the tile
        '         #0	index number for a multiplexed sample (0 for no indexing)
        '         /1	the member of a pair, /1 or /2 (paired-end or mate-pair reads only)

        'Versions of the Illumina pipeline since 1.4 appear to use #NNNNNN instead of #0 for the multiplex ID, 
        'where NNNNNN is the sequence of the multiplex tag.





        'With Casava 1.8 the format of the '@' line has changed:

        '@EAS139:136:FC706VJ:2:2104:15343:197393 1:Y:18:ATCACG

        '       EAS139	the unique instrument name
        '        136	the run id
        '    FC706VJ	the flowcell id
        '          2	flowcell lane
        '       2104	tile number within the flowcell lane
        '      15343	'x'-coordinate of the cluster within the tile
        '     197393	'y'-coordinate of the cluster within the tile
        '          1	the member of a pair, 1 or 2 (paired-end or mate-pair reads only)
        '          Y	Y if the read is filtered, N otherwise
        '         18	0 when none of the control bits are on, otherwise it is an even number
        '     ATCACG	index sequence

        ''' <summary>
        ''' The unique instrument name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Identifier As String
        ''' <summary>
        ''' Flowcell lane
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property FlowCellLane As Integer
        ''' <summary>
        ''' Tile number within the flowcell lane
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Tiles As Integer
        ''' <summary>
        ''' 'x'-coordinate of the cluster within the tile
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property X As Integer
        ''' <summary>
        ''' 'y'-coordinate of the cluster within the tile
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Y As Integer
        ''' <summary>
        ''' Index number for a multiplexed sample (0 for no indexing)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property MsIndex As String
        ''' <summary>
        ''' The member of a pair, /1 or /2 (paired-end or mate-pair reads only)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property PairMember As String

        ''' <summary>
        ''' @HWUSI-EAS100R:6:73:941:1973#0/1
        ''' </summary>
        ''' <param name="str"></param>
        ''' <returns></returns>
        ''' <example>@FCC00ACABXX:8:1101:1333:1980#TTAGGCAT/1</example>
        Public Shared Function IDParser(str As String) As FastaqIdentifier
            If Len(str) = 1 AndAlso str(0) = "+"c Then '可能是第三行数据
                Return New FastaqIdentifier
            End If

            Dim Tokens As String() = str.Split(":"c)
            Dim Identifier As FastaqIdentifier = New FastaqIdentifier
            Identifier.Identifier = Tokens(0)
            Identifier.FlowCellLane = CInt(Val(Tokens(1)))
            Identifier.Tiles = CInt(Val(Tokens(2)))
            Identifier.X = CInt(Val(Tokens(3)))
            Identifier.Y = CInt(Val(Tokens(4)))

            Tokens = Tokens(4).Split("#"c).Last.Split("/"c)

            Identifier.MsIndex = Tokens(0)
            Identifier.PairMember = Tokens(1)

            Return Identifier
        End Function

        Public Overrides Function ToString() As String
            Return $"{Identifier}:{FlowCellLane}:{Tiles}:{X}:{Y}#{MsIndex}/{PairMember}"
        End Function
    End Class
End Namespace