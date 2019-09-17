#Region "Microsoft.VisualBasic::4d60b270bddb972197a3b22e5c11129a, GO_gene-ontology\Test\DAGtest.vb"

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

    ' Module DAGtest
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

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
