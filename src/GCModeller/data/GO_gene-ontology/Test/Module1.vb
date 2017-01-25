#Region "Microsoft.VisualBasic::6f0a34345ab4425b348f7ac69f776475, ..\GCModeller\data\GO_gene-ontology\Test\Module1.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
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

Module Module1

    Sub Main()

        Dim tree = SMRUCC.genomics.Data.GeneOntology.DAG.BuildTree("H:\GO_DB\go-basic.obo")

        Dim file = SMRUCC.genomics.Data.GeneOntology.OBO.GO_OBO.LoadDocument("G:\GCModeller\src\GCModeller\data\GO_gene-ontology\obographs\resources\basic.obo")
        file = SMRUCC.genomics.Data.GeneOntology.OBO.GO_OBO.LoadDocument("G:\GCModeller\src\GCModeller\data\GO_gene-ontology\obographs\resources\equivNodeSetTest.obo")

        Pause()
    End Sub
End Module
