#Region "Microsoft.VisualBasic::c4c344af79f479d3055a1a01d63b2f82, R#\annotationKit\UniProt.vb"

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

'   Total Lines: 57
'    Code Lines: 44 (77.19%)
' Comment Lines: 6 (10.53%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 7 (12.28%)
'     File Size: 2.06 KB


' Module UniProt
' 
'     Function: Addnumbers, ExtractFasta, OpenOrCreateEnzymeSequencePack
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.Uniprot.Web
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop
Imports protein = SMRUCC.genomics.Assembly.Uniprot.XML.entry

<Package("UniProt")>
Public Module UniProt

    <ExportAPI("ECnumber_pack")>
    <RApiReturn(GetType(ECNumberWriter), GetType(ECNumberReader))>
    Public Function OpenOrCreateEnzymeSequencePack(file As String, Optional create_new As Boolean = False) As Object
        If create_new Then
            Return New ECNumberWriter(file.Open(
                mode:=FileMode.OpenOrCreate,
                doClear:=True,
                [readOnly]:=False
            ))
        Else
            Return New ECNumberReader(file.Open(FileMode.Open, doClear:=False, [readOnly]:=True))
        End If
    End Function

    <ExportAPI("add_ecNumbers")>
    Public Function Addnumbers(pack As ECNumberWriter,
                               <RRawVectorArgument>
                               uniprot As Object,
                               Optional env As Environment = Nothing) As Object

        Dim source = getUniprotData(uniprot, env)

        If source Like GetType(Message) Then
            Return source.TryCast(Of Message)
        Else
            For Each protein As protein In source.TryCast(Of IEnumerable(Of protein))
                Call pack.AddProtein(protein)
            Next
        End If

        Return Nothing
    End Function

    ''' <summary>
    ''' extract fasta data from a HDS stream database
    ''' </summary>
    ''' <param name="pack"></param>
    ''' <param name="enzyme"></param>
    ''' <returns></returns>
    <ExportAPI("extract_fasta")>
    Public Function ExtractFasta(pack As ECNumberReader, Optional enzyme As Boolean = True) As FastaFile
        Return New FastaFile(pack.QueryFasta(enzymeQuery:=enzyme))
    End Function

    ''' <summary>
    ''' make query of the proteins from the uniprot rest api
    ''' </summary>
    ''' <param name="q"></param>
    ''' <returns></returns>
    <ExportAPI("rest_query")>
    Public Function uniprot_query(q As String, Optional tax_id As UInteger? = Nothing) As RestQueryResult()
        Return WebServices.CreateQuery(q, tax_id)
    End Function
End Module
