#Region "Microsoft.VisualBasic::be1886e3e77e5cf9f08deece6317de29, analysis\RNA-Seq\Toolkits.RNA-Seq.RTools\PfsNET\RInvoke\PfsNET.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:

    '     Module PfsNETRInvoke
    ' 
    '         Function: Evaluate, InitializeSession, RInvoke
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports RDotNET
Imports RDotNET.Extensions
Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.API.base
Imports SMRUCC.genomics.Analysis.PFSNet

#Const DEBUG = True

Namespace PfsNET

    ''' <summary>
    ''' PFSNet computes signifiance of subnetworks generated through a process that selects genes in a pathway based on fuzzy scoring and a majority voting procedure.
    ''' (一个程序仅一个实例，是否是因为将Module修改为Class的原因所以导致了在64位服务器上面的Java初始化失败？？？？)
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <Package("PfsNET.R.Invoke",
                      Description:="PFSNet computes signifiance of subnetworks generated through a process that selects genes in a pathway based on fuzzy scoring and a majority voting procedure.",
                      Cites:="Lim, K. and L. Wong (2014). ""Finding consistent disease subnetworks Using PFSNet."" Bioinformatics 30(2): 189-196.
<p> MOTIVATION: Microarray data analysis is often applied to characterize disease populations by identifying individual genes linked to the disease. 
                In recent years, efforts have shifted to focus on sets of genes known to perform related biological functions (i.e. in the same pathways). 
                Evaluating gene sets reduces the need to correct for false positives in multiple hypothesis testing. However, pathways are often large, 
                and genes in the same pathway that do not contribute to the disease can cause a method to miss the pathway. 
                In addition, large pathways may not give much insight to the cause of the disease. Moreover, when such a method is applied independently 
                to two datasets of the same disease phenotypes, the two resulting lists of significant pathways often have low agreement. 
                RESULTS: We present a powerful method, PFSNet, that identifies smaller parts of pathways (which we call subnetworks), and show that significant subnetworks (and the genes therein) discovered by PFSNet are up to 51% (64%) more consistent across independent datasets of the same disease phenotypes, even for datasets based on different platforms, than previously published methods. We further show that those methods which initially declared some large pathways to be insignificant would declare subnetworks detected by PFSNet in those large pathways to be significant, if they were given those subnetworks as input instead of the entire large pathways. AVAILABILITY: http://compbio.ddns.comp.nus.edu.sg:8080/pfsnet/

",
                        Publisher:="Kevin Lim and Limsoon Wong",
                        Url:="http://compbio.ddns.comp.nus.edu.sg:8080/pfsnet/")>
    Public Module PfsNETRInvoke

        ''' <summary>
        ''' 假若<paramref name="java_class"></paramref>参数为空，则初始化为非rJava的调用版本
        ''' </summary>
        ''' <param name="java_class"></param>
        ''' <param name="R_HOME"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Init()")>
        Public Function InitializeSession(<Parameter("Filter.java")> Optional java_class As String = "", Optional R_HOME As String = "") As Boolean
            If Not String.IsNullOrEmpty(R_HOME) Then
                Call TryInit(R_HOME)
            End If

            Call library("igraph")
            Call New RScriptInvoke(Encoding.ASCII.GetString(My.Resources.onLoad)).PrintSTDOUT()

            If Not String.IsNullOrEmpty(java_class) Then
                Dim JavaClassPath As String

                Call "Try to start the Java VMs...".__DEBUG_ECHO

                If FileIO.FileSystem.FileExists(java_class) Then
                    JavaClassPath = FileIO.FileSystem.GetFileInfo(java_class).FullName
                    Call $"Java class file path found at location ""{JavaClassPath.ToFileURL}""!".__DEBUG_ECHO
                Else
                    Throw New Exception("Required java class file is not found!")
                End If

                Call "call library('rJava')".__DEBUG_ECHO
                Call New RScriptInvoke("library(rJava)").PrintSTDOUT()
                Call "call .jinit()".__DEBUG_ECHO
                Call New RScriptInvoke(".jinit()").PrintSTDOUT()
                Call "call .jaddClassPath() for the user specific Java class file...".__DEBUG_ECHO
                Call New RScriptInvoke(String.Format(".jaddClassPath(""{0}"")", JavaClassPath.Replace("\", "/"))).PrintSTDOUT()
                Call "R create pfsnet function delegate.... ".__DEBUG_ECHO
                Call New RScriptInvoke(Encoding.ASCII.GetString(My.Resources.pfsnet)).PrintSTDOUT()
            Else '调用非Java版本
                Call "Java filter class is not assigned, using the non-java edition!".Warning
                Call New RScriptInvoke(Encoding.ASCII.GetString(My.Resources.pfsnet_not_rJava)).PrintSTDOUT()
            End If

            Return True
        End Function

        <ExportAPI("Analysis.Invoke")>
        Public Function RInvoke(file1 As String, file2 As String, file3 As String,
                                Optional b As String = "0.5",
                                Optional t1 As String = "0.95",
                                Optional t2 As String = "0.85",
                                Optional n As String = "1000") As SymbolicExpression

            Call Console.WriteLine()

            file1 = FileIO.FileSystem.GetFileInfo(file1).FullName
            file2 = FileIO.FileSystem.GetFileInfo(file2).FullName
            file3 = FileIO.FileSystem.GetFileInfo(file3).FullName

            Dim Script As New PfsNETScript(b, t1, t2, n) With {
                .File1 = file1,
                .File2 = file2,
                .File3 = file3
            }
            Dim Expr As SymbolicExpression = Script.__call

            Try
                Call Script.RScript.SaveTo("./PfsNET.txt")
            Catch ex As Exception
                Call App.LogException(ex)
                Call ex.PrintException
            End Try

            Return Expr
        End Function

        ''' <summary>
        ''' R脚本版本的PfsNET计算引擎
        ''' </summary>
        ''' <param name="file1"></param>
        ''' <param name="file2"></param>
        ''' <param name="file3"></param>
        ''' <param name="b"></param>
        ''' <param name="t1"></param>
        ''' <param name="t2"></param>
        ''' <param name="n"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Invoke")>
        Public Function Evaluate(file1 As String, file2 As String, file3 As String,
                                 Optional b As String = "0.5",
                                 Optional t1 As String = "0.95",
                                 Optional t2 As String = "0.85",
                                 Optional n As String = "1000") As PFSNetResultOut

            Dim STDOutput As String()

            Call Threading.Thread.Sleep(1000) '为了防止在进行pfsnet的批量计算的过程之中出现文件被当前进程占用的情况出现而导致的计算错误，需要在这里休眠1秒钟，使当前的进程可以完全的释放数据文件的文件句柄

            Call Console.WriteLine()

            file1 = FileIO.FileSystem.GetFileInfo(file1).FullName
            file2 = FileIO.FileSystem.GetFileInfo(file2).FullName
            file3 = FileIO.FileSystem.GetFileInfo(file3).FullName

            Dim script As New PfsNETScript(b, t1, t2, n) With {
                .File1 = file1,
                .File2 = file2,
                .File3 = file3
            }

            SyncLock R
                With R
                    STDOutput = .WriteLine(script)
                End With
            End SyncLock

            STDOutput = LinqAPI.Exec(Of String) <=
 _
                From strLine As String
                In STDOutput
                Select strLine.Replace(vbCr, "").Replace(vbLf, "")

            For Each Line As String In STDOutput
                Call Console.WriteLine(Line)
            Next

            Call Console.WriteLine()

            Try
                Dim log As String = Now.ToString.NormalizePathString
                log = Settings.Session.LogDIR & $"/pfsnet_debug.{log}.log"
                STDOutput.FlushAllLines(log, encoding:=Encodings.ASCII)
            Catch ex As Exception

            End Try

            Dim Parsed = SubnetParser.TryParse(STDOutput)
            Dim Result = SubnetParser.GenerateDefaultStruct(Parsed, "")
            Result.STD_OUTPUT = STDOutput
            Return Result
        End Function
    End Module
End Namespace
