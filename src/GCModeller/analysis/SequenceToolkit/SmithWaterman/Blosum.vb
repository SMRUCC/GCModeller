#Region "Microsoft.VisualBasic::deb2017e8ce81fa99b39a7b3363f3ed4, ..\GCModeller\analysis\SequenceToolkit\SmithWaterman\Blosum.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Linq
Imports Microsoft.VisualBasic.Linq.Extensions

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

    ''' <summary>
    ''' Default Blosum-62 substitution matrix from inner resource
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Matrix As Integer()()

    ''' <summary>
    ''' Load Blosum matrix from the text file, and this Blosum matrix file which is available downloads from NCBI FTP site.
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns></returns>
    Public Shared Function LoadMatrix(path As String) As Blosum
        Return LoadFromStream(FileIO.FileSystem.ReadAllText(path))
    End Function

    ''' <summary>
    ''' Load Blosum matrix from the text file, and this Blosum matrix file which is available downloads from NCBI FTP site.
    ''' </summary>
    ''' <param name="doc"></param>
    ''' <returns></returns>
    Public Shared Function LoadFromStream(doc As String) As Blosum
        Dim tokens As String() = doc.lTokens
        Dim i As Integer

        Do While tokens.Read(i).First = "#"c
        Loop

        tokens = tokens.Skip(i).ToArray

        Dim MAT As Integer()() = (From line As String In tokens
                                  Where Not String.IsNullOrWhiteSpace(line)
                                  Select __toVector(line)).ToArray
        Return New Blosum() With {
            ._Matrix = MAT
        }
    End Function

    Private Shared Function __toVector(line As String) As Integer()
        Dim array As Integer() = (From x As String
                                  In line.Split.Skip(1)
                                  Where Not String.IsNullOrWhiteSpace(x)
                                  Select CInt(Val(x))).ToArray
        Return array
    End Function

    '''<summary>
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
        SyncLock My.Resources.BLOSUM62
            Return LoadFromStream(My.Resources.BLOSUM62)
        End SyncLock
    End Function

    ' quick and dirty equivalent of typesafe enum pattern, can also use HashMap
    ' or even better, EnumMap in Java 5. 
    ' This code is for Java 1.4.2, so we will stick to the simple implementation
    Private Function getIndex(a As Char) As Integer
        ' check for upper and lowercase characters
        Select Case Char.ToUpper(a)
            Case "A"c
                Return 0
            Case "R"c
                Return 1
            Case "N"c
                Return 2
            Case "D"c
                Return 3
            Case "C"c
                Return 4
            Case "Q"c
                Return 5
            Case "E"c
                Return 6
            Case "G"c
                Return 7
            Case "H"c
                Return 8
            Case "I"c
                Return 9
            Case "L"c
                Return 10
            Case "K"c
                Return 11
            Case "M"c
                Return 12
            Case "F"c
                Return 13
            Case "P"c
                Return 14
            Case "S"c
                Return 15
            Case "T"c
                Return 16
            Case "W"c
                Return 17
            Case "Y"c
                Return 18
            Case "V"c
                Return 19
            Case Else
                Throw New Exception($"Invalid amino acid character!  --> ""{a}""")
        End Select
    End Function

    Private Function getDistance(i As Integer, j As Integer) As Integer
        If i < 0 OrElse i > Matrix(0).Length Then
            Throw New Exception("Invalid amino acid character at string1, position " & i)
        End If
        If j < 0 OrElse j > Matrix(0).Length Then
            Throw New Exception("Invalid amino acid character at string2, position " & j)
        End If

        Return Matrix(i)(j)
    End Function

    ''' <summary>
    ''' 函数对字符的大小写不敏感
    ''' </summary>
    ''' <param name="a1"></param>
    ''' <param name="a2"></param>
    ''' <returns></returns>
    Public Function getDistance(a1 As Char, a2 As Char) As Integer
        ' toUpper
        Return getDistance(getIndex(a1), getIndex(a2))
    End Function
End Class
