#Region "Microsoft.VisualBasic::483760fc7c145da2fed5cae156889af7, RNA-Seq\RNA-seq.Data\FastQ\IlluminaFastQID.vb"

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

    '     Class IlluminaFastQID
    ' 
    '         Properties: ControlBits, Filtered, FlowCellID, FlowCellLane, IndexSequence
    '                     instrument_name, IsEmpty, PairMember, RunID, Tiles
    '                     X, Y
    ' 
    '         Function: IDParser, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Field = Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps.DataFrameColumnAttribute

Namespace FQ

    ''' <summary>
    ''' Illumina sequence identifiers
    ''' </summary>
    ''' <remarks></remarks>
    Public Class IlluminaFastQID

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

        ' @M04670:66:000000000-B83GM:1:2106:20428:2266 1:N:0:CGGCTATG+GGCTCTGA

        ' @EAS139:136:FC706VJ:2:2104:15343:197393 1:Y:18:ATCACG

        ' With Casava 1.8 the format of the '@' line has changed:

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

#Region "A"
        ''' <summary>
        ''' A1 The unique instrument name
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Field(0)> Public Property instrument_name As String
        ''' <summary>
        ''' A2
        ''' </summary>
        ''' <returns></returns>
        <Field(1)> Public Property RunID As String
        ''' <summary>
        ''' A3
        ''' </summary>
        ''' <returns></returns>
        <Field(2)> Public Property FlowCellID As String
        ''' <summary>
        ''' A4 Flowcell lane
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Field(3)> Public Property FlowCellLane As String
        ''' <summary>
        ''' A5 Tile number within the flowcell lane
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Field(4)> Public Property Tiles As String
        ''' <summary>
        ''' A6 'x'-coordinate of the cluster within the tile
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Field(5)> Public Property X As Integer
        ''' <summary>
        ''' A7 'y'-coordinate of the cluster within the tile
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Field(6)> Public Property Y As Integer
#End Region
#Region "B"
        ''' <summary>
        ''' B8 The member of a pair, /1 or /2 (paired-end or mate-pair reads only)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Field(8)> Public Property PairMember As String
        ''' <summary>
        ''' B9 Y if the read is filtered, N otherwise
        ''' </summary>
        ''' <returns></returns>
        <Field(9)> Public Property Filtered As String
        ''' <summary>
        ''' B10 0 when none of the control bits are on, otherwise it is an even number
        ''' </summary>
        ''' <returns></returns>
        <Field(10)> Public Property ControlBits As Integer
        ''' <summary>
        ''' B11 index sequence
        ''' </summary>
        ''' <returns></returns>
        <Field(11)> Public Property IndexSequence As String
#End Region

        Public ReadOnly Property IsEmpty As Boolean
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return instrument_name.StringEmpty
            End Get
        End Property

        ''' <summary>
        ''' @HWUSI-EAS100R:6:73:941:1973#0/1
        ''' </summary>
        ''' <param name="str"></param>
        ''' <returns></returns>
        ''' <example>@FCC00ACABXX:8:1101:1333:1980#TTAGGCAT/1</example>
        Public Shared Function IDParser(str As String) As IlluminaFastQID
            If Len(str) = 1 AndAlso str(0) = "+"c Then '可能是第三行数据
                Return New IlluminaFastQID
            End If

            Dim tokens As String() = str.Split(" "c)
            Dim A = tokens(Scan0).Split(":"c)
            Dim B = tokens _
                .ElementAtOrDefault(1) _
               ?.Split(":"c)
            Dim ID As New IlluminaFastQID With {
                .instrument_name = A(Scan0)
            }

            If tokens.Length = 1 Then
                ' 只有一个序列的标记符，其他的什么也没有了，直接返回
                Return ID
            Else
                With ID
                    .RunID = A(1)
                    .FlowCellID = A(2)
                    .FlowCellLane = A(3)
                    .Tiles = A(4)
                    .X = CInt(Val(A(5)))
                    .Y = CInt(Val(A(6)))

                    .PairMember = B(0)
                    .Filtered = B(1)
                    .ControlBits = CInt(Val(B(2)))
                    .IndexSequence = B(3)
                End With
            End If

            Return ID
        End Function

        ''' <summary>
        ''' Create fastq document line
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return $"{instrument_name}:{RunID}:{FlowCellID}:{FlowCellLane}:{Tiles}:{X}:{Y} {PairMember}:{Filtered}:{ControlBits}:{IndexSequence}"
        End Function
    End Class
End Namespace
