Imports LANS.SystemsBiology.Assembly.SBML.Specifics.MetaCyc

Public NotInheritable Class Replacer

    Protected Friend Sub New()
        Throw New NotImplementedException
    End Sub

    Public Shared Function ApplyReplacements(Of T_REF As LANS.SystemsBiology.ComponentModel.EquaionModel.ICompoundSpecies,
                                                TModel As LANS.SystemsBiology.Assembly.SBML.FLuxBalanceModel.I_FBAC2(Of T_REF))(
                Model As TModel, StringList As IEnumerable(Of Escaping)) As Integer

        Dim n = From Metabolite In Model.Metabolites Select Metabolite.Replace2(StringList) '
        Dim m = From flux In Model.MetabolismNetwork Select flux.Replace(StringList) '
        Return n.ToArray.Count + m.ToArray.Count
    End Function

    Public Overrides Function ToString() As String
        Throw New NotImplementedException
    End Function
End Class
