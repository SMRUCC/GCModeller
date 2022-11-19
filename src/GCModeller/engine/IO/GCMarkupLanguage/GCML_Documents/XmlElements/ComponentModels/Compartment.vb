#Region "Microsoft.VisualBasic::83f3c9e6f9da3cc4d540dac226e97acb, GCModeller\engine\IO\GCMarkupLanguage\GCML_Documents\XmlElements\ComponentModels\Compartment.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 64
    '    Code Lines: 47
    ' Comment Lines: 6
    '   Blank Lines: 11
    '     File Size: 2.57 KB


    '     Class Compartment
    ' 
    '         Function: CastTo
    ' 
    '     Class CompoundSpeciesReference
    ' 
    '         Properties: CompartmentId, Identifier, StoiChiometry
    ' 
    '         Function: CreateObject, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports SMRUCC.genomics.Model
Imports SMRUCC.genomics.Model.SBML.Level2.Elements

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
        <XmlAttribute> Public Property Identifier As String Implements INamedValue.Key
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
