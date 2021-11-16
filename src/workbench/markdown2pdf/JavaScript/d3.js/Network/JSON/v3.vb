#Region "Microsoft.VisualBasic::e9c45af00c17dcc94ac5b96cbe5ed6c7, markdown2pdf\JavaScript\d3.js\Network\JSON\v3.vb"

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

    '     Class Regulation
    ' 
    '         Properties: MotifFamily, ORF_ID, Regulator
    ' 
    '         Function: ToString
    ' 
    '         Structure out
    ' 
    '             Properties: links, nodes
    ' 
    '             Function: ToString
    ' 
    '         Class node
    ' 
    '             Properties: color, group, ID, name, size
    '                         type, value
    ' 
    '             Function: ToString
    ' 
    '             Sub: Assign
    ' 
    '         Class link
    ' 
    '             Properties: source, target, value
    ' 
    '             Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Network.JSON

    Public Class Regulation

        <Column("ORF ID")> Public Property ORF_ID As String
        Public Property MotifFamily As String
        Public Property Regulator As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Namespace v3

        ''' <summary>
        ''' helper class for json text generation 
        ''' </summary>
        Public Structure out

            ''' <summary>
            ''' 网络之中的节点对象
            ''' </summary>
            ''' <returns></returns>
            Public Property nodes As node()
            ''' <summary>
            ''' 节点之间的边链接
            ''' </summary>
            ''' <returns></returns>
            Public Property links As link(Of Integer)()

            Public Overrides Function ToString() As String
                Return Me.GetJson
            End Function
        End Structure

        Public Class node : Implements IAddressOf, INamedValue

            Public Property name As String Implements INamedValue.Key
            Public Property group As String
            Public Property size As Double
            Public Property type As String
            Public Property ID As Integer Implements IAddressOf.Address
            Public Property color As String
            Public Property value As Dictionary(Of String, String)

            Public Sub Assign(address As Integer) Implements IAddress(Of Integer).Assign
                ID = address
            End Sub

            Public Overrides Function ToString() As String
                Return Me.GetJson
            End Function
        End Class

        ''' <summary>
        ''' 在d3.js v3之中，边的连接使用的是节点集合之中的下标数值
        ''' 但是在d3.js v4之中，边的连接则可以直接使用节点的``id``属性来表示了
        ''' </summary>
        Public Class link(Of T)

            Public Property source As T
            Public Property target As T
            Public Property value As Double

            Public Overrides Function ToString() As String
                Return Me.GetJson
            End Function
        End Class
    End Namespace
End Namespace
