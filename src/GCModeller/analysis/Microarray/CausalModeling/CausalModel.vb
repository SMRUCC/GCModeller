#Region "Microsoft.VisualBasic::a4b67b0e463a499a2efd04b825480a8c, analysis\Microarray\CausalModeling\CausalModel.vb"

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

    '   Total Lines: 87
    '    Code Lines: 65 (74.71%)
    ' Comment Lines: 5 (5.75%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 17 (19.54%)
    '     File Size: 3.34 KB


    ' Class CausalModel
    ' 
    '     Properties: data, latentDefs, paths, sampleIds, varNames
    ' 
    '     Function: AsPathTuple, (+2 Overloads) Create, MakeDataFrame
    '     Class Path
    ' 
    '         Properties: U, V
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Data.GraphTheory.Network
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Public Class CausalModel : Implements IndexGraph(Of Path)

    Public Class Path : Implements IndexEdge

        Public Property U As Integer Implements IndexEdge.U
        Public Property V As Integer Implements IndexEdge.V

        Sub New()
        End Sub

        Friend Sub New(i As Integer, j As Integer)
            U = i
            V = j
        End Sub

        Public Overrides Function ToString() As String
            Return $"[U#{U} -> V#{V}]"
        End Function

    End Class

    Public Property data As Double(,)
    Public Property varNames As String() Implements IndexGraph(Of Path).Nodes
    ''' <summary>
    ''' 1. for <see cref="latentDefs"/> definition is missing, node index is reference to the <see cref="varNames"/> elements
    ''' 2. for <see cref="latentDefs"/> definition is presentes, node index is reference to the <see cref="latentDefs"/> elements
    ''' </summary>
    ''' <returns></returns>
    Public Property paths As Path() Implements IndexGraph(Of Path).Edges
    Public Property sampleIds As String()
    Public Property latentDefs As LatentDefinition()

    Friend Iterator Function AsPathTuple() As IEnumerable(Of (from As Integer, [to] As Integer))
        For Each node As Path In paths.SafeQuery
            Yield (node.U, node.V)
        Next
    End Function

    Public Shared Function Create(Of T As SparseGraph.IInteraction)(x As Matrix, paths As IEnumerable(Of T), latents As IEnumerable(Of LatentDefinition)) As CausalModel
        Dim tensor As Double(,) = x.AsTensorArray
        Dim latentVars = latents.ToArray
        Dim latentNodes As String() = latentVars.Select(Function(i) i.varName).ToArray
        Dim model As CausalModel = IndexGraphExtensions.FromNetwork(Of T, Path, CausalModel)(paths, latentNodes.Indexing, Function(i, j) New Path(i, j))
        model.data = tensor
        model.varNames = x.rownames
        model.sampleIds = x.sampleID
        model.latentDefs = latentVars
        Return model
    End Function

    Public Shared Function Create(Of T As SparseGraph.IInteraction)(x As Matrix, paths As IEnumerable(Of T)) As CausalModel
        Dim tensor As Double(,) = x.AsTensorArray
        Dim model As CausalModel = IndexGraphExtensions.FromNetwork(Of T, Path, CausalModel)(paths, x.rownames.Indexing, Function(i, j) New Path(i, j))
        model.data = tensor
        model.sampleIds = x.sampleID
        Return model
    End Function

    Public Function MakeDataFrame() As DataFrame
        Dim df As New DataFrame With {
            .rownames = sampleIds,
            .features = New Dictionary(Of String, FeatureVector)
        }
        Dim n As Integer = data.GetLength(0)
        Dim p As Integer = data.GetLength(1)

        For j As Integer = 0 To p - 1
            Dim col(n - 1) As Double

            For i As Integer = 0 To n - 1
                col(i) = data(i, j)
            Next

            Call df.add(varNames(j), col)
        Next

        Return df
    End Function

End Class

