#Region "Microsoft.VisualBasic::6abd7044ec4085725ea6911bf9af7267, ..\GCModeller\visualize\visualizeTools\GeneticClockDiagram\ShellScriptAPI.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.CommandLine
Imports SMRUCC.genomics.InteractionModel
Imports Oracle.Java.IO.Properties.Reflector
Imports Microsoft.VisualBasic.DataMining.Framework.Serials.PeriodAnalysis
Imports Microsoft.VisualBasic

Namespace GeneticClock

    <[Namespace]("Diagram.Genetic_Clock")> Public Module ShellScriptAPI

        <ExportAPI("Doppler_Effect.New_Analysis_Session")>
        Public Function CreateDopplerEffectsAnalyser(ExperimentData As SerialsData()) As DopplerEffect
            Return New DopplerEffect(ExperimentData)
        End Function

        <ExportAPI("Doppler_Effect.Calculate")>
        Public Function CalculateDopplerEffect([operator] As DopplerEffect, IdCollection As Generic.IEnumerable(Of Object), Optional WindowSize As Integer = 6) As SerialsData()
            Return [operator].CalculateDopplerEffect((From item In IdCollection Let strValue As String = item.ToString Select strValue).ToArray, WindowSize)
        End Function

        <ExportAPI("Read.csv.Serials")>
        Public Function LoadExperimentData(CsvFile As String) As SerialsData()
            Return DataServicesExtension.LoadCsv(CsvFile)
        End Function

        <ExportAPI("Diagram.Create")>
        Public Function DrawDiagram(data As SerialsData(), scale As Integer) As System.Drawing.Image
            Return DrawingDevice.Draw(data, Scale:=scale)
        End Function

        <ExportAPI("Save.Bitmap")>
        Public Function SaveImageBitmap(image As System.Drawing.Image, saveto As String) As Boolean
            Call Console.WriteLine("Image data was saved to location: ""{0}"".", saveto)
            Call image.Save(saveto, System.Drawing.Imaging.ImageFormat.Bmp)
            Return True
        End Function

        <ExportAPI("Data.Interpolate")>
        Public Function Interpolate(data As SerialsData(), n As Integer) As SerialsData()
            Dim LQuery = (From item In data Select New SerialsData With {.Tag = item.Tag, .ChunkBuffer = Interpolate(item.ChunkBuffer, n)}).ToList
            Return LQuery.ToArray
        End Function

        ''' <summary>
        ''' 默认第一个元素总是表示时间，所以第一个时间不会被去除
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Data.Select")>
        Public Function [Select](data As SerialsData(), id As Generic.IEnumerable(Of Object)) As SerialsData()
            Dim idlist As String() = (From item In id Where Not item Is Nothing Let strValue As String = item.ToString Select strValue Order By strValue Ascending).ToArray
            Dim LQuery = (From valueId As String In idlist
                          Let SelectedValue = Function() As SerialsData
                                                  Dim value = data.GetItem(valueId)
                                                  If value.ChunkBuffer.IsNullOrEmpty Then
                                                      Call Console.WriteLine("Could not found object data ""{0}""", valueId)
                                                      Return Nothing
                                                  End If
                                                  Return value
                                              End Function() Where Not SelectedValue.ChunkBuffer.IsNullOrEmpty Select SelectedValue).ToList
            Call LQuery.Insert(0, data.First)
            Return LQuery.ToArray
        End Function

        <ExportAPI("Data.Filtering_With_Periods")>
        Public Function FilteringPeriodData(data As SerialsData(), WindowSize As Integer) As SerialsData()
            Dim Chunkbuffer = (From item In data Select New SerialsVarialble With {.Identifier = item.Tag, .SerialsData = item.ChunkBuffer}).ToArray
            Dim LQuery = (From item In Chunkbuffer Select PeriodAnalysis.Analysis(item, WindowSize)).ToArray
            Throw New NotImplementedException
        End Function

        Private Function Interpolate(data As Double(), n As Integer) As Double()
            For i As Integer = 0 To n
                data = ColorRender.Interpolate(data)
            Next
            Return data
        End Function
    End Module
End Namespace
