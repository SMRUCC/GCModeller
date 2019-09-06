#Region "Microsoft.VisualBasic::6e4092ff8d22d9f4ebebb81c0ad8c1bc, RDotNET.Extensions.VisualBasic\ScriptBuilder\RPackage\flashClust.vb"

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

    '     Class flashClust
    ' 
    '         Properties: d, members, method
    ' 
    '     Class hclust
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Abstract
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Rtypes

Namespace SymbolBuilder.packages.flashClust

    ''' <summary>
    ''' Faster alternative to hclust.
    ''' This function implements optimal hierarchical clustering with the same interface as hclust.
    ''' </summary>
    ''' <remarks>
    ''' See the description of hclust for details on available clustering methods.
    '''
    ''' If members! = NULL, Then d Is taken To be a dissimilarity matrix between clusters instead Of dissimilarities between singletons And members gives the number Of observations per cluster. 
    ''' This way the hierarchical cluster algorithm can be 'started in the middle of the dendrogram’, e.g., in order to reconstruct the part of the tree above a cut (see examples). 
    ''' Dissimilarities between clusters can be efficiently computed (i.e., without hclust itself) only for a limited number of distance/linkage combinations, the simplest one being squared Euclidean distance and centroid linkage. 
    ''' In this case the dissimilarities between the clusters are the squared Euclidean distances between cluster means.
    ''' flashClust Is a wrapper for compatibility with older code.
    ''' </remarks>
    <RFunc("flashClust")> Public Class flashClust : Inherits IRToken

        Default Public ReadOnly Property Func(x As String) As String
            Get
                Dim R As flashClust = Me.ShadowsCopy
                R.d = x
                Return R.RScript
            End Get
        End Property

        ''' <summary>
        ''' a dissimilarity Structure As produced by 'dist'.
        ''' </summary>
        ''' <returns></returns>
        Public Property d As RExpression
        ''' <summary>
        ''' the agglomeration method To be used. This should be (an unambiguous abbreviation Of) one Of "ward", "single", "complete", "average", "mcquitty", "median" Or "centroid".
        ''' </summary>
        ''' <returns></returns>
        Public Property method As String
        ''' <summary>
        ''' NULL Or a vector with length size of d. See the 'Details’ section.
        ''' </summary>
        ''' <returns></returns>
        Public Property members As RExpression
    End Class

    <RFunc("hclust")> Public Class hclust : Inherits flashClust
    End Class
End Namespace
