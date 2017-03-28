#Region "Microsoft.VisualBasic::6567d06820b3eff9caf1cc75458163f4, ..\core\Bio.Assembly\Assembly\NCBI\Taxonomy\Accession2Taxid.vb"

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
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Text

Namespace Assembly.NCBI.Taxonomy

    Public Module Accession2Taxid

        Public Function LoadAll(DIR$) As BucketDictionary(Of String, Integer)
            Return DIR.__loadData _
                .CreateBuckets(Function(x) x.Name,
                               Function(x) x.Value)
        End Function

        ''' <summary>
        ''' 在返回的数据之中，属性<see cref="NamedValue(Of Integer).Description"/>是原始的行数据，
        ''' Name属性不包含有Accession的版本号
        ''' </summary>
        ''' <param name="file$"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function ReadFile(file$) As IEnumerable(Of NamedValue(Of Integer))
            Dim line$
            Dim tokens$()

            Using reader As StreamReader = file.OpenReader
                Call reader.ReadLine() ' skip first line, headers

                Do While Not reader.EndOfStream
                    line = reader.ReadLine
                    tokens = line.Split(ASCII.TAB)

                    ' accession       accession.version       taxid   gi
                    Yield New NamedValue(Of Integer) With {
                        .Name = tokens(Scan0),
                        .Value = CInt(Val(tokens(2))),
                        .Description = line
                    }
                Loop
            End Using
        End Function

        <Extension>
        Private Iterator Function __loadData(DIR$, Optional gb_priority? As Boolean = False) As IEnumerable(Of NamedValue(Of Integer))
            Dim files$() = (ls - l - r - "*.*" <= DIR).ToArray

            If gb_priority Then
                For i As Integer = 0 To files.Length - 1
                    If files(i).BaseName.TextEquals("nucl_gb") Then ' 优先加载gb库，提升匹配查找函数的效率
                        Call files.Swap(i, Scan0)
                        Exit For
                    End If
                Next
            End If

            For Each file$ In files
                Call file.ToFileURL.__DEBUG_ECHO

                For Each x In file.ReadFile
                    Yield x
                Next
            Next
        End Function

        Const null$ = Nothing

        Public Const Acc2Taxid_Header As String = "accession" & vbTab & "accession.version" & vbTab & "taxid" & vbTab & "gi"

        ''' <summary>
        ''' 这个函数所返回来的数据之中是包含有表头的
        ''' </summary>
        ''' <param name="acc_list"></param>
        ''' <param name="DIR$"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function Matchs(acc_list As IEnumerable(Of String),
                                        DIR$,
                                        Optional gb_priority? As Boolean = False,
                                        Optional debug? As Boolean = False) As IEnumerable(Of String)

            Dim list As Dictionary(Of String, String) = acc_list _
                .Distinct _
                .ToDictionary(Function(id) id.Split("."c).First,  ' 在这里移除版本号
                              Function(s) null)
            Yield {
                "accession", "accession.version", "taxid", "gi"
            }.JoinBy(vbTab)

            For Each x As NamedValue(Of Integer) In __loadData(DIR, gb_priority).AsParallel
                If list.ContainsKey(x.Name) Then
                    Yield x.Description

                    If list.Count = 0 Then
                        Exit For
                    Else
                        Call list.Remove(x.Name)

                        If debug Then
                            Call x.Description.__DEBUG_ECHO
                        End If
                    End If
                End If
            Next
        End Function
    End Module
End Namespace
