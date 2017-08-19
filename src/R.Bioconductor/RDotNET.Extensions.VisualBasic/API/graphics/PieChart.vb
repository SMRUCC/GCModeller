#Region "Microsoft.VisualBasic::adf9d72c6671857ec30de2e5bd752dc7, ..\R.Bioconductor\RDotNET.Extensions.VisualBasic\API\graphics\PieChart.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.MMFProtocol.Pipeline
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.packages.Graphics

Namespace API.Graphics

    <Package("PieChart",
                  Description:="Pie charts are not recommended in the R documentation, and their features are somewhat limited. 
                  The authors recommend bar or dot plots over pie charts because people are able to judge length more accurately than volume. 
                  Pie charts are created with the function pie(x, labels=) where x is a non-negative numeric vector indicating the area of each slice and labels= notes a character vector of names for the slices.",
                  Publisher:="amethyst.asuka@gcmodeller.org", Url:="http://statmethods.net/graphs/pie.html")>
    Public Module PieChart

        <ExportAPI("Pie", Info:="Draw a simple pie chart.")>
        Public Function PieSimple(slices As IEnumerable(Of NamedValue(Of Double)),
                              Optional Title As String = "",
                              <Parameter("Path.Save")> Optional SaveTo As String = "./pie.png") As Boolean

            Call $"# Simple Pie Chart
slices <- c({String.Join(", ", (From n In slices Select CStr(n.Value)).ToArray)})
lbls   <- c({String.Join(", ", (From n In slices Select $"""{n.Name}""").ToArray)})
{GraphicsDevice.tiff(plot:=$"pie(slices, labels = lbls, main=""{Title}"")", filename:=SaveTo, width:=3000, height:=2500)}".__call
            Return True
        End Function

        <ExportAPI("Data.Frame")>
        Public Function DataFrame(path As String) As NamedValue(Of Double)()
            Dim df As File = File.Load(path)
            Dim LQuery = LinqAPI.Exec(Of NamedValue(Of Double)) <=
 _
                From row As RowObject
                In df.Skip(1).AsParallel
                Select New NamedValue(Of Double) With {
                    .Name = row(0),
                    .Value = Val(row(1))
                }

            Return LQuery
        End Function

        <ExportAPI("Data.Frame")>
        Public Function DataFrame(data As IEnumerable(Of Object)) As NamedValue(Of Double)()
            Dim LQuery = LinqAPI.Exec(Of NamedValue(Of Double)) <=
 _
                From obj As Object
                In data.AsParallel
                Let values = InputHandler.CastArray(Of Object)(obj)
                Let name As String = InputHandler.ToString(values(0)),
                    value As Double = Val(InputHandler.ToString(values(1)))
                Select New NamedValue(Of Double) With {
                    .Name = name,
                    .Value = value
                }

            Return LQuery
        End Function
    End Module
End Namespace
