Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.IO.netCDF
Imports Microsoft.VisualBasic.Data.IO.netCDF.Components
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.SignalProcessing

''' <summary>
''' cdf writer of the signals
''' </summary>
Public Module cdfSignalsWriter

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

            Dim nsignals As Integer
            Dim data As CDFData
            Dim attrs As attribute()
            Dim [dim] As Dimension

            For Each signal As GeneralSignal In signals
                data = New CDFData With {
                    .numerics = signal.Measures _
                        .JoinIterates(signal.Strength) _
                        .ToArray
                }
                attrs = signal.meta _
                    .Select(Function(a)
                                Return New attribute With {
                                    .name = a.Key,
                                    .type = CDFDataTypes.CHAR,
                                    .value = a.Value
                                }
                            End Function) _
                    .ToArray
                attrs = attrs _
                    .JoinIterates({
                         New attribute With {.name = "ticks", .type = CDFDataTypes.INT, .value = signal.Measures.Length},
                         New attribute With {.name = NameOf(GeneralSignal.measureUnit), .type = CDFDataTypes.CHAR, .value = signal.measureUnit}
                     }) _
                    .ToArray
                [dim] = New Dimension With {
                    .name = dimension_prefix & (nsignals + 1),
                    .size = data.numerics.Length
                }

                cdffile.AddVariable(signal.reference, data, [dim], attrs)

                nsignals += 1
            Next

            Call cdffile.GlobalAttributes(New attribute With {.name = "signals", .type = CDFDataTypes.INT, .value = nsignals})
        End Using

        Return True
    End Function
End Module
