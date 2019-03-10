#Region "Microsoft.VisualBasic::47268217b096e7bb1487e49d21ad0cd9, sub-system\PLAS.NET\SSystem\Script\ScriptWriter.vb"

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

    '     Module ScriptWriter
    ' 
    '         Function: (+2 Overloads) WriteScript
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text

Namespace Script

    Public Module ScriptWriter

        ''' <summary>
        ''' Generates the script text from the data model
        ''' </summary>
        ''' <param name="model"></param>
        ''' <param name="path"></param>
        ''' <returns></returns>
        <Extension>
        Public Function WriteScript(model As Model, path As String) As Boolean
            Return model.WriteScript(path.Open)
        End Function

        ''' <summary>
        ''' 向流指针之中写入模型数据
        ''' </summary>
        ''' <param name="model"></param>
        ''' <param name="s"></param>
        ''' <returns></returns>
        <Extension>
        Public Function WriteScript(model As Model, ByRef s As Stream) As Boolean
            Dim sb As New StreamWriter(s)

            For Each rxn In model.sEquations
                Call sb.WriteLine($"RXN {rxn.x}={rxn.Expression}")
            Next

            Call sb.WriteLine()

            For Each var In model.Vars
                Call sb.WriteLine($"INIT {var.UniqueId}={var.Value}")
            Next
            Call sb.WriteLine()
            Call sb.WriteLine("FINALTIME " & model.FinalTime)
            Call s.Flush()

            Return True
        End Function
    End Module
End Namespace
