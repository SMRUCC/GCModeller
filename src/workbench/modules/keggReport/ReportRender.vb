Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Scripting.SymbolBuilder
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Public Class ReportRender

    Public Function CreateMap(compound As Compound, location As PointF) As MapShape
        Return New MapShape With {
            .entities = {compound.entry},
            .title = compound.commonNames.FirstOrDefault,
            .location = {location.X, location.Y},
            .shape = "circle"
        }
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="gene">
    ''' <see cref="htext.ko00001"/>
    ''' </param>
    ''' <param name="location"></param>
    ''' <returns></returns>
    Public Function CreateMap(gene As BriteHText, location As PointF) As MapShape
        Return New MapShape With {
            .entities = {gene.entryID},
            .location = {location.X, location.Y},
            .shape = "rect",
            .title = gene.description
        }
    End Function

    Public Function CreateMap(area As Area) As MapShape
        Return New MapShape With {
            .shape = area.shape,
            .title = area.title,
            .entities = area.IDVector,
            .location = area.coords _
                .Split(","c) _
                .Select(AddressOf Val) _
                .ToArray
        }
    End Function

    Public Function Render(map As Map, highlights As IEnumerable(Of NamedValue(Of String))) As String
        Dim mapjson As MapShape() = map.shapes _
            .Select(AddressOf CreateMap) _
            .ToArray
        Dim objectList As NamedValue(Of String)() = highlights.ToArray
        Dim rendering As Image = LocalRender.Rendering(map, objectList)

        With New ScriptBuilder(My.Resources.map_template)
            !map_json = mapjson.GetJson
            !map_base64 = New DataURI(rendering).ToString
            !image_width = rendering.Width
            !keggLink = New NamedCollection(Of NamedValue(Of String))() With {
                .name = map.id,
                .description = map.Name,
                .value = objectList
            }.KEGGURLEncode

            Return .ToString
        End With
    End Function
End Class

Public Class MapShape

    Public Property shape As String
    Public Property location As Double()
    ''' <summary>
    ''' kegg id list
    ''' </summary>
    ''' <returns></returns>
    Public Property entities As String()
    Public Property title As String

End Class