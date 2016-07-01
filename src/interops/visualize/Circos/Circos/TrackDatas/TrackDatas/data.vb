#Region "Microsoft.VisualBasic::0c95f15044ddda81b9864f162a4fabaa, ..\interops\visualize\Circos\Circos\TrackDatas\TrackDatas\data.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports System.Text
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace TrackDatas

    Public Interface Idata
        Property FileName As String
        Function GetDocumentText() As String
        Function GetEnumerator() As IEnumerable(Of ITrackData)
    End Interface

    ''' <summary>
    ''' Tracks data document generator.(使用这个对象生成data文件夹之中的数据文本文件)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    Public Class data(Of T As ITrackData)
        Implements IEnumerable(Of T)
        Implements Idata

        Public Property FileName As String Implements Idata.FileName

        Protected __source As List(Of T)

        ''' <summary>
        ''' Gets the element type <typeparamref name="T"/>
        ''' </summary>
        ''' <returns></returns>
        Public Overloads Function [GetType]() As Type
            Return GetType(T)
        End Function

        Sub New(source As IEnumerable(Of T))
            __source = New List(Of T)(source)
        End Sub

        Protected Sub New()
        End Sub

        ''' <summary>
        ''' <see cref="GetJson"/>
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Function GetDocumentText() As String Implements Idata.GetDocumentText
            Dim sb As New StringBuilder

            For Each x As T In Me
                If Not String.IsNullOrEmpty(x.comment) Then
                    Call sb.AppendLine("# " & x.comment)
                End If
                Call sb.AppendLine(x.GetLineData)
            Next

            Return sb.ToString
        End Function

        Public Overridable Iterator Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
            For Each x As T In __source.SafeQuery
                Yield x
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function

        Private Iterator Function IEnumerable_GetEnumerator1() As IEnumerable(Of ITrackData) Implements Idata.GetEnumerator
            For Each x As T In __source.SafeQuery
                Yield TryCast(x, ITrackData)
            Next
        End Function
    End Class
End Namespace
