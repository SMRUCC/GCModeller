#Region "Microsoft.VisualBasic::d387a44f7fc34c7bbfe31142252d0e81, ..\CLI_tools\c2\Reconstruction\Operation\Session.vb"

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

Imports Microsoft.VisualBasic.ConsoleDevice.STDIO
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions

Namespace Reconstruction : Partial Class Operation

        ''' <summary>
        ''' Reconstruction operation session instance
        ''' </summary>
        ''' <remarks></remarks>
        Public Class OperationSession

            ''' <summary>
            ''' 本地BLAST程序
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property LocalBLAST As LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.InteropService.InteropService
            ''' <summary>
            ''' 工作目录，即临时文件夹
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property WorkDir As String
            ''' <summary>
            ''' 待重建的目标数据库
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property ReconstructedMetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder
            ''' <summary>
            ''' 重建目标数据库的过程中所需要用到的参考数据源
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Subject As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder

            ''' <summary>
            ''' 获取两个基因组之间同源的蛋白质列表
            ''' </summary>
            ''' <value></value>
            ''' <returns>{Reconstructed, Subject}</returns>
            ''' <remarks></remarks>
            Public Property HomologousProteins As OperationSession.HomologousProteinsF

            Public Property MYSQL As Oracle.LinuxCompatibility.MySQL.ConnectionUri

            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="ReconstructedMetaCyc">目标待重建的MetaCyc数据库</param>
            ''' <param name="Subject">重建操作所使用的数据源</param>
            ''' <param name="p">本地BLAST程序初始化所需要的参数</param>
            ''' <remarks></remarks>
            Sub New(ReconstructedMetaCyc As String, Subject As String, p As LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.InteropService.InitializeParameter, WorkDir As String, MotifSampler As String)
                LocalBLAST = LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.InteropService.CreateInstance(p)
                Me.ReconstructedMetaCyc = New LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder(LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.PGDB.PreLoad(ReconstructedMetaCyc))
                Me.Subject = New LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder(LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.PGDB.PreLoad(Subject))
                Me.WorkDir = WorkDir

                Printf("Create a temporary workspace at location:\n  ""%s""", WorkDir)
                Call FileIO.FileSystem.CreateDirectory(WorkDir)
            End Sub

            Public Sub Intialize()
                Printf("Initialize operation session for the reconstruction...\n\n")
                Printf("Perfermance local blast operation to find out the homologous proteins...\n")
                Dim GrepScript As Microsoft.VisualBasic.Text.TextGrepScriptEngine = Microsoft.VisualBasic.Text.TextGrepScriptEngine.Compile("tokens | 2;tokens ' ' 0")
                Dim Collection As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File =
                    New LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.BBH.BidirectionalBesthit_BLAST(Me.LocalBLAST, Me.WorkDir) _
                        .Peformance(ReconstructedMetaCyc.Database.FASTAFiles.ProteinSourceFile,
                                    Subject.Database.FASTAFiles.ProteinSourceFile,
                                    AddressOf GrepScript.Grep,
                                    AddressOf GrepScript.Grep).GenerateCsvDocument(False)
                Dim s As String = WorkDir & "\homologous_proteins.csv"

                Call Collection.Save(Path:=s)

                If FileIO.FileSystem.FileExists(Me.Subject.Database.DataDir & "/protseq.fsa") Then '应用程序会根据是否存在目标参考数据库中的蛋白质序列数据文件来选择相应的多肽链蛋白质分子的等价性构建工作
                    Me.HomologousProteins = HomologousProteinsF.[New](Collection)
                Else
                    Dim MonomerEquals = New c2.Reconstruction.ObjectEquals.MonomerEquals(Session:=Me)
                    Call MonomerEquals.Initialize()
                    Me.HomologousProteins = MonomerEquals.GetList
                End If

                Dim rctSpecie = ReconstructedMetaCyc.Database.Species.First, sbjSpecie = Subject.Database.Species.First

                Printf("\n\n ***** End of initialization, there are %s proteins were homologous between the bacteria '%s' and '%s'.",
                        HomologousProteins.Count,
                        rctSpecie.Genome & " " & rctSpecie.StrainName, sbjSpecie.Genome & " " & sbjSpecie.StrainName)
                Printf("For more detail information about the homologous protein, please goto the temp file:\n  ""%s""", s)
            End Sub

            Public Class HomologousProteinsF : Implements Generic.IEnumerable(Of KeyValuePair(Of String, String))

                Public Property Proteins As KeyValuePair(Of String, String)()

                Public Shared Function [New](Collection As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File) As HomologousProteinsF
                    Dim LQuery = From item As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject
                                 In Collection
                                 Where Not String.IsNullOrEmpty(item(1))
                                 Select New KeyValuePair(Of String, String)(item(0), item(1)) '
                    Dim HomologousProteins As HomologousProteinsF = New HomologousProteinsF With {.Proteins = LQuery.ToArray}
                    Return HomologousProteins
                End Function

                ''' <summary>
                ''' 通过Subject中的蛋白质的UniqueId查找出所重建的数据库中等价的蛋白质单体
                ''' </summary>
                ''' <param name="Subject">Subject数据库之中的蛋白质的UniqueID属性值</param>
                ''' <returns></returns>
                ''' <remarks></remarks>
                Public Function GetUniqueID(Subject As String) As String
                    Dim LQuery = From item As KeyValuePair(Of String, String) In Me._Proteins.AsParallel Where String.Equals(item.Value, Subject) Select item.Key  '
                    Try
                        Return LQuery.First
                    Catch ex As Exception
                        Return ""
                    End Try
                End Function

                Public Iterator Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of String, String)) Implements IEnumerable(Of KeyValuePair(Of String, String)).GetEnumerator
                    For i As Integer = 0 To Me._Proteins.Count - 1
                        Yield Me._Proteins(i)
                    Next
                End Function

                Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
                    Yield GetEnumerator()
                End Function
            End Class
        End Class
    End Class
End Namespace
