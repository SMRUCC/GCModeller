
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.GCModeller.Workbench.KEGGReport
Imports SMRUCC.Rsharp.Runtime
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

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="map">the blank template of the kegg map</param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("keggMap.reportHtml")>
    Public Function showReportHtml(map As Map, <RRawVectorArgument> highlights As Object, Optional env As Environment = Nothing) As Object
        Dim highlightObjs As NamedValue(Of String)()

        If TypeOf highlights Is NamedValue(Of String)() Then
            highlightObjs = highlights
        ElseIf TypeOf highlights Is String()() Then
            highlightObjs = DirectCast(highlights, String()()) _
                .Select(Function(tuple)
                            Return New NamedValue(Of String) With {
                                .Name = tuple(Scan0),
                                .Value = tuple(1)
                            }
                        End Function) _
                .ToArray
        ElseIf TypeOf highlights Is list Then
            highlightObjs = DirectCast(highlights, list).slots _
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

        Return ReportRender.Render(map, highlightObjs)
    End Function

End Module
