#Region "Microsoft.VisualBasic::1f020f47a5e50d5ac711f8a646a7dc07, analysis\SequenceToolkit\SequenceAlignment\MSA\TabularMSA\Reader.vb"

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

    '   Total Lines: 91
    '    Code Lines: 75 (82.42%)
    ' Comment Lines: 3 (3.30%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 13 (14.29%)
    '     File Size: 3.67 KB


    '     Module Reader
    ' 
    '         Function: Parser, (+2 Overloads) Read
    ' 
    ' 
    ' /********************************************************************************/

#End Region


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
                        Case "#=GF"
                            tuple = tuple.Value.GetTagValue(" ", trim:=True)

                            If tuple.Name = "CC" Then
                                Call comments.Add(tuple.Value)
                            Else
                                If Not metadata.ContainsKey(tuple.Name) Then
                                    Call metadata.Add(tuple.Name, New List(Of String))
                                End If

                                Call metadata(tuple.Name).Add(tuple.Value)
                            End If
                        Case "#=GS"
                            Dim cols As String() = tuple.Value.StringSplit("\s+")
                            Dim name As String = cols(0)
                            Dim acc As String = cols.Last

                            source_names(name) = acc
                        Case "#=GC"
                            tuple = tuple.Value.GetTagValue(" ", trim:=True)

                            If tuple.Name = "seq_cons" Then
                                seq_cons = tuple.Value
                            End If
                        Case "#=GR"
                            ' do nothing, skip
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
