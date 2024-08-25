﻿#Region "Microsoft.VisualBasic::ae8f7399e5cd44a3b4c4deb1d1f7aadc, models\SBML\SBML\Level3\XmlFile.vb"

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

    '   Total Lines: 101
    '    Code Lines: 64 (63.37%)
    ' Comment Lines: 16 (15.84%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 21 (20.79%)
    '     File Size: 3.57 KB


    '     Class XmlFile
    ' 
    '         Properties: level, model, version
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: LoadDocument, ToString
    ' 
    '     Class Model
    ' 
    '         Properties: listOfCompartments, listOfFunctionDefinitions, listOfReactions, listOfSpecies, listOfUnitDefinitions
    '                     notes
    ' 
    '     Class functionDefinition
    ' 
    '         Properties: expression, math, sboTerm
    ' 
    '         Function: ToString
    ' 
    '     Class compartment
    ' 
    '         Properties: annotation, Constant, sboTerm, Size
    ' 
    '     Class species
    ' 
    '         Properties: annotation, constant, hasOnlySubstanceUnits, initialConcentration, metaid
    '                     notes
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.rdf_xml
Imports Microsoft.VisualBasic.MIME.application.xml
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder
Imports SMRUCC.genomics.Model.SBML.Components

Namespace Level3

    ''' <summary>
    ''' A generic sbml document file model
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    <XmlRoot("sbml", Namespace:="http://www.sbml.org/sbml/level3/version1/core")>
    Public Class XmlFile(Of T As Reaction)

        <XmlAttribute("level")> Public Property level As Integer
        <XmlAttribute("version")> Public Property version As Integer
        <XmlElement("model")> Public Property model As Model(Of T)

        <XmlNamespaceDeclarations()>
        Public xmlns As New XmlSerializerNamespaces

        Sub New()
            xmlns.Add("rdf", RDFEntity.XmlnsNamespace)
        End Sub

        Public Overrides Function ToString() As String
            Return model.ToString
        End Function

        Public Shared Function LoadDocument(xml As String) As XmlFile(Of T)
            Return xml.LoadXml(Of XmlFile(Of T))
        End Function
    End Class

    <XmlType("model", Namespace:=sbmlXmlns)>
    Public Class Model(Of T As Reaction) : Inherits ModelBase

        <XmlElement("notes")> Public Property notes As Notes

        ''' <summary>
        ''' subcellular location list
        ''' </summary>
        ''' <returns></returns>
        Public Property listOfCompartments As compartment()
        ''' <summary>
        ''' compound and metabolite list
        ''' </summary>
        ''' <returns></returns>
        Public Property listOfSpecies As species()
        ''' <summary>
        ''' reaction list
        ''' </summary>
        ''' <returns></returns>
        Public Property listOfReactions As reactionList(Of T)
        Public Property listOfUnitDefinitions As unitDefinition()
        ''' <summary>
        ''' the enzyme kinetics list
        ''' </summary>
        ''' <returns></returns>
        Public Property listOfFunctionDefinitions As functionDefinition()

    End Class

    Public Class reactionList(Of T As Reaction) : Implements Enumeration(Of T)

        <XmlElement("reaction")>
        Public Property reactions As T()

        Public Iterator Function GenericEnumerator() As IEnumerator(Of T) Implements Enumeration(Of T).GenericEnumerator
            For Each rxn As T In reactions.SafeQuery
                Yield rxn
            Next
        End Function
    End Class

    Public Class functionDefinition : Inherits IPartsBase

        <XmlElement("math", [Namespace]:="http://www.w3.org/1998/Math/MathML")>
        Public Property math As MathML.Math
        <XmlAttribute>
        Public Property sboTerm As String

        Public ReadOnly Property expression As String
            Get
                Dim args = math.lambda.bvar.JoinBy(", ")
                Dim exp = MathML.Math.BuildExpressionString(math.lambda.apply)
                Return $"{args} => {exp}"
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{id}: {math}"
        End Function

    End Class

    <XmlType("compartment", Namespace:="http://www.sbml.org/sbml/level3/version1/core")>
    Public Class compartment : Inherits Components.Compartment

        <XmlAttribute("size")> Public Property Size As Integer
        <XmlAttribute("constant")> Public Property Constant As Boolean
        <XmlAttribute> Public Property sboTerm As String

        Public Property annotation As annotation

    End Class

    ''' <summary>
    ''' Compound molecule model
    ''' </summary>
    <XmlType("species", Namespace:=sbmlXmlns)>
    Public Class species : Inherits Components.Specie

        <XmlAttribute> Public Property hasOnlySubstanceUnits As Boolean
        <XmlAttribute> Public Property constant As Boolean
        <XmlAttribute> Public Property metaid As String
        <XmlAttribute> Public Property initialConcentration As Double
        Public Property notes As Notes
        Public Property annotation As annotation

        ''' <summary>
        ''' get external db_xrefs of current compound molecule
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property db_xrefs As DBLink()
            Get
                If annotation Is Nothing Then Return Nothing
                If annotation.RDF Is Nothing OrElse annotation.RDF.description.IsNullOrEmpty Then Return Nothing

                Return annotation.RDF.description _
                    .Select(Function(d)
                                If d Is Nothing OrElse d.is.IsNullOrEmpty Then
                                    Return Nothing
                                End If

                                Return d.is _
                                    .Select(Function(a) a.AsEnumerable) _
                                    .IteratesALL
                            End Function) _
                    .IteratesALL _
                    .Select(Function(url)
                                Dim tokens = url.Split("/"c)
                                Dim name = tokens(tokens.Length - 2)
                                Dim id = tokens(tokens.Length - 1)

                                Return New DBLink(name, id)
                            End Function) _
                    .ToArray
            End Get
        End Property

    End Class
End Namespace
