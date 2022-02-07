Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.Data.BioCyc.Assembly.MetaCyc.Schema.Metabolism

Module Factory

    ReadOnly reactionDirections As New Dictionary(Of String, ReactionDirections)

    Sub New()
        For Each flag As ReactionDirections In Enums(Of ReactionDirections)()
            reactionDirections(flag.Description) = flag
        Next
    End Sub

    Public Function ParseReactionDirection(value As ValueString) As ReactionDirections
        Return reactionDirections(value.value)
    End Function

    Public Function ParseCompoundReference(value As ValueString) As CompoundSpecieReference
        Dim coef As String
        Dim ref As New CompoundSpecieReference With {
            .ID = value.value,
            .StoiChiometry = 1
        }

        ref.Compartment = value("COMPARTMENT")
        coef = value("COEFFICIENT")

        If Not coef.StringEmpty Then
            If coef = "n" OrElse coef Like "n*" Then
                ref.StoiChiometry = Double.PositiveInfinity
            Else
                ref.StoiChiometry = Double.Parse(coef)
            End If
        End If

        Return ref
    End Function

    Public Function ParseKineticsFactor(value As ValueString) As KineticsFactor
        Return New KineticsFactor With {
           .Km = Double.Parse(value.value),
           .citations = value.getAttributes("CITATIONS"),
           .substrate = value("SUBSTRATE")
       }
    End Function

End Module
