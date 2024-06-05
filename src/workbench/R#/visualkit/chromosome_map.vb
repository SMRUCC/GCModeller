#Region "Microsoft.VisualBasic::0e1b7d07b2e3749d5f029b52b057f7b5, R#\visualkit\chromosome_map.vb"

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


    ' Code Statistics:

    '   Total Lines: 52
    '    Code Lines: 32 (61.54%)
    ' Comment Lines: 15 (28.85%)
    '    - Xml Docs: 93.33%
    ' 
    '   Blank Lines: 5 (9.62%)
    '     File Size: 1.94 KB


    ' Module chromosome_map
    ' 
    '     Function: config, draw
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Visualize
Imports SMRUCC.genomics.Visualize.ChromosomeMap
Imports SMRUCC.genomics.Visualize.ChromosomeMap.Configuration
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components

''' <summary>
''' chromosome map visualize for bacterial genome 
''' </summary>
<Package("chromosome_map", Category:=APICategories.ResearchTools)>
Public Module chromosome_map

    ''' <summary>
    ''' load configuration file or create default configuration.
    ''' </summary>
    ''' <param name="conf"></param>
    ''' <returns></returns>
    <ExportAPI("config")>
    Public Function config(Optional conf As String = Nothing) As Config
        If conf.FileExists Then
            Return ChromesomeMapAPI.LoadConfig(conf)
        Else
            Return ChromosomeMap.GetDefaultConfiguration(conf)
        End If
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="genome"></param>
    ''' <param name="config"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("draw")>
    Public Function draw(genome As Object, Optional config As Object = Nothing, Optional env As Environment = Nothing) As Object
        If config Is Nothing Then
            config = ChromosomeMap.GetDefaultConfiguration(App.GetTempFile)
        ElseIf TypeOf config Is String Then
            config = ChromesomeMapAPI.LoadConfig(config)
        ElseIf Not TypeOf config Is Config Then
            Return Internal.debug.stop(Message.InCompatibleType(GetType(Config), config.GetType, env), env)
        End If

        If genome Is Nothing Then
            Return Internal.debug.stop("the plot data for target genome can not be nothing!", env)
        End If

        Throw New NotImplementedException
    End Function
End Module
