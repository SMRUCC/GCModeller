Imports SMRUCC.genomics.Assembly.Expasy.AnnotationsTool
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular.Compiler
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular.FileStream.IO
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions

Public Class FullFillModel

    Public Property ModelLoader As XmlresxLoader
    Dim _MetaCyc As DatabaseLoadder
    Dim _KEGGCompounds As bGetObject.Compound()
    Dim _KeggReactions As bGetObject.Reaction()

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="CellSystemXml"><see cref="FileStream.XmlFormat.CellSystemXmlModel"></see></param>
    ''' <remarks></remarks>
    Sub New(CellSystemXml As String, MetaCyc As String)
        Me._ModelLoader = New FileStream.IO.XmlresxLoader(CellSystemXml)
        Me._MetaCyc = DatabaseLoadder.CreateInstance(MetaCyc, False)
    End Sub

    Public Sub FullFillModel_Kegg(KEGGCompoundsCsv As String, KEGGReactionsCsv As String, CARMENCsv As String)
        Call New Compiler.Components.MergeKEGGCompounds(_ModelLoader, KEGGCompoundsCsv).InvokeMergeCompoundSpecies()
        Call New Compiler.Components.MergeKEGGReactions(_ModelLoader, KEGGReactionsCsv, KEGGCompoundsCsv, CARMENCsv).InvokeMethods()
        _KEGGCompounds = KEGGCompoundsCsv.LoadCsv(Of bGetObject.Compound)(False).ToArray
        _KeggReactions = KEGGReactionsCsv.LoadCsv(Of bGetObject.Reaction)(False).ToArray
    End Sub

    Public Sub FullFillModel_Sabiork(SabiorkCompoundsCsv As String, SabiorkKineticsCsv As String, EnzymeModifyKinetics As String, Expasy As T_EnzymeClass_BLAST_OUT())
        Call New Compiler.Components.MergeSabiork(_ModelLoader, SabiorkCompoundsCsv).InvokeMergeCompoundSpecies()
        Call New Compiler.Components.SabiorkKinetics(_ModelLoader, SabiorkKineticsCsv, EnzymeModifyKinetics, Expasy).InvokeMethod(MetaCyc:=_MetaCyc, KEGGCompounds:=_KEGGCompounds, KEGGReactions:=_KeggReactions)
    End Sub

    Public Sub WriteData()
        Call _ModelLoader.SaveTo(_ModelLoader.CellSystemModel.FilePath)
    End Sub
End Class
