#Region "Microsoft.VisualBasic::ae789282944dbd2d30c9d8ad5ec9d4f3, analysis\Metagenome\Metagenome\Enterotype.vb"

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

    '   Total Lines: 60
    '    Code Lines: 46 (76.67%)
    ' Comment Lines: 9 (15.00%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 5 (8.33%)
    '     File Size: 2.49 KB


    ' Module Enterotype
    ' 
    '     Function: JSD, PAMclustering
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Math.Correlations
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.DataMining.BinaryTree
Imports Microsoft.VisualBasic.ComponentModel.Collection

''' <summary>
''' Protocol module to produce enterotype clusters
''' </summary>
Public Module Enterotype

    ''' <summary>
    ''' First, the abundances of classified genera are used to 
    ''' produce a JSD matrix between samples.
    ''' </summary>
    ''' <param name="abundances"></param>
    ''' <returns>A JSD correlation matrix between samples.</returns>
    <Extension>
    Public Iterator Function JSD(abundances As IEnumerable(Of DataSet), Optional parallel As Boolean = True) As IEnumerable(Of DataSet)
        Dim matrix As DataSet() = abundances.ToArray
        Dim taxonomy As String() = matrix.PropertyNames
        Dim jsdMatrix = From sample As DataSet
                        In matrix.Populate(parallel:=parallel)
                        Let P As Double() = sample(taxonomy)
                        Let jsdi As Dictionary(Of String, Double) = matrix.ToDictionary(
                            Function(another) another.ID,
                            Function(another)
                                Dim Q As Double() = another(taxonomy)
                                Return Correlations.JSD(P, Q)
                            End Function)
                        Select New DataSet With {
                            .ID = sample.ID,
                            .Properties = jsdi
                        }

        For Each sample As DataSet In jsdMatrix
            Yield sample
        Next
    End Function

    <Extension>
    Public Function PAMclustering(JSD As IEnumerable(Of DataSet), Optional maxIterations% = 1000) As EntityClusterModel()
        Dim JSDMatrix = JSD.ToArray
        Dim sampleNames$() = JSDMatrix.PropertyNames
        Dim entities As ClusterEntity() = JSDMatrix _
            .Select(Function(d)
                        Return New ClusterEntity With {
                            .uid = d.ID,
                            .entityVector = d(sampleNames)
                        }
                    End Function) _
            .ToArray
        Dim clusters = entities.DoKMedoids(3, maxIterations)
        Dim result = clusters _
            .Select(Function(c) c.ToDataModel(sampleNames)) _
            .ToArray

        Return result
    End Function

    <Extension>
    Public Function BuildClusterTree(table As OTUTable(), Optional equals As Double = 0.85, Optional gt As Double = 0.6) As BTreeCluster
        Dim jsd As New OTUComparer(table, equals, gt)
        Dim tree As BTreeCluster = table.Keys.BTreeCluster(alignment:=jsd)
        Return tree
    End Function
End Module
