Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.SignalProcessing
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Serialization.JSON

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

            Call cdffile.GlobalAttributes(New attribute With {.name = "signals", .type = CDFDataTypes.INT, .value = package.Length})

            For Each attr As NamedValue(Of CDFData) In package.createAttributes()

            Next

            For Each signal As GeneralSignal In signals

            Next
        End Using

        Return True
    End Function

    <Extension>
    Private Iterator Function createAttributes(package As GeneralSignal()) As IEnumerable(Of NamedValue(Of CDFData))
        Dim allNames As String() = package.Select(Function(sig) sig.meta.Keys).IteratesALL.Distinct.ToArray

        For Each name As String In allNames
            Dim values As New List(Of String)
            Dim data As CDFData

            For Each signal As GeneralSignal In package
                values.Add(signal.meta.TryGetValue(name, [default]:=""))
            Next

            If values.All(Function(s) s Is Nothing OrElse s = "" OrElse s.IsPattern("\d+")) Then
                Dim longs = values.Select(AddressOf Long.Parse).ToArray

                If longs.All(Function(b) b <= 255 AndAlso b >= -255) Then
                    data = New CDFData With {.byteStream = longs.Select(Function(l) CByte(l)).ToBase64String}
                ElseIf longs.All(Function(s) s <= Short.MaxValue AndAlso s >= Short.MinValue) Then
                    data = New CDFData With {.tiny_int = longs.Select(Function(l) CShort(l)).ToArray}
                ElseIf longs.All(Function(i) i <= Integer.MaxValue AndAlso i >= Integer.MinValue) Then
                    data = New CDFData With {.integers = longs.Select(Function(l) CInt(l)).ToArray}
                Else
                    data = New CDFData With {.longs = longs}
                End If

            ElseIf values.All(Function(s) s Is Nothing OrElse s = "" OrElse s.IsNumeric) Then
                data = New CDFData With {.numerics = values.Select(AddressOf ParseDouble).ToArray}
            ElseIf values.All(Function(s) s Is Nothing OrElse s = "" OrElse s.IsPattern("((true)|(false))", RegexICSng)) Then
                data = New CDFData With {.flags = values.Select(AddressOf ParseBoolean).ToArray}
            Else
                data = New CDFData With {.chars = values.AsEnumerable.GetJson}
            End If

            Yield New NamedValue(Of CDFData) With {
                .Name = name,
                .Value = data
            }
        Next
    End Function
End Module
