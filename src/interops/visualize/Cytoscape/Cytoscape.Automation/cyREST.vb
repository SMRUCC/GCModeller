Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.Cyjs

Public MustInherit Class cyREST

    Public MustOverride Function layouts() As String()

    ''' <summary>
    ''' Returns a list of all networks as names and their corresponding SUIDs.
    ''' </summary>
    ''' <returns></returns>
    Public MustOverride Function networksNames() As String()

    ''' <summary>
    ''' Creates a new network in the current session from a file or URL source.
    ''' </summary>
    ''' <returns></returns>
    Public MustOverride Function putNetwork(network As Cyjs, Optional collection$ = Nothing, Optional title$ = Nothing, Optional source$ = Nothing, Optional format As formats = formats.json)


End Class

Public Enum formats
    ''' <summary>
    ''' SIF format
    ''' </summary>
    egdeList
    ''' <summary>
    ''' cx format
    ''' </summary>
    cx
    ''' <summary>
    ''' cytoscape.js format
    ''' </summary>
    json
End Enum