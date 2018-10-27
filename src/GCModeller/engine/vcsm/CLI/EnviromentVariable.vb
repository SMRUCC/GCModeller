#Region "Microsoft.VisualBasic::fb690ffba5b25621730cb22452e823a6, vcsm\CLI\EnviromentVariable.vb"

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

    ' Module CommandLines
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection

Partial Module CommandLines

    ' ''' <summary>
    ' ''' 
    ' ''' </summary>
    ' ''' <param name="CommandLine"></param>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    '<Command("matrix", info:="", usage:="matrix <systemTarget> <- <matrix_file>", example:="")>
    '<ParameterDescription("<systemTarget>", optional:=False,
    '    description:="Set up the matrix profile data file for the sub system module calculation parameter," & vbCrLf &
    '                    "Metabolism, ExpressionNetwork")>
    'Public Shared Function Matrix(CommandLine As Microsoft.VisualBasic.CommandLine.CommandLine) As Integer
    '    Using MatrixFile As Microsoft.VisualBasic.ComponentModel.Settings.Settings(Of LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.ObjectModels.System.Matrix.MatrixFile) =
    '        Microsoft.VisualBasic.ComponentModel.Settings.Settings(Of LANS.SystemsBiology.GCModeller.ModellingEngine.EngineSystem.ObjectModels.System.Matrix.MatrixFile).LoadFile(Program.MatrixFile)
    '        Dim SystemTarget As String = CommandLine.Parameters.First
    '        Dim FilePath As String = CommandLine.Parameters(2)

    '        Call MatrixFile.Set(SystemTarget, FilePath)   '向文件之中写入配置数据
    '    End Using
    '    Return 0
    'End Function
End Module
