Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream
Imports Microsoft.VisualBasic.MMFProtocol.Pipeline
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace Graphics

    <PackageNamespace("PieChart",
                  Description:="Pie charts are not recommended in the R documentation, and their features are somewhat limited. 
                  The authors recommend bar or dot plots over pie charts because people are able to judge length more accurately than volume. 
                  Pie charts are created with the function pie(x, labels=) where x is a non-negative numeric vector indicating the area of each slice and labels= notes a character vector of names for the slices.",
                  Publisher:="amethyst.asuka@gcmodeller.org", Url:="http://statmethods.net/graphs/pie.html")>
    Public Module PieChart

        <ExportAPI("Pie", Info:="Draw a simple pie chart.")>
        Public Function PieSimple(slices As IEnumerable(Of KeyValuePair(Of String, Double)),
                              Optional Title As String = "",
                              <Parameter("Path.Save")> Optional SaveTo As String = "./pie.png") As Boolean
            Dim Script As String =
$"# Simple Pie Chart
 slices <- c({String.Join(", ", (From n In slices Select CStr(n.Value)).ToArray)})
 lbls <- c({String.Join(", ", (From n In slices Select $"""{n.Key}""").ToArray)})
{GraphicsDevice.tiff(plot:=$"pie(slices, labels = lbls, main=""{Title}"")", filename:=SaveTo, width:=3000, height:=2500)}"
            Dim STD As String() = RServer.WriteLine(Script)
            Return True
        End Function

        <ExportAPI("Data.Frame")>
        Public Function DataFrame(path As String) As KeyValuePair(Of String, Double)()
            Dim Csv = File.Load(path)
            Dim LQuery = (From row As RowObject In Csv.Skip(1).AsParallel
                          Select New KeyValuePair(Of String, Double)(row(0), Val(row(1)))).ToArray
            Return LQuery
        End Function

        <ExportAPI("Data.Frame")>
        Public Function DataFrame(data As IEnumerable(Of Object)) As KeyValuePair(Of String, Double)()
            Dim LQuery = (From obj In data.AsParallel
                          Let values = InputHandler.CastArray(Of Object)(obj)
                          Select New KeyValuePair(Of String, Double)(
                          InputHandler.ToString(values(0)),
                          Val(InputHandler.ToString(values(1))))).ToArray
            Return LQuery
        End Function
    End Module
End Namespace