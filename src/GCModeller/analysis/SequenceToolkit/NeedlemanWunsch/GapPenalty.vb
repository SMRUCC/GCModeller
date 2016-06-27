
''' <summary>
''' Class to implement linear and affine gap penalties
''' Bioinformatics 1, WS 15/16
''' Jonas Ditz and Benjamin Schroeder
''' </summary>
Public Class GapPenalty

    Dim __affine As Boolean = False

    ''' <summary>
    ''' get gap opening cost </summary>
    ''' <returns> gapOpening </returns>
    Public Property GapOpening As Integer = 1

    ''' <summary>
    ''' get gap extension cost </summary>
    ''' <returns> gapExtension </returns>
    Public Property GapExtension As Integer = 1

    ''' <summary>
    ''' get gap penalty typ </summary>
    ''' <returns> 0 if linear, 1 else </returns>
    Public Property PenaltyTyp As Integer
        Get
            If __affine Then
                Return 1
            Else
                Return 0
            End If
        End Get
        Set(penaltyType As Integer)
            If penaltyType = 0 Then
                Me.__affine = False
            Else
                Me.__affine = True
            End If
        End Set
    End Property

    ''' <summary>
    ''' get gap cost for currend gap </summary>
    ''' <param name="gapOpen"> </param>
    ''' <returns> gapCost </returns>
    Public Function getGapCost(gapOpen As Boolean) As Integer
        If __affine And gapOpen Then
            Return GapOpening
        ElseIf __affine And (Not gapOpen) Then
            Return GapExtension
        Else
            Return GapOpening
        End If
    End Function
End Class