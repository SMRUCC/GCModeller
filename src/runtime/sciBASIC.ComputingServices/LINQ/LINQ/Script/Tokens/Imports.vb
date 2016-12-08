#Region "Microsoft.VisualBasic::31967f4b330b0ec9b1b06097b2d16f5a, ..\sciBASIC.ComputingServices\LINQ\LINQ\Script\Tokens\Imports.vb"

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
Imports sciBASIC.ComputingServices.Linq.LDM.Statements

Namespace Script.Tokens

    Public Class [Imports] : Inherits TokenBase
        Implements IEnumerable(Of String)

        Public ReadOnly Property Namespaces As String()

        Sub New(source As IEnumerable(Of Token(Of TokenIcer.Tokens)))
            Call MyBase.New(source)

            Namespaces = (From x As Token(Of TokenIcer.Tokens)
                          In source
                          Where Not x.Type = TokenIcer.Tokens.WhiteSpace
                          Select x.Text).ToArray
        End Sub

        Public Overrides Function ToString() As String
            Return "Imports " & String.Join(", ", Namespaces)
        End Function

        ''' <summary>
        ''' 枚举所有导入的命名空间
        ''' </summary>
        ''' <returns></returns>
        Public Iterator Function GetEnumerator() As IEnumerator(Of String) Implements IEnumerable(Of String).GetEnumerator
            For Each ns As String In Namespaces
                If Not String.IsNullOrEmpty(ns) Then
                    Yield ns
                End If
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace
