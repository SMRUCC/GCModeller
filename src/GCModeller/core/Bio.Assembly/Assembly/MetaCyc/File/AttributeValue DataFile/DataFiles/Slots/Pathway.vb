#Region "Microsoft.VisualBasic::af592cc4c25b6d8d8f9e97023fc3677c, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Slots\Pathway.vb"

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

    '   Total Lines: 244
    '    Code Lines: 35
    ' Comment Lines: 185
    '   Blank Lines: 24
    '     File Size: 13.81 KB


    '     Class Pathway
    ' 
    '         Properties: AssumeUniqueEnzymes, ClassInstanceLinks, DisableDisplay, EnzymesNotUsed, EnzymeUse
    '                     HypotheticalReactions, InPathway, LayoutAdvice, PathwayInteractions, PathwayLinks
    '                     PolymerizationLinks, Predecessors, Primaries, ReactionList, Species
    '                     SubPathways, SuperPathways, Table
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Reflection
Imports SMRUCC.genomics.Assembly.MetaCyc.Schema.Reflection
Imports Microsoft.VisualBasic

Namespace Assembly.MetaCyc.File.DataFiles.Slots

    Public Class Pathway : Inherits [Object]

        ''' <summary>
        ''' This slot holds a comment that describes interactions between this pathway and other
        ''' biochemical pathways, such as those pathways that supply an important precursor.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField()> Public Property PathwayInteractions As String

        ''' <summary>
        ''' This slot describes the linked reactions that compose the current pathway. Since pathways
        ''' have a variety of topologies — from linear to circular to tree structured — pathways
        ''' cannot be represented as simple sequences of reactions. A pathway is a list of reaction/
        ''' predecessor pairs.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField(type:=MetaCycField.Types.TStr)> Public Property Predecessors As List(Of String)

        ''' <summary>
        ''' This slot lists all reactions in the current pathway, in no particular order.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExternalKey("reactions,enzrxns", "contains", ExternalKey.Directions.Out)> <MetaCycField(Type:=MetaCycField.Types.TStr)>
        Public Property ReactionList As List(Of String)

        ''' <summary>
        ''' A list of reactions in this pathway that are considered hypothetical, probably because
        ''' presence of the enzyme has not been demonstrated.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExternalKey("reactions,enzrxns", "contains", ExternalKey.Directions.Out)> <MetaCycField(Type:=MetaCycField.Types.TStr)>
        Public Property HypotheticalReactions As List(Of String)

        ''' <summary>
        ''' By default it is assumed that all enzymes that can catalyze a reaction will do so in each
        ''' pathway in which the reaction occurs. That default assumption is encoded by the default
        ''' value of FALSE for this slot; when you want to assume that only one enzyme exists in the
        ''' DB to catalyze every reaction in this pathway, this slot should be given the value TRUE.
        ''' This slot can be used for consistency-checking purposes, that is, in a pathway for which
        ''' this slot is TRUE, there should not be any reactions that are catalyzed by more than one
        ''' reaction.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Boolean</remarks>
        <MetaCycField()> Public Property AssumeUniqueEnzymes As String

        ''' <summary>
        ''' By default it is assumed that all enzymes that can catalyze a reaction will do so in each
        ''' pathway in which the reaction occurs. This slot is used in the case that this assumption
        ''' does not hold, that is, if a reaction is catalyzed in a particular pathway by only a subset
        ''' (or none) of the possible enzymes that are known to catalyze that reaction. Therefore,
        ''' this slot can be used only when the value of the assume-unique-enzymes slot is FALSE
        ''' (because multiple enzymes catalyze some step in the pathway).
        ''' The form of a value for the slot is (reaction-ID enzymatic-reaction-ID-1... enzymaticreaction-
        ''' ID-n). That is, each value specifies a reaction, and specifies the one or more
        ''' enzymatic reactions that catalyze that reaction in this pathway. If no enzymatic reactions
        ''' are specified, then none of the enzymes that are known to catalyze the reaction do so in
        ''' this pathway.
        ''' For example, under aerobic conditions the oxidation of succinate to fumarate is catalyzed
        ''' by succinate dehydrogenase in the forward direction, and, under anaerobic conditions,
        ''' by fumarate reductase in the reverse direction. The TCA cycle is active only in aerobic
        ''' conditions, so only succinate dehydrogenase is used in this pathway. This fact would be
        ''' recorded as follows:
        ''' enzyme-use: (succ-fum-oxred-rxn succinate-oxn-enzrxn)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField()> Public Property EnzymeUse As String

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
        <MetaCycField(type:=MetaCycField.Types.TStr)> Public Property EnzymesNotUsed As List(Of String)

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
        <MetaCycField(type:=MetaCycField.Types.TStr)> Public Property Primaries As List(Of String)

        ''' <summary>
        ''' This slot is used only in pathway frames in the MetaCyc DB, in which case the slot identifies
        ''' the one or more species in which this pathway is known to occur experimentally.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField()> Public Property Species As String

        ''' <summary>
        ''' When the value is true, this slot disables display of the pathway drawing for a pathway.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField()> Public Property DisableDisplay As String

        ''' <summary>
        ''' This slot lists direct super-pathways of a pathway.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField()> Public Property SuperPathways As String

        ''' <summary>
        ''' This slot is the inverse of the Super-Pathways slot. It lists all the direct subpathways of a
        ''' pathway.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField(type:=MetaCycField.Types.TStr)> Public Property SubPathways As List(Of String)

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
        <MetaCycField(name:="PATHWAY-LINKS", type:=MetaCycField.Types.TStr)> Public Property PathwayLinks As List(Of String)

        ''' <summary>
        ''' This slot controls drawing of polymerization relationships within a pathway. Each
        ''' value of this slot is of the form (cpd-class product-rxn reactant-rxn). When both reactions
        ''' are non-nil, an identity link is created between the polymer compound class cpd-
        ''' class, a product of product-rxn, and the same compound class as a reactant of reactantrxn.
        ''' The PRODUCT-NAME-SLOT and REACTANT-NAME-SLOT annotations specify
        ''' which slot should be used to derive the compound label in product-rxn and reactant-rxn
        ''' above, respectively, if one or both are omitted, COMMON-NAME is assumed. Either
        ''' reaction above may be nil; in this case, no identity link is created. This form is used solely
        ''' in conjunction with one of the name-slot annotations to specify a name-slot other than
        ''' COMMON-NAME for a polymer compound class in a reaction of the pathway.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField()> Public Property PolymerizationLinks As String

        ''' <summary>
        ''' Each value of this slot is a reaction in the pathway. Two annotations (in addition to the
        ''' usual possibilities) are available on this slot: REACTANT-INSTANCES and PRODUCTINSTANCES,
        ''' whose values are compounds. If one of the reactants of the slot-value
        ''' reaction is a class C and the REACTANT-INSTANCES are instances of C, then the instances
        ''' are drawn as part of the pathway, with identity links to the class. The PRODUCTINSTANCES
        ''' are treated similarly.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField()> Public Property ClassInstanceLinks As String

        ''' <summary>
        ''' Each value of this slot is a dotted pair of the form (advice-keyword . advice, and represents
        ''' some piece of advice to the automatic pathway layout code. Currently supported
        ''' advice keywords are
        '''   1. :CYCLE-TOP-CPD: The advice is a compound key. In pathways containing a cycle,
        '''      the cycle will be rotated so that the specified compound is positioned at twelve
        '''      o’clock.
        '''   2. :REVERSIBLE-RXNS: The advice is a list of reactions that should be drawn as reversible,
        '''      even when the pathway is being drawn to show pathway flow (rather than
        '''      true reversibility).
        '''   3. :CASCADE-RXN-ORDERING: The advice is a list of reactions that form a partial order
        '''      for reactions in a cascade pathway (i.e., the 2-component signaling pathways).
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField()> Public Property LayoutAdvice As String
        <MetaCycField(type:=MetaCycField.Types.TStr)> Public Property InPathway As List(Of String)

        Public Overrides ReadOnly Property Table As [Object].Tables
            Get
                Return Tables.pathways
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0} [{1} reactions]", Identifier, ReactionList.Count)
        End Function

        'Public Shared Shadows Widening Operator CType(e As MetaCyc.File.AttributeValue.Object) As Pathway
        '    Dim NewObj As Pathway = New Pathway

        '    Call MetaCyc.File.DataFiles.Slots.[Object].TypeCast(Of Pathway) _
        '        (MetaCyc.File.AttributeValue.Object.Format(Pathways.AttributeList, e), NewObj)

        '    NewObj.PathwayLinks() = StringQuery(NewObj.Object, "PATHWAY-LINKS( \d+)?")
        '    NewObj.Predecessors = StringQuery(NewObj.Object, "PREDECESSORS( \d+)?")
        '    NewObj.SubPathways = StringQuery(NewObj.Object, "SUB-PATHWAYS( \d+)?")
        '    NewObj.InPathway = StringQuery(NewObj.Object, "IN-PATHWAY( \d+)?")
        '    NewObj.EnzymesNotUsed = StringQuery(NewObj.Object, "ENZYMES-NOT-USED( \d+)?")
        '    NewObj.Primaries = StringQuery(NewObj.Object, "PRIMARIES( \d+)?")
        '    NewObj.ReactionList = StringQuery(NewObj.Object, "REACTION-LIST( \d+)?")

        '    Return NewObj
        'End Operator
    End Class
End Namespace
