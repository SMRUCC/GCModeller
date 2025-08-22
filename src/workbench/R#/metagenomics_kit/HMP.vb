#Region "Microsoft.VisualBasic::c06e290ad97ad28e21c1c822dc9b3b9b, R#\metagenomics_kit\HMP.vb"

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

'   Total Lines: 48
'    Code Lines: 32 (66.67%)
' Comment Lines: 11 (22.92%)
'    - Xml Docs: 90.91%
' 
'   Blank Lines: 5 (10.42%)
'     File Size: 1.78 KB


' Module HMP
' 
'     Function: fetch, readFileManifest
' 
' /********************************************************************************/

#End Region

Imports Darwinism.OSSUtil
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data.Repository.NIH.HMP
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' An internal ``HMP`` client for download data files from ``https://portal.hmpdacc.org/`` website
''' </summary>
''' 
<Package("HMP_portal")>
Module HMP

    ''' <summary>
    ''' run file downloads
    ''' </summary>
    ''' <param name="files"></param>
    ''' <param name="outputdir"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("fetch")>
    Public Function fetch(<RRawVectorArgument> files As Object, outputdir As String,
                          Optional aspera As Aspera = Nothing,
                          Optional env As Environment = Nothing) As Object

        Dim filesManifest As pipeline = pipeline.TryCreatePipeline(Of manifest)(files, env)

        If filesManifest.isError Then
            Return filesManifest.getError
        End If

        Return filesManifest _
            .populates(Of manifest)(env) _
            .HandleFileDownloads(save:=outputdir, aspera) _
            .ToArray
    End Function

    <ExportAPI("read.manifest")>
    <RApiReturn(GetType(manifest))>
    Public Function readFileManifest(file As String, Optional env As Environment = Nothing) As Object
        If file Is Nothing Then
            Return RInternal.debug.stop("the required file path can not be nothing!", env)
        ElseIf file.FileExists Then
            Return manifest.LoadTable(file).ToArray
        Else
            Return RInternal.debug.stop({"the given file is not exists!", "path: " & file}, env)
        End If
    End Function
End Module
