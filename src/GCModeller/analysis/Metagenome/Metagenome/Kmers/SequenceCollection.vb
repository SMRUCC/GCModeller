Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Linq

Namespace Kmers

    Public Class SequenceCollection

        ReadOnly seqs As New Dictionary(Of String, SequenceSource)
        ReadOnly index As New List(Of String)

        Default Public ReadOnly Property GetSource(accession_id As String) As SequenceSource
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return seqs.TryGetValue(accession_id)
            End Get
        End Property

        Default Public ReadOnly Property GetSource(seq_id As UInteger) As SequenceSource
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return seqs(index(seq_id))
            End Get
        End Property

        Sub New(load As IEnumerable(Of SequenceSource))
            seqs = load.SafeQuery _
                .GroupBy(Function(a) a.accession_id) _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return a.First
                              End Function)
            index = New List(Of String)(seqs.Keys)
        End Sub

        Public Function AddSequenceID(taxid As UInteger, name As String) As UInteger
            Dim id As UInteger = seqs.Count + 1
            Dim genbank_info As NamedValue(Of String) = name.GetTagValue(" ", trim:=True, failureNoName:=False)

            If seqs.ContainsKey(genbank_info.Name) Then
                Return seqs(genbank_info.Name).id
            End If

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