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
    Public Structure link : Implements ITrackData

        Dim a As TrackData
        Dim b As TrackData

        Public Property comment As String Implements ITrackData.comment

        Public Overrides Function ToString() As String Implements ITrackData.GetLineData
            ' link 是一个 Structure，其在使用默认构造函数创建的时候内部的两个数据点都是 Nothing，
            ' 直接调用 ToString 会抛出 NRE，所以在这里给出一个明确的错误提示信息
            If a Is Nothing OrElse b Is Nothing Then
                Throw New InvalidOperationException(
                    $"Incomplete link data: the {(If(a Is Nothing, "first", "second"))} end of the link data is not initialized! " &
                    $"A link requires two registered genomic regions.")
            End If

            Return a.ToString & " " & b.ToString
        End Function
    End Structure

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
