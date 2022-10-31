#Region "Microsoft.VisualBasic::758a7b10992d8c29e635cabd0a5082b0, localblast\LocalBLAST\LocalBLAST\BlastOutput\BLASTOutput.vb"

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

'     Class IBlastOutput
' 
'         Properties: Database, FilePath
' 
'         Function: ToString
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Scripting
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.Views
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace LocalBLAST.BLASTOutput

    ''' <summary>
    ''' Blast程序结果对外输出的统一接口类型对象
    ''' </summary>
    ''' <remarks>
    ''' Reader文件夹之下为各种格式的日志文件的读取类对象
    ''' 对于BLAST日志文件，则有一个BlastLogFile对象作为对外保存和其他程序读取的统一接口
    ''' </remarks>
    Public MustInherit Class IBlastOutput

        Public Const HITS_NOT_FOUND As String = "HITS_NOT_FOUND"

        <XmlIgnore> <ScriptIgnore>
        Public Property FilePath As String

        Public MustOverride Function Grep(Query As TextGrepMethod, Hits As TextGrepMethod) As IBlastOutput
        ''' <summary>
        ''' 仅导出每条记录的第一个最佳匹配的结果
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function ExportBestHit(Optional coverage As Double = 0.5, Optional identities_cutoff As Double = 0.15) As LocalBLAST.Application.BBH.BestHit()
        Public MustOverride Function ExportOverview() As Overview
        ''' <summary>
        ''' 导出每条记录中的所有最佳的匹配结果
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function ExportAllBestHist(Optional coverage As Double = 0.5, Optional identities_cutoff As Double = 0.15) As LocalBLAST.Application.BBH.BestHit()

        <XmlElement("BlastOutput_db")>
        Public Overridable Property Database As String

        Public Overrides Function ToString() As String
            Return FilePath
        End Function

        Public MustOverride Function CheckIntegrity(QuerySource As FastaFile) As Boolean
    End Class
End Namespace
