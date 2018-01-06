#Region "Microsoft.VisualBasic::8e9070485a9207cd770e8b54474c5174, ..\GCModeller\core\Bio.Assembly\test\TaxonomyTreeTest.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
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

Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy

Module TaxonomyTreeTest
    Sub Main()
        Dim tree As New NcbiTaxonomyTree("T:\Resources\NCBI_taxonomy")

        Dim nodes = tree.GetAscendantsWithRanksAndNames(526962, only_std_ranks:=True).BuildBIOM
        Dim lll = tree.GetAscendantsWithRanksAndNames(1051655, only_std_ranks:=True)

        Pause()
    End Sub
End Module

