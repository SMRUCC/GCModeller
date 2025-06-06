#Region "Microsoft.VisualBasic::bee7241748060dc113cf4bedae3fdc44, core\Bio.Assembly\SequenceModel\NucleicAcid\Bases.vb"

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
    '    Code Lines: 24 (64.86%)
    ' Comment Lines: 7 (18.92%)
    '    - Xml Docs: 85.71%
    ' 
    '   Blank Lines: 6 (16.22%)
    '     File Size: 1.53 KB


    '     Module Bases
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.ComponentModel

Namespace SequenceModel.NucleotideModels

    Module Bases

        ''' <summary>
        ''' Formula data of each DNA bases + degenerate bases
        ''' </summary>
        Public ReadOnly Deoxyribonucleotide As New Dictionary(Of Char, FormulaData) From {
            {"A"c, PeriodicTable.SimpleParser("C10H12N5O6P") + FormulaData.H2O},
            {"T"c, PeriodicTable.SimpleParser("C10H13N2O8P") + FormulaData.H2O},
            {"C"c, PeriodicTable.SimpleParser("C9H12N3O7P") + FormulaData.H2O},
            {"G"c, PeriodicTable.SimpleParser("C10H12N5O7P") + FormulaData.H2O}
        }

        ''' <summary>
        ''' Formula data of each RNA bases
        ''' </summary>
        Public ReadOnly Ribonucleotide As New Dictionary(Of Char, FormulaData) From {
            {"A"c, PeriodicTable.SimpleParser("C10H12N5O7P")},
            {"U"c, PeriodicTable.SimpleParser("C9H11N2O9P")},
            {"C"c, PeriodicTable.SimpleParser("C9H12N3O8P")},
            {"G"c, PeriodicTable.SimpleParser("C10H12N5O8P")}
        }

        Sub New()
            ' processing of degenerate bases for DNA sequence
            For Each var In New DegenerateBasesExtensions().DegenerateBases
                Dim sum As FormulaData = var.Value.Select(Function(c) Deoxyribonucleotide(c)).Sum
                Dim nsize As Integer = var.Value.Length

                Deoxyribonucleotide(var.Key) = sum / nsize
            Next
        End Sub
    End Module
End Namespace
