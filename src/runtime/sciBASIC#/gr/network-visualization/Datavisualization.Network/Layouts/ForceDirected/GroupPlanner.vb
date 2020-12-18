Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports stdNum = System.Math

Namespace Layouts.ForceDirected

    ''' <summary>
    ''' 这个模块之中的方法会尽量让相同类型分组的节点聚集在一块
    ''' </summary>
    Public Class GroupPlanner : Inherits Planner

        ReadOnly groupBy As Dictionary(Of String, String)

        Public Sub New(g As NetworkGraph,
                       Optional ejectFactor As Integer = 6,
                       Optional condenseFactor As Integer = 3,
                       Optional maxtx As Integer = 4,
                       Optional maxty As Integer = 3,
                       Optional dist_threshold As String = "30,250",
                       Optional size As String = "1000,1000")

            Call MyBase.New(g, ejectFactor, condenseFactor, maxtx, maxty, dist_threshold, size)

            Me.groupBy = g.vertex _
                .ToDictionary(Function(n) n.label,
                              Function(n)
                                  Dim classLabel As String = n.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE)

                                  If classLabel.StringEmpty Then
                                      Return "n/a"
                                  Else
                                      Return classLabel
                                  End If
                              End Function)
        End Sub

        Protected Overrides Sub runAttraction()
            Dim u, v As Node
            Dim distX, distY, dist As Double
            Dim dx, dy As Double

            For Each edge As Edge In g.graphEdges
                u = edge.U
                v = edge.V
                distX = u.data.initialPostion.x - v.data.initialPostion.x
                distY = u.data.initialPostion.y - v.data.initialPostion.y
                dist = stdNum.Sqrt(distX * distX + distY * distY)
                dx = distX * dist / k * condenseFactor
                dy = distY * dist / k * condenseFactor

                If groupBy(v.label) = groupBy(u.label) AndAlso groupBy(v.label) <> "n/a" Then
                    dx *= 10
                    dy *= 10
                Else
                    dx /= 10
                    dy /= 10
                End If

                mDxMap(u.label) = mDxMap(u.label) - dx
                mDyMap(u.label) = mDyMap(u.label) - dy
                mDxMap(v.label) = mDxMap(v.label) + dx
                mDyMap(v.label) = mDyMap(v.label) + dy
            Next
        End Sub

        Protected Overrides Sub runRepulsive()
            Dim distX, distY, dist As Double
            Dim id As String
            Dim ejectFactor = Me.ejectFactor
            Dim dx, dy As Double

            For Each v As Node In g.vertex
                id = v.label

                mDxMap(id) = 0.0
                mDyMap(id) = 0.0

                For Each u As Node In g.vertex.Where(Function(ui) Not ui Is v)
                    distX = v.data.initialPostion.x - u.data.initialPostion.x
                    distY = v.data.initialPostion.y - u.data.initialPostion.y
                    dist = stdNum.Sqrt(distX * distX + distY * distY)

                    If (dist < dist_thresh.Min) Then
                        ejectFactor = 5
                    End If

                    If dist > 0 AndAlso dist < dist_thresh.Max Then
                        dx = (distX / dist * k * k / dist) * ejectFactor
                        dy = (distY / dist * k * k / dist) * ejectFactor

                        If groupBy(v.label) = groupBy(u.label) AndAlso groupBy(v.label) <> "n/a" Then
                            dx /= 10
                            dy /= 10
                        Else
                            dx *= 10
                            dy *= 10
                        End If

                        mDxMap(id) = mDxMap(id) + dx
                        mDyMap(id) = mDyMap(id) + dy
                    End If
                Next
            Next
        End Sub
    End Class
End Namespace