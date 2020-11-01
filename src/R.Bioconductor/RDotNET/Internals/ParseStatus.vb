Namespace Internals
    ''' <summary>
    ''' Parsing status enumeration.
    ''' </summary>
    Public Enum ParseStatus
        ''' <summary>
        ''' The default value.
        ''' </summary>
        Null

        ''' <summary>
        ''' No error.
        ''' </summary>
        OK

        ''' <summary>
        ''' Statement is incomplete.
        ''' </summary>
        Incomplete

        ''' <summary>
        ''' Error occurred.
        ''' </summary>
        [Error]

        ''' <summary>
        ''' EOF.
        ''' </summary>
        EOF

        '#region Original Definitions
        '[Obsolete("Use ParseStatus.Null instead.")]
        'PARSE_NULL = Null,
        '[Obsolete("Use ParseStatus.OK instead.")]
        'PARSE_OK = OK,
        '[Obsolete("Use ParseStatus.Incomplete instead.")]
        'PARSE_INCOMPLETE = Incomplete,
        '[Obsolete("Use ParseStatus.Error instead.")]
        'PARSE_ERROR = Error,
        '[Obsolete("Use ParseStatus.EOF instead.")]
        'PARSE_EOF = EOF,
        '#endregion
    End Enum
End Namespace
