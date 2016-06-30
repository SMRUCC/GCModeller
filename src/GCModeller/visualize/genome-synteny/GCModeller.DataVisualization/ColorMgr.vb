Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class ColorMgr

    ''' <summary>
    ''' <see cref="ClMap.Identifier"/> --> <see cref="ClMap.map"/>
    ''' </summary>
    ReadOnly __entityMaps As Dictionary(Of ClMap)
    ''' <summary>
    ''' <see cref="ClMap.map"/> --> <see cref="Color"/>
    ''' </summary>
    ReadOnly __colorMaps As MapsHelper(Of Color)
    ''' <summary>
    ''' Array of <see cref="ClMap.map"/>
    ''' </summary>
    ReadOnly __categories As String()

    Public ReadOnly Property [Default] As Color
        Get
            Return __colorMaps.__default
        End Get
    End Property

    Sub New(source As IEnumerable(Of ClMap), [default] As Color)
        __entityMaps = source.ToDictionary
        __categories = (From x As ClMap In source Select x.map Distinct).ToArray
        __colorMaps =
            New MapsHelper(Of Color)(__categories.GenerateColorProfiles, [default])
    End Sub

    ''' <summary>
    ''' <see cref="ClMap.Identifier"/>
    ''' </summary>
    ''' <param name="id"><see cref="ClMap.Identifier"/></param>
    ''' <returns></returns>
    Public Function GetEntityColor(id As String) As Color
        If __entityMaps.ContainsKey(id) Then
            Return __colorMaps.GetValue(__entityMaps(id).map)
        Else
            Return __colorMaps.__default
        End If
    End Function

    ''' <summary>
    ''' <see cref="ClMap.map"/>
    ''' </summary>
    ''' <param name="category"><see cref="ClMap.map"/></param>
    ''' <returns></returns>
    Public Function GetValue(category As String) As Color
        Return __colorMaps.GetValue(category)
    End Function

    Public Overrides Function ToString() As String
        Return __categories.GetJson
    End Function
End Class

Public Class ClMap : Implements sIdEnumerable

    Public Property Identifier As String Implements sIdEnumerable.Identifier
    Public Property map As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class