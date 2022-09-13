#Region "Microsoft.VisualBasic::321c161cca0a079d86fc2f1f5ee4483c, GCModeller\analysis\SequenceToolkit\SmithWaterman\Matrix\Blosum.vb"

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

    '   Total Lines: 108
    '    Code Lines: 40
    ' Comment Lines: 57
    '   Blank Lines: 11
    '     File Size: 4.41 KB


    ' Class Blosum
    ' 
    '     Properties: keys, matrix
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: distanceByIndex, FromInnerBlosum62, GetDistance, getIndex
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection

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
    ''' The Blosum substitution matrix
    ''' </summary>
    ''' <returns></returns>
    Public Property matrix As Integer()()

    Public ReadOnly Property keys As String()
        Get
            Return index.Objects
        End Get
    End Property

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
        SyncLock My.Resources.BLOSUM62
            Return BlosumParser.LoadFromStream(My.Resources.BLOSUM62)
        End SyncLock
    End Function

    Sub New(base As String())
        index = base
    End Sub

    ReadOnly index As Index(Of String)

    Protected Overridable Function getIndex(a As Char) As Integer
        ' check for upper and lowercase characters
        Dim i As Integer = index.IndexOf(Char.ToUpper(a))

        If i = -1 Then
            Throw New Exception($"Invalid amino acid character!  --> ""{a}""")
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
        ' toUpper
        Return distanceByIndex(getIndex(a1), getIndex(a2))
    End Function
End Class
