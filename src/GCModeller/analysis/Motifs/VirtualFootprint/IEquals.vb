#Region "Microsoft.VisualBasic::27b1f525bca2bc84a709b62881bf8e5c, analysis\Motifs\SharedDataModels\IEquals.vb"

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

    ' Module IEqualsAPI
    ' 
    '     Function: __equals, __equalsStrict, Equals, Intersection
    '     Structure __motifEquals
    ' 
    '         Function: Equals
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming.Levenshtein
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports SMRUCC.genomics.ComponentModel.Loci

Public Module IEqualsAPI

    <Extension>
    Public Function Equals(Of T As DocumentFormat.RegulatesFootprints)(a As T, b As T, Optional strict As Boolean = True) As Boolean
        If strict Then
            Return __equalsStrict(Of T)(a, b)
        Else
            Return __equals(Of T)(a, b)
        End If
    End Function

    ''' <summary>
    ''' 在基础属性的比较之上再添加对序列的比较
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="a"></param>
    ''' <param name="b"></param>
    ''' <returns></returns>
    Private Function __equalsStrict(Of T As DocumentFormat.RegulatesFootprints)(a As T, b As T) As Boolean
        If Not __equals(a, b) Then
            Return False
        End If
        Dim lev = LevenshteinDistance.ComputeDistance(a.Sequence, b.Sequence)
        If lev Is Nothing Then
            Return False
        Else
            Return lev.MatchSimilarity >= 0.85
        End If
    End Function

    ''' <summary>
    ''' 基础属性的比较
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="a"></param>
    ''' <param name="b"></param>
    ''' <returns></returns>
    Private Function __equals(Of T As DocumentFormat.RegulatesFootprints)(a As T, b As T) As Boolean
        If Not String.Equals(a.ORF, b.ORF, StringComparison.OrdinalIgnoreCase) Then
            Return False
        End If
        If Not String.Equals(a.Regulator, b.Regulator, StringComparison.OrdinalIgnoreCase) Then
            Return False
        End If
        Dim la As New NucleotideLocation(a)
        Dim lb As New NucleotideLocation(b)
        Return la.Equals(lb, AllowedOffset:=CInt(a.Length / 2))
    End Function

    ''' <summary>
    ''' 请注意，这个函数总是会挑选出交集之中的<paramref name="s1"/>第一个集合之中的元素的
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="s1"></param>
    ''' <param name="s2"></param>
    ''' <returns></returns>
    <Extension>
    Public Function Intersection(Of T As DocumentFormat.RegulatesFootprints)(s1 As IEnumerable(Of T), s2 As IEnumerable(Of T), Optional strict As Boolean = False) As T()
        'Dim comparer As New __motifEquals(Of T) With {
        '    .strict = strict
        '}
        If strict Then
            Return s1.Intersection(s2, getUID:=AddressOf DocumentFormat.RegulatesFootprints.TraceUidStrict)
        Else
            Return s1.Intersection(s2, getUID:=AddressOf DocumentFormat.RegulatesFootprints.TraceUid)
        End If
    End Function

    Private Structure __motifEquals(Of T As DocumentFormat.RegulatesFootprints)
        Dim strict As Boolean

        Public Overloads Function Equals(a As T, b As T) As Boolean
            Return IEqualsAPI.Equals(a, b, strict)
        End Function
    End Structure
End Module
