#Region "Microsoft.VisualBasic::ff07f1bbcfe5d4dfa87662af6ad266a5, core\Bio.Assembly\MetabolicModel\KEGGConvertor.vb"

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

    '   Total Lines: 33
    '    Code Lines: 28 (84.85%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 5 (15.15%)
    '     File Size: 1.25 KB


    '     Module KEGGConvertor
    ' 
    '         Function: ConvertCompound, ConvertReaction
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes

Namespace MetabolicModel

    Public Module KEGGConvertor

        Public Function ConvertReaction(reaction As Reaction) As MetabolicReaction
            Dim eq As Equation = reaction.ReactionModel

            Return New MetabolicReaction With {
                .ECNumbers = reaction.Enzyme,
                .id = reaction.ID,
                .is_reversible = reaction.Reversible,
                .is_spontaneous = Not .ECNumbers.IsNullOrEmpty,
                .name = reaction.CommonNames.JoinBy("; "),
                .description = reaction.Definition,
                .left = eq.Reactants,
                .right = eq.Products
            }
        End Function

        Public Function ConvertCompound(compound As Compound) As MetabolicCompound
            Return New MetabolicCompound With {
                .formula = compound.formula,
                .id = compound.entry,
                .moleculeWeight = compound.exactMass,
                .name = compound.commonNames.JoinBy("; "),
                .xref = compound.DbLinks
            }
        End Function
    End Module
End Namespace
