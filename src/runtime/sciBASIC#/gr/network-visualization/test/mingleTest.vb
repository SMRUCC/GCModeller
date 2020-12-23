Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts.EdgeBundling.Mingle
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Serialization.JSON

Module mingleTest

    Dim json As String = <json>

                             [
  {
    "id": 1,
    "name": 1,
    "data": {
      "coords": [
        10,
        10,
        10,
        450
      ],
      "weight": 5,
      "color": "rgb(241, 205, 15)"
    }
  },
  {
    "id": 2,
    "name": 2,
    "data": {
      "coords": [
        30,
        10,
        30,
        450
      ],
      "weight": 5,
      "color": "rgb(16, 99, 179)"
    }
  },
  {
    "id": 3,
    "name": 3,
    "data": {
      "coords": [
        50,
        10,
        50,
        450
      ],
      "weight": 5,
      "color": "rgb(147, 148, 58)"
    }
  },
  {
    "id": 4,
    "name": 4,
    "data": {
      "coords": [
        100,
        10,
        100,
        450
      ],
      "weight": 5,
      "color": "rgb(187, 71, 149)"
    }
  },
  {
    "id": 5,
    "name": 5,
    "data": {
      "coords": [
        200,
        10,
        200,
        450
      ],
      "weight": 5,
      "color": "rgb(244, 141, 72)"
    }
  },
  {
    "id": 6,
    "name": 6,
    "data": {
      "coords": [
        230,
        10,
        230,
        450
      ],
      "weight": 5,
      "color": "rgb(110, 196, 145)"
    }
  }
]


                         </json>.Value

    Sub Main()
        Dim bundle As New Bundler(New Options With {
          .angleStrength = 10
        })
        Dim json = mingleTest.json.LoadJSON(Of testJsonNode())
        Dim nodes As New List(Of Node)
        Dim data As MingleNodeData

        For Each item In json
            data = New MingleNodeData With {.mass = item.data.weight, .color = item.data.color.GetBrush, .coords = item.data.coords}
            nodes.Add(New Node(item.name, data))
        Next

        bundle.setNodes(nodes.ToArray)
        bundle.buildNearestNeighborGraph(10)
        bundle.MINGLE()
    End Sub
End Module

Public Class testJsonNode

    Public Property id As String
    Public Property name As String
    Public Property data As data

End Class

Public Class data
    Public Property coords As Double()
    Public Property weight As Double
    Public Property color As String
End Class