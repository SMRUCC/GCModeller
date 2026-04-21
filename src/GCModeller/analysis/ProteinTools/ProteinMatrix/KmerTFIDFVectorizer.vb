#Region "Microsoft.VisualBasic::4cfd1aee0e05f1268564f332042e7c66, analysis\ProteinTools\ProteinMatrix\KmerTFIDFVectorizer.vb"

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

'   Total Lines: 47
'    Code Lines: 32 (68.09%)
' Comment Lines: 5 (10.64%)
'    - Xml Docs: 80.00%
' 
'   Blank Lines: 10 (21.28%)
'     File Size: 1.35 KB


' Class KmerTFIDFVectorizer
' 
'     Constructor: (+1 Overloads) Sub New
' 
'     Function: OneHotVectorizer, TfidfVectorizer
' 
'     Sub: Add, AddRange
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.NLP
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.Slicer

Public Class KmerTFIDFVectorizer

    ReadOnly vec As New TFIDF
    ReadOnly k As Integer
    ReadOnly type As SeqTypes

    Sub New(Optional type As SeqTypes = SeqTypes.Protein, Optional k As Integer = 6)
        Me.k = k
        Me.type = type
    End Sub

    Public Sub Add(seq As FastaSeq)
        Dim chars As String = seq.SequenceData

        If type <> SeqTypes.Protein Then
            chars = NucleicAcid.Canonical(chars)
        End If

        Call vec.Add(seq.Title, KSeq.KmerSpans(chars, k))
    End Sub

    Public Sub AddRange(seqs As IEnumerable(Of FastaSeq))
        For Each seq As FastaSeq In seqs
            Call Add(seq)
        Next
    End Sub

    Public Function TfidfVectorizer(Optional normalize As Boolean = False) As DataFrame
        ' Call vec.SetWords(kmers)
        Return vec.TfidfVectorizer(normalize)
    End Function

    ''' <summary>
    ''' n-gram One-hot(Bag-of-n-grams)
    ''' </summary>
    ''' <returns></returns>
    Public Function OneHotVectorizer() As DataFrame
        Return vec.OneHotVectorizer
    End Function

End Class

