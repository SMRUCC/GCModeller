
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.GCModeller.Workbench.KEGGReport
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' 
''' </summary>
<Package("report.utils")>
Module report

    <ExportAPI("map.local_render")>
    Public Function MapRender(maps As Dictionary(Of String, Map)) As LocalRender
        Return New LocalRender(maps)
    End Function

    <ExportAPI("nodes.colorAs")>
    Public Function singleColor(nodes As String(), color$) As NamedValue(Of String)()
        Return nodes.Select(Function(id) New NamedValue(Of String)(id, color)).ToArray
    End Function

    <ExportAPI("keggMap.highlights")>
    Public Function renderMapHighlights(map As Map, <RRawVectorArgument> highlights As Object, Optional env As Environment = Nothing) As Object
        Dim highlightObjs = getHighlightObjects(highlights, env)

        If highlightObjs Like GetType(Message) Then
            Return highlightObjs.TryCast(Of Message)
        Else
            Return LocalRender.Rendering(map, highlightObjs.TryCast(Of NamedValue(Of String)()))
        End If
    End Function

    Private Function getHighlightObjects(highlights As Object, env As Environment) As [Variant](Of Message, NamedValue(Of String)())
        If TypeOf highlights Is NamedValue(Of String)() Then
            Return DirectCast(highlights, NamedValue(Of String)())
        ElseIf TypeOf highlights Is String()() Then
            Return DirectCast(highlights, String()()) _
                .Select(Function(tuple)
                            Return New NamedValue(Of String) With {
                                .Name = tuple(Scan0),
                                .Value = tuple(1)
                            }
                        End Function) _
                .ToArray
        ElseIf TypeOf highlights Is list Then
            Return DirectCast(highlights, list).slots _
                .Select(Function(tuple)
                            Dim colorVal As String = InteropArgumentHelper.getColor(tuple.Value)

                            Return New NamedValue(Of String) With {
                                .Name = tuple.Key,
                                .Value = colorVal
                            }
                        End Function) _
                .ToArray
        Else
            Return Internal.debug.stop(New InvalidCastException(highlights.GetType.FullName), env)
        End If
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="map">the blank template of the kegg map</param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("keggMap.reportHtml")>
    Public Function showReportHtml(map As Map, <RRawVectorArgument> highlights As Object, Optional env As Environment = Nothing) As Object
        Dim highlightObjs = getHighlightObjects(highlights, env)

        If highlightObjs Like GetType(Message) Then
            Return highlightObjs.TryCast(Of Message)
        Else
            Return ReportRender.Render(map, highlightObjs.TryCast(Of NamedValue(Of String)()))
        End If
    End Function

End Module
