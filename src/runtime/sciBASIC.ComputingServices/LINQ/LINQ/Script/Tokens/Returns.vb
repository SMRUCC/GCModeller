#Region "Microsoft.VisualBasic::3c435e7c252015ac504b0f81b9664ad5, ..\sciBASIC.ComputingServices\LINQ\LINQ\Script\Tokens\Returns.vb"

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

    Public Class Returns : Inherits TokenBase

        Public ReadOnly Property Ref As String

        Sub New(source As IEnumerable(Of Token(Of TokenIcer.Tokens)))
            Call MyBase.New(source)

            For Each x In source.Skip(1)
                If Not x.Type = TokenIcer.Tokens.WhiteSpace Then
                    Ref = x.Text
                    Exit For
                End If
            Next
        End Sub

        Public Overrides Function ToString() As String
            Return "Return " & Ref
        End Function
    End Class
End Namespace
