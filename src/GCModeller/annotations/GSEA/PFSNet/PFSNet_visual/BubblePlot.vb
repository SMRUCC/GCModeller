Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.Analysis.PFSNet.DataStructure

''' <summary>
''' the bubble plot for the PFSNnet result.
''' </summary>
Public Module BubblePlot

    Public Function Plot(data As PFSNetResultOut, Optional size$ = "2600,3600") As GraphicsData
        Dim bubbles As SerialData() = {
            data.phenotype1.CreateSerial,
            data.phenotype2.CreateSerial
        }

        Return Bubble.Plot(bubbles, size.SizeParser, padding:=g.DefaultPadding)
    End Function

    <Extension>
    Public Function CreateSerial(classResult As PFSNetGraph()) As SerialData

    End Function
End Module
