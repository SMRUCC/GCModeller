#Region "Microsoft.VisualBasic::419cad5a6b1c54e91d1707a156482fb0, analysis\SequenceToolkit\SequenceAlignment\SmithWaterman\Matrix\Blosum.vb"

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

    '   Total Lines: 132
    '    Code Lines: 55 (41.67%)
    ' Comment Lines: 64 (48.48%)
    '    - Xml Docs: 78.12%
    ' 
    '   Blank Lines: 13 (9.85%)
    '     File Size: 5.76 KB


    '     Class Blosum
    ' 
    '         Properties: DNAMatrix, keys, matrix
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: distanceByIndex, FromInnerBlosum62, GetDistance, getIndex
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace BestLocalAlignment

    ''' <summary>
    ''' Blosum-62 substitution matrix
    ''' #   A  R  N  D  C  Q  E  G  H  I  L  K  M  F  P  S  T  W  Y  V 
    ''' A  4 -1 -2 -2  0 -1 -1  0 -2 -1 -1 -1 -1 -2 -1  1  0 -3 -2  0 
    ''' R -1  5  0 -2 -3  1  0 -2  0 -3 -2  2 -1 -3 -2 -1 -1 -3 -2 -3 
    ''' N -2  0  6  1 -3  0  0  0  1 -3 -3  0 -2 -3 -2  1  0 -4 -2 -3 
    ''' D -2 -2  1  6 -3  0  2 -1 -1 -3 -4 -1 -3 -3 -1  0 -1 -4 -3 -3 
    ''' C  0 -3 -3 -3  9 -3 -4 -3 -3 -1 -1 -3 -1 -2 -3 -1 -1 -2 -2 -1 
    ''' Q -1  1  0  0 -3  5  2 -2  0 -3 -2  1  0 -3 -1  0 -1 -2 -1 -2 
    ''' E -1  0  0  2 -4  2  5 -2  0 -3 -3  1 -2 -3 -1  0 -1 -3 -2 -2 
    ''' G  0 -2  0 -1 -3 -2 -2  6 -2 -4 -4 -2 -3 -3 -2  0 -2 -2 -3 -3 
    ''' H -2  0  1 -1 -3  0  0 -2  8 -3 -3 -1 -2 -1 -2 -1 -2 -2  2 -3 
    ''' I -1 -3 -3 -3 -1 -3 -3 -4 -3  4  2 -3  1  0 -3 -2 -1 -3 -1  3 
    ''' L -1 -2 -3 -4 -1 -2 -3 -4 -3  2  4 -2  2  0 -3 -2 -1 -2 -1  1 
    ''' K -1  2  0 -1 -3  1  1 -2 -1 -3 -2  5 -1 -3 -1  0 -1 -3 -2 -2 
    ''' M -1 -1 -2 -3 -1  0 -2 -3 -2  1  2 -1  5  0 -2 -1 -1 -1 -1  1 
    ''' F -2 -3 -3 -3 -2 -3 -3 -3 -1  0  0 -3  0  6 -4 -2 -2  1  3 -1 
    ''' P -1 -2 -2 -1 -3 -1 -1 -2 -2 -3 -3 -1 -2 -4  7 -1 -1 -4 -3 -2 
    ''' S  1 -1  1  0 -1  0  0  0 -1 -2 -2  0 -1 -2 -1  4  1 -3 -2 -2 
    ''' T  0 -1  0 -1 -1 -1 -1 -2 -2 -1 -1 -1 -1 -2 -1  1  5 -2 -2  0 
    ''' W -3 -3 -4 -4 -2 -2 -3 -2 -2 -3 -2 -3 -1  1 -4 -3 -2 11  2 -3 
    ''' Y -2 -2 -2 -3 -2 -1 -2 -3  2 -1 -1 -2 -1  3 -3 -2 -2  2  7 -1 
    ''' V  0 -3 -3 -3 -1 -2 -2 -3 -3  3  1 -2  1 -1 -2 -2  0 -3 -1  4 
    ''' </summary>
    Public Class Blosum

        Private _matrix As Integer()()

        ''' <summary>
        ''' The Blosum substitution matrix
        ''' </summary>
        ''' <returns></returns>
        Public Property matrix As Integer()()
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return _matrix
            End Get
            Set(value As Integer()())
                _matrix = value
                ' 矩阵被替换后,已编译的快速查表失效
                _fastTable = Nothing
            End Set
        End Property

        ''' <summary>
        ''' matrix row/col char index, example as ATGC for nucleotide sequence
        ''' </summary>
        ReadOnly index As Index(Of String)

        ''' <summary>
        ''' 字符对 -> 得分 的扁平快速查表(256 × 256),惰性构建。
        ''' 该表把 "字符 -> 矩阵下标 -> 得分" 的两级查找压缩为一次内存访问,
        ''' 是无空位延伸与 SW 矩阵构建的最内层热路径。
        ''' </summary>
        Private _fastTable As Integer()

        ''' <summary>快速查表中"该字符对无有效映射"的哨兵值。</summary>
        Private Const INVALID As Integer = Integer.MinValue

        Public ReadOnly Property keys As String()
            Get
                Return index.Objects
            End Get
        End Property

        ''' <summary>
        ''' 一个简单的DNA替换矩阵，用于Smith-Waterman比对。
        ''' 匹配得分为+1，错配得分为-1。
        ''' </summary>
        Public Shared ReadOnly Property DNAMatrix As Blosum
            Get
                Return New Blosum(New String() {"A", "T", "G", "C"}) With {
                    .matrix = {
                        New Integer() {1, -1, -1, -1}, ' A vs A,T,G,C
                        New Integer() {-1, 1, -1, -1}, ' T vs A,T,G,C
                        New Integer() {-1, -1, 1, -1}, ' G vs A,T,G,C
                        New Integer() {-1, -1, -1, 1}  ' C vs A,T,G,C
                    }
                }
            End Get
        End Property

        Sub New(base As String())
            index = base
        End Sub

        Protected Overridable Function getIndex(a As Char) As Integer
            ' check for upper and lowercase characters
            Dim i As Integer = index.IndexOf(Char.ToUpper(a))

            If i = -1 Then
                Throw New InvalidDataException($"Invalid nucleotide base or amino acid character! -> ""{a}""")
            Else
                Return i
            End If
        End Function

        ''' <summary>
        ''' Get distance by given two index of matrix
        ''' </summary>
        ''' <param name="i%"></param>
        ''' <param name="j%"></param>
        ''' <returns></returns>
        Private Function distanceByIndex(i%, j%) As Integer
            If i < 0 OrElse i > matrix(0).Length Then
                Throw New Exception("Invalid amino acid character at string1, position " & i)
            End If
            If j < 0 OrElse j > matrix(0).Length Then
                Throw New Exception("Invalid amino acid character at string2, position " & j)
            End If

            Return matrix(i)(j)
        End Function

        ''' <summary>
        ''' 函数对字符的大小写不敏感
        ''' </summary>
        ''' <param name="a1"></param>
        ''' <param name="a2"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetDistance(a1 As Char, a2 As Char) As Integer
            Dim table As Integer() = _fastTable

            If table Is Nothing Then
                table = BuildFastTable()
            End If

            Dim i1 As Integer = AscW(a1)
            Dim i2 As Integer = AscW(a2)

            If i1 >= 0 AndAlso i1 < 256 AndAlso i2 >= 0 AndAlso i2 < 256 Then
                Dim score As Integer = table((i1 << 8) Or i2)

                If score <> INVALID Then
                    Return score
                End If
            End If

            ' 未映射字符:回退到原逻辑(保留原有的异常语义)
            Return distanceByIndex(getIndex(a1), getIndex(a2))
        End Function

        ''' <summary>
        ''' 构建 256 × 256 的字符对得分查表。
        ''' 对无法映射的字符写入 <see cref="INVALID"/> 哨兵,由 <see cref="GetDistance"/> 回退处理。
        ''' </summary>
        <MethodImpl(MethodImplOptions.NoInlining)>
        Private Function BuildFastTable() As Integer()
            Dim table(65535) As Integer
            Dim mapped(255) As Integer

            For c As Integer = 0 To 255
                Try
                    mapped(c) = getIndex(ChrW(c))
                Catch ex As Exception
                    mapped(c) = -1
                End Try
            Next

            For a As Integer = 0 To 255
                Dim ia As Integer = mapped(a)

                For b As Integer = 0 To 255
                    Dim ib As Integer = mapped(b)

                    If ia < 0 OrElse ib < 0 OrElse _matrix Is Nothing Then
                        table((a << 8) Or b) = INVALID
                    ElseIf ia >= _matrix.Length OrElse ib >= _matrix(ia).Length Then
                        table((a << 8) Or b) = INVALID
                    Else
                        table((a << 8) Or b) = _matrix(ia)(ib)
                    End If
                Next
            Next

            _fastTable = table
            Return table
        End Function

        '''<summary>
        ''' Default Blosum-62 substitution matrix from inner resource
        '''
        '''  Looks up a localized string similar to #  Matrix made by matblas from blosum62.iij
        '''#  * column uses minimum score
        '''#  BLOSUM Clustered Scoring Matrix in 1/2 Bit Units
        '''#  Blocks Database = /data/blocks_5.0/blocks.dat
        '''#  Cluster Percentage: &gt;= 62
        '''#  Entropy =   0.6979, Expected =  -0.5209
        '''   A  R  N  D  C  Q  E  G  H  I  L  K  M  F  P  S  T  W  Y  V  B  Z  X  *
        '''A  4 -1 -2 -2  0 -1 -1  0 -2 -1 -1 -1 -1 -2 -1  1  0 -3 -2  0 -2 -1  0 -4 
        '''R -1  5  0 -2 -3  1  0 -2  0 -3 -2  2 -1 -3 -2 -1 -1 -3 -2 -3 -1  0 -1 -4 
        '''N -2  0  6  1 -3  0  0  0  1 -3 -3  0 -2 [rest of string was truncated]&quot;;.
        '''</summary>
        Public Shared Function FromInnerBlosum62() As Blosum
            SyncLock My.Resources.BLOSUM.BLOSUM62
                Return BlosumParser.LoadFromStream(My.Resources.BLOSUM.BLOSUM62)
            End SyncLock
        End Function
    End Class
End Namespace
