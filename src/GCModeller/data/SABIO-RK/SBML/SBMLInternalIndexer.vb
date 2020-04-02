Imports SMRUCC.genomics.Model.SBML.Level3

Namespace SBML

    Public Class SBMLInternalIndexer

        ReadOnly compartments As Dictionary(Of String, compartment)
        ReadOnly species As Dictionary(Of String, species)

        Sub New(sbml As XmlFile(Of SBMLReaction))
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