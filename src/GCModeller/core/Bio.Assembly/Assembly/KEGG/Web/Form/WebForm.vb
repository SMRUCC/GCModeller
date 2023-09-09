﻿#Region "Microsoft.VisualBasic::c599619f1db13f2817d60143807b7661, GCModeller\core\Bio.Assembly\Assembly\KEGG\Web\Form\WebForm.vb"

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

    '   Total Lines: 233
    '    Code Lines: 155
    ' Comment Lines: 40
    '   Blank Lines: 38
    '     File Size: 8.83 KB


    '     Class WebForm
    ' 
    '         Properties: Count, GetMultipleItems, Keys, References, Values
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: ContainsKey, doFormParse, GetEnumerator, GetEnumerator1, GetValue
    '                   GetXmlTuples, TryGetValue
    ' 
    '         Sub: (+2 Overloads) Dispose, MoveNext, pullReference
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Option Strict Off

Imports Microsoft.VisualBasic.Emit.Marshal
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

Namespace Assembly.KEGG.WebServices.InternalWebFormParsers

    ''' <summary>
    ''' parser for the kegg form data text, example like request text 
    ''' content from the rest: ``https://rest.kegg.jp/get/hsa00592``
    ''' (KEGG 网页表格的数据解析方法，在Value之中可能会有重复的Key数据出现)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class WebForm : Implements IReadOnlyDictionary(Of String, String())
        Implements IDisposable

        ''' <summary>
        ''' Entry, {trim_formatted, non-process}
        ''' </summary>
        Dim _strData As New SortedDictionary(Of String, String())

        ''' <summary>
        ''' Reference list of this biological object
        ''' </summary>
        ''' <returns></returns>
        Public Property References As bGetObject.Reference()

        Default Public ReadOnly Property Item(key As String) As String
            Get
                If _strData.ContainsKey(key) Then
                    Return _strData(key).First
                Else
                    Return ""
                End If
            End Get
        End Property

        ''' <summary>
        ''' 这个构造函数同时支持url或者文本内容
        ''' </summary>
        ''' <param name="resource"></param>
        Sub New(resource As String, Optional unsafe As Boolean = False)
            Dim lines As New Pointer(Of String)(resource.SolveStream.LineTokens)

            If lines.Length = 1 AndAlso lines.Current.StringEmpty Then
                References = {}
            Else
                References = doFormParse(lines, unsafe)
            End If
        End Sub

        Private Function doFormParse(lines As Pointer(Of String), unsafe As Boolean) As bGetObject.Reference()
            ' fix size 12 chars
            Dim key As String = Nothing
            Dim name As String = Nothing
            Dim value As String = Nothing
            Dim list As New List(Of String)
            Dim refs As New List(Of bGetObject.Reference)
            Dim ref As bGetObject.Reference = Nothing

            Do While Not lines.EndRead
                Call MoveNext(lines, name, value)

                If name.StringEmpty Then
                    Call list.Add(value)
                ElseIf name = "///" Then
                    ' is break of current section
                    Exit Do
                Else
                    If list > 0 Then
                        If unsafe Then
                            _strData.Add(key, list.PopAll)
                        Else
                            _strData(key) = list.PopAll
                        End If
                    End If

                    list += value
                    key = name

                    If key = "REFERENCE" Then
                        key = Nothing
                        list *= 0
                        ref = New bGetObject.Reference With {
                            .PMID = value
                        }

                        Call pullReference(lines, ref)
                        Call refs.Add(ref)
                    End If
                End If
            Loop

            If list > 0 AndAlso Not key.StringEmpty Then
                If unsafe Then
                    _strData.Add(key, list.PopAll)
                Else
                    _strData(key) = list.PopAll
                End If
            End If

            Return refs.ToArray
        End Function

        Private Shared Sub pullReference(ByRef lines As Pointer(Of String), ByRef ref As bGetObject.Reference)
            Dim name As String = Nothing
            Dim value As String = Nothing

            Do While Not lines.EndRead
                Call MoveNext(lines, name, value)

                Select Case name
                    Case "AUTHORS" : ref.Authors = value.Split(","c)
                    Case "TITLE" : ref.Title = value
                    Case "JOURNAL" : ref.Journal = value
                    Case Else
                        If name.StringEmpty Then
                            ref.DOI = value
                            Exit Do
                        ElseIf name = "REFERENCE" Then
                            lines -= 1
                            Exit Do
                        Else
                            lines -= 1
                            Exit Do
                        End If
                End Select
            Loop
        End Sub

        Private Shared Sub MoveNext(lines As Pointer(Of String), ByRef name$, ByRef value$)
            Dim line As String = ++lines

            name = Mid(line, 1, 12).Trim
            value = Mid(line, 13).Trim
        End Sub

        ''' <summary>
        ''' 获取某一个字段的数据
        ''' </summary>
        ''' <param name="KeyWord">网页的表格之中的最左端的字段名</param>
        ''' <returns>
        ''' this is a null safe function: key not found will 
        ''' returns an empty collection.
        ''' </returns>
        ''' <remarks></remarks>
        Public Function GetValue(KeyWord As String) As String()
            If _strData.ContainsKey(KeyWord) Then
                Return _strData.Item(KeyWord)
            Else
                Return New String() {}
            End If
        End Function

        Public Iterator Function GetXmlTuples(key As String, Optional split As Boolean = False) As IEnumerable(Of NamedValue)
            For Each line As String In GetValue(key)
                Dim tuple = line.GetTagValue(" ", trim:=True, failureNoName:=False)

                If split Then
                    For Each id As String In tuple.Name.Split(","c)
                        Yield New NamedValue(id, tuple.Value)
                    Next
                Else
                    Yield New NamedValue(tuple)
                End If
            Next
        End Function

        Public Iterator Function GetDBLinks() As IEnumerable(Of DBLink)
            For Each line As String In GetValue("DBLINKS").SafeQuery
                Dim tag = line.GetTagValue(":", trim:=True)
                Dim idset = tag.Value.Split

                For Each id As String In idset
                    Yield New DBLink With {
                        .DBName = tag.Name,
                        .entry = id
                    }
                Next
            Next
        End Function

#Region "Implements IReadOnlyDictionary(Of String, String)"

        Public Iterator Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of String, String())) Implements IEnumerable(Of KeyValuePair(Of String, String())).GetEnumerator
            For Each item As KeyValuePair(Of String, String()) In Me._strData
                Yield item
            Next
        End Function

        Public ReadOnly Property Count As Integer Implements IReadOnlyCollection(Of KeyValuePair(Of String, String())).Count
            Get
                Return _strData.Count
            End Get
        End Property

        Public Function ContainsKey(key As String) As Boolean Implements IReadOnlyDictionary(Of String, String()).ContainsKey
            Return _strData.ContainsKey(key)
        End Function

        Private ReadOnly Property GetMultipleItems(key As String) As String() Implements IReadOnlyDictionary(Of String, String()).Item
            Get
                Return GetValue(key)
            End Get
        End Property

        Public ReadOnly Property Keys As IEnumerable(Of String) Implements IReadOnlyDictionary(Of String, String()).Keys
            Get
                Return _strData.Keys.ToArray
            End Get
        End Property

        Public Function TryGetValue(key As String, ByRef value As String()) As Boolean Implements IReadOnlyDictionary(Of String, String()).TryGetValue
            Return _strData.TryGetValue(key, value)
        End Function

        Public ReadOnly Property Values As IEnumerable(Of String()) Implements IReadOnlyDictionary(Of String, String()).Values
            Get
                Return _strData.Values
            End Get
        End Property

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function

#End Region

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose( disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose( disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
End Namespace
