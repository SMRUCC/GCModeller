#Region "Microsoft.VisualBasic::6f6fac8d1404353fd35801d1b47cde75, ..\localblast\LocalBLAST\LocalBLAST\BlastOutput\GrepOperation.vb"

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

Imports Microsoft.VisualBasic.Text

Namespace LocalBLAST.BLASTOutput

    Public Class GrepOperation

        Dim _query, _subject As TextGrepMethod

        Sub New(Script As String)
            Dim Engine = Microsoft.VisualBasic.Text.TextGrepScriptEngine.Compile(Script)
            _query = Engine.Method
            _subject = Engine.Method
        End Sub

        Sub New(Query As String, Subject As String)
            _query = Microsoft.VisualBasic.Text.TextGrepScriptEngine.Compile(Query).Method
            _subject = Microsoft.VisualBasic.Text.TextGrepScriptEngine.Compile(Subject).Method
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
