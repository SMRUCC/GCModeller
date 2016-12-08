#Region "Microsoft.VisualBasic::b88dd16a99ec7e573a9c42ce1b49d66e, ..\sciBASIC.ComputingServices\LINQ\LINQ\Framewok\ObjectModel\ParallelLinq.vb"

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

Imports sciBASIC.ComputingServices.Linq.Framework.DynamicCode
Imports sciBASIC.ComputingServices.Linq.LDM.Statements
Imports sciBASIC.ComputingServices.Linq.Script

Namespace Framework.ObjectModel

    ''' <summary>
    ''' 并行LINQ查询表达式的对象模型
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ParallelLinq : Inherits Linq

        Sub New(Expr As LinqStatement, FrameworkRuntime As DynamicsRuntime)
            Call MyBase.New(Expr, Runtime:=FrameworkRuntime)
        End Sub

        Public Overrides Function EXEC() As IEnumerable
            Dim Linq = (From x As Object In __getSource.AsParallel
                        Let value As LinqValue = __project(x)
                        Where value.IsTrue
                        Select value.Projects)
            Return Linq
        End Function
    End Class
End Namespace
