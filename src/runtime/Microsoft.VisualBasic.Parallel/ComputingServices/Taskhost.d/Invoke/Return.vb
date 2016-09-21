#Region "Microsoft.VisualBasic::948802395d5d1ec12c0c93837cedf378, ..\ComputingServices\Taskhost.d\Invoke\Return.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports Microsoft.VisualBasic.Net
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Net.Protocols
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace TaskHost

    ''' <summary>
    ''' The returns value.(远端调用的函数返回)
    ''' </summary>
    Public Class Rtvl

        Public Property errCode As Integer
        ''' <summary>
        ''' Exception Message
        ''' </summary>
        ''' <returns></returns>
        Public Property ex As String
        ''' <summary>
        ''' Json value
        ''' </summary>
        ''' <returns></returns>
        Public Property value As String

        Sub New()
        End Sub

        Sub New(ex As Exception)
            _errCode = HTTP_RFC.RFC_INTERNAL_SERVER_ERROR
            _ex = ex.ToString
        End Sub

        Sub New(value As Object, type As Type)
            _errCode = HTTP_RFC.RFC_OK
            _value = JsonContract.GetObjectJson(value, type)
        End Sub

        ''' <summary>
        ''' If the remote execute raising a exception, then a exception will be throw from this function.
        ''' </summary>
        ''' <param name="type"></param>
        ''' <returns></returns>
        Public Function GetValue(type As Type) As Object
            If errCode <> HTTP_RFC.RFC_OK Then
                Throw New Exception(ex)
            End If
            Return LoadObject(value, type)
        End Function

        Public Function GetValue(func As [Delegate]) As Object
            Dim type As Type = func.Method.ReturnType
            Return GetValue(type)
        End Function

        Public Shared Function CreateObject(Of T)(x As T) As Rtvl
            Return New Rtvl(x, GetType(T))
        End Function
    End Class
End Namespace
