#Region "Microsoft.VisualBasic::2f6d9f1b241bece95b7fd2dc4d3550c4, data\Rhea\RDF\RheaRDF.vb"

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

    '   Total Lines: 152
    '    Code Lines: 120 (78.95%)
    ' Comment Lines: 13 (8.55%)
    '    - Xml Docs: 38.46%
    ' 
    '   Blank Lines: 19 (12.50%)
    '     File Size: 5.99 KB


    ' Class RheaRDF
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: GetBidirectionalReaction, GetCompounds, GetDirectionalReaction, GetReactions, getSideCompounds
    '               Load
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.rdf_xml
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes

''' <summary>
''' 
''' </summary>
''' <remarks>
''' https://ftp.expasy.org/databases/rhea/rdf/rhea.rdf.gz
''' </remarks>
''' 
<XmlRoot("RDF", [Namespace]:=RDFEntity.XmlnsNamespace)>
Public Class RheaRDF : Inherits RDF(Of RheaDescription)

    Public Const rh As String = "http://rdf.rhea-db.org/"

    Sub New()
        Call MyBase.New()
        Call xmlns.Add("rh", rh)
    End Sub

    Public Iterator Function GetReactions() As IEnumerable(Of Reaction)
        Dim groups = description _
            .SafeQuery _
            .GroupBy(Function(res) res.GetClassType) _
            .ToDictionary(Function(res) res.Key,
                          Function(res)
                              Return res.ToArray
                          End Function)
        Dim objs = description.GroupBy(Function(o) o.about) _
            .ToDictionary(Function(o) o.Key,
                          Function(o)
                              Return o.ToArray
                          End Function)
        Dim reaction As New Value(Of Reaction)

        For Each r As RheaDescription In groups!Reaction
            Dim ecnumber As String() = r.GetECNumber.ToArray

            If Not r.directionalReaction Is Nothing Then
                For Each dr In r.directionalReaction
                    If reaction = GetDirectionalReaction(objs(dr.resource).First, ecnumber, objs) IsNot Nothing Then
                        Yield CType(reaction, Reaction)
                    End If
                Next
            End If
            If Not r.bidirectionalReaction Is Nothing Then
                For Each br In r.bidirectionalReaction
                    If reaction = GetBidirectionalReaction(objs(br.resource).First, ecnumber, objs) IsNot Nothing Then
                        Yield CType(reaction, Reaction)
                    End If
                Next
            End If
        Next
    End Function

    Private Function GetBidirectionalReaction(r As RheaDescription, ec As String(), objs As Dictionary(Of String, RheaDescription())) As Reaction
        If r.substratesOrProducts.IsNullOrEmpty Then
            ' obsolete reaction
            ' The reaction has been replaced by RHEA:xxxxx
            ' just ignores
            Return Nothing
        End If

        Dim compounds = getSideCompounds(r.substratesOrProducts, "*", objs)

        Return New Reaction With {
            .definition = r.equation,
            .entry = r.accession,
            .enzyme = ec,
            .equation = Equation.TryParse(.definition),
            .compounds = compounds,
            .isTransport = r.isTransport
        }
    End Function

    Private Function GetDirectionalReaction(r As RheaDescription, ec As String(), objs As Dictionary(Of String, RheaDescription())) As Reaction
        ' obsolete reaction
        ' The reaction has been replaced by RHEA:xxxxx
        ' just ignores
        If r.substrates Is Nothing AndAlso r.products Is Nothing Then
            Return Nothing
        End If

        Dim left = getSideCompounds(r.substrates, "substrate", objs)
        Dim right = getSideCompounds(r.products, "product", objs)

        Return New Reaction With {
            .definition = r.equation,
            .entry = r.accession,
            .enzyme = ec,
            .equation = Equation.TryParse(.definition),
            .compounds = left.JoinIterates(right).ToArray,
            .isTransport = r.isTransport,
            .comment = r.comment,
            .db_xrefs = r.GetDbXrefs.ToArray
        }
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Shared Function getSideCompounds(refs As Resource(), side As String, objs As Dictionary(Of String, RheaDescription())) As SideCompound()
        Return refs _
            .Select(Function(c)
                        Return GetCompounds(c, objs).Select(Function(ci) New SideCompound(side, ci))
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(c) c.compound.entry) _
            .Select(Function(c) c.First) _
            .ToArray
    End Function

    Shared ReadOnly compoundType As Index(Of String) = {"Compound", "GenericCompound", "SmallMolecule",
            "GenericPolypeptide", "GenericPolynucleotide", "GenericHeteropolysaccharide",
            "GenericSmallMolecule", "Polymer"}

    Private Shared Iterator Function GetCompounds(ref As Resource, objs As Dictionary(Of String, RheaDescription())) As IEnumerable(Of CompoundSpecies)
        Dim links = objs(ref.resource)

        If links.Length = 1 AndAlso links(0).GetClassType Like compoundType Then
            Yield links(0).GetCompound
            Return
        End If

        For Each link As RheaDescription In links
            If link.contains IsNot Nothing Then
                For Each compound In GetCompounds(link.contains, objs)
                    Yield compound
                Next
            End If
            If link.contains1 IsNot Nothing Then
                For Each compound In GetCompounds(link.contains1, objs)
                    Yield compound
                Next
            End If
            If Not link.compound Is Nothing Then
                For Each compound In GetCompounds(link.compound, objs)
                    Yield compound
                Next
            End If
        Next
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function Load(doc As String) As RheaRDF
        Return doc.SolveStream.LoadFromXml(Of RheaRDF)
    End Function

End Class
