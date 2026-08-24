#Region "Microsoft.VisualBasic::b5ca41b28435d8c7e8044fb6da721b84, core\Bio.Assembly\Test\kgml_test.vb"

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

    '   Total Lines: 17
    '    Code Lines: 14 (82.35%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 3 (17.65%)
    '     File Size: 791 B


    ' Module kgml_test
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.KGML

Public Module kgml_test

    Sub Main()
        Dim maps = {"F:\datapool\20260301\202608-Figures\分子表达图\network\taes00941.xml",
"F:\datapool\20260301\202608-Figures\分子表达图\network\taes00999.xml",
"F:\datapool\20260301\202608-Figures\分子表达图\network\taes04120.xml",
"F:\datapool\20260301\202608-Figures\分子表达图\network\taes00940.xml"}
        Dim kgml_maps = maps.Select(Function(file) pathway.LoadMap(file)).ToArray
        Dim network = kgml_maps.Select(Function(p) GeneMetaboliteNetwork.ExtractNetwork(p, True)).IteratesALL.ToArray

        Call network.SaveTo("Z:/network.csv")
    End Sub
End Module

