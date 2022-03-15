Imports System.Runtime.CompilerServices

Namespace Analysis.FastUnfolding

    Module Utils

        ''' <summary>
        ''' 根据tag和图的连接方式计算模块度
        ''' </summary>
        ''' <param name="tags"></param>
        ''' <param name="map_dict"></param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function Modularity(tags As Dictionary(Of String, String), map_dict As KeyMaps) As Double
            Dim m As Double = 0
            Dim community As New KeyMaps
            Dim Q As Double = 0
            Dim sum_in As Double = 0
            Dim sum_tot As Double = 0

            ' 同属一个社群的人都有谁
            For Each key As String In map_dict.Keys
                m += map_dict(key).Count
                community(tags(key)).Add(key)
            Next

            For Each com As String In community.Keys
                sum_in = 0
                sum_tot = 0

                For Each u As String In community(com)
                    sum_tot += map_dict(u).Count

                    For Each v As String In map_dict(u)
                        If tags(v) = tags(u) Then
                            sum_in += 1
                        End If
                    Next
                Next

                Q += (sum_in / m - (sum_tot / m) ^ 2)
            Next

            Return Q
        End Function
    End Module
End Namespace