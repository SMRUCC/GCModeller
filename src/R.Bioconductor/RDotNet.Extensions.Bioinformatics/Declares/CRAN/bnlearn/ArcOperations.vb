#Region "Microsoft.VisualBasic::9b3f0ae6e4ae1b865ed21bee64a30d06, RDotNet.Extensions.Bioinformatics\Declares\CRAN\bnlearn\ArcOperations.vb"

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

    '     Class setArc
    ' 
    '         Properties: [to], checkCycles, debug, from, x
    ' 
    '     Class dropArc
    ' 
    '         Properties: [to], debug, from, x
    ' 
    '     Class reverseArc
    ' 
    '         Properties: [to], checkCycles, debug, from, x
    ' 
    '     Class setEdge
    ' 
    '         Properties: [to], checkCycles, debug, from, x
    ' 
    '     Class dropEdge
    ' 
    '         Properties: [to], debug, from, x
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNET.Extensions.VisualBasic.SymbolBuilder
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Rtypes

Namespace declares.bnlearn

    ''' <summary>
    ''' The set.arc function operates in the following way:
    ''' 
    ''' If there Is no arc between from And To, the arc from -> To Is added.
    ''' If there Is an undirected arc between from And To, its direction Is Set To from -> To.
    ''' If the arc To -> from Is present, it Is reversed.
    ''' If the arc from -> To Is present, no action Is taken.
    ''' </summary>
    <RFunc("set.arc")> Public Class setArc : Inherits bnlearnBase
        Public Property x As RExpression
        Public Property from As RExpression
        Public Property [to] As RExpression
        <Parameter("check.cycles")> Public Property checkCycles As Boolean = True
        Public Property debug As Boolean = False
    End Class

    ''' <summary>
    ''' The drop.arc function operates in the following way:
    '''
    ''' If there Is no arc between from And To, no action Is taken.
    ''' If there Is a directed Or an undirected arc between from And To, it Is dropped regardless Of its direction.
    ''' </summary>
    <RFunc("drop.arc")> Public Class dropArc : Inherits bnlearnBase
        Public Property x As RExpression
        Public Property from As RExpression
        Public Property [to] As RExpression
        Public Property debug As Boolean = False
    End Class

    ''' <summary>
    ''' The reverse.arc function operates in the following way:
    '''
    ''' If there Is no arc between from And To, it returns an Error.
    ''' If there Is an undirected arc between from And To, it returns an Error.
    ''' If the arc To -> from Is present, it Is reversed.
    ''' If the arc from -> To Is present, it Is reversed.
    ''' </summary>
    <RFunc("reverse.arc")> Public Class reverseArc : Inherits bnlearnBase
        Public Property x As RExpression
        Public Property from As RExpression
        Public Property [to] As RExpression
        <Parameter("check.cycles")> Public Property checkCycles As Boolean = True
        Public Property debug As Boolean = False
    End Class

    ''' <summary>
    ''' The set.edge function operates in the following way:
    '''
    ''' If there Is no arc between from And To, the undirected arc from - To Is added.
    ''' If there Is an undirected arc between from And To, no action Is taken.
    ''' If either the arc from -> To Or the arc To -> from are present, they are replaced With the undirected arc from - To.
    ''' </summary>
    <RFunc("set.edge")> Public Class setEdge : Inherits bnlearnBase
        Public Property x As RExpression
        Public Property from As RExpression
        Public Property [to] As RExpression
        <Parameter("check.cycles")> Public Property checkCycles As Boolean = True
        Public Property debug As Boolean = False
    End Class

    ''' <summary>
    ''' The drop.edge function operates in the following way:
    '''
    ''' If there Is no undirected arc between from And To, no action Is taken.
    ''' If there Is an undirected arc between from And To, it Is removed.
    ''' If there Is a directed arc between from And To, no action Is taken.
    ''' </summary>
    <RFunc("drop.edge")> Public Class dropEdge : Inherits bnlearnBase
        Public Property x As RExpression
        Public Property from As RExpression
        Public Property [to] As RExpression
        Public Property debug As Boolean = False
    End Class
End Namespace
