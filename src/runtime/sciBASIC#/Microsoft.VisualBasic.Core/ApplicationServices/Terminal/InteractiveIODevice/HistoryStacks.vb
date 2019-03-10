﻿#Region "Microsoft.VisualBasic::39af66f7c97b0329f39a08cf036d2591, Microsoft.VisualBasic.Core\ApplicationServices\Terminal\InteractiveIODevice\HistoryStacks.vb"

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

    '     Class HistoryStacks
    ' 
    '         Properties: HistoryList
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: __getDefaultPath, __getHistory, MoveFirst, MoveLast, MoveNext
    '                   MovePrevious, Save, ToString
    ' 
    '         Sub: __init, PushStack, StartInitialize
    '         Structure History
    ' 
    '             Function: ToString
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Terminal

    Public Class HistoryStacks : Inherits ITextFile
        Implements ISaveHandle

        Dim _historyList As List(Of String)
        Dim _lsthistory As List(Of History)

        ''' <summary>
        ''' 指向<see cref="_historyList"></see>
        ''' </summary>
        ''' <remarks></remarks>
        Dim p As Integer

        Public Property HistoryList As List(Of History)
            Get
                Return _lsthistory
            End Get
            Set(value As List(Of History))
                _lsthistory = value
            End Set
        End Property

        Dim LastHistory As History

        Sub New()
            LastHistory = New History With {
                .Date = Now.ToString,
                .Histories = New List(Of String)
            }
        End Sub

        Sub New(path As String)
            Call Me.New
            FilePath = path
        End Sub

        Public Sub StartInitialize()
            Call __init()
            _historyList = (From his As History In _lsthistory Select his.Histories).Unlist
            p = _historyList.Count - 1
            If p < 0 Then p = 0
        End Sub

        Public Sub PushStack(s As String)
            If _historyList.IsNullOrEmpty Then
                Call __init()
            End If

            Call LastHistory.Histories.Add(s)
            Call _historyList.Insert(p, s)
        End Sub

        Private Sub __init()
            _historyList = New List(Of String)
            If _lsthistory.IsNullOrEmpty Then _lsthistory = New List(Of History)
            Call _lsthistory.Add(LastHistory)
        End Sub

        Public Function MovePrevious() As String
            p -= 1
            Return __getHistory()
        End Function

        Public Function MoveNext() As String
            p += 1
            Return __getHistory()
        End Function

        Public Function MoveFirst() As String
            p = 0
            Return __getHistory()
        End Function

        Public Function MoveLast() As String
            p = _historyList.Count - 1
            Return __getHistory()
        End Function

        Private Function __getHistory() As String
            If p < 0 Then
                p = 0
            End If

            If _historyList.IsNullOrEmpty Then
                Call __init()
                Return ""
            End If

            If p > _historyList.Count - 1 Then
                p = _historyList.Count - 1
            End If

            Dim s As String = _historyList(p)
            Return s
        End Function

        Public Structure History

            Public [Date] As String
            Public Histories As List(Of String)

            Public Overrides Function ToString() As String
                Return Me.GetJson
            End Function
        End Structure

        Public Overrides Function ToString() As String
            Return String.Join(";  ", _historyList.Take(3).ToArray) & "......."
        End Function

        Public Overrides Function Save(Optional Path As String = "", Optional encoding As System.Text.Encoding = Nothing) As Boolean
            Path = MyBase.getPath(Path)
            Return Me.GetXml.SaveTo(Path, encoding)
        End Function

        Protected Overrides Function __getDefaultPath() As String
            Return FilePath
        End Function
    End Class
End Namespace
