Imports System.Runtime.CompilerServices
Imports SMRUCC.WebCloud.highcharts.viz3D

Friend Module ChartProfiles

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function PieChart3D() As chart
        Return New chart With {
            .type = "pie",
            .options3d = New options3d With {
                .enabled = True,
                .alpha = 45,
                .beta = 0
            }
        }
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function BarChart3D() As chart
        Return New chart With {
            .type = "column",
            .options3d = New options3d With {
                .enabled = True,
                .alpha = 10,
                .beta = 25,
                .depth = 70
            }
        }
    End Function
End Module
