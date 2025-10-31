#Region "Microsoft.VisualBasic::b4a45c5fa70c8dcae2afabf2cd667e89, engine\IO\GCMarkupLanguage\v2\Xml\MetabolismStructure.vb"

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

    '   Total Lines: 419
    '    Code Lines: 246 (58.71%)
    ' Comment Lines: 101 (24.11%)
    '    - Xml Docs: 97.03%
    ' 
    '   Blank Lines: 72 (17.18%)
    '     File Size: 14.52 KB


    '     Class MetabolismStructure
    ' 
    '         Properties: compounds, enzymes, maps, reactions
    ' 
    '         Function: FindByKEGG, GetAllFluxID, GetKEGGMapping
    ' 
    '     Class ReactionGroup
    ' 
    '         Properties: enzymatic, etc, size
    ' 
    '         Function: CompoundLinks, GenericEnumerator
    ' 
    '     Class Compound
    ' 
    '         Properties: ID, kegg_id, mass0, name
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ToString
    ' 
    '     Class Reaction
    ' 
    '         Properties: bounds, ec_number, equation, ID, is_enzymatic
    '                     name, note, product, substrate
    ' 
    '         Function: GenericEnumerator, ToString
    ' 
    '     Class CompoundFactor
    ' 
    '         Properties: compartment, compound, factor
    ' 
    '         Constructor: (+3 Overloads) Sub New
    '         Function: factorString, ToString
    ' 
    '     Class FunctionalCategory
    ' 
    '         Properties: category, pathways
    ' 
    '         Function: ToString
    ' 
    '     Class Pathway
    ' 
    '         Properties: enzymes, ID, name
    ' 
    '         Function: ToString
    ' 
    '     Class Enzyme
    ' 
    '         Properties: catalysis, ECNumber, geneID, KO
    ' 
    '         Function: ToString
    ' 
    '     Class Catalysis
    ' 
    '         Properties: formula, parameter, PH, reaction, temperature
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ToString
    ' 
    '     Class KineticsParameter
    ' 
    '         Properties: isModifier, name, target, value
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace v2

    ''' <summary>
    ''' model of the metabolic network
    ''' </summary>
    <XmlType("metabolome", [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class MetabolismStructure

        <XmlArray("compounds")> Public Property compounds As Compound()

        ''' <summary>
        ''' the metabolic network inside this cellular model, includes all enzymatic reaction and non-enzymatic reaction.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 在这个属性之中包含有所有的代谢反应过程的定义
        ''' </remarks>
        <XmlElement("reactions")> Public Property reactions As ReactionGroup

        ''' <summary>
        ''' 在这个属性里面只会出现具有KO分类编号的蛋白序列，如果需要找所有基因的数据，可以
        ''' 读取<see cref="Genome.replicons"/>里面的基因的数据
        ''' </summary>
        ''' <returns></returns>
        <XmlArray("enzymes")> Public Property enzymes As Enzyme()

        ''' <summary>
        ''' the pathway function category groups of the enzymes
        ''' </summary>
        ''' <returns></returns>
        <XmlArray("pathwayMaps")>
        Public Property maps As FunctionalCategory()

        Sub New()
        End Sub

        Sub New(copy As MetabolismStructure)
            compounds = copy.compounds.SafeQuery.ToArray
            enzymes = copy.enzymes.SafeQuery.ToArray
            maps = copy.maps.SafeQuery.ToArray
            reactions = New ReactionGroup(copy.reactions)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetAllFluxID() As String()
            Return reactions _
                .SafeQuery _
                .Select(Function(r) r.ID) _
                .ToArray
        End Function

        Dim m_refs As Dictionary(Of String, Compound())

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="protein_id"></param>
        ''' <returns></returns>
        Public Iterator Function GetImpactedMetabolicNetwork(protein_id As String) As IEnumerable(Of Reaction)
            Dim enzymeList = enzymes _
                .Where(Function(enz)
                           Return enz.proteinID = protein_id AndAlso
                              Not enz.catalysis.IsNullOrEmpty
                       End Function) _
                .ToArray
            Dim fluxID As String() = enzymeList _
                .Select(Function(enz)
                            Return enz.catalysis.Select(Function(c) c.reaction)
                        End Function) _
                .IteratesALL _
                .Distinct _
                .ToArray

            For Each id As String In fluxID
                Yield _reactions(id)
            Next
        End Function

        Public Function FindByReference(id As String) As Compound()
            If m_refs Is Nothing Then
                m_refs = compounds.SafeQuery _
                    .Where(Function(c) Not c.referenceIds.IsNullOrEmpty) _
                    .Select(Function(c)
                                Return c.referenceIds.Select(Function(r_id) (r_id, c))
                            End Function) _
                    .IteratesALL _
                    .GroupBy(Function(c) c.r_id) _
                    .ToDictionary(Function(c) c.Key,
                                  Function(c)
                                      Return c.Select(Function(ci) ci.c).ToArray
                                  End Function)
            End If

            Return m_refs.TryGetValue(id)
        End Function

        Public Function GetReferMapping(id As String, map_define As String, links As Dictionary(Of String, Reaction())) As Compound
            Dim refs As Compound() = FindByReference(id)

            If refs.IsNullOrEmpty Then
                Call ($"no mapping for reference term '{map_define}'({id})!").Warning
                Return New Compound(id, map_define)
            Else
                Return refs _
                    .OrderByDescending(Function(c) links(c.ID).Length) _
                    .First
            End If
        End Function
    End Class

End Namespace
