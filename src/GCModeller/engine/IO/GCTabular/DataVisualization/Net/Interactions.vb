#Region "Microsoft.VisualBasic::7b6cc83848f5b86a282b1a26bfce997f, engine\IO\GCTabular\DataVisualization\Net\Interactions.vb"

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

    '     Class Interactions
    ' 
    '         Properties: FluxValue, Pathway, UniqueId
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream

Namespace DataVisualization

    ''' <summary>
    ''' 可以这样认为，一个Interaction对象就是一个MetabolismFlux
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Interactions : Inherits NetworkEdge

        Public Property Pathway As String
        Public Property UniqueId As String
        Public Property FluxValue As Double

        Public Overrides Function ToString() As String
            Return String.Format("[{0}]  {1}  --> {2}", UniqueId, FromNode, ToNode)
        End Function
    End Class
End Namespace
