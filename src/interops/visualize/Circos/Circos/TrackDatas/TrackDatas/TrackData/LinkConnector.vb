#Region "Microsoft.VisualBasic::170f1dffdd3879affebd2d7e3e9a6764, visualize\Circos\Circos\TrackDatas\TrackDatas\TrackData\LinkConnector.vb"

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

    '     Structure link
    ' 
    '         Properties: comment
    ' 
    '         Function: ToString
    ' 
    '     Structure Connection
    ' 
    '         Properties: [to], chr, comment, from, IsEmpty
    ' 
    '         Function: GetLineData, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON

Namespace TrackDatas

    ''' <summary>
    ''' + Finally, links are a special track type which associates two ranges together. The format For links Is analogous To other data types, 
    ''' except now two coordinates are specified.
    ''' 
    ''' ```
    ''' chr12 1000 5000 chr15 5000 7000
    ''' ```
    ''' </summary>
    Public Class LinkData : Implements ITrackData

        ''' <summary>
        ''' 连接的第一个端点 ``CHR START END``
        ''' </summary>
        ''' <returns></returns>
        Public Property A As TrackData
        ''' <summary>
        ''' 连接的第二个端点 ``CHR START END``
        ''' </summary>
        ''' <returns></returns>
        Public Property B As TrackData

        Sub New()
        End Sub

        ''' <summary>
        ''' 创建一条新的连接关系
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        Sub New(a As TrackData, b As TrackData)
            Me.A = a
            Me.B = b
        End Sub

        Public Property comment As String Implements ITrackData.comment

        Public Overrides Function ToString() As String Implements ITrackData.GetLineData
            ' 未初始化的连接会得到一个空的绘图数据行，circos 在读取到空行的时候会报错，
            ' 所以在这里给出一个明确的错误提示信息
            If A Is Nothing OrElse B Is Nothing Then
                Throw New InvalidOperationException(
                    $"Incomplete link data: the {(If(A Is Nothing, "first", "second"))} end of the link data is not initialized! " &
                    $"A link requires two registered genomic regions.")
            End If

            Return A.ToString & " " & B.ToString
        End Function
    End Class

    Public Structure Connection
        Implements ITrackData

        Public Property comment As String Implements ITrackData.comment
        Public Property from As Integer
        Public Property [to] As Integer
        Public Property chr As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public ReadOnly Property IsEmpty As Boolean
            Get
                If Not String.IsNullOrEmpty(chr) OrElse from > 0 OrElse [to] > 0 Then
                    Return False
                Else
                    Return True
                End If
            End Get
        End Property

        Public Function GetLineData() As String Implements ITrackData.GetLineData
            Return $"{chr} {from} {[to]}"
        End Function
    End Structure
End Namespace
