Imports System.Drawing

Namespace CytoscapeGraphView.XGMML

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' https://github.com/cytoscape/cytoscape-impl/blob/93530ef3b35511d9b1fe0d0eb913ecdcd3b456a8/ding-impl/ding-presentation-impl/src/main/java/org/cytoscape/ding/impl/HandleImpl.java#L247
    ''' </remarks>
    Public Class Handle

        Dim cosTheta# = Double.NaN
        Dim sinTheta# = Double.NaN
        Dim ratio# = Double.NaN

        ' Original handle location
        Dim x# = 0
        Dim y# = 0

        Const DELIMITER As Char = ","c

        ''' <summary>
        ''' Rotate And scale the vector to the handle position
        ''' </summary>
        ''' <param name="sX">x of source node</param>
        ''' <param name="sY">y of source node</param>
        ''' <param name="tX">x of target node</param>
        ''' <param name="tY">y of target node</param>
        ''' <returns></returns>
        Public Function convert(sX As Double, sY As Double, tX As Double, tY As Double) As PointF
            ' Original edge vector v = (vx, vy). (from source to target)
            Dim vx = tX - sX
            Dim vy = tY - sY

            ' rotate
            Dim newX = vx * cosTheta - vy * sinTheta
            Dim newY = vx * sinTheta + vy * cosTheta

            ' New rotated vector v' = (newX, newY).
            ' Resize vector
            newX = newX * ratio
            newY = newY * ratio

            ' ... And this Is the New handle position.
            Dim handleX = newX + sX
            Dim handleY = newY + sY

            Dim newPoint As New PointF(handleX, handleY)

            Return newPoint
        End Function

        ''' <summary>
        ''' Serialized string Is "cos,sin,ratio".
        ''' </summary>
        ''' <returns></returns>
        Public Function getSerializableString() As String
            Return cosTheta & DELIMITER & sinTheta & DELIMITER & ratio
        End Function

        Public Shared Function parseHandles(strRepresentation As String) As IEnumerable(Of Handle)
            Return strRepresentation _
                .Split("|"c) _
                .Select(Function(str)
                            Dim parts As Double() = str _
                                .Split(DELIMITER) _
                                .Select(AddressOf Double.Parse) _
                                .ToArray

                            If parts.Length = 2 Then
                                Return New Handle With {.x = parts(0), .y = parts(1)}
                            ElseIf parts.Length = 3 Then
                                Return New Handle With {
                                    .cosTheta = parts(0),
                                    .sinTheta = parts(1),
                                    .ratio = parts(3)
                                }
                            Else
                                Return Nothing
                            End If
                        End Function)
        End Function
    End Class
End Namespace