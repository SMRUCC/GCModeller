Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.AnalysisTools.DataVisualization.Interaction.Cytoscape.CytoscapeGraphView.XGMML
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace CytoscapeGraphView.XGMML

    Public Module GraphExtensions

        <Extension>
        Public Function Distinct(Edges As Edge()) As Edge()
            Dim LQuery = From edge As Edge
                         In Edges
                         Select edge
                         Group edge By edge.__internalUID Into Group
            Dim buf As Edge() = LQuery.Select(Function(x) x.Group) _
                .ToArray(AddressOf MergeEdges, Parallel:=True) _
                .AddHandle.ToArray
            Return buf
        End Function

        <Extension>
        Private Function MergeEdges(source As IEnumerable(Of Edge)) As Edge
            Dim edges As Edge() = source.ToArray

            If edges.Length = 1 Then
                Return edges.First
            End If

            Dim First As Edge = edges.First
            Dim attrs As Attribute() =
                LinqAPI.Exec(Of Attribute) <= edges.Select(Function(x) x.Attributes)

            First.Attributes = MergeAttributes(attrs)

            Return First
        End Function

        Private Function MergeAttributes(attrs As Attribute()) As Attribute()
            Dim LQuery = From attr As Attribute
                         In attrs
                         Select attr
                         Group attr By attr.Name Into Group
            Dim attrsBuffer = From g
                              In LQuery
                              Select g.Group.First,
                                  values = (From x As Attribute
                                            In g.Group
                                            Select x.Value
                                            Distinct).ToArray
            Dim result As Attribute() =
                attrsBuffer.ToArray(Function(x) x.First.__setValue(x.values))
            Return result
        End Function

        <Extension>
        Private Function __setValue(attr As Attribute, values As String()) As Attribute
            If String.Equals(attr.Type, ATTR_VALUE_TYPE_REAL) Then
                attr.Value = (From s As String In values Select Val(s)).Min
            Else
                attr.Value = String.Join("; ", values)
            End If

            Return attr
        End Function
    End Module
End Namespace