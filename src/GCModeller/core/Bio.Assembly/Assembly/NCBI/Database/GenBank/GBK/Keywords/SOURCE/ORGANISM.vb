#Region "Microsoft.VisualBasic::24f944ef2fc4f40724ee16df71e6f4f1, ..\core\Bio.Assembly\Assembly\NCBI\Database\GenBank\GBK\Keywords\SOURCE\ORGANISM.vb"

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

Namespace Assembly.NCBI.GenBank.GBFF.Keywords

    Public Class ORGANISM

        Public Property Categorys As String()
        Public Property SpeciesName As String

        Public Overrides Function ToString() As String
            Return SpeciesName
        End Function

        Public Shared Function InternalParser(str As String()) As ORGANISM
            Call KeyWord.__trimHeadKey(str)
            Dim Org As ORGANISM = New ORGANISM With {.SpeciesName = str.First}
            Org.Categorys = Strings.Split(String.Join(" ", (From s As String In str.Skip(1) Select s.Trim).ToArray), "; ")
            Return Org
        End Function
    End Class
End Namespace
