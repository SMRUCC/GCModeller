Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.Cyjs
Imports SMRUCC.genomics.Visualize.Cytoscape.Tables

Public MustInherit Class cyREST : Implements IDisposable

    ' Protected Shared ReadOnly virtualFilesystem As New FileHost(Net.Tcp.GetFirstAvailablePort(-1))
    ' Protected Shared ReadOnly fsThread As Thread

    Shared Sub New()
        '  fsThread = virtualFilesystem.DriverRun
        ' Thread.Sleep(500)
    End Sub

    Private disposedValue As Boolean

    Public Shared Function addUploadFile(file As String) As String
        '   Return virtualFilesystem.addUploadFile(file)
    End Function

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
    Public MustOverride Function putNetwork(network As [Variant](Of Cyjs, SIF()), Optional collection$ = Nothing, Optional title$ = Nothing) As NetworkReference
    Public MustOverride Function applyLayout(network As Integer, Optional algorithm As String = "force-directed") As String

    ''' <summary>
    ''' Saves the current session to a file. If successful, the session file location will be returned.
    ''' </summary>
    ''' <param name="file">
    ''' Session file location as an absolute path.(``*.cys``)
    ''' </param>
    ''' <returns></returns>
    Public MustOverride Function saveSession(file As String)
    Public MustOverride Sub destroySession()

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
            ' TODO: set large fields to null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
    ' Protected Overrides Sub Finalize()
    '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub

    Public Shared Sub Close()
        ' Call virtualFilesystem.Dispose()
        ' Call fsThread.Abort()
    End Sub
End Class

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


Namespace Upload

    Public Class CyjsUpload

        Public Property data As cyjsdata
        Public Property elements As networkElement

        Sub New(cyjs As Cyjs, title As String)
            data = New cyjsdata With {
                .name = If(title, App.NextTempName)
            }
            elements = New networkElement With {
                .edges = cyjs.elements.edges.Select(Function(a) New cyjsedge With {.data = New edgeData2 With {.interaction = a.data.interaction, .source = a.data.source, .target = a.data.target}}).ToArray,
                .nodes = cyjs.elements.nodes.Select(Function(a) New cyjsNode With {.data = New nodeData2 With {.common = a.data.common, .id = a.data.id}}).ToArray
            }
        End Sub

    End Class

    Public Class networkElement
        Public Property nodes As cyjsNode()
        Public Property edges As cyjsedge()
    End Class

    Public Class cyjsNode
        Public Property data As nodeData2
    End Class

    Public Class cyjsedge
        Public Property data As edgeData2
    End Class

    Public Class edgeData2
        Public Property source As String
        Public Property target As String
        Public Property interaction As String
    End Class

    Public Class nodeData2
        Public Property id As String
        Public Property common As String
    End Class

    Public Class cyjsdata
        Public Property name As String = App.NextTempName
    End Class
End Namespace

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