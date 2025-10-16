#Region "Microsoft.VisualBasic::17d100fe8073de020a59bd0fc8ad6786, analysis\OperonMapper\ODB\OperonRow.vb"

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

    ' Class OperonRow
    ' 
    '     Properties: definition, koid, name, op, org
    '                 source
    ' 
    '     Function: Load, LoadInternalResource
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports ASCII = Microsoft.VisualBasic.Text.ASCII

''' <summary>
''' A row of the data in ODB dataset ``known_operon.download.txt``
''' </summary>
Public Class OperonRow : Implements INamedValue

    Public Property koid As String Implements IKeyedEntity(Of String).Key
    Public Property org As String
    Public Property name As String
    Public Property op As String()
    Public Property definition As String
    Public Property source As String

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function LoadInternalResource() As IEnumerable(Of OperonRow)
        Return Load(Encoding.UTF8.GetString(My.Resources.Data.known_operon_download))
    End Function

    Public Shared Iterator Function Load(file As String) As IEnumerable(Of OperonRow)
        For Each line As String In file.SolveStream.LineTokens.Skip(1)
            Dim tokens As String() = line.Split(ASCII.TAB)
            Dim operon As New OperonRow With {
                .koid = tokens(Scan0),
                .org = tokens(1),
                .name = tokens(2),
                .op = tokens(3).StringSplit(","),
                .definition = tokens(4),
                .source = tokens(5)
            }

            Yield operon
        Next
    End Function

End Class
