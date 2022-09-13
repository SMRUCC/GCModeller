#Region "Microsoft.VisualBasic::ffd85abe93ebab5394aa58a84a8d5a2a, GCModeller\engine\Model\Extensions.vb"

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

    '   Total Lines: 65
    '    Code Lines: 56
    ' Comment Lines: 0
    '   Blank Lines: 9
    '     File Size: 1.97 KB


    ' Module Extensions
    ' 
    '     Function: (+2 Overloads) CreateVector, EvalEffects, ProteinFromVector, RNAFromVector
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Vector

<HideModuleName>
Public Module Extensions

    <Extension>
    Public Function EvalEffects(regMode As String) As Double
        If regMode.StringEmpty Then
            Return 0.25
        End If

        If regMode.TextEquals("repressor") Then
            Return -1
        ElseIf regMode.TextEquals("activator") Then
            Return 1
        Else
            Return 0.25
        End If
    End Function

    <Extension>
    Public Function CreateVector(protein As ProteinComposition) As NumericVector
        Return New NumericVector With {
            .name = protein.proteinID,
            .vector = ProteinComposition.aa _
                .Select(Function(p) CDbl(p.GetValue(protein))) _
                .ToArray
        }
    End Function

    Public Function ProteinFromVector(vector As NumericVector) As ProteinComposition
        Dim protein As New ProteinComposition With {
            .proteinID = vector.name
        }
        Dim i As i32 = Scan0

        For Each aa As PropertyInfo In ProteinComposition.aa
            Call aa.SetValue(protein, CInt(vector(++i)))
        Next

        Return protein
    End Function

    <Extension>
    Public Function CreateVector(rna As RNAComposition) As NumericVector
        Return New NumericVector With {
            .name = rna.geneID,
            .vector = {rna.A, rna.C, rna.G, rna.U}
        }
    End Function

    Public Function RNAFromVector(vector As NumericVector) As RNAComposition
        Return New RNAComposition With {
            .geneID = vector.name,
            .A = vector(0),
            .C = vector(1),
            .G = vector(2),
            .U = vector(3)
        }
    End Function
End Module
