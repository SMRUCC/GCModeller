#Region "Microsoft.VisualBasic::e4a2519fd02438ce699026ec9e70ae24, Bio.Assembly\ComponentModel\Equations\Abstract.vb"

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

    '     Interface ICompoundSpecies
    ' 
    '         Properties: StoiChiometry
    ' 
    '     Interface IEquation
    ' 
    '         Properties: Products, Reactants, Reversible
    ' 
    '     Interface ICompoundObject
    ' 
    '         Properties: CHEBI, CommonNames, KEGG_cpd, PUBCHEM
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace ComponentModel.EquaionModel

    Public Interface ICompoundSpecies : Inherits INamedValue
        Property StoiChiometry As Double
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

    Public Interface ICompoundObject : Inherits INamedValue
        Property CommonNames As String()
        Property PUBCHEM As String
        ''' <summary>
        ''' ChEBI ID list
        ''' </summary>
        ''' <returns></returns>
        Property CHEBI As String()

        ''' <summary>
        ''' KEGG compound ID
        ''' </summary>
        ''' <returns></returns>
        Property KEGG_cpd As String
    End Interface
End Namespace
