Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components
Imports Microsoft.VisualBasic.Math.SignalProcessing

''' <summary>
''' cdf writer of the signals
''' </summary>
Public Module cdfSignalsWriter

    <Extension>
    Public Function WriteCDF(signals As IEnumerable(Of GeneralSignal), file As String) As Boolean
        Using cdffile As New CDFWriter(file)
            Call cdffile.Dimensions(Dimension.Double, Dimension.Float, Dimension.Integer, Dimension.Long, Dimension.Text(fixedChars:=1024))
            Call cdffile.GlobalAttributes(New attribute With {.name = "time", .type = CDFDataTypes.CHAR, .value = Now.ToString})
            Call cdffile.GlobalAttributes(New attribute With {.name = "filename", .type = CDFDataTypes.CHAR, .value = file.FileName})
            Call cdffile.GlobalAttributes(New attribute With {.name = "github", .type = CDFDataTypes.CHAR, .value = LICENSE.githubURL})

            Dim nsignals As Integer
            Dim data1, data2 As CDFData
            Dim attrs As attribute()

            For Each signal As GeneralSignal In signals
                data1 = New CDFData With {.numerics = signal.Measures}
                data2 = New CDFData With {.numerics = signal.Strength}
                attrs = signal.meta _
                    .Select(Function(a)
                                Return New attribute With {
                                    .name = a.Key,
                                    .type = CDFDataTypes.CHAR,
                                    .value = a.Value
                                }
                            End Function) _
                    .ToArray
                cdffile.AddVariable("axis_" & (nsignals + 1), data1, Dimension.Double, attrs)
                cdffile.AddVariable("signal_" & (nsignals + 1), data2, Dimension.Double, attrs)

                nsignals += 1
            Next

            Call cdffile.GlobalAttributes(New attribute With {.name = "signals", .type = CDFDataTypes.INT, .value = nsignals})
        End Using

        Return True
    End Function
End Module
