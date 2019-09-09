#Region "Microsoft.VisualBasic::e340a584a00f20308776b31c9f5ec208, RDotNET.Extensions.VisualBasic\ScriptBuilder\RPackage\Graphics\gplots\venn.vb"

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

    '     Class venn
    ' 
    '         Properties: data, intersections, showPlot, showSetLogicLabel, simplify
    '                     small, universe
    ' 
    '     Class plot
    ' 
    '         Properties: showSetLogicLabel, simplify, small, x, y
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Abstract
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Rtypes

Namespace SymbolBuilder.packages.gplots

    ''' <summary>
    ''' Plot a Venn diagrams for up to 5 sets
    ''' </summary>
    ''' <remarks>
    ''' data should be either a named list of vectors containing character string names ("GeneAABBB", "GeneBBBCY", .., "GeneXXZZ")
    ''' or indexes of group intersections (1, 2, .., N), or a data frame containing indicator variables (TRUE, FALSE, TRUE, ..)
    ''' for group intersectionship. Group names will be taken from the component list element or column names.
    '''
    ''' Invisibly returns an object of class "venn", containing a matrix of all possible sets of groups, and the observed count of
    ''' items belonging to each The fist column contains observed counts, subsequent columns contain 0-1 indicators of group
    ''' intersectionship.
    ''' </remarks>
    <RFunc(NameOf(venn))> Public Class venn : Inherits IRToken

        ''' <summary>
        ''' Either a list list containing vectors of names or indices of group intersections,
        ''' or a data frame containing boolean indicators of group intersectionship (see below)
        ''' </summary>
        ''' <returns></returns>
        Public Property data As RExpression
        ''' <summary>
        ''' Subset of valid name/index elements. Values ignore values in codedata not
        ''' in this list will be ignored. Use NA to use all elements of data (the default).
        ''' </summary>
        ''' <returns></returns>
        Public Property universe As RExpression = NA
        ''' <summary>
        ''' Character scaling of the smallest group counts
        ''' </summary>
        ''' <returns></returns>
        Public Property small As Double = 0.7
        ''' <summary>
        ''' Logical flag indicating whether the internal group label should be displayed
        ''' </summary>
        ''' <returns></returns>
        Public Property showSetLogicLabel As Boolean = False
        ''' <summary>
        ''' Logical flag indicating whether unobserved groups should be omitted.
        ''' </summary>
        ''' <returns></returns>
        Public Property simplify As Boolean = False
        ''' <summary>
        ''' Logical flag indicating whether the plot should be displayed. If false, simply returns the group count matrix.
        ''' </summary>
        ''' <returns></returns>
        <Parameter("show.plot")> Public Property showPlot As Boolean = True
        ''' <summary>
        ''' Logical flag indicating if the returned object should have the attribute "individuals.in.intersections"
        ''' featuring for every set a list of individuals that are assigned to it.
        ''' </summary>
        ''' <returns></returns>
        Public Property intersections As Boolean = True
    End Class

    ''' <summary>
    ''' ## S3 method for class '<see cref="venn"/>'
    ''' </summary>
    <RFunc(NameOf(plot))> Public Class plot : Inherits IRToken
        ''' <summary>
        ''' Either a list list containing vectors of names or indices of group intersections,
        ''' or a data frame containing boolean indicators of group intersectionship (see below)
        ''' </summary>
        ''' <returns></returns>
        Public Property x As RExpression
        Public Property y As RExpression
        ''' <summary>
        ''' Character scaling of the smallest group counts
        ''' </summary>
        ''' <returns></returns>
        Public Property small As Double = 0.7
        ''' <summary>
        ''' Logical flag indicating whether the internal group label should be displayed
        ''' </summary>
        ''' <returns></returns>
        Public Property showSetLogicLabel As Boolean = False
        ''' <summary>
        ''' Logical flag indicating whether unobserved groups should be omitted.
        ''' </summary>
        ''' <returns></returns>
        Public Property simplify As Boolean = False
    End Class
End Namespace
