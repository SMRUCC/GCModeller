#Region "Microsoft.VisualBasic::2639e8e615b35e4969e173a4686b1706, visualize\Circos\Circos\TrackDatas\TrackDatas\data.vb"

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

    '     Class data
    ' 
    '         Properties: FileName
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: [GetType], GetDocumentText, GetEnumerator, IEnumerable_GetEnumerator, IEnumerable_GetEnumerator1
    '                   ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace TrackDatas

    ''' <summary>
    ''' Tracks data document generator.(使用这个对象生成data文件夹之中的数据文本文件)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    Public Class data(Of T As ITrackData)
        Implements IEnumerable(Of T)
        Implements Idata

        Public Property FileName As String Implements Idata.FileName

        Protected Friend source As List(Of T)

        ''' <summary>
        ''' Gets the element type <typeparamref name="T"/>
        ''' </summary>
        ''' <returns></returns>
        Public Overloads Function [GetType]() As Type
            Return GetType(T)
        End Function

        Sub New(source As IEnumerable(Of T))
            Me.source = New List(Of T)(source)
        End Sub

        Protected Sub New()
            source = New List(Of T)
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

            For Each data As T In Me
                If Not String.IsNullOrEmpty(data.comment) Then
                    Call sb.AppendLine("# " & data.comment)
                End If
                Call sb.AppendLine(data.GetLineData)
            Next

            Return sb.ToString
        End Function

        Public Overridable Iterator Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
            For Each x As T In source.SafeQuery
                Yield x
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function

        Private Iterator Function IEnumerable_GetEnumerator1() As IEnumerable(Of ITrackData) Implements Idata.GetEnumerator
            For Each x As T In source.SafeQuery
                Yield TryCast(x, ITrackData)
            Next
        End Function
    End Class
End Namespace
