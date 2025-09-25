#Region "Microsoft.VisualBasic::0a1b10941c61423b7728d82eabb8df34, data\RCSB PDB\PDB\Parser.vb"

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

    '   Total Lines: 178
    '    Code Lines: 132 (74.16%)
    ' Comment Lines: 8 (4.49%)
    '    - Xml Docs: 62.50%
    ' 
    '   Blank Lines: 38 (21.35%)
    '     File Size: 7.54 KB


    ' Class Parser
    ' 
    '     Function: Load, ReadLine
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Data.RCSB.PDB.Keywords

Friend Class Parser

    Dim last As Keyword
    Dim model As Atom = Nothing
    Dim modelId As String = Nothing
    Dim lines As New List(Of String)

    ''' <summary>
    ''' Load multiple molecule pdb file
    ''' </summary>
    ''' <param name="s"></param>
    ''' <returns></returns>
    Public Shared Iterator Function Load(s As Stream, Optional verbose As Boolean = False) As IEnumerable(Of PDB)
        Dim pdb As New PDB
        Dim reader As New Parser

        For Each line As String In s.ReadAllLines
            If pdb Is Nothing Then
                pdb = New PDB
            End If

            If reader.ReadLine(pdb, line, verbose) Then
                If Not reader.last Is Nothing Then
                    Call reader.last.Flush()
                End If

                pdb.SourceText = reader.lines _
                    .PopAll _
                    .JoinBy(vbCrLf)
                reader.FlushModel(pdb)

                Yield pdb

                pdb = Nothing
            End If
        Next

        If Not pdb Is Nothing Then
            If Not reader.last Is Nothing Then
                Call reader.last.Flush()
            End If

            reader.FlushModel(pdb)
            pdb.SourceText = reader.lines _
                .PopAll _
                .JoinBy(vbCrLf)

            Yield pdb
        End If
    End Function

    Private Function ReadLine(ByRef pdb As PDB, line As String, verbose As Boolean) As Boolean
        Dim data = line.GetTagValue(trim:=False, failureNoName:=False)

        Call lines.Add(line)

        If data.Name.IsPattern("HETATM\d+") Then
            data = New NamedValue(Of String)("HETATM", line.Substring(6))
        ElseIf data.Name.IsPattern("CONECT\d+") Then
            data = New NamedValue(Of String)("CONECT", line.Substring(6))
        ElseIf data.Name.IsPattern("ANISOU\d+") Then
            data = New NamedValue(Of String)("ANISOU", line.Substring(6))
        End If

        If Not last Is Nothing Then
            If data.Name <> last.Keyword Then
                last.Flush()
                last = Nothing
            End If
        End If

        Select Case data.Name
            Case Keyword.KEYWORD_HEADER : pdb.Header = Header.Parse(data.Value)
            Case Keyword.KEYWORD_TITLE : pdb.Title = Title.Append(last, data.Value)
            Case Keyword.KEYWORD_COMPND : pdb.Compound = Compound.Append(last, data.Value)
            Case Keyword.KEYWORD_SOURCE : pdb.Source = Source.Append(last, data.Value)
            Case Keyword.KEYWORD_KEYWDS : pdb.Keywords = RCSB.PDB.Keywords.Keywords.Parse(data.Value)
            Case Keyword.KEYWORD_EXPDTA : pdb.Experiment = ExperimentData.Parse(data.Value)
            Case Keyword.KEYWORD_AUTHOR : pdb.Author = Author.Parse(data.Value)
            Case Keyword.KEYWORD_REVDAT : pdb.Revisions = Revision.Append(last, data.Value)
            Case Keyword.KEYWORD_JRNL : pdb.Journal = Journal.Append(last, data.Value)
            Case Keyword.KEYWORD_REMARK : pdb.Remark = Remark.Append(last, data.Value)
            Case Keyword.KEYWORD_DBREF, "DBREF1", "DBREF2" : pdb.DbRef = DbReference.Append(last, data.Value)
            Case Keyword.KEYWORD_SEQRES : pdb.Sequence = Sequence.Append(last, data.Value)
            Case Keyword.KEYWORD_CRYST1 : pdb.crystal1 = CRYST1.Append(last, data.Value)

            Case "ORIGX1" : pdb.Origin1 = Spatial3D.Parse(Of ORIGX123)(data.Value)
            Case "ORIGX2" : pdb.Origin2 = Spatial3D.Parse(Of ORIGX123)(data.Value)
            Case "ORIGX3" : pdb.Origin3 = Spatial3D.Parse(Of ORIGX123)(data.Value)

            Case "SCALE1" : pdb.Scale1 = Spatial3D.Parse(Of SCALE123)(data.Value)
            Case "SCALE2" : pdb.Scale2 = Spatial3D.Parse(Of SCALE123)(data.Value)
            Case "SCALE3" : pdb.Scale3 = Spatial3D.Parse(Of SCALE123)(data.Value)

            Case "MTRIX1" : pdb.Matrix1 = Spatial3D.Parse(Of MTRIX123)(data.Value)
            Case "MTRIX2" : pdb.Matrix2 = Spatial3D.Parse(Of MTRIX123)(data.Value)
            Case "MTRIX3" : pdb.Matrix3 = Spatial3D.Parse(Of MTRIX123)(data.Value)

            Case "SSBOND" : pdb.SSBOND = SSBOND.Append(last, data.Value)
            Case "SPRSDE" : pdb.SPRSDE = SPRSDE.Append(last, data.Value)
            Case "CAVEAT" : pdb.CAVEAT = CAVEAT.Append(last, data.Value)
            Case "MDLTYP" : pdb.MDLTYP = MDLTYP.Append(last, data.Value)
            Case "ANISOU" : pdb.ANISOU = ANISOU.Append(last, data.Value)

            Case "SPLIT" : pdb.SPLIT = SPLIT.Append(last, data.Value)

            Case "SEQADV" : pdb.seqadv = SEQADV.Append(last, data.Value)
            Case "NUMMDL"

                pdb.NUMMDL = NUMMDL.Parse(last, data.Value)

                If verbose Then
                    Call VBDebugger.EchoLine($"Found {pdb.NUMMDL} structure models inside {pdb.Header.ToString}.")
                End If

            Case Keyword.KEYWORD_HET : pdb.Het = Het.Append(last, data.Value)
            Case Keyword.KEYWORD_HETNAM : pdb.HetName = HetName.Append(last, data.Value)
            Case "HETSYN" : pdb.HETSYN = HETSYN.Append(last, data.Value)

            Case Keyword.KEYWORD_FORMUL : pdb.Formula = Formula.Append(last, data.Value)
            Case "LINK" : pdb.Links = Link.Append(last, data.Value)

            Case "MODEL"

                modelId = data.Value

                If verbose Then
                    Call VBDebugger.EchoLine($"Parse structure model: {modelId}...")
                End If

            Case "ENDMDL"

                model.ModelId = modelId
                model.Flush()
                pdb._atomStructuresData.Add(modelId, model)
                model = Nothing
                modelId = Nothing

            Case Keyword.KEYWORD_ATOM
                model = Atom.Append(model, data.Value)
            Case "TER"
                ' chain/model terminator
                model = Atom.AppendTerminator(model, data.Value)
                model.Flush()

            Case Keyword.KEYWORD_MASTER : pdb.Master = Master.Parse(data.Value)
            Case Keyword.KEYWORD_SITE : pdb.Site = Site.Append(last, data.Value)
            Case "CISPEP" : pdb.CISPEP = CISPEP.Append(last, data.Value)
            Case Keyword.KEYWORD_HELIX : pdb.Helix = Helix.Append(last, data.Value)
            Case Keyword.KEYWORD_SHEET : pdb.Sheet = Sheet.Append(last, data.Value)

            Case Keyword.KEYWORD_CONECT : pdb.Conect = CONECT.Append(last, data.Value)
            Case Keyword.KEYWORD_HETATM

                model = HETATM.Append(model, data.Value)

            Case "MODRES" : pdb.MODRES = MODRES.Append(last, data.Value)
            Case "SIGATM" : pdb.SIGATM = SIGATM.Append(last, data.Value)
            Case "SIGUIJ" : pdb.SIGUIJ = SIGUIJ.Append(last, data.Value)

            Case "END"
                Return FlushModel(pdb)

                ' 20250924 ignores of the pdbqt specific tags
            Case "ROOT", "ENDROOT", "BRANCH", "ENDBRANCH", "TORSDOF"
                ' just do nothing at here
            Case Else
                Throw New NotImplementedException(data.Name)
        End Select

        Return False
    End Function

    Private Function FlushModel(pdb As PDB) As Boolean
        ' end of current protein/molecule structure data
        If pdb._atomStructuresData.IsNullOrEmpty Then
            If model IsNot Nothing Then
                ' contains only one structure model data
                ' inside current pdb object
                model.ModelId = "1"
                model.Flush()
                pdb._atomStructuresData.Add("1", model)
                model = Nothing
            Else
                Call $"RCSB pdb object '{pdb.Header}' has no structure model data!".warning
            End If
        ElseIf Not model Is Nothing Then
            ' contains only one structure model data
            ' inside current pdb object
            model.ModelId = pdb.AtomStructures.Count + 1
            model.Flush()
            pdb._atomStructuresData.Add(model.ModelId, model)
            model = Nothing
        End If

        Return True
    End Function
End Class
