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

Imports System.Xml.Serialization
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Assembly.NCBI.GenBank.GBFF.Keywords

    ''' <summary>
    ''' This GenBank keyword section stores the sequence data for this database.
    ''' </summary>
    ''' <remarks>
    ''' 序列数据可能以两种方式存在: 1. 已经加载到内存之中的字符串；2. 仍然保留在
    ''' 原始的gbff文件之中，通过<see cref="Provider"/>进行按需回读的低内存模式。
    ''' 
    ''' 因此在大型基因组(例如人基因组)的处理过程之中，请优先使用<see cref="Slice"/>
    ''' 以及<see cref="GetSequence"/>方法，而不是直接访问<see cref="SequenceData"/>属性，
    ''' 因为后者会导致整条染色体的序列数据被加载到内存之中。
    ''' </remarks>
    Public Class ORIGIN : Inherits KeyWord
        Implements IAbstractFastaToken
        Implements IEnumerable(Of Char)

        Private _sequence As String

        ''' <summary>
        ''' The sequence data that stores in this GenBank database, which can be a genomics DNA sequence, protein sequence or RNA sequence.
        ''' (序列数据，类型可以包括基因组DNA序列，蛋白质序列或者RNA序列)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' 在低内存模式之下访问这个属性会将整条序列数据加载到内存之中。
        ''' </remarks>
        <XmlIgnore> Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData
            Get
                If _sequence Is Nothing AndAlso Provider IsNot Nothing Then
                    _sequence = Provider.Materialize()
                End If

                Return If(_sequence, "")
            End Get
            Set(value As String)
                _sequence = If(value, "")
                Provider = New InMemorySequenceProvider(_sequence)
            End Set
        End Property

        ''' <summary>
        ''' 序列数据的提供者
        ''' </summary>
        <XmlIgnore> Friend Property Provider As IGenomeSequenceProvider

        ''' <summary>
        ''' 序列数据是否已经加载到内存之中
        ''' </summary>
        Public ReadOnly Property IsMaterialized As Boolean
            Get
                Return _sequence IsNot Nothing
            End Get
        End Property

        ''' <summary>
        ''' ``<see cref="SequenceData"/> -> index``
        ''' </summary>
        ''' <param name="index"></param>
        ''' <returns></returns>
        Default Public ReadOnly Property [Char](index As Long) As Char
            Get
                Return SequenceData(index)
            End Get
        End Property

        ''' <summary>
        ''' 基因组的大小
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Size As Integer
            Get
                If _sequence IsNot Nothing Then
                    Return _sequence.Length
                ElseIf Provider IsNot Nothing Then
                    Return CInt(Math.Min(Provider.Length, Integer.MaxValue))
                Else
                    Return 0
                End If
            End Get
        End Property

        ''' <summary>
        ''' 序列数据的切片，下标是从1开始的闭区间
        ''' </summary>
        Public Function Slice(left As Long, right As Long) As String
            If Provider IsNot Nothing Then
                Return Provider.Slice(left, right)
            Else
                Return NucleotideSlice.Slice(_sequence, left, right)
            End If
        End Function

        ''' <summary> 
        ''' 获取该Feature位点的序列数据
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 这个函数只会返回位点的连续区域(即所有片段的起始位置到结束位置)，
        ''' 对于具有多个外显子的feature，请使用<see cref="GetSequence"/>方法
        ''' </remarks>
        Public Function GetFeatureSegment(feature As Feature) As String
            Dim loci As NucleotideLocation = feature.Location.ContiguousRegion.Normalization

            Return Slice(loci.left, loci.right)
        End Function

        ''' <summary>
        ''' 提取feature所对应的完整序列: 多段位点会按照顺序拼接，并且会在需要的时候进行反向互补
        ''' </summary>
        Public Function GetSequence(feature As Feature) As String
            If Provider Is Nothing Then
                Return ""
            End If

            Return Provider.GetSequence(feature)
        End Function

        Public Overrides Function ToString() As String
            Return SequenceData
        End Function

        ''' <summary>
        ''' 是整条序列的GC偏移: ``(G - C) / (G + C)``
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' 旧版本的实现写成了<c>(G + C) / (G - C)</c>，在G与C的数量相等的时候
        ''' 会产生除零错误，这里修正成了标准的GC skew定义。
        ''' </remarks>
        Public ReadOnly Property GCSkew As Double
            Get
                Dim G As Long = 0
                Dim C As Long = 0
                Dim seq As String = SequenceData

                For i As Integer = 0 To seq.Length - 1
                    Select Case seq(i)
                        Case "G"c, "g"c
                            G += 1
                        Case "C"c, "c"c
                            C += 1
                    End Select
                Next

                If (G + C) = 0 Then
                    Return 0
                Else
                    Return (G - C) / CDbl(G + C)
                End If
            End Get
        End Property

        ''' <summary>
        ''' GC含量
        ''' </summary>
        Public ReadOnly Property GCContent As Double
            Get
                Dim G As Long = 0
                Dim C As Long = 0
                Dim total As Long = 0
                Dim seq As String = SequenceData

                For i As Integer = 0 To seq.Length - 1
                    Select Case seq(i)
                        Case "G"c, "g"c
                            G += 1
                            total += 1
                        Case "C"c, "c"c
                            C += 1
                            total += 1
                        Case "A"c, "a"c, "T"c, "t"c
                            total += 1
                    End Select
                Next

                If total = 0 Then
                    Return 0
                Else
                    Return (G + C) / CDbl(total)
                End If
            End Get
        End Property

        Public Shared Widening Operator CType(section As String()) As ORIGIN
            Dim origin As New ORIGIN With {
                .SequenceData = __letters(section)
            }

            Return origin
        End Operator

        ''' <summary>
        ''' 从``ORIGIN``段的文本行之中提取出碱基序列
        ''' </summary>
        Friend Shared Function __letters(section As String()) As String
            If section.IsNullOrEmpty Then
                Return ""
            End If

            Dim capacity As Integer = 0

            For Each line As String In section
                capacity += line.Length
            Next

            Dim buffer As Char() = New Char(capacity - 1) {}
            Dim offset As Integer = 0

            For Each line As String In section
                offset += NucleotideSlice.ExtractLetters(line, buffer, offset)
            Next

            If offset = buffer.Length Then
                Return New String(buffer)
            Else
                Return New String(buffer, 0, offset)
            End If
        End Function

        Public Shared Narrowing Operator CType(ori As ORIGIN) As String
            Return ori.SequenceData
        End Operator

        Public Shared Narrowing Operator CType(obj As ORIGIN) As FastaSeq
            Return obj.ToFasta
        End Operator

        ''' <summary>
        ''' Returns the whole genome sequence which was records in this GenBank database file. 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' (返回记录在本Genbank数据库文件之中的全基因组核酸序列)
        ''' </remarks>
        Public Function ToFasta() As FastaSeq
            Dim id As String = gb.Accession.AccessionId
            Dim size As Integer = Size
            ' ncbi sequence standard headers
            ' accession_id title
            Dim attrs As String() = {id & " " & title & " " & If(size < 1024, size & "bp", StringFormats.Lanudry(size))}
            Dim seq$ = SequenceData.ToUpper

            Return New FastaSeq(attrs, seq)
        End Function

        Public ReadOnly Property title As String Implements IAbstractFastaToken.title
            Get
                Return gb.Definition.Value
            End Get
        End Property

        Public Property Headers As String() Implements IAbstractFastaToken.headers

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
