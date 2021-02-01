#Region "Microsoft.VisualBasic::2cd30ce51aaa69fb4c30e1a5c9737eca, comparative_genomics\operonMapper.vb"

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
Imports SMRUCC.genomics.ComparativeGenomics.OperonMapper
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object

<Package("operon")>
Module operonMapper

    Sub New()
        Call Internal.Object.Converts.addHandler(GetType(OperonRow()), AddressOf operonTable)
    End Sub

    Private Function operonTable(rows As OperonRow(), args As list, env As Environment) As dataframe
        Dim cols As New Dictionary(Of String, Array)

        cols(NameOf(OperonRow.koid)) = rows.Select(Function(a) a.koid).ToArray
        cols(NameOf(OperonRow.name)) = rows.Select(Function(a) a.name).ToArray
        cols(NameOf(OperonRow.org)) = rows.Select(Function(a) a.org).ToArray
        cols(NameOf(OperonRow.op)) = rows.Select(Function(a) a.op.JoinBy(", ")).ToArray
        cols(NameOf(OperonRow.definition)) = rows.Select(Function(a) a.definition).ToArray
        cols(NameOf(OperonRow.source)) = rows.Select(Function(a) a.source).ToArray

        Return New dataframe With {
            .columns = cols,
            .rownames = rows.Keys
        }
    End Function

    <ExportAPI("known_operons")>
    Public Function knownOperons() As OperonRow()
        Return OperonRow.LoadInternalResource.ToArray
    End Function
End Module

