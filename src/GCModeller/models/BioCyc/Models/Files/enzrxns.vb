#Region "Microsoft.VisualBasic::325e9a86f308122d8595d746f3c94b9d, models\BioCyc\Models\enzrxns.vb"

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

    '   Total Lines: 43
    '    Code Lines: 37 (86.05%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 6 (13.95%)
    '     File Size: 1.47 KB


    ' Class enzrxns
    ' 
    '     Properties: alternativeSubstrates, cofactors, EC_number, enzyme, Kcat
    '                 Km, PH, reaction, reactionDirection, regulatedBy
    '                 specificActivity, temperature, Vmax
    ' 
    ' Class KineticsFactor
    ' 
    '     Properties: citations, Km, substrate
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder
Imports SMRUCC.genomics.Data.BioCyc.Assembly.MetaCyc.Schema.Metabolism

<Xref("enzrxns.dat")>
Public Class enzrxns : Inherits Model

    <AttributeField("ALTERNATIVE-SUBSTRATES")>
    Public Property alternativeSubstrates As String()

    ''' <summary>
    ''' This slot lists the enzyme whose activity is described in this frame. More specifically, 
    ''' the value of this slot is the key of a frame from the class [Protein-Complexes] or 
    ''' [Polypeptides].
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <AttributeField("ENZYME")>
    Public Property enzyme As String

    ''' <summary>
    ''' The value of this slot is the key of a frame from the Reactions class -- the second half 
    ''' of the enzyme/reaction pair that the current frame describes. In fact, this slot can have 
    ''' multiple values, which encode the multiple reactions that one catalytic site of an enzyme 
    ''' catalyzes.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>
    ''' 这里表示的是一个多对多的关系
    ''' 即本对象表示的是Enzyme所指向的蛋白质分子可以催化本列表的所有反应，即某一个酶分子可以催化一系列反应
    ''' 而对于每一个Reaction对象而言，其EnzymaticReaction也为一个列表，即某一个反应可以被几个酶分子进行催化
    ''' </remarks>
    <AttributeField("REACTION")>
    Public Property reaction As String

    ''' <summary>
    ''' This slot specifies the directionality of a reaction. This slot is used in slightly different 
    ''' ways in class Reactions and Enzymatic-Reactions. In class Enzymatic-Reactions, the slot 
    ''' specifies information about the direction of the reaction associated with the enzymatic-reaction, 
    ''' by the associated enzyme. That is, the directionality information refers only to the case in 
    ''' which the reaction is catalyzed by that enzyme, and may be influenced by the regulation of 
    ''' that enzyme.
    ''' The slot is particularly important to fill for reactions that are not part of a pathway, because 
    ''' for such reactions, the direction cannot be determined automatically, whereas for reactions 
    ''' within a pathway, the direction can be inferred from the pathway context. This slot aids the user 
    ''' and software in inferring the direction in which the reaction typically occurs in physiological 
    ''' settings, relative to the direction in which the reaction is stored in the database.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <AttributeField("REACTION-DIRECTION")>
    Public Property reactionDirection As ReactionDirections
    <AttributeField("COFACTORS")>
    Public Property cofactors As String()

    ''' <summary>
    ''' The Michaelis constant (KM) of an enzyme is equal to the substrate concentration at which
    ''' the rate of the reaction is at half of its maximum value. The Michaelis constant is an apparent
    ''' dissociation constant of the enzyme-substrate complex, and thereby is an indicator
    ''' of the affinity of an enzyme to a given substrate. Values of this slot are two-element lists
    ''' of the form (cpd-frame Km) where cpd-frame is the frame id for a substrate of the reaction
    ''' referred to by this enzymatic-reaction frame and Km is the Michaelis constant, a floating
    ''' point number.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <AttributeField("KM")>
    Public Property Km As KineticsFactor()
    <AttributeField("KCAT")>
    Public Property Kcat As KineticsFactor()
    <AttributeField("PH-OPT")>
    Public Property PH As Double
    <AttributeField("TEMPERATURE-OPT")>
    Public Property temperature As Double
    <AttributeField("SPECIFIC-ACTIVITY")>
    Public Property specificActivity As Double

    ''' <summary>
    ''' The values of this slot are members of the [Regulation] class, describing activator or 
    ''' inhibitor compounds for this enzymatic reaction.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <AttributeField("REGULATED-BY")>
    Public Property regulatedBy As String()
    <AttributeField("ENZRXN-EC-NUMBER")>
    Public Property EC_number As ECNumber
    <AttributeField("VMAX")>
    Public Property Vmax As Double

End Class

Public Class KineticsFactor

    Public Property Km As Double
    Public Property substrate As String
    Public Property citations As String()

End Class
