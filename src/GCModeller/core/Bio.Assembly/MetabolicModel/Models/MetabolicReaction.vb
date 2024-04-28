#Region "Microsoft.VisualBasic::11333bc247e4868d97c3722c283173ca, G:/GCModeller/src/GCModeller/core/Bio.Assembly//MetabolicModel/Models/MetabolicReaction.vb"

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

    '   Total Lines: 37
    '    Code Lines: 20
    ' Comment Lines: 11
    '   Blank Lines: 6
    '     File Size: 1.57 KB


    '     Class MetabolicReaction
    ' 
    '         Properties: description, ECNumbers, id, is_reversible, is_spontaneous
    '                     left, name, right
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes

Namespace MetabolicModel

    ''' <summary>
    ''' A unify reaction model in the GCModeller system
    ''' </summary>
    Public Class MetabolicReaction : Implements INamedValue
        Implements IEquation(Of CompoundSpecieReference)

        Public Property id As String Implements IKeyedEntity(Of String).Key
        Public Property name As String
        Public Property description As String
        Public Property left As CompoundSpecieReference() Implements IEquation(Of CompoundSpecieReference).Reactants
        Public Property right As CompoundSpecieReference() Implements IEquation(Of CompoundSpecieReference).Products

        ''' <summary>
        ''' if not is reversible, then the reaction direction is left to right by default
        ''' </summary>
        ''' <returns></returns>
        Public Property is_reversible As Boolean Implements IEquation(Of CompoundSpecieReference).Reversible
        ''' <summary>
        ''' could be react no required of the enzymatic
        ''' </summary>
        ''' <returns></returns>
        Public Property is_spontaneous As Boolean
        Public Property ECNumbers As String()

        Public Overrides Function ToString() As String
            Return $"[{id}] {name}"
        End Function

    End Class
End Namespace
