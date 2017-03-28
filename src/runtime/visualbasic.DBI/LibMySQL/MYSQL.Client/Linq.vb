#Region "Microsoft.VisualBasic::250346551b8b27be904005ec8ff33931, ..\visualbasic.DBI\LibMySQL\MYSQL.Client\Linq.vb"

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

''' <summary>
''' Linq to MySQL
''' </summary>
''' <typeparam name="TTable"></typeparam>
Public Class Linq(Of TTable As SQLTable) : Inherits Table(Of TTable)

    Dim __reflector As Reflection.DbReflector

    Sub New(uri As ConnectionUri)
        Call MyBase.New(uri)
        Call __init()
    End Sub

    Sub New(Engine As MySQL)
        Call MyBase.New(Engine)
        Call __init()
    End Sub

    Sub New(base As Table(Of TTable))
        Call MyBase.New(base.MySQL)
        Call __init()
    End Sub

    Private Sub __init()
        __reflector = New Reflection.DbReflector(MySQL.UriMySQL)
    End Sub

    Public Overloads Shared Operator <=(DBI As Linq(Of TTable), SQL As String) As Generic.IEnumerable(Of TTable)
        Dim err As String = ""
        Dim query As Generic.IEnumerable(Of TTable) = DBI.__reflector.AsQuery(Of TTable)(SQL, getError:=err)
        Return query
    End Operator

    Public Overloads Shared Operator >=(DBI As Linq(Of TTable), SQL As String) As Generic.IEnumerable(Of TTable)
        Return DBI <= SQL
    End Operator
End Class
