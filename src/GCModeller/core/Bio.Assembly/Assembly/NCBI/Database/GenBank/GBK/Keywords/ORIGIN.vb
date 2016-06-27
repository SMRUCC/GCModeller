Imports System.Text
Imports System.Xml.Serialization
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports LANS.SystemsBiology.SequenceModel
Imports LANS.SystemsBiology.SequenceModel.FASTA
Imports LANS.SystemsBiology.SequenceModel.NucleotideModels
Imports Microsoft.VisualBasic.Language

Namespace Assembly.NCBI.GenBank.GBFF.Keywords

    ''' <summary>
    ''' This GenBank keyword section stores the sequence data for this database.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ORIGIN : Inherits KeyWord
        Implements IAbstractFastaToken
        Implements IEnumerable(Of Char)

        ''' <summary>
        ''' The sequence data that stores in this GenBank database, which can be a genomics DNA sequence, protein sequence or RNA sequence.(序列数据，类型可以包括基因组DNA序列，蛋白质序列或者RNA序列)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlIgnore> Public Property SequenceData As String Implements I_PolymerSequenceModel.SequenceData
            Get
                Return __sequenceParser.OriginalSequence
            End Get
            Set(value As String)
                Try
                    __sequenceParser = New SegmentReader(value)
                Catch ex As Exception
                    __sequenceParser = New SegmentReader(NucleicAcid.CopyNT(value))
                    Call $"[WARN] The origin nucleic acid sequence contains illegal character in the nt sequence, ignored as character N...".__DEBUG_ECHO
                End Try
            End Set
        End Property

        Dim __sequenceParser As SegmentReader

        Default Public ReadOnly Property [Char](index As Long) As Char
            Get
                Return SequenceData(index)
            End Get
        End Property

        Public ReadOnly Property SequenceParser As SegmentReader
            Get
                Return __sequenceParser
            End Get
        End Property

        ''' <summary>
        ''' 基因组的大小
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Size As Long
            Get
                Return Len(__sequenceParser.OriginalSequence)
            End Get
        End Property

        ''' <summary> 
        ''' 获取该Feature位点的序列数据
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetFeatureSegment(Feature As Feature) As String
            Return __sequenceParser.TryParse(Feature.Location.ContiguousRegion).SequenceData
        End Function

        Public Overrides Function ToString() As String
            Return SequenceData
        End Function

        ''' <summary>
        ''' 是整条序列的GC偏移
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property GCSkew As Double
            Get
                Dim G As Integer = (From ch In Me.SequenceData Where ch = "G"c OrElse ch = "g"c Select 1).Count
                Dim C As Integer = (From ch In Me.SequenceData Where ch = "C"c OrElse ch = "c"c Select 1).Count
                Return (G + C) / (G - C)
            End Get
        End Property

        Public Shared Widening Operator CType(Data As String()) As ORIGIN
            Dim sBuilder As StringBuilder = New StringBuilder(2048)

            For Each line As String In Data
                sBuilder.Append(Mid$(line, 10))
            Next

            Dim trimChars As Char() =
                LinqAPI.Exec(Of Char) <= From b As Char In sBuilder.ToString
                                         Where b <> " "c
                                         Select b

            Return New ORIGIN With {
                .SequenceData = New String(trimChars)
            }
        End Operator

        Public Shared Narrowing Operator CType(ori As ORIGIN) As String
            Return ori.SequenceData
        End Operator

        Public Shared Narrowing Operator CType(obj As ORIGIN) As FastaToken
            Return obj.ToFasta
        End Operator

        ''' <summary>
        ''' Returns the whole genome sequence which was records in this GenBank database file.
        ''' (返回记录在本Genbank数据库文件之中的全基因组核酸序列)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ToFasta() As FastaToken
            Dim attrs As String() = {"GBK_ORIGIN", "Length=" & Len(SequenceData)}
            Return New FastaToken(attrs, SequenceData)
        End Function

        Public ReadOnly Property Title As String Implements IAbstractFastaToken.Title
            Get
                Return "GBK_ORIGIN"
            End Get
        End Property

        Public Property Attributes As String() Implements IAbstractFastaToken.Attributes

        Public Iterator Function GetEnumerator() As IEnumerator(Of Char) Implements IEnumerable(Of Char).GetEnumerator
            For Each ch As Char In SequenceData
                Yield ch
            Next
        End Function

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace

