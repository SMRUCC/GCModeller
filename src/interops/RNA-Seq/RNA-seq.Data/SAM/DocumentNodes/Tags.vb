Imports System.ComponentModel

Namespace SAM

    Public Enum Tags As Byte
        ''' <summary>
        ''' The header line. The first line if present.
        ''' </summary>
        ''' 
        <Description("The header line. The first line if present.")>
        HD = 0
        ''' <summary>
        ''' Reference sequence dictionary. The order of @SQ lines defines the alignment sorting order.
        ''' </summary>
        ''' 
        <Description("Reference sequence dictionary. The order of @SQ lines defines the alignment sorting order.")>
        SQ
        ''' <summary>
        ''' Read group. Unordered multiple @RG lines are allowed.
        ''' </summary>
        ''' 
        <Description("Read group. Unordered multiple @RG lines are allowed.")>
        RG
        ''' <summary>
        ''' Program.
        ''' </summary>
        ''' 
        <Description("Program.")>
        PG
        ''' <summary>
        ''' One-line text comment. Unordered multiple @CO lines are allowed.
        ''' </summary>
        ''' 
        <Description("One-line text comment. Unordered multiple @CO lines are allowed.")>
        CO
    End Enum
End Namespace