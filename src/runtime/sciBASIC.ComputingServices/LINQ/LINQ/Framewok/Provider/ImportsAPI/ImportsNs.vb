#Region "Microsoft.VisualBasic::b175dd2863ef5f18cef06a39ffe111fa, ..\sciBASIC.ComputingServices\LINQ\LINQ\Framewok\Provider\ImportsAPI\ImportsNs.vb"

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

Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace Framework.Provider.ImportsAPI

    ''' <summary>
    ''' 导入的命名空间
    ''' </summary>
    Public Class ImportsNs : Inherits PackageNamespace

        ''' <summary>
        ''' {namespace, typeinfo}
        ''' </summary>
        ''' <returns></returns>
        Public Property Modules As TypeInfo()
            Get
                Return __list.ToArray
            End Get
            Set(value As TypeInfo())
                If value Is Nothing Then
                    __list = New List(Of TypeInfo)
                Else
                    __list = New List(Of TypeInfo)(value)
                End If
            End Set
        End Property

        Dim __list As New List(Of TypeInfo)

        Sub New()
        End Sub

        Sub New(base As PackageNamespace)
            Call MyBase.New(base)
        End Sub

        Public Sub Add(type As Type)
            Dim info As New TypeInfo(type)
            Call __list.Add(info)
        End Sub

        Public Function Remove(type As Type) As Boolean
            Dim LQuery = (From x In __list Where x = type Select x).ToArray
            For Each x In LQuery
                Call __list.Remove(x)
            Next

            Return Not LQuery.IsNullOrEmpty
        End Function
    End Class
End Namespace
