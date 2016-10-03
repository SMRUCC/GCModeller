#Region "Microsoft.VisualBasic::b9931ebedb3115a9b1a8b3de741dcd86, ..\GCModeller\data\Reactome\ObjectModels\Metabolite.vb"

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

Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace ObjectModels

    Public Class Metabolite : Implements IMetabolite, sIdEnumerable

        Public Property ChEBI As String() Implements IMetabolite.ChEBI
        Public Property KEGGCompound As String Implements IMetabolite.KEGGCompound

        Public Property Identifier As String Implements sIdEnumerable.Identifier
        Public Property CommonNames As String()
        Public Property MetaboliteType As MetaboliteTypes

        Public Enum MetaboliteTypes
            SmallMolecule
            Complex
        End Enum

        Public Overrides Function ToString() As String
            Return String.Format("[{0}]  {1}", MetaboliteType.ToString, Identifier)
        End Function
    End Class
End Namespace
