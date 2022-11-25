#Region "Microsoft.VisualBasic::6ce7848b5c1dc1cf3fd5bb5f62a1e4b5, Bio.Repository\KEGG\Extensions.vb"

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

' Module Extensions
' 
'     Function: GetCompoundNames
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

<HideModuleName>
Public Module Extensions

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetCompoundNames(repository As String) As Dictionary(Of String, String)
        Return CompoundRepository.ScanRepository(repository, False) _
            .GroupBy(Function(c) c.entry) _
            .ToDictionary(Function(cpd) cpd.Key,
                          Function(cpd)
                              Return cpd.First _
                                  .commonNames _
                                  .FirstOrDefault Or cpd.Key.AsDefault
                          End Function)
    End Function

    ''' <summary>
    ''' read kegg pathway maps from a HDS cache database file
    ''' </summary>
    ''' <param name="buffer"></param>
    ''' <returns></returns>
    Public Function ReadKeggMaps(buffer As Stream) As Pathway()
        Using reader As New StreamPack(buffer, [readonly]:=True)
            Dim pathways As StreamGroup = reader.GetObject("/pathways/")
            Dim modelFiles = pathways _
                .ListFiles _
                .Where(Function(f) TypeOf f Is StreamBlock) _
                .Select(Function(f) DirectCast(f, StreamBlock)) _
                .ToArray

            Return modelFiles _
                .Select(Function(f) New StreamReader(reader.OpenBlock(f)).ReadToEnd) _
                .Select(Function(xml) DirectCast(xml.LoadFromXml(GetType(Pathway)), Pathway)) _
                .ToArray
        End Using
    End Function
End Module
