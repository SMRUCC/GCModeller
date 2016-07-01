Imports System.Xml.Serialization
Imports SMRUCC.genomics.Assembly.SBML
Imports SMRUCC.genomics.Assembly.SBML.Level2.Elements
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Extensions

Namespace FBACompatibility

    Public Class MetabolismFlux : Implements FLuxBalanceModel.I_ReactionModel(Of speciesReference)
        <XmlAttribute> Public Property Identifier As String Implements FLuxBalanceModel.I_ReactionModel(Of speciesReference).Identifier
        <XmlAttribute> Public Property UPPER_BOUND As Double
        <XmlAttribute> Public Property LOWER_BOUND As Double
        <XmlAttribute> Public Property ObjectiveCoefficient As Double

        Public Overrides Function ToString() As String
            Return String.Format("{0}  <- {1}, {2} ->", Identifier, LOWER_BOUND, UPPER_BOUND)
        End Function

        Public Function GetStoichiometry(Metabolite As String) As Double Implements FLuxBalanceModel.I_ReactionModel(Of speciesReference).GetStoichiometry
            Throw New NotImplementedException("This is a not necessary method")
        End Function

        Public ReadOnly Property Get_LOWER As Double Implements FLuxBalanceModel.I_ReactionModel(Of speciesReference).LOWER_BOUND
            Get
                Return LOWER_BOUND
            End Get
        End Property

        Public Property Name As String Implements FLuxBalanceModel.I_ReactionModel(Of speciesReference).Name

        Public ReadOnly Property GetObjCoefficient As Integer Implements FLuxBalanceModel.I_ReactionModel(Of speciesReference).ObjectiveCoefficient
            Get
                Return ObjectiveCoefficient
            End Get
        End Property

        Public ReadOnly Property Get_UPPER As Double Implements FLuxBalanceModel.I_ReactionModel(Of speciesReference).UPPER_BOUND
            Get
                Return UPPER_BOUND
            End Get
        End Property

        Public Shared Function Convert(Flux As FLuxBalanceModel.I_ReactionModel(Of speciesReference)) As MetabolismFlux
            Return New FBACompatibility.MetabolismFlux With {
                .Identifier = Flux.Identifier,
                .LOWER_BOUND = Flux.LOWER_BOUND,
                .UPPER_BOUND = Flux.UPPER_BOUND,
                .Name = Flux.Name,
                .ObjectiveCoefficient = Flux.ObjectiveCoefficient
            }
        End Function

        ''' <summary>
        ''' Not implement null reference property.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlIgnore>
        Public Property Products As speciesReference() Implements FLuxBalanceModel.I_ReactionModel(Of speciesReference).Products

        ''' <summary>
        ''' Not implement null reference property.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlIgnore>
        Public Property Reactants As speciesReference() Implements FLuxBalanceModel.I_ReactionModel(Of speciesReference).Reactants

        Public Property Reversible As Boolean Implements IEquation(Of speciesReference).Reversible
    End Class
End Namespace