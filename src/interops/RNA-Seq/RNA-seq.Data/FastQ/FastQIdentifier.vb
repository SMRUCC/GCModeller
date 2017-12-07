#Region "Microsoft.VisualBasic::ecb92a8719a34bb2c482d9149adcc507, ..\interops\RNA-Seq\RNA-seq.Data\FastQ\FastQIdentifier.vb"

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


Namespace FQ

    ''' <summary>
    ''' Illumina sequence identifiers
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FastQIdentifier

        'Sequences from the Illumina software use a systematic identifier:

        '@HWUSI-EAS100R:6:73:941:1973#0/1

        'HWUSI-EAS100R	the unique instrument name
        '          6    flowcell lane
        '         73	tile number within the flowcell lane
        '        941	'x'-coordinate of the cluster within the tile
        '       1973    'y'-coordinate of the cluster within the tile
        '         #0	index number for a multiplexed sample (0 for no indexing)
        '         /1	the member of a pair, /1 or /2 (paired-end or mate-pair reads only)

        'Versions of the Illumina pipeline since 1.4 appear to use #NNNNNN instead of #0 for the multiplex ID, 
        'where NNNNNN is the sequence of the multiplex tag.

        'With Casava 1.8 the format of the '@' line has changed:

        '@EAS139:136:FC706VJ:2:2104:15343:197393 1:Y:18:ATCACG

        '       EAS139	the unique instrument name
        '        136	the run id
        '    FC706VJ	the flowcell id
        '          2	flowcell lane
        '       2104	tile number within the flowcell lane
        '      15343	'x'-coordinate of the cluster within the tile
        '     197393	'y'-coordinate of the cluster within the tile
        '          1	the member of a pair, 1 or 2 (paired-end or mate-pair reads only)
        '          Y	Y if the read is filtered, N otherwise
        '         18	0 when none of the control bits are on, otherwise it is an even number
        '     ATCACG	index sequence

        ''' <summary>
        ''' The unique instrument name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property instrument_name As String
        ''' <summary>
        ''' Flowcell lane
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property FlowCellLane As Integer
        ''' <summary>
        ''' Tile number within the flowcell lane
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Tiles As Integer
        ''' <summary>
        ''' 'x'-coordinate of the cluster within the tile
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property X As Integer
        ''' <summary>
        ''' 'y'-coordinate of the cluster within the tile
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Y As Integer
        ''' <summary>
        ''' Index number for a multiplexed sample (0 for no indexing)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property MsIndex As String
        ''' <summary>
        ''' The member of a pair, /1 or /2 (paired-end or mate-pair reads only)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property PairMember As String

        ''' <summary>
        ''' @HWUSI-EAS100R:6:73:941:1973#0/1
        ''' </summary>
        ''' <param name="str"></param>
        ''' <returns></returns>
        ''' <example>@FCC00ACABXX:8:1101:1333:1980#TTAGGCAT/1</example>
        Public Shared Function IDParser(str As String) As FastQIdentifier
            If Len(str) = 1 AndAlso str(0) = "+"c Then '可能是第三行数据
                Return New FastQIdentifier
            End If

            Dim tokens$() = str.Split(":"c)
            Dim ID As New FastQIdentifier With {
                .instrument_name = tokens(Scan0)
            }

            If tokens.Length = 1 Then
                ' 只有一个序列的标记符，其他的什么也没有了，直接返回
                Return ID
            End If

            ID.FlowCellLane = CInt(Val(tokens(1)))
            ID.Tiles = CInt(Val(tokens(2)))
            ID.X = CInt(Val(tokens(3)))
            ID.Y = CInt(Val(tokens(4)))

            tokens = tokens(4).Split("#"c).Last.Split("/"c)

            ID.MsIndex = tokens(0)
            ID.PairMember = tokens.ElementAtOrDefault(1)

            Return ID
        End Function

        Public Overrides Function ToString() As String
            Return $"{instrument_name}:{FlowCellLane}:{Tiles}:{X}:{Y}#{MsIndex}/{PairMember}"
        End Function
    End Class
End Namespace
