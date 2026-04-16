#Region "Microsoft.VisualBasic::764255166df4a894cd08cc3e24fac97f, analysis\Metagenome\Metagenome\Kmers\SequenceCollection.vb"

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

    '   Total Lines: 93
    '    Code Lines: 62 (66.67%)
    ' Comment Lines: 14 (15.05%)
    '    - Xml Docs: 71.43%
    ' 
    '   Blank Lines: 17 (18.28%)
    '     File Size: 3.42 KB


    '     Class SequenceCollection
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: AddSequenceID, HasSequence, Load
    ' 
    '         Sub: SaveTo
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Framework

Namespace Kmers

    Public Class SequenceCollection

        ReadOnly seqs As New Dictionary(Of String, SequenceSource)
        ReadOnly index As New List(Of String)

        ''' <summary>
        ''' get sequence source information by its ncbi sequence accession id
        ''' </summary>
        ''' <param name="accession_id"></param>
        ''' <returns></returns>
        Default Public ReadOnly Property GetSource(accession_id As String) As SequenceSource
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return seqs.TryGetValue(accession_id)
            End Get
        End Property

        ''' <summary>
        ''' get sequence source information by its sequence id
        ''' </summary>
        ''' <param name="seq_id"></param>
        ''' <returns></returns>
        Default Public ReadOnly Property GetSource(seq_id As UInteger) As SequenceSource
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                ' sequence id is 1-based
                ' translate to zero-based index at here
                seq_id = seq_id - 1

                ' 20251205 some sequence information has not been saved when
                ' the kmer database builder program is crashed
                If seq_id >= index.Count Then
                    Return Nothing
                End If

                Return seqs(index(seq_id))
            End Get
        End Property

        Sub New(load As IEnumerable(Of SequenceSource))
            For Each seq As SequenceSource In load.OrderBy(Function(a) a.id)
                Call seqs.Add(seq.accession_id, seq)
                Call index.Add(seq.accession_id)
            Next
        End Sub

        Public Function HasSequence(seq_id As UInteger) As Boolean
            If seq_id < 0 OrElse seq_id >= index.Count Then
                Return False
            Else
                Return True
            End If
        End Function

        Public Function AddSequenceID(taxid As UInteger, name As String) As UInteger
            Dim genbank_info As NamedValue(Of String) = name.GetTagValue(" ", trim:=True, failureNoName:=False)

            If seqs.ContainsKey(genbank_info.Name) Then
                Return seqs(genbank_info.Name).id
            End If

            Dim id As UInteger = seqs.Count + 1
            Dim seq As New SequenceSource With {
                .id = id,
                .name = If(genbank_info.Value.StringEmpty, "no_name", genbank_info.Value),
                .ncbi_taxid = taxid,
                .accession_id = genbank_info.Name
            }

            Call index.Add(seq.accession_id)
            Call seqs.Add(seq.accession_id, seq)

            Return id
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function Load(file As String) As SequenceCollection
            Return New SequenceCollection(file.LoadCsv(Of SequenceSource)(mute:=True))
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub SaveTo(file As String)
            Call seqs.Values.SaveTo(file, silent:=True)
        End Sub

    End Class
End Namespace
