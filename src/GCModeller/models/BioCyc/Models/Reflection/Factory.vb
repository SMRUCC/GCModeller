#Region "Microsoft.VisualBasic::c7f6c667eef5325504777d897873db1f, GCModeller\models\BioCyc\Models\Reflection\Factory.vb"

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

    '   Total Lines: 50
    '    Code Lines: 40
    ' Comment Lines: 0
    '   Blank Lines: 10
    '     File Size: 1.69 KB


    ' Module Factory
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: ParseCompoundReference, ParseKineticsFactor, ParseReactionDirection
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.Data.BioCyc.Assembly.MetaCyc.Schema.Metabolism

Module Factory

    ReadOnly reactionDirections As New Dictionary(Of String, ReactionDirections)

    Sub New()
        For Each flag As ReactionDirections In Enums(Of ReactionDirections)()
            reactionDirections(flag.Description) = flag
        Next
    End Sub

    Public Function ParseReactionDirection(value As ValueString) As ReactionDirections
        Return reactionDirections(value.value)
    End Function

    Public Function ParseCompoundReference(value As ValueString) As CompoundSpecieReference
        Dim coef As String
        Dim ref As New CompoundSpecieReference With {
            .ID = value.value,
            .StoiChiometry = 1
        }
        Dim factor As Double

        ref.Compartment = value("COMPARTMENT")
        coef = value("COEFFICIENT")

        If Not coef.StringEmpty Then
            If coef = "n" OrElse coef = "N" OrElse coef Like "n*" OrElse coef Like "*n" Then
                ref.StoiChiometry = Double.PositiveInfinity
            ElseIf Double.TryParse(coef, result:=factor) Then
                ref.StoiChiometry = factor
            Else
                ref.StoiChiometry = 0
            End If
        End If

        Return ref
    End Function

    Public Function ParseKineticsFactor(value As ValueString) As KineticsFactor
        Return New KineticsFactor With {
           .Km = Double.Parse(value.value),
           .citations = value.getAttributes("CITATIONS"),
           .substrate = value("SUBSTRATE")
       }
    End Function

End Module
