#Region "Microsoft.VisualBasic::625d5d9a1709d8e6d2925003907da628, R#\comparative_genomics\operonMapper.vb"

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

    ' Module operonMapper
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: knownOperons, operonTable
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Model.OperonMapper
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object

<Package("operon")>
Module operonMapper

    Sub New()
        Call Internal.Object.Converts.addHandler(GetType(ODBOperon()), AddressOf operonTable)
    End Sub

    Private Function operonTable(rows As ODBOperon(), args As list, env As Environment) As dataframe
        Dim cols As New Dictionary(Of String, Array)

        cols(NameOf(ODBOperon.koid)) = rows.Select(Function(a) a.koid).ToArray
        cols(NameOf(ODBOperon.name)) = rows.Select(Function(a) a.name).ToArray
        cols(NameOf(ODBOperon.org)) = rows.Select(Function(a) a.org).ToArray
        cols(NameOf(ODBOperon.op)) = rows.Select(Function(a) a.op.JoinBy(", ")).ToArray
        cols(NameOf(ODBOperon.definition)) = rows.Select(Function(a) a.definition).ToArray
        cols(NameOf(ODBOperon.source)) = rows.Select(Function(a) a.source).ToArray

        Return New dataframe With {
            .columns = cols,
            .rownames = rows.Keys
        }
    End Function

    <ExportAPI("known_operons")>
    Public Function knownOperons() As ODBOperon()
        Return ODBOperon.LoadInternalResource.ToArray
    End Function

    ''' <summary>
    ''' load operon set data from the ODB database
    ''' </summary>
    ''' <param name="file">dataset text file that download from https://operondb.jp/</param>
    ''' <returns></returns>
    <ExportAPI("operon_set")>
    Public Function operon_set(Optional file As String = Nothing) As ODBOperon()
        If file.StringEmpty(, True) Then
            Return ODBOperon.LoadInternalResource.ToArray
        Else
            Return ODBOperon.Load(file).ToArray
        End If
    End Function
End Module
