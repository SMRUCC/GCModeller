#Region "Microsoft.VisualBasic::569dbb09b57729e6bc45d3b3f273fe0e, engine\GCModeller\EngineSystem\ObjectModels\Entity\Entity.vb"

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

    ' Class Entity
    ' 
    '     Properties: Compartment, Guid, Quantity, uid
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' 细胞系统之中的一个实体对象，请注意，这个对象只用来表示一个现实的物理世界之中存在的生物分子对象
''' </summary>
Public Class Entity

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Guid As Long
    ''' <summary>
    ''' The unique-id
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property uid As String
    ''' <summary>
    ''' 数量
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Quantity As Double
    ''' <summary>
    ''' 在细胞内或者细胞外的区域的编号
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Compartment As String

    Sub New()

    End Sub
End Class
