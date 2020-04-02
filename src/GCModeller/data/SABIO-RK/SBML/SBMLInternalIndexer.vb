Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.Model.SBML.Level3

Namespace SBML

    Public Class SBMLInternalIndexer

        ReadOnly compartments As Dictionary(Of String, compartment)
        ReadOnly species As Dictionary(Of String, species)
        ReadOnly keggReactions As New Dictionary(Of String, List(Of SBMLReaction))

        Sub New(sbml As XmlFile(Of SBMLReaction))
            Dim entries As NamedValue(Of String)()

            For Each react As SBMLReaction In sbml.model.listOfReactions
                entries = react.getIdentifiers.Where(Function(r) r.Name = "kegg.reaction").ToArray

                For Each id In entries
                    If Not keggReactions.ContainsKey(id.Value) Then
                        keggReactions.Add(id.Value, New List(Of SBMLReaction))
                    End If

                    keggReactions(id.Value).Add(react)
                Next
            Next

            compartments = sbml.model.listOfCompartments.ToDictionary(Function(c) c.id)
            species = sbml.model.listOfSpecies.ToDictionary(Function(c) c.id)
        End Sub



        Public Iterator Function getEnzymes(react As SBMLReaction) As IEnumerable(Of species)
            If Not react.listOfModifiers.IsNullOrEmpty Then
                For Each modifier In react.listOfModifiers
                    Yield species(modifier.species)
                Next
            End If
        End Function
    End Class
End Namespace