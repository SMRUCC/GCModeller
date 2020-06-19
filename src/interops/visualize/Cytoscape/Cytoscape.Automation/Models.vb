
' [{"source":"http://localhost:8887/tmp0000b/upload.json","networkSUID":[445]}]"

Public Class NetworkReference
    Public Property source As String
    Public Property networkSUID As String
End Class

''' <summary>
''' local file reference for cytoscape automation
''' </summary>
Public Class FileReference
    Public Property source_location As String
    Public Property source_method As String = "GET"
    Public Property ndex_uuid As String = "12345"
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