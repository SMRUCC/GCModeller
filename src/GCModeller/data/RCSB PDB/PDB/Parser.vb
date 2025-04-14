Imports SMRUCC.genomics.Data.RCSB.PDB.Keywords
Imports System.IO
Imports System.Runtime.CompilerServices

Module Parser

    ''' <summary>
    ''' Load multiple molecule pdb file
    ''' </summary>
    ''' <param name="s"></param>
    ''' <returns></returns>
    Public Iterator Function Load(s As Stream) As IEnumerable(Of PDB)
        Dim pdb As New PDB
        Dim last As Keyword = Nothing

        For Each line As String In s.ReadAllLines
            If pdb.ReadLine(line, last) Then
                If Not last Is Nothing Then
                    Call last.Flush()
                End If
                Yield pdb
                pdb = New PDB
            End If
        Next

        If Not last Is Nothing Then
            Call last.Flush()
        End If

        Yield pdb
    End Function

    <Extension>
    Private Function ReadLine(ByRef pdb As PDB, line As String, ByRef last As Keyword) As Boolean
        Dim data = line.GetTagValue(trim:=True, failureNoName:=False)
        Dim model As Atom = Nothing
        Dim modelId As String = Nothing

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
            Case Keyword.KEYWORD_DBREF : pdb.DbRef = DbReference.Append(last, data.Value)
            Case Keyword.KEYWORD_SEQRES : pdb.Sequence = Sequence.Append(last, data.Value)
            Case Keyword.KEYWORD_CRYST1 : pdb.crystal1 = CRYST1.Append(last, data.Value)

            Case "ORIGX1" : pdb.Origin1 = Spatial3D.Parse(Of ORIGX123)(data.Value)
            Case "ORIGX2" : pdb.Origin2 = Spatial3D.Parse(Of ORIGX123)(data.Value)
            Case "ORIGX3" : pdb.Origin3 = Spatial3D.Parse(Of ORIGX123)(data.Value)

            Case "SCALE1" : pdb.Scale1 = Spatial3D.Parse(Of SCALE123)(data.Value)
            Case "SCALE2" : pdb.Scale2 = Spatial3D.Parse(Of SCALE123)(data.Value)
            Case "SCALE3" : pdb.Scale3 = Spatial3D.Parse(Of SCALE123)(data.Value)

            Case "SEQADV" : pdb.seqadv = SEQADV.Append(last, data.Value)
            Case "NUMMDL" : pdb.NUMMDL = NUMMDL.Parse(last, data.Value)

            Case Keyword.KEYWORD_HET : pdb.Het = Het.Append(last, data.Value)

            Case "MODEL" : modelId = data.Value
            Case "ENDMDL"

                model.Flush()
                pdb._atomStructuresData.Add(modelId, model)
                model = Nothing
                modelId = Nothing

            Case Keyword.KEYWORD_ATOM : model = Atom.Append(last, data.Value)
            Case "TER"
                model = Atom.Append(model, data.Value)
                model.Flush()

            Case Keyword.KEYWORD_MASTER : pdb.Master = Master.Parse(data.Value)

            Case Keyword.KEYWORD_HELIX : pdb.Helix = Helix.Append(last, data.Value)
            Case Keyword.KEYWORD_SHEET : pdb.Sheet = Sheet.Append(last, data.Value)

            Case "END"
                ' end of current protein/molecule structure data
                If pdb._atomStructuresData.IsNullOrEmpty Then
                    ' contains only one structure model data inside current pdb object
                    pdb._atomStructuresData.Add("1", model)
                End If

                Return True

            Case Else
                Throw New NotImplementedException(data.Name)
        End Select

        Return False
    End Function
End Module
