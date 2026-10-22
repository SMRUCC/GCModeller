Imports System.Collections.Generic

Namespace Enumeration

    ''' <summary>
    ''' The exhaustive enumerator of the network topology space of the three
    ''' node enzyme network, together with the structural pre filters of the
    ''' paper.
    ''' (三节点酶网络拓扑空间的穷举枚举器与结构预过滤器)
    ''' </summary>
    ''' <remarks>
    ''' The complete space is ``3^9 = 19683`` sign vectors. The paper keeps only
    ''' the topologies in which the input node A can reach the output node C
    ''' through the active links (16,038 topologies), because a network that
    ''' can not transmit the input to the output can never respond to it.
    ''' The forward search strategy of the paper starts from the minimal
    ''' networks with at most three links, which is the
    ''' <see cref="EnumerateUpTo(Integer)"/> entry point.
    ''' </remarks>
    Public Module TopologyEnumerator

        ''' <summary>
        ''' Enumerate every topology of the three node network, all of the
        ''' 19,683 sign vectors in the identifier order.
        ''' (穷举全部 19683 个拓扑)
        ''' </summary>
        Public Iterator Function EnumerateAll() As IEnumerable(Of Model.NetworkTopology)
            For id As Integer = 0 To Model.NetworkTopology.TotalCount - 1
                Yield Model.NetworkTopology.FromId(id)
            Next
        End Function

        ''' <summary>
        ''' Enumerate the topologies in which the input node A can reach the
        ''' output node C, i.e. the 16,038 feasible topologies of the paper.
        ''' (枚举输入到输出因果连通的可行拓扑)
        ''' </summary>
        Public Iterator Function EnumerateConnected() As IEnumerable(Of Model.NetworkTopology)
            For Each topo As Model.NetworkTopology In EnumerateAll()
                If topo.HasInputOutputPath Then
                    Yield topo
                End If
            Next
        End Function

        ''' <summary>
        ''' Enumerate the connected topologies that own at most the given number
        ''' of active links. This is the entry point of the forward search
        ''' strategy of the paper: the minimal adaptation networks own three links.
        ''' (枚举链接数不超过上限的连通拓扑，即论文正向策略的最小网络子集)
        ''' </summary>
        ''' <param name="maxLinks">the maximum number of active links, 0 .. 9</param>
        Public Iterator Function EnumerateUpTo(maxLinks As Integer) As IEnumerable(Of Model.NetworkTopology)
            If maxLinks < 0 OrElse maxLinks > Model.NetworkTopology.LinkSlots Then
                Throw New ArgumentOutOfRangeException(NameOf(maxLinks), maxLinks, $"the maximum link count must be inside of 0 .. {Model.NetworkTopology.LinkSlots}.")
            End If

            For Each topo As Model.NetworkTopology In EnumerateAll()
                If topo.HasInputOutputPath AndAlso topo.LinkCount <= maxLinks Then
                    Yield topo
                End If
            Next
        End Function

        ''' <summary>
        ''' Enumerate the connected topologies that own exactly the given number
        ''' of active links.
        ''' (枚举链接数恰好等于给定值的连通拓扑)
        ''' </summary>
        ''' <param name="linkCount">the exact number of active links, 0 .. 9</param>
        Public Iterator Function EnumerateWithLinkCount(linkCount As Integer) As IEnumerable(Of Model.NetworkTopology)
            If linkCount < 0 OrElse linkCount > Model.NetworkTopology.LinkSlots Then
                Throw New ArgumentOutOfRangeException(NameOf(linkCount), linkCount, $"the link count must be inside of 0 .. {Model.NetworkTopology.LinkSlots}.")
            End If

            For Each topo As Model.NetworkTopology In EnumerateAll()
                If topo.HasInputOutputPath AndAlso topo.LinkCount = linkCount Then
                    Yield topo
                End If
            Next
        End Function

        ''' <summary>
        ''' Count the topologies of the complete space, which must be 19,683.
        ''' (拓扑空间大小，应为 19683)
        ''' </summary>
        Public Function CountAll() As Integer
            Dim n As Integer = 0

            For Each topo As Model.NetworkTopology In EnumerateAll()
                n += 1
            Next

            Return n
        End Function

        ''' <summary>
        ''' Count the topologies in which the input node A can reach the output
        ''' node C, which must be 16,038 for the three node network.
        ''' (输入到输出连通的拓扑数量，三节点网络应为 16038)
        ''' </summary>
        Public Function CountConnected() As Integer
            Dim n As Integer = 0

            For Each topo As Model.NetworkTopology In EnumerateAll()
                If topo.HasInputOutputPath Then
                    n += 1
                End If
            Next

            Return n
        End Function

        ''' <summary>
        ''' Count the connected topologies with at most the given number of
        ''' active links.
        ''' (链接数不超过上限的连通拓扑数量)
        ''' </summary>
        ''' <param name="maxLinks">the maximum number of active links</param>
        Public Function CountUpTo(maxLinks As Integer) As Integer
            Dim n As Integer = 0

            For Each topo As Model.NetworkTopology In EnumerateUpTo(maxLinks)
                n += 1
            Next

            Return n
        End Function

        ''' <summary>
        ''' The histogram of the active link counts of a topology collection,
        ''' the index is the link count and the value is the number of topologies.
        ''' (拓扑集合的链接数直方图，下标为链接数、值为拓扑数量)
        ''' </summary>
        ''' <param name="topologies">the topology collection</param>
        Public Function LinkCountHistogram(topologies As IEnumerable(Of Model.NetworkTopology)) As Integer()
            Dim histogram As Integer() = New Integer(Model.NetworkTopology.LinkSlots) {}

            For Each topo As Model.NetworkTopology In topologies
                histogram(topo.LinkCount) += 1
            Next

            Return histogram
        End Function

    End Module

End Namespace
