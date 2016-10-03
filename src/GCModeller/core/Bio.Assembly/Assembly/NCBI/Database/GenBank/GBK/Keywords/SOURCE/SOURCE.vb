#Region "Microsoft.VisualBasic::1a0c5f39f86825ebde493becee9f4b19, ..\GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\GenBank\GBK\Keywords\SOURCE\SOURCE.vb"

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

    Public Class SOURCE : Inherits KeyWord

        Public Property SpeciesName As String
        Public Property OrganismHierarchy As ORGANISM

        Public Overrides Function ToString() As String
            Return OrganismHierarchy.ToString
        End Function

        Public Shared Widening Operator CType(str As String()) As SOURCE
            Dim Source As SOURCE = New SOURCE
            If Not str.IsNullOrEmpty Then
                Call __trimHeadKey(str)
                Source.SpeciesName = str.First
                Source.OrganismHierarchy = ORGANISM.InternalParser(str.Skip(1).ToArray)
            End If
            Return Source
        End Operator
    End Class
End Namespace
