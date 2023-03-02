Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes

Namespace MetabolicModel

    Public Module KEGGConvertor

        Public Function ConvertReaction(reaction As Reaction) As MetabolicReaction
            Dim eq As Equation = reaction.ReactionModel

            Return New MetabolicReaction With {
                .ECNumbers = reaction.Enzyme,
                .id = reaction.ID,
                .is_reversible = reaction.Reversible,
                .is_spontaneous = Not .ECNumbers.IsNullOrEmpty,
                .name = reaction.CommonNames.JoinBy("; "),
                .description = reaction.Definition,
                .left = eq.Reactants,
                .right = eq.Products
            }
        End Function

        Public Function ConvertCompound(compound As Compound) As MetabolicCompound
            Return New MetabolicCompound With {
                .formula = compound.formula,
                .id = compound.entry,
                .moleculeWeight = compound.exactMass,
                .name = compound.commonNames.JoinBy("; "),
                .xref = compound.DbLinks
            }
        End Function
    End Module
End Namespace