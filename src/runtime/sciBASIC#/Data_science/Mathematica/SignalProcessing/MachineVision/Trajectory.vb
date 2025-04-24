Imports System.Drawing
Imports System.Runtime.CompilerServices

''' <summary>
''' Trajectory of a object
''' </summary>
Public Class Trajectory

    Public ReadOnly Property TrajectoryID As Integer
    Public ReadOnly Property positions As New List(Of PointF)

    Public ReadOnly Property LastPosition As PointF
        Get
            Return positions.Last()
        End Get
    End Property

    Public Sub New(id As Integer, t0 As Detection)
        TrajectoryID = id
        positions.Add(t0.Position)
    End Sub

    ''' <summary>
    ''' Add last position to the current object trajectory
    ''' </summary>
    ''' <param name="detection"></param>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub Update(detection As Detection)
        Call positions.Add(detection.Position)
    End Sub

End Class
