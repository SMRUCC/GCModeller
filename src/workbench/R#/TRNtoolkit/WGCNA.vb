#Region "Microsoft.VisualBasic::8b21d77b637ccd27f9c13293e61123aa, R#\TRNtoolkit\WGCNA.vb"

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

    '   Total Lines: 73
    '    Code Lines: 47 (64.38%)
    ' Comment Lines: 15 (20.55%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 11 (15.07%)
    '     File Size: 2.68 KB


    ' Module WGCNA
    ' 
    '     Function: CorrelationNetwork, FilterRegulation
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools.WGCNA.Network
Imports SMRUCC.genomics.Model.Network.Regulons
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports any = Microsoft.VisualBasic.Scripting
Imports HTSMatrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm

<Package("WGCNA")>
Module WGCNA

    ''' <summary>
    ''' filter regulation network by WGCNA result weights
    ''' </summary>
    ''' <param name="g"></param>
    ''' <param name="WGCNA"></param>
    ''' <param name="threshold"></param>
    ''' <returns></returns>
    <ExportAPI("shapeTRN")>
    Public Function FilterRegulation(g As NetworkGraph, WGCNA As WGCNAWeight, Optional threshold As Double = 0.3) As Object
        Dim w As Double

        For Each edge As Edge In g.graphEdges.ToArray
            w = WGCNA.GetValue(edge.U.label, edge.V.label)

            If w < threshold Then
                g.RemoveEdge(edge)
            Else
                edge.weight = w
            End If
        Next

        Return g
    End Function

    ''' <summary>
    ''' append protein iteration network based on the WGCNA weights.
    ''' </summary>
    ''' <param name="g"></param>
    ''' <param name="WGCNA"></param>
    ''' <param name="modules"></param>
    ''' <param name="threshold"></param>
    ''' <returns></returns>
    <ExportAPI("interations")>
    Public Function CorrelationNetwork(g As NetworkGraph, WGCNA As WGCNAWeight, modules As list, Optional threshold As Double = 0.3) As Object
        For Each conn As Weight In WGCNA.AsEnumerable.Where(Function(cn) cn.weight >= threshold)
            Dim u As Node = g.GetElementByID(conn.fromNode)
            Dim v As Node = g.GetElementByID(conn.toNode)

            If u Is Nothing OrElse v Is Nothing Then
                Continue For
            End If

            Dim edges As Edge() = g.GetEdges(u, v).SafeQuery.ToArray
            Dim data As New EdgeData
            Dim color1 As String = any.ToString(modules.slots(u.label))
            Dim color2 As String = any.ToString(modules.slots(v.label))

            data(NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE) = If(color1 = color2, color1, $"{color1}+{color2}")

            If edges.Length = 0 Then
                g.CreateEdge(u, v, conn.weight, data)
            Else
                For Each link In edges
                    link.weight += conn.weight
                Next
            End If
        Next

        Return g
    End Function

    <ExportAPI("expr_cor")>
    <RApiReturn(GetType(LazyCorrelationMatrix), GetType(dataframe))>
    Public Function expr_cor(expr As Object,
                             <RRawVectorArgument(TypeCodes.string)> Optional id1 As Object = Nothing,
                             <RRawVectorArgument(TypeCodes.string)> Optional id2 As Object = Nothing,
                             Optional env As Environment = Nothing) As Object

        Dim check_eval As Boolean = id1 IsNot Nothing AndAlso id2 IsNot Nothing

        If check_eval Then
            If Not TypeOf expr Is LazyCorrelationMatrix Then
                Return RInternal.debug.stop("the given data expr object should be a correlation matrix object when id1 and id2 is presentes!", env)
            End If

            Dim cor As LazyCorrelationMatrix = DirectCast(expr, LazyCorrelationMatrix)
            Dim idset1 As GetVectorElement = GetVectorElement.Create(Of String)(id1)
            Dim idset2 As GetVectorElement = GetVectorElement.Create(Of String)(id2)

            If Not GetVectorElement.DoesSizeMatch(idset1, idset2) Then
                Return RInternal.debug.stop($"the dimension size of id1({idset1.size}) is not matched with the dimension size of id2({idset2.size})!", env)
            Else
                Return expr_cor(cor, idset1, idset2)
            End If
        ElseIf TypeOf expr Is HTSMatrix Then
            Return New LazyCorrelationMatrix(DirectCast(expr, HTSMatrix))
        Else
            Return Message.InCompatibleType(GetType(HTSMatrix), expr.GetType, env)
        End If
    End Function

    Private Function expr_cor(cor As LazyCorrelationMatrix, idset1 As GetVectorElement, idset2 As GetVectorElement) As dataframe
        Dim eval As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }
        Dim id1vec As New List(Of String)
        Dim id2vec As New List(Of String)
        Dim corvec As New List(Of Double)
        Dim pvalvec As New List(Of Double)

        For Each tuple In TqdmWrapper.Wrap(GetVectorElement.Zip(idset1, idset2).ToArray)
            Dim x As String = CStr(tuple.Item1)
            Dim y As String = CStr(tuple.Item2)
            Dim corResult = cor.Correlation(x, y)

            Call id1vec.Add(x)
            Call id2vec.Add(y)
            Call corvec.Add(corResult.cor)
            Call pvalvec.Add(corResult.pval)
        Next

        Call eval.add("id1", id1vec)
        Call eval.add("id2", id2vec)
        Call eval.add("cor", corvec)
        Call eval.add("pval", pvalvec)

        Return eval
    End Function
End Module
