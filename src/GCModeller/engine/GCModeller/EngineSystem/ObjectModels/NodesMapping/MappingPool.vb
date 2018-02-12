#Region "Microsoft.VisualBasic::c0d114928f9f47bce011b764d4bc2dd9, engine\GCModeller\EngineSystem\ObjectModels\NodesMapping\MappingPool.vb"

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

    '     Class MappingPool
    ' 
    '         Properties: Count, HandlerIdCollection, Values
    ' 
    '         Function: ContainsHandle, get_MappedNodes, GetEnumerator, GetEnumerator1, ModifyMapping
    '                   TryGetValue
    ' 
    '         Sub: (+2 Overloads) Dispose, UpdateCache, UpdateEnzymes
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.RuntimeObjects
Imports Microsoft.VisualBasic

Namespace EngineSystem.ObjectModels.PoolMappings

    ''' <summary>
    ''' Handler for the network node mapping in the evolution experiment.(节点映射管理器：突变与进化过程之中的映射关系)
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class MappingPool(Of IPoolHandleType As PoolMappings.IPoolHandle, EntityFeatureMapping As EngineSystem.ObjectModels.Feature.MappingFeature(Of IPoolHandleType))
        Inherits RuntimeObject

        Implements IReadOnlyDictionary(Of String, List(Of EntityFeatureMapping))
        Implements IDisposable

#Region "突变与进化过程之中的映射关系"

        ''' <summary>
        ''' Cache data
        ''' </summary>
        ''' <remarks></remarks>
        Protected _CACHE_MappingPool As EntityFeatureMapping()()
        Protected _MappingHandlers As IPoolHandleType()
        ''' <summary>
        ''' Nodes mapping dictionary data
        ''' </summary>
        ''' <remarks></remarks>
        Protected __mappingPool As Dictionary(Of String, List(Of EntityFeatureMapping))

#End Region

        Public Function get_MappedNodes(Handle As IPoolHandleType) As EntityFeatureMapping()
            Return _CACHE_MappingPool(Handle.Address)
        End Function

        ''' <summary>
        ''' 在执行了突变或者进化实验时候，假若改变了节点的映射关系的话，则需要调用本方法更新整个引擎之中的映射关系
        ''' </summary>
        ''' <param name="Edge"></param>
        ''' <remarks></remarks>
        Public Sub UpdateEnzymes(Edge As PoolMappings.IMappingEdge(Of IPoolHandleType, EntityFeatureMapping))
            Dim Nodes = _CACHE_MappingPool(Edge.MappingHandler.Address)
            Call Edge.set_Nodes(Nodes)
        End Sub

        Protected Sub UpdateCache()
            _CACHE_MappingPool = (From Line In __mappingPool.Values Select Line.ToArray).ToArray
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Node"></param>
        ''' <param name="NewClass">新的EC编号</param>
        ''' <returns></returns>
        ''' <remarks>这里不要使用并行化，因为需要使用<see cref="ModellingEngine.EngineSystem.ObjectModels.PoolMappings.MotifClass.Handle"></see>或者<see cref="ModellingEngine.EngineSystem.ObjectModels.PoolMappings.EnzymeClass.Handle"></see>进行映射操作</remarks>
        Public Function ModifyMapping(Node As EntityFeatureMapping, NewClass As String) As Boolean
            Dim ChunkBuffer = __mappingPool(Node.MappingHandler.locusId)
            Call ChunkBuffer.Remove(Node)
            ChunkBuffer = __mappingPool(NewClass)
            Call ChunkBuffer.Add(Node)
            Call UpdateCache()

            Return True
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of String, List(Of EntityFeatureMapping))) Implements IEnumerable(Of KeyValuePair(Of String, List(Of EntityFeatureMapping))).GetEnumerator
            For Each Item As KeyValuePair(Of String, List(Of EntityFeatureMapping)) In __mappingPool
                Yield Item
            Next
        End Function

        Public ReadOnly Property Count As Integer Implements IReadOnlyCollection(Of KeyValuePair(Of String, List(Of EntityFeatureMapping))).Count
            Get
                Return __mappingPool.Count
            End Get
        End Property

        Public Function ContainsHandle(HandlerId As String) As Boolean Implements IReadOnlyDictionary(Of String, List(Of EntityFeatureMapping)).ContainsKey
            Return __mappingPool.ContainsKey(HandlerId)
        End Function

        Default Public ReadOnly Property Item(key As String) As List(Of EntityFeatureMapping) Implements IReadOnlyDictionary(Of String, List(Of EntityFeatureMapping)).Item
            Get
                Return __mappingPool(key)
            End Get
        End Property

        Public ReadOnly Property HandlerIdCollection As IEnumerable(Of String) Implements IReadOnlyDictionary(Of String, List(Of EntityFeatureMapping)).Keys
            Get
                Return __mappingPool.Keys
            End Get
        End Property

        Public Function TryGetValue(key As String, ByRef value As List(Of EntityFeatureMapping)) As Boolean Implements IReadOnlyDictionary(Of String, List(Of EntityFeatureMapping)).TryGetValue
            Return __mappingPool.TryGetValue(key, value)
        End Function

        Public ReadOnly Property Values As IEnumerable(Of List(Of EntityFeatureMapping)) Implements IReadOnlyDictionary(Of String, List(Of EntityFeatureMapping)).Values
            Get
                Return __mappingPool.Values
            End Get
        End Property

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose( disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose( disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
