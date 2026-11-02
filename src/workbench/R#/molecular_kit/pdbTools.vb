Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.RCSB.PDB
Imports SMRUCC.genomics.Data.RCSB.PDB.Keywords
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("pdb")>
Public Module pdbTools

    Sub Main()

    End Sub

    ''' <summary>
    ''' parse the pdb struct data from a given document text data
    ''' </summary>
    ''' <param name="pdb_txt">
    ''' the PDB document text data for parse as the protein structure object 
    ''' model.
    ''' </param>
    ''' <param name="safe">
    ''' catch and ignore the exception when the parsing is failed? if this 
    ''' parameter is TRUE, then a NULL value will be returned instead of 
    ''' throwing an exception when the given document text data is invalid.
    ''' </param>
    ''' <param name="verbose">
    ''' print the verbose log message of the parsing progress?
    ''' </param>
    ''' <returns>
    ''' a <see cref="PDB"/> protein structure object model that parsed from 
    ''' the given PDB document text data; NULL will be returned when the 
    ''' parsing is failed and the ``safe`` parameter is TRUE.
    ''' </returns>
    <ExportAPI("parse_pdb")>
    <RApiReturn(GetType(PDB))>
    Public Function parsePdb(pdb_txt As String, Optional safe As Boolean = False, Optional verbose As Boolean = False) As Object
        If safe Then
            Try
                Return PDB.Parse(pdb_txt, verbose)
            Catch ex As Exception
                Call App.LogException(ex)
                Call ex.Message.warning

                Return Nothing
            End Try
        Else
            Return PDB.Parse(pdb_txt, verbose)
        End If
    End Function

    ''' <summary>
    ''' Reads a Protein Data Bank (PDB) file and parses it into a PDB object model.
    ''' </summary>
    ''' <param name="file">A file path string or Stream object representing the PDB file to read.</param>
    ''' <param name="env">The R runtime environment for error handling and resource management.</param>
    ''' <returns>Returns a parsed <see cref="PDB"/> object if successful. Returns a <see cref="Message"/> 
    ''' error object if file loading fails due to invalid path or format issues.</returns>
    ''' <example>
    ''' let pdb = read.pdb("1abc.pdb");
    ''' </example>
    <ExportAPI("read.pdb")>
    <RApiReturn(GetType(PDB))>
    Public Function readPdb(<RRawVectorArgument>
                            file As Object,
                            Optional safe As Boolean = False,
                            Optional env As Environment = Nothing) As Object

        Dim is_path As Boolean = False
        Dim s = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env, is_filepath:=is_path)

        If s Like GetType(Message) Then
            Return s.TryCast(Of Message)
        End If

        Dim pdb As PDB() = Nothing

        If safe Then
            Try
                pdb = RCSB.PDB.PDB.Load(s.TryCast(Of Stream)).ToArray
            Catch ex As Exception
                Call ex.Message.warning
                Call App.LogException(ex)
            End Try
        Else
            pdb = RCSB.PDB.PDB.Load(s.TryCast(Of Stream)).ToArray
        End If
        If is_path Then
            Call s.TryCast(Of Stream).Dispose()
        End If

        Return pdb
    End Function

    ''' <summary>
    ''' get structure models inside the given pdb object
    ''' </summary>
    ''' <param name="pdb">
    ''' a <see cref="PDB"/> protein structure object for get its structure 
    ''' models, which is created by the ``parse_pdb`` or ``read.pdb`` api.
    ''' </param>
    ''' <returns>
    ''' a vector of the structure model data that inside the given pdb object, 
    ''' each structure model element is a collection of the 
    ''' <see cref="Atom"/> atom object.
    ''' </returns>
    <ExportAPI("pdb_models")>
    <RApiReturn(GetType(Atom))>
    Public Function pdbModels(pdb As PDB) As Object
        Return pdb.AsEnumerable.ToArray
    End Function

    ''' <summary>
    ''' get the geometry center coordinates of the given protein structure 
    ''' object
    ''' </summary>
    ''' <param name="pdb">
    ''' a <see cref="PDB"/> protein structure object for evaluate its centroid 
    ''' coordinates.
    ''' </param>
    ''' <param name="as_vector">
    ''' returns the centroid coordinates as a numeric vector? if this 
    ''' parameter is FALSE(the default value), then a <see cref="Point3D"/> 
    ''' coordinates object will be returned.
    ''' </param>
    ''' <returns>
    ''' the centroid coordinates of the given protein structure object.
    ''' </returns>
    <ExportAPI("pdb_centroid")>
    <RApiReturn(GetType(Point3D), GetType(Double))>
    Public Function pdb_centroid(pdb As PDB, Optional as_vector As Boolean = False) As Object
        If as_vector Then
            With pdb.ModelCentroid
                Return New Double() { .X, .Y, .Z}
            End With
        Else
            Return pdb.ModelCentroid
        End If
    End Function

    ''' <summary>
    ''' list the small molecule ligand compound data from the given protein 
    ''' structure object
    ''' </summary>
    ''' <param name="pdb">
    ''' a <see cref="PDB"/> protein structure object for list its ligand 
    ''' compound data.
    ''' </param>
    ''' <param name="key">
    ''' the compound name for filter the ligand data.
    ''' </param>
    ''' <param name="number">
    ''' the sequence number of the target ligand for filter the ligand data, a 
    ''' negative value means no filter.
    ''' </param>
    ''' <returns>
    ''' a vector of the <see cref="Het.HETRecord"/> ligand compound object 
    ''' when no filter condition is specified, or the single ligand compound 
    ''' object that matches the given compound name and sequence number.
    ''' </returns>
    <ExportAPI("ligands")>
    <RApiReturn(GetType(Het.HETRecord))>
    Public Function ligands(pdb As PDB, Optional key As String = Nothing, Optional number As Integer = -1) As Object
        If key.StringEmpty(, True) OrElse number <= 0 Then
            ' get all 
            Return pdb.ListLigands.Values.ToArray
        Else
            Return pdb.ListLigands _
                .Where(Function(li)
                           Return li.Name = key AndAlso
                               li.Value.SequenceNumber = number
                       End Function) _
                .FirstOrDefault
        End If
    End Function
End Module
