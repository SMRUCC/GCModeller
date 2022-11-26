#Region "Microsoft.VisualBasic::1845887d81c3234c9c753a8a7689d940, GCModeller\models\BioCyc\Models\ReactionDirections.vb"

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

    '   Total Lines: 72
    '    Code Lines: 19
    ' Comment Lines: 50
    '   Blank Lines: 3
    '     File Size: 3.69 KB


    '     Enum ReactionDirections
    ' 
    '         IrreversibleLeftToRight, IrreversibleRightToLeft, LeftToRight, PhysiolLeftToRight, PhysiolRightToLeft
    '         Reversible, RightToLeft
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel

Namespace Assembly.MetaCyc.Schema.Metabolism

    ''' <summary>
    ''' This slot specifies the directionality of a reaction. This slot is used in slightly different 
    ''' ways in class Reactions and Enzymatic-Reactions. In class Enzymatic-Reactions, the slot specifies 
    ''' information about the direction of the reaction associated with the enzymatic-reaction, by the 
    ''' associated enzyme. That is, the directionality information refers only to the case in which the 
    ''' reaction is catalyzed by that enzyme, and may be influenced by the regulation of that enzyme.
    ''' 
    ''' The slot is particularly important to fill for reactions that are not part of a pathway, because 
    ''' for such reactions, the direction cannot be determined automatically, whereas for reactions 
    ''' within a pathway, the direction can be inferred from the pathway context. This slot aids the 
    ''' user and software in inferring the direction in which the reaction typically occurs in 
    ''' physiological settings, relative to the direction in which the reaction is stored in the database.
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum ReactionDirections

        ''' <summary>
        ''' The reaction occurs in both directions in physiological settings.
        ''' </summary>
        ''' <remarks></remarks>
        <Description("REVERSIBLE")>
        Reversible
        ''' <summary>
        ''' The reaction occurs in the specified direction in physiological settings, because of several 
        ''' possible factors including the energetics of the reaction, local concentrations of reactants 
        ''' and products, and the regulation of the enzyme or its expression.
        ''' </summary>
        ''' <remarks></remarks>
        <Description("PHYSIOL-LEFT-TO-RIGHT")>
        PhysiolLeftToRight
        ''' <summary>
        ''' The reaction occurs in the specified direction in physiological settings, because of several 
        ''' possible factors including the energetics of the reaction, local concentrations of reactants 
        ''' and products, and the regulation of the enzyme or its expression.
        ''' </summary>
        ''' <remarks></remarks>
        <Description("PHYSIOL-RIGHT-TO-LEFT")>
        PhysiolRightToLeft
        ''' <summary>
        ''' For all practical purposes, the reaction occurs only in the specified direction in 
        ''' physiological settings, because of chemical properties of the reaction.
        ''' </summary>
        ''' <remarks></remarks>
        <Description("IRREVERSIBLE-LEFT-TO-RIGHT")>
        IrreversibleLeftToRight
        ''' <summary>
        ''' For all practical purposes, the reaction occurs only in the specified direction in 
        ''' physiological settings, because of chemical properties of the reaction.
        ''' </summary>
        ''' <remarks></remarks>
        <Description("IRREVERSIBLE-RIGHT-TO-LEFT")>
        IrreversibleRightToLeft
        ''' <summary>
        ''' The reaction occurs in the specified direction in physiological settings, but it is unknown 
        ''' whether the reaction is considered irreversible.
        ''' </summary>
        ''' <remarks></remarks>
        <Description("LEFT-TO-RIGHT")>
        LeftToRight
        ''' <summary>
        ''' The reaction occurs in the specified direction in physiological settings, but it is unknown 
        ''' whether the reaction is considered irreversible.
        ''' </summary>
        ''' <remarks></remarks>
        <Description("RIGHT-TO-LEFT")>
        RightToLeft
    End Enum
End Namespace
