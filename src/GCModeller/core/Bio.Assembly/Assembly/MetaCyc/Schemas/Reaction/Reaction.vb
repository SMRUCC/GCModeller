#Region "Microsoft.VisualBasic::0f28e2e0e9939fa9246be1670ec2f2d5, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\Schemas\Reaction\Reaction.vb"

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

    '   Total Lines: 44
    '    Code Lines: 34
    ' Comment Lines: 0
    '   Blank Lines: 10
    '     File Size: 2.31 KB


    '     Class Compound
    ' 
    '         Properties: StoiChiometry
    ' 
    '     Class Reaction
    ' 
    '         Properties: Equation, Products, Reactants, Reversible
    ' 
    '         Function: [DirectCast], CreateObject
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic

Namespace Assembly.MetaCyc.Schema.Metabolism

    Public Class Compound : Inherits MetaCyc.File.DataFiles.Slots.Object
        Implements ComponentModel.EquaionModel.ICompoundSpecies

        Public Property StoiChiometry As Double Implements ComponentModel.EquaionModel.ICompoundSpecies.StoiChiometry
    End Class

    Public Class Reaction : Inherits MetaCyc.File.DataFiles.Slots.Object
        Implements ComponentModel.EquaionModel.IEquation(Of Compound)

        Protected Friend _InnerBaseType As MetaCyc.File.DataFiles.Slots.Reaction
        Protected Friend _strEquation As String

        Public ReadOnly Property Equation As String
            Get
                Return _strEquation
            End Get
        End Property

        Public Property Reactants As Compound() Implements ComponentModel.EquaionModel.IEquation(Of Compound).Reactants
        Public Property Reversible As Boolean Implements ComponentModel.EquaionModel.IEquation(Of Compound).Reversible
        Public Property Products As Compound() Implements ComponentModel.EquaionModel.IEquation(Of Compound).Products

        Public Shared Function CreateObject(FileObject As MetaCyc.File.DataFiles.Slots.Reaction) As Reaction
            Dim SchemaModel As Reaction = New Reaction With {._InnerBaseType = FileObject}
            Call FileObject.CopyTo(Of Reaction)(SchemaModel)
            SchemaModel.Reversible = String.Equals(FileObject.ReactionDirection, "REVERSIBLE")
            SchemaModel.Reactants = (From Id As String In FileObject.Left Select New Compound With {.Identifier = Id, .StoiChiometry = 1}).ToArray
            SchemaModel.Products = (From Id As String In FileObject.Right Select New Compound With {.Identifier = Id, .StoiChiometry = 1}).ToArray
            SchemaModel._strEquation = ComponentModel.EquaionModel.ToString(Of Compound)(SchemaModel)

            Return SchemaModel
        End Function

        Public Shared Function [DirectCast](Reactions As MetaCyc.File.DataFiles.Reactions) As Reaction()
            Dim LQuery = (From item In Reactions.AsParallel Select CreateObject(item)).ToArray
            Call Console.WriteLine("Complete object type cast!")
            Return LQuery
        End Function
    End Class
End Namespace
