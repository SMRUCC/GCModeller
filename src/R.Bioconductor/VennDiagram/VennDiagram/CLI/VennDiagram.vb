Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Terminal.STDIO
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports RDotNet.Extensions.VisualBasic.RSystem
Imports RDotNet.Extensions.VisualBasic
Imports RDotNet.Extensions.Bioinformatics.VennDiagram.ModelAPI
Imports Microsoft.VisualBasic.CommandLine

<PackageNamespace("VennTools.CLI", Category:=APICategories.CLI_MAN,
                  Description:="Tools for creating venn diagram model for the R program and venn diagram visualize drawing.",
                  Publisher:="xie.guigang@gmail.com",
                  Url:="http://gcmodeller.org")>
Public Module CLI

    <ExportAPI(".Draw",
               Info:="Draw the venn diagram from a csv data file, you can specific the diagram drawing options from this command switch value. " &
                     "The generated venn dragram will be saved as tiff file format.",
        Usage:=".Draw -i <csv_file> [-t <diagram_title> -o <_diagram_saved_path> -s <partitions_option_pairs> -rbin <r_bin_directory>]",
        Example:=".Draw -i /home/xieguigang/Desktop/genomes.csv -t genome-compared -o ~/Desktop/xcc8004.tiff -s ""Xcc8004,blue,Xcc 8004;ecoli,green,Ecoli. K12;pa14,yellow,PA14;ftn,black,FTN;aciad,red,ACIAD""")>
    <ParameterInfo("-i",
        Description:="The csv data source file for drawing the venn diagram graph.",
        Example:="/home/xieguigang/Desktop/genomes.csv")>
    <ParameterInfo("-t", True,
        Description:="Optional, the venn diagram title text",
        Example:="genome-compared")>
    <ParameterInfo("-o", True,
        Description:="Optional, the saved file location for the venn diagram, if this switch value is not specific by the user then \n" &
                     "the program will save the generated venn diagram to user desktop folder and using the file name of the input csv file as default.",
        Example:="~/Desktop/xcc8004.tiff")>
    <ParameterInfo("-s", True,
        Description:="Optional, the profile settings for the partitions in the venn diagram, each partition profile data is\n " &
                     "in a key value paired like: name,color, and each partition profile pair is seperated by a ';' character.\n" &
                     "If this switch value is not specific by the user then the program will trying to parse the partition name\n" &
                     "from the column values and apply for each partition a randomize color.",
        Example:="Xcc8004,blue,Xcc 8004;ecoli,green,Ecoli. K12;pa14,yellow,PA14;ftn,black,FTN;aciad,red,ACIAD")>
    <ParameterInfo("-rbin", True,
        Description:="Optional, Set up the r bin path for drawing the venn diagram, if this switch value is not specific by the user then \n" &
                     "the program just output the venn diagram drawing R script file in a specific location, or if this switch \n" &
                     "value is specific by the user and is valid for call the R program then will output both venn diagram tiff image " &
                     "file and R script for drawing the output venn diagram.\n" &
                     "This switch value is just for the windows user, when this program was running on a LINUX/UNIX/MAC platform operating \n" &
                     "system, you can ignore this switch value, but you should install the R program in your linux/MAC first if you wish to\n " &
                     "get the venn diagram directly from this program.",
        Example:="C:\\R\\bin\\")>
    Public Function VennDiagramA(args As CommandLine) As Integer
        Dim inds As String = args("-i")
        Dim title As String = args.GetValue("-t", inds.BaseName)
        Dim partitionsOption As String = args("-s")
        Dim out As String = args.GetValue("-o", App.Desktop & $"/{title.NormalizePathString(True)}_venn.tiff")
        Dim RBin As String = args("-rbin")

        If Not inds.FileExists Then '-i开关参数无效
            printf("Could not found the source file!")
            Return -1
        Else
            out = UnixPath(out, True)
        End If

        Return __run(inds, title, partitionsOption, out, RBin)
    End Function

    Private Function __run(inData As String, title As String, options As String, out As String, R_HOME As String) As Integer
        Dim dataset As DocumentStream.File = New DocumentStream.File(inData)
        Dim VennDiagram As VennDiagram = RModelAPI.Generate(source:=dataset)

        If String.IsNullOrEmpty(options) Then '从原始数据中进行推测
            VennDiagram += From col As String In dataset.First Select {col, GetRandomColor()} '
        Else '从用户输入之中进行解析
            VennDiagram += From s As String In options.Split(CChar(";")) Select s.Split(CChar(",")) '
        End If

        VennDiagram.Title = title
        VennDiagram.saveTiff = out

        Dim RScript As String = VennDiagram.RScript
        Dim EXPORT As String = FileIO.FileSystem.GetParentPath(out)
        EXPORT = $"{EXPORT}/{title.NormalizePathString}_venn.r"

        If Not R_HOME.DirectoryExists Then
            Call TryInit()
        Else
            Call TryInit(R_HOME)
        End If

        Call RScript.SaveTo(EXPORT, Encodings.ASCII.GetEncodings)
        Call VennDiagram.SaveAsXml(EXPORT.TrimSuffix & ".Xml")
        Call RSystem.Source(EXPORT)

        printf("The venn diagram r script were saved at location:\n '%s'", EXPORT)

        Call Process.Start(out)

        Return 0
    End Function

    Private Function GetRandomColor() As String
        Call VBMath.Randomize()
        Return RSystem.RColors(Rnd() * (RSystem.RColors.Length - 1))
    End Function

    ''' <summary>
    ''' Supports directly run Venn Xml model or csv dataset raw data
    ''' </summary>
    ''' <param name="path"></param>
    ''' <param name="args">null</param>
    ''' <returns></returns>
    Public Function DrawFile(path As String, args As CommandLine) As Integer
        Dim ext As String = path.Split("."c).Last
        If String.Equals(ext, "csv", StringComparison.OrdinalIgnoreCase) Then
            Return __run(path, path.BaseName, Nothing, $"{App.Desktop}/{path.BaseName}_venn.tiff", Nothing)
        Else
            Dim venn As VennDiagram = path.LoadXml(Of VennDiagram)
            Dim EXPORT As String = venn.saveTiff.TrimSuffix & ".r"

            Call TryInit()
            Call venn.RScript.SaveTo(EXPORT, Encodings.ASCII.GetEncodings)
            Call RSystem.Source(EXPORT)
            Call Process.Start(venn.saveTiff)

            Return 0
        End If
    End Function
End Module
