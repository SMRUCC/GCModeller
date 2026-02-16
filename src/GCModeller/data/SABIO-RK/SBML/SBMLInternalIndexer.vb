#Region "Microsoft.VisualBasic::3f837d22bbfd1dce24befb326440cea7, data\SABIO-RK\SBML\SBMLInternalIndexer.vb"

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

'   Total Lines: 171
'    Code Lines: 118 (69.01%)
' Comment Lines: 28 (16.37%)
'    - Xml Docs: 96.43%
' 
'   Blank Lines: 25 (14.62%)
'     File Size: 7.16 KB


'     Class SBMLInternalIndexer
' 
'         Properties: molecules, reactions
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: factorString, getCompartmentName, getEnzymes, getKEGGCompoundId, GetKeggReactionId
'                   getKEGGreactions, getKineticLaw, getSpecies, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.xml.MathML
Imports SMRUCC.genomics.Model.SBML.Level3

Namespace SBML

    ''' <summary>
    ''' helper for get reaction model from sabio-rk <see cref="SbmlDocument"/> dataset
    ''' </summary>
    Public Class SBMLInternalIndexer

        ''' <summary>
        ''' a list of the sub-cellular compartments
        ''' </summary>
        ReadOnly compartments As Dictionary(Of String, compartment)
        ''' <summary>
        ''' a list of the metabolites
        ''' </summary>
        ReadOnly species As Dictionary(Of String, species)
        ''' <summary>
        ''' mapping of the kegg metabolites
        ''' </summary>
        ReadOnly keggCompounds As New Dictionary(Of String, String)
        ReadOnly keggReactions As New Dictionary(Of String, List(Of SBMLReaction))
        ReadOnly formulaLambdas As New Dictionary(Of String, LambdaExpression)

        Public ReadOnly Property reactions As SBMLReaction()
        Public ReadOnly Property molecules As species()
            Get
                Return species.Values.ToArray
            End Get
        End Property

        Sub New(sbml As SbmlDocument)
            Dim entries As String()
            Dim ref As NamedValue(Of String)

            For Each react As SBMLReaction In sbml.sbml.model.listOfReactions.AsEnumerable
                entries = GetKeggReactionId(react).ToArray

                For Each id As String In entries
                    If Not keggReactions.ContainsKey(id) Then
                        keggReactions.Add(id, New List(Of SBMLReaction))
                    End If

                    keggReactions(id).Add(react)
                Next
            Next

            For Each lambda As NamedValue(Of LambdaExpression) In sbml.mathML
                If Not lambda.Value Is Nothing Then
                    formulaLambdas.Add(lambda.Name, lambda.Value)
                End If
            Next

            compartments = sbml.sbml.model.listOfCompartments.ToDictionary(Function(c) c.id)
            species = sbml.sbml.model.listOfSpecies.ToDictionary(Function(c) c.id)
            reactions = sbml.sbml.model.listOfReactions.AsEnumerable.ToArray

            For Each sp In species.Values
                If Not sp.annotation?.RDF?.description.ElementAtOrNull(0)?.is.IsNullOrEmpty Then
                    For Each bag In sp.annotation.RDF.description(0).is
                        For Each li In bag.Bag.list
                            ref = li.getIdentifier

                            If ref.Name = "kegg.compound" Then
                                If keggCompounds.ContainsKey(sp.id) Then
                                    Call $"found duplicated entry: {sp.id} -> {ref.Value}, where {keggCompounds(sp.id)} is already assigned!".PrintException
                                Else
                                    keggCompounds.Add(sp.id, ref.Value)
                                End If
                            End If
                        Next
                    Next
                End If
            Next
        End Sub

        Public Overloads Function ToString(rxn As SBMLReaction, ByRef xrefs As Dictionary(Of String, NamedCollection(Of String))) As String
            Dim left = rxn.listOfReactants.Select(AddressOf factorString).ToArray
            Dim right = rxn.listOfProducts.Select(AddressOf factorString).ToArray

            If xrefs Is Nothing Then
                xrefs = New Dictionary(Of String, NamedCollection(Of String))
            End If

            For Each factor In left.JoinIterates(right)
                ' 20241223 duplicated compounds maybe existsed
                If Not xrefs.ContainsKey(factor.rawId) Then
                    Call xrefs.Add(factor.rawId, New NamedCollection(Of String)(factor.name, factor.xref))
                End If
            Next

            Return $"{left.Select(Function(i) i.Item1).JoinBy(" + ")} -> {right.Select(Function(i) i.Item1).JoinBy(" + ")}"
        End Function

        Private Function factorString(factor As SpeciesReference) As (rawId$, String, name As String, xref As String())
            Dim ref = getSpecies(factor.species)
            Dim reference = ref.annotation?.RDF.description(0).is
            Dim annos As String() = reference _
                .SafeQuery _
                .Select(Function(bag) bag.Bag.list) _
                .IteratesALL _
                .Select(Function(li) li.resource.Split("/"c).Last) _
                .Distinct _
                .ToArray
            Dim i As String = If(factor.stoichiometry <= 1, ref.name, factor.stoichiometry & " " & ref.name)

            Return (factor.species, i, ref.name, annos)
        End Function

        ''' <summary>
        ''' try to get kegg reaction id from the given sabio-rk reaction model
        ''' </summary>
        ''' <param name="react"></param>
        ''' <returns></returns>
        Public Shared Iterator Function GetKeggReactionId(react As SBMLReaction) As IEnumerable(Of String)
            For Each id In react.getIdentifiers.Where(Function(r) r.Name = "kegg.reaction")
                Yield id.Value
            Next
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function getCompartmentName(id As String) As String
            Return compartments(id).name
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function getKEGGCompoundId(speciesId As String) As String
            Return keggCompounds.TryGetValue(speciesId)
        End Function

        ''' <summary>
        ''' 获取得到反应过程对应的动力学函数
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        Public Function getKineticLaw(id As String) As LambdaExpression
            If formulaLambdas.ContainsKey(id) Then
                Return formulaLambdas(id)
            Else
                Return Nothing
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function getKEGGreactions(id As String) As IEnumerable(Of SBMLReaction)
            Return keggReactions.TryGetValue(id)
        End Function

        ''' <summary>
        ''' try to get enzyme molecules from a given reaction model
        ''' </summary>
        ''' <param name="react"></param>
        ''' <returns></returns>
        Public Iterator Function getEnzymes(react As SBMLReaction) As IEnumerable(Of species)
            If Not react.listOfModifiers.IsNullOrEmpty Then
                For Each modifier In react.listOfModifiers
                    Yield species(modifier.species)
                Next
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function getSpecies(id As String) As species
            Return species.TryGetValue(id)
        End Function
    End Class
End Namespace
