Imports System.Text
Imports LANS.SystemsBiology.ComponentModel.Loci
Imports LANS.SystemsBiology.SequenceModel.ISequenceModel

Namespace SequenceModel.NucleotideModels

    ''' <summary>
    ''' The reader of the nucleotide sequence loci segment.(核酸链上面的一个片段区域的读取对象，注意，这个数据结构都是以正义链为标准的)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class SegmentReader : Inherits SegmentObject
        Implements I_PolymerSequenceModel

        ''' <summary>
        ''' 返回当前阅读区域之中的序列数据，请尽量使用本属性读取序列数据，但是请小心的通过本属性写入数据，因为每一次写入数据，都会重置内部的<see cref="_innerNTsource"></see>对象的值
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Property SequenceData As String
            Get
                Dim s As String = _reader.ReadDownStream(Left, FragmentSize, True)

                If Complement Then
                    Return NucleicAcid.Complement(s)
                Else
                    Return s
                End If
            End Get
            Set(value As String)
                _innerNTsource = New NucleicAcid(value)
                Me.Left = 0
                Me.Right = Len(value)
                Me.Complement = False
            End Set
        End Property

        Dim _innerNTsource As NucleicAcid
        Dim _isCircularMolecular As Boolean

        ''' <summary>
        ''' 这个属性是原始的完整的序列数据
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property OriginalSequence As String
            Get
                Return _innerNTsource.SequenceData
            End Get
        End Property

        Protected ReadOnly _reader As InternalReader

        Sub New(Optional LinearMolecule As Boolean = True)
            _isCircularMolecular = Not LinearMolecule
            _reader = New InternalReader(Me)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="NucleicAcid"></param>
        ''' <param name="LinearMolecule">本DNA分子是否为线状的分子</param>
        Sub New(NucleicAcid As FASTA.FastaToken, Optional LinearMolecule As Boolean = True)
            _innerNTsource = New NucleicAcid(NucleicAcid)
            _isCircularMolecular = Not LinearMolecule
            _reader = New InternalReader(Me)
            Complement = False
        End Sub

        Sub New(NucleicAcid As String, Optional LinearMolecule As Boolean = True)
            _innerNTsource = New NucleicAcid(NucleicAcid)
            _isCircularMolecular = Not LinearMolecule
            _reader = New InternalReader(Me)
            Complement = False
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="SequenceData"></param>
        ''' <param name="Location"></param>
        ''' <param name="LinearMolecule">是否为环状的DNA分子，默认为线状的DNA分子</param>
        ''' <remarks></remarks>
        Sub New(SequenceData As NucleicAcid, Location As NucleotideLocation, Optional LinearMolecule As Boolean = True)
            _innerNTsource = SequenceData
            _isCircularMolecular = Not LinearMolecule
            _reader = New InternalReader(Me)
            Left = Location.Left
            Right = Location.Right
            Complement = Location.Strand = Strands.Reverse
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="SequenceData"></param>
        ''' <param name="LinearMolecule">
        ''' Does this DNA sequence is a circular DNA molecule or a linear DNA molecule, default it is a linear DNA molecule.(是否为环状的DNA分子，默认为线形)
        ''' </param>
        ''' <remarks></remarks>
        Sub New(SequenceData As NucleicAcid, Optional LinearMolecule As Boolean = True)
            _innerNTsource = SequenceData
            _isCircularMolecular = Not LinearMolecule
            _reader = New InternalReader(Me)
        End Sub

        ''' <summary>
        ''' 按照给定的位置来获取序列片段，请注意，这个函数仅仅是针对正义链的
        ''' 例如：Left=10,  Right=50，则函数会取出10-50bp这个区间的长度为41个碱基的序列片段
        ''' </summary>
        ''' <param name="Left"></param>
        ''' <param name="Right"></param>
        ''' <returns></returns>
        Public Function GetSegmentSequence(Left As Integer, Right As Integer) As String
            Dim Length As Integer = Right - Left + 1
            If Length <= 0 Then
                Call Console.WriteLine("[DEBUG] {0}  ==> {1}, length {2} is a negative or ZERO value!", Left, Right, Length)
                Return ""
            End If
            Return _reader.GetSegmentSequenceValue(Left - 1, Length, True)
        End Function

        ''' <summary>
        ''' Try get the nucleotide sequence from a specific start point with a certen length.(这个函数仅仅是解析正向链的序列数据的)
        ''' </summary>
        ''' <param name="Start">This should be the left position of the target segment.</param>
        ''' <param name="SegLength">Segment length of the target sequence.</param>
        ''' <param name="directionDownstream">是否是截取下游的序列数据</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function TryParse(Start As Integer, SegLength As Integer, Optional directionDownstream As Boolean = True, Optional WARN As Boolean = True) As String
            If Start <= 0 OrElse SegLength <= 0 Then
                Dim exMsg As String =
                    $"Parameter error during trying to parsing the nt sequence:  {NameOf(Start)}:={Start}; {NameOf(SegLength)}:={SegLength}; {NameOf(directionDownstream)}:={directionDownstream}..."
                Throw New Exception(exMsg)
            End If
            Return _reader.GetSegmentSequenceValue(Start, SegLength, WARN, directionDownstream)
        End Function

        ''' <summary>
        ''' <paramref name="St"></paramref> to End Join 1 to <paramref name="Sp"></paramref>
        ''' </summary>
        ''' <param name="St"></param>
        ''' <param name="Sp"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ReadJoinLocation(St As Integer, Sp As Integer) As String
            Dim Seq1 As String = TryParse(St, Me._innerNTsource.Length - St)
            Dim Seq2 As String = TryParse(1, Sp)
            Return Seq1 & Seq2
        End Function

        Public Function ReadJoinLocation(ParamArray JoinedLoci As Location()) As String
            Dim LQuery As String() = (From Loci As Location
                                      In JoinedLoci
                                      Select TryParse(Loci.Left, SegLength:=Loci.FragmentSize)).ToArray
            Return String.Join("", LQuery)
        End Function

        ''' <summary>
        ''' Try parsing the DNA sequence using a specific nucleotide loci value.(假若序列是在反向链之上，在反向链之上，则会自动进行互补反向)
        ''' </summary>
        ''' <param name="Location">位点的位置，由于一般情况之下右边的位置的值是大于左边的位置的值的</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function TryParse(Location As NucleotideLocation) As SegmentObject
            Dim Sequence As String = TryParse(Location.Normalization.Left,
                                              Location.FragmentSize,
                                              Location.Strand,
                                              Location.Ends)
            Return New SegmentObject(Sequence, Location)
        End Function

        Public Function TryParse(Left As Long,
                                 Length As Integer,
                                 Strand As Strands,
                                 Optional JoinEnds As Integer = 0,
                                 Optional WARN As Boolean = True) As String
            Dim Sequence As String = If(Left < 0,
                ReadJoinLocation(Me._innerNTsource.Length + Left, JoinEnds),
                TryParse(Left,
                         Length,
                         directionDownstream:=True,
                         WARN:=WARN))

            If Strand = Strands.Reverse Then  '在反向链之上，则会进行互补反向
                Sequence = New String(NucleicAcid.Complement(Sequence).Reverse.ToArray)
            End If

            Return Sequence
        End Function

        Public Function TryParse(Left As Long,
                                 Right As Long,
                                 Strand As Strands,
                                 Optional WARN As Boolean = True) As String
            Dim Sequence As String =
                TryParse(Left, Right - Left, directionDownstream:=True, WARN:=WARN)

            If Strand = Strands.Reverse Then  '在反向链之上，则会进行互补反向
                Sequence = New String(NucleicAcid.Complement(Sequence).Reverse.ToArray)
            End If

            Return Sequence
        End Function

        ''' <summary>
        ''' 首先按照正向链的方法取出序列片段，然后在进行互补反向得到反向链的序列片段数据，这个方法是专门用于读取反向序列的
        ''' </summary>
        ''' <param name="Start"></param>
        ''' <param name="Length"></param>
        ''' <param name="directionDownstream"></param>
        ''' <returns></returns>
        Public Function ReadComplement(Start As Integer, Length As Integer, Optional directionDownstream As Boolean = True) As String
            Dim TryPs = TryParse(Start, Length, directionDownstream)
            Dim Chunks = NucleicAcid.Complement(TryPs).Reverse.ToArray
            Return New String(Chunks)
        End Function

        Public Function ExtendFrameLeft(Interval As Integer) As SegmentReader
            Left -= Interval
            Return Me
        End Function

        Public Function ExtendFrameRight(Interval As Integer) As SegmentReader
            Right += Interval
            Return Me
        End Function

        ''' <summary>
        ''' 以当前的位置为参考向前移动一段<paramref name="internal"></paramref>距离
        ''' </summary>
        ''' <param name="internal"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function FrameMoveForward(internal As Integer) As SegmentReader
            Left += internal
            Right += internal
            Return Me
        End Function

        Public Function FrameMoveBackward(internal As Integer) As SegmentReader
            Left -= internal
            Right -= internal
            Return Me
        End Function

        Public Function MoveToHead() As SegmentReader
            Dim SegmentLength As Integer = Me.FragmentSize
            Left = 0
            Right = SegmentLength
            Return Me
        End Function

        Public Function MoveToTerminal() As SegmentReader
            Dim SegmentLength As Integer = Me.FragmentSize
            Left = _innerNTsource.Length - SegmentLength
            Right = _innerNTsource.Length
            Return Me
        End Function

        Public Overloads Shared Function CreateObject(Fasta As FASTA.FastaToken) As SegmentReader
            If Fasta Is Nothing Then
                Return Nothing
            End If
            Return New SegmentReader(Fasta)
        End Function
    End Class
End Namespace