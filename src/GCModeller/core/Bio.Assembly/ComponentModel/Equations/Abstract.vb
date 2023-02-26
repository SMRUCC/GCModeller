#Region "Microsoft.VisualBasic::fc753b82c0acec5eadd7e94f835dc0eb, GCModeller\core\Bio.Assembly\ComponentModel\Equations\Abstract.vb"

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

    '   Total Lines: 22
    '    Code Lines: 11
    ' Comment Lines: 8
    '   Blank Lines: 3
    '     File Size: 697 B


    '     Interface ICompoundSpecies
    ' 
    '         Properties: StoiChiometry
    ' 
    '     Interface IEquation
    ' 
    '         Properties: Products, Reactants, Reversible
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
End Namespace
