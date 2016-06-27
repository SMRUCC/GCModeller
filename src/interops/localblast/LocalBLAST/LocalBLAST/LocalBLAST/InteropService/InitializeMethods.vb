Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace LocalBLAST.InteropService

    <PackageNamespace("NCBI.Blast.Initialize")>
    Public Module InitializeMethods

        ReadOnly __createService As Dictionary(Of Program, System.Func(Of String, InteropService)) =
            New Dictionary(Of Program, Func(Of String, InteropService)) From {
 _
            {Program.LocalBlast, Function(BlastBin As String) New Programs.LocalBLAST(BlastBin)},
            {Program.BlastPlus, Function(BlastBin As String) New Programs.BLASTPlus(BlastBin)}
        }

        <ExportAPI("CreateSession")>
        Public Function CreateInstance(p As InitializeParameter) As InteropService
            If __createService.ContainsKey(p.Program) Then
                Return __createService(p.Program)(p.BlastBin)
            Else
                Return Nothing
            End If
        End Function

        <ExportAPI("CreateSession")>
        Public Function CreateInstance(Bin As String, Type As Program) As InteropService
            Return __createService(Type)(Bin)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Name">Only two value: 'blastplus' or 'localblast'</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Get.Type")>
        Public Function GetProgram(Name As String) As Program
            Return CType(InStr(LocalBLAST.InteropService.PROGRAMS, Name) / 10, Program)
        End Function

        Public Enum Program
            BlastPlus
            LocalBlast
        End Enum

        Public Const PROGRAMS As String = "blastplus+localblast"
    End Module

    ''' <summary>
    ''' 对本地BLAST外部命令的初始化参数
    ''' </summary>
    ''' <remarks></remarks>
    Public Class InitializeParameter

        ''' <summary>
        ''' BLAST程序的类型
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Program As LocalBLAST.InteropService.Program
        ''' <summary>
        ''' BLAST程序的可执行文件的存放的文件夹
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property BlastBin As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="BLASTBIN">BLAST程序的可执行文件的存放文件夹</param>
        ''' <param name="Type">BLAST程序的类型</param>
        ''' <remarks></remarks>
        Sub New(BLASTBIN As String, Optional Type As Program = LocalBLAST.InteropService.Program.LocalBlast)
            Me.BlastBin = BLASTBIN
            Me.Program = Type
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("{0}: {1}", Program, BlastBin)
        End Function
    End Class
End Namespace