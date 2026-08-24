#Region "Microsoft.VisualBasic::2bdfc36374ba89d32a8c5f189833eaed, analysis\Microarray\CausalModeling\LatentSymbol.vb"

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

    '   Total Lines: 54
    '    Code Lines: 45 (83.33%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 9 (16.67%)
    '     File Size: 2.17 KB


    ' Class LatentSymbol
    ' 
    '     Properties: [class], latent, manifest_id
    ' 
    '     Function: LatentSymbols, MakeFullPath, MakeLatents, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.GraphTheory

Public Class LatentSymbol

    Public Property [class] As String
    Public Property latent As String
    Public Property manifest_id As String

    Public Overrides Function ToString() As String
        Return $"[{[class]}:{latent}] {manifest_id}"
    End Function

    Public Shared Iterator Function MakeLatents(symbols As IEnumerable(Of LatentSymbol)) As IEnumerable(Of LatentDefinition)
        For Each cls_group As IGrouping(Of String, LatentSymbol) In symbols.GroupBy(Function(s) s.class)
            For Each latent As IGrouping(Of String, LatentSymbol) In cls_group.GroupBy(Function(s) s.latent)
                Yield New LatentDefinition(
                    name:=$"{cls_group.Key}:{latent.Key}",
                    manifest:=From s As LatentSymbol
                              In latent
                              Select s.manifest_id
                )
            Next
        Next
    End Function

    Public Shared Iterator Function MakeFullPath(symbols As IEnumerable(Of LatentSymbol), from As String(), [to] As String()) As IEnumerable(Of SparseGraph.Edge)
        Dim class_group As Dictionary(Of String, String()) = symbols _
            .GroupBy(Function(s) s.class) _
            .ToDictionary(Function(s) s.Key,
                          Function(s)
                              Return LatentSymbols(s)
                          End Function)

        For i As Integer = 0 To from.Length - 1
            Dim listFrom = class_group(from(i))
            Dim listTo = class_group([to](i))

            For Each u In listFrom
                For Each v In listTo
                    If u <> v Then
                        Yield New SparseGraph.Edge(u, v)
                    End If
                Next
            Next
        Next
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Shared Function LatentSymbols(g As IGrouping(Of String, LatentSymbol)) As String()
        Return MakeLatents(g).Select(Function(s) s.varName).Distinct.ToArray
    End Function

End Class

