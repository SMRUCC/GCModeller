#Region "Microsoft.VisualBasic::f93e95e9d25e810ad151d185724f7268, localblast\LocalBLAST\LocalBLAST\BlastOutput\GrepOperation.vb"

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

    '     Class GrepOperation
    ' 
    '         Constructor: (+3 Overloads) Sub New
    '         Function: Grep
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Scripting

Namespace LocalBLAST.BLASTOutput

    Public Class GrepOperation

        Dim _query, _subject As TextGrepMethod

        Sub New(Script As String)
            Dim Engine = TextGrepScriptEngine.Compile(Script)
            _query = Engine.PipelinePointer
            _subject = Engine.PipelinePointer
        End Sub

        Sub New(Query As String, Subject As String)
            _query = TextGrepScriptEngine.Compile(Query).PipelinePointer
            _subject = TextGrepScriptEngine.Compile(Subject).PipelinePointer
        End Sub

        Sub New(Query As TextGrepMethod, Subject As TextGrepMethod)
            _query = Query
            _subject = Subject
        End Sub

        Public Function Grep(Of Log As IBlastOutput)(LogOutput As Log) As Log
            Call LogOutput.Grep(_query, _subject)
            Return LogOutput
        End Function
    End Class
End Namespace
