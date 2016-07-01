Imports System.Xml.Serialization
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.SBML.Level2.Elements
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace GCML_Documents.ComponentModels

    <XmlType("CompartmentObject", Namespace:="http://code.google.com/p/genome-in-code/GCMarkupLanguage/component_models")>
    Public Class Compartment : Inherits SBML.Components.Compartment

        Public Shared Function CastTo(c As SBML.Components.Compartment) As Compartment
            Return New Compartment With {
                .id = c.id,
                .name = c.name
            }
        End Function
    End Class

    <XmlType("CompoundSpecies-Refx", Namespace:="http://code.google.com/p/genome-in-code/GCMarkupLanguage/component_models")>
    Public Class CompoundSpeciesReference : Implements ICompoundSpecies

        Protected _ObjectBaseType As speciesReference = New speciesReference()

        ''' <summary>
        ''' Compound species unique-id
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Identifier As String Implements sIdEnumerable.Identifier
            Get
                Return _ObjectBaseType.species
            End Get
            Set(value As String)
                _ObjectBaseType.species = value
            End Set
        End Property

        <XmlAttribute> Public Property StoiChiometry As Double Implements ICompoundSpecies.StoiChiometry
            Get
                Return _ObjectBaseType.stoichiometry
            End Get
            Set(value As Double)
                _ObjectBaseType.stoichiometry = value
            End Set
        End Property

        <XmlElement("CompartmentObject_ID", Namespace:="http://code.google.com/p/genome-in-code/GCMarkupLanguage/component_models/CompartmentObject")>
        Public Property CompartmentId As String

        Public Overrides Function ToString() As String
            Return String.Format("{0} {1} [{2}]", StoiChiometry, Identifier, CompartmentId)
        End Function

        Public Shared Narrowing Operator CType(CompoundSpeciesReference As CompoundSpeciesReference) As speciesReference
            Return CompoundSpeciesReference._ObjectBaseType
        End Operator

        Public Shared Function CreateObject(obj As speciesReference) As CompoundSpeciesReference
            Return New CompoundSpeciesReference With {._ObjectBaseType = obj}
        End Function
    End Class
End Namespace