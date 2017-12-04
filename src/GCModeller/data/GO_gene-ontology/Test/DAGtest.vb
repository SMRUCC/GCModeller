#Region "Microsoft.VisualBasic::74fade42c3c5bd4ed5673228bc7d5cdf, ..\GCModeller\data\GO_gene-ontology\Test\DAGtest.vb"

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

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.Data.GeneOntology
Imports SMRUCC.genomics.Data.GeneOntology.DAG

Module DAGtest

    Sub Main()
        Dim g As Graph = New Graph("D:\smartnucl_integrative\DATA\go.obo")

        Dim t = g.Family("GO:0000007").Select(Function(x) x.Strip).ToArray


        Dim stat As New Dictionary(Of String, NamedValue(Of Integer)()) From {
            {"biological_process",
                {
                    New NamedValue(Of Integer)("GO:0009409", 1),
                    New NamedValue(Of Integer)("GO:0009725", 1),
                    New NamedValue(Of Integer)("GO:0033993", 1),
                    New NamedValue(Of Integer)("GO:0097305", 1),
                    New NamedValue(Of Integer)("GO:0009743", 1),
                    New NamedValue(Of Integer)("GO:0014070", 1)
                }
            }
        }


        Dim level3 = stat.LevelGOTerms(3, g)


        Pause()
    End Sub
End Module

