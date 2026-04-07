#Region "Microsoft.VisualBasic::ec41024d5473beba2185d21da6b51de4, core\Bio.Assembly\ComponentModel\Equations\Equation\Abstract.vb"

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

    '   Total Lines: 60
    '    Code Lines: 16 (26.67%)
    ' Comment Lines: 34 (56.67%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 10 (16.67%)
    '     File Size: 1.89 KB


    '     Interface ICompoundSpecies
    ' 
    '         Properties: Stoichiometry
    ' 
    '     Interface IEquation
    ' 
    '         Properties: Products, Reactants, Reversible
    ' 
    '     Enum ReactionDirection
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace ComponentModel.EquaionModel

    ''' <summary>
    ''' A metabolite compound abstract model which contains 
    ''' the unique reference id of the metabolite and the
    ''' chemical factor to current equation
    ''' </summary>
    Public Interface ICompoundSpecies : Inherits INamedValue

        ''' <summary>
        ''' stoichiometry, in chemistry, the determination of the 
        ''' proportions in which elements or compounds react with 
        ''' one another. The rules followed in the determination 
        ''' of stoichiometric relationships are based on the laws 
        ''' of conservation of mass and energy and the law of 
        ''' combining weights or volumes.
        ''' </summary>
        ''' <returns></returns>
        Property Stoichiometry As Double

    End Interface

    Public Interface IEquation(Of TCompound As ICompoundSpecies)
        ''' <summary>
        ''' On the equation left side.
        ''' </summary>
        ''' <returns></returns>
        Property Reactants As TCompound()
        ''' <summary>
        ''' On the equation right side.
        ''' </summary>
        ''' <returns></returns>
        Property Products As TCompound()
        Property Reversible As Boolean
    End Interface

    ''' <summary>
    ''' Represents the direction of a reaction
    ''' </summary>
    Public Enum ReactionDirection As Integer

        ''' <summary>
        ''' Reaction is at or near equilibrium
        ''' </summary>
        Equilibrium = 0

        ''' <summary>
        ''' Reaction proceeds in the forward direction
        ''' </summary>
        Forward = 1

        ''' <summary>
        ''' Reaction proceeds in the reverse direction
        ''' </summary>
        Reverse = -1
    End Enum

End Namespace
