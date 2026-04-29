
Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace MSA.Tabular

    Public Module Reader

        Public Iterator Function Read(filepath As String) As IEnumerable(Of Stockholm)
            Using file As Stream = filepath.OpenReadonly
                For Each motif As Stockholm In file.Read
                    Yield motif
                Next
            End Using
        End Function

        <Extension>
        Public Iterator Function Read(s As Stream) As IEnumerable(Of Stockholm)
            For Each block As String() In s.ReadAllLines.Split(delimiter:="//")
                Yield block.Parser
            Next
        End Function

        <Extension>
        Public Function Parser(lines As String()) As Stockholm
            Dim comments As New List(Of String)
            Dim seq_cons As String = Nothing
            Dim msa_seqs As New List(Of NamedValue(Of String))
            Dim source_names As New Dictionary(Of String, String)
            Dim metadata As New Dictionary(Of String, List(Of String))

            ' first line is: # STOCKHOLM 1.0
            For Each line As String In lines.Skip(1)
                If line.StartsWith("#") Then
                    Dim tuple As NamedValue(Of String) = line.GetTagValue(" ")

                    Select Case tuple.Name
                        Case "GF"
                            tuple = tuple.Value.GetTagValue(" ", trim:=True)

                            If tuple.Name = "CC" Then
                                Call comments.Add(tuple.Value)
                            Else
                                If Not metadata.ContainsKey(tuple.Name) Then
                                    Call metadata.Add(tuple.Name, New List(Of String))
                                End If

                                Call metadata(tuple.Name).Add(tuple.Value)
                            End If
                        Case "GS"
                            Dim cols As String() = tuple.Value.StringSplit("\s+")
                            Dim name As String = cols(0)
                            Dim acc As String = cols.Last

                            source_names(name) = acc
                        Case "GC"
                            tuple = tuple.Value.GetTagValue(" ", trim:=True)

                            If tuple.Name = "seq_cons" Then
                                seq_cons = tuple.Value
                            End If
                        Case Else
                            Throw New NotImplementedException(line)
                    End Select
                Else
                    ' msa data
                    Call msa_seqs.Add(line.GetTagValue(" ", trim:=True))
                End If
            Next

            Return New Stockholm With {
                .comment = comments.JoinBy(" "),
                .seq_cons = seq_cons,
                .msa = New MSAOutput With {
                    .names = msa_seqs.Keys.ToArray,
                    .MSA = msa_seqs.Values
                },
                .metadata = metadata _
                    .ToDictionary(Function(a) a.Key,
                                  Function(a)
                                      Return a.Value.JoinBy(" ")
                                  End Function),
                .seq_source = source_names
            }
        End Function
    End Module
End Namespace