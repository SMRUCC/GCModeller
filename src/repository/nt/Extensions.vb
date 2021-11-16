#Region "Microsoft.VisualBasic::eb2f4a7cde2ad40ba414fdea8c798e04, nt\Extensions.vb"

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

    ' Module Extensions
    ' 
    '     Function: BatchSearch, FastaSearch
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.IO.SearchEngine
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module Extensions

    <Extension>
    Public Iterator Function FastaSearch(source As IEnumerable(Of FastaSeq), query$) As IEnumerable(Of FastaSeq)
        Dim expression As Expression = query.Build
        Dim type As New IObject(GetType(Text))

        For Each x As FastaSeq In source
            If expression.Evaluate(
                type,
                New Text With {
                    .Text = x.Title
                }).Success Then

                Yield x
            End If
        Next
    End Function

    ''' <summary>
    ''' Open file handle failure, perhaps there are duplicated name in your query data and this may cause error on Windows file system!
    ''' </summary>
    Const DuplicatedName$ = "Open file handle failure, perhaps there are duplicated name in your query data and this may cause error on Windows file system!"

    <Extension>
    Public Function BatchSearch(source As IEnumerable(Of FastaSeq), arguments As IEnumerable(Of NamedValue(Of String)), out$) As Boolean
        Dim expressions As New Dictionary(Of Expression, StreamWriter)
        Dim def As New IObject(GetType(Text))

        Try
            For Each query As NamedValue(Of String) In arguments
                Dim path$ = out & $"/{query.Name.NormalizePathString}.fasta"
                Call expressions.Add(query.Value.Build,
                                     path.OpenWriter(Encodings.ASCII))
            Next
        Catch ex As Exception
            ex = New Exception(DuplicatedName, ex)
            Throw ex
        End Try

        Call Parallel.ForEach(
            source,
            Sub(fa)
                Dim title As New Text With {
                    .Text = fa.Title
                }

                For Each query In expressions
                    If query.Key.Evaluate(def, title).Success Then
                        SyncLock query.Value
                            Call query.Value.WriteLine(fa.GenerateDocument(120))
                        End SyncLock
                    End If
                Next
            End Sub)

        For Each file In expressions.Values
            Call file.Flush()
            Call file.Close()
            Call file.Dispose()
        Next

        Return True
    End Function
End Module
