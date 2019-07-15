#Region "Microsoft.VisualBasic::2a55cae024cab91fe18c87f4bd488d4d, WebCloud\SMRUCC.HTTPInternal\AppEngine\Sessions\Session.vb"

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

    '     Class Session
    ' 
    '         Properties: ID, Table
    ' 
    '         Function: ToString
    ' 
    '         Sub: SetValue
    ' 
    '     Class Value
    ' 
    '         Properties: Table, Value
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace AppEngine.Sessions

    Public Class Session : Implements INamedValue

        Public Property ID As String Implements INamedValue.Key
        Public Property Table As Dictionary(Of String, Value)

        Default Public Property Item(name As String) As Value
            Get
                Return Table.TryGetValue(name)
            End Get
            Set(value As Value)
                Table(name) = value
            End Set
        End Property

        Public Sub SetValue(key$, value$)
            Item(key) = value
        End Sub

        Public Overrides Function ToString() As String
            Return $"[{ID} => {Table.Keys.ToArray.GetJson}]"
        End Function
    End Class

    Public Class Value

        Public Property Value As String
        Public Property Table As Dictionary(Of String, Value)

        Public Overrides Function ToString() As String
            If Table.IsNullOrEmpty Then
                Return Value
            Else
                Return Table.GetJson
            End If
        End Function

        Public Shared Widening Operator CType(value As String) As Value
            Return New Value With {
                .Value = value,
                .Table = New Dictionary(Of String, Value)
            }
        End Operator
    End Class
End Namespace
