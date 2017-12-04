#Region "Microsoft.VisualBasic::c12a845ad2f2a4742d2267a7546af923, ..\httpd\WebCloud\SMRUCC.WebCloud.d3js\Network\Json.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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
        Public Property links As link()       

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure

    Public Class node : Implements IAddressOf, INamedValue

        Public Property name As String Implements INamedValue.Key
        Public Property group As Integer
        Public Property size As Integer
        Public Property type As String
        Public Property Address As Integer Implements IAddressOf.Address
        Public Property color As String 

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class link

        Public Property source As Integer
        Public Property target As Integer
        Public Property value As Integer

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
