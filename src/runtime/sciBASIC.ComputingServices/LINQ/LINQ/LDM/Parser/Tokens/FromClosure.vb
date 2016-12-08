#Region "Microsoft.VisualBasic::1286d8836de9ac4dec8c0072550755e3, ..\sciBASIC.ComputingServices\LINQ\LINQ\LDM\Parser\Tokens\FromClosure.vb"

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

Imports sciBASIC.ComputingServices.Linq.Framework
Imports sciBASIC.ComputingServices.Linq.Framework.DynamicCode
Imports sciBASIC.ComputingServices.Linq.Framework.Provider

Namespace LDM.Statements.Tokens

    ''' <summary>
    ''' The init variable.
    ''' </summary>
    Public Class FromClosure : Inherits Closure

        ''' <summary>
        ''' 变量的名称
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Name As String
        ''' <summary>
        ''' 变量的类型标识符
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property TypeId As String

        Sub New(tokens As ClosureTokens(), parent As LinqStatement)
            Call MyBase.New(TokenIcer.Tokens.From, tokens, parent)

            Name = Source.Tokens(Scan0).TokenValue
            TypeId = Source.Tokens(2).TokenValue
        End Sub

        Public Overloads Function [GetType](defs As TypeRegistry) As Type
            Dim value = defs.Find(TypeId)
            If value Is Nothing Then
                Return Scripting.GetType(TypeId)
            Else
                Dim type As Type = value.GetType

                If Not type Is Nothing Then
                    Return type
                Else
                    Return value.GetType
                End If
            End If
        End Function

        Public Function GetEntityRepository(defs As TypeRegistry) As Provider.GetLinqResource
            Dim handle As GetLinqResource = defs.GetHandle(TypeId)
            If handle Is Nothing Then
                Throw New Exception
            Else
                Return handle
            End If
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("Dim {0} As {1}", Name, TypeId)
        End Function
    End Class
End Namespace
