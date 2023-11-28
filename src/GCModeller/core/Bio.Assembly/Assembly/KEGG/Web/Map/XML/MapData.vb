Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq

Namespace Assembly.KEGG.WebServices.XML

    Public Class MapData

        ''' <summary>
        ''' the pathway map data
        ''' </summary>
        ''' <returns></returns>
        Public Property mapdata As Area()
        ''' <summary>
        ''' the module network data
        ''' </summary>
        ''' <returns></returns>
        Public Property module_mapdata As Area()

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function PopulateAll() As IEnumerable(Of Area)
            Return mapdata.JoinIterates(module_mapdata)
        End Function

        Public Overrides Function ToString() As String
            Return $"{mapdata.TryCount} mapdata and {module_mapdata.TryCount} module mapdata"
        End Function

    End Class
End Namespace