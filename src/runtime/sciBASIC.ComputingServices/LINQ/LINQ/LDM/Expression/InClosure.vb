#Region "Microsoft.VisualBasic::5d09a6cbe0455d4781088042f5baf4d6, ..\sciBASIC.ComputingServices\LINQ\LINQ\LDM\Expression\InClosure.vb"

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

Imports System.CodeDom
Imports Microsoft.VisualBasic.LINQ.Framework
Imports Microsoft.VisualBasic.LINQ.Framework.Provider
Imports Microsoft.VisualBasic.LINQ.Script
Imports Microsoft.VisualBasic.Linq.LDM.Statements.Tokens

Namespace LDM.Expression

    ''' <summary>
    ''' 表示目标对象的数据集合的文件路径或者内存对象的引用
    ''' </summary>
    ''' <remarks></remarks>
    Public Class InClosure : Inherits Closure

        Public ReadOnly Property Type As SourceTypes

        Public ReadOnly Property IsParallel As Boolean

        Dim handle As GetLinqResource
        Dim resource As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="source">[in] var -> parallel</param>
        Sub New(source As Statements.Tokens.InClosure, from As FromClosure, registry As TypeRegistry)
            Call MyBase.New(source)

            If source.Type = SourceTypes.FileURI Then
                resource = DirectCast(source, UriRef).URI
                handle = registry.GetHandle(from.TypeId)
            Else
                resource = DirectCast(source, Reference).Source.Tokens(Scan0).TokenValue
            End If
        End Sub

        Public Function GetResource(runtime As DynamicsRuntime) As IEnumerable
            If Type = SourceTypes.FileURI Then
                Return handle(resource)
            Else
                Return runtime.GetResource(Me.resource)
            End If
        End Function

        Protected Overrides Function __parsing() As CodeExpression

        End Function
    End Class
End Namespace
