#Region "Microsoft.VisualBasic::45298106d40a3d95bfe36f7e515b86cc, engine\GCModeller\EngineSystem\ObjectModels\Entity\CompartmentExchanges.vb"

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

    '     Class CompartmentExchangesVirtualCompound
    ' 
    '         Properties: Quantity
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace EngineSystem.ObjectModels.Entity

    ''' <summary>
    ''' 本类型是为了方便程序在空间之间进行物质交换的程序的编写而设置的一个虚构的代谢物的类型
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CompartmentExchangesVirtualCompound : Inherits Compound

        Protected Friend _CompartmentCompound As Compound
        Protected Friend _CompartmentId As String

        Public Overrides Property Quantity As Double
            Get
                Return _CompartmentCompound.Quantity
            End Get
            Set(value As Double)
                _CompartmentCompound.Quantity = value
            End Set
        End Property
    End Class
End Namespace
