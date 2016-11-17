Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Scripting

''' <summary>
''' Circos script ``-debug_group`` options
''' </summary>
Public Enum DebugGroups As Long
    NULL = 0

    angle = 1
    anglepos = 2
    axis = 4
    background = 8
    bezier = 16
    brush = 32
    cache = 64
    chrfilter = 128
    color = 256
    conf = 512
    counter = 1024
    cover = 2048
    eval = 4096
    font = 8192
    heatmap = 16384
    ideogram = 32768
    image = 65536
    io = image * 2
    karyotype = io * 2
    layer = karyotype * 2
    legend = layer * 2
    link = legend * 2
    output = link * 2
    parse = output * 2
    png = parse * 2
    rule = png * 2
    scale = rule * 2
    spacing = scale * 2
    stats = spacing * 2
    summary = stats * 2
    svg = summary * 2
    text = svg * 2
    textplace = text * 2
    tick = textplace * 2
    tile = tick * 2
    timer = tile * 2
    unit = timer * 2
    url = unit * 2
    zoom = url * 2
End Enum

''' <summary>
''' Generates the circos debugger options
''' </summary>
Public Module CircosDebugger

    ReadOnly __options As DebugGroups() = Enums(Of DebugGroups)()

    ''' <summary>
    ''' ``-debug_group`` command argument name
    ''' </summary>
    Public Const Debugger As String = " -debug_group "

    ''' <summary>
    ''' Generates the ``-debug_group`` options from the enum values
    ''' </summary>
    ''' <param name="arg"></param>
    ''' <returns></returns>
    <Extension> Public Function GetOptions(arg As DebugGroups) As String
        If arg = DebugGroups.NULL Then
            Return ""
        Else
            Dim options As New List(Of String)

            For Each o As DebugGroups In __options
                If arg.HasFlag(o) Then
                    Call options.Add(o.ToString)
                End If
            Next

            Return Debugger & options.JoinBy(",")
        End If
    End Function

    Public Function EnableAllOptions() As String
        Return Debugger & __options _
            .Select(AddressOf InputHandler.ToString) _
            .JoinBy(",")
    End Function
End Module