Imports BioCyc.Assembly.MetaCyc.Schema.Metabolism
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes

Public Class reactions : Inherits Model

    <AttributeField("EC-NUMBER")>
    Public Property ec_number As ECNumber
    <AttributeField("ENZYMATIC-REACTION")>
    Public Property enzymaticReaction As String()
    <AttributeField("GIBBS-0")>
    Public Property gibbs0 As Double
    <AttributeField("IN-PATHWAY")>
    Public Property inPathway As String()
    <AttributeField("LEFT")>
    Public Property left As CompoundSpecieReference()
    <AttributeField("RIGHT")>
    Public Property right As CompoundSpecieReference()
    <AttributeField("PHYSIOLOGICALLY-RELEVANT?")>
    Public Property physiologicallyRelevant As Boolean
    <AttributeField("REACTION-BALANCE-STATUS")>
    Public Property reactionBalanceStatus As String
    <AttributeField("REACTION-DIRECTION")>
    Public Property reactionDirection As ReactionDirections
    <AttributeField("REACTION-LIST")>
    Public Property reactionList As String()
    <AttributeField("RXN-LOCATIONS")>
    Public Property reactionLocations As String()
    <AttributeField("SPONTANEOUS?")>
    Public Property spontaneous As Boolean
    <AttributeField("ATOM-MAPPINGS")>
    Public Property atomMappings As String()
    <AttributeField("ORPHAN?")>
    Public Property orphan As String
    <AttributeField("SYSTEMATIC-NAME")>
    Public Property systematicName As String
    <AttributeField("CANNOT-BALANCE?")>
    Public Property cannotBalance As Boolean
    <AttributeField("SIGNAL")>
    Public Property signal As String
    <AttributeField("SPECIES")>
    Public Property species As String

    Public ReadOnly Property equation As Equation
        Get
            Select Case reactionDirection
                Case ReactionDirections.IrreversibleLeftToRight, ReactionDirections.LeftToRight, ReactionDirections.PhysiolLeftToRight
                    Return New Equation With {
                        .Id = MyBase.ToString,
                        .reversible = False,
                        .Reactants = left,
                        .Products = right
                    }
                Case ReactionDirections.Reversible
                    Return New Equation With {
                        .Id = MyBase.ToString,
                        .Reactants = left,
                        .Products = right,
                        .reversible = True
                    }
                Case Else
                    ' right to left
                    Return New Equation With {
                        .Id = MyBase.ToString,
                        .reversible = False,
                        .Reactants = right,
                        .Products = left
                    }
            End Select
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return equation.ToString
    End Function

End Class
