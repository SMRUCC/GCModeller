Imports System.Xml.Serialization
Imports SMRUCC.genomics.ComponentModel.EquaionModel

Namespace v2

    Public Class CompoundFactor : Implements ICompoundSpecies

        <XmlAttribute>
        Public Property factor As Double Implements ICompoundSpecies.Stoichiometry
        <XmlText>
        Public Property compound As String Implements ICompoundSpecies.Key
        <XmlAttribute>
        Public Property compartment As String

        <XmlAttribute> Public Property cid As UInteger

        Sub New()
        End Sub

        Sub New(factor As Double, compound As String, Optional compartment As String = Nothing)
            Me.compartment = compartment
            Me.factor = factor
            Me.compound = compound
        End Sub

        Sub New(compound As String, factor As Double, Optional compartment As String = Nothing)
            Me.factor = factor
            Me.compound = compound
            Me.compartment = compartment
        End Sub

        Public Overrides Function ToString() As String
            Return $"[{compartment}]" & compound
        End Function

        Friend Function factorString() As String
            If factor <= 1 Then
                Return compound
            Else
                Return factor & " " & compound
            End If
        End Function

    End Class
End Namespace