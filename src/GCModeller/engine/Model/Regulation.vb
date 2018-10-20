Public Structure Regulation

    Public name As String
    ''' <summary>
    ''' Compound / RNA / Proteins
    ''' </summary>
    Public regulator As String
    ''' <summary>
    ''' The regulated target process name
    ''' </summary>
    Public process As String
    ''' <summary>
    ''' The type of the target process
    ''' </summary>
    Public type As Processes
    ''' <summary>
    ''' + positive: accelerate
    ''' + negative: inhibition
    ''' </summary>
    Public effects As Double

End Structure

Public Enum Processes
    Transcription
    Translation
    Flux
End Enum