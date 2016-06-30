Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

''' <summary>
''' Phylip的命令行程序编程接口
''' </summary>
''' <remarks></remarks>
Public Class PhylipInvoker

    Dim PhylipBin As String

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="bin">程序组所在的文件夹</param>
    ''' <remarks></remarks>
    Sub New(bin As String)
        PhylipBin = bin
    End Sub

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="Matrix"></param>
    ''' <param name="ResultSaved"></param>
    ''' <param name="Settings">
    ''' Genetic Distance Matrix program, version 3.695
    '''
    ''' Settings for this run:
    '''   A   Input file contains all alleles at each locus?  One omitted at each locus
    '''   N                        Use Nei genetic distance?  Yes
    '''   C                Use Cavalli-Sforza chord measure?  No
    '''   R                   Use Reynolds genetic distance?  No
    '''   L                         Form of distance matrix?  Square
    '''   M                      Analyze multiple data sets?  No
    '''   0              Terminal type (IBM PC, ANSI, none)?  IBM PC
    '''   1            Print indications of progress of run?  Yes
    '''
    '''   Y to accept these or type the letter for one to change
    ''' </param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Gendist(Matrix As MatrixFile.MatrixFile, ResultSaved As String, Optional Settings As String = "Y") As Boolean
        Dim GendistExec As IORedirect = New IORedirect(PhylipBin & "/gendist.exe")
        Dim InputMatrixFile As String = My.Computer.FileSystem.GetTempFileName

        On Error Resume Next

        Call Matrix.GenerateDocument.SaveTo(InputMatrixFile, System.Text.Encoding.ASCII)
        Call FileIO.FileSystem.DeleteFile("./outfile", FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin)
        Call GendistExec.Start(True, New String() {InputMatrixFile, Settings, vbCrLf}, True)
        Call FileIO.FileSystem.DeleteFile(ResultSaved, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin)
        Call FileIO.FileSystem.CopyFile("./outfile", ResultSaved)

        Return GendistExec.ProcessInfo.ExitCode = 0
    End Function

    Public Overrides Function ToString() As String
        Return PhylipBin
    End Function
End Class
