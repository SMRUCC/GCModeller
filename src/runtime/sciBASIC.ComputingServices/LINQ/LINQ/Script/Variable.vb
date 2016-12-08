#Region "Microsoft.VisualBasic::7c1f98717068dc3c91de95e4cb5192e2, ..\sciBASIC.ComputingServices\LINQ\LINQ\Script\Variable.vb"

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

Imports System.Dynamic
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports sciBASIC.ComputingServices.Linq.Framework

Namespace Script

    Public Class Variable : Implements INamedValue
        Implements IEnumerable

        Public Property Name As String Implements INamedValue.Key
        Public Property Data As IEnumerable

        Public Overrides Function ToString() As String
            Return Name
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield Data.GetEnumerator
        End Function

        Public Shared Operator +(hash As Dictionary(Of String, Variable), x As Variable) As Dictionary(Of String, Variable)
            x.Name = x.Name.ToLower
            If hash.ContainsKey(x.Name) Then
                Call hash.Remove(x.Name)
            End If
            Call hash.Add(x.Name, x)
            Return hash
        End Operator
    End Class
End Namespace
