Imports Microsoft.VisualBasic.ApplicationServices.Zip
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Namespace Session

    ''' <summary>
    ''' ``*.cys`` cytoscape session file reader model
    ''' </summary>
    Public Class CysSessionFile

        ''' <summary>
        ''' the original *.cys session file location.
        ''' </summary>
        Public ReadOnly source As String

        ReadOnly tempDir As String

        Private Sub New(tempDir As String, cys As String)
            Me.tempDir = tempDir
            Me.source = cys
        End Sub

        ''' <summary>
        ''' 加载一个已经具有网络布局信息的网络模型
        ''' </summary>
        ''' <returns></returns>
        Public Function GetLayoutedGraph(Optional collection$ = Nothing, Optional name$ = Nothing) As NetworkGraph

        End Function

        Public Shared Function Open(cys As String) As CysSessionFile
            Dim temp As String = App.GetAppSysTempFile(".zip", App.PID, "cytoscape_")

            Call UnZip.ImprovedExtractToDirectory(cys, temp, Overwrite.Always, extractToFlat:=False)

            Return New CysSessionFile(temp, cys)
        End Function
    End Class
End Namespace