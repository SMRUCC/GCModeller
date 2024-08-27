﻿#Region "Microsoft.VisualBasic::fbeade0794c19db1c48104dc12207054, engine\vcellkit\Modeller\Sabiork.vb"

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

'   Total Lines: 37
'    Code Lines: 31 (83.78%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 6 (16.22%)
'     File Size: 1.36 KB


' Module sabiork_repository
' 
'     Function: createNewRepository, getKineticis, openRepository, parseSbml, query
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.SABIORK.docuRESTfulWeb
Imports SMRUCC.genomics.Data.SABIORK.SBML

''' <summary>
''' sabio-rk data repository
''' </summary>
<Package("sabiork")>
Public Module sabiork_repository

    <ExportAPI("new")>
    Public Function createNewRepository(file As String) As SabiorkRepository
        Return New SabiorkRepository(file.Open(FileMode.OpenOrCreate, doClear:=True))
    End Function

    <ExportAPI("open")>
    Public Function openRepository(file As String) As SabiorkRepository
        Return New SabiorkRepository(file.Open(FileMode.OpenOrCreate, doClear:=False))
    End Function

    <ExportAPI("query")>
    Public Function query(ec_number As String, cache As SabiorkRepository) As Object
        Return cache.GetByECNumber(ec_number)
    End Function

    <ExportAPI("get_kineticis")>
    Public Function getKineticis(cache As SabiorkRepository, ec_number As String) As Object
        Return cache.GetKineticisLaw(ec_number).ToArray
    End Function

    <ExportAPI("parseSbml")>
    Public Function parseSbml(data As String) As SbmlDocument
        Dim xml As String = data.LineIterators.JoinBy(vbLf)
        Dim model As SbmlDocument = ModelQuery.parseSBML(xml, schema:=GetType(SbmlDocument))
        Return model
    End Function

    ''' <summary>
    ''' Create a helper reader for load element model from the sbml document
    ''' </summary>
    ''' <param name="sbml"></param>
    ''' <returns></returns>
    <ExportAPI("sbmlReader")>
    Public Function documentReader(sbml As SbmlDocument) As SBMLInternalIndexer
        Return New SBMLInternalIndexer(sbml)
    End Function
End Module
