#Region "Microsoft.VisualBasic::3b04c9858af1e384983c4c18a40f8ea7, Bio.Assembly\Assembly\NCBI\Taxonomy\Accession2Taxid.vb"

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

    '     Module Accession2Taxid
    ' 
    '         Function: __loadData, LoadAll, Matchs, ReadFile
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
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
        ''' 做数据库的subset操作。这个函数所返回来的数据之中是包含有表头的
        ''' </summary>
        ''' <param name="acc_list"></param>
        ''' <param name="DIR$"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function Matchs(acc_list As IEnumerable(Of String),
                                        DIR$,
                                        Optional gb_priority? As Boolean = False,
                                        Optional debug? As Boolean = False) As IEnumerable(Of String)

            ' 2017-12-25 
            ' 因为后面的循环之中需要进行已经被match上的对象的remove操作
            ' 所以在这里就不适用Index对象了，直接使用Dictionary
            Dim list As Dictionary(Of String, String) = acc_list _
                .Select(Function(id)
                            ' 在这里移除版本号
                            Return id.Split("."c).First
                        End Function) _
                .Distinct _
                .ToDictionary(Function(id) id)

            Yield {
                "accession", "accession.version", "taxid", "gi"
            }.JoinBy(vbTab)

            Dim n% = 0
            Dim ALL% = list.Count

            For Each x As NamedValue(Of Integer) In __loadData(DIR, gb_priority).AsParallel
                If list.ContainsKey(x.Name) Then
                    Yield x.Description

                    If list.Count = 0 Then
                        Exit For
                    Else
                        Call list.Remove(x.Name)
                        Call n.SetValue(n + 1)

                        If debug Then
                            Call x.Description.__DEBUG_ECHO
                        End If
                    End If
                End If
            Next

            Call $"{ALL} accession id match {n} taxonomy info.".__INFO_ECHO
        End Function
    End Module
End Namespace
