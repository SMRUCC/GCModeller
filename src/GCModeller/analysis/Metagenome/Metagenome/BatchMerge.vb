#Region "Microsoft.VisualBasic::8071947b9cdef41b49b6d4a9e044d3b8, analysis\Metagenome\Metagenome\BatchMerge.vb"

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

    '   Total Lines: 34
    '    Code Lines: 30 (88.24%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 4 (11.76%)
    '     File Size: 1.34 KB


    ' Module BatchMerge
    ' 
    '     Function: BatchCombine
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Public Module BatchMerge

    <Extension>
    Public Iterator Function BatchCombine(batch1 As OTUTable(), batch2 As OTUTable()) As IEnumerable(Of OTUTable)
        Dim merge As NamedCollection(Of OTUTable)() = batch1 _
            .JoinIterates(batch2) _
            .GroupBy(Function(otu) otu.taxonomy.ToString) _
            .Select(Function(otu) New NamedCollection(Of OTUTable)(otu.Key, otu.ToArray)) _
            .ToArray
        Dim otu_id As i32 = 1

        For Each otu As NamedCollection(Of OTUTable) In merge
            Dim vec As Dictionary(Of String, Double) = otu _
                .Select(Function(a) a.Properties) _
                .IteratesALL _
                .GroupBy(Function(a) a.Key) _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return a.Sum(Function(xi) xi.Value)
                              End Function)

            Yield New OTUTable With {
                .ID = $"otu{++otu_id}",
                .Properties = vec,
                .taxonomy = otu.First.taxonomy
            }
        Next
    End Function
End Module

