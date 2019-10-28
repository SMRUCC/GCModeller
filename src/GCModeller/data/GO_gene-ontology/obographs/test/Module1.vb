#Region "Microsoft.VisualBasic::50c6ea33bfbbc9a7c61974bf99ad5a2f, GO_gene-ontology\obographs\test\Module1.vb"

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

' Module Module1
' 
'     Sub: Main
' 
' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Data.GeneOntology

Module Module1

    Sub Main()
        Call inferTest()

        Call App.Stop()
    End Sub

    Sub inferTest()
        ' is_a
        Call print(OntologyRelations.is_a, OntologyRelations.is_a)
        Call print(OntologyRelations.is_a, OntologyRelations.part_of)
        Call print(OntologyRelations.is_a, OntologyRelations.regulates)
        Call print(OntologyRelations.is_a, OntologyRelations.positively_regulates)
        Call print(OntologyRelations.is_a, OntologyRelations.negatively_regulates)
        Call print(OntologyRelations.is_a, OntologyRelations.has_part)

        Call Console.WriteLine()

        ' part_of
        Call print(OntologyRelations.part_of, OntologyRelations.is_a)
        Call print(OntologyRelations.part_of, OntologyRelations.part_of)

        Call Console.WriteLine()

        ' regulates
        Call print(OntologyRelations.regulates, OntologyRelations.is_a)
        Call print(OntologyRelations.regulates, OntologyRelations.part_of)

        Call Console.WriteLine()

        ' positive_regulates
        Call print(OntologyRelations.positively_regulates, OntologyRelations.is_a)
        Call print(OntologyRelations.positively_regulates, OntologyRelations.part_of)

        Call Console.WriteLine()

        ' negative_regulates
        Call print(OntologyRelations.negatively_regulates, OntologyRelations.is_a)
        Call print(OntologyRelations.negatively_regulates, OntologyRelations.part_of)

        Call Console.WriteLine()

        ' has_part
        Call print(OntologyRelations.has_part, OntologyRelations.is_a)
        Call print(OntologyRelations.has_part, OntologyRelations.has_part)
    End Sub

    Sub print(AB As OntologyRelations, BC As OntologyRelations)
        Call Console.WriteLine($"{AB.Description} -> o -> {BC.Description} = {Axioms.InferRule(AB, BC).Description}")
    End Sub
End Module
