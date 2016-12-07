#Region "Microsoft.VisualBasic::f25a15d610194d94cbf45c94f7c07f57, ..\sciBASIC.ComputingServices\LINQ\LINQ\Script\Tokens\Linq.vb"

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

Imports Microsoft.VisualBasic.Scripting.TokenIcer
Imports sciBASIC.ComputingServices.Linq.Framework.Provider
Imports sciBASIC.ComputingServices.Linq.LDM.Statements

Namespace Script.Tokens

    Public MustInherit Class TokenBase

        Protected ReadOnly __source As Token(Of TokenIcer.Tokens)()

        Sub New(source As IEnumerable(Of Token(Of TokenIcer.Tokens)))
            __source = source.ToArray
        End Sub
    End Class

    ''' <summary>
    ''' Value assignment statement for assign the value the a variable in the LINQ script runtime.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Linq : Inherits TokenBase

        ''' <summary>
        ''' Variable name
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Name As String
        Public ReadOnly Property Linq As LinqStatement

        Sub New(source As IEnumerable(Of Token(Of TokenIcer.Tokens)), types As TypeRegistry)
            Call MyBase.New(source)

            If String.Equals(__source(Scan0).Text, "var", StringComparison.OrdinalIgnoreCase) AndAlso
                   String.Equals(__source(2).Text, "=") Then
                source = source.Skip(1)
            End If

            Name = source(Scan0).Text
            Linq = LinqStatement.TryParse(source.Skip(2), types)
        End Sub

        Public Overrides Function ToString() As String
            Return $"var {Name} = {Linq.ToString}"
        End Function

        Public Shared Narrowing Operator CType(linq As Linq) As LinqStatement
            Return linq.Linq
        End Operator
    End Class
End Namespace
