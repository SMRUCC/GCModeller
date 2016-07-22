#Region "Microsoft.VisualBasic::94c02dfb09070b8d9c69cd4c07dee2d0, ..\interops\visualize\Circos\Circos\ConfFiles\Extensions.vb"

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

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Settings
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas.Highlights

Namespace Configurations

    Public Module Extensions

        ''' <summary>
        ''' Generates the docuemtn text data for write circos file.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="data"></param>
        ''' <param name="Tag"></param>
        ''' <param name="IndentLevel"></param>
        ''' <param name="InsertElements"></param>
        ''' <returns></returns>
        <ExportAPI("GenerateDoc", Info:="Generates the docuemtn text data for write circos file.")>
        <Extension> Public Function GenerateCircosDocumentElement(Of T As CircosDocument) _
        (data As T,
         Tag As String,
         IndentLevel As Integer,
         InsertElements As IEnumerable(Of ICircosDocNode)) As String

            Dim IndentBlanks As String = New String(" "c, IndentLevel + 2)
            Dim sb As New StringBuilder(1024)

            For Each strLine As String In SimpleConfig.GenerateConfigurations(Of T)(data)
                Call sb.AppendLine($"{IndentBlanks}{strLine}")
            Next

            If Not InsertElements.IsNullOrEmpty Then
                Call sb.AppendLine()

                For Each item In InsertElements
                    If item Is Nothing Then
                        Continue For
                    End If

                    Call sb.AppendLine(item.GenerateDocument(IndentLevel + 2))
                Next
            End If

            Return String.Format("<{0}>{1}{2}{1}</{0}>", Tag, vbCrLf, sb.ToString)
        End Function

        ''' <summary>
        ''' 不可以使用并行拓展，因为有顺序之分
        ''' 
        ''' {SpeciesName, Color}
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <Extension>
        Public Function GetBlastAlignmentData(circos As Circos) As NamedValue(Of String)()
            Dim LQuery As NamedValue(Of String)() =
                LinqAPI.Exec(Of NamedValue(Of String)) <= From trackPlot As ITrackPlot
                                                          In circos.Plots
                                                          Where String.Equals(trackPlot.type, "highlight", StringComparison.OrdinalIgnoreCase) AndAlso
                                                              TypeOf trackPlot.TracksData Is BlastMaps
                                                          Let Alignment = DirectCast(trackPlot.TracksData, BlastMaps)
                                                          Select New NamedValue(Of String)(Alignment.SubjectSpecies, Alignment.SpeciesColor)
            Return LQuery
        End Function
    End Module
End Namespace
