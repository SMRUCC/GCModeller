Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Net
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Net.Protocols
Imports Microsoft.VisualBasic.Parallel
Imports sciBASIC.ComputingServices.TaskHost

Namespace Cluster

    ''' <summary>
    ''' Client
    ''' </summary>
    Public Class Master

        ''' <summary>
        ''' Online avaliable nodes in this server cluster.
        ''' </summary>
        Dim _nodes As New Dictionary(Of TaskRemote)
        Dim node_port%
        Dim net$

        ''' <summary>
        ''' 返回节点的数量
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Nodes As Integer
            Get
                Return _nodes.Count
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="net$"><see cref="EnumerateAddress"/></param>
        ''' <param name="node_port%"></param>
        Sub New(net$, node_port%)
            Me.node_port = node_port
            Me.net = net
        End Sub

        Public Sub Scan()
            Dim request As New RequestStream(Protocols.ProtocolEntry, TaskProtocols.Handshake)

            For Each IP$ In EnumerateAddress(Me.net)
                Dim response = New AsynInvoke(IP, node_port) _
                    .SendMessage(request, 200)

                SyncLock _nodes

                    If response.Protocol = HTTP_RFC.RFC_OK Then
                        If Not _nodes.ContainsKey(IP) Then
                            Call _nodes.Add(New TaskRemote(IP, node_port))
                            Call $"Add new node: {IP}".__DEBUG_ECHO
                        End If
                    Else
                        If _nodes.ContainsKey(IP) Then
                            Call _nodes.Remove(IP)
                            Call $"Removes offline node: {IP}".__DEBUG_ECHO
                        End If
                    End If
                End SyncLock
            Next
        End Sub

        ''' <summary>
        ''' 返回负载量最低的节点
        ''' </summary>
        ''' <returns></returns>
        Public Function GetPreferNode() As TaskRemote
            SyncLock _nodes

                Return LinqAPI.DefaultFirst(Of TaskRemote) <=
 _
                    From node As TaskRemote
                    In _nodes.Values.AsParallel
                    Select node,
                        node.Load
                    Order By Load Ascending

            End SyncLock
        End Function

        ''' <summary>
        ''' 自动分配空闲的计算节点
        ''' </summary>
        ''' <param name="target"></param>
        ''' <param name="args"></param>
        Public Function Invoke(target As [Delegate], ParamArray args As Object()) As Object
            Dim node As TaskRemote = GetPreferNode()
            Dim out As Object = node.Invoke(target, args)
            Return out
        End Function

        Public Iterator Function [Select](Of T, Tout)(source As IEnumerable(Of T), task As Func(Of T, Tout), args As Object()) As IEnumerable(Of Tout)
            Dim node As TaskRemote = GetPreferNode()
            Dim out As ILinq(Of Tout) = node.Select(Of T, Tout)(task, source.ToArray, args)

            For Each x As Tout In out
                Yield x
            Next
        End Function

        Public Sub ScanTask()
            Call RunTask(AddressOf Scan)
        End Sub
    End Class
End Namespace