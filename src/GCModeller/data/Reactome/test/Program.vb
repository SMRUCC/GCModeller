#Region "Microsoft.VisualBasic::e22c5f17dee1e1360e71cf1ef0d32978, data\Reactome\test\Program.vb"

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

    '   Total Lines: 24
    '    Code Lines: 17 (70.83%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 7 (29.17%)
    '     File Size: 644 B


    ' Module Program
    ' 
    '     Sub: Main, pathwayTree
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.MIME.application.json
Imports SMRUCC.genomics.Data.Reactome

Module Program
    Sub Main(args As String())
        Call pathwayTree()

        Console.WriteLine("Hello World!")
    End Sub

    Sub pathwayTree()
        Dim hsa_tree = Hierarchy.LoadInternal(tax:="Homo sapiens")
        Dim json As String = Hierarchy.TreeJSON(hsa_tree)

        Call Console.WriteLine(json)
        Call json.SaveTo("./HSA.json")

        Dim json2 As String = HierarchyLink.LoadInternal("Homo sapiens").Values.ToArray.GetJson

        Call json2.SaveTo("./HSA.json")

        Pause()
    End Sub
End Module
