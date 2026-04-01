Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.TagData

Namespace ComponentModel.EquaionModel.DefaultTypes

    ''' <summary>
    ''' the compound model reference.
    ''' </summary>
    Public Class CompoundSpecieReference : Implements ICompoundSpecies

        ''' <summary>
        ''' 化学计量数
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Stoichiometry As Double Implements ICompoundSpecies.Stoichiometry
        <XmlText> Public Property ID As String Implements ICompoundSpecies.Key
        <XmlAttribute> Public Property Compartment As String

        Sub New()
        End Sub

        Sub New(ref As ICompoundSpecies)
            Stoichiometry = ref.Stoichiometry
            ID = ref.Key
        End Sub

        Sub New(factor As Double, compound As String)
            Stoichiometry = factor
            ID = compound
        End Sub

        Sub New(factor As Double, compound As String, compart As String)
            Me.Stoichiometry = factor
            Me.ID = compound
            Me.Compartment = compart
        End Sub

        Public Overloads Function Equals(b As ICompoundSpecies, strict As Boolean) As Boolean
            Return Equivalence.Equals(Me, b, strict)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function AsFactor() As FactorString(Of Double)
            Return New FactorString(Of Double) With {
                .factor = Stoichiometry,
                .result = ID
            }
        End Function

        Public Overrides Function ToString() As String
            If Stoichiometry > 1 Then
                Return String.Format("{0} {1}", Stoichiometry, ID)
            Else
                Return ID
            End If
        End Function
    End Class

End Namespace