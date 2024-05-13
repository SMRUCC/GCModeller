#Region "Microsoft.VisualBasic::e65024210a373a8938b91b8f2412a92f, core\Bio.Assembly\MetabolicModel\Models\MetabolicCompound.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 22
    '    Code Lines: 17
    ' Comment Lines: 0
    '   Blank Lines: 5
    '     File Size: 868 B


    '     Class MetabolicCompound
    ' 
    '         Properties: formula, id, moleculeWeight, name, synonym
    '                     xref
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

Namespace MetabolicModel

    Public Class MetabolicCompound : Implements INamedValue, IMolecule

        Public Property id As String Implements IKeyedEntity(Of String).Key, IMolecule.EntryId
        Public Property name As String Implements IMolecule.Name
        Public Property synonym As String()
        Public Property formula As String Implements IMolecule.Formula
        Public Property moleculeWeight As Double Implements IMolecule.Mass
        Public Property xref As DBLink()

        Public Overrides Function ToString() As String
            Return name
        End Function

    End Class
End Namespace
