#Region "Microsoft.VisualBasic::b29f96c37cbb5bee5b50ece1e30899f5, models\BioCyc\Models\pathways.vb"

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

    '   Total Lines: 41
    '    Code Lines: 27 (65.85%)
    ' Comment Lines: 4 (9.76%)
    '    - Xml Docs: 75.00%
    ' 
    '   Blank Lines: 10 (24.39%)
    '     File Size: 1.21 KB


    ' Class pathways
    ' 
    '     Properties: enzymesNotUsed, inPathway, pathwayLinks, predecessors, primaries
    '                 reactionLayout, reactionList, subPathways, superPathways
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

''' <summary>
''' Pathways, including relationships among reactions
''' </summary>
''' 
<Xref("pathways.dat")>
Public Class pathways : Inherits Model

    <AttributeField("IN-PATHWAY")>
    Public Property inPathway As String()
    ''' <summary>
    ''' This slot lists direct super-pathways of a pathway.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <AttributeField("SUPER-PATHWAYS")>
    Public Property superPathways As String()
    ''' <summary>
    ''' This slot is the inverse of the Super-Pathways slot. It lists all the direct subpathways of a
    ''' pathway.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <AttributeField("SUB-PATHWAYS")>
    Public Property subPathways As String()
    ''' <summary>
    ''' This slot indicates linkages among pathways in pathway drawings. Each value of this slot
    ''' is a list of the form (cpd other-pwy*). The Navigator draws an arrow from the specified
    ''' compound pointing to the names of the specified pathways, to note that the compound
    ''' is also a substrate in those other pathways. If no other pathways are specified, then links
    ''' are drawn to and from all other pathways that the compound is in (i.e., if the compound
    ''' is produced by the current pathway, then links are drawn to all other pathways that consume
    ''' it, and vice versa).
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <AttributeField("PATHWAY-LINKS")>
    Public Property pathwayLinks As String()

    ''' <summary>
    ''' This slot describes the linked reactions that compose the current pathway. Since pathways
    ''' have a variety of topologies — from linear to circular to tree structured — pathways
    ''' cannot be represented as simple sequences of reactions. A pathway is a list of reaction/
    ''' predecessor pairs.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <AttributeField("PREDECESSORS")>
    Public Property predecessors As String()
    ''' <summary>
    ''' When drawing a pathway, the Navigator software usually computes automatically which
    ''' compounds are primaries (mains) and which compounds are secondaries (sides). Occasionally,
    ''' the heuristics used are not sufficient to make the correct distinction, in which
    ''' case you can specify primary compounds explicitly. This slot can contain the list of primary
    ''' reactants, primary products, or both for a particular reaction in the pathway. Each
    ''' value for this slot is of the form (reaction-ID (primary-reactant-ID-1 ... primary-reactant-
    ''' ID-n) (primary-product-ID-1 ... primary-product-ID-n)), where an empty list in either
    ''' the reactant or product position means that that information is not supplied and should
    ''' be computed. An empty list in the product position can also be omitted completely.
    ''' For example, in the purine synthesis pathway, we want to specify that the primary product
    ''' for the final reaction in the pathway should be AMP and not fumarate. The primary
    ''' reactants are still computed. The corresponding slot value would be
    ''' primaries: (ampsyn-rxn () (amp))
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <AttributeField("PRIMARIES")>
    Public Property primaries As String()

    <AttributeField("REACTION-LAYOUT")>
    Public Property reactionLayout As String()
    ''' <summary>
    ''' This slot lists all reactions in the current pathway, in no particular order.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <AttributeField("REACTION-LIST")>
    Public Property reactionList As String()
    ''' <summary>
    ''' Proteins or protein-RNA complexes listed in this slot are those which would otherwise
    ''' have been inferred to take part in the pathway or reaction, but which in reality do not.
    ''' The protein may catalyze a reaction of the pathway in other circumstances, but not as
    ''' part of the pathway (e.g. it may be not be in the same cellular compartment as the other
    ''' components of the pathway, or it may not be expressed in situations when the pathway
    ''' is active.).
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <AttributeField("ENZYMES-NOT-USED")>
    Public Property enzymesNotUsed As String()

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return If(commonName, uniqueId)
    End Function

End Class
