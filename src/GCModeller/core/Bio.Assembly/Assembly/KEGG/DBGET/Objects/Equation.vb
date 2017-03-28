#Region "Microsoft.VisualBasic::36e277f341a731d55dacce0717d18c62, ..\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Equation.vb"

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

Imports System.Text

Namespace Assembly.KEGG.DBGET.bGetObject

    Public Class Equation
        Public Class SpeciesReference
            Public Property AccessionId As String
            Public Property Stoichiometric As Integer

            Public Overrides Function ToString() As String
                Return String.Format("{0} {1}", Stoichiometric, AccessionId)
            End Function
        End Class

        Public Property Left As SpeciesReference()
        Public Property Right As SpeciesReference()
        Public Property Reversible As Boolean

        Public Overrides Function ToString() As String
            Dim sBuilder As StringBuilder = New StringBuilder(1024)
            For Each Item In Left
                Call sBuilder.Append(Item.ToString & " + ")
            Next
            Call sBuilder.Remove(sBuilder.Length - 3, 3)
            If Reversible Then
                Call sBuilder.Append(" <=> ")
            Else
                Call sBuilder.Append(" ==> ")
            End If

            For Each Item In Right
                Call sBuilder.Append(Item.ToString, " + ")
            Next
            Call sBuilder.Remove(sBuilder.Length - 3, 3)

            Return sBuilder.ToString
        End Function
    End Class
End Namespace
