Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.SignalProcessing

''' <summary>
''' cdf writer of the signals
''' </summary>
Public Module cdfSignalsWriter

    ' 20200704 redesign of the general signal cdf file storage layout:
    ' the previous version of the signal data file is too slow that reading 
    ' in R script code when the signal data count is large

    <Extension>
    Public Function WriteCDF(signals As IEnumerable(Of GeneralSignal), file As String, Optional description$ = Nothing, Optional dimension_prefix$ = "signalChunk_") As Boolean
        Using cdffile As New CDFWriter(file)
            Call cdffile.Dimensions(Dimension.Double, Dimension.Float, Dimension.Integer, Dimension.Long, Dimension.Text(fixedChars:=1024))
            Call cdffile.GlobalAttributes(New attribute With {.name = "time", .type = CDFDataTypes.CHAR, .value = Now.ToString})
            Call cdffile.GlobalAttributes(New attribute With {.name = "filename", .type = CDFDataTypes.CHAR, .value = file.FileName})
            Call cdffile.GlobalAttributes(New attribute With {.name = "github", .type = CDFDataTypes.CHAR, .value = LICENSE.githubURL})

            If Not description.StringEmpty Then
                Call cdffile.GlobalAttributes(New attribute With {.name = NameOf(description), .type = CDFDataTypes.CHAR, .value = description})
            End If

            Dim package As GeneralSignal() = signals.ToArray
            Dim chunksize As New List(Of Integer)
            Dim attrs As New Dictionary(Of String, List(Of String))

            Call cdffile.GlobalAttributes(New attribute With {.name = "signals", .type = CDFDataTypes.INT, .value = package.Length})




            For Each signal As GeneralSignal In signals

            Next
        End Using

        Return True
    End Function

    <Extension>
    Private Function createAttributes(package As GeneralSignal()) As Dictionary(Of String, CDFData)
        Dim attrs As New Dictionary(Of String, CDFData)
        Dim allNames As String() = package.Select(Function(sig) sig.meta.Keys).IteratesALL.Distinct.ToArray

        For Each name As String In allNames
            Dim values As New List(Of String)

            For Each signal As GeneralSignal In package
                values.Add(signal.meta.TryGetValue(name))
            Next

            If values.All(Function(s) s.IsPattern("\d+")) Then

            End If
        Next

        Return attrs
    End Function
End Module
