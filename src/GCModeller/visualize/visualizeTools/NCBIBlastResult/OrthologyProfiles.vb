Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract

Namespace NCBIBlastResult

    Public Class OrthologyProfile

    End Class

    Public Module OrthologyProfiles

        <Extension>
        Public Function OrthologyProfiles(result As IEnumerable(Of BBHIndex)) As OrthologyProfile()

        End Function

        <Extension>
        Public Function Plot(profile As IEnumerable(Of OrthologyProfile),
                             Optional size$ = "3300,2700",
                             Optional margin$ = g.DefaultPadding,
                             Optional bg$ = "white") As GraphicsData

        End Function
    End Module
End Namespace