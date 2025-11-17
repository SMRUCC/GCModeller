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

Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
''' k-mers
''' </summary>
Public Class KMers

    ''' <summary>
    ''' the kmer sequence
    ''' </summary>
    ''' <returns></returns>
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

    Public Shared Iterator Function Create(seq As ISequenceProvider, Optional k As Integer = 3) As IEnumerable(Of KMers)
        For Each group As IGrouping(Of String, KSeq) In KSeq.Kmers(seq, k).GroupBy(Function(a) a.Seq)
            Yield New KMers With {
                .Tag = group.Key,
                .Count = group.Count
            }
        Next
    End Function
End Class
