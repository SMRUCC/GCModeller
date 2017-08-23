Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Public Module PathwayMapRender

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="keggList"></param>
    ''' <param name="internalResource">zip package data</param>
    ''' <returns></returns>
    Public Function QueryMaps(keggList As IEnumerable(Of String), internalResource As IEnumerable(Of Byte)) As IEnumerable(Of NamedValue(Of Image))
        Dim zip$ = App.GetAppSysTempFile(".zip", App.PID)
        Dim repo$ = App.GetAppSysTempFile(".zip", App.PID)

        Call internalResource.FlushStream(zip)
        Call GZip.ImprovedExtractToDirectory(zip, repo, Overwrite.Always)

        Return keggList.QueryMaps(repo)
    End Function

    <Extension>
    Public Function QueryMaps(keggList As IEnumerable(Of String), repo$, Optional color$ = "blue", Optional scale$ = "1,1") As IEnumerable(Of NamedValue(Of Image))
        Return LocalRender.FromRepository(repo).QueryMaps(keggList, 1, color, scale:=scale)
    End Function

    <Extension>
    Public Iterator Function QueryMaps(render As LocalRender,
                                       keggList As IEnumerable(Of String),
                                       Optional threshold% = 3,
                                       Optional color$ = "red",
                                       Optional scale$ = "1,1",
                                       Optional throwException As Boolean = True) As IEnumerable(Of NamedValue(Of Image))

        ' 首先查找出化合物在哪些map之中出现，然后生成绘图查询数据
        For Each foundResult As NamedValue(Of String()) In render.IteratesMapNames(keggList, 3)
            Dim nodes = foundResult _
                .Value _
                .Select(Function(x)
                            Return New NamedValue(Of String) With {
                                .Name = x,
                                .Value = color
                            }
                        End Function) _
                .ToArray

            Try
                Yield New NamedValue(Of Image) With {
                    .Name = foundResult.Name,
                    .Value = render _
                        .Rendering(.Name, nodes,, scale:=scale)
                }
            Catch ex As Exception
                ex = New Exception(foundResult.GetJson, ex)

                If throwException Then
                    Throw ex
                Else
                    Call ex.PrintException
                    Call App.LogException(ex)
                End If
            End Try
        Next
    End Function
End Module
