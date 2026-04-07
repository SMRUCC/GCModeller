#Region "Microsoft.VisualBasic::32a6b20d1172b2bd5a996d2aba9598db, analysis\OperonMapper\ODB\ODBOperon.vb"

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

    '   Total Lines: 66
    '    Code Lines: 42 (63.64%)
    ' Comment Lines: 15 (22.73%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 9 (13.64%)
    '     File Size: 2.17 KB


    ' Class ODBOperon
    ' 
    '     Properties: definition, koid, name, op, org
    '                 source
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: Load, LoadInternalResource, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Linq
Imports ASCII = Microsoft.VisualBasic.Text.ASCII

''' <summary>
''' A row of the data in ODB dataset ``known_operon.download.txt``
''' </summary>
Public Class ODBOperon : Implements INamedValue

    ''' <summary>
    ''' the operon cluster id
    ''' </summary>
    ''' <returns></returns>
    Public Property koid As String Implements IKeyedEntity(Of String).Key
    Public Property org As String
    ''' <summary>
    ''' the operon name
    ''' </summary>
    ''' <returns></returns>
    Public Property name As String
    ''' <summary>
    ''' the operon member gene ids, separated by comma
    ''' </summary>
    ''' <returns></returns>
    Public Property op As String()
    Public Property definition As String
    Public Property source As String

    Sub New()
    End Sub

    Sub New(operonID As String, name As String, members As IEnumerable(Of String))
        Me.koid = operonID
        Me.name = name
        Me.op = members.SafeQuery.ToArray
    End Sub

    Public Overrides Function ToString() As String
        Return name
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function LoadInternalResource() As IEnumerable(Of ODBOperon)
        Return Load(Encoding.UTF8.GetString(My.Resources.Data.known_operon_download))
    End Function

    Public Shared Iterator Function Load(file As String) As IEnumerable(Of ODBOperon)
        For Each line As String In file.SolveStream.LineTokens.Skip(1)
            Dim tokens As String() = line.Split(ASCII.TAB)
            Dim operon As New ODBOperon With {
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
