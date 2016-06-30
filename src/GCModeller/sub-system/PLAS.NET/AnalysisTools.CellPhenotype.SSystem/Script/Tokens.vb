Imports System.ComponentModel

Namespace Script

    Public Enum Tokens
        UNDEFINE

        ''' <summary>
        ''' TITLE
        ''' </summary>
        <Description("TITLE")> Title
        ''' <summary>
        ''' RXN
        ''' </summary>
        <Description("RXN")> Reaction
        ''' <summary>
        ''' INIT
        ''' </summary>
        <Description("INIT")> InitValue
        ''' <summary>
        ''' FUNC
        ''' </summary>
        <Description("FUNC")> [Function]
        ''' <summary>
        ''' FINALTIME
        ''' </summary>
        <Description("FINALTIME")> Time
        ''' <summary>
        ''' NAMED
        ''' </summary>
        <Description("NAMED")> [Alias]
        ''' <summary>
        ''' STIMULATE
        ''' </summary>
        <Description("STIMULATE")> Disturb
        ''' <summary>
        ''' COMMENT
        ''' </summary>
        <Description("COMMENT")> Comment
        ''' <summary>
        ''' REM-SUB
        ''' </summary>
        <Description("REM-SUB")> SubsComments
        ''' <summary>
        ''' CONST
        ''' </summary>
        <Description("CONST")> Constant
    End Enum
End Namespace