Imports Microsoft.VisualBasic

Namespace Assembly.MetaCyc.Schema.Metabolism

    Public Class Compound : Inherits MetaCyc.File.DataFiles.Slots.Object
        Implements ComponentModel.EquaionModel.ICompoundSpecies

        Public Property StoiChiometry As Double Implements ComponentModel.EquaionModel.ICompoundSpecies.StoiChiometry
    End Class

    Public Class Reaction : Inherits MetaCyc.File.DataFiles.Slots.Object
        Implements ComponentModel.EquaionModel.IEquation(Of Compound)

        Protected Friend _InnerBaseType As MetaCyc.File.DataFiles.Slots.Reaction
        Protected Friend _strEquation As String

        Public ReadOnly Property Equation As String
            Get
                Return _strEquation
            End Get
        End Property

        Public Property Reactants As Compound() Implements ComponentModel.EquaionModel.IEquation(Of Compound).Reactants
        Public Property Reversible As Boolean Implements ComponentModel.EquaionModel.IEquation(Of Compound).Reversible
        Public Property Products As Compound() Implements ComponentModel.EquaionModel.IEquation(Of Compound).Products

        Public Shared Function CreateObject(FileObject As MetaCyc.File.DataFiles.Slots.Reaction) As Reaction
            Dim SchemaModel As Reaction = New Reaction With {._InnerBaseType = FileObject}
            Call FileObject.CopyTo(Of Reaction)(SchemaModel)
            SchemaModel.Reversible = String.Equals(FileObject.ReactionDirection, "REVERSIBLE")
            SchemaModel.Reactants = (From Id As String In FileObject.Left Select New Compound With {.Identifier = Id, .StoiChiometry = 1}).ToArray
            SchemaModel.Products = (From Id As String In FileObject.Right Select New Compound With {.Identifier = Id, .StoiChiometry = 1}).ToArray
            SchemaModel._strEquation = ComponentModel.EquaionModel.ToString(Of Compound)(SchemaModel)

            Return SchemaModel
        End Function

        Public Shared Function [DirectCast](Reactions As MetaCyc.File.DataFiles.Reactions) As Reaction()
            Dim LQuery = (From item In Reactions.AsParallel Select CreateObject(item)).ToArray
            Call Console.WriteLine("Complete object type cast!")
            Return LQuery
        End Function
    End Class
End Namespace
