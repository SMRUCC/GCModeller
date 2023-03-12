Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes

Namespace MetabolicModel

    ''' <summary>
    ''' A unify reaction model in the GCModeller system
    ''' </summary>
    Public Class MetabolicReaction : Implements INamedValue
        Implements IEquation(Of CompoundSpecieReference)

        Public Property id As String Implements IKeyedEntity(Of String).Key
        Public Property name As String
        Public Property description As String
        Public Property left As CompoundSpecieReference() Implements IEquation(Of CompoundSpecieReference).Reactants
        Public Property right As CompoundSpecieReference() Implements IEquation(Of CompoundSpecieReference).Products

        ''' <summary>
        ''' if not is reversible, then the reaction direction is left to right by default
        ''' </summary>
        ''' <returns></returns>
        Public Property is_reversible As Boolean Implements IEquation(Of CompoundSpecieReference).Reversible
        ''' <summary>
        ''' could be react no required of the enzymatic
        ''' </summary>
        ''' <returns></returns>
        Public Property is_spontaneous As Boolean
        Public Property ECNumbers As String()

        Public Overrides Function ToString() As String
            Return $"[{id}] {name}"
        End Function

    End Class
End Namespace