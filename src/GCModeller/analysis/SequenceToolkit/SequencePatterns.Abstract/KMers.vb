#Region "Microsoft.VisualBasic::8a1591452def432b92e41510d24504bb, analysis\SequenceToolkit\SequencePatterns.Abstract\KMers.vb"

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

    '   Total Lines: 62
    '    Code Lines: 43 (69.35%)
    ' Comment Lines: 7 (11.29%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 12 (19.35%)
    '     File Size: 1.92 KB


    ' Class KMers
    ' 
    '     Properties: Count, Size, Tag, Unique
    ' 
    '     Function: (+2 Overloads) Create, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
''' k-mers
''' </summary>
Public Class KMers

    Public Property Tag As String
    ''' <summary>
    ''' count current k-mers <see cref="Tag"/> on a given sequence
    ''' </summary>
    ''' <returns></returns>
    Public Property Count As Integer

    Public ReadOnly Property Unique As Boolean
        Get
            Return Count = 1
        End Get
    End Property

    Public ReadOnly Property Size As Integer
        Get
            Return Tag.Length
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return Tag
    End Function

    Public Shared Iterator Function Create(seq As ISequenceProvider,
                                           Optional k As Integer = 3,
                                           Optional charSet As IReadOnlyCollection(Of Char) = Nothing) As IEnumerable(Of KMers)

        Dim seq_s As String = seq.GetSequenceData

        If charSet Is Nothing OrElse charSet.Count = 0 Then
            charSet = seq_s.Distinct.ToArray
        End If

        For Each kmer As KMers In Create(k, charSet)
            kmer.Count = seq_s.Count(kmer.Tag)
            Yield kmer
        Next
    End Function

    Public Shared Iterator Function Create(k As Integer, charSet As IReadOnlyCollection(Of Char)) As IEnumerable(Of KMers)
        Dim combines As String() = charSet.Select(Function(ci) ci.ToString).ToArray

        For i As Integer = 1 To k
            combines = CombinationExtensions _
                .CreateCombos(combines, charSet) _
                .Select(Function(t) t.a & t.b.ToString) _
                .ToArray
        Next

        For Each tag As String In combines
            Yield New KMers With {.Tag = tag}
        Next
    End Function
End Class
