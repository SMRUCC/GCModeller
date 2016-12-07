#Region "Microsoft.VisualBasic::2b41a5c7af0df7a0d0e000f48a339598, ..\sciBASIC.ComputingServices\LINQ\LINQ\Framewok\DynamicCode\LinqClosure\LinqValue.vb"

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

Namespace Framework.DynamicCode

    ''' <summary>
    ''' From x in $source let value as LinqValue = Project(x) Where value.IsTrue Select value.value
    ''' </summary>
    Public Structure LinqValue

        Public Property IsTrue As Boolean
        ''' <summary>
        ''' Linq表达式在Select语句之中所产生的数据投影
        ''' </summary>
        ''' <returns></returns>
        Public Property Projects As Object

        Public Shared Function Unavailable() As LinqValue
            Return New LinqValue With {
                .IsTrue = False,
                .Projects = Nothing
            }
        End Function

        Sub New(obj As Object)
            IsTrue = True
            Projects = obj
        End Sub
    End Structure
End Namespace
