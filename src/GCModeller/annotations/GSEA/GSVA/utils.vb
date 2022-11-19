#Region "Microsoft.VisualBasic::4bcfff424c07d0d5809c31f37f9d2dcd, GCModeller\annotations\GSEA\GSVA\utils.vb"

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

    '   Total Lines: 67
    '    Code Lines: 42
    ' Comment Lines: 15
    '   Blank Lines: 10
    '     File Size: 2.78 KB


    ' Module utils
    ' 
    '     Function: filterFeatures, filterGeneSets, mapGeneSetsToFeatures
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.GSEA

Module utils

    Public Function filterGeneSets(mapped_gset_idx_list As Dictionary(Of String, String()), min_sz As Integer, max_sz As Integer) As Dictionary(Of String, String())
        Return mapped_gset_idx_list _
            .Where(Function(d)
                       Return d.Value.Length >= min_sz AndAlso d.Value.Length <= max_sz
                   End Function) _
            .ToDictionary
    End Function

    ''' <summary>
    ''' filter out genes with constant expression values
    ''' </summary>
    ''' <param name="expr"></param>
    ''' <param name="method"></param>
    ''' <returns></returns>
    Public Function filterFeatures(expr As Matrix, method As Methods) As Matrix
        Dim sdGenes As Vector = expr.rowSds.Values.AsVector

        sdGenes(sdGenes < 0.0000000001) = Vector.Zero

        If sdGenes.Any(Function(x) x = 0.0 OrElse x.IsNaNImaginary) Then
            Call $"{(sdGenes = 0.0 OrElse sdGenes.IsNaNImaginary).Sum} genes with constant expression values throuhgout the samples.".Warning

            If method <> Methods.ssgsea Then
                Call "Since argument method!='ssgsea', genes with constant expression values are discarded.".Warning

                expr = expr(sdGenes > 0 & Not sdGenes.IsNaNImaginary)
            End If
        End If

        If expr.size < 2 Then
            Throw New InvalidProgramException("Less than two genes in the input assay object")
        Else
            Return expr
        End If
    End Function

    ''' <summary>
    ''' maps gene sets content in 'gsets' to 'features', where 'gsets'
    ''' Is a 'list' object with character string vectors as elements,
    ''' And 'features' is a character string vector object. it assumes
    ''' features In both input objects follow the same nomenclature
    ''' </summary>
    ''' <param name="gsets"></param>
    ''' <param name="features"></param>
    ''' <returns></returns>
    Public Function mapGeneSetsToFeatures(gsets As Background, features As String()) As Dictionary(Of String, String())
        Dim mapdgenesets = gsets.clusters _
            .ToDictionary(Function(c) c.ID,
                          Function(c)
                              Return c.Intersect(features).ToArray
                          End Function)

        If mapdgenesets.Values.IteratesALL.Count = 0 Then
            Throw New InvalidProgramException("No identifiers in the gene sets could be matched to the identifiers in the expression data.")
        Else
            Return mapdgenesets
        End If
    End Function
End Module
