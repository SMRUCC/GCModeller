#Region "Microsoft.VisualBasic::824216e5ec159753fbe59ae8c086e6a9, core\Bio.Assembly\Assembly\NCBI\Taxonomy\Accession2Taxid.vb"

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

    '   Total Lines: 140
    '    Code Lines: 82 (58.57%)
    ' Comment Lines: 36 (25.71%)
    '    - Xml Docs: 72.22%
    ' 
    '   Blank Lines: 22 (15.71%)
    '     File Size: 5.64 KB


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
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Assembly.NCBI.Taxonomy

    ''' <summary>
    ''' 将序列的AccessionID编号转换为Taxid编号
    ''' </summary>
    Public Class Accession2Taxid

        Public Property accession As String
        Public Property accession_version As String
        Public Property taxid As UInteger
        Public Property gi As UInteger

        Public Const acc2Taxid_Header As String = "accession" & vbTab & "accession.version" & vbTab & "taxid" & vbTab & "gi"

        Public Overrides Function ToString() As String
            Return New String() {accession, accession_version, taxid, gi}.JoinBy(vbTab)
        End Function

        ''' <summary>
        ''' 一次性的加载完整个数据库之中的数据到内存之中（不推荐）
        ''' </summary>
        ''' <param name="DIR$"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function LoadAll(DIR As String) As BucketDictionary(Of String, UInteger)
            Return LoadDataAll(DIR) _
                .CreateBuckets(Function(a) a.accession,
                               Function(a)
                                   Return a.taxid
                               End Function)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="file"></param>
        ''' <returns></returns>
        Public Shared Iterator Function ReadFile(file As String) As IEnumerable(Of Accession2Taxid)
            Dim line As Value(Of String) = ""
            Dim tokens$()

            Using reader As StreamReader = file.OpenReader
                ' skip first line, headers
                Call reader.ReadLine()

                Do While (line = reader.ReadLine) IsNot Nothing
                    tokens = line.Split(ASCII.TAB)

                    ' 2018-11-03 因为下面的解析过程是依据具体的index来进行的
                    ' 所以即使输入的原始数据之中的行末尾追加了其他的数据
                    ' 也不会对数据的读取造成影响

                    ' 0               1                       2       3
                    ' accession       accession.version       taxid   gi
                    Yield New Accession2Taxid With {
                        .accession = tokens(0),
                        .accession_version = tokens(1),
                        .taxid = CUInt(Val(tokens(2))),
                        .gi = CUInt(Val(tokens(3)))
                    }
                Loop
            End Using
        End Function

        ''' <summary>
        ''' 这个函数返回``{name: accession(不带版本号), value:taxid, description: raw_input_line}``
        ''' </summary>
        ''' <param name="DIR$"></param>
        ''' <param name="gb_priority"></param>
        ''' <returns></returns>
        Private Shared Iterator Function LoadDataAll(DIR$, Optional gb_priority? As Boolean = False) As IEnumerable(Of Accession2Taxid)
            Dim files$() = (ls - l - r - "*.*" <= DIR).ToArray

            If gb_priority Then
                For i As Integer = 0 To files.Length - 1
                    ' 优先加载gb库，提升匹配查找函数的效率
                    If files(i).BaseName.TextEquals("nucl_gb") Then
                        Call files.Swap(i, Scan0)
                        Exit For
                    End If
                Next
            End If

            For Each file$ In files
                Call file.ToFileURL.debug

                For Each x As Accession2Taxid In ReadFile(file)
                    Yield x
                Next
            Next
        End Function

        ''' <summary>
        ''' 做数据库的subset操作。这个函数所返回来的数据之中是包含有表头的
        ''' </summary>
        ''' <param name="acc_list"></param>
        ''' <param name="DIR$"></param>
        ''' <returns></returns>
        Public Shared Iterator Function Matchs(acc_list As IEnumerable(Of String),
                                               DIR$,
                                               Optional gb_priority? As Boolean = False,
                                               Optional debug? As Boolean = False) As IEnumerable(Of String)

            ' 2017-12-25 
            ' 因为后面的循环之中需要进行已经被match上的对象的remove操作
            ' 所以在这里就不适用Index对象了，直接使用Dictionary
            Dim list As Dictionary(Of String, String) = acc_list _
                .Select(AddressOf TrimAccessionVersion) _
                .Distinct _
                .ToDictionary(Function(id) id)

            Yield Acc2Taxid_Header

            Dim n% = 0
            Dim all% = list.Count

            For Each accessionId As Accession2Taxid In LoadDataAll(DIR, gb_priority).AsParallel
                If list.ContainsKey(accessionId.accession) Then
                    Yield accessionId.ToString

                    If list.Count = 0 Then
                        Exit For
                    Else
                        Call list.Remove(accessionId.accession)
                        Call n.InlineCopy(n + 1)

                        If debug Then
                            Call accessionId.ToString.debug
                        End If
                    End If
                End If
            Next

            Call $"{all} accession id match {n} taxonomy info.".info
        End Function
    End Class
End Namespace
