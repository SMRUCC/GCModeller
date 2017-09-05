#Region "Microsoft.VisualBasic::714e32c94e0f6e9e0ca6669240a258a8, ..\repository\nt\Extensions.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.IO.SearchEngine
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module Extensions

    <Extension>
    Public Iterator Function FastaSearch(source As IEnumerable(Of FastaToken), query$) As IEnumerable(Of FastaToken)
        Dim expression As Expression = query.Build
        Dim type As New IObject(GetType(Text))

        For Each x As FastaToken In source
            If expression.Evaluate(
                type,
                New Text With {
                    .Text = x.Title
                }) Then

                Yield x
            End If
        Next
    End Function

    ''' <summary>
    ''' Open file handle failure, perhaps there are duplicated name in your query data and this may cause error on Windows file system!
    ''' </summary>
    Const DuplicatedName$ = "Open file handle failure, perhaps there are duplicated name in your query data and this may cause error on Windows file system!"

    <Extension>
    Public Function BatchSearch(source As IEnumerable(Of FastaToken), arguments As IEnumerable(Of NamedValue(Of String)), out$) As Boolean
        Dim expressions As New Dictionary(Of Expression, StreamWriter)
        Dim def As New IObject(GetType(Text))

        Try
            For Each query In arguments
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
                    If query.Key.Evaluate(def, title) Then
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

