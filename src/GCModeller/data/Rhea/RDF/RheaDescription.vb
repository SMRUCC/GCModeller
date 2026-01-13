#Region "Microsoft.VisualBasic::f6b57ad0edb1f2657b4aa17ce8b53d91, data\Rhea\RDF\RheaDescription.vb"

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
    '    Code Lines: 80 (79.21%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 21 (20.79%)
    '     File Size: 3.83 KB


    ' Class RheaDescription
    ' 
    '     Properties: accession, bidirectionalReaction, chebi, compound, contains
    '                 contains1, directionalReaction, ec, equation, formula
    '                 id, isTransport, name, products, seeAlso
    '                 status, subClassOf, substrates, substratesOrProducts
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: GetClassType, GetCompound, GetDbXrefs, GetECNumber
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.MIME.application.rdf_xml
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.EquaionModel

<XmlType("Description", [Namespace]:=RheaRDF.rh)>
Public Class RheaDescription : Inherits Description

    <XmlElement("subClassOf", [Namespace]:=RDFEntity.rdfs)>
    Public Property subClassOf As Resource()
    <XmlElement("id", [Namespace]:=RheaRDF.rh)> Public Property id As RDFProperty
    <XmlElement("accession", [Namespace]:=RheaRDF.rh)> Public Property accession As String
    <XmlElement("name", [Namespace]:=RheaRDF.rh)> Public Property name As String
    <XmlElement("formula", [Namespace]:=RheaRDF.rh)> Public Property formula As String
    <XmlElement("chebi", [Namespace]:=RheaRDF.rh)> Public Property chebi As String
    <XmlElement("equation", [Namespace]:=RheaRDF.rh)> Public Property equation As String
    <XmlElement("htmlEquation", [Namespace]:=RheaRDF.rh)> Public Property htmlEquation As String
    <XmlElement("status", [Namespace]:=RheaRDF.rh)> Public Property status As Resource
    <XmlElement("ec", [Namespace]:=RheaRDF.rh)> Public Property ec As Resource()

    <XmlElement("directionalReaction", [Namespace]:=RheaRDF.rh)>
    Public Property directionalReaction As Resource()

    <XmlElement("bidirectionalReaction", [Namespace]:=RheaRDF.rh)>
    Public Property bidirectionalReaction As Resource()

    <XmlElement("compound", [Namespace]:=RheaRDF.rh)>
    Public Property compound As Resource
    <XmlElement("contains", [Namespace]:=RheaRDF.rh)> Public Property contains As Resource
    <XmlElement("contains1", [Namespace]:=RheaRDF.rh)> Public Property contains1 As Resource

    <XmlElement("substrates", [Namespace]:=RheaRDF.rh)> Public Property substrates As Resource()
    <XmlElement("products", [Namespace]:=RheaRDF.rh)> Public Property products As Resource()

    <XmlElement("substratesOrProducts", [Namespace]:=RheaRDF.rh)>
    Public Property substratesOrProducts As Resource()

    <XmlElement("seeAlso", [Namespace]:=RDFEntity.rdfs)>
    Public Property seeAlso As Resource()
    <XmlElement("isTransport", [Namespace]:=RheaRDF.rh)> Public Property isTransport As RDFProperty

    Sub New()
        Call MyBase.New()
    End Sub

    Public Function GetClassType() As String
        If subClassOf Is Nothing Then
            Return ""
        End If

        Static i32 As New Regex("\d+")

        For Each subclassOf As Resource In Me.subClassOf
            If subclassOf.resource.StartsWith("http://rdf.rhea-db.org/") Then
                Dim cls = subclassOf.resource.Replace("http://rdf.rhea-db.org/", "").Trim(" "c, "/"c)

                If Not i32.Match(cls).Success Then
                    Return cls
                End If
            End If
        Next

        Return ""
    End Function

    Public Iterator Function GetECNumber() As IEnumerable(Of String)
        If ec Is Nothing Then
            Return
        End If

        For Each num As Resource In ec
            Dim m As Match = ECNumber.r.Match(num.resource)

            If m.Success Then
                Yield m.Value
            End If
        Next
    End Function

    Public Iterator Function GetDbXrefs() As IEnumerable(Of NamedValue)
        If seeAlso Is Nothing Then
            Return
        End If

        For Each xref In seeAlso
            Dim tokens = xref.resource.Split("/"c)
            Dim id = tokens(tokens.Length - 1)
            Dim dbname = tokens(tokens.Length - 2)

            Yield New NamedValue(dbname, id)
        Next
    End Function

    Public Function GetCompound() As CompoundSpecies
        Return New CompoundSpecies With {
            .entry = accession,
            .name = name,
            .formula = formula
        }
    End Function
End Class
