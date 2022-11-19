#Region "Microsoft.VisualBasic::6e4fa33d183951ef97e6361257e7bb50, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Slots\Enzrxn.vb"

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

    '   Total Lines: 128
    '    Code Lines: 29
    ' Comment Lines: 80
    '   Blank Lines: 19
    '     File Size: 6.61 KB


    '     Class Enzrxn
    ' 
    '         Properties: AlternativeCofactors, AlternativeSubstrates, Cofactors, Enzyme, IsPhysiologicallyRelevant
    '                     Km, PhysiologicallyRelevant, ProstheticGroups, Reaction, ReactionDirection
    '                     RegulatedBy, RequiredProteinComplex, Table
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Reflection
Imports SMRUCC.genomics.Assembly.MetaCyc.Schema.Reflection
Imports Microsoft.VisualBasic

Namespace Assembly.MetaCyc.File.DataFiles.Slots

    Public Class Enzrxn : Inherits MetaCyc.File.DataFiles.Slots.Object

        ''' <summary>
        ''' This slot lists the enzyme whose activity is described in this frame. More specifically, 
        ''' the value of this slot is the key of a frame from the class [Protein-Complexes] or 
        ''' [Polypeptides].
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExternalKey("proteins,protligandcplxes", "", ExternalKey.Directions.Out)> <MetaCycField()> Public Property Enzyme As String

        ''' <summary>
        ''' Some enzymes catalyze only a particular reaction when they are components of a larger 
        ''' protein complex. For such an enzyme, this slot identifies the particular protein complex 
        ''' of which the enzyme must be a component.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExternalKey("proteins,protligandcplxes", "", ExternalKey.Directions.Out)> <MetaCycField()> Public Property RequiredProteinComplex As String

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
        <ExternalKey("reactions,enzrxns", "", ExternalKey.Directions.Out)> <MetaCycField(type:=MetaCycField.Types.TStr)> Public Property Reaction As List(Of String)

        ''' <summary>
        ''' The values of this slot are members of the [Regulation] class, describing activator or 
        ''' inhibitor compounds for this enzymatic reaction.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField(type:=MetaCycField.Types.TStr)> Public Property RegulatedBy As List(Of String)

        <MetaCycField()> Public Property Cofactors As String

        <MetaCycField()> Public Property ProstheticGroups As String

        <MetaCycField(type:=MetaCycField.Types.TStr)> Public Property AlternativeSubstrates As List(Of String)

        <MetaCycField()> Public Property AlternativeCofactors As String

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
        <MetaCycField(type:=MetaCycField.Types.TStr)> Public Property Km As List(Of String)

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
        <MetaCycField()> Public Property ReactionDirection As String

        ''' <summary>
        ''' PHYSIOLOGICALLY-RELEVANT?
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <MetaCycField(name:="PHYSIOLOGICALLY-RELEVANT?")> Public Property PhysiologicallyRelevant As String

        Public Overrides ReadOnly Property Table As [Object].Tables
            Get
                Return Tables.enzrxns
            End Get
        End Property

        Public ReadOnly Property IsPhysiologicallyRelevant As Boolean
            Get
                Return String.Equals(PhysiologicallyRelevant, "T")
            End Get
        End Property

        'Public Shared Shadows Widening Operator CType(e As MetaCyc.File.AttributeValue.Object) As Enzrxn
        '    Dim NewObj As Enzrxn = New Enzrxn

        '    Call MetaCyc.File.DataFiles.Slots.[Object].TypeCast(Of Enzrxn) _
        '        (MetaCyc.File.AttributeValue.Object.Format(Enzrxns.AttributeList, e), NewObj)

        '    NewObj.RegulatedBy = StringQuery(NewObj.Object, "REGULATED-BY( \d+)?")
        '    NewObj.Km = StringQuery(NewObj.Object, "KM( \d+)?")
        '    NewObj.Reaction = StringQuery(NewObj.Object, "^REACTION$|REACTION \d+")
        '    NewObj.AlternativeSubstrates = StringQuery(NewObj.Object, "ALTERNATIVE-SUBSTRATES( \d+)?")

        '    Return NewObj
        'End Operator
    End Class
End Namespace
